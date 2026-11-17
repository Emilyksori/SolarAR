namespace SolarAR.Games.Quiz
{
    public readonly struct QuizAnswerResult
    {
        public QuizAnswerResult(
            bool isCorrect,
            int selectedAlternativeIndex,
            int correctAlternativeIndex,
            string correctAlternative,
            string explanation)
        {
            IsCorrect = isCorrect;
            SelectedAlternativeIndex = selectedAlternativeIndex;
            CorrectAlternativeIndex = correctAlternativeIndex;
            CorrectAlternative = correctAlternative;
            Explanation = explanation;
        }

        public bool IsCorrect { get; }
        public int SelectedAlternativeIndex { get; }
        public int CorrectAlternativeIndex { get; }
        public string CorrectAlternative { get; }
        public string Explanation { get; }
    }
}
