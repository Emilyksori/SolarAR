using System;
using System.Collections.Generic;
using SolarAR.Games.Shared;

namespace SolarAR.Games.Memory
{
    public sealed class MemoryGameSession
    {
        public const int PairCount = 6;

        private readonly List<MemoryCardState> _cards;
        private int _firstSelectionIndex = -1;
        private int _pendingFirstIndex = -1;
        private int _pendingSecondIndex = -1;

        public MemoryGameSession(Random random)
            : this(RandomSelection.SelectDistinct(PlanetCatalog.All, PairCount, random), random)
        {
        }

        public MemoryGameSession(IReadOnlyList<PlanetId> selectedPlanets, Random random)
        {
            if (selectedPlanets == null)
            {
                throw new ArgumentNullException(nameof(selectedPlanets));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (selectedPlanets.Count != PairCount ||
                new HashSet<PlanetId>(selectedPlanets).Count != PairCount)
            {
                throw new ArgumentException(
                    "A memory game requires six distinct planets.",
                    nameof(selectedPlanets));
            }

            _cards = new List<MemoryCardState>(PairCount * 2);
            int cardId = 0;
            for (int index = 0; index < selectedPlanets.Count; index++)
            {
                _cards.Add(new MemoryCardState(cardId++, selectedPlanets[index]));
                _cards.Add(new MemoryCardState(cardId++, selectedPlanets[index]));
            }

            RandomSelection.Shuffle(_cards, random);
        }

        public IReadOnlyList<MemoryCardState> Cards => _cards;
        public int Attempts { get; private set; }
        public int MatchedPairs { get; private set; }
        public bool IsInteractionLocked { get; private set; }
        public bool IsComplete => MatchedPairs == PairCount;

        public MemorySelectionResult SelectCard(int cardIndex)
        {
            if (IsInteractionLocked || cardIndex < 0 || cardIndex >= _cards.Count)
            {
                return Invalid();
            }

            MemoryCardState card = _cards[cardIndex];
            if (card.IsRevealed || card.IsMatched)
            {
                return Invalid();
            }

            card.IsRevealed = true;
            if (_firstSelectionIndex < 0)
            {
                _firstSelectionIndex = cardIndex;
                return new MemorySelectionResult(
                    MemorySelectionStatus.FirstCardRevealed,
                    cardIndex,
                    -1);
            }

            int firstIndex = _firstSelectionIndex;
            _firstSelectionIndex = -1;
            Attempts++;

            MemoryCardState firstCard = _cards[firstIndex];
            if (firstCard.Planet == card.Planet)
            {
                firstCard.IsMatched = true;
                card.IsMatched = true;
                MatchedPairs++;
                return new MemorySelectionResult(
                    IsComplete ? MemorySelectionStatus.Completed : MemorySelectionStatus.Match,
                    firstIndex,
                    cardIndex);
            }

            IsInteractionLocked = true;
            _pendingFirstIndex = firstIndex;
            _pendingSecondIndex = cardIndex;
            return new MemorySelectionResult(
                MemorySelectionStatus.Mismatch,
                firstIndex,
                cardIndex);
        }

        public bool ResolveMismatch()
        {
            if (!IsInteractionLocked || _pendingFirstIndex < 0 || _pendingSecondIndex < 0)
            {
                return false;
            }

            _cards[_pendingFirstIndex].IsRevealed = false;
            _cards[_pendingSecondIndex].IsRevealed = false;
            _pendingFirstIndex = -1;
            _pendingSecondIndex = -1;
            IsInteractionLocked = false;
            return true;
        }

        public static int CalculateStars(int attempts)
        {
            if (attempts <= 9)
            {
                return 3;
            }

            return attempts <= 15 ? 2 : 1;
        }

        private static MemorySelectionResult Invalid()
        {
            return new MemorySelectionResult(MemorySelectionStatus.Invalid, -1, -1);
        }
    }
}
