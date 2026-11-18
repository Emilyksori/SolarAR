using System;
using System.Collections;
using System.Collections.Generic;
using SolarAR.Interaction;
using SolarAR.Planets;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SolarAR.AR
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public sealed class TrackedImageHandler : MonoBehaviour
    {
        public sealed class ActiveImageState
        {
            public string ReferenceImageName { get; private set; }
            public TrackableId TrackableId { get; private set; }

            public bool Activate(string referenceImageName, TrackableId trackableId)
            {
                if (string.IsNullOrWhiteSpace(referenceImageName))
                {
                    return false;
                }

                bool changed = !string.Equals(
                    ReferenceImageName,
                    referenceImageName,
                    StringComparison.Ordinal) || TrackableId != trackableId;
                ReferenceImageName = referenceImageName;
                TrackableId = trackableId;
                return changed;
            }

            public bool ClearIfActive(string referenceImageName, TrackableId trackableId)
            {
                if (!string.Equals(
                        ReferenceImageName,
                        referenceImageName,
                        StringComparison.Ordinal) || TrackableId != trackableId)
                {
                    return false;
                }

                ReferenceImageName = null;
                TrackableId = default;
                return true;
            }

            public void Clear()
            {
                ReferenceImageName = null;
                TrackableId = default;
            }
        }

        public enum TrackedImageStatus
        {
            Searching,
            Tracking,
            Limited
        }

        private sealed class TrackedPlanet
        {
            public TrackedPlanet(GameObject instance, TrackableId trackableId)
            {
                Instance = instance;
                TrackableId = trackableId;
            }

            public GameObject Instance { get; }
            public TrackableId TrackableId { get; set; }
        }

        [SerializeField] private PlanetRegistry _planetRegistry;
        [SerializeField] private bool _prewarmPlanetInstances = true;
        [SerializeField] private bool _enablePerformanceDiagnostics = true;

        private readonly Dictionary<string, TrackedPlanet> _trackedPlanets = new();
        private readonly Dictionary<string, TrackingState> _reportedTrackingStates = new();
        private readonly HashSet<string> _unconfiguredImages = new();
        private readonly ActiveImageState _activeImageState = new();
        private ARTrackedImageManager _trackedImageManager;
        private bool _missingRegistryReported;
        private string _lastPublishedReferenceImageName;

        public event Action<string, TrackedImageStatus> TrackingStatusChanged;

        public TrackedImageStatus CurrentStatus { get; private set; } =
            TrackedImageStatus.Searching;
        public string ActiveReferenceImageName => _activeImageState.ReferenceImageName;

        public void SetActivePlanetPresentationScale(float multiplier)
        {
            if (string.IsNullOrEmpty(ActiveReferenceImageName) ||
                !_trackedPlanets.TryGetValue(
                    ActiveReferenceImageName,
                    out TrackedPlanet trackedPlanet) ||
                trackedPlanet.Instance == null)
            {
                return;
            }

            EarthGestureController gestures =
                trackedPlanet.Instance.GetComponent<EarthGestureController>();
            if (gestures != null)
            {
                gestures.SetPresentationScale(multiplier);
            }
        }

        private void Awake()
        {
            _trackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            _trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
            if (_prewarmPlanetInstances)
            {
                StartCoroutine(PrewarmPlanetInstances());
            }
        }

        private void OnDisable()
        {
            _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
            StopAllCoroutines();

            foreach (var trackedPlanet in _trackedPlanets.Values)
            {
                if (trackedPlanet.Instance != null)
                {
                    trackedPlanet.Instance.SetActive(false);
                }
            }

            _unconfiguredImages.Clear();
            _reportedTrackingStates.Clear();
            _missingRegistryReported = false;
            _activeImageState.Clear();
            _lastPublishedReferenceImageName = null;
            CurrentStatus = TrackedImageStatus.Searching;
        }

        private void OnDestroy()
        {
            foreach (var trackedPlanet in _trackedPlanets.Values)
            {
                if (trackedPlanet.Instance != null)
                {
                    Destroy(trackedPlanet.Instance);
                }
            }

            _trackedPlanets.Clear();
        }

        private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
        {
            double eventReceivedAt = Time.realtimeSinceStartupAsDouble;

            // Release stale/limited images first. Tracking updates are processed afterwards,
            // and newly added images last, so a new valid card wins within the same event.
            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState != TrackingState.Tracking)
                {
                    UpdatePlanet(trackedImage, "updated", eventReceivedAt);
                }
            }

            foreach (var trackedImage in eventArgs.added)
            {
                if (trackedImage.trackingState != TrackingState.Tracking)
                {
                    UpdatePlanet(trackedImage, "added", eventReceivedAt);
                }
            }

            ARTrackedImage trackingWinner = null;
            string winnerEventKind = null;
            bool winnerJustReachedTracking = false;
            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState != TrackingState.Tracking)
                {
                    continue;
                }

                bool isCurrent = string.Equals(
                    trackedImage.referenceImage.name,
                    ActiveReferenceImageName,
                    StringComparison.Ordinal);
                bool justReachedTracking =
                    !_reportedTrackingStates.TryGetValue(
                        trackedImage.referenceImage.name,
                        out TrackingState previousState) ||
                    previousState != TrackingState.Tracking;
                if (trackingWinner == null ||
                    (justReachedTracking && !winnerJustReachedTracking) ||
                    (!winnerJustReachedTracking && isCurrent))
                {
                    trackingWinner = trackedImage;
                    winnerEventKind = "updated";
                    winnerJustReachedTracking = justReachedTracking;
                }
            }

            // A newly acquired Tracking image intentionally wins over an older image.
            // On later frames the current winner remains stable while both cards are visible.
            foreach (var trackedImage in eventArgs.added)
            {
                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    trackingWinner = trackedImage;
                    winnerEventKind = "added";
                }
            }

            if (trackingWinner != null)
            {
                UpdatePlanet(trackingWinner, winnerEventKind, eventReceivedAt);
            }

            // Remember every observed Tracking transition, including cards that were not
            // selected for presentation, so two visible cards do not alternate each frame.
            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    _reportedTrackingStates[trackedImage.referenceImage.name] =
                        TrackingState.Tracking;
                }
            }

            foreach (var trackedImage in eventArgs.added)
            {
                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    _reportedTrackingStates[trackedImage.referenceImage.name] =
                        TrackingState.Tracking;
                }
            }

            foreach (var removedImage in eventArgs.removed)
            {
                RemovePlanet(removedImage.Value.referenceImage.name, removedImage.Key);
            }
        }

        private void UpdatePlanet(
            ARTrackedImage trackedImage,
            string eventKind,
            double eventReceivedAt)
        {
            double handlerStartedAt = Time.realtimeSinceStartupAsDouble;
            var referenceImageName = trackedImage.referenceImage.name;
            var trackingState = trackedImage.trackingState;

            if (!_reportedTrackingStates.TryGetValue(referenceImageName, out var reportedState) ||
                reportedState != trackingState)
            {
                Debug.Log(
                    $"[Tracking] {referenceImageName} -> {trackingState}",
                    this);
                _reportedTrackingStates[referenceImageName] = trackingState;
            }

            if (trackingState == TrackingState.None)
            {
                HidePlanet(referenceImageName, trackedImage.trackableId);
                if (_activeImageState.ClearIfActive(
                        referenceImageName,
                        trackedImage.trackableId))
                {
                    LogActivePlanetCleared(referenceImageName);
                    PublishStatus(referenceImageName, TrackedImageStatus.Searching);
                }
                else if (string.IsNullOrEmpty(ActiveReferenceImageName) &&
                         CurrentStatus == TrackedImageStatus.Limited &&
                         string.Equals(
                             _lastPublishedReferenceImageName,
                             referenceImageName,
                             StringComparison.Ordinal))
                {
                    PublishStatus(referenceImageName, TrackedImageStatus.Searching);
                }

                return;
            }

            bool activationRequested = trackingState == TrackingState.Tracking &&
                                       (!string.Equals(
                                            ActiveReferenceImageName,
                                            referenceImageName,
                                            StringComparison.Ordinal) ||
                                        _activeImageState.TrackableId != trackedImage.trackableId);
            bool measureActivation = activationRequested && PerformanceDiagnosticsEnabled;
            if (measureActivation)
            {
                Debug.Log(
                    $"[ARPerf] {referenceImageName} {eventKind}: AR Foundation callback " +
                    $"received at {eventReceivedAt:F6}s; handler started at " +
                    $"{handlerStartedAt:F6}s. This excludes time before the AR event.",
                    this);
            }

            double prefabRequestedAt = Time.realtimeSinceStartupAsDouble;
            bool hasInstance = _trackedPlanets.TryGetValue(
                                   referenceImageName,
                                   out var trackedPlanet) &&
                               trackedPlanet.Instance != null;

            if (trackingState == TrackingState.Limited)
            {
                if (hasInstance && trackedPlanet.TrackableId == trackedImage.trackableId)
                {
                    trackedPlanet.Instance.SetActive(false);
                }

                if (_activeImageState.ClearIfActive(
                        referenceImageName,
                        trackedImage.trackableId))
                {
                    LogActivePlanetCleared(referenceImageName);
                    PublishStatus(referenceImageName, TrackedImageStatus.Limited);
                }

                return;
            }

            if (!hasInstance && _planetRegistry == null)
            {
                if (!_missingRegistryReported)
                {
                    Debug.LogError("[ImageTracking] PlanetRegistry is not assigned.", this);
                    _missingRegistryReported = true;
                }

                return;
            }

            if (!hasInstance)
            {
                if (!_planetRegistry.TryGetPrefab(referenceImageName, out var planetPrefab))
                {
                    if (_unconfiguredImages.Add(referenceImageName))
                    {
                        Debug.LogWarning(
                            $"[ImageTracking] No prefab found for {referenceImageName}",
                            this);
                    }

                    return;
                }

                var instance = Instantiate(
                    planetPrefab,
                    trackedImage.transform.position,
                    trackedImage.transform.rotation);
                trackedPlanet = new TrackedPlanet(instance, trackedImage.trackableId);
                _trackedPlanets[referenceImageName] = trackedPlanet;

                Debug.Log(
                    $"[ImageTracking] Created {instance.name} for {referenceImageName}",
                    instance);
            }
            else
            {
                trackedPlanet.TrackableId = trackedImage.trackableId;
            }

            double prefabReadyAt = Time.realtimeSinceStartupAsDouble;

            trackedPlanet.Instance.transform.SetPositionAndRotation(
                trackedImage.transform.position,
                trackedImage.transform.rotation);

            DeactivateOtherPlanets(referenceImageName);
            string previousReferenceImageName = ActiveReferenceImageName;
            bool activePlanetChanged = _activeImageState.Activate(
                referenceImageName,
                trackedImage.trackableId);
            trackedPlanet.Instance.SetActive(true);
            double modelActiveAt = Time.realtimeSinceStartupAsDouble;
            if (activePlanetChanged)
            {
                if (!string.IsNullOrEmpty(previousReferenceImageName))
                {
                    LogActivePlanetCleared(previousReferenceImageName);
                }

                Debug.Log($"[ActivePlanet] {GetPlanetLogName(referenceImageName)}", this);
            }

            PublishStatus(referenceImageName, TrackedImageStatus.Tracking);
            if (measureActivation)
            {
                StartCoroutine(LogRenderReadyAfterFrame(
                    referenceImageName,
                    eventKind,
                    eventReceivedAt,
                    handlerStartedAt,
                    prefabRequestedAt,
                    prefabReadyAt,
                    modelActiveAt,
                    hasInstance,
                    trackedPlanet.Instance));
            }
        }

        private IEnumerator PrewarmPlanetInstances()
        {
            if (_planetRegistry == null)
            {
                yield break;
            }

            int warmedCount = 0;
            for (int index = 0; index < _planetRegistry.Count; index++)
            {
                if (!_planetRegistry.TryGetEntry(
                        index,
                        out string referenceImageName,
                        out GameObject planetPrefab) ||
                    _trackedPlanets.ContainsKey(referenceImageName))
                {
                    continue;
                }

                GameObject instance = Instantiate(planetPrefab, transform);
                instance.SetActive(false);
                _trackedPlanets[referenceImageName] = new TrackedPlanet(instance, default);
                warmedCount++;

                // Spread initialization to avoid a single startup frame spike on Android.
                yield return null;
            }

            if (PerformanceDiagnosticsEnabled)
            {
                Debug.Log(
                    $"[ARPerf] Planet pool ready: {warmedCount} new, " +
                    $"{_trackedPlanets.Count} cached instances.",
                    this);
            }
        }

        private IEnumerator LogRenderReadyAfterFrame(
            string referenceImageName,
            string eventKind,
            double eventReceivedAt,
            double handlerStartedAt,
            double prefabRequestedAt,
            double prefabReadyAt,
            double modelActiveAt,
            bool reusedInstance,
            GameObject instance)
        {
            yield return new WaitForEndOfFrame();

            double renderSampleAt = Time.realtimeSinceStartupAsDouble;
            bool hasActiveRenderer = false;
            if (instance != null && instance.activeInHierarchy)
            {
                foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>())
                {
                    if (renderer.enabled && renderer.gameObject.activeInHierarchy)
                    {
                        hasActiveRenderer = true;
                        break;
                    }
                }
            }

            Debug.Log(
                $"[ARPerf] {referenceImageName} {eventKind}: " +
                $"event→handler={ToMilliseconds(handlerStartedAt - eventReceivedAt):F2}ms, " +
                $"prefab acquire={ToMilliseconds(prefabReadyAt - prefabRequestedAt):F2}ms " +
                $"({(reusedInstance ? "pool" : "instantiate")}), " +
                $"event→active={ToMilliseconds(modelActiveAt - eventReceivedAt):F2}ms, " +
                $"event→end-of-frame={ToMilliseconds(renderSampleAt - eventReceivedAt):F2}ms, " +
                $"activeRenderer={hasActiveRenderer}. " +
                "Times start at the AR Foundation callback, not when the physical card entered view.",
                this);
        }

        private bool PerformanceDiagnosticsEnabled =>
            _enablePerformanceDiagnostics && Debug.isDebugBuild;

        private static double ToMilliseconds(double seconds)
        {
            return seconds * 1000d;
        }

        private void DeactivateOtherPlanets(string activeReferenceImageName)
        {
            foreach (var pair in _trackedPlanets)
            {
                if (pair.Key != activeReferenceImageName && pair.Value.Instance != null)
                {
                    pair.Value.Instance.SetActive(false);
                }
            }
        }

        private void HidePlanet(string referenceImageName, TrackableId trackableId)
        {
            if (!_trackedPlanets.TryGetValue(referenceImageName, out var trackedPlanet) ||
                trackedPlanet.TrackableId != trackableId ||
                trackedPlanet.Instance == null)
            {
                return;
            }

            trackedPlanet.Instance.SetActive(false);
        }

        private void RemovePlanet(string referenceImageName, TrackableId removedTrackableId)
        {
            if (!_trackedPlanets.TryGetValue(referenceImageName, out var trackedPlanet) ||
                trackedPlanet.TrackableId != removedTrackableId)
            {
                return;
            }

            trackedPlanet.Instance?.SetActive(false);
            trackedPlanet.TrackableId = default;
            _reportedTrackingStates.Remove(referenceImageName);
            if (_activeImageState.ClearIfActive(referenceImageName, removedTrackableId))
            {
                LogActivePlanetCleared(referenceImageName);
                PublishStatus(referenceImageName, TrackedImageStatus.Searching);
            }
        }

        private void LogActivePlanetCleared(string referenceImageName)
        {
            Debug.Log(
                $"[ActivePlanet] cleared ({GetPlanetLogName(referenceImageName)})",
                this);
        }

        private static string GetPlanetLogName(string referenceImageName)
        {
            const string cardSuffix = "Card";
            return referenceImageName.EndsWith(cardSuffix, StringComparison.Ordinal)
                ? referenceImageName[..^cardSuffix.Length]
                : referenceImageName;
        }

        private void PublishStatus(string referenceImageName, TrackedImageStatus status)
        {
            if (CurrentStatus == status &&
                string.Equals(
                    _lastPublishedReferenceImageName,
                    referenceImageName,
                    StringComparison.Ordinal))
            {
                return;
            }

            CurrentStatus = status;
            _lastPublishedReferenceImageName = referenceImageName;
            TrackingStatusChanged?.Invoke(referenceImageName, status);
        }
    }
}
