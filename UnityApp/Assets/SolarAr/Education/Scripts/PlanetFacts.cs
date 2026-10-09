using System.Collections.Generic;
using UnityEngine;

namespace SolarAR.Education
{
    [CreateAssetMenu(
        fileName = "PlanetFacts",
        menuName = "SolarAR/Education/Planet Facts"
    )]
    public sealed class PlanetFacts : ScriptableObject
    {
        [SerializeField] private string _planetKey;
        [SerializeField] private List<PlanetFact> _facts = new();

        public string PlanetKey => _planetKey;
        public IReadOnlyList<PlanetFact> Facts => _facts;
    }
}
