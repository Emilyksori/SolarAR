using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.Games.Quiz
{
    public sealed class QuizScreenController : MonoBehaviour
    {
        [Header("Game")]
        [SerializeField] private QuizManager _quizManager;

        [Header("States")]
        [SerializeField] private GameObject _introRoot;
        [SerializeField] private GameObject _questionRoot;
        [SerializeField] private GameObject _resultRoot;

        [Header("Introduction")]
        [SerializeField] private TMP_Text _introMessageText;
        [SerializeField] private TMP_Text _progressSummaryText;

        [Header("Question")]
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private Image _progressFill;
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private QuizAnswerButton[] _answerButtons;
        [SerializeField] private GameObject _feedbackPanel;
        [SerializeField] private TMP_Text _feedbackTitleText;
        [SerializeField] private TMP_Text _feedbackExplanationText;
        [SerializeField] private Button _nextButton;

        [Header("Result")]
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _correctAnswersText;
        [SerializeField] private QuizStarGraphic[] _stars;
        [SerializeField] private TMP_Text _resultMessageText;

        public void OnTabOpened()
        {
            _quizManager?.CancelCurrentGame();
            ShowIntro();
        }

        public void OnTabClosed()
        {
            _quizManager?.CancelCurrentGame();
            ShowIntro();
        }

        public void StartMission()
        {
            if (_quizManager == null || !_quizManager.StartNewGame())
            {
                if (_introMessageText != null)
                {
                    _introMessageText.text =
                        "A missão está se preparando. Tente novamente em alguns instantes.";
                }

                return;
            }

            ShowQuestion();
        }

        public void SelectAnswer(int answerIndex)
        {
            if (_quizManager == null ||
                !_quizManager.TrySubmitAnswer(answerIndex, out QuizAnswerResult result))
            {
                return;
            }

            for (int index = 0; index < _answerButtons.Length; index++)
            {
                _answerButtons[index].ShowResult(
                    index == result.CorrectAlternativeIndex,
                    index == result.SelectedAlternativeIndex,
                    new Color32(182, 233, 201, 255),
                    new Color32(255, 244, 205, 255));
            }

            if (_feedbackTitleText != null)
            {
                _feedbackTitleText.text = result.IsCorrect
                    ? "Muito bem! Resposta correta."
                    : $"Quase! A resposta correta é: {result.CorrectAlternative}";
            }

            if (_feedbackExplanationText != null)
            {
                _feedbackExplanationText.text = result.Explanation;
            }

            SetActive(_feedbackPanel, true);
            if (_nextButton != null)
            {
                _nextButton.interactable = true;
            }
        }

        public void NextQuestion()
        {
            if (_quizManager == null || !_quizManager.TryAdvance())
            {
                return;
            }

            if (_quizManager.Session.IsComplete)
            {
                ShowResult();
            }
            else
            {
                ShowQuestion();
            }
        }

        public void Replay()
        {
            StartMission();
        }

        private void ShowIntro()
        {
            SetActive(_introRoot, true);
            SetActive(_questionRoot, false);
            SetActive(_resultRoot, false);
            if (_introMessageText != null)
            {
                _introMessageText.text =
                    "Responda cinco perguntas e descubra quanto você conhece sobre os planetas!";
            }

            if (_progressSummaryText != null && _quizManager != null)
            {
                _progressSummaryText.text =
                    $"Melhor pontuação: {_quizManager.BestScore}/50  •  " +
                    $"Missões concluídas: {_quizManager.CompletedGames}";
            }
        }

        private void ShowQuestion()
        {
            QuizSession session = _quizManager?.Session;
            QuizRoundQuestion question = session?.CurrentQuestion;
            if (question == null || _answerButtons == null || _answerButtons.Length != 3)
            {
                Debug.LogError("[Quiz] Question UI is incomplete.", this);
                ShowIntro();
                return;
            }

            SetActive(_introRoot, false);
            SetActive(_questionRoot, true);
            SetActive(_resultRoot, false);
            SetActive(_feedbackPanel, false);

            _progressText.text = $"Pergunta {session.QuestionNumber}/{session.QuestionCount}";
            _progressFill.fillAmount = session.QuestionNumber / (float)session.QuestionCount;
            _questionText.text = question.Prompt;
            for (int index = 0; index < _answerButtons.Length; index++)
            {
                _answerButtons[index].Prepare(question.Alternatives[index]);
            }

            if (_nextButton != null)
            {
                _nextButton.interactable = false;
            }
        }

        private void ShowResult()
        {
            QuizSession session = _quizManager?.Session;
            if (session == null || !session.IsComplete)
            {
                ShowIntro();
                return;
            }

            SetActive(_introRoot, false);
            SetActive(_questionRoot, false);
            SetActive(_resultRoot, true);

            int stars = QuizScoreCalculator.CalculateStars(session.CorrectAnswers);
            _scoreText.text = $"{session.Score}/50 pontos";
            _correctAnswersText.text =
                $"Você acertou {session.CorrectAnswers} de {session.QuestionCount} perguntas.";
            if (_stars != null)
            {
                for (int index = 0; index < _stars.Length; index++)
                {
                    if (_stars[index] != null)
                    {
                        _stars[index].color = index < stars
                            ? new Color32(255, 207, 90, 255)
                            : new Color32(255, 255, 255, 70);
                    }
                }
            }
            _resultMessageText.text = stars switch
            {
                3 => "Incrível! Você completou a missão como um grande explorador!",
                2 => "Muito bem! Cada descoberta deixa sua viagem ainda melhor!",
                _ => "Boa tentativa! A Lua está pronta para explorar de novo com você!"
            };
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null && target.activeSelf != active)
            {
                target.SetActive(active);
            }
        }
    }
}
