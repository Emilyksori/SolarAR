using System.Collections.Generic;

namespace SolarAR.Games.PlanetOrder
{
    public sealed class PlanetOrderCheckResult
    {
        public PlanetOrderCheckResult(bool[] correctPositions, bool isComplete, int checkCount)
        {
            CorrectPositions = correctPositions;
            IsComplete = isComplete;
            CheckCount = checkCount;
        }

        public IReadOnlyList<bool> CorrectPositions { get; }
        public bool IsComplete { get; }
        public int CheckCount { get; }
    }
}
