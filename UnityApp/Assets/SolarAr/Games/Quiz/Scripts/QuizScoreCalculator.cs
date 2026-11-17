namespace SolarAR.Games.Quiz
{
    public static class QuizScoreCalculator
    {
        public const int PointsPerCorrectAnswer = 10;
        public const int MaximumScore = 50;

        public static int CalculateScore(int correctAnswers)
        {
            return correctAnswers <= 0 ? 0 : correctAnswers * PointsPerCorrectAnswer;
        }

        public static int CalculateStars(int correctAnswers)
        {
            if (correctAnswers >= 4)
            {
                return 3;
            }

            return correctAnswers >= 2 ? 2 : 1;
        }
    }
}
