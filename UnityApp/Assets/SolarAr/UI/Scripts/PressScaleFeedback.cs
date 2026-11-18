using UnityEngine;
using UnityEngine.EventSystems;

namespace SolarAR.UI
{
    public sealed class PressScaleFeedback : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerExitHandler
    {
        [SerializeField, Range(0.9f, 1f)] private float _pressedScale = 0.975f;
        [SerializeField, Min(1f)] private float _response = 18f;

        private RectTransform _rectTransform;
        private bool _isPressed;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
        }

        private void OnDisable()
        {
            _isPressed = false;
            if (_rectTransform != null)
            {
                _rectTransform.localScale = Vector3.one;
            }
        }

        private void Update()
        {
            if (_rectTransform == null)
            {
                return;
            }

            float target = _isPressed ? _pressedScale : 1f;
            float factor = 1f - Mathf.Exp(-_response * Time.unscaledDeltaTime);
            _rectTransform.localScale = Vector3.Lerp(
                _rectTransform.localScale,
                Vector3.one * target,
                factor);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPressed = false;
        }
    }
}
