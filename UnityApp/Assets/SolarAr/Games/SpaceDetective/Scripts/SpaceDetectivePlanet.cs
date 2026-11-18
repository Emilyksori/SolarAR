using System;
using System.Collections.Generic;
using SolarAR.Games.Shared;

namespace SolarAR.Games.SpaceDetective
{
    public sealed class SpaceDetectivePlanet
    {
        public SpaceDetectivePlanet(
            PlanetId id,
            string explanation,
            IReadOnlyList<string> clues)
        {
            if (string.IsNullOrWhiteSpace(explanation))
            {
                throw new ArgumentException("Explanation is required.", nameof(explanation));
            }

            if (clues == null || clues.Count < 3)
            {
                throw new ArgumentException("At least three clues are required.", nameof(clues));
            }

            Id = id;
            Explanation = explanation;
            Clues = clues;
        }

        public PlanetId Id { get; }
        public string Explanation { get; }
        public IReadOnlyList<string> Clues { get; }
    }
}
