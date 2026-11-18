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

        private Dictionary<string, GameObject> _prefabsByReferenceImageName;
        private Dictionary<string, string> _planetKeysByReferenceImageName;
        private Dictionary<string, GameObject> _prefabsByPlanetKey;
        private bool _cacheDirty = true;

        public int Count => _planets?.Count ?? 0;

        private void Awake()
        {
            RebuildCache();
        }

        private void OnValidate()
        {
            _cacheDirty = true;
        }

        public bool TryGetPrefab(string referenceImageName, out GameObject planetPrefab)
        {
            EnsureCache();
            return _prefabsByReferenceImageName.TryGetValue(
                       referenceImageName ?? string.Empty,
                       out planetPrefab) && planetPrefab != null;
        }

        public bool TryGetPlanetKey(string referenceImageName, out string planetKey)
        {
            EnsureCache();
            return _planetKeysByReferenceImageName.TryGetValue(
                       referenceImageName ?? string.Empty,
                       out planetKey) && !string.IsNullOrWhiteSpace(planetKey);
        }

        public bool TryGetPrefabByPlanetKey(string planetKey, out GameObject planetPrefab)
        {
            EnsureCache();
            return _prefabsByPlanetKey.TryGetValue(
                       planetKey ?? string.Empty,
                       out planetPrefab) && planetPrefab != null;
        }

        public bool TryGetEntry(
            int index,
            out string referenceImageName,
            out GameObject planetPrefab)
        {
            if (_planets == null || index < 0 || index >= _planets.Count)
            {
                referenceImageName = null;
                planetPrefab = null;
                return false;
            }

            PlanetEntry planet = _planets[index];
            referenceImageName = planet?.ReferenceImageName;
            planetPrefab = planet?.PlanetPrefab;
            return !string.IsNullOrWhiteSpace(referenceImageName) && planetPrefab != null;
        }

        private void EnsureCache()
        {
            if (_cacheDirty || _prefabsByReferenceImageName == null)
            {
                RebuildCache();
            }
        }

        private void RebuildCache()
        {
            _prefabsByReferenceImageName = new Dictionary<string, GameObject>(
                StringComparer.Ordinal);
            _planetKeysByReferenceImageName = new Dictionary<string, string>(
                StringComparer.Ordinal);
            _prefabsByPlanetKey = new Dictionary<string, GameObject>(StringComparer.Ordinal);

            if (_planets != null)
            {
                foreach (PlanetEntry planet in _planets)
                {
                    if (planet == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(planet.ReferenceImageName))
                    {
                        _prefabsByReferenceImageName[planet.ReferenceImageName] =
                            planet.PlanetPrefab;
                        _planetKeysByReferenceImageName[planet.ReferenceImageName] =
                            planet.PlanetKey;
                    }

                    if (!string.IsNullOrWhiteSpace(planet.PlanetKey))
                    {
                        _prefabsByPlanetKey[planet.PlanetKey] = planet.PlanetPrefab;
                    }
                }
            }

            _cacheDirty = false;
        }
    }
}
