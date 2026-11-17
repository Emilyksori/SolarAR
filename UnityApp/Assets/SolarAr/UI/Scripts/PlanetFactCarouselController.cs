using System.Collections;
using SolarAR.Education;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SolarAR.UI
{
    public sealed class PlanetFactCarouselController : MonoBehaviour,
        IBeginDragHandler,
        IEndDragHandler
    {
        [SerializeField] private CanvasGroup _cardCanvasGroup;
        [SerializeField] private Graphic _accent;
        [SerializeField] private TMP_Text _categoryText;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;
        [SerializeField, Min(24f)] private float _swipeThreshold = 48f;
        [SerializeField, Min(0f)] private float _transitionSeconds = 0.18f;

        private readonly PlanetFactCarousel _carousel = new();
        private Coroutine _transition;
        private float _dragStartX;

        public PlanetFactCarousel Carousel => _carousel;

        public void ShowPlanet(PlanetFacts facts)
        {
            _carousel.SetFacts(facts?.Facts);
            RenderCurrent();
        }

        public void SetPlanetPalette(string planetKey)
        {
            if (_accent == null)
            {
                return;
            }

            SolarARColors.GetPlanetPalette(planetKey, out Color primary, out _);
            _accent.color = primary;
        }

        public void ShowPrevious()
        {
            if (_carousel.MovePrevious())
            {
                RenderCurrentWithTransition();
            }
        }

        public void ShowNext()
        {
            if (_carousel.MoveNext())
            {
                RenderCurrentWithTransition();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragStartX = eventData.position.x;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            float delta = eventData.position.x - _dragStartX;
            if (Mathf.Abs(delta) < _swipeThreshold)
            {
                return;
            }

            if (delta < 0f)
            {
                ShowNext();
            }
            else
            {
                ShowPrevious();
            }
        }

        private void RenderCurrentWithTransition()
        {
            RenderCurrent();
            if (_cardCanvasGroup == null || _transitionSeconds <= 0f)
            {
                return;
            }

            if (_transition != null)
            {
                StopCoroutine(_transition);
            }

            _transition = StartCoroutine(FadeCardIn());
        }

        private void RenderCurrent()
        {
            PlanetFact fact = _carousel.Current;
            if (_categoryText != null)
            {
                _categoryText.text = fact?.CategoryLabel ?? "Curiosidade";
            }

            if (_titleText != null)
            {
                _titleText.text = fact?.Title ?? "Conteúdo indisponível";
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = fact?.Description ?? string.Empty;
            }

            if (_progressText != null)
            {
                _progressText.text = _carousel.ProgressText;
            }

            if (_previousButton != null)
            {
                _previousButton.interactable = _carousel.CanMovePrevious;
            }

            if (_nextButton != null)
            {
                _nextButton.interactable = _carousel.CanMoveNext;
            }
        }

        private IEnumerator FadeCardIn()
        {
            _cardCanvasGroup.alpha = 0.55f;
            float elapsed = 0f;
            while (elapsed < _transitionSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                _cardCanvasGroup.alpha = Mathf.Lerp(
                    0.55f,
                    1f,
                    Mathf.Clamp01(elapsed / _transitionSeconds));
                yield return null;
            }

            _cardCanvasGroup.alpha = 1f;
            _transition = null;
        }
    }
}
