using System;
using System.Linq;
using NUnit.Framework;
using SolarAR.Games.Shared;

namespace SolarAR.Games.PlanetOrder.Tests
{
    public sealed class PlanetOrderSessionTests
    {
        [Test]
        public void NewGame_StartsWithShuffledCompletePlanetSet()
        {
            PlanetOrderSession session = new(new Random(12));

            Assert.That(session.CurrentOrder.Count, Is.EqualTo(7));
            Assert.That(session.CurrentOrder, Is.Unique);
            Assert.That(session.CurrentOrder, Is.EquivalentTo(PlanetCatalog.All));
            Assert.That(session.CurrentOrder, Is.Not.EqualTo(PlanetCatalog.All));
        }

        [Test]
        public void SwapPositions_MovesBothPlanetsAndRejectsInvalidMoves()
        {
            PlanetOrderSession session = new(new Random(3));
            PlanetId first = session.CurrentOrder[0];
            PlanetId second = session.CurrentOrder[1];

            Assert.That(session.SwapPositions(0, 1), Is.True);
            Assert.That(session.CurrentOrder[0], Is.EqualTo(second));
            Assert.That(session.CurrentOrder[1], Is.EqualTo(first));
            Assert.That(session.SwapPositions(0, 0), Is.False);
            Assert.That(session.SwapPositions(-1, 2), Is.False);
            Assert.That(session.SwapPositions(2, 7), Is.False);
        }

        [Test]
        public void CheckOrder_ReportsEveryPositionAndCountsChecks()
        {
            PlanetOrderSession session = new(new Random(22));

            PlanetOrderCheckResult firstCheck = session.CheckOrder();
            PlanetOrderCheckResult secondCheck = session.CheckOrder();

            Assert.That(firstCheck.CorrectPositions.Count, Is.EqualTo(7));
            Assert.That(firstCheck.IsComplete, Is.False);
            Assert.That(secondCheck.CheckCount, Is.EqualTo(2));
            Assert.That(session.CheckCount, Is.EqualTo(2));
        }

        [Test]
        public void CorrectSequence_CompletesGameAndBlocksFurtherMovement()
        {
            PlanetOrderSession session = new(new Random(8));
            PutInExpectedOrder(session);

            PlanetOrderCheckResult result = session.CheckOrder();

            Assert.That(result.CorrectPositions, Is.All.True);
            Assert.That(session.IsComplete, Is.True);
            Assert.That(session.SwapPositions(0, 1), Is.False);
        }

        [Test]
        public void NewSession_DoesNotReusePreviousChecks()
        {
            PlanetOrderSession previous = new(new Random(18));
            previous.CheckOrder();

            PlanetOrderSession restarted = new(new Random(19));

            Assert.That(restarted.CheckCount, Is.Zero);
            Assert.That(restarted.IsComplete, Is.False);
        }

        [TestCase(1, 3)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(4, 1)]
        [TestCase(8, 1)]
        public void Stars_FollowCheckThresholds(int checks, int expectedStars)
        {
            Assert.That(PlanetOrderSession.CalculateStars(checks), Is.EqualTo(expectedStars));
        }

        private static void PutInExpectedOrder(PlanetOrderSession session)
        {
            for (int targetIndex = 0; targetIndex < PlanetCatalog.All.Count; targetIndex++)
            {
                PlanetId target = PlanetCatalog.All[targetIndex];
                int currentIndex = session.CurrentOrder.ToList().IndexOf(target);
                if (currentIndex != targetIndex)
                {
                    session.SwapPositions(targetIndex, currentIndex);
                }
            }
        }
    }
}
