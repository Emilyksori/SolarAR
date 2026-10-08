using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SolarAR.AR
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public sealed class TrackedImageDebugLogger : MonoBehaviour
    {
        private readonly Dictionary<TrackableId, TrackingState> _lastTrackingStates = new();
        private ARTrackedImageManager _trackedImageManager;

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
            _lastTrackingStates.Clear();
        }

        private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                Debug.Log($"[ImageTracking] Added: {trackedImage.referenceImage.name}");
            }

            foreach (var trackedImage in eventArgs.updated)
            {
                if (_lastTrackingStates.TryGetValue(trackedImage.trackableId, out var previousState) &&
                    previousState == trackedImage.trackingState)
                {
                    continue;
                }

                _lastTrackingStates[trackedImage.trackableId] = trackedImage.trackingState;
                Debug.Log($"[ImageTracking] Updated: {trackedImage.referenceImage.name} - {trackedImage.trackingState}");
            }

            foreach (var removedImage in eventArgs.removed)
            {
                var trackedImage = removedImage.Value;
                Debug.Log($"[ImageTracking] Removed: {trackedImage.referenceImage.name}");
                _lastTrackingStates.Remove(removedImage.Key);
            }
        }
    }
}
