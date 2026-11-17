using System;
using System.Collections.Generic;
using SolarAR.Games.Shared;

namespace SolarAR.Games.PlanetOrder
{
    public sealed class PlanetOrderSession
    {
        private readonly List<PlanetId> _currentOrder;

        public PlanetOrderSession(Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            _currentOrder = new List<PlanetId>(PlanetCatalog.All);
            RandomSelection.Shuffle(_currentOrder, random);
            if (IsExpectedOrder(_currentOrder))
            {
                (_currentOrder[0], _currentOrder[1]) = (_currentOrder[1], _currentOrder[0]);
            }
        }

        public IReadOnlyList<PlanetId> CurrentOrder => _currentOrder;
        public int CheckCount { get; private set; }
        public bool IsComplete { get; private set; }

        public bool SwapPositions(int firstIndex, int secondIndex)
        {
            if (IsComplete ||
                firstIndex < 0 || firstIndex >= _currentOrder.Count ||
                secondIndex < 0 || secondIndex >= _currentOrder.Count ||
                firstIndex == secondIndex)
            {
                return false;
            }

            (_currentOrder[firstIndex], _currentOrder[secondIndex]) =
                (_currentOrder[secondIndex], _currentOrder[firstIndex]);
            return true;
        }

        public PlanetOrderCheckResult CheckOrder()
        {
            if (!IsComplete)
            {
                CheckCount++;
            }

            bool[] correctPositions = new bool[PlanetCatalog.All.Count];
            bool allCorrect = true;
            for (int index = 0; index < correctPositions.Length; index++)
            {
                correctPositions[index] = _currentOrder[index] == PlanetCatalog.All[index];
                allCorrect &= correctPositions[index];
            }

            IsComplete = allCorrect;
            return new PlanetOrderCheckResult(correctPositions, IsComplete, CheckCount);
        }

        public static int CalculateStars(int checkCount)
        {
            if (checkCount <= 1)
            {
                return 3;
            }

            return checkCount <= 3 ? 2 : 1;
        }

        private static bool IsExpectedOrder(IReadOnlyList<PlanetId> order)
        {
            for (int index = 0; index < PlanetCatalog.All.Count; index++)
            {
                if (order[index] != PlanetCatalog.All[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
