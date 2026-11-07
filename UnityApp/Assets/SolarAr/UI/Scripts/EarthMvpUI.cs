using System.Collections;
using SolarAR.AR;
using SolarAR.Education;
using SolarAR.Planets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace SolarAR.UI
{
    public sealed class EarthMvpUI : MonoBehaviour
    {
        private enum ScreenId
        {
            Onboarding,
            Permission,
            Instructions,
            Explore,
            Searching,
            Recognized,
            Information,
            Lost,
            Planets,
            PlanetDetail
        }

        private static readonly string[] PlanetOrder =
        {
            "mercury",
            "earth",
            "mars",
            "jupiter",
            "saturn",
            "uranus",
            "neptune"
        };

        [Header("Data and tracking")]
        [SerializeField] private TrackedImageHandler _trackedImageHandler;
        [SerializeField] private PlanetRegistry _planetRegistry;
        [SerializeField] private EducationalContentManager _educationalContentManager;
        [SerializeField] private string _planetKey = "earth";

        [Header("Experience roots")]
        [SerializeField] private GameObject _arSessionRoot;
        [SerializeField] private GameObject _xrOriginRoot;
        [SerializeField] private PlanetPreviewController _planetPreviewController;

        [Header("Shared UI")]
        [SerializeField] private GameObject _uiBackgroundCamera;
        [SerializeField] private GameObject _nightBackground;
        [SerializeField] private GameObject _bottomNavigation;
        [SerializeField] private Graphic _exploreTabBackground;
        [SerializeField] private Graphic _planetsTabBackground;
        [SerializeField] private TMP_Text _exploreTabText;
        [SerializeField] private TMP_Text _planetsTabText;
        [SerializeField] private TMP_Text _permissionExplanation;

        [Header("Screens")]
        [SerializeField] private GameObject _onboardingScreen;
        [SerializeField] private GameObject _permissionScreen;
        [SerializeField] private GameObject _instructionsScreen;
        [SerializeField] private GameObject _exploreScreen;
        [SerializeField] private GameObject _searchingScreen;
        [SerializeField] private GameObject _recognizedScreen;
        [SerializeField] private GameObject _informationScreen;
        [SerializeField] private GameObject _lostScreen;
        [SerializeField] private GameObject _planetsScreen;
        [SerializeField] private GameObject _planetDetailScreen;

        [Header("AR educational panel")]
        [SerializeField] private TMP_Text _planetNameText;
        [SerializeField] private TMP_Text _firstFactText;
        [SerializeField] private TMP_Text _secondFactText;

        [Header("Planet detail")]
        [SerializeField] private TMP_Text _detailPlanetNameText;
        [SerializeField] private TMP_Text _detailPanelNameText;
        [SerializeField] private TMP_Text _detailFirstFactText;
        [SerializeField] private TMP_Text _detailSecondFactText;

        [Header("Timing")]
        [SerializeField, Min(0f)] private float _exploreIntroductionSeconds = 1.5f;

        private TrackedImageHandler.TrackedImageStatus _trackingStatus;
        private ScreenId _currentScreen;
        private ScreenId _instructionsReturnScreen = ScreenId.Onboarding;
        private Coroutine _exploreIntroduction;
        private bool _arFlowActive;
        private bool _hasRecognizedCard;
        private int _selectedPlanetIndex = 1;

#if UNITY_ANDROID
        private PermissionCallbacks _permissionCallbacks;
#endif

        private void Awake()
        {
            ShowScreen(ScreenId.Onboarding);
        }

        private void Start()
        {
            PopulateArEducationalPanel(_planetKey);
        }

        private void OnEnable()
        {
            if (_trackedImageHandler == null)
            {
                Debug.LogError("[UI] TrackedImageHandler is not assigned.", this);
                return;
            }

            _trackedImageHandler.TrackingStatusChanged += OnTrackingStatusChanged;
            _trackingStatus = _trackedImageHandler.CurrentStatus;
        }

        private void OnDisable()
        {
            if (_trackedImageHandler != null)
            {
                _trackedImageHandler.TrackingStatusChanged -= OnTrackingStatusChanged;
            }

#if UNITY_ANDROID
            ClearPermissionCallbacks();
#endif
        }

        public void BeginCameraFlow()
        {
            ShowScreen(ScreenId.Permission);
        }

        public void RequestCameraPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                EnterArExperience();
                return;
            }

            ClearPermissionCallbacks();
            _permissionCallbacks = new PermissionCallbacks();
            _permissionCallbacks.PermissionGranted += OnCameraPermissionGranted;
            _permissionCallbacks.PermissionDenied += OnCameraPermissionDenied;
            _permissionCallbacks.PermissionDeniedAndDontAskAgain += OnCameraPermissionDenied;
            Permission.RequestUserPermission(Permission.Camera, _permissionCallbacks);
#else
            EnterArExperience();
#endif
        }

        public void ShowExploreTab()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                BeginCameraFlow();
                return;
            }
