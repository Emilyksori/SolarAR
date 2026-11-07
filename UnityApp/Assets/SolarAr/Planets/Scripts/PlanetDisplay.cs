using UnityEngine;

namespace SolarAR.Planets
{
    public sealed class PlanetDisplay : MonoBehaviour
    {
        [SerializeField] private string _planetKey;
        [SerializeField] private Transform _visualRoot;
        [SerializeField, Min(0.001f)] private float _displayDiameterMeters = 0.048f;

        public string PlanetKey => _planetKey;
        public Transform VisualRoot => _visualRoot;
        public float DisplayDiameterMeters => _displayDiameterMeters;
    }
}
