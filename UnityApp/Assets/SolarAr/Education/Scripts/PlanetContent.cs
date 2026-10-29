using UnityEngine;

namespace SolarAR.Education
{
    [CreateAssetMenu(
        fileName = "PlanetContent",
        menuName = "SolarAR/Education/Planet Content"
    )]
    public sealed class PlanetContent : ScriptableObject
    {
        [Header("Identification")]
        [SerializeField] private string _planetKey;
        [SerializeField] private string _displayName;
        [SerializeField, Min(1)] private int _orderFromSun = 1;

        [Header("Educational Content")]
        [SerializeField, TextArea(2, 4)] private string _shortDescription;
        [SerializeField, TextArea(4, 10)] private string _educationalSummary;

        [Header("Scientific Data")]
        [SerializeField, Min(0f)] private float _diameterKm;
        [SerializeField, Min(0f)] private float _distanceFromSunMillionKm;
        [SerializeField, Min(0f)] private float _orbitalPeriodDays;
        [SerializeField] private float _rotationPeriodHours;
        [SerializeField, Min(0)] private int _moonCount;
        [SerializeField] private float _averageTemperatureCelsius;

        [Header("Visual Content")]
        [SerializeField] private Sprite _thumbnail;

        public string PlanetKey => _planetKey;
        public string DisplayName => _displayName;
        public int OrderFromSun => _orderFromSun;
        public string ShortDescription => _shortDescription;
        public string EducationalSummary => _educationalSummary;
        public float DiameterKm => _diameterKm;
        public float DistanceFromSunMillionKm => _distanceFromSunMillionKm;
        public float OrbitalPeriodDays => _orbitalPeriodDays;
        public float RotationPeriodHours => _rotationPeriodHours;
        public int MoonCount => _moonCount;
        public float AverageTemperatureCelsius => _averageTemperatureCelsius;
        public Sprite Thumbnail => _thumbnail;
    }
}
