using System.Collections.Generic;
using SolarAR.Planets;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SolarAR.AR
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public sealed class TrackedImageHandler : MonoBehaviour
    {
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
        private ARTrackedImageManager _trackedImageManager;
        private bool _missingRegistryReported;

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
            foreach (var trackedImage in eventArgs.added)
            {
                UpdatePlanet(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
            {
                UpdatePlanet(trackedImage);
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
                    $"[ImageTracking] Handler processing {referenceImageName} - {trackingState}",
                    this);
                _reportedTrackingStates[referenceImageName] = trackingState;
            }

            if (trackingState == TrackingState.None)
            {
                HidePlanet(referenceImageName, trackedImage.trackableId);
                return;
            }

            var hasInstance = _trackedPlanets.TryGetValue(referenceImageName, out var trackedPlanet) &&
                              trackedPlanet.Instance != null;

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
                Debug.Log($"[ImageTracking] Registry lookup for {referenceImageName}", this);

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

                Debug.Log(
                    $"[ImageTracking] Prefab found for {referenceImageName}: {planetPrefab.name}",
                    this);
                Debug.Log($"[ImageTracking] Instantiating {planetPrefab.name}", this);

                var instance = Instantiate(
                    planetPrefab,
                    trackedImage.transform.position,
                    trackedImage.transform.rotation);
                trackedPlanet = new TrackedPlanet(instance, trackedImage.trackableId);
                _trackedPlanets[referenceImageName] = trackedPlanet;

                Debug.Log($"[ImageTracking] Created instance: {instance.name}", instance);
            }
            else
            {
                trackedPlanet.TrackableId = trackedImage.trackableId;
            }

            if (trackingState == TrackingState.Tracking)
            {
                trackedPlanet.Instance.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation);
            }

            var wasActive = trackedPlanet.Instance.activeSelf;
            trackedPlanet.Instance.SetActive(true);

            if (!hasInstance || !wasActive)
            {
                Debug.Log($"[ImageTracking] Instance active: {trackedPlanet.Instance.name}", this);
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
            Debug.Log($"[ImageTracking] Hidden instance: {trackedPlanet.Instance.name}", this);
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
        }
    }
}
