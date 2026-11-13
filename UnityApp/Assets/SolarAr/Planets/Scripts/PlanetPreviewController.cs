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
        [SerializeField] private int _previewLayer = 30;
        [SerializeField, Min(0.001f)] private float _previewDiameterMeters = 0.048f;

        private GameObject _currentPlanet;
        private RenderTexture _renderTexture;

        private void Awake()
        {
            EnsureRenderTexture();
            SetPreviewActive(false);
        }

        private void OnDestroy()
        {
            ClearPlanet();
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

        private void ConfigureStablePreview(GameObject instance)
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

            EarthGestureController gestures = instance.GetComponent<EarthGestureController>();
            if (gestures != null && display.DisplayDiameterMeters > Mathf.Epsilon)
            {
                gestures.SetPresentationScale(
                    _previewDiameterMeters / display.DisplayDiameterMeters);
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
