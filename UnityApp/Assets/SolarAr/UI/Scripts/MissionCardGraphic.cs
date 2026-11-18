using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class MissionCardGraphic : MaskableGraphic
    {
        [SerializeField] private Color _topColor = new(0.20f, 0.14f, 0.48f, 1f);
        [SerializeField] private Color _bottomColor = new(0.08f, 0.10f, 0.28f, 1f);
        [SerializeField] private Color _borderColor = new(0.55f, 0.35f, 1f, 1f);
        [SerializeField, Min(0f)] private float _radius = 28f;
        [SerializeField, Min(0f)] private float _borderWidth = 2f;
        [SerializeField, Range(2, 12)] private int _cornerSegments = 6;

        private readonly List<Vector2> _contour = new();

        public void Configure(
            Color topColor,
            Color bottomColor,
            Color borderColor,
            float radius = 28f,
            float borderWidth = 2f)
        {
            _topColor = topColor;
            _bottomColor = bottomColor;
            _borderColor = borderColor;
            _radius = radius;
            _borderWidth = borderWidth;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            Rect outerRect = GetPixelAdjustedRect();
            if (outerRect.width <= 0f || outerRect.height <= 0f)
            {
                return;
            }

            float radius = Mathf.Min(_radius, Mathf.Min(outerRect.width, outerRect.height) * 0.5f);
            AddRoundedFill(vertexHelper, outerRect, radius, _borderColor * color, _borderColor * color);

            float inset = Mathf.Min(
                _borderWidth,
                Mathf.Min(outerRect.width, outerRect.height) * 0.5f);
            Rect innerRect = new(
                outerRect.xMin + inset,
                outerRect.yMin + inset,
                outerRect.width - inset * 2f,
                outerRect.height - inset * 2f);
            AddRoundedFill(
                vertexHelper,
                innerRect,
                Mathf.Max(0f, radius - inset),
                _topColor * color,
                _bottomColor * color);
        }

        private void AddRoundedFill(
            VertexHelper vertexHelper,
            Rect rect,
            float radius,
            Color topColor,
            Color bottomColor)
        {
            BuildContour(rect, radius);
            int centerIndex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(rect.center, Color.Lerp(bottomColor, topColor, 0.5f), Vector2.zero);

            for (int index = 0; index < _contour.Count; index++)
            {
                Vector2 point = _contour[index];
                float normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, point.y);
                vertexHelper.AddVert(point, Color.Lerp(bottomColor, topColor, normalizedY), Vector2.zero);
            }

            for (int index = 0; index < _contour.Count; index++)
            {
                int next = (index + 1) % _contour.Count;
                vertexHelper.AddTriangle(centerIndex, centerIndex + index + 1, centerIndex + next + 1);
            }
        }

        private void BuildContour(Rect rect, float radius)
        {
            _contour.Clear();
            if (radius <= 0f)
            {
                _contour.Add(new Vector2(rect.xMin, rect.yMin));
                _contour.Add(new Vector2(rect.xMax, rect.yMin));
                _contour.Add(new Vector2(rect.xMax, rect.yMax));
                _contour.Add(new Vector2(rect.xMin, rect.yMax));
                return;
            }

            AddCorner(new Vector2(rect.xMin + radius, rect.yMin + radius), radius, 180f, 270f);
            AddCorner(new Vector2(rect.xMax - radius, rect.yMin + radius), radius, 270f, 360f);
            AddCorner(new Vector2(rect.xMax - radius, rect.yMax - radius), radius, 0f, 90f);
            AddCorner(new Vector2(rect.xMin + radius, rect.yMax - radius), radius, 90f, 180f);
        }

        private void AddCorner(
            Vector2 center,
            float radius,
            float startAngle,
            float endAngle)
        {
            for (int index = 0; index <= _cornerSegments; index++)
            {
                float t = index / (float)_cornerSegments;
                float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
                _contour.Add(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
            }
        }
    }
}
