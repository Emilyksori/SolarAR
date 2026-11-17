using System;
using System.Collections.Generic;
using System.Linq;
using SolarAR.Games.Shared;

namespace SolarAR.Games.SpaceDetective
{
    public sealed class SpaceDetectiveSession
    {
        public const int ChallengeCount = 5;
        public const int MaximumClues = 3;

        private readonly List<SpaceDetectiveChallenge> _challenges;
        private int _currentIndex;

        public SpaceDetectiveSession(
            IReadOnlyList<SpaceDetectivePlanet> content,
            Random random)
        {
            if (content == null || content.Count < 7)
            {
                throw new ArgumentException("Content for all seven planets is required.", nameof(content));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (content.Select(item => item.Id).Distinct().Count() != content.Count)
            {
                throw new ArgumentException("Planet content must be unique.", nameof(content));
            }

            List<SpaceDetectivePlanet> selected =
                RandomSelection.SelectDistinct(content, ChallengeCount, random);
            _challenges = new List<SpaceDetectiveChallenge>(ChallengeCount);
            foreach (SpaceDetectivePlanet answer in selected)
            {
                List<PlanetId> wrongPool = content
                    .Where(item => item.Id != answer.Id)
                    .Select(item => item.Id)
                    .ToList();
                List<PlanetId> alternatives =
                    RandomSelection.SelectDistinct(wrongPool, 2, random);
                alternatives.Add(answer.Id);
                RandomSelection.Shuffle(alternatives, random);
                _challenges.Add(new SpaceDetectiveChallenge(answer, alternatives));
            }
        }

        public IReadOnlyList<SpaceDetectiveChallenge> Challenges => _challenges;
        public SpaceDetectiveChallenge CurrentChallenge => _challenges[_currentIndex];
        public int ChallengeNumber => _currentIndex + 1;
        public int Score { get; private set; }
        public bool IsComplete { get; private set; }

        public bool RevealNextClue()
        {
            SpaceDetectiveChallenge challenge = CurrentChallenge;
            if (IsComplete || challenge.HasAnswered || challenge.CluesShown >= MaximumClues)
            {
                return false;
            }

            challenge.CluesShown++;
            return true;
        }

        public SpaceDetectiveAnswerResult SubmitAnswer(PlanetId answer)
        {
            SpaceDetectiveChallenge challenge = CurrentChallenge;
            if (IsComplete || challenge.HasAnswered || !challenge.Alternatives.Contains(answer))
            {
                return new SpaceDetectiveAnswerResult(
                    false,
                    false,
                    0,
                    challenge.Planet.Id,
                    challenge.Planet.Explanation);
            }

            challenge.HasAnswered = true;
            challenge.SelectedAnswer = answer;
            challenge.IsCorrect = answer == challenge.Planet.Id;
            challenge.PointsAwarded = challenge.IsCorrect
                ? PointsForClues(challenge.CluesShown)
                : 0;
            Score += challenge.PointsAwarded;

            return new SpaceDetectiveAnswerResult(
                true,
                challenge.IsCorrect,
                challenge.PointsAwarded,
                challenge.Planet.Id,
                challenge.Planet.Explanation);
        }

        public bool Advance()
        {
            if (IsComplete || !CurrentChallenge.HasAnswered)
            {
                return false;
            }

            if (_currentIndex == _challenges.Count - 1)
            {
                IsComplete = true;
                return true;
            }

            _currentIndex++;
            return true;
        }

        public static int CalculateStars(int score)
        {
            if (score >= 40)
            {
                return 3;
            }

            return score >= 20 ? 2 : 1;
        }

        private static int PointsForClues(int cluesShown)
        {
            return cluesShown switch
            {
                1 => 10,
                2 => 7,
                _ => 5
            };
        }
    }
}
