using UnityEngine;

namespace SolarAR.Planets
{
    public sealed class RotationController : MonoBehaviour
    {
        [SerializeField] private Transform _rotationTarget;
        [SerializeField] private Vector3 _rotationAxis = Vector3.up;
        [SerializeField] private float _rotationSpeedDegreesPerSecond = 10f;
        [SerializeField] private bool _autoRotate = true;

        public Transform RotationTarget => _rotationTarget;
        public Vector3 RotationAxis => _rotationAxis;
        public float RotationSpeedDegreesPerSecond => _rotationSpeedDegreesPerSecond;
        public bool AutoRotate => _autoRotate;

        private void Awake()
        {
            if (_rotationTarget == null)
            {
                Debug.LogWarning("[Planets] Rotation target is not assigned.", this);
            }
        }

        private void Update()
        {
            if (!_autoRotate || _rotationTarget == null)
            {
                return;
            }

            _rotationTarget.Rotate(
                _rotationAxis,
                _rotationSpeedDegreesPerSecond * Time.deltaTime,
                Space.Self);
        }

        public void SetAutoRotation(bool enabled)
        {
            _autoRotate = enabled;
        }
    }
}
