using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolarAR.Education
{
    public sealed class EducationalContentManager : MonoBehaviour
    {
        [Header("Educational Data")]
        [SerializeField] private List<PlanetContent> _planetContents = new();
        [SerializeField] private List<PlanetFacts> _planetFacts = new();

        private readonly Dictionary<string, PlanetContent> _contentByPlanetKey =
            new(StringComparer.Ordinal);
        private readonly Dictionary<string, PlanetFacts> _factsByPlanetKey =
            new(StringComparer.Ordinal);

        private void Awake()
        {
            BuildLookups();
        }

        public bool TryGetContent(string planetKey, out PlanetContent content)
        {
            if (string.IsNullOrWhiteSpace(planetKey))
            {
                content = null;
                return false;
            }

            return _contentByPlanetKey.TryGetValue(planetKey, out content);
        }

        public bool TryGetFacts(string planetKey, out PlanetFacts facts)
        {
            if (string.IsNullOrWhiteSpace(planetKey))
            {
                facts = null;
                return false;
            }

            return _factsByPlanetKey.TryGetValue(planetKey, out facts);
        }

        private void BuildLookups()
        {
            _contentByPlanetKey.Clear();
            _factsByPlanetKey.Clear();

            BuildContentLookup();
            BuildFactsLookup();
        }

        private void BuildContentLookup()
        {
            if (_planetContents == null)
            {
                Debug.LogWarning("[Education] PlanetContent list is null.", this);
                return;
            }

            foreach (PlanetContent content in _planetContents)
            {
                if (content == null)
                {
                    Debug.LogWarning("[Education] PlanetContent list contains a null item.", this);
                    continue;
                }

                string planetKey = content.PlanetKey;

                if (string.IsNullOrWhiteSpace(planetKey))
                {
                    Debug.LogWarning("[Education] PlanetContent has an empty planetKey.", content);
                    continue;
                }

                if (!_contentByPlanetKey.TryAdd(planetKey, content))
                {
                    Debug.LogWarning($"[Education] Duplicate PlanetContent key '{planetKey}'.", content);
                }
            }
        }

        private void BuildFactsLookup()
        {
            if (_planetFacts == null)
            {
                Debug.LogWarning("[Education] PlanetFacts list is null.", this);
                return;
            }

            foreach (PlanetFacts facts in _planetFacts)
            {
                if (facts == null)
                {
                    Debug.LogWarning("[Education] PlanetFacts list contains a null item.", this);
                    continue;
                }

                string planetKey = facts.PlanetKey;

                if (string.IsNullOrWhiteSpace(planetKey))
                {
                    Debug.LogWarning("[Education] PlanetFacts has an empty planetKey.", facts);
                    continue;
                }

                if (!_factsByPlanetKey.TryAdd(planetKey, facts))
                {
                    Debug.LogWarning($"[Education] Duplicate PlanetFacts key '{planetKey}'.", facts);
                }
            }
        }
    }
}
