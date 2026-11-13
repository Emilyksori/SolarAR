using UnityEngine;

namespace SolarAR.Planets
{
    public sealed class PlanetDisplay : MonoBehaviour
    {
        [SerializeField] private string _planetKey;
        [SerializeField] private Transform _visualRoot;
        [SerializeField, Min(0.001f)] private float _displayDiameterMeters = 0.058f;
        [SerializeField, Min(0f)] private float _visualOffsetMeters = 0.025f;

        public string PlanetKey => _planetKey;
        public Transform VisualRoot => _visualRoot;
        public float DisplayDiameterMeters => _displayDiameterMeters;
        public float VisualOffsetMeters => _visualOffsetMeters;
    }
}
