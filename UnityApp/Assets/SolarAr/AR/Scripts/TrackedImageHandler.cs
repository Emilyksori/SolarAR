using System;
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
        }

        private void OnDisable()
        {
            _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);

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
            // Release stale/limited images first. Tracking updates are processed afterwards,
            // and newly added images last, so a new valid card wins within the same event.
            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState != TrackingState.Tracking)
                {
                    UpdatePlanet(trackedImage);
                }
            }

            foreach (var trackedImage in eventArgs.added)
            {
                if (trackedImage.trackingState != TrackingState.Tracking)
                {
                    UpdatePlanet(trackedImage);
                }
            }

            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    UpdatePlanet(trackedImage);
                }
            }

            foreach (var trackedImage in eventArgs.added)
            {
                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    UpdatePlanet(trackedImage);
                }
            }

            foreach (var removedImage in eventArgs.removed)
            {
                RemovePlanet(removedImage.Value.referenceImage.name, removedImage.Key);
            }
        }

        private void UpdatePlanet(ARTrackedImage trackedImage)
        {
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

            var hasInstance = _trackedPlanets.TryGetValue(referenceImageName, out var trackedPlanet) &&
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

            trackedPlanet.Instance.transform.SetPositionAndRotation(
                trackedImage.transform.position,
                trackedImage.transform.rotation);

            DeactivateOtherPlanets(referenceImageName);
            string previousReferenceImageName = ActiveReferenceImageName;
            bool activePlanetChanged = _activeImageState.Activate(
                referenceImageName,
                trackedImage.trackableId);
            trackedPlanet.Instance.SetActive(true);
            if (activePlanetChanged)
            {
                if (!string.IsNullOrEmpty(previousReferenceImageName))
                {
                    LogActivePlanetCleared(previousReferenceImageName);
                }

                Debug.Log($"[ActivePlanet] {GetPlanetLogName(referenceImageName)}", this);
            }

            PublishStatus(referenceImageName, TrackedImageStatus.Tracking);
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

            if (trackedPlanet.Instance != null)
            {
                Destroy(trackedPlanet.Instance);
            }

            _trackedPlanets.Remove(referenceImageName);
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
