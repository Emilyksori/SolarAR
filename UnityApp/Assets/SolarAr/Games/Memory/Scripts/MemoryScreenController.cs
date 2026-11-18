using System;
using System.Collections;
using SolarAR.Games.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.Games.Memory
{
    public sealed class MemoryScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject _boardRoot;
        [SerializeField] private GameObject _resultRoot;
        [SerializeField] private Button[] _cardButtons;
        [SerializeField] private RawImage[] _cardImages;
        [SerializeField] private TMP_Text[] _cardLabels;
        [SerializeField] private Texture[] _planetTextures;
        [SerializeField] private TMP_Text _pairsText;
        [SerializeField] private TMP_Text _attemptsText;
        [SerializeField] private TMP_Text _resultAttemptsText;
        [SerializeField] private TMP_Text[] _resultStars;
        [SerializeField, Min(0f)] private float _mismatchDelaySeconds = 1f;

        private MemoryGameSession _session;
        private Coroutine _mismatchRoutine;

        public MemoryGameSession Session => _session;

        public void SetPlanetTextures(Texture[] planetTextures)
        {
            _planetTextures = planetTextures;
            if (_session != null)
            {
                RenderBoard();
            }
        }

        public void OnGameOpened()
        {
            StartNewGame();
        }

        public void OnGameClosed()
        {
            if (_mismatchRoutine != null)
            {
                StopCoroutine(_mismatchRoutine);
                _mismatchRoutine = null;
            }

            _session = null;
        }

        public void StartNewGame()
        {
            OnGameClosed();
            _session = new MemoryGameSession(new System.Random());
            SetActive(_boardRoot, true);
            SetActive(_resultRoot, false);
            RenderBoard();
        }

        public void SelectCard(int cardIndex)
        {
            if (_session == null)
            {
                return;
            }

            MemorySelectionResult result = _session.SelectCard(cardIndex);
            if (!result.IsValid)
            {
                return;
            }

            RenderBoard();
            if (result.Status == MemorySelectionStatus.Mismatch)
            {
                _mismatchRoutine = StartCoroutine(HideMismatchAfterDelay());
            }
            else if (result.Status == MemorySelectionStatus.Completed)
            {
                ShowResult();
            }
        }

        private IEnumerator HideMismatchAfterDelay()
        {
            yield return new WaitForSeconds(_mismatchDelaySeconds);
            _session?.ResolveMismatch();
            _mismatchRoutine = null;
            RenderBoard();
        }

        private void RenderBoard()
        {
            if (_session == null || _cardButtons == null)
            {
                return;
            }

            for (int index = 0; index < _session.Cards.Count && index < _cardButtons.Length; index++)
            {
                MemoryCardState card = _session.Cards[index];
                bool showFace = card.IsRevealed || card.IsMatched;
                if (_cardImages != null && index < _cardImages.Length && _cardImages[index] != null)
                {
                    _cardImages[index].texture = GetPlanetTexture(card.Planet);
                    _cardImages[index].gameObject.SetActive(showFace);
                }

                if (_cardLabels != null && index < _cardLabels.Length && _cardLabels[index] != null)
                {
                    _cardLabels[index].text = showFace
                        ? PlanetCatalog.GetDisplayName(card.Planet)
                        : "?";
                }

                _cardButtons[index].interactable = !_session.IsInteractionLocked && !showFace;
            }

            if (_pairsText != null)
            {
                _pairsText.text = $"Pares: {_session.MatchedPairs}/{MemoryGameSession.PairCount}";
            }

            if (_attemptsText != null)
            {
                _attemptsText.text = $"Tentativas: {_session.Attempts}";
            }
        }

        private Texture GetPlanetTexture(PlanetId planet)
        {
            int index = PlanetCatalog.GetIndex(planet);
            return _planetTextures != null && index < _planetTextures.Length
                ? _planetTextures[index]
                : null;
        }

        private void ShowResult()
        {
            SetActive(_boardRoot, false);
            SetActive(_resultRoot, true);
            if (_resultAttemptsText != null)
            {
                _resultAttemptsText.text = $"Você encontrou os 6 pares em {_session.Attempts} tentativas!";
            }

            int stars = MemoryGameSession.CalculateStars(_session.Attempts);
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
