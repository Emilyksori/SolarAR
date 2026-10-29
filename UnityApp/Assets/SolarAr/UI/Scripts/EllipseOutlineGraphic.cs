using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class EllipseOutlineGraphic : MaskableGraphic
    {
        [SerializeField, Min(1f)] private float _thickness = 2f;
        [SerializeField, Range(12, 96)] private int _segments = 48;

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            Rect rect = GetPixelAdjustedRect();
            Vector2 outerRadius = rect.size * 0.5f;
            Vector2 innerRadius = new(
                Mathf.Max(0f, outerRadius.x - _thickness),
                Mathf.Max(0f, outerRadius.y - _thickness));
            Vector2 center = rect.center;

            for (int index = 0; index <= _segments; index++)
            {
                float angle = index / (float)_segments * Mathf.PI * 2f;
                Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
                vertexHelper.AddVert(center + Vector2.Scale(direction, outerRadius), color, Vector2.zero);
                vertexHelper.AddVert(center + Vector2.Scale(direction, innerRadius), color, Vector2.zero);
            }

            for (int index = 0; index < _segments; index++)
            {
                int start = index * 2;
                vertexHelper.AddTriangle(start, start + 2, start + 1);
                vertexHelper.AddTriangle(start + 2, start + 3, start + 1);
            }
        }
    }
}
