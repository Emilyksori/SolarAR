using System.Collections.Generic;
using SolarAR.Games.Shared;

namespace SolarAR.Games.SpaceDetective
{
    public sealed class SpaceDetectiveChallenge
    {
        internal SpaceDetectiveChallenge(
            SpaceDetectivePlanet planet,
            IReadOnlyList<PlanetId> alternatives)
        {
            Planet = planet;
            Alternatives = alternatives;
            CluesShown = 1;
        }

        public SpaceDetectivePlanet Planet { get; }
        public IReadOnlyList<PlanetId> Alternatives { get; }
        public int CluesShown { get; internal set; }
        public bool HasAnswered { get; internal set; }
        public PlanetId? SelectedAnswer { get; internal set; }
        public bool IsCorrect { get; internal set; }
        public int PointsAwarded { get; internal set; }
    }
}
