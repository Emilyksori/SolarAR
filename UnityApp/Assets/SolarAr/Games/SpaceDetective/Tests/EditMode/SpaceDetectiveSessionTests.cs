using System;
using System.Linq;
using NUnit.Framework;
using SolarAR.Games.Shared;

namespace SolarAR.Games.SpaceDetective.Tests
{
    public sealed class SpaceDetectiveSessionTests
    {
        [Test]
        public void NewGame_SelectsFiveDifferentPlanets()
        {
            SpaceDetectiveSession session = CreateSession(4);

            Assert.That(session.Challenges.Count, Is.EqualTo(5));
            Assert.That(session.Challenges.Select(item => item.Planet.Id), Is.Unique);
        }

        [Test]
        public void Clues_AreProgressiveAndLimitedToThree()
        {
            SpaceDetectiveSession session = CreateSession(5);

            Assert.That(session.CurrentChallenge.CluesShown, Is.EqualTo(1));
            Assert.That(session.RevealNextClue(), Is.True);
            Assert.That(session.RevealNextClue(), Is.True);
            Assert.That(session.CurrentChallenge.CluesShown, Is.EqualTo(3));
            Assert.That(session.RevealNextClue(), Is.False);
        }

        [Test]
        public void EveryChallenge_HasThreeDistinctPlanetAlternativesIncludingAnswer()
        {
            SpaceDetectiveSession session = CreateSession(6);

            foreach (SpaceDetectiveChallenge challenge in session.Challenges)
            {
                Assert.That(challenge.Alternatives.Count, Is.EqualTo(3));
                Assert.That(challenge.Alternatives, Is.Unique);
                Assert.That(challenge.Alternatives, Does.Contain(challenge.Planet.Id));
            }
        }

        [Test]
        public void Answer_CanOnlyBeSubmittedOnce()
        {
            SpaceDetectiveSession session = CreateSession(7);
            PlanetId correct = session.CurrentChallenge.Planet.Id;

            SpaceDetectiveAnswerResult first = session.SubmitAnswer(correct);
            SpaceDetectiveAnswerResult duplicate = session.SubmitAnswer(correct);

            Assert.That(first.Accepted, Is.True);
            Assert.That(duplicate.Accepted, Is.False);
            Assert.That(session.Score, Is.EqualTo(10));
            Assert.That(session.RevealNextClue(), Is.False);
        }

        [TestCase(0, 10)]
        [TestCase(1, 7)]
        [TestCase(2, 5)]
        public void CorrectAnswer_ScoresAccordingToCluesRequested(
            int extraClues,
            int expectedPoints)
        {
            SpaceDetectiveSession session = CreateSession(8 + extraClues);
            for (int index = 0; index < extraClues; index++)
            {
                session.RevealNextClue();
            }

            SpaceDetectiveAnswerResult result = session.SubmitAnswer(
                session.CurrentChallenge.Planet.Id);

            Assert.That(result.Points, Is.EqualTo(expectedPoints));
            Assert.That(session.Score, Is.EqualTo(expectedPoints));
        }

        [Test]
        public void IncorrectAnswer_ScoresZero()
        {
            SpaceDetectiveSession session = CreateSession(11);
            PlanetId wrong = session.CurrentChallenge.Alternatives
                .First(option => option != session.CurrentChallenge.Planet.Id);

            SpaceDetectiveAnswerResult result = session.SubmitAnswer(wrong);

            Assert.That(result.Accepted, Is.True);
            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Points, Is.Zero);
            Assert.That(session.Score, Is.Zero);
        }

        [Test]
        public void FiveAnsweredChallenges_CompleteGame()
        {
            SpaceDetectiveSession session = CreateSession(12);

            for (int index = 0; index < SpaceDetectiveSession.ChallengeCount; index++)
            {
                session.SubmitAnswer(session.CurrentChallenge.Planet.Id);
                Assert.That(session.Advance(), Is.True);
            }

            Assert.That(session.IsComplete, Is.True);
            Assert.That(session.Score, Is.EqualTo(50));
        }

        [Test]
        public void NewSession_DoesNotReusePreviousScoreOrProgress()
        {
            SpaceDetectiveSession previous = CreateSession(20);
            previous.SubmitAnswer(previous.CurrentChallenge.Planet.Id);
            previous.Advance();

            SpaceDetectiveSession restarted = CreateSession(21);

            Assert.That(restarted.Score, Is.Zero);
            Assert.That(restarted.ChallengeNumber, Is.EqualTo(1));
            Assert.That(restarted.IsComplete, Is.False);
            Assert.That(restarted.CurrentChallenge.HasAnswered, Is.False);
        }

        [TestCase(50, 3)]
        [TestCase(40, 3)]
        [TestCase(39, 2)]
        [TestCase(20, 2)]
        [TestCase(19, 1)]
        [TestCase(0, 1)]
        public void Stars_FollowScoreThresholds(int score, int expectedStars)
        {
            Assert.That(SpaceDetectiveSession.CalculateStars(score), Is.EqualTo(expectedStars));
        }

        [Test]
        public void Content_HasThreeNameFreeCluesForEveryPlanet()
        {
            foreach (SpaceDetectivePlanet planet in SpaceDetectiveContent.CreateDefault())
            {
                string name = PlanetCatalog.GetDisplayName(planet.Id);
                Assert.That(planet.Clues.Count, Is.GreaterThanOrEqualTo(3));
                Assert.That(planet.Clues.Any(clue =>
                    clue.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
            }
        }

        private static SpaceDetectiveSession CreateSession(int seed)
        {
            return new SpaceDetectiveSession(
                SpaceDetectiveContent.CreateDefault(),
                new Random(seed));
        }
    }
}
