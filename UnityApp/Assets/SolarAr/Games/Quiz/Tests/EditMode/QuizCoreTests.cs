using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;

namespace SolarAR.Games.Quiz.Tests
{
    public sealed class QuizCoreTests
    {
        private const string BankPath =
            "Assets/SolarAr/Games/Quiz/Data/QuizQuestionBank.asset";

        [Test]
        public void QuestionBank_HasThirtyFiveUniqueValidQuestions()
        {
            QuizQuestionBank bank = AssetDatabase.LoadAssetAtPath<QuizQuestionBank>(BankPath);

            Assert.That(bank, Is.Not.Null);
            Assert.That(bank.Questions, Has.Count.EqualTo(35));
            Assert.That(bank.Questions.Select(question => question.Id).Distinct().Count(), Is.EqualTo(35));
            Assert.That(bank.Questions.All(question => question.TryValidate(out _)), Is.True);
        }

        [Test]
        public void Selector_ReturnsFiveDistinctQuestions()
        {
            var selected = SelectFive();

            Assert.That(selected.Count, Is.EqualTo(5));
            Assert.That(selected.Select(question => question.Id).Distinct().Count(), Is.EqualTo(5));
        }

        [Test]
        public void Selector_PrioritizesFiveDifferentPlanets()
        {
            var selected = SelectFive();

            Assert.That(selected.Select(question => question.PlanetKey).Distinct().Count(), Is.EqualTo(5));
        }

        [Test]
        public void Selector_UsesPreferredDifficultyMix()
        {
            var selected = SelectFive();

            Assert.That(selected.Count(question => question.Difficulty == QuizDifficulty.Easy), Is.EqualTo(2));
            Assert.That(selected.Count(question => question.Difficulty == QuizDifficulty.Medium), Is.EqualTo(2));
            Assert.That(selected.Count(question => question.Difficulty == QuizDifficulty.Hard), Is.EqualTo(1));
        }

        [Test]
        public void Selector_ShufflesAlternativesAndPreservesCorrectAnswer()
        {
            QuizQuestion original = CreateQuestion("shuffle", "earth", QuizDifficulty.Easy);
            QuizQuestionSelector selector = new(new Random(1));
            var selected = selector.Select(new[] { original }, 1, out _);

            Assert.That(selected.Count, Is.EqualTo(1));
            Assert.That(selected[0].Alternatives, Is.EquivalentTo(original.Alternatives));
            Assert.That(selected[0].CorrectAlternative, Is.EqualTo("Correct"));
            Assert.That(selected[0].Alternatives.SequenceEqual(original.Alternatives), Is.False);
        }

        [TestCase(0, 0)]
        [TestCase(1, 10)]
        [TestCase(3, 30)]
        [TestCase(5, 50)]
        public void ScoreCalculator_AwardsTenPointsPerCorrectAnswer(int correct, int expected)
        {
            Assert.That(QuizScoreCalculator.CalculateScore(correct), Is.EqualTo(expected));
        }

        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(4, 3)]
        [TestCase(5, 3)]
        public void ScoreCalculator_MapsStarsToPerformance(int correct, int expected)
        {
            Assert.That(QuizScoreCalculator.CalculateStars(correct), Is.EqualTo(expected));
        }

        [Test]
        public void Session_BlocksDuplicateAnswerAndDuplicateScore()
        {
            QuizSession session = new(CreateRounds(1));

            Assert.That(session.TrySubmitAnswer(0, out _), Is.True);
            Assert.That(session.TrySubmitAnswer(0, out _), Is.False);
            Assert.That(session.CorrectAnswers, Is.EqualTo(1));
            Assert.That(session.Score, Is.EqualTo(10));
        }

        [Test]
        public void Session_DoesNotAdvanceBeforeAnswering()
        {
            QuizSession session = new(CreateRounds(1));

            Assert.That(session.TryAdvance(), Is.False);
            Assert.That(session.CurrentQuestionIndex, Is.Zero);
        }

        [Test]
        public void Session_CompletesOnlyAfterFiveAnsweredQuestions()
        {
            QuizSession session = new(CreateRounds(5));

            for (int index = 0; index < 5; index++)
            {
                Assert.That(session.TrySubmitAnswer(0, out _), Is.True);
                Assert.That(session.TryAdvance(), Is.True);
            }

            Assert.That(session.IsComplete, Is.True);
            Assert.That(session.CorrectAnswers, Is.EqualTo(5));
            Assert.That(session.Score, Is.EqualTo(50));
        }

        [Test]
        public void Session_RestartClearsPreviousState()
        {
            QuizSession session = new(CreateRounds(1));
            session.TrySubmitAnswer(0, out _);
            session.TryAdvance();

            session.Restart(CreateRounds(5));

            Assert.That(session.IsComplete, Is.False);
            Assert.That(session.HasAnsweredCurrent, Is.False);
            Assert.That(session.CorrectAnswers, Is.Zero);
            Assert.That(session.QuestionCount, Is.EqualTo(5));
            Assert.That(session.CurrentQuestionIndex, Is.Zero);
        }

        [Test]
        public void Selector_HandlesInsufficientBankWithoutLooping()
        {
            QuizQuestion[] source =
            {
                CreateQuestion("one", "earth", QuizDifficulty.Easy),
                CreateQuestion("two", "mars", QuizDifficulty.Medium)
            };
            QuizQuestionSelector selector = new(new Random(3));

            var selected = selector.Select(source, 5, out string warning);

            Assert.That(selected.Count, Is.EqualTo(2));
            Assert.That(warning, Is.Not.Null.And.Not.Empty);
        }

        private static IReadOnlyList<QuizRoundQuestion> SelectFive()
        {
            QuizQuestionSelector selector = new(new Random(7));
            return selector.Select(CreateBalancedQuestions(), 5, out _);
        }

        private static IReadOnlyList<QuizQuestion> CreateBalancedQuestions()
        {
            string[] planets =
            {
                "mercury", "earth", "mars", "jupiter", "saturn", "uranus", "neptune"
            };
            QuizDifficulty[] difficulties =
            {
                QuizDifficulty.Easy,
                QuizDifficulty.Easy,
                QuizDifficulty.Medium,
                QuizDifficulty.Medium,
                QuizDifficulty.Hard
            };
            List<QuizQuestion> questions = new();
            foreach (string planet in planets)
            {
                for (int index = 0; index < difficulties.Length; index++)
                {
                    questions.Add(CreateQuestion($"{planet}-{index}", planet, difficulties[index]));
                }
            }

            return questions;
        }

        private static QuizQuestion CreateQuestion(
            string id,
            string planet,
            QuizDifficulty difficulty)
        {
            return new QuizQuestion(
                id,
                planet,
                difficulty,
                "Prompt?",
                new[] { "Correct", "Wrong A", "Wrong B" },
                0,
                "Accessible explanation.");
        }

        private static IReadOnlyList<QuizRoundQuestion> CreateRounds(int count)
        {
            List<QuizRoundQuestion> questions = new();
            for (int index = 0; index < count; index++)
            {
                questions.Add(new QuizRoundQuestion(
                    $"round-{index}",
                    "earth",
                    QuizDifficulty.Easy,
                    "Prompt?",
                    new[] { "Correct", "Wrong A", "Wrong B" },
                    0,
                    "Explanation."));
            }

            return questions;
        }
    }
}
