using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolarAR.Planets
{
    public sealed class PlanetRegistry : MonoBehaviour
    {
        [Serializable]
        private sealed class PlanetEntry
        {
            [SerializeField] private string _referenceImageName;
            [SerializeField] private GameObject _planetPrefab;

            public string ReferenceImageName => _referenceImageName;
            public GameObject PlanetPrefab => _planetPrefab;
        }

        [SerializeField] private List<PlanetEntry> _planets = new();

        public bool TryGetPrefab(string referenceImageName, out GameObject planetPrefab)
        {
            foreach (var planet in _planets)
            {
                if (planet != null &&
                    string.Equals(planet.ReferenceImageName, referenceImageName, StringComparison.Ordinal))
                {
                    planetPrefab = planet.PlanetPrefab;
                    return planetPrefab != null;
                }
            }

            planetPrefab = null;
            return false;
        }
    }
}
