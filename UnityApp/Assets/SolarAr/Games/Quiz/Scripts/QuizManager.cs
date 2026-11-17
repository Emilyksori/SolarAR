using UnityEngine;

namespace SolarAR.Games.Quiz
{
    public sealed class QuizManager : MonoBehaviour
    {
        public const int QuestionsPerGame = 5;

        [SerializeField] private QuizQuestionBank _questionBank;

        private readonly QuizProgressStorage _progressStorage = new();
        private QuizSession _session;
        private bool _completionRecorded;

        public QuizSession Session => _session;
        public int BestScore => _progressStorage.BestScore;
        public int CompletedGames => _progressStorage.CompletedGames;

        public bool StartNewGame()
        {
            if (_questionBank == null)
            {
                Debug.LogError("[Quiz] QuizQuestionBank is not assigned.", this);
                _session = null;
                return false;
            }

            QuizQuestionSelector selector = new(new System.Random());
            var questions = selector.Select(
                _questionBank.Questions,
                QuestionsPerGame,
                out string warning);
            if (!string.IsNullOrEmpty(warning))
            {
                Debug.LogWarning($"[Quiz] {warning}", _questionBank);
            }

            if (questions.Count != QuestionsPerGame)
            {
                Debug.LogError(
                    $"[Quiz] A game requires {QuestionsPerGame} valid questions; " +
                    $"the bank provided {questions.Count}.",
                    _questionBank);
                _session = null;
                return false;
            }

            _session = new QuizSession(questions);
            _completionRecorded = false;
            return true;
        }

        public bool TrySubmitAnswer(int alternativeIndex, out QuizAnswerResult result)
        {
            if (_session == null)
            {
                result = default;
                return false;
            }

            return _session.TrySubmitAnswer(alternativeIndex, out result);
        }

        public bool TryAdvance()
        {
            if (_session == null || !_session.TryAdvance())
            {
                return false;
            }

            if (_session.IsComplete && !_completionRecorded)
            {
                _progressStorage.RecordCompletedGame(_session.Score);
                _completionRecorded = true;
            }

            return true;
        }

        public void CancelCurrentGame()
        {
            _session = null;
            _completionRecorded = false;
        }
    }
}
