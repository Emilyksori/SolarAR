using System.Collections.Generic;
using SolarAR.Interaction;
using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.Planets
{
    public sealed class PlanetPreviewController : MonoBehaviour
    {
        [SerializeField] private Camera _previewCamera;
        [SerializeField] private Transform _previewRoot;
        [SerializeField] private RawImage _outputImage;
        [SerializeField, Min(128)] private int _textureSize = 512;
        [SerializeField, Min(128)] private int _thumbnailSize = 256;
        [SerializeField] private int _previewLayer = 30;
        [SerializeField, Min(0.001f)] private float _previewDiameterMeters = 0.048f;

        private readonly Dictionary<string, Texture2D> _thumbnailCache = new();
        private GameObject _currentPlanet;
        private Transform _currentVisualRoot;
        private RotationController _currentRotationController;
        private bool _resumePreviewRotation;
        private RenderTexture _renderTexture;
        private RenderTexture _thumbnailRenderTexture;

        private void Awake()
        {
            EnsureRenderTexture();
            SetPreviewActive(false);
        }

        private void OnDestroy()
        {
            ClearPlanet();
            foreach (Texture2D thumbnail in _thumbnailCache.Values)
            {
                Destroy(thumbnail);
            }

            _thumbnailCache.Clear();
            ReleaseRenderTexture(ref _thumbnailRenderTexture);
            if (_renderTexture == null)
            {
                return;
            }

            _renderTexture.Release();
            Destroy(_renderTexture);
            _renderTexture = null;
        }

        public void ShowPlanet(GameObject planetPrefab)
        {
            ClearPlanet();
            if (planetPrefab == null || _previewRoot == null)
            {
                return;
            }

            EnsureRenderTexture();
            _currentPlanet = Instantiate(planetPrefab, _previewRoot);
            _currentPlanet.name = planetPrefab.name + " Preview";
            _currentPlanet.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            ConfigureStablePreview(_currentPlanet);
            SetLayerRecursively(_currentPlanet.transform, _previewLayer);
            SetPreviewActive(true);
        }

        public Texture GetOrCreateThumbnail(string planetKey, GameObject planetPrefab)
        {
            if (string.IsNullOrWhiteSpace(planetKey) || planetPrefab == null)
            {
                return null;
            }

            if (_thumbnailCache.TryGetValue(planetKey, out Texture2D cached))
            {
                return cached;
            }

            EnsureThumbnailRenderTexture();
            if (_previewCamera == null || _previewRoot == null || _thumbnailRenderTexture == null)
            {
                return null;
            }

            bool rootWasActive = _previewRoot.gameObject.activeSelf;
            RenderTexture previousTarget = _previewCamera.targetTexture;
            RenderTexture previousActive = RenderTexture.active;
            _previewRoot.gameObject.SetActive(true);

            GameObject instance = Instantiate(planetPrefab, _previewRoot);
            instance.name = planetPrefab.name + " Thumbnail";
            instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            ConfigureStablePreview(instance, false);
            SetLayerRecursively(instance.transform, _previewLayer);

            _previewCamera.targetTexture = _thumbnailRenderTexture;
            _previewCamera.Render();
            RenderTexture.active = _thumbnailRenderTexture;
            Texture2D thumbnail = new(
                _thumbnailSize,
                _thumbnailSize,
                TextureFormat.RGBA32,
                false)
            {
                name = $"{planetKey} 3D Thumbnail",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            thumbnail.ReadPixels(new Rect(0f, 0f, _thumbnailSize, _thumbnailSize), 0, 0);
            thumbnail.Apply(false, true);

            RenderTexture.active = previousActive;
            _previewCamera.targetTexture = previousTarget;
            instance.SetActive(false);
            Destroy(instance);
            _previewRoot.gameObject.SetActive(rootWasActive);
            _thumbnailCache.Add(planetKey, thumbnail);
            return thumbnail;
        }

        public void RotatePreview(Vector2 pointerDelta)
        {
            if (_currentVisualRoot == null)
            {
                return;
            }

            _currentVisualRoot.Rotate(Vector3.up, -pointerDelta.x, Space.World);
            _currentVisualRoot.Rotate(Vector3.right, pointerDelta.y, Space.Self);
        }

        public void BeginPreviewInteraction()
        {
            if (_currentRotationController == null)
            {
                return;
            }

            _resumePreviewRotation = _currentRotationController.AutoRotate;
            _currentRotationController.SetAutoRotation(false);
        }

        public void EndPreviewInteraction()
        {
            if (_currentRotationController != null)
            {
                _currentRotationController.SetAutoRotation(_resumePreviewRotation);
            }
        }

        private void ConfigureStablePreview(GameObject instance, bool keepRotation = true)
        {
            FloatingController floating = instance.GetComponent<FloatingController>();
            if (floating != null)
            {
                floating.enabled = false;
            }

            PlanetDisplay display = instance.GetComponent<PlanetDisplay>();
            if (display == null || display.VisualRoot == null)
            {
                return;
            }

            display.VisualRoot.localPosition = Vector3.zero;
            if (instance == _currentPlanet)
            {
                _currentVisualRoot = display.VisualRoot;
            }

            RotationController rotation = instance.GetComponent<RotationController>();
            if (instance == _currentPlanet)
            {
                _currentRotationController = rotation;
                _resumePreviewRotation = rotation != null && rotation.AutoRotate;
            }

            if (!keepRotation && rotation != null)
            {
                rotation.SetAutoRotation(false);
            }

            EarthGestureController gestures = instance.GetComponent<EarthGestureController>();
            if (gestures != null)
            {
                gestures.enabled = false;
            }

            if (display.DisplayDiameterMeters > Mathf.Epsilon)
            {
                display.VisualRoot.localScale *=
                    _previewDiameterMeters / display.DisplayDiameterMeters;
            }
        }

        public void SetPreviewActive(bool active)
        {
            if (_previewCamera != null)
            {
                _previewCamera.enabled = active;
            }

            if (_outputImage != null)
            {
                _outputImage.enabled = active;
            }

            if (_previewRoot != null)
            {
                _previewRoot.gameObject.SetActive(active);
            }
        }

        public void ClearPlanet()
        {
            if (_currentPlanet == null)
            {
                return;
            }

            Destroy(_currentPlanet);
            _currentPlanet = null;
            _currentVisualRoot = null;
            _currentRotationController = null;
            _resumePreviewRotation = false;
        }

        private void EnsureRenderTexture()
        {
            if (_renderTexture != null || _previewCamera == null || _outputImage == null)
            {
                return;
            }

            _renderTexture = new RenderTexture(
                _textureSize,
                _textureSize,
                24,
                RenderTextureFormat.ARGB32)
            {
                name = "SolarAR Planet Preview",
                antiAliasing = 2
            };
            _renderTexture.Create();
            _previewCamera.targetTexture = _renderTexture;
            _outputImage.texture = _renderTexture;
            _outputImage.color = Color.white;
        }

        private void EnsureThumbnailRenderTexture()
        {
            if (_thumbnailRenderTexture != null)
            {
                return;
            }

            _thumbnailRenderTexture = new RenderTexture(
                _thumbnailSize,
                _thumbnailSize,
                24,
                RenderTextureFormat.ARGB32)
            {
                name = "SolarAR Planet Thumbnail Renderer",
                antiAliasing = 2
            };
            _thumbnailRenderTexture.Create();
        }

        private static void ReleaseRenderTexture(ref RenderTexture renderTexture)
        {
            if (renderTexture == null)
            {
                return;
            }

            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }

        private static void SetLayerRecursively(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform child in root)
            {
                SetLayerRecursively(child, layer);
            }
        }
    }
}
