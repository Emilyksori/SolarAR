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
            [SerializeField] private string _planetKey;
            [SerializeField] private GameObject _planetPrefab;

            public string ReferenceImageName => _referenceImageName;
            public string PlanetKey => _planetKey;
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

        public bool TryGetPlanetKey(string referenceImageName, out string planetKey)
        {
            foreach (var planet in _planets)
            {
                if (planet != null &&
                    string.Equals(planet.ReferenceImageName, referenceImageName, StringComparison.Ordinal))
                {
                    planetKey = planet.PlanetKey;
                    return !string.IsNullOrWhiteSpace(planetKey);
                }
            }

            planetKey = null;
            return false;
        }

        public bool TryGetPrefabByPlanetKey(string planetKey, out GameObject planetPrefab)
        {
            foreach (var planet in _planets)
            {
                if (planet != null &&
                    string.Equals(planet.PlanetKey, planetKey, StringComparison.Ordinal))
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
