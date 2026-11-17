using System;
using System.Collections.Generic;

namespace SolarAR.Education
{
    public sealed class PlanetFactCarousel
    {
        private readonly List<PlanetFact> _facts = new();

        public int CurrentIndex { get; private set; }
        public int Count => _facts.Count;
        public bool HasFacts => _facts.Count > 0;
        public bool CanMovePrevious => CurrentIndex > 0;
        public bool CanMoveNext => CurrentIndex + 1 < _facts.Count;
        public PlanetFact Current => HasFacts ? _facts[CurrentIndex] : null;
        public string ProgressText => HasFacts ? $"{CurrentIndex + 1}/{Count}" : "0/0";

        public void SetFacts(IReadOnlyList<PlanetFact> facts)
        {
            _facts.Clear();
            CurrentIndex = 0;

            if (facts == null)
            {
                return;
            }

            for (int index = 0; index < facts.Count; index++)
            {
                if (facts[index] != null)
                {
                    _facts.Add(facts[index]);
                }
            }

            _facts.Sort(CompareFacts);
        }

        public bool MovePrevious()
        {
            if (!CanMovePrevious)
            {
                return false;
            }

            CurrentIndex--;
            return true;
        }

        public bool MoveNext()
        {
            if (!CanMoveNext)
            {
                return false;
            }

            CurrentIndex++;
            return true;
        }

        private static int CompareFacts(PlanetFact left, PlanetFact right)
        {
            int orderComparison = left.Order.CompareTo(right.Order);
            return orderComparison != 0
                ? orderComparison
                : string.Compare(left.Id, right.Id, StringComparison.Ordinal);
        }
    }
}
