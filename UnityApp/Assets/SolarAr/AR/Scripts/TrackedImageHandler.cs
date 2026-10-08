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

            if (_planetRegistry == null)
            {
                if (!_missingRegistryReported)
                {
                    Debug.LogError("[ImageTracking] PlanetRegistry is not assigned.", this);
                    _missingRegistryReported = true;
                }

                return;
            }

            if (!_planetRegistry.TryGetPrefab(referenceImageName, out var planetPrefab))
            {
                if (_unconfiguredImages.Add(referenceImageName))
                {
                    Debug.LogWarning(
                        $"[ImageTracking] No planet prefab is configured for reference image '{referenceImageName}'.",
                        this);
                }

                return;
            }

            if (!_trackedPlanets.TryGetValue(referenceImageName, out var trackedPlanet) ||
                trackedPlanet.Instance == null)
            {
                var instance = Instantiate(planetPrefab, trackedImage.transform);
                trackedPlanet = new TrackedPlanet(instance, trackedImage.trackableId);
                _trackedPlanets[referenceImageName] = trackedPlanet;
            }
            else
            {
                trackedPlanet.TrackableId = trackedImage.trackableId;

                if (trackedPlanet.Instance.transform.parent != trackedImage.transform)
                {
                    trackedPlanet.Instance.transform.SetParent(trackedImage.transform, false);
                }
            }

            trackedPlanet.Instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            trackedPlanet.Instance.SetActive(trackedImage.trackingState == TrackingState.Tracking);
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
        }
    }
}