#endif
            EnterArExperience();
        }

        public void ShowPlanets()
        {
            StopExploreIntroduction();
            _arFlowActive = false;
            ShowScreen(ScreenId.Planets);
        }

        public void ShowPlanetDetail(int index)
        {
            _selectedPlanetIndex = WrapPlanetIndex(index);
            string selectedKey = PlanetOrder[_selectedPlanetIndex];
            PopulateDetailPanel(selectedKey);

            if (_planetRegistry == null ||
                !_planetRegistry.TryGetPrefabByPlanetKey(selectedKey, out GameObject prefab))
            {
                Debug.LogError($"[UI] No preview prefab configured for '{selectedKey}'.", this);
                return;
            }

            ShowScreen(ScreenId.PlanetDetail);
            _planetPreviewController.ShowPlanet(prefab);
        }

        public void ShowPreviousPlanet()
        {
            ShowPlanetDetail(_selectedPlanetIndex - 1);
        }

        public void ShowNextPlanet()
        {
            ShowPlanetDetail(_selectedPlanetIndex + 1);
        }

        public void BackToPlanets()
        {
            ShowScreen(ScreenId.Planets);
        }

        public void ShowInstructions()
        {
            _instructionsReturnScreen = _currentScreen;
            ShowScreen(ScreenId.Instructions);
        }

        public void BackFromInstructions()
        {
            ShowScreen(_instructionsReturnScreen);
        }

        public void BackToOnboarding()
        {
            StopExploreIntroduction();
            _arFlowActive = false;
            _hasRecognizedCard = false;
            ShowScreen(ScreenId.Onboarding);
        }

        public void ShowInformation()
        {
            if (_trackingStatus == TrackedImageHandler.TrackedImageStatus.Tracking)
            {
                ShowScreen(ScreenId.Information);
            }
        }

        public void CloseInformation()
        {
            if (_trackingStatus == TrackedImageHandler.TrackedImageStatus.Tracking)
            {
                ShowScreen(ScreenId.Recognized);
            }
            else
            {
                ShowScreen(_hasRecognizedCard ? ScreenId.Lost : ScreenId.Searching);
            }
        }

        private void EnterArExperience()
        {
            _arFlowActive = true;
            _hasRecognizedCard = false;
            ShowScreen(ScreenId.Explore);
            if (_trackedImageHandler != null)
            {
                _trackingStatus = _trackedImageHandler.CurrentStatus;
            }

            StopExploreIntroduction();
            _exploreIntroduction = StartCoroutine(ShowSearchingAfterIntroduction());
        }

        private IEnumerator ShowSearchingAfterIntroduction()
        {
            yield return new WaitForSeconds(_exploreIntroductionSeconds);

            if (_arFlowActive && _trackingStatus != TrackedImageHandler.TrackedImageStatus.Tracking)
            {
                ShowScreen(ScreenId.Searching);
            }

            _exploreIntroduction = null;
        }

        private void StopExploreIntroduction()
        {
            if (_exploreIntroduction == null)
            {
                return;
            }

            StopCoroutine(_exploreIntroduction);
            _exploreIntroduction = null;
        }

        private void OnTrackingStatusChanged(
            string referenceImageName,
            TrackedImageHandler.TrackedImageStatus status)
        {
            if (_planetRegistry == null ||
                !_planetRegistry.TryGetPlanetKey(referenceImageName, out string trackedPlanetKey))
            {
                return;
            }

            _trackingStatus = status;
            if (!_arFlowActive)
            {
                return;
            }

            StopExploreIntroduction();

            if (status == TrackedImageHandler.TrackedImageStatus.Tracking)
            {
                _planetKey = trackedPlanetKey;
                PopulateArEducationalPanel(_planetKey);
                _hasRecognizedCard = true;
                if (_currentScreen != ScreenId.Information)
                {
                    ShowScreen(ScreenId.Recognized);
                }

                return;
            }

            ShowScreen(_hasRecognizedCard ? ScreenId.Lost : ScreenId.Searching);
        }

        private void PopulateArEducationalPanel(string planetKey)
        {
            PopulateEducationalPanel(
                planetKey,
                _planetNameText,
                _firstFactText,
                _secondFactText);
        }

        private void PopulateDetailPanel(string planetKey)
        {
            PopulateEducationalPanel(
                planetKey,
                _detailPanelNameText,
                _detailFirstFactText,
                _detailSecondFactText);

            if (_educationalContentManager != null &&
                _educationalContentManager.TryGetContent(planetKey, out PlanetContent content) &&
                _detailPlanetNameText != null)
            {
                _detailPlanetNameText.text = content.DisplayName;
            }
        }

        private void PopulateEducationalPanel(
            string planetKey,
            TMP_Text nameText,
            TMP_Text firstFactText,
            TMP_Text secondFactText)
        {
            if (_educationalContentManager == null)
            {
                Debug.LogError("[UI] EducationalContentManager is not assigned.", this);
                return;
            }

            if (_educationalContentManager.TryGetContent(planetKey, out PlanetContent content) &&
                nameText != null)
            {
                nameText.text = content.DisplayName;
            }

            if (!_educationalContentManager.TryGetFacts(planetKey, out PlanetFacts facts) ||
                facts.Facts == null)
            {
                Debug.LogWarning($"[UI] No facts found for '{planetKey}'.", this);
                return;
            }

            if (facts.Facts.Count > 0 && firstFactText != null)
            {
                firstFactText.text = facts.Facts[0].Description;
            }

            if (facts.Facts.Count > 1 && secondFactText != null)
            {
                secondFactText.text = facts.Facts[1].Description;
            }
        }

        private void ShowScreen(ScreenId screen)
        {
            _currentScreen = screen;

            SetScreenActive(_onboardingScreen, screen == ScreenId.Onboarding);
            SetScreenActive(_permissionScreen, screen == ScreenId.Permission);
            SetScreenActive(_instructionsScreen, screen == ScreenId.Instructions);
            SetScreenActive(_exploreScreen, screen == ScreenId.Explore);
            SetScreenActive(_searchingScreen, screen == ScreenId.Searching);
            SetScreenActive(_recognizedScreen, screen == ScreenId.Recognized);
            SetScreenActive(_informationScreen, screen == ScreenId.Information);
            SetScreenActive(_lostScreen, screen == ScreenId.Lost);
            SetScreenActive(_planetsScreen, screen == ScreenId.Planets);
            SetScreenActive(_planetDetailScreen, screen == ScreenId.PlanetDetail);

            bool isArScreen = screen == ScreenId.Explore ||
                              screen == ScreenId.Searching ||
                              screen == ScreenId.Recognized ||
                              screen == ScreenId.Information ||
                              screen == ScreenId.Lost;
            bool isMainExperience = isArScreen ||
                                    screen == ScreenId.Planets ||
                                    screen == ScreenId.PlanetDetail;
            bool isPlanetTab = screen == ScreenId.Planets || screen == ScreenId.PlanetDetail;
            bool isPlanetDetail = screen == ScreenId.PlanetDetail;

            SetScreenActive(_bottomNavigation, isMainExperience);
            SetScreenActive(_nightBackground, !isArScreen);
            SetScreenActive(_uiBackgroundCamera, !isArScreen);
            SetArActive(isArScreen);

            if (_planetPreviewController != null)
            {
                _planetPreviewController.SetPreviewActive(isPlanetDetail);
                if (!isPlanetDetail)
                {
                    _planetPreviewController.ClearPlanet();
                }
            }

            UpdateBottomNavigation(isPlanetTab);
        }

        private void UpdateBottomNavigation(bool planetsActive)
        {
            if (_exploreTabBackground != null)
            {
                _exploreTabBackground.color = planetsActive ? SolarARColors.Night : SolarARColors.Blue1;
            }

            if (_planetsTabBackground != null)
            {
                _planetsTabBackground.color = planetsActive ? SolarARColors.Blue1 : SolarARColors.Night;
            }

            if (_exploreTabText != null)
            {
                _exploreTabText.color = planetsActive ? SolarARColors.Lavender : SolarARColors.OnDark;
            }

            if (_planetsTabText != null)
            {
                _planetsTabText.color = planetsActive ? SolarARColors.OnDark : SolarARColors.Lavender;
            }
        }

        private void SetArActive(bool active)
        {
            if (_xrOriginRoot != null)
            {
                _xrOriginRoot.SetActive(active);
            }

            if (_arSessionRoot != null)
            {
                _arSessionRoot.SetActive(active);
            }
        }

        private static void SetScreenActive(GameObject screen, bool active)
        {
            if (screen != null && screen.activeSelf != active)
            {
                screen.SetActive(active);
            }
        }

        private static int WrapPlanetIndex(int index)
        {
            int count = PlanetOrder.Length;
            return ((index % count) + count) % count;
        }

#if UNITY_ANDROID
        private void OnCameraPermissionGranted(string permissionName)
        {
            if (permissionName != Permission.Camera)
            {
                return;
            }

            ClearPermissionCallbacks();
            EnterArExperience();
        }

        private void OnCameraPermissionDenied(string permissionName)
        {
            if (permissionName != Permission.Camera)
            {
                return;
            }

            ClearPermissionCallbacks();
            if (_permissionExplanation != null)
            {
                _permissionExplanation.text =
                    "A câmera é necessária para reconhecer os cards. Você pode permitir o acesso nas configurações do Android.";
            }
        }

        private void ClearPermissionCallbacks()
        {
            if (_permissionCallbacks == null)
            {
                return;
            }

            _permissionCallbacks.PermissionGranted -= OnCameraPermissionGranted;
            _permissionCallbacks.PermissionDenied -= OnCameraPermissionDenied;
            _permissionCallbacks.PermissionDeniedAndDontAskAgain -= OnCameraPermissionDenied;
            _permissionCallbacks = null;
        }
#endif
    }
}
