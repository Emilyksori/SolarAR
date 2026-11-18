using SolarAR.Games.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.Games.PlanetOrder
{
    public sealed class PlanetOrderScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject _gameRoot;
        [SerializeField] private GameObject _resultRoot;
        [SerializeField] private Button[] _positionButtons;
        [SerializeField] private Graphic[] _positionBackgrounds;
        [SerializeField] private RawImage[] _planetImages;
        [SerializeField] private TMP_Text[] _planetLabels;
        [SerializeField] private Texture[] _planetTextures;
        [SerializeField] private TMP_Text _checksText;
        [SerializeField] private TMP_Text _feedbackText;
        [SerializeField] private TMP_Text _resultChecksText;
        [SerializeField] private TMP_Text[] _resultStars;

        private static readonly Color DefaultColor = new Color32(247, 250, 255, 255);
        private static readonly Color SelectedColor = new Color32(151, 228, 244, 255);
        private static readonly Color CorrectColor = new Color32(182, 233, 201, 255);
        private static readonly Color IncorrectColor = new Color32(255, 244, 205, 255);

        private PlanetOrderSession _session;
        private int _selectedPosition = -1;
        private PlanetOrderCheckResult _lastCheck;

        public PlanetOrderSession Session => _session;

        public void SetPlanetTextures(Texture[] planetTextures)
        {
            _planetTextures = planetTextures;
            if (_session != null)
            {
                RenderOrder();
            }
        }

        public void OnGameOpened()
        {
            StartNewGame();
        }

        public void OnGameClosed()
        {
            _session = null;
            _selectedPosition = -1;
            _lastCheck = null;
        }

        public void StartNewGame()
        {
            _session = new PlanetOrderSession(new System.Random());
            _selectedPosition = -1;
            _lastCheck = null;
            SetActive(_gameRoot, true);
            SetActive(_resultRoot, false);
            if (_feedbackText != null)
            {
                _feedbackText.text = "Selecione um planeta e depois a posição de destino.";
            }

            RenderOrder();
        }

        public void SelectPosition(int positionIndex)
        {
            if (_session == null || _session.IsComplete ||
                positionIndex < 0 || positionIndex >= _session.CurrentOrder.Count)
            {
                return;
            }

            _lastCheck = null;
            if (_selectedPosition < 0)
            {
                _selectedPosition = positionIndex;
                if (_feedbackText != null)
                {
                    _feedbackText.text =
                        $"{PlanetCatalog.GetDisplayName(_session.CurrentOrder[positionIndex])} selecionado. Agora escolha o destino.";
                }
            }
            else if (_selectedPosition == positionIndex)
            {
                _selectedPosition = -1;
                if (_feedbackText != null)
                {
                    _feedbackText.text = "Seleção cancelada. Escolha outro planeta.";
                }
            }
            else
            {
                _session.SwapPositions(_selectedPosition, positionIndex);
                _selectedPosition = -1;
                if (_feedbackText != null)
                {
                    _feedbackText.text = "Posições trocadas. Você pode conferir quando quiser.";
                }
            }

            RenderOrder();
        }

        public void CheckOrder()
        {
            if (_session == null || _session.IsComplete)
            {
                return;
            }

            _selectedPosition = -1;
            _lastCheck = _session.CheckOrder();
            RenderOrder();
            if (_lastCheck.IsComplete)
            {
                ShowResult();
                return;
            }

            int correctCount = 0;
            for (int index = 0; index < _lastCheck.CorrectPositions.Count; index++)
            {
                if (_lastCheck.CorrectPositions[index])
                {
                    correctCount++;
                }
            }

            if (_feedbackText != null)
            {
                _feedbackText.text =
                    $"{correctCount} de 7 posições estão corretas. Ajuste as amarelas e tente de novo!";
            }
        }

        private void RenderOrder()
        {
            if (_session == null)
            {
                return;
            }

            for (int index = 0; index < _session.CurrentOrder.Count; index++)
            {
                PlanetId planet = _session.CurrentOrder[index];
                if (_planetImages != null && index < _planetImages.Length && _planetImages[index] != null)
                {
                    int textureIndex = PlanetCatalog.GetIndex(planet);
                    _planetImages[index].texture = _planetTextures != null && textureIndex < _planetTextures.Length
                        ? _planetTextures[textureIndex]
                        : null;
                }

                if (_planetLabels != null && index < _planetLabels.Length && _planetLabels[index] != null)
                {
                    string positionFeedback = _lastCheck == null
                        ? $"{index + 1}."
                        : _lastCheck.CorrectPositions[index] ? "Certo:" : "Ajuste:";
                    _planetLabels[index].text =
                        $"{positionFeedback} {PlanetCatalog.GetDisplayName(planet)}";
                }

                if (_positionBackgrounds != null && index < _positionBackgrounds.Length &&
                    _positionBackgrounds[index] != null)
                {
                    _positionBackgrounds[index].color = GetPositionColor(index);
                }

                if (_positionButtons != null && index < _positionButtons.Length)
                {
                    _positionButtons[index].interactable = !_session.IsComplete;
                }
            }

            if (_checksText != null)
            {
                _checksText.text = $"Conferências: {_session.CheckCount}";
            }
        }

        private Color GetPositionColor(int index)
        {
            if (_selectedPosition == index)
            {
                return SelectedColor;
            }

            if (_lastCheck != null)
            {
                return _lastCheck.CorrectPositions[index] ? CorrectColor : IncorrectColor;
            }

            return DefaultColor;
        }

        private void ShowResult()
        {
            SetActive(_gameRoot, false);
            SetActive(_resultRoot, true);
            if (_resultChecksText != null)
            {
                _resultChecksText.text =
                    $"Ordem correta em {_session.CheckCount} conferência{(_session.CheckCount == 1 ? "" : "s")}!";
            }

            int stars = PlanetOrderSession.CalculateStars(_session.CheckCount);
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
