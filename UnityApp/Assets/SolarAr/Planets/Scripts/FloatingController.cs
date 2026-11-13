using UnityEngine;

namespace SolarAR.Planets
{
    public sealed class FloatingController : MonoBehaviour
    {
        [SerializeField] private Transform _floatTarget;
        [SerializeField, Min(0f)] private float _amplitudeMeters = 0.003f;
        [SerializeField, Min(0f)] private float _cyclesPerSecond = 0.3f;

        private Vector3 _baseLocalPosition;

        private void Awake()
        {
            if (_floatTarget == null)
            {
                Debug.LogWarning("[Planets] Floating target is not assigned.", this);
                enabled = false;
                return;
            }

            _baseLocalPosition = _floatTarget.localPosition;
        }

        private void OnEnable()
        {
            if (_floatTarget != null)
            {
                _baseLocalPosition = _floatTarget.localPosition;
            }
        }

        private void LateUpdate()
        {
            float phase = Time.time * _cyclesPerSecond * Mathf.PI * 2f;
            _floatTarget.localPosition =
                _baseLocalPosition + Vector3.up * (Mathf.Sin(phase) * _amplitudeMeters);
        }

        private void OnDisable()
        {
            if (_floatTarget != null)
            {
                _floatTarget.localPosition = _baseLocalPosition;
            }
        }
    }
}
