using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class RoundedRectangleGraphic : MaskableGraphic
    {
        [SerializeField, Min(0f)] private float _radius = 24f;
        [SerializeField, Range(2, 16)] private int _cornerSegments = 8;

        public float Radius
        {
            get => _radius;
            set
            {
                _radius = Mathf.Max(0f, value);
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            Rect rect = GetPixelAdjustedRect();
            float radius = Mathf.Min(_radius, Mathf.Min(rect.width, rect.height) * 0.5f);

            AddQuad(
                vertexHelper,
                new Rect(rect.xMin + radius, rect.yMin, rect.width - radius * 2f, rect.height));
            AddQuad(
                vertexHelper,
                new Rect(rect.xMin, rect.yMin + radius, radius, rect.height - radius * 2f));
            AddQuad(
                vertexHelper,
                new Rect(rect.xMax - radius, rect.yMin + radius, radius, rect.height - radius * 2f));

            AddCorner(vertexHelper, new Vector2(rect.xMin + radius, rect.yMin + radius), radius, 180f, 270f);
            AddCorner(vertexHelper, new Vector2(rect.xMax - radius, rect.yMin + radius), radius, 270f, 360f);
            AddCorner(vertexHelper, new Vector2(rect.xMax - radius, rect.yMax - radius), radius, 0f, 90f);
            AddCorner(vertexHelper, new Vector2(rect.xMin + radius, rect.yMax - radius), radius, 90f, 180f);
        }

        private void AddCorner(
            VertexHelper vertexHelper,
            Vector2 center,
            float radius,
            float startAngle,
            float endAngle)
        {
            int centerIndex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(center, color, Vector2.zero);

            for (int index = 0; index <= _cornerSegments; index++)
            {
                float angle = Mathf.Lerp(startAngle, endAngle, index / (float)_cornerSegments) * Mathf.Deg2Rad;
                Vector2 position = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                vertexHelper.AddVert(position, color, Vector2.zero);
            }

            for (int index = 0; index < _cornerSegments; index++)
            {
                vertexHelper.AddTriangle(centerIndex, centerIndex + index + 1, centerIndex + index + 2);
            }
        }

        private void AddQuad(VertexHelper vertexHelper, Rect rect)
        {
            if (rect.width <= 0f || rect.height <= 0f)
            {
                return;
            }

            int startIndex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(new Vector2(rect.xMin, rect.yMin), color, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMin, rect.yMax), color, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMax, rect.yMax), color, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMax, rect.yMin), color, Vector2.zero);
            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
        }
    }
}
