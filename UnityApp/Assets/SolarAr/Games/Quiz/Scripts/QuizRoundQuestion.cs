using System;
using System.Collections.Generic;

namespace SolarAR.Games.Quiz
{
    public sealed class QuizRoundQuestion
    {
        public QuizRoundQuestion(
            string id,
            string planetKey,
            QuizDifficulty difficulty,
            string prompt,
            IReadOnlyList<string> alternatives,
            int correctAlternativeIndex,
            string explanation)
        {
            Id = id;
            PlanetKey = planetKey;
            Difficulty = difficulty;
            Prompt = prompt;
            Alternatives = alternatives;
            CorrectAlternativeIndex = correctAlternativeIndex;
            Explanation = explanation;
        }

        public string Id { get; }
        public string PlanetKey { get; }
        public QuizDifficulty Difficulty { get; }
        public string Prompt { get; }
        public IReadOnlyList<string> Alternatives { get; }
        public int CorrectAlternativeIndex { get; }
        public string Explanation { get; }
        public string CorrectAlternative => Alternatives[CorrectAlternativeIndex];

        public static QuizRoundQuestion CreateShuffled(QuizQuestion question, Random random)
        {
            int[] indexes = { 0, 1, 2 };
            for (int index = indexes.Length - 1; index > 0; index--)
            {
                int swapIndex = random.Next(index + 1);
                (indexes[index], indexes[swapIndex]) = (indexes[swapIndex], indexes[index]);
            }

            string[] alternatives = new string[indexes.Length];
            int correctIndex = -1;
            for (int index = 0; index < indexes.Length; index++)
            {
                int originalIndex = indexes[index];
                alternatives[index] = question.Alternatives[originalIndex];
                if (originalIndex == question.CorrectAlternativeIndex)
                {
                    correctIndex = index;
                }
            }

            return new QuizRoundQuestion(
                question.Id,
                question.PlanetKey,
                question.Difficulty,
                question.Prompt,
                alternatives,
                correctIndex,
                question.Explanation);
        }
    }
}
