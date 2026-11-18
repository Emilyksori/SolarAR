using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SolarAR.Games.Shared;

namespace SolarAR.Games.Memory.Tests
{
    public sealed class MemoryGameSessionTests
    {
        [Test]
        public void RandomGame_SelectsSixDistinctPlanetsAndCreatesSixPairs()
        {
            MemoryGameSession session = new(new Random(17));

            Assert.That(session.Cards.Count, Is.EqualTo(12));
            Assert.That(session.Cards.Select(card => card.Planet).Distinct().Count(), Is.EqualTo(6));
            foreach (IGrouping<PlanetId, MemoryCardState> group in
                     session.Cards.GroupBy(card => card.Planet))
            {
                Assert.That(group.Count(), Is.EqualTo(2));
            }
        }

        [Test]
        public void Constructor_ShufflesCardPositions()
        {
            MemoryGameSession session = CreateSession(31);

            Assert.That(session.Cards.Select(card => card.Id),
                Is.Not.EqualTo(Enumerable.Range(0, 12)));
        }

        [Test]
        public void MatchingAndMismatchingCards_UpdateStateAndAttempts()
        {
            MemoryGameSession session = CreateSession(9);
            (int firstMatch, int secondMatch) = FindPair(session);

            Assert.That(session.SelectCard(firstMatch).Status,
                Is.EqualTo(MemorySelectionStatus.FirstCardRevealed));
            Assert.That(session.SelectCard(secondMatch).Status,
                Is.EqualTo(MemorySelectionStatus.Match));
            Assert.That(session.MatchedPairs, Is.EqualTo(1));
            Assert.That(session.Attempts, Is.EqualTo(1));

            (int firstMismatch, int secondMismatch) = FindMismatch(session);
            session.SelectCard(firstMismatch);
            Assert.That(session.SelectCard(secondMismatch).Status,
                Is.EqualTo(MemorySelectionStatus.Mismatch));
            Assert.That(session.IsInteractionLocked, Is.True);
            Assert.That(session.Attempts, Is.EqualTo(2));
            Assert.That(session.ResolveMismatch(), Is.True);
            Assert.That(session.Cards[firstMismatch].IsRevealed, Is.False);
            Assert.That(session.Cards[secondMismatch].IsRevealed, Is.False);
        }

        [Test]
        public void InvalidSelections_AreBlockedWithoutCountingAttempt()
        {
            MemoryGameSession session = CreateSession(7);
            (int firstMismatch, int secondMismatch) = FindMismatch(session);

            session.SelectCard(firstMismatch);
            Assert.That(session.SelectCard(firstMismatch).Status,
                Is.EqualTo(MemorySelectionStatus.Invalid));
            session.SelectCard(secondMismatch);
            Assert.That(session.SelectCard(0).Status,
                Is.EqualTo(MemorySelectionStatus.Invalid));
            Assert.That(session.Attempts, Is.EqualTo(1));
        }

        [Test]
        public void FindingAllPairs_CompletesGame()
        {
            MemoryGameSession session = CreateSession(3);
            foreach (IGrouping<PlanetId, (MemoryCardState card, int index)> pair in
                     session.Cards.Select((card, index) => (card, index)).GroupBy(item => item.card.Planet))
            {
                int[] indices = pair.Select(item => item.index).ToArray();
                session.SelectCard(indices[0]);
                session.SelectCard(indices[1]);
            }

            Assert.That(session.IsComplete, Is.True);
            Assert.That(session.MatchedPairs, Is.EqualTo(6));
            Assert.That(session.Attempts, Is.EqualTo(6));
        }

        [Test]
        public void NewSession_DoesNotReusePreviousProgress()
        {
            MemoryGameSession previous = CreateSession(14);
            (int first, int second) = FindPair(previous);
            previous.SelectCard(first);
            previous.SelectCard(second);

            MemoryGameSession restarted = CreateSession(15);

            Assert.That(restarted.Attempts, Is.Zero);
            Assert.That(restarted.MatchedPairs, Is.Zero);
            Assert.That(restarted.IsComplete, Is.False);
        }

        [TestCase(6, 3)]
        [TestCase(9, 3)]
        [TestCase(10, 2)]
        [TestCase(15, 2)]
        [TestCase(16, 1)]
        [TestCase(25, 1)]
        public void Stars_FollowAttemptThresholds(int attempts, int expectedStars)
        {
            Assert.That(MemoryGameSession.CalculateStars(attempts), Is.EqualTo(expectedStars));
        }

        private static MemoryGameSession CreateSession(int seed)
        {
            return new MemoryGameSession(
                new[]
                {
                    PlanetId.Mercury,
                    PlanetId.Earth,
                    PlanetId.Mars,
                    PlanetId.Jupiter,
                    PlanetId.Saturn,
                    PlanetId.Uranus
                },
                new Random(seed));
        }

        private static (int first, int second) FindPair(MemoryGameSession session)
        {
            for (int first = 0; first < session.Cards.Count; first++)
            {
                for (int second = first + 1; second < session.Cards.Count; second++)
                {
                    if (session.Cards[first].Planet == session.Cards[second].Planet)
                    {
                        return (first, second);
                    }
                }
            }

            throw new AssertionException("No pair found.");
        }

        private static (int first, int second) FindMismatch(MemoryGameSession session)
        {
            for (int first = 0; first < session.Cards.Count; first++)
            {
                if (session.Cards[first].IsMatched)
                {
                    continue;
                }

                for (int second = first + 1; second < session.Cards.Count; second++)
                {
                    if (!session.Cards[second].IsMatched &&
                        session.Cards[first].Planet != session.Cards[second].Planet)
                    {
                        return (first, second);
                    }
                }
            }

            throw new AssertionException("No mismatch found.");
        }
    }
}
