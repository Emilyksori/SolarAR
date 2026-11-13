using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class CircularRawImage : MaskableGraphic
    {
        [SerializeField] private Texture _texture;
        [SerializeField] private Rect _uvRect = new(0f, 0f, 1f, 1f);
        [SerializeField, Range(16, 96)] private int _segments = 64;

        public Texture Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                SetMaterialDirty();
            }
        }

        public Rect UvRect
        {
            get => _uvRect;
            set
            {
                _uvRect = value;
                SetVerticesDirty();
            }
        }

        public override Texture mainTexture => _texture != null ? _texture : s_WhiteTexture;

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            Rect rect = GetPixelAdjustedRect();
            Vector2 center = rect.center;
            Vector2 radius = rect.size * 0.5f;
            Vector2 uvCenter = _uvRect.center;
            Vector2 uvRadius = _uvRect.size * 0.5f;

            vertexHelper.AddVert(center, color, uvCenter);

            for (int index = 0; index <= _segments; index++)
            {
                float angle = index / (float)_segments * Mathf.PI * 2f;
                Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 position = center + Vector2.Scale(direction, radius);
                Vector2 uv = uvCenter + Vector2.Scale(direction, uvRadius);
                vertexHelper.AddVert(position, color, uv);
            }

            for (int index = 0; index < _segments; index++)
            {
                vertexHelper.AddTriangle(0, index + 1, index + 2);
            }
        }
    }
}
