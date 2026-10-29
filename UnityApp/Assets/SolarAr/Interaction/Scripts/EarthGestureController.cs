using SolarAR.Planets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace SolarAR.Interaction
{
    public sealed class EarthGestureController : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private Transform _visualTarget;
        [SerializeField] private RotationController _rotationController;

        [Header("Rotation")]
        [SerializeField, Min(0f)] private float _dragSensitivity = 0.2f;

        [Header("Scale")]
        [SerializeField, Min(0.01f)] private float _minScaleMultiplier = 0.5f;
        [SerializeField, Min(0.01f)] private float _maxScaleMultiplier = 2f;
        [SerializeField, Min(0f)] private float _mouseWheelSensitivity = 0.001f;

        private Vector3 _initialVisualScale;
        private float _currentScaleMultiplier = 1f;
        private float _previousPinchDistance;
        private bool _pinchStarted;
        private bool _manualInteractionActive;
        private bool _resumeAutoRotation;

        private void Awake()
        {
            if (_visualTarget == null)
            {
                Debug.LogError("[Interaction] Earth visual target is not assigned.", this);
                enabled = false;
                return;
            }

            _initialVisualScale = _visualTarget.localScale;
            _resumeAutoRotation = _rotationController != null && _rotationController.AutoRotate;
            NormalizeScaleLimits();
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EndManualInteraction();
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            var touches = EnhancedTouch.activeTouches;
            EnhancedTouch firstTouch = default;
            EnhancedTouch secondTouch = default;
            int activeTouchCount = 0;

            foreach (EnhancedTouch touch in touches)
            {
                if (!touch.isInProgress)
                {
                    continue;
                }

                if (activeTouchCount == 0)
                {
                    firstTouch = touch;
                }
                else
                {
                    secondTouch = touch;
                }

                activeTouchCount++;
                if (activeTouchCount == 2)
                {
                    break;
                }
            }

            if (activeTouchCount >= 2)
            {
                HandlePinch(firstTouch.screenPosition, secondTouch.screenPosition);
                return;
            }

            _pinchStarted = false;

            if (activeTouchCount == 1)
            {
                HandleTouchDrag(firstTouch);
                return;
            }

            HandleMouse();
        }

        private void HandleTouchDrag(EnhancedTouch touch)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                BeginManualInteraction();
                return;
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                BeginManualInteraction();
                RotateVisual(touch.delta.x);
            }
        }

        private void HandlePinch(Vector2 firstPosition, Vector2 secondPosition)
        {
            float currentDistance = Vector2.Distance(firstPosition, secondPosition);

            if (!_pinchStarted || _previousPinchDistance <= Mathf.Epsilon)
            {
                _previousPinchDistance = currentDistance;
                _pinchStarted = true;
                BeginManualInteraction();
                return;
            }

            BeginManualInteraction();
            SetScaleMultiplier(_currentScaleMultiplier * (currentDistance / _previousPinchDistance));
            _previousPinchDistance = currentDistance;
        }

        private void HandleMouse()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                EndManualInteraction();
                return;
            }

            bool pointerOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

            if (mouse.leftButton.isPressed && !pointerOverUi)
            {
                BeginManualInteraction();
                RotateVisual(mouse.delta.ReadValue().x);
            }
            else if (!mouse.leftButton.isPressed)
            {
                EndManualInteraction();
            }

            float scrollDelta = mouse.scroll.ReadValue().y;
            if (!pointerOverUi && !Mathf.Approximately(scrollDelta, 0f))
            {
                SetScaleMultiplier(
                    _currentScaleMultiplier * (1f + scrollDelta * _mouseWheelSensitivity));
            }
        }

        private void RotateVisual(float horizontalDelta)
        {
            _visualTarget.Rotate(Vector3.up, -horizontalDelta * _dragSensitivity, Space.Self);
        }

        private void SetScaleMultiplier(float multiplier)
        {
            _currentScaleMultiplier = Mathf.Clamp(
                multiplier,
                _minScaleMultiplier,
                _maxScaleMultiplier);
            _visualTarget.localScale = _initialVisualScale * _currentScaleMultiplier;
        }

        private void BeginManualInteraction()
        {
            if (_manualInteractionActive)
            {
                return;
            }

            _manualInteractionActive = true;
            if (_rotationController != null)
            {
                _rotationController.SetAutoRotation(false);
            }
        }

        private void EndManualInteraction()
        {
            if (!_manualInteractionActive)
            {
                return;
            }

            _manualInteractionActive = false;
            if (_rotationController != null)
            {
                _rotationController.SetAutoRotation(_resumeAutoRotation);
            }
        }

        private void NormalizeScaleLimits()
        {
            _minScaleMultiplier = Mathf.Max(0.01f, _minScaleMultiplier);
            _maxScaleMultiplier = Mathf.Max(_minScaleMultiplier, _maxScaleMultiplier);
        }

        private void OnValidate()
        {
            NormalizeScaleLimits();
        }
    }
}
