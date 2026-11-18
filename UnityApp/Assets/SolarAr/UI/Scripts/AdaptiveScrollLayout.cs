using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.UI
{
    public sealed class AdaptiveScrollLayout : MonoBehaviour
    {
        private const float BottomNavigationInset = 84f;
        private const float LandscapeContentWidth = 412f;
        private const float ScrollThreshold = 16f;

        [SerializeField] private RectTransform _root;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;
        [SerializeField] private Graphic _contentDragSurface;
        [SerializeField] private Graphic _edgeDragSurface;
        [SerializeField, Min(1f)] private float _baselineHeight = 835f;
        [SerializeField] private bool _reserveBottomNavigation;
        [SerializeField] private bool _edgeScrollOnly;

        private Vector2 _lastRootSize = new(-1f, -1f);

        public void Configure(
            RectTransform root,
            ScrollRect scrollRect,
            RectTransform viewport,
            RectTransform content,
            Graphic contentDragSurface,
            Graphic edgeDragSurface,
            float baselineHeight,
            bool reserveBottomNavigation,
            bool edgeScrollOnly)
        {
            _root = root;
            _scrollRect = scrollRect;
            _viewport = viewport;
            _content = content;
            _contentDragSurface = contentDragSurface;
            _edgeDragSurface = edgeDragSurface;
            _baselineHeight = Mathf.Max(1f, baselineHeight);
            _reserveBottomNavigation = reserveBottomNavigation;
            _edgeScrollOnly = edgeScrollOnly;
            RefreshLayout();
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
            if (_root == null || _scrollRect == null || _viewport == null || _content == null)
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

            bool sizeChanged = Vector2.SqrMagnitude(rootSize - _lastRootSize) >= 0.25f;
            _lastRootSize = rootSize;

            float bottomInset = _reserveBottomNavigation ? BottomNavigationInset : 0f;
            SetStretch(_scrollRect.GetComponent<RectTransform>(), 0f, bottomInset, 0f, 0f);
            SetStretch(_viewport, 0f, 0f, 0f, 0f);

            float viewportHeight = Mathf.Max(1f, rootSize.y - bottomInset);
            bool landscape = rootSize.x > rootSize.y * 1.15f;
            float contentWidth = landscape
                ? Mathf.Min(LandscapeContentWidth, Mathf.Max(280f, rootSize.x - 64f))
                : rootSize.x;
            float contentHeight = Mathf.Max(viewportHeight, _baselineHeight);
            bool needsScroll = contentHeight > viewportHeight + ScrollThreshold;

            _content.anchorMin = _content.anchorMax = new Vector2(0.5f, 1f);
            _content.pivot = new Vector2(0.5f, 1f);
            _content.sizeDelta = new Vector2(contentWidth, contentHeight);
            if (sizeChanged || force)
            {
                _content.anchoredPosition = Vector2.zero;
            }

            _scrollRect.horizontal = false;
            _scrollRect.vertical = needsScroll;
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;
            _scrollRect.inertia = true;
            _scrollRect.decelerationRate = 0.16f;

            bool useEdgeSurface = needsScroll && _edgeScrollOnly;
            if (_contentDragSurface != null)
            {
                _contentDragSurface.raycastTarget = needsScroll && !useEdgeSurface;
            }

            if (_edgeDragSurface != null)
            {
                _edgeDragSurface.gameObject.SetActive(useEdgeSurface);
                _edgeDragSurface.raycastTarget = useEdgeSurface;
            }
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
    }
}
