using System.Text;
using SolarAR.AR;
using SolarAR.Education;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace SolarAR.UI
{
    public sealed class EarthMvpUI : MonoBehaviour
    {
        private const float CollapsedPanelHeight = 280f;
        private const float ExpandedPanelHeight = 680f;

        private static readonly Color Night = Hex("0A1433");
        private static readonly Color Ink = Hex("19365B");
        private static readonly Color Panel = Hex("F7FAFF");
        private static readonly Color Primary = Hex("FFDB69");
        private static readonly Color Cyan = Hex("69D9EF");
        private static readonly Color Lavender = Hex("D4D1F3");
        private static readonly Color Success = Hex("B6E9C9");

        [Header("Data and tracking")]
        [SerializeField] private TrackedImageHandler _trackedImageHandler;
        [SerializeField] private EducationalContentManager _educationalContentManager;
        [SerializeField] private string _referenceImageName = "EarthCard";
        [SerializeField] private string _planetKey = "earth";

        private RectTransform _safeArea;
        private RectTransform _educationPanel;
        private Text _statusText;
        private Text _planetNameText;
        private Text _shortDescriptionText;
        private Text _detailsText;
        private Text _detailsButtonText;
        private PlanetContent _content;
        private PlanetFacts _facts;
        private Rect _lastSafeArea;
        private TrackedImageHandler.TrackedImageStatus _trackingStatus;
        private bool _detailsExpanded;
        private bool _uiReady;

        private void OnEnable()
        {
            if (_trackedImageHandler != null)
            {
                _trackedImageHandler.TrackingStatusChanged += OnTrackingStatusChanged;
                _trackingStatus = _trackedImageHandler.CurrentStatus;
            }
        }

        private void Start()
        {
            LoadEarthContent();
            BuildInterface();
            ApplyTrackingStatus();
        }

        private void Update()
        {
            if (_safeArea != null && Screen.safeArea != _lastSafeArea)
            {
                ApplySafeArea();
            }
        }

        private void OnDisable()
        {
            if (_trackedImageHandler != null)
            {
                _trackedImageHandler.TrackingStatusChanged -= OnTrackingStatusChanged;
            }
        }

        private void LoadEarthContent()
        {
            if (_educationalContentManager == null)
            {
                Debug.LogError("[UI] EducationalContentManager is not assigned.", this);
                return;
            }

            if (!_educationalContentManager.TryGetContent(_planetKey, out _content))
            {
                Debug.LogError($"[UI] No PlanetContent found for '{_planetKey}'.", this);
            }

            if (!_educationalContentManager.TryGetFacts(_planetKey, out _facts))
            {
                Debug.LogError($"[UI] No PlanetFacts found for '{_planetKey}'.", this);
            }
        }

        private void BuildInterface()
        {
            EnsureEventSystem();

            GameObject canvasObject = new(
                "Earth MVP Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(412f, 915f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            _safeArea = CreateRect("Safe Area", canvasObject.transform);
            Stretch(_safeArea);
            ApplySafeArea();

            RectTransform statusPanel = CreatePanel("Tracking Status", _safeArea, Night);
            SetTopPanel(statusPanel, 104f);
            _statusText = CreateText("Status", statusPanel, 22, Color.white, TextAnchor.MiddleCenter);
            SetOffsets(_statusText.rectTransform, 16f, 12f, 16f, 12f);

            _educationPanel = CreatePanel("Earth Education", _safeArea, Panel);
            SetBottomPanel(_educationPanel, CollapsedPanelHeight);

            _planetNameText = CreateText(
                "Planet Name",
                _educationPanel,
                30,
                Ink,
                TextAnchor.MiddleLeft);
            SetTopElement(_planetNameText.rectTransform, 20f, 48f, 24f, 24f);

            _shortDescriptionText = CreateText(
                "Short Description",
                _educationPanel,
                17,
                Ink,
                TextAnchor.UpperLeft);
            _shortDescriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
            _shortDescriptionText.verticalOverflow = VerticalWrapMode.Truncate;
            SetTopElement(_shortDescriptionText.rectTransform, 76f, 78f, 24f, 24f);

            Text gestureHint = CreateText(
                "Gesture Hint",
                _educationPanel,
                15,
                Ink,
                TextAnchor.MiddleLeft);
            gestureHint.text = "Arraste para girar  •  Pinça para redimensionar";
            SetTopElement(gestureHint.rectTransform, 158f, 36f, 24f, 24f);

            _detailsText = CreateText(
                "More Information",
                _educationPanel,
                16,
                Ink,
                TextAnchor.UpperLeft);
            _detailsText.horizontalOverflow = HorizontalWrapMode.Wrap;
            _detailsText.verticalOverflow = VerticalWrapMode.Truncate;
            _detailsText.resizeTextForBestFit = true;
            _detailsText.resizeTextMinSize = 13;
            _detailsText.resizeTextMaxSize = 16;
            SetTopElement(_detailsText.rectTransform, 206f, 390f, 24f, 24f);

            Button detailsButton = CreateButton("Details Button", _educationPanel, Primary);
            RectTransform buttonRect = detailsButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0f, 0f);
            buttonRect.anchorMax = new Vector2(1f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0f);
            buttonRect.anchoredPosition = new Vector2(0f, 20f);
            buttonRect.sizeDelta = new Vector2(-48f, 56f);
            detailsButton.onClick.AddListener(ToggleDetails);

            _detailsButtonText = CreateText(
                "Button Label",
                buttonRect,
                20,
                Ink,
                TextAnchor.MiddleCenter);
            Stretch(_detailsButtonText.rectTransform);

            PopulateContent();
            SetDetailsExpanded(false);
            _uiReady = true;
        }

        private void PopulateContent()
        {
            _planetNameText.text = _content != null ? _content.DisplayName : "Terra";
            _shortDescriptionText.text = _content != null
                ? _content.ShortDescription
                : "Conteúdo educacional indisponível.";
            _detailsText.text = BuildDetails();
        }

        private string BuildDetails()
        {
            if (_content == null)
            {
                return "Conteúdo educacional indisponível.";
            }

            StringBuilder details = new();
            details.AppendLine(_content.EducationalSummary);
            details.AppendLine();
            details.AppendLine($"Diâmetro: {_content.DiameterKm:0.##} km");
            details.AppendLine($"Distância média do Sol: {_content.DistanceFromSunMillionKm:0.##} milhões de km");
            details.AppendLine($"Ano: {_content.OrbitalPeriodDays:0.##} dias");
            details.AppendLine($"Dia: {_content.RotationPeriodHours:0.##} horas");
            details.AppendLine($"Luas: {_content.MoonCount}");
            details.AppendLine($"Temperatura média: {_content.AverageTemperatureCelsius:0.##} °C");

            if (_facts != null && _facts.Facts != null && _facts.Facts.Count > 0)
            {
                details.AppendLine();
                details.AppendLine("Curiosidades");

                foreach (PlanetFact fact in _facts.Facts)
                {
                    if (fact != null)
                    {
                        details.AppendLine($"• {fact.Title}: {fact.Description}");
                    }
                }
            }

            return details.ToString().TrimEnd();
        }

        private void OnTrackingStatusChanged(
            string referenceImageName,
            TrackedImageHandler.TrackedImageStatus status)
        {
            if (referenceImageName != _referenceImageName)
            {
                return;
            }

            _trackingStatus = status;
            if (_uiReady)
            {
                ApplyTrackingStatus();
            }
        }

        private void ApplyTrackingStatus()
        {
            switch (_trackingStatus)
            {
                case TrackedImageHandler.TrackedImageStatus.Tracking:
                    _statusText.text = "Terra";
                    _statusText.color = Success;
                    _educationPanel.gameObject.SetActive(true);
                    break;
                case TrackedImageHandler.TrackedImageStatus.Limited:
                    _statusText.text = "Mantenha o card visível";
                    _statusText.color = Cyan;
                    _educationPanel.gameObject.SetActive(true);
                    break;
                default:
                    _statusText.text = "Aponte a câmera para o card da Terra";
                    _statusText.color = Lavender;
                    _educationPanel.gameObject.SetActive(false);
                    SetDetailsExpanded(false);
                    break;
            }
        }

        private void ToggleDetails()
        {
            SetDetailsExpanded(!_detailsExpanded);
        }

        private void SetDetailsExpanded(bool expanded)
        {
            _detailsExpanded = expanded;
            _detailsText.gameObject.SetActive(expanded);
            _educationPanel.sizeDelta = new Vector2(
                _educationPanel.sizeDelta.x,
                expanded ? ExpandedPanelHeight : CollapsedPanelHeight);
            _detailsButtonText.text = expanded ? "Fechar detalhes" : "Saiba mais";
        }

        private void ApplySafeArea()
        {
            _lastSafeArea = Screen.safeArea;
            Vector2 screenSize = new(Screen.width, Screen.height);

            if (screenSize.x <= 0f || screenSize.y <= 0f)
            {
                return;
            }

            _safeArea.anchorMin = _lastSafeArea.position / screenSize;
            _safeArea.anchorMax = (_lastSafeArea.position + _lastSafeArea.size) / screenSize;
            _safeArea.offsetMin = Vector2.zero;
            _safeArea.offsetMax = Vector2.zero;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            GameObject eventSystem = new(
                "EventSystem",
                typeof(EventSystem),
                typeof(InputSystemUIInputModule));
            Object.DontDestroyOnLoad(eventSystem);
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject gameObject = new(name, typeof(RectTransform));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            GameObject gameObject = new(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            gameObject.GetComponent<Image>().color = color;
            return rect;
        }

        private static Text CreateText(
            string name,
            Transform parent,
            int fontSize,
            Color color,
            TextAnchor alignment)
        {
            GameObject gameObject = new(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            Text text = gameObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, Color color)
        {
            GameObject gameObject = new(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            Image image = gameObject.GetComponent<Image>();
            image.color = color;
            Button button = gameObject.GetComponent<Button>();
            button.targetGraphic = image;
            return button;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SetTopPanel(RectTransform rect, float height)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -16f);
            rect.sizeDelta = new Vector2(-32f, height);
        }

        private static void SetBottomPanel(RectTransform rect, float height)
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 16f);
            rect.sizeDelta = new Vector2(-32f, height);
        }

        private static void SetTopElement(
            RectTransform rect,
            float top,
            float height,
            float left,
            float right)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2((left - right) * 0.5f, -top);
            rect.sizeDelta = new Vector2(-(left + right), height);
        }

        private static void SetOffsets(
            RectTransform rect,
            float left,
            float bottom,
            float right,
            float top)
        {
            Stretch(rect);
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString($"#{value}", out Color color);
            return color;
        }
    }
}
