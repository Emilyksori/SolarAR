using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class VerticalGradientGraphic : MaskableGraphic
    {
        [SerializeField] private Color _topColor = Color.white;
        [SerializeField] private Color _bottomColor = Color.white;

        public void SetColors(Color topColor, Color bottomColor)
        {
            _topColor = topColor;
            _bottomColor = bottomColor;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            Rect rect = GetPixelAdjustedRect();
            Color top = _topColor * color;
            Color bottom = _bottomColor * color;

            vertexHelper.AddVert(new Vector2(rect.xMin, rect.yMin), bottom, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMin, rect.yMax), top, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMax, rect.yMax), top, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMax, rect.yMin), bottom, Vector2.zero);
            vertexHelper.AddTriangle(0, 1, 2);
            vertexHelper.AddTriangle(0, 2, 3);
        }
    }
}
