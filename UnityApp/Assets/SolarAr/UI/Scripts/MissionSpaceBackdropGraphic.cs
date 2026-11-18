using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class MissionSpaceBackdropGraphic : MaskableGraphic
    {
        private static readonly Vector2[] StarPositions =
        {
            new(0.08f, 0.82f), new(0.16f, 0.66f), new(0.24f, 0.91f),
            new(0.38f, 0.76f), new(0.52f, 0.90f), new(0.67f, 0.79f),
            new(0.82f, 0.87f), new(0.93f, 0.69f), new(0.06f, 0.40f),
            new(0.19f, 0.28f), new(0.33f, 0.47f), new(0.58f, 0.35f),
            new(0.76f, 0.47f), new(0.90f, 0.30f), new(0.48f, 0.18f)
        };

        [SerializeField] private Color _topColor = new(0.055f, 0.045f, 0.20f, 1f);
        [SerializeField] private Color _bottomColor = new(0.035f, 0.055f, 0.18f, 1f);

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            Rect rect = GetPixelAdjustedRect();
            if (rect.width <= 0f || rect.height <= 0f)
            {
                return;
            }

            AddGradientQuad(vertexHelper, rect, _topColor * color, _bottomColor * color);

            AddCircle(
                vertexHelper,
                Point(rect, 1.02f, 1.03f),
                Mathf.Min(rect.width, rect.height) * 0.19f,
                new Color(0.36f, 0.16f, 0.78f, 0.46f) * color,
                28);
            AddCircle(
                vertexHelper,
                Point(rect, -0.02f, -0.02f),
                Mathf.Min(rect.width, rect.height) * 0.16f,
                new Color(0.20f, 0.12f, 0.58f, 0.38f) * color,
                26);
            AddEllipse(
                vertexHelper,
                Point(rect, 0.53f, -0.09f),
                new Vector2(rect.width * 0.34f, rect.height * 0.12f),
                new Color(0.31f, 0.18f, 0.72f, 0.34f) * color,
                32);

            for (int index = 0; index < StarPositions.Length; index++)
            {
                Vector2 position = Point(rect, StarPositions[index].x, StarPositions[index].y);
                float size = index % 5 == 0 ? 2.2f : 1.25f;
                Color starColor = index % 3 == 0
                    ? new Color(0.68f, 0.52f, 1f, 0.72f)
                    : new Color(0.92f, 0.95f, 1f, 0.68f);
                AddDiamond(vertexHelper, position, size, starColor * color);
            }
        }

        private static Vector2 Point(Rect rect, float x, float y)
        {
            return new Vector2(
                Mathf.Lerp(rect.xMin, rect.xMax, x),
                Mathf.Lerp(rect.yMin, rect.yMax, y));
        }

        private static void AddGradientQuad(
            VertexHelper vertexHelper,
            Rect rect,
            Color topColor,
            Color bottomColor)
        {
            int start = vertexHelper.currentVertCount;
            vertexHelper.AddVert(new Vector2(rect.xMin, rect.yMin), bottomColor, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMin, rect.yMax), topColor, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMax, rect.yMax), topColor, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMax, rect.yMin), bottomColor, Vector2.zero);
            vertexHelper.AddTriangle(start, start + 1, start + 2);
            vertexHelper.AddTriangle(start, start + 2, start + 3);
        }

        private static void AddCircle(
            VertexHelper vertexHelper,
            Vector2 center,
            float radius,
            Color fill,
            int segments)
        {
            AddEllipse(vertexHelper, center, Vector2.one * radius, fill, segments);
        }

        private static void AddEllipse(
            VertexHelper vertexHelper,
            Vector2 center,
            Vector2 radii,
            Color fill,
            int segments)
        {
            int centerIndex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(center, fill, Vector2.zero);
            for (int index = 0; index <= segments; index++)
            {
                float angle = index / (float)segments * Mathf.PI * 2f;
                Vector2 point = center + new Vector2(
                    Mathf.Cos(angle) * radii.x,
                    Mathf.Sin(angle) * radii.y);
                vertexHelper.AddVert(point, fill, Vector2.zero);
                if (index > 0)
                {
                    vertexHelper.AddTriangle(centerIndex, centerIndex + index, centerIndex + index + 1);
                }
            }
        }

        private static void AddDiamond(
            VertexHelper vertexHelper,
            Vector2 center,
            float radius,
            Color fill)
        {
            int start = vertexHelper.currentVertCount;
            vertexHelper.AddVert(center + Vector2.up * radius, fill, Vector2.zero);
            vertexHelper.AddVert(center + Vector2.right * radius, fill, Vector2.zero);
            vertexHelper.AddVert(center + Vector2.down * radius, fill, Vector2.zero);
            vertexHelper.AddVert(center + Vector2.left * radius, fill, Vector2.zero);
            vertexHelper.AddTriangle(start, start + 1, start + 2);
            vertexHelper.AddTriangle(start, start + 2, start + 3);
        }
    }
}
