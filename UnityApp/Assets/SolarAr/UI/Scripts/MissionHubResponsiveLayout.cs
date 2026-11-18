using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.UI
{
    public sealed class MissionHubResponsiveLayout : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _header;
        [SerializeField] private RectTransform _backButton;
        [SerializeField] private RectTransform _rocket;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _subtitleText;
        [SerializeField] private RectTransform _accentLine;

        [Header("Cards")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform[] _cards;
        [SerializeField] private RectTransform[] _illustrations;
        [SerializeField] private TMP_Text[] _cardTitles;
        [SerializeField] private TMP_Text[] _cardDescriptions;
        [SerializeField] private RectTransform[] _arrows;

        private Vector2 _lastRootSize = new(-1f, -1f);

        public void Configure(
            RectTransform root,
            RectTransform header,
            RectTransform backButton,
            RectTransform rocket,
            TMP_Text titleText,
            TMP_Text subtitleText,
            RectTransform accentLine,
            ScrollRect scrollRect,
            RectTransform viewport,
            RectTransform content,
            RectTransform[] cards,
            RectTransform[] illustrations,
            TMP_Text[] cardTitles,
            TMP_Text[] cardDescriptions,
            RectTransform[] arrows)
        {
            _root = root;
            _header = header;
            _backButton = backButton;
            _rocket = rocket;
            _titleText = titleText;
            _subtitleText = subtitleText;
            _accentLine = accentLine;
            _scrollRect = scrollRect;
            _viewport = viewport;
            _content = content;
            _cards = cards;
            _illustrations = illustrations;
            _cardTitles = cardTitles;
            _cardDescriptions = cardDescriptions;
            _arrows = arrows;
            ApplyLayout(true);
        }

        public void RefreshLayout()
        {
            ApplyLayout(true);
        }

        private void OnEnable()
        {
            ApplyLayout(true);
        }

        private void LateUpdate()
        {
            ApplyLayout(false);
        }

        private void ApplyLayout(bool force)
        {
            if (_root == null || _cards == null || _cards.Length != 4)
            {
                return;
            }

            Vector2 rootSize = _root.rect.size;
            if (rootSize.x <= 0f || rootSize.y <= 0f)
            {
                return;
            }

            if (!force && Vector2.SqrMagnitude(rootSize - _lastRootSize) < 0.25f)
            {
                return;
            }

            _lastRootSize = rootSize;
            bool landscape = rootSize.x > rootSize.y * 1.15f;
            if (landscape)
            {
                LayoutLandscape(rootSize);
            }
            else
            {
                LayoutPortrait(rootSize);
            }
        }

        private void LayoutPortrait(Vector2 rootSize)
        {
            const float headerHeight = 154f;
            const float bottomNavigationClearance = 90f;
            const float cardHeight = 148f;
            const float gap = 12f;
            const float contentPadding = 4f;

            SetTopStretch(_header, 0f, headerHeight, 0f, 0f);
            PlaceTopLeft(_backButton, 14f, 14f, 52f, 52f);
            PlaceTopCenter(_rocket, -129f, 21f, 28f, 28f);
            PlaceTopCenter(_titleText.rectTransform, 25f, 12f, 282f, 48f);
            _titleText.fontSize = 28f;
            PlaceTopStretch(_subtitleText.rectTransform, 65f, 46f, 34f, 34f);
            _subtitleText.fontSize = 16f;
            PlaceTopCenter(_accentLine, 0f, 123f, 70f, 5f);

            SetStretch(
                _scrollRect.GetComponent<RectTransform>(),
                0f,
                bottomNavigationClearance,
                0f,
                headerHeight);
            SetStretch(_viewport, 0f, 0f, 0f, 0f);
            _scrollRect.vertical = true;
            _scrollRect.horizontal = false;
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;

            float totalHeight = contentPadding * 2f + cardHeight * 4f + gap * 3f;
            _content.anchorMin = new Vector2(0f, 1f);
            _content.anchorMax = new Vector2(1f, 1f);
            _content.pivot = new Vector2(0.5f, 1f);
            _content.anchoredPosition = Vector2.zero;
            _content.sizeDelta = new Vector2(0f, totalHeight);

            float cardWidth = Mathf.Min(rootSize.x - 28f, 440f);
            for (int index = 0; index < _cards.Length; index++)
            {
                PlaceTopCenter(
                    _cards[index],
                    0f,
                    contentPadding + index * (cardHeight + gap),
                    cardWidth,
                    cardHeight);
                LayoutCard(index, cardWidth, cardHeight, false);
            }
        }

        private void LayoutLandscape(Vector2 rootSize)
        {
            const float headerHeight = 126f;
            const float bottomNavigationClearance = 78f;
            const float horizontalPadding = 24f;
            const float columnGap = 18f;
            const float rowGap = 12f;

            SetTopStretch(_header, 0f, headerHeight, 0f, 0f);
            PlaceTopLeft(_backButton, 18f, 16f, 54f, 54f);
            PlaceTopCenter(_rocket, -218f, 12f, 50f, 50f);
            PlaceTopCenter(_titleText.rectTransform, 18f, 6f, 430f, 56f);
            _titleText.fontSize = 40f;
            PlaceTopStretch(_subtitleText.rectTransform, 61f, 36f, 92f, 92f);
            _subtitleText.fontSize = 17f;
            PlaceTopCenter(_accentLine, 0f, 104f, 82f, 5f);

            SetStretch(
                _scrollRect.GetComponent<RectTransform>(),
                0f,
                bottomNavigationClearance,
                0f,
                headerHeight);
            SetStretch(_viewport, 0f, 0f, 0f, 0f);
            _scrollRect.vertical = false;
            _scrollRect.horizontal = false;
            _content.anchorMin = Vector2.zero;
            _content.anchorMax = Vector2.one;
            _content.pivot = new Vector2(0.5f, 0.5f);
            _content.anchoredPosition = Vector2.zero;
            _content.sizeDelta = Vector2.zero;

            float availableWidth = rootSize.x - horizontalPadding * 2f;
            float availableHeight = rootSize.y - headerHeight - bottomNavigationClearance;
            float cardWidth = (availableWidth - columnGap) * 0.5f;
            float cardHeight = (availableHeight - rowGap) * 0.5f;

            for (int index = 0; index < _cards.Length; index++)
            {
                int column = index % 2;
                int row = index / 2;
                float x = (column == 0 ? -1f : 1f) * (cardWidth + columnGap) * 0.5f;
                float y = (row == 0 ? 1f : -1f) * (cardHeight + rowGap) * 0.5f;
                PlaceCenter(_cards[index], x, y, cardWidth, cardHeight);
                LayoutCard(index, cardWidth, cardHeight, true);
            }
        }

        private void LayoutCard(int index, float cardWidth, float cardHeight, bool landscape)
        {
            if (index >= _illustrations.Length ||
                index >= _cardTitles.Length ||
                index >= _cardDescriptions.Length ||
                index >= _arrows.Length)
            {
                return;
            }

            float illustrationSize = Mathf.Min(
                landscape ? 112f : 100f,
                Mathf.Max(76f, cardHeight - 24f));
            float illustrationLeft = landscape ? 17f : 14f;
            PlaceLeftCenter(
                _illustrations[index],
                illustrationLeft,
                0f,
                illustrationSize,
                illustrationSize);

            float arrowSize = Mathf.Clamp(cardHeight * 0.34f, 44f, 52f);
            PlaceRightCenter(_arrows[index], 15f, 0f, arrowSize, arrowSize);

            float textLeft = illustrationLeft + illustrationSize + (landscape ? 19f : 14f);
            float textRight = arrowSize + 28f;
            float titleTop = Mathf.Max(17f, cardHeight * 0.18f);
            float titleHeight = Mathf.Clamp(cardHeight * 0.30f, 34f, 48f);
            PlaceTopStretch(
                _cardTitles[index].rectTransform,
                titleTop,
                titleHeight,
                textLeft,
                textRight);
            _cardTitles[index].fontSize = landscape
                ? Mathf.Clamp(cardHeight * 0.17f, 19f, 25f)
                : 20f;

            float descriptionTop = titleTop + titleHeight + 2f;
            PlaceTopStretch(
                _cardDescriptions[index].rectTransform,
                descriptionTop,
                Mathf.Max(38f, cardHeight - descriptionTop - 14f),
                textLeft,
                textRight);
            _cardDescriptions[index].fontSize = landscape
                ? Mathf.Clamp(cardHeight * 0.105f, 13f, 16f)
                : 14f;
        }

        private static void SetStretch(
            RectTransform rect,
            float left,
            float bottom,
            float right,
            float top)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        private static void SetTopStretch(
            RectTransform rect,
            float top,
            float height,
            float left,
            float right)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2((left - right) * 0.5f, -top);
            rect.sizeDelta = new Vector2(-(left + right), height);
        }

        private static void PlaceTopStretch(
            RectTransform rect,
            float top,
            float height,
            float left,
            float right)
        {
            SetTopStretch(rect, top, height, left, right);
        }

        private static void PlaceTopLeft(
            RectTransform rect,
            float left,
            float top,
            float width,
            float height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(left, -top);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void PlaceTopCenter(
            RectTransform rect,
            float x,
            float top,
            float width,
            float height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(x, -top);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void PlaceCenter(
            RectTransform rect,
            float x,
            float y,
            float width,
            float height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void PlaceLeftCenter(
            RectTransform rect,
            float left,
            float y,
            float width,
            float height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(left, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void PlaceRightCenter(
            RectTransform rect,
            float right,
            float y,
            float width,
            float height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-right, y);
            rect.sizeDelta = new Vector2(width, height);
        }
    }
}
