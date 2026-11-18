using SolarAR.Games.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.Games.SpaceDetective
{
    public sealed class SpaceDetectiveScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject _gameRoot;
        [SerializeField] private GameObject _resultRoot;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _cluesText;
        [SerializeField] private TMP_Text _clueCountText;
        [SerializeField] private Button _moreClueButton;
        [SerializeField] private Button[] _answerButtons;
        [SerializeField] private TMP_Text[] _answerLabels;
        [SerializeField] private Graphic[] _answerBackgrounds;
        [SerializeField] private GameObject _feedbackPanel;
        [SerializeField] private TMP_Text _feedbackTitleText;
        [SerializeField] private TMP_Text _feedbackExplanationText;
        [SerializeField] private RawImage _revealedPlanetImage;
        [SerializeField] private Texture[] _planetTextures;
        [SerializeField] private Button _nextButton;
        [SerializeField] private TMP_Text _nextButtonLabel;
        [SerializeField] private TMP_Text _resultScoreText;
        [SerializeField] private TMP_Text[] _resultStars;

        private static readonly Color DefaultColor = new Color32(151, 228, 244, 255);
        private static readonly Color CorrectColor = new Color32(182, 233, 201, 255);
        private static readonly Color IncorrectColor = new Color32(255, 244, 205, 255);

        private SpaceDetectiveSession _session;

        public SpaceDetectiveSession Session => _session;

        public void SetPlanetTextures(Texture[] planetTextures)
        {
            _planetTextures = planetTextures;
        }

        public void OnGameOpened()
        {
            StartNewGame();
        }

        public void OnGameClosed()
        {
            _session = null;
        }

        public void StartNewGame()
        {
            _session = new SpaceDetectiveSession(
                SpaceDetectiveContent.CreateDefault(),
                new System.Random());
            SetActive(_gameRoot, true);
            SetActive(_resultRoot, false);
            ShowChallenge();
        }

        public void RevealNextClue()
        {
            if (_session != null && _session.RevealNextClue())
            {
                RenderClues();
            }
        }

        public void SelectAnswer(int answerIndex)
        {
            if (_session == null || answerIndex < 0 ||
                answerIndex >= _session.CurrentChallenge.Alternatives.Count)
            {
                return;
            }

            PlanetId answer = _session.CurrentChallenge.Alternatives[answerIndex];
            SpaceDetectiveAnswerResult result = _session.SubmitAnswer(answer);
            if (!result.Accepted)
            {
                return;
            }

            for (int index = 0; index < _answerButtons.Length; index++)
            {
                PlanetId option = _session.CurrentChallenge.Alternatives[index];
                _answerButtons[index].interactable = false;
                if (_answerBackgrounds != null && index < _answerBackgrounds.Length &&
                    _answerBackgrounds[index] != null)
                {
                    _answerBackgrounds[index].color = option == result.CorrectPlanet
                        ? CorrectColor
                        : option == answer ? IncorrectColor : DefaultColor;
                }
            }

            _moreClueButton.interactable = false;
            _feedbackTitleText.text = result.IsCorrect
                ? $"Correto! +{result.Points} pontos"
                : $"Quase! O planeta era {PlanetCatalog.GetDisplayName(result.CorrectPlanet)}.";
            _feedbackExplanationText.text = result.Explanation;
            if (_revealedPlanetImage != null)
            {
                int textureIndex = PlanetCatalog.GetIndex(result.CorrectPlanet);
                _revealedPlanetImage.texture =
                    _planetTextures != null && textureIndex < _planetTextures.Length
                        ? _planetTextures[textureIndex]
                        : null;
            }
            SetActive(_feedbackPanel, true);
            _nextButton.interactable = true;
            _nextButtonLabel.text = _session.ChallengeNumber == SpaceDetectiveSession.ChallengeCount
                ? "Ver resultado"
                : "Próximo desafio";
            RenderScore();
        }

        public void NextChallenge()
        {
            if (_session == null || !_session.Advance())
            {
                return;
            }

            if (_session.IsComplete)
            {
                ShowResult();
            }
            else
            {
                ShowChallenge();
            }
        }

        private void ShowChallenge()
        {
            SetActive(_feedbackPanel, false);
            _progressText.text =
                $"Desafio {_session.ChallengeNumber}/{SpaceDetectiveSession.ChallengeCount}";
            for (int index = 0; index < _answerButtons.Length; index++)
            {
                _answerLabels[index].text = PlanetCatalog.GetDisplayName(
                    _session.CurrentChallenge.Alternatives[index]);
                _answerButtons[index].interactable = true;
                if (_answerBackgrounds != null && index < _answerBackgrounds.Length &&
                    _answerBackgrounds[index] != null)
                {
                    _answerBackgrounds[index].color = DefaultColor;
                }
            }

            _nextButton.interactable = false;
            RenderClues();
            RenderScore();
        }

        private void RenderClues()
        {
            SpaceDetectiveChallenge challenge = _session.CurrentChallenge;
            string clues = string.Empty;
            for (int index = 0; index < challenge.CluesShown; index++)
            {
                if (index > 0)
                {
                    clues += "\n";
                }

                clues += $"{index + 1}. {challenge.Planet.Clues[index]}";
            }

            _cluesText.text = clues;
            _clueCountText.text = $"Pistas: {challenge.CluesShown}/{SpaceDetectiveSession.MaximumClues}";
            _moreClueButton.interactable =
                !challenge.HasAnswered && challenge.CluesShown < SpaceDetectiveSession.MaximumClues;
        }

        private void RenderScore()
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Pontos: {_session.Score}/50";
            }
        }

        private void ShowResult()
        {
            SetActive(_gameRoot, false);
            SetActive(_resultRoot, true);
            _resultScoreText.text = $"{_session.Score}/50 pontos";
            int stars = SpaceDetectiveSession.CalculateStars(_session.Score);
            for (int index = 0; _resultStars != null && index < _resultStars.Length; index++)
            {
                _resultStars[index].color = index < stars
                    ? new Color32(255, 207, 90, 255)
                    : new Color32(255, 255, 255, 70);
            }
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
