using System;
using System.Collections.Generic;

namespace SolarAR.Games.Quiz
{
    public sealed class QuizSession
    {
        private IReadOnlyList<QuizRoundQuestion> _questions = Array.Empty<QuizRoundQuestion>();
        private int _currentQuestionIndex;
        private int _correctAnswers;
        private bool _hasAnsweredCurrent;
        private bool _isComplete;

        public QuizSession(IReadOnlyList<QuizRoundQuestion> questions)
        {
            Restart(questions);
        }

        public IReadOnlyList<QuizRoundQuestion> Questions => _questions;
        public int CurrentQuestionIndex => _currentQuestionIndex;
        public int QuestionNumber => _questions.Count == 0 ? 0 : _currentQuestionIndex + 1;
        public int QuestionCount => _questions.Count;
        public int CorrectAnswers => _correctAnswers;
        public int Score => QuizScoreCalculator.CalculateScore(_correctAnswers);
        public bool HasAnsweredCurrent => _hasAnsweredCurrent;
        public bool IsComplete => _isComplete;
        public QuizRoundQuestion CurrentQuestion =>
            _questions.Count == 0 || _isComplete ? null : _questions[_currentQuestionIndex];

        public void Restart(IReadOnlyList<QuizRoundQuestion> questions)
        {
            _questions = questions ?? Array.Empty<QuizRoundQuestion>();
            _currentQuestionIndex = 0;
            _correctAnswers = 0;
            _hasAnsweredCurrent = false;
            _isComplete = false;
        }

        public bool TrySubmitAnswer(int alternativeIndex, out QuizAnswerResult result)
        {
            QuizRoundQuestion question = CurrentQuestion;
            if (question == null || _hasAnsweredCurrent ||
                alternativeIndex < 0 || alternativeIndex >= question.Alternatives.Count)
            {
                result = default;
                return false;
            }

            _hasAnsweredCurrent = true;
            bool isCorrect = alternativeIndex == question.CorrectAlternativeIndex;
            if (isCorrect)
            {
                _correctAnswers++;
            }

            result = new QuizAnswerResult(
                isCorrect,
                alternativeIndex,
                question.CorrectAlternativeIndex,
                question.CorrectAlternative,
                question.Explanation);
            return true;
        }

        public bool TryAdvance()
        {
            if (_isComplete || !_hasAnsweredCurrent || _questions.Count == 0)
            {
                return false;
            }

            if (_currentQuestionIndex >= _questions.Count - 1)
            {
                _isComplete = true;
                return true;
            }

            _currentQuestionIndex++;
            _hasAnsweredCurrent = false;
            return true;
        }
    }
}
