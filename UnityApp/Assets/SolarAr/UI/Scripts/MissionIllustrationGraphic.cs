using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class MissionIllustrationGraphic : MaskableGraphic
    {
        public enum Kind
        {
            Quiz,
            Memory,
            PlanetOrder,
            SpaceDetective,
            Rocket,
            BackArrow,
            ForwardArrow
        }

        [SerializeField] private Kind _kind;
        private readonly List<Vector2> _points = new();

        public void Configure(Kind kind)
        {
            _kind = kind;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            Rect rect = GetPixelAdjustedRect();
            if (rect.width <= 0f || rect.height <= 0f)
            {
                return;
            }

            switch (_kind)
            {
                case Kind.Quiz:
                    DrawQuiz(vertexHelper, rect);
                    break;
                case Kind.Memory:
                    DrawMemory(vertexHelper, rect);
                    break;
                case Kind.PlanetOrder:
                    DrawPlanetOrder(vertexHelper, rect);
                    break;
                case Kind.SpaceDetective:
                    DrawSpaceDetective(vertexHelper, rect);
                    break;
                case Kind.Rocket:
                    DrawRocket(vertexHelper, rect);
                    break;
                case Kind.BackArrow:
                    DrawBackArrow(vertexHelper, rect);
                    break;
                case Kind.ForwardArrow:
                    DrawForwardArrow(vertexHelper, rect);
                    break;
            }
        }

        private void DrawQuiz(VertexHelper vertexHelper, Rect rect)
        {
            Color shadow = Tint("27185F", 0.70f);
            Color purple = Tint("7951E8");
            Color lavender = Tint("B99CFF");
            Color paper = Tint("F1EEFF");
            Color ink = Tint("6E49D8");

            AddRoundedRect(vertexHelper, rect, new Rect(0.13f, 0.12f, 0.55f, 0.68f), 0.09f, shadow);
            AddRoundedRect(vertexHelper, rect, new Rect(0.10f, 0.17f, 0.55f, 0.68f), 0.09f, lavender);
            AddRoundedRect(vertexHelper, rect, new Rect(0.14f, 0.21f, 0.47f, 0.59f), 0.07f, paper);
            AddRoundedRect(vertexHelper, rect, new Rect(0.25f, 0.76f, 0.25f, 0.13f), 0.06f, purple);
            AddCircle(vertexHelper, rect, new Vector2(0.375f, 0.87f), 0.052f, lavender);

            for (int index = 0; index < 2; index++)
            {
                float y = 0.58f - index * 0.20f;
                AddRoundedRect(
                    vertexHelper,
                    rect,
                    new Rect(0.21f, y, 0.14f, 0.13f),
                    0.035f,
                    purple);
                AddLine(vertexHelper, rect, new Vector2(0.235f, y + 0.065f), new Vector2(0.27f, y + 0.03f), 0.025f, Color.white);
                AddLine(vertexHelper, rect, new Vector2(0.27f, y + 0.03f), new Vector2(0.325f, y + 0.105f), 0.025f, Color.white);
                AddRoundedRect(
                    vertexHelper,
                    rect,
                    new Rect(0.39f, y + 0.074f, 0.15f, 0.025f),
                    0.012f,
                    ink);
                AddRoundedRect(
                    vertexHelper,
                    rect,
                    new Rect(0.39f, y + 0.025f, 0.10f, 0.020f),
                    0.01f,
                    Tint("A78FEB"));
            }

            AddCircle(vertexHelper, rect, new Vector2(0.72f, 0.69f), 0.19f, Tint("BE59F4"));
            AddQuestionMark(vertexHelper, rect, new Vector2(0.72f, 0.71f), 0.12f, Color.white);
        }

        private void DrawMemory(VertexHelper vertexHelper, Rect rect)
        {
            Color border = Tint("A9C9FF");
            Color cardBack = Tint("5131B4");
            Color cardFront = Tint("6241D8");

            AddRoundedRect(vertexHelper, rect, new Rect(0.35f, 0.26f, 0.48f, 0.58f), 0.09f, border);
            AddRoundedRect(vertexHelper, rect, new Rect(0.38f, 0.29f, 0.42f, 0.52f), 0.07f, cardBack);
            AddStar(vertexHelper, rect, new Vector2(0.59f, 0.57f), 0.15f, 0.07f, Tint("D67BFF"));

            AddRoundedRect(vertexHelper, rect, new Rect(0.10f, 0.12f, 0.52f, 0.62f), 0.10f, border);
            AddRoundedRect(vertexHelper, rect, new Rect(0.13f, 0.15f, 0.46f, 0.56f), 0.08f, cardFront);
            AddCircle(vertexHelper, rect, new Vector2(0.36f, 0.43f), 0.13f, Tint("F4B567"));
            AddEllipseRing(
                vertexHelper,
                rect,
                new Vector2(0.36f, 0.43f),
                new Vector2(0.22f, 0.075f),
                0.025f,
                Tint("FFD27A"),
                -12f);
            AddCircle(vertexHelper, rect, new Vector2(0.32f, 0.48f), 0.025f, Tint("FFE1A4", 0.72f));
        }

        private void DrawPlanetOrder(VertexHelper vertexHelper, Rect rect)
        {
            AddEllipseRing(
                vertexHelper,
                rect,
                new Vector2(0.50f, 0.49f),
                new Vector2(0.43f, 0.28f),
                0.018f,
                Tint("6F75FF", 0.92f),
                12f);
            AddCircle(vertexHelper, rect, new Vector2(0.09f, 0.47f), 0.23f, Tint("FFB93E"));
            AddCircle(vertexHelper, rect, new Vector2(0.12f, 0.52f), 0.16f, Tint("FFD56A", 0.78f));

            AddCircle(vertexHelper, rect, new Vector2(0.30f, 0.35f), 0.075f, Tint("BBB4D7"));
            AddCircle(vertexHelper, rect, new Vector2(0.51f, 0.43f), 0.115f, Tint("4BADE8"));
            AddCircle(vertexHelper, rect, new Vector2(0.48f, 0.45f), 0.040f, Tint("74CF77"));
            AddCircle(vertexHelper, rect, new Vector2(0.57f, 0.39f), 0.032f, Tint("69C66D"));
            AddCircle(vertexHelper, rect, new Vector2(0.72f, 0.59f), 0.095f, Tint("E75D4A"));
            AddCircle(vertexHelper, rect, new Vector2(0.69f, 0.63f), 0.022f, Tint("F28A6F", 0.82f));

            AddLine(vertexHelper, rect, new Vector2(0.31f, 0.18f), new Vector2(0.70f, 0.18f), 0.022f, Tint("68DCCB", 0.76f));
            AddTriangle(
                vertexHelper,
                Point(rect, new Vector2(0.74f, 0.18f)),
                Point(rect, new Vector2(0.68f, 0.23f)),
                Point(rect, new Vector2(0.68f, 0.13f)),
                Tint("68DCCB"));
        }

        private void DrawSpaceDetective(VertexHelper vertexHelper, Rect rect)
        {
            Color glass = Tint("37247D", 0.88f);
            Color rim = Tint("9B8CFF");
            AddLine(vertexHelper, rect, new Vector2(0.28f, 0.22f), new Vector2(0.44f, 0.39f), 0.11f, Tint("7A47D8"));
            AddLine(vertexHelper, rect, new Vector2(0.25f, 0.18f), new Vector2(0.35f, 0.29f), 0.08f, Tint("A96EF2"));
            AddCircle(vertexHelper, rect, new Vector2(0.55f, 0.57f), 0.31f, rim);
            AddCircle(vertexHelper, rect, new Vector2(0.55f, 0.57f), 0.23f, glass);

            AddCircle(vertexHelper, rect, new Vector2(0.55f, 0.56f), 0.115f, Tint("F4B46C"));
            AddEllipseRing(
                vertexHelper,
                rect,
                new Vector2(0.55f, 0.56f),
                new Vector2(0.19f, 0.060f),
                0.021f,
                Tint("FFD079"),
                -8f);
            AddStar(vertexHelper, rect, new Vector2(0.82f, 0.80f), 0.07f, 0.025f, Tint("FFD65D"));
            AddStar(vertexHelper, rect, new Vector2(0.80f, 0.34f), 0.035f, 0.014f, Tint("EEE8FF"));
            AddQuestionMark(vertexHelper, rect, new Vector2(0.78f, 0.55f), 0.10f, Tint("E7DFFF"));
        }

        private void DrawRocket(VertexHelper vertexHelper, Rect rect)
        {
            AddTriangle(
                vertexHelper,
                Point(rect, new Vector2(0.50f, 0.94f)),
                Point(rect, new Vector2(0.31f, 0.70f)),
                Point(rect, new Vector2(0.69f, 0.70f)),
                Tint("F7F4FF"));
            AddRoundedRect(vertexHelper, rect, new Rect(0.31f, 0.30f, 0.38f, 0.47f), 0.16f, Tint("D7CBFF"));
            AddTriangle(
                vertexHelper,
                Point(rect, new Vector2(0.31f, 0.48f)),
                Point(rect, new Vector2(0.18f, 0.26f)),
                Point(rect, new Vector2(0.38f, 0.31f)),
                Tint("8C5BE8"));
            AddTriangle(
                vertexHelper,
                Point(rect, new Vector2(0.69f, 0.48f)),
                Point(rect, new Vector2(0.82f, 0.26f)),
                Point(rect, new Vector2(0.62f, 0.31f)),
                Tint("8C5BE8"));
            AddCircle(vertexHelper, rect, new Vector2(0.50f, 0.62f), 0.11f, Tint("6842D3"));
            AddCircle(vertexHelper, rect, new Vector2(0.50f, 0.62f), 0.065f, Tint("A9DCFF"));
            AddTriangle(
                vertexHelper,
                Point(rect, new Vector2(0.39f, 0.31f)),
                Point(rect, new Vector2(0.50f, 0.02f)),
                Point(rect, new Vector2(0.61f, 0.31f)),
                Tint("FF9B58"));
            AddTriangle(
                vertexHelper,
                Point(rect, new Vector2(0.44f, 0.30f)),
                Point(rect, new Vector2(0.50f, 0.12f)),
                Point(rect, new Vector2(0.56f, 0.30f)),
                Tint("FFD46B"));
        }

        private void DrawBackArrow(VertexHelper vertexHelper, Rect rect)
        {
            Color arrow = Tint("FFFFFF");
            AddLine(vertexHelper, rect, new Vector2(0.70f, 0.20f), new Vector2(0.34f, 0.50f), 0.13f, arrow);
            AddLine(vertexHelper, rect, new Vector2(0.34f, 0.50f), new Vector2(0.70f, 0.80f), 0.13f, arrow);
        }

        private void DrawForwardArrow(VertexHelper vertexHelper, Rect rect)
        {
            Color arrow = Tint("FFFFFF");
            AddLine(vertexHelper, rect, new Vector2(0.30f, 0.20f), new Vector2(0.66f, 0.50f), 0.13f, arrow);
            AddLine(vertexHelper, rect, new Vector2(0.66f, 0.50f), new Vector2(0.30f, 0.80f), 0.13f, arrow);
        }

        private void AddQuestionMark(
            VertexHelper vertexHelper,
            Rect rect,
            Vector2 center,
            float radius,
            Color fill)
        {
            Vector2 previous = center + new Vector2(-radius * 0.52f, radius * 0.38f);
            Vector2[] points =
            {
                center + new Vector2(-radius * 0.30f, radius * 0.72f),
                center + new Vector2(radius * 0.22f, radius * 0.72f),
                center + new Vector2(radius * 0.54f, radius * 0.36f),
                center + new Vector2(radius * 0.36f, 0f),
                center + new Vector2(0f, -radius * 0.22f),
                center + new Vector2(0f, -radius * 0.43f)
            };
            foreach (Vector2 point in points)
            {
                AddLine(vertexHelper, rect, previous, point, radius * 0.18f, fill);
                previous = point;
            }

            AddCircle(vertexHelper, rect, center + new Vector2(0f, -radius * 0.78f), radius * 0.12f, fill);
        }

        private void AddRoundedRect(
            VertexHelper vertexHelper,
            Rect canvasRect,
            Rect normalizedRect,
            float normalizedRadius,
            Color fill)
        {
            Rect rect = new(
                Mathf.Lerp(canvasRect.xMin, canvasRect.xMax, normalizedRect.x),
                Mathf.Lerp(canvasRect.yMin, canvasRect.yMax, normalizedRect.y),
                canvasRect.width * normalizedRect.width,
                canvasRect.height * normalizedRect.height);
            float radius = Mathf.Min(canvasRect.width, canvasRect.height) * normalizedRadius;
            radius = Mathf.Min(radius, Mathf.Min(rect.width, rect.height) * 0.5f);

            _points.Clear();
            AddCornerPoints(new Vector2(rect.xMin + radius, rect.yMin + radius), radius, 180f, 270f);
            AddCornerPoints(new Vector2(rect.xMax - radius, rect.yMin + radius), radius, 270f, 360f);
            AddCornerPoints(new Vector2(rect.xMax - radius, rect.yMax - radius), radius, 0f, 90f);
            AddCornerPoints(new Vector2(rect.xMin + radius, rect.yMax - radius), radius, 90f, 180f);
            AddPolygon(vertexHelper, _points, fill);
        }

        private void AddCornerPoints(
            Vector2 center,
            float radius,
            float startAngle,
            float endAngle)
        {
            const int segments = 5;
            for (int index = 0; index <= segments; index++)
            {
                float angle = Mathf.Lerp(startAngle, endAngle, index / (float)segments) * Mathf.Deg2Rad;
                _points.Add(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
            }
        }

        private static void AddCircle(
            VertexHelper vertexHelper,
            Rect rect,
            Vector2 normalizedCenter,
            float normalizedRadius,
            Color fill)
        {
            float scale = Mathf.Min(rect.width, rect.height);
            AddEllipse(vertexHelper, Point(rect, normalizedCenter), Vector2.one * scale * normalizedRadius, fill, 24, 0f);
        }

        private static void AddEllipseRing(
            VertexHelper vertexHelper,
            Rect rect,
            Vector2 normalizedCenter,
            Vector2 normalizedRadii,
            float normalizedThickness,
            Color fill,
            float angle)
        {
            Vector2 center = Point(rect, normalizedCenter);
            Vector2 radii = new(rect.width * normalizedRadii.x, rect.height * normalizedRadii.y);
            float thickness = Mathf.Min(rect.width, rect.height) * normalizedThickness;
            const int segments = 32;
            int start = vertexHelper.currentVertCount;

            for (int index = 0; index <= segments; index++)
            {
                float radians = index / (float)segments * Mathf.PI * 2f;
                Vector2 direction = Rotate(new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)), angle);
                Vector2 outer = center + Rotate(
                    new Vector2(Mathf.Cos(radians) * radii.x, Mathf.Sin(radians) * radii.y),
                    angle);
                Vector2 inner = outer - direction * thickness;
                vertexHelper.AddVert(outer, fill, Vector2.zero);
                vertexHelper.AddVert(inner, fill, Vector2.zero);
                if (index > 0)
                {
                    int current = start + index * 2;
                    int previous = current - 2;
                    vertexHelper.AddTriangle(previous, current, previous + 1);
                    vertexHelper.AddTriangle(current, current + 1, previous + 1);
                }
            }
        }

        private static void AddEllipse(
            VertexHelper vertexHelper,
            Vector2 center,
            Vector2 radii,
            Color fill,
            int segments,
            float angle)
        {
            int centerIndex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(center, fill, Vector2.zero);
            for (int index = 0; index <= segments; index++)
            {
                float radians = index / (float)segments * Mathf.PI * 2f;
                Vector2 offset = Rotate(
                    new Vector2(Mathf.Cos(radians) * radii.x, Mathf.Sin(radians) * radii.y),
                    angle);
                vertexHelper.AddVert(center + offset, fill, Vector2.zero);
                if (index > 0)
                {
                    vertexHelper.AddTriangle(centerIndex, centerIndex + index, centerIndex + index + 1);
                }
            }
        }

        private static void AddLine(
            VertexHelper vertexHelper,
            Rect rect,
            Vector2 normalizedStart,
            Vector2 normalizedEnd,
            float normalizedWidth,
            Color fill)
        {
            Vector2 start = Point(rect, normalizedStart);
            Vector2 end = Point(rect, normalizedEnd);
            float width = Mathf.Min(rect.width, rect.height) * normalizedWidth;
            Vector2 direction = (end - start).normalized;
            Vector2 normal = new(-direction.y, direction.x);
            Vector2 offset = normal * width * 0.5f;
            int vertexStart = vertexHelper.currentVertCount;
            vertexHelper.AddVert(start - offset, fill, Vector2.zero);
            vertexHelper.AddVert(start + offset, fill, Vector2.zero);
            vertexHelper.AddVert(end + offset, fill, Vector2.zero);
            vertexHelper.AddVert(end - offset, fill, Vector2.zero);
            vertexHelper.AddTriangle(vertexStart, vertexStart + 1, vertexStart + 2);
            vertexHelper.AddTriangle(vertexStart, vertexStart + 2, vertexStart + 3);
            AddEllipse(vertexHelper, start, Vector2.one * width * 0.5f, fill, 10, 0f);
            AddEllipse(vertexHelper, end, Vector2.one * width * 0.5f, fill, 10, 0f);
        }

        private static void AddStar(
            VertexHelper vertexHelper,
            Rect rect,
            Vector2 normalizedCenter,
            float outerRadius,
            float innerRadius,
            Color fill)
        {
            Vector2 center = Point(rect, normalizedCenter);
            float scale = Mathf.Min(rect.width, rect.height);
            int centerIndex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(center, fill, Vector2.zero);
            for (int index = 0; index <= 10; index++)
            {
                float radius = (index & 1) == 0 ? outerRadius : innerRadius;
                float angle = (90f + index * 36f) * Mathf.Deg2Rad;
                Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * scale;
                vertexHelper.AddVert(point, fill, Vector2.zero);
                if (index > 0)
                {
                    vertexHelper.AddTriangle(centerIndex, centerIndex + index, centerIndex + index + 1);
                }
            }
        }

        private static void AddTriangle(
            VertexHelper vertexHelper,
            Vector2 first,
            Vector2 second,
            Vector2 third,
            Color fill)
        {
            int start = vertexHelper.currentVertCount;
            vertexHelper.AddVert(first, fill, Vector2.zero);
            vertexHelper.AddVert(second, fill, Vector2.zero);
            vertexHelper.AddVert(third, fill, Vector2.zero);
            vertexHelper.AddTriangle(start, start + 1, start + 2);
        }

        private static void AddPolygon(
            VertexHelper vertexHelper,
            IReadOnlyList<Vector2> points,
            Color fill)
        {
            if (points.Count < 3)
            {
                return;
            }

            Vector2 center = Vector2.zero;
            for (int index = 0; index < points.Count; index++)
            {
                center += points[index];
            }

            center /= points.Count;
            int centerIndex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(center, fill, Vector2.zero);
            for (int index = 0; index < points.Count; index++)
            {
                vertexHelper.AddVert(points[index], fill, Vector2.zero);
            }

            for (int index = 0; index < points.Count; index++)
            {
                int next = (index + 1) % points.Count;
                vertexHelper.AddTriangle(centerIndex, centerIndex + index + 1, centerIndex + next + 1);
            }
        }

        private static Vector2 Point(Rect rect, Vector2 normalized)
        {
            return new Vector2(
                Mathf.Lerp(rect.xMin, rect.xMax, normalized.x),
                Mathf.Lerp(rect.yMin, rect.yMax, normalized.y));
        }

        private static Vector2 Rotate(Vector2 point, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            float cosine = Mathf.Cos(radians);
            float sine = Mathf.Sin(radians);
            return new Vector2(
                point.x * cosine - point.y * sine,
                point.x * sine + point.y * cosine);
        }

        private static Color Tint(string hex, float alpha = 1f)
        {
            ColorUtility.TryParseHtmlString($"#{hex}", out Color result);
            result.a *= alpha;
            return result;
        }
    }
}
