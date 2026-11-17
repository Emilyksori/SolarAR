using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.Games.Quiz
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class QuizStarGraphic : MaskableGraphic
    {
        [SerializeField, Range(0.2f, 0.8f)] private float _innerRadiusRatio = 0.45f;

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            Rect drawingRect = GetPixelAdjustedRect();
            Vector2 center = drawingRect.center;
            float outerRadius = Mathf.Min(drawingRect.width, drawingRect.height) * 0.5f;
            if (outerRadius <= 0f)
            {
                return;
            }

            vertexHelper.AddVert(center, color, Vector2.one * 0.5f);
            for (int point = 0; point < 10; point++)
            {
                float angle = Mathf.Deg2Rad * (90f + point * 36f);
                float radius = point % 2 == 0 ? outerRadius : outerRadius * _innerRadiusRatio;
                Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
                vertexHelper.AddVert(center + direction * radius, color, Vector2.zero);
            }

            for (int point = 0; point < 10; point++)
            {
                vertexHelper.AddTriangle(0, point + 1, ((point + 1) % 10) + 1);
            }
        }
    }
}
