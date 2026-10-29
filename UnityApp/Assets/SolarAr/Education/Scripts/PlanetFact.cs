using System;
using UnityEngine;

namespace SolarAR.Education
{
    [Serializable]
    public sealed class PlanetFact
    {
        [SerializeField] private string _title;
        [SerializeField, TextArea(2, 5)] private string _description;

        public string Title => _title;
        public string Description => _description;
    }
}
