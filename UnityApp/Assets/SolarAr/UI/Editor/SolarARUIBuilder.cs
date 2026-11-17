using System;
using System.Collections.Generic;
using SolarAR.AR;
using SolarAR.Education;
using SolarAR.Education.Editor;
using SolarAR.Games.Memory;
using SolarAR.Games.PlanetOrder;
using SolarAR.Games.Quiz;
using SolarAR.Games.SpaceDetective;
using SolarAR.Planets;
using SolarAR.UI;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace SolarAR.Editor
{
    public static class SolarARUIBuilder
    {
        private const string ScenePath = "Assets/SolarAr/Scenes/MainAR.unity";
        private const string FontFolder = "Assets/SolarAr/UI/Fonts";
        private const string PrefabFolder = "Assets/SolarAr/UI/Prefabs";
        private const string LogoPath = "Assets/SolarAr/Branding/SolarARLogo.png";
        private const string CuriosityMoonPath =
            "Assets/SolarAr/UI/Illustrations/CuriosityMoon.png";

        private const string BottomNavigationPath = PrefabFolder + "/BottomNavigation.prefab";
        private const string EducationalPanelPath = PrefabFolder + "/EducationalPanel.prefab";
        private const string TitleFontAssetPath = FontFolder + "/Fredoka-SolarAR.asset";
        private const string BodyFontAssetPath = FontFolder + "/Nunito-SolarAR.asset";
        private const string UiCharacterSet =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" +
            "ÁÀÂÃÉÊÍÓÔÕÚÇáàâãéêíóôõúç" +
            " .,!?():;/-+•×‹›★→";

        private static SolarARUIFactory _ui;
        private static Texture2D _logoTexture;
        private static Texture2D _curiosityMoonTexture;
        private static QuizQuestionBank _quizQuestionBank;
        private static readonly string[] PlanetKeys =
        {
            "mercury", "earth", "mars", "jupiter", "saturn", "uranus", "neptune"
        };
        private static readonly string[] PlanetNames =
        {
            "Mercúrio", "Terra", "Marte", "Júpiter", "Saturno", "Urano", "Netuno"
        };

        private sealed class SceneReferences
        {
            public GameObject BackgroundCamera;
            public GameObject NightBackground;
            public GameObject BottomNavigation;
            public GameObject Onboarding;
            public GameObject Permission;
            public GameObject Instructions;
            public GameObject Explore;
            public GameObject Searching;
            public GameObject Recognized;
            public GameObject Information;
            public GameObject Lost;
            public GameObject Planets;
            public GameObject PlanetDetail;
            public GameObject Missions;
            public MissionHubController MissionHubController;
            public PlanetPreviewController PreviewController;
            public Graphic ExploreTabBackground;
            public Graphic PlanetsTabBackground;
            public Graphic MissionsTabBackground;
            public TMP_Text ExploreTabText;
            public TMP_Text PlanetsTabText;
            public TMP_Text MissionsTabText;
            public TMP_Text PermissionExplanation;
            public TMP_Text PlanetName;
            public TMP_Text FirstFact;
            public TMP_Text SecondFact;
            public TMP_Text RecognizedPlanetName;
            public TMP_Text RecognizedFirstFact;
            public TMP_Text RecognizedSecondFact;
            public GameObject CuriosityPanel;
            public GameObject CuriosityPrompt;
            public Graphic CuriosityAccent;
            public Graphic CuriosityFirstFactBackground;
            public Graphic CuriositySecondFactBackground;
            public RawImage[] PlanetGalleryImages;
            public TMP_Text DetailPlanetName;
            public PlanetFactCarouselController FactCarouselController;
            public PlanetPreviewDragSurface PreviewDragSurface;
        }

        [MenuItem("SolarAR/UI/Rebuild Figma MVP UI")]
        public static void BuildFromMenu()
        {
            Build();
        }

        public static void ImportTmpEssentialsAndExit()
        {
            UnityEditor.PackageManager.PackageInfo package =
                UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TMP_Text).Assembly);
            if (package == null)
            {
                Debug.LogError("[SolarAR UI] The UGUI package containing TMP was not found.");
                EditorApplication.Exit(1);
                return;
            }

            string packagePath = System.IO.Path.Combine(
                package.resolvedPath,
                "Package Resources/TMP Essential Resources.unitypackage");

            AssetDatabase.importPackageCompleted += OnTmpImportCompleted;
            AssetDatabase.importPackageCancelled += OnTmpImportCancelled;
            AssetDatabase.importPackageFailed += OnTmpImportFailed;
            UnityEditor.AssetPackage.Package.Import(packagePath, false);
        }

        public static void CapturePreviews()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Canvas canvas = GameObject.Find("SolarAR Canvas").GetComponent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            GameObject background = GameObject.Find("Background");
            GameObject bottomNavigation = FindSceneObject("BottomNavigation");

            string[] screenNames =
            {
                "01_Inicio",
                "01b_Permissao_Camera",
                "01c_Como_Usar",
                "02_Explorar",
                "02a_Procurando",
                "02b_Reconhecido",
                "02c_Informacoes",
                "02d_Card_Perdido",
                "03_Planetas",
                "04_PlanetDetail"
            };

            List<GameObject> screens = new();
            foreach (string screenName in screenNames)
            {
                screens.Add(FindSceneObject(screenName));
            }

            GameObject cameraObject = new("UI Preview Camera", typeof(Camera));
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = SolarARColors.Night;
            camera.orthographic = true;
            camera.orthographicSize = 4.575f;
            camera.aspect = 412f / 915f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.transform.rotation = Quaternion.identity;

            RenderTexture renderTexture = new(412, 915, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = renderTexture;

            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = camera;
            canvasRect.anchorMin = canvasRect.anchorMax = new Vector2(0.5f, 0.5f);
            canvasRect.pivot = new Vector2(0.5f, 0.5f);
            canvasRect.sizeDelta = new Vector2(412f, 915f);
            canvasRect.position = Vector3.zero;
            canvasRect.localScale = Vector3.one * 0.01f;

            string outputFolder = "/tmp/solarar-ui-previews";
            System.IO.Directory.CreateDirectory(outputFolder);

            for (int index = 0; index < screens.Count; index++)
            {
                foreach (GameObject screen in screens)
                {
                    screen.SetActive(false);
                }

                GameObject activeScreen = screens[index];
                activeScreen.SetActive(true);
                bool isMainExperience = index >= 3;
                bottomNavigation.SetActive(isMainExperience);
                bool planetsActive = index >= 8;
                Button exploreTab = FindChildComponent<Button>(bottomNavigation.transform, "Explore Tab");
                Button planetsTab = FindChildComponent<Button>(bottomNavigation.transform, "Planets Tab");
                exploreTab.targetGraphic.color = planetsActive ? SolarARColors.Night : SolarARColors.Blue1;
                planetsTab.targetGraphic.color = planetsActive ? SolarARColors.Blue1 : SolarARColors.Night;
                exploreTab.GetComponentInChildren<TMP_Text>().color =
                    planetsActive ? SolarARColors.Lavender : SolarARColors.OnDark;
                planetsTab.GetComponentInChildren<TMP_Text>().color =
                    planetsActive ? SolarARColors.OnDark : SolarARColors.Lavender;
                background.SetActive(index != 5 && index != 6);

                Canvas.ForceUpdateCanvases();
                camera.Render();
                Canvas.ForceUpdateCanvases();
                camera.Render();
                RenderTexture.active = renderTexture;

                Texture2D screenshot = new(412, 915, TextureFormat.RGBA32, false);
                screenshot.ReadPixels(new Rect(0f, 0f, 412f, 915f), 0, 0);
                screenshot.Apply();
                System.IO.File.WriteAllBytes(
                    System.IO.Path.Combine(outputFolder, screenNames[index] + ".png"),
                    screenshot.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(screenshot);
            }

            RenderTexture.active = null;
            camera.targetTexture = null;
            UnityEngine.Object.DestroyImmediate(renderTexture);
            UnityEngine.Object.DestroyImmediate(cameraObject);
            Debug.Log($"[SolarAR UI] Preview images written to {outputFolder}.");
        }

        public static void ValidateScene()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            List<string> errors = new();

            Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
                FindObjectsInactive.Include);
            if (canvases.Length != 1)
            {
                errors.Add($"Expected one Canvas, found {canvases.Length}.");
            }
            else
            {
                CanvasScaler scaler = canvases[0].GetComponent<CanvasScaler>();
                if (scaler == null || scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                {
                    errors.Add("Canvas Scaler is not using Scale With Screen Size.");
                }
                else if (scaler.referenceResolution != new Vector2(412f, 915f))
                {
                    errors.Add($"Unexpected reference resolution: {scaler.referenceResolution}.");
                }
            }

            EventSystem[] eventSystems = UnityEngine.Object.FindObjectsByType<EventSystem>(
                FindObjectsInactive.Include);
            if (eventSystems.Length != 1)
            {
                errors.Add($"Expected one EventSystem, found {eventSystems.Length}.");
            }

            SafeAreaController[] safeAreas = UnityEngine.Object.FindObjectsByType<SafeAreaController>(
                FindObjectsInactive.Include);
            if (safeAreas.Length != 1)
            {
                errors.Add($"Expected one SafeAreaController, found {safeAreas.Length}.");
            }

            EarthMvpUI[] controllers = UnityEngine.Object.FindObjectsByType<EarthMvpUI>(
                FindObjectsInactive.Include);
            if (controllers.Length != 1)
            {
                errors.Add($"Expected one EarthMvpUI, found {controllers.Length}.");
            }
            else
            {
                SerializedObject serialized = new(controllers[0]);
                string[] requiredReferences =
                {
                    "_trackedImageHandler",
                    "_planetRegistry",
                    "_educationalContentManager",
                    "_arSessionRoot",
                    "_xrOriginRoot",
                    "_planetPreviewController",
                    "_uiBackgroundCamera",
                    "_nightBackground",
                    "_bottomNavigation",
                    "_missionsTabBackground",
                    "_missionsTabText",
                    "_permissionExplanation",
                    "_onboardingScreen",
                    "_permissionScreen",
                    "_instructionsScreen",
                    "_exploreScreen",
                    "_searchingScreen",
                    "_recognizedScreen",
                    "_informationScreen",
                    "_lostScreen",
                    "_planetsScreen",
                    "_planetDetailScreen",
                    "_missionsScreen",
                    "_missionHubController",
                    "_planetNameText",
                    "_firstFactText",
                    "_secondFactText",
                    "_recognizedPlanetNameText",
                    "_recognizedFirstFactText",
                    "_recognizedSecondFactText",
                    "_curiosityPanel",
                    "_curiosityPrompt",
                    "_curiosityAccent",
                    "_curiosityFirstFactBackground",
                    "_curiositySecondFactBackground",
                    "_detailPlanetNameText",
                    "_factCarouselController"
                };

                foreach (string reference in requiredReferences)
                {
                    if (serialized.FindProperty(reference)?.objectReferenceValue == null)
                    {
                        errors.Add($"Missing controller reference: {reference}.");
                    }
                }

                SerializedProperty galleryImages = serialized.FindProperty("_planetGalleryImages");
                if (galleryImages == null || galleryImages.arraySize != PlanetKeys.Length)
                {
                    errors.Add("Planet gallery must contain seven 3D thumbnail outputs.");
                }
                else
                {
                    for (int index = 0; index < galleryImages.arraySize; index++)
                    {
                        if (galleryImages.GetArrayElementAtIndex(index).objectReferenceValue == null)
                        {
                            errors.Add($"Missing gallery thumbnail output at index {index}.");
                        }
                    }
                }

                GameObject backgroundCameraObject = serialized
                    .FindProperty("_uiBackgroundCamera")
                    ?.objectReferenceValue as GameObject;
                Camera backgroundCamera = backgroundCameraObject != null
                    ? backgroundCameraObject.GetComponent<Camera>()
                    : null;
                if (backgroundCamera == null)
                {
                    errors.Add("UIBackgroundCamera does not contain a Camera component.");
                }
                else
                {
                    if (backgroundCamera.cullingMask != 0)
                    {
                        errors.Add("UIBackgroundCamera Culling Mask must be Nothing.");
                    }

                    if (backgroundCamera.clearFlags != CameraClearFlags.SolidColor ||
                        backgroundCamera.backgroundColor != SolarARColors.Night)
                    {
                        errors.Add("UIBackgroundCamera must use the Night solid color.");
                    }

                    if (backgroundCamera.GetComponent<AudioListener>() != null)
                    {
                        errors.Add("UIBackgroundCamera must not have an AudioListener.");
                    }
                }
            }

            QuizManager[] quizManagers = UnityEngine.Object.FindObjectsByType<QuizManager>(
                FindObjectsInactive.Include);
            if (quizManagers.Length != 1)
            {
                errors.Add($"Expected one QuizManager, found {quizManagers.Length}.");
            }

            QuizScreenController[] quizControllers =
                UnityEngine.Object.FindObjectsByType<QuizScreenController>(
                    FindObjectsInactive.Include);
            if (quizControllers.Length != 1)
            {
                errors.Add($"Expected one QuizScreenController, found {quizControllers.Length}.");
            }

            MissionHubController[] missionHubs =
                UnityEngine.Object.FindObjectsByType<MissionHubController>(
                    FindObjectsInactive.Include);
            if (missionHubs.Length != 1)
            {
                errors.Add($"Expected one MissionHubController, found {missionHubs.Length}.");
            }

            ValidateSingleComponent<MemoryScreenController>(errors);
            ValidateSingleComponent<PlanetOrderScreenController>(errors);
            ValidateSingleComponent<SpaceDetectiveScreenController>(errors);
            ValidateSingleComponent<PlanetFactCarouselController>(errors);
            ValidateSingleComponent<PlanetPreviewDragSurface>(errors);

            QuizQuestionBankSeeder.ValidateBank(
                AssetDatabase.LoadAssetAtPath<QuizQuestionBank>(QuizQuestionBankSeeder.BankPath),
                errors);
            PlanetFactsSeeder.ValidateAll(errors);

            foreach (TMP_Text text in UnityEngine.Object.FindObjectsByType<TMP_Text>(
                         FindObjectsInactive.Include))
            {
                string fontPath = AssetDatabase.GetAssetPath(text.font);
                if (fontPath != TitleFontAssetPath && fontPath != BodyFontAssetPath)
                {
                    errors.Add($"Unexpected font on {GetHierarchyPath(text.transform)}: {fontPath}.");
                }
            }

            RawImage[] rawImages = UnityEngine.Object.FindObjectsByType<RawImage>(
                FindObjectsInactive.Include);
            int curiosityMoonCount = 0;
            int quizMoonCount = 0;
            foreach (RawImage rawImage in rawImages)
            {
                bool isCuriosityMoon = rawImage.name == "Curiosity Moon Artwork" ||
                                       rawImage.name == "Journal Moon Mascot";
                bool isQuizMoon = rawImage.name == "Quiz Intro Moon Mascot" ||
                                  rawImage.name == "Quiz Result Moon Mascot";
                if (!isCuriosityMoon && !isQuizMoon)
                {
                    continue;
                }

                if (isCuriosityMoon)
                {
                    curiosityMoonCount++;
                }

                if (isQuizMoon)
                {
                    quizMoonCount++;
                }

                if (AssetDatabase.GetAssetPath(rawImage.texture) != CuriosityMoonPath)
                {
                    errors.Add(
                        $"Curiosity mascot must use the official asset: {CuriosityMoonPath}.");
                }
            }

            if (curiosityMoonCount != 2)
            {
                errors.Add(
                    $"Expected two official curiosity moon images, found {curiosityMoonCount}.");
            }

            if (quizMoonCount != 2)
            {
                errors.Add($"Expected two official quiz moon images, found {quizMoonCount}.");
            }

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
                {
                    foreach (Component component in transform.GetComponents<Component>())
                    {
                        if (component == null)
                        {
                            errors.Add($"Missing Script on {GetHierarchyPath(transform)}.");
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                throw new InvalidOperationException(
                    "SolarAR UI validation failed:\n- " + string.Join("\n- ", errors));
            }

            Debug.Log("[SolarAR UI Validation] PASS: Canvas, SafeArea, EventSystem, references and scripts are valid.");
        }

        private static string GetHierarchyPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }

        private static void OnTmpImportCompleted(string packageName)
        {
            RemoveTmpImportCallbacks();
            AssetDatabase.SaveAssets();
            Debug.Log($"[SolarAR UI] TMP resources imported: {packageName}");
            EditorApplication.Exit(0);
        }

        private static void OnTmpImportCancelled(string packageName)
        {
            RemoveTmpImportCallbacks();
            Debug.LogError($"[SolarAR UI] TMP import cancelled: {packageName}");
            EditorApplication.Exit(1);
        }

        private static void OnTmpImportFailed(string packageName, string errorMessage)
        {
            RemoveTmpImportCallbacks();
            Debug.LogError($"[SolarAR UI] TMP import failed ({packageName}): {errorMessage}");
            EditorApplication.Exit(1);
        }

        private static void RemoveTmpImportCallbacks()
        {
            AssetDatabase.importPackageCompleted -= OnTmpImportCompleted;
            AssetDatabase.importPackageCancelled -= OnTmpImportCancelled;
            AssetDatabase.importPackageFailed -= OnTmpImportFailed;
        }

        public static void Build()
        {
            AssetDatabase.Refresh();
            EnsureFolder(PrefabFolder);
            EnsureTmpEssentials();

            TMP_FontAsset titleFont = EnsureFontAsset(
                FontFolder + "/Fredoka-Variable.ttf",
                TitleFontAssetPath);
            TMP_FontAsset bodyFont = EnsureFontAsset(
                FontFolder + "/Nunito-Variable.ttf",
                BodyFontAssetPath);

            if (titleFont == null || bodyFont == null)
            {
                throw new InvalidOperationException("SolarAR TMP font assets could not be created.");
            }

            _ui = new SolarARUIFactory(titleFont, bodyFont);
            _logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);
            if (_logoTexture == null)
            {
                throw new InvalidOperationException($"Official SolarAR logo not found: {LogoPath}");
            }

            _curiosityMoonTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(CuriosityMoonPath);
            if (_curiosityMoonTexture == null)
            {
                throw new InvalidOperationException(
                    $"Official curiosity moon not found: {CuriosityMoonPath}");
            }

            _quizQuestionBank = QuizQuestionBankSeeder.EnsureBank();
            if (_quizQuestionBank == null)
            {
                throw new InvalidOperationException("Quiz question bank could not be created.");
            }

            PlanetFactsSeeder.EnsureAll();

            BuildPrefabs();

            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject manager = FindSceneObject("EducationalContentManager");
            GameObject arSession = FindSceneObject("AR Session");
            GameObject xrOrigin = FindSceneObject("XR Origin");

            if (manager == null || arSession == null || xrOrigin == null)
            {
                throw new InvalidOperationException("Required MainAR roots were not found.");
            }

            GameObject existingCanvas = FindSceneObjectOrNull("SolarAR Canvas");
            if (existingCanvas != null)
            {
                UnityEngine.Object.DestroyImmediate(existingCanvas);
            }

            GameObject existingBackgroundCamera = FindSceneObjectOrNull("UIBackgroundCamera");
            if (existingBackgroundCamera != null)
            {
                UnityEngine.Object.DestroyImmediate(existingBackgroundCamera);
            }

            GameObject existingPreviewSystem = FindSceneObjectOrNull("PlanetPreviewSystem");
            if (existingPreviewSystem != null)
            {
                UnityEngine.Object.DestroyImmediate(existingPreviewSystem);
            }

            EnsureSingleEventSystem();

            EarthMvpUI existingController = manager.GetComponent<EarthMvpUI>();
            if (existingController != null)
            {
                UnityEngine.Object.DestroyImmediate(existingController);
            }

            EarthMvpUI controller = manager.AddComponent<EarthMvpUI>();
            SceneReferences references = BuildCanvas(controller, scene);

            AssignControllerReferences(
                controller,
                manager.GetComponent<EducationalContentManager>(),
                xrOrigin.GetComponent<TrackedImageHandler>(),
                arSession,
                xrOrigin,
                references);

            arSession.SetActive(false);
            xrOrigin.SetActive(false);
            SetInitialScreenStates(references);

            EditorUtility.SetDirty(controller);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[SolarAR UI] Serialized Figma MVP UI rebuilt successfully.");
        }

        private static void EnsureTmpEssentials()
        {
            if (TMP_Settings.LoadDefaultSettings() != null &&
                Shader.Find("TextMeshPro/Mobile/Distance Field") != null)
            {
                return;
            }

            UnityEditor.PackageManager.PackageInfo package =
                UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TMP_Text).Assembly);
            if (package == null)
            {
                throw new InvalidOperationException("The UGUI package containing TMP was not found.");
            }

            string packagePath = System.IO.Path.Combine(
                package.resolvedPath,
                "Package Resources/TMP Essential Resources.unitypackage");
            UnityEditor.AssetPackage.Package.Import(packagePath, false);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            if (TMP_Settings.LoadDefaultSettings() == null ||
                Shader.Find("TextMeshPro/Mobile/Distance Field") == null)
            {
                throw new InvalidOperationException("TMP Essential Resources could not be imported.");
            }
        }

        private static TMP_FontAsset EnsureFontAsset(string fontPath, string assetPath)
        {
            TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (existing != null)
            {
                WarmFontAsset(existing);
                return existing;
            }

            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            if (sourceFont == null)
            {
                Debug.LogError($"[SolarAR UI] Font not found: {fontPath}");
                return null;
            }

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                90,
                9,
                GlyphRenderMode.SDFAA,
                1024,
                1024,
                AtlasPopulationMode.Dynamic,
                true);
            fontAsset.name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            AssetDatabase.CreateAsset(fontAsset, assetPath);

            if (fontAsset.material != null && !AssetDatabase.Contains(fontAsset.material))
            {
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            }

            foreach (Texture2D atlas in fontAsset.atlasTextures)
            {
                if (atlas != null && !AssetDatabase.Contains(atlas))
                {
                    AssetDatabase.AddObjectToAsset(atlas, fontAsset);
                }
            }

            EditorUtility.SetDirty(fontAsset);
            WarmFontAsset(fontAsset);
            AssetDatabase.SaveAssets();
            return fontAsset;
        }

        private static void WarmFontAsset(TMP_FontAsset fontAsset)
        {
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            fontAsset.TryAddCharacters(UiCharacterSet, out string missingCharacters, true);
            if (!string.IsNullOrEmpty(missingCharacters))
            {
                Debug.LogWarning(
                    $"[SolarAR UI] {fontAsset.name} is missing glyphs: {missingCharacters}");
            }

            EditorUtility.SetDirty(fontAsset);
        }

        private static void BuildPrefabs()
        {
            SavePrefab(BuildBottomNavigationPrefab(), BottomNavigationPath);
            SavePrefab(BuildEducationalPanelPrefab(), EducationalPanelPath);
        }

        private static GameObject BuildBottomNavigationPrefab()
        {
            RectTransform root = _ui.Panel(
                "Bottom Navigation",
                null,
                SolarARColors.Blue4,
                20f);
            root.sizeDelta = new Vector2(380f, 72f);

            Button explore = _ui.Button(
                "Explore Tab",
                root,
                "Explorar",
                SolarARColors.Blue1,
                SolarARColors.OnDark,
                null,
                16f,
                14f);
            SolarARUIFactory.Place(
                explore.GetComponent<RectTransform>(),
                -124f,
                0f,
                116f,
                56f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            explore.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

            Button planets = _ui.Button(
                "Planets Tab",
                root,
                "Planetas",
                SolarARColors.Night,
                SolarARColors.Lavender,
                null,
                16f,
                14f);
            planets.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            SolarARUIFactory.Place(
                planets.GetComponent<RectTransform>(),
                0f,
                0f,
                116f,
                56f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));

            Button missions = _ui.Button(
                "Missions Tab",
                root,
                "Missões",
                SolarARColors.Night,
                SolarARColors.Lavender,
                null,
                16f,
                14f);
            missions.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            SolarARUIFactory.Place(
                missions.GetComponent<RectTransform>(),
                124f,
                0f,
                116f,
                56f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            return root.gameObject;
        }

        private static GameObject BuildEducationalPanelPrefab()
        {
            RectTransform root = _ui.Panel(
                "Educational Panel",
                null,
                SolarARColors.Panel,
                28f);
            root.sizeDelta = new Vector2(380f, 360f);

            TextMeshProUGUI name = _ui.Text(
                "Planet Name",
                root,
                "Terra",
                28f,
                SolarARColors.Ink,
                TextAlignmentOptions.MidlineLeft,
                true,
                FontStyles.Bold);
            SolarARUIFactory.Top(name.rectTransform, 16f, 48f, 24f, 72f);

            Button close = _ui.IconButton(
                "Close Button",
                root,
                "×",
                null,
                SolarARColors.Panel,
                SolarARColors.Blue1,
                48f);
            SolarARUIFactory.Place(
                close.GetComponent<RectTransform>(),
                -12f,
                -12f,
                48f,
                48f,
                Vector2.one,
                Vector2.one);

            RectTransform firstCard = _ui.Panel(
                "First Fact Card",
                root,
                SolarARColors.Secondary,
                20f);
            SolarARUIFactory.Place(
                firstCard,
                16f,
                -80f,
                166f,
                144f,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f));
            TextMeshProUGUI firstFact = _ui.Text(
                "First Fact Text",
                firstCard,
                "É o único planeta conhecido que tem vida.",
                17f,
                SolarARColors.Ink,
                TextAlignmentOptions.TopLeft);
            SolarARUIFactory.Stretch(firstFact.rectTransform, 16f, 16f, 16f, 16f);

            RectTransform secondCard = _ui.Panel(
                "Second Fact Card",
                root,
                SolarARColors.FactSun,
                20f);
            SolarARUIFactory.Place(
                secondCard,
                -16f,
                -80f,
                166f,
                144f,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f));
            TextMeshProUGUI secondFact = _ui.Text(
                "Second Fact Text",
                secondCard,
                "Tem 1 lua, chamada Lua.",
                17f,
                SolarARColors.Ink,
                TextAlignmentOptions.TopLeft);
            SolarARUIFactory.Stretch(secondFact.rectTransform, 16f, 16f, 16f, 16f);

            TextMeshProUGUI gesture = _ui.Text(
                "Gesture Hint",
                root,
                "Gire • Aproxime • Reduza",
                15f,
                SolarARColors.Ink,
                TextAlignmentOptions.Center);
            SolarARUIFactory.Bottom(gesture.rectTransform, 12f, 28f, 16f, 16f);
            return root.gameObject;
        }

        private static SceneReferences BuildCanvas(EarthMvpUI controller, Scene scene)
        {
            SceneReferences references = new();

            references.BackgroundCamera = BuildBackgroundCamera(scene);

            GameObject canvasObject = new(
                "SolarAR Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            SceneManager.MoveGameObjectToScene(canvasObject, scene);

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(412f, 915f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            references.NightBackground = BuildNightBackground(canvasObject.transform).gameObject;

            RectTransform safeArea = _ui.Rect("SafeArea", canvasObject.transform);
            SolarARUIFactory.Stretch(safeArea);
            safeArea.gameObject.AddComponent<SafeAreaController>();

            references.Onboarding = BuildOnboarding(safeArea, controller);
            references.Permission = BuildPermission(
                safeArea,
                controller,
                out references.PermissionExplanation);
            references.Instructions = BuildInstructions(safeArea, controller);

            RectTransform arScreen = _ui.Rect("ARScreen", safeArea);
            SolarARUIFactory.Stretch(arScreen);
            references.Explore = BuildExplore(arScreen, controller);
            references.Searching = BuildSearching(arScreen, controller);
            references.Recognized = BuildRecognized(
                arScreen,
                controller,
                out references.RecognizedPlanetName,
                out references.RecognizedFirstFact,
                out references.RecognizedSecondFact,
                out references.CuriosityPanel,
                out references.CuriosityPrompt,
                out references.CuriosityAccent,
                out references.CuriosityFirstFactBackground,
                out references.CuriositySecondFactBackground);
            references.Information = BuildInformation(
                arScreen,
                controller,
                out references.PlanetName,
                out references.FirstFact,
                out references.SecondFact);
            references.Lost = BuildLost(arScreen, controller);

            references.Planets = BuildPlanets(
                safeArea,
                controller,
                out references.PlanetGalleryImages);
            references.PlanetDetail = BuildPlanetDetail(
                safeArea,
                controller,
                out RawImage previewImage,
                out references.DetailPlanetName,
                out references.FactCarouselController,
                out references.PreviewDragSurface);
            references.Missions = BuildMissions(
                safeArea,
                out references.MissionHubController);
            references.PreviewController = BuildPreviewCamera(scene, previewImage);
            SerializedObject dragSurface = new(references.PreviewDragSurface);
            SetObject(dragSurface, "_previewController", references.PreviewController);
            dragSurface.ApplyModifiedPropertiesWithoutUndo();

            references.BottomNavigation = InstantiatePrefabForAuthoring(
                BottomNavigationPath,
                arScreen,
                "BottomNavigation");
            SolarARUIFactory.Bottom(
                references.BottomNavigation.GetComponent<RectTransform>(),
                8f,
                72f,
                16f,
                16f);

            Button exploreTab = FindChildComponent<Button>(
                references.BottomNavigation.transform,
                "Explore Tab");
            Button planetsTab = FindChildComponent<Button>(
                references.BottomNavigation.transform,
                "Planets Tab");
            Button missionsTab = FindChildComponent<Button>(
                references.BottomNavigation.transform,
                "Missions Tab");
            Wire(exploreTab, controller.ShowExploreTab);
            Wire(planetsTab, controller.ShowPlanets);
            Wire(missionsTab, controller.ShowMissions);
            references.ExploreTabBackground = exploreTab.targetGraphic;
            references.PlanetsTabBackground = planetsTab.targetGraphic;
            references.MissionsTabBackground = missionsTab.targetGraphic;
            references.ExploreTabText = exploreTab.GetComponentInChildren<TMP_Text>();
            references.PlanetsTabText = planetsTab.GetComponentInChildren<TMP_Text>();
            references.MissionsTabText = missionsTab.GetComponentInChildren<TMP_Text>();

            return references;
        }

        private static PlanetPreviewController BuildPreviewCamera(Scene scene, RawImage outputImage)
        {
            const int previewLayer = 30;
            GameObject system = new("PlanetPreviewSystem", typeof(PlanetPreviewController));
            SceneManager.MoveGameObjectToScene(system, scene);

            GameObject cameraObject = new("PlanetPreviewCamera", typeof(Camera));
            cameraObject.transform.SetParent(system.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 0f, -0.2f);
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            camera.cullingMask = 1 << previewLayer;
            camera.orthographic = true;
            camera.orthographicSize = 0.035f;
            camera.nearClipPlane = 0.01f;
            camera.farClipPlane = 2f;
            camera.allowHDR = false;
            camera.enabled = false;

            GameObject previewRoot = new("PlanetPreviewRoot");
            previewRoot.transform.SetParent(system.transform, false);
            previewRoot.layer = previewLayer;

            GameObject lightObject = new("PlanetPreviewLight", typeof(Light));
            lightObject.transform.SetParent(system.transform, false);
            lightObject.transform.localRotation = Quaternion.Euler(35f, -35f, 0f);
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.4f;
            light.cullingMask = 1 << previewLayer;

            PlanetPreviewController controller = system.GetComponent<PlanetPreviewController>();
            SerializedObject serialized = new(controller);
            SetObject(serialized, "_previewCamera", camera);
            SetObject(serialized, "_previewRoot", previewRoot.transform);
            SetObject(serialized, "_outputImage", outputImage);
            serialized.FindProperty("_previewLayer").intValue = previewLayer;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return controller;
        }

        private static GameObject BuildBackgroundCamera(Scene scene)
        {
            GameObject cameraObject = new("UIBackgroundCamera", typeof(Camera));
            SceneManager.MoveGameObjectToScene(cameraObject, scene);

            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = SolarARColors.Night;
            camera.cullingMask = 0;
            camera.depth = -100f;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            return cameraObject;
        }

        private static RectTransform BuildNightBackground(Transform parent)
        {
            GameObject background = new(
                "Background",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(VerticalGradientGraphic));
            RectTransform rect = background.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            SolarARUIFactory.Stretch(rect);

            VerticalGradientGraphic gradient = background.GetComponent<VerticalGradientGraphic>();
            gradient.SetColors(
                Color.Lerp(SolarARColors.DeepSpace, SolarARColors.Purple, 0.18f),
                SolarARColors.DeepSpace);
            gradient.raycastTarget = false;

            AddStars(rect);
            return rect;
        }

        private static void AddStars(RectTransform parent)
        {
            (float x, float y, float size, Color color)[] stars =
            {
                (28f, 104f, 3f, SolarARColors.OnDark),
                (84f, 176f, 5f, SolarARColors.Lavender),
                (146f, 80f, 3f, SolarARColors.Cyan),
                (224f, 136f, 4f, SolarARColors.OnDark),
                (330f, 96f, 5f, SolarARColors.Primary),
                (382f, 206f, 3f, SolarARColors.OnDark),
                (44f, 314f, 4f, SolarARColors.Cyan),
                (188f, 352f, 3f, SolarARColors.OnDark),
                (360f, 402f, 4f, SolarARColors.Lavender),
                (72f, 508f, 3f, SolarARColors.OnDark),
                (286f, 536f, 4f, SolarARColors.Cyan),
                (394f, 618f, 3f, SolarARColors.OnDark),
                (34f, 714f, 4f, SolarARColors.Lavender),
                (178f, 762f, 3f, SolarARColors.OnDark),
                (344f, 828f, 4f, SolarARColors.Cyan)
            };

            foreach ((float x, float y, float size, Color color) in stars)
            {
                RectTransform star = _ui.Circle("Star", parent, color, size);
                SolarARUIFactory.Place(
                    star,
                    x,
                    -y,
                    size,
                    size,
                    new Vector2(0f, 1f),
                    new Vector2(0.5f, 0.5f));
            }
        }

        private static GameObject BuildOnboarding(RectTransform parent, EarthMvpUI controller)
        {
            RectTransform root = CreateScreen("01_Inicio", parent);

            AddText(root, "Greeting", "Olá, pequeno explorador!", 18f, 32f, 32f, 32f, 32f, false);
            CreateLogoArt(root, 88f);
            AddText(root, "Brand", "SolarAR", 38f, 286f, 52f, 24f, 24f, true);
            AddText(
                root,
                "Tagline",
                "O universo nas suas mãos!",
                28f,
                338f,
                68f,
                40f,
                40f,
                true);
            AddText(
                root,
                "Introduction",
                "Descubra os planetas e veja a magia acontecer com seus cards.",
                18f,
                420f,
                64f,
                40f,
                40f,
                false,
                SolarARColors.Lavender);

            Button explore = AddButton(
                root,
                "Explore Button",
                "Explorar com a câmera",
                SolarARColors.Secondary,
                SolarARColors.Ink,
                566f,
                56f,
                32f,
                32f);
            Wire(explore, controller.BeginCameraFlow);

            Button instructions = AddButton(
                root,
                "How To Button",
                "?  Como usar",
                SolarARColors.Night,
                SolarARColors.Lavender,
                654f,
                56f,
                80f,
                80f,
                20f,
                17f);
            Wire(instructions, controller.ShowInstructions);

            return root.gameObject;
        }

        private static GameObject BuildPermission(
            RectTransform parent,
            EarthMvpUI controller,
            out TMP_Text explanation)
        {
            RectTransform root = CreateScreen("01b_Permissao_Camera", parent);
            BuildHeader(root, "Vamos explorar?", controller.BackToOnboarding, null);

            RectTransform card = _ui.Panel("Permission Card", root, SolarARColors.Panel, 28f);
            SolarARUIFactory.Top(card, 112f, 640f, 24f, 24f);

            BuildCameraIllustration(card, 40f);

            explanation = AddText(
                card,
                "Explanation",
                "Para ver os planetas em 3D, o SolarAR precisa enxergar seus cards.",
                18f,
                212f,
                72f,
                32f,
                32f,
                false,
                SolarARColors.Ink,
                TextAlignmentOptions.Center,
                FontStyles.Normal);

            AddText(
                card,
                "Camera Support",
                "A câmera encontra o card e traz o planeta para perto de você.",
                16f,
                300f,
                48f,
                32f,
                32f,
                false,
                SolarARColors.Ink);

            Button openCamera = AddButton(
                card,
                "Open Camera Button",
                "Abrir câmera",
                SolarARColors.Secondary,
                SolarARColors.Ink,
                372f,
                64f,
                48f,
                48f,
                24f,
                20f,
                true);
            Wire(openCamera, controller.RequestCameraPermission);

            RectTransform alternative = _ui.Panel(
                "Gallery Alternative",
                card,
                SolarARColors.Secondary,
                20f);
            SolarARUIFactory.Top(alternative, 468f, 72f, 24f, 24f);
            AddText(
                alternative,
                "Alternative Text",
                "Você também pode conhecer os planetas sem usar a câmera.",
                15f,
                8f,
                56f,
                16f,
                16f,
                false,
                SolarARColors.Ink,
                TextAlignmentOptions.Center,
                FontStyles.Bold);

            AddText(
                root,
                "Footer",
                "Uma aventura segura e no seu ritmo.",
                14f,
                776f,
                40f,
                48f,
                48f,
                false,
                SolarARColors.Lavender,
                TextAlignmentOptions.Center,
                FontStyles.Bold);
            return root.gameObject;
        }

        private static GameObject BuildInstructions(RectTransform parent, EarthMvpUI controller)
        {
            RectTransform root = CreateScreen("01c_Como_Usar", parent);
            BuildHeader(root, "Como usar", controller.BackFromInstructions, null);

            RectTransform card = _ui.Panel("Instructions Card", root, SolarARColors.Panel, 28f);
            SolarARUIFactory.Top(card, 112f, 672f, 24f, 24f);
            AddText(
                card,
                "Title",
                "Seu planeta em 3 passos",
                28f,
                24f,
                72f,
                24f,
                24f,
                true,
                SolarARColors.Ink);

            AddInstructionStep(
                card,
                112f,
                "1",
                "Escolha um card",
                "Coloque o card em uma superfície\nbem iluminada.");
            AddInstructionStep(
                card,
                248f,
                "2",
                "Aponte a câmera",
                "Deixe o card inteiro dentro da\nmoldura.");
            AddInstructionStep(
                card,
                384f,
                "3",
                "Explore o planeta",
                "Gire e aproxime para descobrir\ncada detalhe!");

            AddText(
                card,
                "Warning",
                "Não cubra o card com a mão.",
                16f,
                520f,
                40f,
                24f,
                24f,
                false,
                SolarARColors.Ink,
                TextAlignmentOptions.Center,
                FontStyles.Bold);
            return root.gameObject;
        }

        private static void AddInstructionStep(
            Transform parent,
            float top,
            string number,
            string title,
            string description)
        {
            RectTransform step = _ui.Panel("Step " + number, parent, SolarARColors.Secondary, 20f);
            SolarARUIFactory.Top(step, top, 120f, 16f, 16f);

            RectTransform badge = _ui.Circle("Number", step, SolarARColors.Cyan, 32f);
            SolarARUIFactory.Place(
                badge,
                16f,
                -16f,
                32f,
                32f,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f));
            TextMeshProUGUI numberText = _ui.Text(
                "Label",
                badge,
                number,
                16f,
                SolarARColors.Ink,
                TextAlignmentOptions.Center,
                false,
                FontStyles.Bold);
            SolarARUIFactory.Stretch(numberText.rectTransform);

            AddText(step, "Step Title", title, 18f, 14f, 32f, 64f, 16f, false, SolarARColors.Ink, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            AddText(
                step,
                "Step Description",
                description,
                16f,
                52f,
                52f,
                64f,
                16f,
                false,
                SolarARColors.Ink,
                TextAlignmentOptions.TopLeft,
                FontStyles.Bold);
        }

        private static GameObject BuildExplore(RectTransform parent, EarthMvpUI controller)
        {
            RectTransform root = CreateScreen("02_Explorar", parent);
            BuildHeader(root, "Explorar", controller.BackToOnboarding, controller.ShowInstructions);

            AddText(
                root,
                "Title",
                "Vamos encontrar um planeta?",
                28f,
                104f,
                72f,
                32f,
                32f,
                true);
            AddText(
                root,
                "Support",
                "Pegue um card e prepare a câmera.",
                17f,
                176f,
                40f,
                32f,
                32f,
                false,
                SolarARColors.Lavender);

            RectTransform frame = CreateScanFrame(root, 246f, 390f);
            CreateSimpleIcon(frame, "Camera Placeholder", "▣", 130f, 72f);
            AddText(
                frame,
                "Placeholder Text",
                "O planeta vai aparecer aqui!",
                17f,
                226f,
                56f,
                40f,
                40f,
                false,
                SolarARColors.OnDark);

            AddText(
                root,
                "Footer",
                "Aponte para um card do SolarAR.",
                15f,
                688f,
                32f,
                32f,
                32f,
                false,
                SolarARColors.Lavender);
            return root.gameObject;
        }

        private static GameObject BuildSearching(RectTransform parent, EarthMvpUI controller)
        {
            RectTransform root = CreateScreen("02a_Procurando", parent);
            BuildHeader(root, "Explorar", controller.BackToOnboarding, controller.ShowInstructions);
            RectTransform frame = CreateScanFrame(root, 138f, 500f);

            CreateSimpleIcon(frame, "Tracking Icon", "◎", 108f, 72f);
            AddText(
                frame,
                "Prompt",
                "Aponte a câmera\npara um card",
                30f,
                206f,
                104f,
                40f,
                40f,
                true);
            AddText(
                frame,
                "Searching Status",
                "◌  Procurando um card...",
                16f,
                352f,
                40f,
                40f,
                40f,
                false,
                SolarARColors.Cyan);

            AddText(
                root,
                "Guidance",
                "Mantenha o card inteiro na moldura e o celular paradinho.",
                16f,
                680f,
                64f,
                32f,
                32f,
                false,
                SolarARColors.Lavender);
            return root.gameObject;
        }

        private static GameObject BuildRecognized(
            RectTransform parent,
            EarthMvpUI controller,
            out TMP_Text planetName,
            out TMP_Text firstFact,
            out TMP_Text secondFact,
            out GameObject curiosityPanel,
            out GameObject curiosityPrompt,
            out Graphic accent,
            out Graphic firstFactBackground,
            out Graphic secondFactBackground)
        {
            RectTransform root = CreateScreen("02b_Reconhecido", parent);
            BuildHeader(root, string.Empty, controller.BackToOnboarding, controller.ShowInstructions);

            RectTransform chip = _ui.Panel("Success Chip", root, SolarARColors.Success, 20f);
            SolarARUIFactory.Top(chip, 24f, 48f, 104f, 104f);
            AddText(
                chip,
                "Label",
                "Card reconhecido!",
                17f,
                0f,
                48f,
                40f,
                8f,
                false,
                SolarARColors.Ink,
                TextAlignmentOptions.Center,
                FontStyles.Bold);
            CreateCheckMark(chip);

            RectTransform journal = _ui.Panel(
                "Moon Curiosity Journal",
                root,
                SolarARColors.Panel,
                28f);
            SolarARUIFactory.Bottom(journal, 96f, 400f, 16f, 16f);
            curiosityPanel = journal.gameObject;

            SolarARColors.GetPlanetPalette("earth", out Color earthBlue, out Color earthGreen);
            RectTransform accentBar = _ui.Panel(
                "Planet Accent",
                journal,
                earthBlue,
                5f);
            SolarARUIFactory.Top(accentBar, 0f, 10f, 0f, 0f);
            accent = accentBar.GetComponent<Graphic>();

            AddText(
                journal,
                "Editorial Label",
                "CADERNO DE BORDO  •  CURIOSIDADES",
                12f,
                24f,
                24f,
                24f,
                64f,
                false,
                SolarARColors.Blue4,
                TextAlignmentOptions.MidlineLeft,
                FontStyles.Bold);
            planetName = AddText(
                journal,
                "Recognized Planet Name",
                "Terra",
                28f,
                52f,
                42f,
                24f,
                64f,
                true,
                SolarARColors.Ink,
                TextAlignmentOptions.MidlineLeft,
                FontStyles.Bold);

            RectTransform journalMoon = CreateCuriosityMoonArtwork(
                journal,
                "Journal Moon Mascot");
            SolarARUIFactory.Place(
                journalMoon,
                -70f,
                -8f,
                42f,
                56f,
                Vector2.one,
                Vector2.one);

            Button close = _ui.IconButton(
                "Close Curiosities Button",
                journal,
                "×",
                null,
                SolarARColors.Panel,
                SolarARColors.Blue1,
                48f);
            SolarARUIFactory.Place(
                close.GetComponent<RectTransform>(),
                -12f,
                -16f,
                48f,
                48f,
                Vector2.one,
                Vector2.one);
            Wire(close, controller.CloseCuriosities);

            RectTransform firstCard = _ui.Panel(
                "Recognized First Fact Card",
                journal,
                Color.Lerp(earthBlue, SolarARColors.Panel, 0.72f),
                18f);
            SolarARUIFactory.Place(
                firstCard,
                16f,
                -104f,
                348f,
                108f,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f));
            firstFactBackground = firstCard.GetComponent<Graphic>();
            firstFact = _ui.Text(
                "Recognized First Fact Text",
                firstCard,
                "É o único planeta conhecido que tem vida.",
                17f,
                SolarARColors.Ink,
                TextAlignmentOptions.MidlineLeft,
                false,
                FontStyles.Bold);
            SolarARUIFactory.Stretch(firstFact.rectTransform, 18f, 12f, 18f, 12f);

            RectTransform secondCard = _ui.Panel(
                "Recognized Second Fact Card",
                journal,
                Color.Lerp(earthGreen, SolarARColors.Panel, 0.58f),
                18f);
            SolarARUIFactory.Place(
                secondCard,
                16f,
                -224f,
                348f,
                108f,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f));
            secondFactBackground = secondCard.GetComponent<Graphic>();
            secondFact = _ui.Text(
                "Recognized Second Fact Text",
                secondCard,
                "Tem 1 lua, chamada Lua.",
                17f,
                SolarARColors.Ink,
                TextAlignmentOptions.MidlineLeft,
                false,
                FontStyles.Bold);
            SolarARUIFactory.Stretch(secondFact.rectTransform, 18f, 12f, 18f, 12f);

            AddText(
                journal,
                "Interaction Hint",
                "Arraste em qualquer direção • Use a pinça para aproximar",
                12f,
                352f,
                24f,
                16f,
                16f,
                false,
                SolarARColors.Blue4,
                TextAlignmentOptions.Center,
                FontStyles.Bold);

            RectTransform prompt = _ui.Rect("Curiosity Prompt", root);
            SolarARUIFactory.Stretch(prompt);
            curiosityPrompt = prompt.gameObject;

            Button mascot = _ui.Button(
                "Curiosity Mascot Button",
                prompt,
                string.Empty,
                Color.clear,
                SolarARColors.Ink,
                null,
                48f,
                1f);
            SolarARUIFactory.Place(
                mascot.GetComponent<RectTransform>(),
                20f,
                106f,
                96f,
                128f,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f));
            RectTransform mascotArtwork = CreateCuriosityMoonArtwork(
                mascot.transform,
                "Curiosity Moon Artwork");
            SolarARUIFactory.Stretch(mascotArtwork);
            Wire(mascot, controller.ToggleCuriosities);

            RectTransform indicator = _ui.Panel(
                "Curiosity Indicator",
                prompt,
                SolarARColors.Panel,
                20f);
            SolarARUIFactory.Place(
                indicator,
                104f,
                136f,
                142f,
                52f,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f));
            TMP_Text indicatorText = _ui.Text(
                "Curiosity Indicator Text",
                indicator,
                "Curiosidades",
                16f,
                SolarARColors.Ink,
                TextAlignmentOptions.Center,
                true,
                FontStyles.Bold);
            SolarARUIFactory.Stretch(indicatorText.rectTransform, 12f, 6f, 12f, 6f);

            RectTransform thoughtDot = _ui.Circle(
                "Curiosity Thought Dot",
                prompt,
                SolarARColors.Panel,
                14f);
            SolarARUIFactory.Place(
                thoughtDot,
                96f,
                128f,
                14f,
                14f,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f));

            journal.gameObject.SetActive(false);
            return root.gameObject;
        }

        private static GameObject BuildInformation(
            RectTransform parent,
            EarthMvpUI controller,
            out TMP_Text planetName,
            out TMP_Text firstFact,
            out TMP_Text secondFact)
        {
            RectTransform root = CreateScreen("02c_Informacoes", parent);

            RectTransform dim = _ui.Panel(
                "Camera Dim",
                root,
                new Color(SolarARColors.Night.r, SolarARColors.Night.g, SolarARColors.Night.b, 0.48f));
            SolarARUIFactory.Stretch(dim);

            BuildHeader(root, string.Empty, controller.BackToOnboarding, controller.ShowInstructions);

            GameObject orbitObject = new(
                "Planet Orbit",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(EllipseOutlineGraphic));
            RectTransform orbit = orbitObject.GetComponent<RectTransform>();
            orbit.SetParent(root, false);
            SolarARUIFactory.Place(
                orbit,
                0f,
                -350f,
                220f,
                56f,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f));
            EllipseOutlineGraphic ellipse = orbitObject.GetComponent<EllipseOutlineGraphic>();
            ellipse.color = SolarARColors.Cyan;
            ellipse.raycastTarget = false;

            GameObject panel = InstantiatePrefabForAuthoring(
                EducationalPanelPath,
                root,
                "EducationalPanel");
            SolarARUIFactory.Bottom(panel.GetComponent<RectTransform>(), 92f, 360f, 16f, 16f);

            planetName = FindChildComponent<TMP_Text>(panel.transform, "Planet Name");
            firstFact = FindChildComponent<TMP_Text>(panel.transform, "First Fact Text");
            secondFact = FindChildComponent<TMP_Text>(panel.transform, "Second Fact Text");
            Button close = FindChildComponent<Button>(panel.transform, "Close Button");
            Wire(close, controller.CloseInformation);
            return root.gameObject;
        }

        private static GameObject BuildLost(RectTransform parent, EarthMvpUI controller)
        {
            RectTransform root = CreateScreen("02d_Card_Perdido", parent);
            BuildHeader(root, string.Empty, controller.BackToOnboarding, controller.ShowInstructions);
            RectTransform frame = CreateScanFrame(root, 150f, 484f);

            CreateSimpleIcon(frame, "Lost Card Icon", "⊘", 88f, 80f);
            AddText(
                frame,
                "Prompt",
                "Aponte a câmera\npara um card",
                30f,
                190f,
                104f,
                32f,
                32f,
                true);
            AddText(
                frame,
                "Support",
                "Mostre este card novamente ou aponte para outro.",
                17f,
                304f,
                56f,
                40f,
                40f,
                false,
                SolarARColors.OnDark);

            AddText(
                root,
                "Reassurance",
                "Não se preocupe!\nSeu planeta volta assim que encontrar o card.",
                16f,
                672f,
                72f,
                32f,
                32f,
                false,
                SolarARColors.Lavender);
            return root.gameObject;
        }

        private static GameObject BuildPlanets(
            RectTransform parent,
            EarthMvpUI controller,
            out RawImage[] planetImages)
        {
            RectTransform root = CreateScreen("03_Planetas", parent);
            BuildHeader(root, "Planetas", controller.BackToOnboarding, null);
            AddText(
                root,
                "Support",
                "Escolha um planeta 3D para explorar",
                17f,
                80f,
                32f,
                24f,
                24f,
                false,
                SolarARColors.Lavender);

            planetImages = new RawImage[PlanetNames.Length];
            for (int index = 0; index < PlanetNames.Length; index++)
            {
                int row = index / 2;
                bool lastCentered = index == PlanetNames.Length - 1;
                float left = lastCentered ? 119f : (index % 2 == 0 ? 24f : 214f);
                float top = 120f + row * 158f;
                Button card = BuildPlanetCard(
                    root,
                    PlanetNames[index],
                    left,
                    top,
                    out planetImages[index]);
                WireInt(card, controller.ShowPlanetDetail, index);
            }

            return root.gameObject;
        }

        private static Button BuildPlanetCard(
            Transform parent,
            string planetName,
            float left,
            float top,
            out RawImage planetImage)
        {
            RectTransform card = _ui.Panel(
                planetName + " Card",
                parent,
                Color.Lerp(SolarARColors.SpaceCard, SolarARColors.Purple, 0.08f),
                20f);
            SolarARUIFactory.Place(
                card,
                left,
                -top,
                174f,
                142f,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f));

            Graphic background = card.GetComponent<Graphic>();
            background.raycastTarget = true;
            Button button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = background;

            RectTransform glow = _ui.Circle(
                "Planet Glow",
                card,
                new Color(SolarARColors.Cyan.r, SolarARColors.Cyan.g, SolarARColors.Cyan.b, 0.14f),
                94f);
            SolarARUIFactory.Place(
                glow,
                40f,
                -4f,
                94f,
                94f,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f));

            RectTransform imageRect = _ui.Rect("Planet 3D Thumbnail", card);
            SolarARUIFactory.Place(
                imageRect,
                35f,
                -1f,
                104f,
                104f,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f));
            planetImage = imageRect.gameObject.AddComponent<RawImage>();
            planetImage.color = Color.white;
            planetImage.raycastTarget = false;

            TextMeshProUGUI label = _ui.Text(
                "Planet Name",
                card,
                planetName,
                18f,
                SolarARColors.OnDark,
                TextAlignmentOptions.Center,
                true,
                FontStyles.Bold);
            SolarARUIFactory.Bottom(label.rectTransform, 6f, 28f, 8f, 8f);
            return button;
        }

        private static GameObject BuildPlanetDetail(
            RectTransform parent,
            EarthMvpUI controller,
            out RawImage previewImage,
            out TMP_Text detailPlanetName,
            out PlanetFactCarouselController factCarouselController,
            out PlanetPreviewDragSurface previewDragSurface)
        {
            RectTransform root = CreateScreen("04_PlanetDetail", parent);
            BuildHeader(root, "Terra", controller.BackToPlanets, null);
            detailPlanetName = FindChildComponent<TMP_Text>(root, "Title");
            AddText(
                root,
                "Preview Label",
                "Arraste o planeta para girar",
                16f,
                80f,
                28f,
                24f,
                24f,
                false,
                SolarARColors.Lavender);

            RectTransform previewFrame = _ui.Panel(
                "Planet Preview Frame",
                root,
                Color.Lerp(SolarARColors.SpaceCard, SolarARColors.Purple, 0.12f),
                28f);
            SolarARUIFactory.Top(previewFrame, 112f, 292f, 40f, 40f);
            RectTransform output = _ui.Rect("Planet Preview Output", previewFrame);
            SolarARUIFactory.Stretch(output, 12f, 12f, 12f, 12f);
            previewImage = output.gameObject.AddComponent<RawImage>();
            previewImage.color = Color.clear;
            previewImage.raycastTarget = true;
            previewDragSurface = output.gameObject.AddComponent<PlanetPreviewDragSurface>();

            RectTransform panel = _ui.Panel(
                "Fact Carousel",
                root,
                SolarARColors.SpaceCard,
                28f);
            SolarARUIFactory.Top(panel, 420f, 278f, 16f, 16f);
            panel.GetComponent<Graphic>().raycastTarget = true;
            CanvasGroup canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
            factCarouselController = panel.gameObject.AddComponent<PlanetFactCarouselController>();

            RectTransform accent = _ui.Panel(
                "Planet Accent",
                panel,
                SolarARColors.Cyan,
                3f);
            SolarARUIFactory.Top(accent, 0f, 6f, 0f, 0f);

            TMP_Text category = AddText(
                panel,
                "Fact Category",
                "Vida e água",
                14f,
                18f,
                24f,
                20f,
                88f,
                false,
                SolarARColors.Cyan,
                TextAlignmentOptions.MidlineLeft,
                FontStyles.Bold);
            TMP_Text progress = AddText(
                panel,
                "Fact Progress",
                "1/8",
                15f,
                18f,
                24f,
                278f,
                20f,
                false,
                SolarARColors.Lavender,
                TextAlignmentOptions.MidlineRight,
                FontStyles.Bold);
            TMP_Text factTitle = AddText(
                panel,
                "Fact Title",
                "Planeta azul",
                23f,
                48f,
                36f,
                20f,
                20f,
                true,
                SolarARColors.OnDark,
                TextAlignmentOptions.MidlineLeft,
                FontStyles.Bold);
            TMP_Text factDescription = AddText(
                panel,
                "Fact Description",
                "A água cobre a maior parte da superfície da Terra.",
                17f,
                88f,
                96f,
                20f,
                20f,
                false,
                SolarARColors.OnDark,
                TextAlignmentOptions.TopLeft);
            AddText(
                panel,
                "Swipe Hint",
                "Deslize ou use os botões",
                13f,
                180f,
                22f,
                20f,
                20f,
                false,
                SolarARColors.Lavender);

            Button previousFact = AddButton(
                panel,
                "Previous Fact Button",
                "‹ Anterior",
                SolarARColors.Secondary,
                SolarARColors.Ink,
                210f,
                48f,
                20f,
                194f,
                20f,
                15f);
            Button nextFact = AddButton(
                panel,
                "Next Fact Button",
                "Próxima ›",
                SolarARColors.Primary,
                SolarARColors.Ink,
                210f,
                48f,
                194f,
                20f,
                20f,
                15f);
            Wire(previousFact, factCarouselController.ShowPrevious);
            Wire(nextFact, factCarouselController.ShowNext);

            SerializedObject carouselData = new(factCarouselController);
            SetObject(carouselData, "_cardCanvasGroup", canvasGroup);
            SetObject(carouselData, "_accent", accent.GetComponent<Graphic>());
            SetObject(carouselData, "_categoryText", category);
            SetObject(carouselData, "_titleText", factTitle);
            SetObject(carouselData, "_descriptionText", factDescription);
            SetObject(carouselData, "_progressText", progress);
            SetObject(carouselData, "_previousButton", previousFact);
            SetObject(carouselData, "_nextButton", nextFact);
            carouselData.ApplyModifiedPropertiesWithoutUndo();

            Button previous = AddButton(
                root,
                "Previous Button",
                "Planeta anterior",
                SolarARColors.Secondary,
                SolarARColors.Ink,
                710f,
                56f,
                24f,
                214f,
                20f,
                14f);
            Button next = AddButton(
                root,
                "Next Button",
                "Próximo planeta",
                SolarARColors.Primary,
                SolarARColors.Ink,
                710f,
                56f,
                214f,
                24f,
                20f,
                14f);
            Wire(previous, controller.ShowPreviousPlanet);
            Wire(next, controller.ShowNextPlanet);
            return root.gameObject;
        }

        private static GameObject BuildMissions(
            RectTransform parent,
            out MissionHubController hubController)
        {
            RectTransform root = CreateScreen("05_Missoes_Espaciais", parent);
            hubController = root.gameObject.AddComponent<MissionHubController>();

            RectTransform hub = CreateScreen("Mission Hub", root);
            AddText(
                hub,
                "Title",
                "Missões Espaciais",
                32f,
                24f,
                48f,
                24f,
                24f,
                true);
            AddText(
                hub,
                "Introduction",
                "Escolha uma atividade e embarque em uma nova descoberta!",
                17f,
                80f,
                48f,
                32f,
                32f,
                false,
                SolarARColors.Lavender);

            Button quizCard = BuildMissionCard(
                hub,
                "Quiz Mission Card",
                "?",
                "Quiz Galáctico",
                "Responda perguntas sobre os planetas.",
                -80f,
                152f);
            Button memoryCard = BuildMissionCard(
                hub,
                "Memory Mission Card",
                "◆ ◆",
                "Memória Cósmica",
                "Encontre os pares de planetas.",
                80f,
                152f);
            Button orderCard = BuildMissionCard(
                hub,
                "Order Mission Card",
                "1 → 7",
                "Ordem Planetária",
                "Organize os planetas perto do Sol.",
                -80f,
                382f);
            Button detectiveCard = BuildMissionCard(
                hub,
                "Detective Mission Card",
                "? +",
                "Detetive Espacial",
                "Use pistas para descobrir o planeta.",
                80f,
                382f);

            GameObject quizRoot = BuildQuiz(root, hubController, out QuizScreenController quizController);
            GameObject memoryRoot = BuildMemoryGame(
                root,
                hubController,
                out MemoryScreenController memoryController);
            GameObject orderRoot = BuildPlanetOrderGame(
                root,
                hubController,
                out PlanetOrderScreenController orderController);
            GameObject detectiveRoot = BuildSpaceDetectiveGame(
                root,
                hubController,
                out SpaceDetectiveScreenController detectiveController);
            Wire(quizCard, hubController.OpenQuiz);
            Wire(memoryCard, hubController.OpenMemory);
            Wire(orderCard, hubController.OpenPlanetOrder);
            Wire(detectiveCard, hubController.OpenSpaceDetective);

            SerializedObject hubData = new(hubController);
            SetObject(hubData, "_hubRoot", hub.gameObject);
            SetObject(hubData, "_quizRoot", quizRoot);
            SetObject(hubData, "_quizController", quizController);
            SetObject(hubData, "_memoryRoot", memoryRoot);
            SetObject(hubData, "_memoryController", memoryController);
            SetObject(hubData, "_planetOrderRoot", orderRoot);
            SetObject(hubData, "_planetOrderController", orderController);
            SetObject(hubData, "_spaceDetectiveRoot", detectiveRoot);
            SetObject(hubData, "_spaceDetectiveController", detectiveController);
            hubData.ApplyModifiedPropertiesWithoutUndo();

            hub.gameObject.SetActive(true);
            quizRoot.SetActive(false);
            memoryRoot.SetActive(false);
            orderRoot.SetActive(false);
            detectiveRoot.SetActive(false);
            return root.gameObject;
        }

        private static Button BuildMissionCard(
            Transform parent,
            string objectName,
            string icon,
            string title,
            string description,
            float x,
            float top)
        {
            RectTransform card = _ui.Panel(
                objectName,
                parent,
                Color.Lerp(SolarARColors.SpaceCard, SolarARColors.Purple, top < 300f ? 0.10f : 0.05f),
                24f);
            SolarARUIFactory.Place(
                card,
                x,
                -top,
                144f,
                202f,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f));
            Button button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = card.GetComponent<Graphic>();
            button.targetGraphic.raycastTarget = true;

            Color accentColor = x < 0f ? SolarARColors.Cyan : SolarARColors.Magenta;
            RectTransform iconGlow = _ui.Circle(
                "Icon Glow",
                card,
                new Color(accentColor.r, accentColor.g, accentColor.b, 0.16f),
                54f);
            SolarARUIFactory.Place(
                iconGlow,
                45f,
                -8f,
                54f,
                54f,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f));

            TMP_Text iconText = AddText(
                card,
                "Icon",
                icon,
                25f,
                16f,
                44f,
                12f,
                12f,
                true,
                accentColor);
            iconText.overflowMode = TextOverflowModes.Overflow;
            AddText(
                card,
                "Game Name",
                title,
                18f,
                70f,
                48f,
                12f,
                12f,
                true,
                SolarARColors.OnDark);
            AddText(
                card,
                "Description",
                description,
                13f,
                124f,
                60f,
                12f,
                12f,
                false,
                SolarARColors.Lavender);
            return button;
        }

        private static GameObject BuildQuiz(
            RectTransform parent,
            MissionHubController hubController,
            out QuizScreenController screenController)
        {
            RectTransform root = CreateScreen("Quiz Galactico", parent);
            QuizManager quizManager = root.gameObject.AddComponent<QuizManager>();
            screenController = root.gameObject.AddComponent<QuizScreenController>();

            SerializedObject managerData = new(quizManager);
            SetObject(managerData, "_questionBank", _quizQuestionBank);
            managerData.ApplyModifiedPropertiesWithoutUndo();

            RectTransform intro = CreateScreen("Quiz Introduction", root);
            AddMissionBackButton(intro, hubController);
            AddText(
                intro,
                "Title",
                "Missões Espaciais",
                32f,
                32f,
                48f,
                128f,
                24f,
                true);

            RectTransform introMoon = CreateCuriosityMoonArtwork(
                intro,
                "Quiz Intro Moon Mascot");
            SolarARUIFactory.Place(
                introMoon,
                0f,
                -104f,
                176f,
                160f,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f));

            AddText(
                intro,
                "Mission Name",
                "Quiz Galáctico",
                28f,
                280f,
                48f,
                32f,
                32f,
                true,
                SolarARColors.Primary);
            TMP_Text introMessage = AddText(
                intro,
                "Mission Introduction",
                "Responda cinco perguntas e descubra quanto você conhece sobre os planetas!",
                18f,
                344f,
                72f,
                40f,
                40f,
                false,
                SolarARColors.Lavender);

            RectTransform summaryPanel = _ui.Panel(
                "Mission Summary",
                intro,
                SolarARColors.Blue4,
                20f);
            SolarARUIFactory.Top(summaryPanel, 448f, 72f, 32f, 32f);
            TMP_Text progressSummary = _ui.Text(
                "Progress Summary",
                summaryPanel,
                "Melhor pontuação: 0/50  •  Missões concluídas: 0",
                15f,
                SolarARColors.OnDark,
                TextAlignmentOptions.Center,
                false,
                FontStyles.Bold);
            SolarARUIFactory.Stretch(progressSummary.rectTransform, 12f, 8f, 12f, 8f);

            Button startButton = AddButton(
                intro,
                "Start Mission Button",
                "Começar missão",
                SolarARColors.Primary,
                SolarARColors.Ink,
                560f,
                64f,
                40f,
                40f,
                24f,
                20f,
                true);
            Wire(startButton, screenController.StartMission);

            RectTransform question = CreateScreen("Quiz Question", root);
            AddMissionBackButton(question, hubController);
            TMP_Text progressText = AddText(
                question,
                "Question Progress",
                "Pergunta 1/5",
                18f,
                24f,
                32f,
                128f,
                24f,
                false,
                SolarARColors.Lavender,
                TextAlignmentOptions.Center,
                FontStyles.Bold);

            RectTransform progressBar = _ui.Panel(
                "Question Progress Bar",
                question,
                SolarARColors.Blue4,
                8f);
            SolarARUIFactory.Top(progressBar, 72f, 16f, 32f, 32f);
            Image progressFill = _ui.Line("Progress Fill", progressBar, SolarARColors.Cyan);
            SolarARUIFactory.Stretch(progressFill.rectTransform, 2f, 2f, 2f, 2f);
            progressFill.type = Image.Type.Filled;
            progressFill.fillMethod = Image.FillMethod.Horizontal;
            progressFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            progressFill.fillAmount = 0.2f;

            RectTransform questionCard = _ui.Panel(
                "Question Card",
                question,
                SolarARColors.Panel,
                24f);
            SolarARUIFactory.Top(questionCard, 112f, 184f, 24f, 24f);
            AddText(
                questionCard,
                "Question Label",
                "PERGUNTA",
                14f,
                16f,
                24f,
                20f,
                20f,
                false,
                SolarARColors.Blue1,
                TextAlignmentOptions.Center,
                FontStyles.Bold);
            TMP_Text questionText = AddText(
                questionCard,
                "Question Text",
                "Qual planeta fica mais perto do Sol?",
                22f,
                48f,
                112f,
                20f,
                20f,
                true,
                SolarARColors.Ink);

            QuizAnswerButton[] answerButtons = new QuizAnswerButton[3];
            for (int index = 0; index < answerButtons.Length; index++)
            {
                Button answer = AddButton(
                    question,
                    $"Answer {index + 1} Button",
                    $"Alternativa {index + 1}",
                    SolarARColors.Secondary,
                    SolarARColors.Ink,
                    320f + index * 80f,
                    64f,
                    32f,
                    32f,
                    20f,
                    16f,
                    true);
                QuizAnswerButton answerController = answer.gameObject.AddComponent<QuizAnswerButton>();
                SerializedObject answerData = new(answerController);
                SetObject(answerData, "_button", answer);
                SetObject(answerData, "_label", answer.GetComponentInChildren<TMP_Text>());
                SetObject(answerData, "_background", answer.targetGraphic);
                SetObject(answerData, "_controller", screenController);
                answerData.FindProperty("_answerIndex").intValue = index;
                answerData.ApplyModifiedPropertiesWithoutUndo();
                Wire(answer, answerController.HandlePressed);
                answerButtons[index] = answerController;
            }

            RectTransform feedback = _ui.Panel(
                "Answer Feedback",
                question,
                SolarARColors.Panel,
                20f);
            SolarARUIFactory.Top(feedback, 568f, 128f, 24f, 24f);
            TMP_Text feedbackTitle = AddText(
                feedback,
                "Feedback Title",
                "Muito bem! Resposta correta.",
                17f,
                12f,
                38f,
                16f,
                16f,
                false,
                SolarARColors.Ink,
                TextAlignmentOptions.MidlineLeft,
                FontStyles.Bold);
            TMP_Text feedbackExplanation = AddText(
                feedback,
                "Feedback Explanation",
                "Aqui aparece uma explicação curta para aprender ainda mais.",
                15f,
                48f,
                68f,
                16f,
                16f,
                false,
                SolarARColors.Ink,
                TextAlignmentOptions.TopLeft);

            Button nextButton = AddButton(
                question,
                "Next Question Button",
                "Próxima pergunta",
                SolarARColors.Primary,
                SolarARColors.Ink,
                712f,
                56f,
                48f,
                48f,
                22f,
                18f,
                true);
            nextButton.interactable = false;
            Wire(nextButton, screenController.NextQuestion);

            RectTransform result = CreateScreen("Quiz Result", root);
            AddMissionBackButton(result, hubController);
            AddText(
                result,
                "Title",
                "Missão concluída!",
                32f,
                40f,
                48f,
                128f,
                24f,
                true);
            RectTransform resultMoon = CreateCuriosityMoonArtwork(
                result,
                "Quiz Result Moon Mascot");
            SolarARUIFactory.Place(
                resultMoon,
                0f,
                -112f,
                168f,
                152f,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f));

            TMP_Text scoreText = AddText(
                result,
                "Final Score",
                "50/50 pontos",
                30f,
                288f,
                48f,
                32f,
                32f,
                true,
                SolarARColors.Primary);
            RectTransform starsRoot = _ui.Rect("Stars", result);
            SolarARUIFactory.Top(starsRoot, 352f, 48f, 32f, 32f);
            QuizStarGraphic[] stars = new QuizStarGraphic[3];
            for (int index = 0; index < stars.Length; index++)
            {
                GameObject starObject = new(
                    $"Star {index + 1}",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(QuizStarGraphic));
                RectTransform starRect = starObject.GetComponent<RectTransform>();
                starRect.SetParent(starsRoot, false);
                SolarARUIFactory.Place(
                    starRect,
                    (index - 1) * 56f,
                    0f,
                    42f,
                    42f,
                    Vector2.one * 0.5f,
                    Vector2.one * 0.5f);
                stars[index] = starObject.GetComponent<QuizStarGraphic>();
                stars[index].color = SolarARColors.Primary;
                stars[index].raycastTarget = false;
            }
            TMP_Text correctAnswersText = AddText(
                result,
                "Correct Answers",
                "Você acertou 5 de 5 perguntas.",
                18f,
                416f,
                32f,
                32f,
                32f,
                false,
                SolarARColors.OnDark,
                TextAlignmentOptions.Center,
                FontStyles.Bold);

            RectTransform resultMessagePanel = _ui.Panel(
                "Mascot Result Message",
                result,
                SolarARColors.Blue4,
                20f);
            SolarARUIFactory.Top(resultMessagePanel, 472f, 104f, 32f, 32f);
            TMP_Text resultMessage = _ui.Text(
                "Result Message",
                resultMessagePanel,
                "Incrível! Você completou a missão como um grande explorador!",
                17f,
                SolarARColors.OnDark,
                TextAlignmentOptions.Center,
                false,
                FontStyles.Bold);
            SolarARUIFactory.Stretch(resultMessage.rectTransform, 16f, 12f, 16f, 12f);

            Button replayButton = AddButton(
                result,
                "Replay Button",
                "Jogar novamente",
                SolarARColors.Primary,
                SolarARColors.Ink,
                624f,
                64f,
                40f,
                40f,
                24f,
                20f,
                true);
            Wire(replayButton, screenController.Replay);

            SerializedObject screenData = new(screenController);
            SetObject(screenData, "_quizManager", quizManager);
            SetObject(screenData, "_introRoot", intro.gameObject);
            SetObject(screenData, "_questionRoot", question.gameObject);
            SetObject(screenData, "_resultRoot", result.gameObject);
            SetObject(screenData, "_introMessageText", introMessage);
            SetObject(screenData, "_progressSummaryText", progressSummary);
            SetObject(screenData, "_progressText", progressText);
            SetObject(screenData, "_progressFill", progressFill);
            SetObject(screenData, "_questionText", questionText);
            SerializedProperty answerButtonData = screenData.FindProperty("_answerButtons");
            answerButtonData.arraySize = answerButtons.Length;
            for (int index = 0; index < answerButtons.Length; index++)
            {
                answerButtonData.GetArrayElementAtIndex(index).objectReferenceValue = answerButtons[index];
            }

            SetObject(screenData, "_feedbackPanel", feedback.gameObject);
            SetObject(screenData, "_feedbackTitleText", feedbackTitle);
            SetObject(screenData, "_feedbackExplanationText", feedbackExplanation);
            SetObject(screenData, "_nextButton", nextButton);
            SetObject(screenData, "_scoreText", scoreText);
            SetObject(screenData, "_correctAnswersText", correctAnswersText);
            SerializedProperty starsData = screenData.FindProperty("_stars");
            starsData.arraySize = stars.Length;
            for (int index = 0; index < stars.Length; index++)
            {
                starsData.GetArrayElementAtIndex(index).objectReferenceValue = stars[index];
            }
            SetObject(screenData, "_resultMessageText", resultMessage);
            screenData.ApplyModifiedPropertiesWithoutUndo();

            intro.gameObject.SetActive(true);
            question.gameObject.SetActive(false);
            result.gameObject.SetActive(false);
            feedback.gameObject.SetActive(false);
            return root.gameObject;
        }

        private static void AddMissionBackButton(Transform parent, MissionHubController hubController)
        {
            Button backButton = _ui.Button(
                "Back to Mission Hub",
                parent,
                "‹ Central",
                SolarARColors.Blue4,
                SolarARColors.OnDark,
                null,
                20f,
                15f,
                false);
            SolarARUIFactory.Place(
                backButton.GetComponent<RectTransform>(),
                16f,
                -16f,
                104f,
                48f,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f));
            Wire(backButton, hubController.BackToHub);
        }

        private static GameObject BuildMemoryGame(
            RectTransform parent,
            MissionHubController hubController,
            out MemoryScreenController controller)
        {
            RectTransform root = CreateScreen("Memory Game", parent);
            controller = root.gameObject.AddComponent<MemoryScreenController>();

            RectTransform board = CreateScreen("Memory Board", root);
            AddMissionBackButton(board, hubController);
            AddText(board, "Title", "Memória Cósmica", 28f, 20f, 44f, 116f, 24f, true);
            TMP_Text pairsText = AddText(
                board, "Pairs", "Pares: 0/6", 16f, 76f, 28f, 24f, 190f, false,
                SolarARColors.Lavender, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            TMP_Text attemptsText = AddText(
                board, "Attempts", "Tentativas: 0", 16f, 76f, 28f, 190f, 24f, false,
                SolarARColors.Lavender, TextAlignmentOptions.MidlineRight, FontStyles.Bold);
            AddText(
                board,
                "Instructions",
                "Encontre os seis pares. Duas cartas formam uma tentativa.",
                14f,
                110f,
                38f,
                24f,
                24f,
                false,
                SolarARColors.OnDark);

            Button[] cardButtons = new Button[12];
            RawImage[] cardImages = new RawImage[12];
            TMP_Text[] cardLabels = new TMP_Text[12];
            for (int index = 0; index < cardButtons.Length; index++)
            {
                int row = index / 3;
                int column = index % 3;
                RectTransform card = _ui.Panel(
                    $"Memory Card {index + 1}", board, SolarARColors.Blue4, 16f);
                SolarARUIFactory.Place(
                    card,
                    (column - 1) * 104f,
                    -(158f + row * 126f),
                    92f,
                    112f,
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f));
                Button button = card.gameObject.AddComponent<Button>();
                button.targetGraphic = card.GetComponent<Graphic>();
                button.targetGraphic.raycastTarget = true;

                GameObject imageObject = new(
                    "Planet Image",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(RawImage));
                RectTransform imageRect = imageObject.GetComponent<RectTransform>();
                imageRect.SetParent(card, false);
                SolarARUIFactory.Top(imageRect, 8f, 72f, 10f, 10f);
                RawImage image = imageObject.GetComponent<RawImage>();
                image.raycastTarget = false;
                image.uvRect = new Rect(0f, 0f, 1f, 1f);

                TMP_Text label = _ui.Text(
                    "Card Label",
                    card,
                    "?",
                    18f,
                    SolarARColors.OnDark,
                    TextAlignmentOptions.Center,
                    true,
                    FontStyles.Bold);
                SolarARUIFactory.Bottom(label.rectTransform, 6f, 24f, 4f, 4f);

                WireInt(button, controller.SelectCard, index);
                cardButtons[index] = button;
                cardImages[index] = image;
                cardLabels[index] = label;
            }

            RectTransform result = CreateScreen("Memory Result", root);
            AddMissionBackButton(result, hubController);
            AddText(result, "Title", "Memória completa!", 32f, 72f, 48f, 24f, 24f, true);
            TMP_Text resultAttempts = AddText(
                result,
                "Result Attempts",
                "Você encontrou os 6 pares!",
                20f,
                184f,
                64f,
                32f,
                32f,
                false,
                SolarARColors.OnDark);
            TMP_Text[] resultStars = BuildTextStars(result, 280f);
            AddText(
                result,
                "Result Message",
                "Excelente memória, explorador! Cada planeta encontrou seu par.",
                18f,
                360f,
                88f,
                40f,
                40f,
                false,
                SolarARColors.Lavender);
            Button replay = AddButton(
                result, "Replay Memory Button", "Jogar novamente", SolarARColors.Primary,
                SolarARColors.Ink, 504f, 64f, 40f, 40f, 24f, 20f, true);
            Wire(replay, controller.StartNewGame);

            SerializedObject data = new(controller);
            SetObject(data, "_boardRoot", board.gameObject);
            SetObject(data, "_resultRoot", result.gameObject);
            SetObjectArray(data, "_cardButtons", cardButtons);
            SetObjectArray(data, "_cardImages", cardImages);
            SetObjectArray(data, "_cardLabels", cardLabels);
            SetObject(data, "_pairsText", pairsText);
            SetObject(data, "_attemptsText", attemptsText);
            SetObject(data, "_resultAttemptsText", resultAttempts);
            SetObjectArray(data, "_resultStars", resultStars);
            data.ApplyModifiedPropertiesWithoutUndo();

            board.gameObject.SetActive(true);
            result.gameObject.SetActive(false);
            return root.gameObject;
        }

        private static GameObject BuildPlanetOrderGame(
            RectTransform parent,
            MissionHubController hubController,
            out PlanetOrderScreenController controller)
        {
            RectTransform root = CreateScreen("Planet Order Game", parent);
            controller = root.gameObject.AddComponent<PlanetOrderScreenController>();

            RectTransform game = CreateScreen("Planet Order Board", root);
            AddMissionBackButton(game, hubController);
            AddText(game, "Title", "Ordem Planetária", 28f, 20f, 44f, 116f, 24f, true);
            AddText(
                game,
                "Scope Note",
                "Organize pela distância ao Sol. Esta atividade usa apenas os 7 planetas disponíveis no SolarAR; não é a lista completa do Sistema Solar.",
                13f,
                72f,
                64f,
                24f,
                24f,
                false,
                SolarARColors.Lavender);

            Button[] positionButtons = new Button[7];
            Graphic[] positionBackgrounds = new Graphic[7];
            RawImage[] planetImages = new RawImage[7];
            TMP_Text[] planetLabels = new TMP_Text[7];
            for (int index = 0; index < positionButtons.Length; index++)
            {
                RectTransform position = _ui.Panel(
                    $"Order Position {index + 1}", game, SolarARColors.Panel, 16f);
                SolarARUIFactory.Top(position, 146f + index * 62f, 54f, 32f, 32f);
                Button button = position.gameObject.AddComponent<Button>();
                button.targetGraphic = position.GetComponent<Graphic>();
                button.targetGraphic.raycastTarget = true;

                GameObject imageObject = new(
                    "Planet Image",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(RawImage));
                RectTransform imageRect = imageObject.GetComponent<RectTransform>();
                imageRect.SetParent(position, false);
                SolarARUIFactory.Place(
                    imageRect,
                    8f,
                    0f,
                    44f,
                    44f,
                    new Vector2(0f, 0.5f),
                    new Vector2(0f, 0.5f));
                RawImage image = imageObject.GetComponent<RawImage>();
                image.raycastTarget = false;

                TMP_Text label = _ui.Text(
                    "Planet Label",
                    position,
                    $"{index + 1}. Planeta",
                    17f,
                    SolarARColors.Ink,
                    TextAlignmentOptions.MidlineLeft,
                    false,
                    FontStyles.Bold);
                SolarARUIFactory.Stretch(label.rectTransform, 64f, 6f, 12f, 6f);
                WireInt(button, controller.SelectPosition, index);
                positionButtons[index] = button;
                positionBackgrounds[index] = position.GetComponent<Graphic>();
                planetImages[index] = image;
                planetLabels[index] = label;
            }

            TMP_Text checksText = AddText(
                game, "Checks", "Conferências: 0", 15f, 584f, 24f, 32f, 32f, false,
                SolarARColors.Lavender, TextAlignmentOptions.Center, FontStyles.Bold);
            Button checkButton = AddButton(
                game, "Check Order Button", "Conferir ordem", SolarARColors.Primary,
                SolarARColors.Ink, 616f, 56f, 48f, 48f, 22f, 18f, true);
            Wire(checkButton, controller.CheckOrder);
            TMP_Text feedback = AddText(
                game,
                "Order Feedback",
                "Selecione um planeta e depois a posição de destino.",
                14f,
                680f,
                40f,
                32f,
                32f,
                false,
                SolarARColors.OnDark);

            RectTransform result = CreateScreen("Planet Order Result", root);
            AddMissionBackButton(result, hubController);
            AddText(result, "Title", "Ordem correta!", 32f, 72f, 48f, 24f, 24f, true);
            TMP_Text resultChecks = AddText(
                result, "Result Checks", "Ordem correta!", 20f, 184f, 48f, 32f, 32f,
                false, SolarARColors.OnDark);
            TMP_Text[] resultStars = BuildTextStars(result, 264f);
            AddText(
                result,
                "Educational Curiosity",
                "Curiosidade: quanto mais distante do Sol, maior costuma ser o caminho da órbita. Netuno leva cerca de 165 anos terrestres para completar uma volta!",
                18f,
                344f,
                120f,
                40f,
                40f,
                false,
                SolarARColors.Lavender);
            Button replay = AddButton(
                result, "Replay Order Button", "Jogar novamente", SolarARColors.Primary,
                SolarARColors.Ink, 520f, 64f, 40f, 40f, 24f, 20f, true);
            Wire(replay, controller.StartNewGame);

            SerializedObject data = new(controller);
            SetObject(data, "_gameRoot", game.gameObject);
            SetObject(data, "_resultRoot", result.gameObject);
            SetObjectArray(data, "_positionButtons", positionButtons);
            SetObjectArray(data, "_positionBackgrounds", positionBackgrounds);
            SetObjectArray(data, "_planetImages", planetImages);
            SetObjectArray(data, "_planetLabels", planetLabels);
            SetObject(data, "_checksText", checksText);
            SetObject(data, "_feedbackText", feedback);
            SetObject(data, "_resultChecksText", resultChecks);
            SetObjectArray(data, "_resultStars", resultStars);
            data.ApplyModifiedPropertiesWithoutUndo();

            game.gameObject.SetActive(true);
            result.gameObject.SetActive(false);
            return root.gameObject;
        }

        private static GameObject BuildSpaceDetectiveGame(
            RectTransform parent,
            MissionHubController hubController,
            out SpaceDetectiveScreenController controller)
        {
            RectTransform root = CreateScreen("Space Detective Game", parent);
            controller = root.gameObject.AddComponent<SpaceDetectiveScreenController>();

            RectTransform game = CreateScreen("Space Detective Challenge", root);
            AddMissionBackButton(game, hubController);
            AddText(game, "Title", "Detetive Espacial", 28f, 20f, 44f, 116f, 24f, true);
            TMP_Text progress = AddText(
                game, "Challenge Progress", "Desafio 1/5", 15f, 70f, 28f, 24f, 190f,
                false, SolarARColors.Lavender, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            TMP_Text score = AddText(
                game, "Score", "Pontos: 0/50", 15f, 70f, 28f, 190f, 24f,
                false, SolarARColors.Lavender, TextAlignmentOptions.MidlineRight, FontStyles.Bold);

            RectTransform cluePanel = _ui.Panel(
                "Clues Panel", game, SolarARColors.Panel, 20f);
            SolarARUIFactory.Top(cluePanel, 106f, 178f, 24f, 24f);
            AddText(
                cluePanel, "Clue Header", "PLANETA MISTERIOSO", 14f, 12f, 24f, 16f, 16f,
                false, SolarARColors.Blue1, TextAlignmentOptions.Center, FontStyles.Bold);
            TMP_Text clues = AddText(
                cluePanel,
                "Visible Clues",
                "1. Aqui aparece a primeira pista.",
                15f,
                42f,
                124f,
                16f,
                16f,
                false,
                SolarARColors.Ink,
                TextAlignmentOptions.TopLeft);

            TMP_Text clueCount = AddText(
                game, "Clue Count", "Pistas: 1/3", 14f, 294f, 40f, 32f, 204f,
                false, SolarARColors.Lavender, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            Button clueButton = AddButton(
                game, "More Clue Button", "Pedir outra pista", SolarARColors.Blue4,
                SolarARColors.OnDark, 292f, 48f, 184f, 32f, 18f, 15f, false);
            Wire(clueButton, controller.RevealNextClue);

            Button[] answerButtons = new Button[3];
            TMP_Text[] answerLabels = new TMP_Text[3];
            Graphic[] answerBackgrounds = new Graphic[3];
            for (int index = 0; index < answerButtons.Length; index++)
            {
                Button answer = AddButton(
                    game,
                    $"Detective Answer {index + 1}",
                    $"Planeta {index + 1}",
                    SolarARColors.Secondary,
                    SolarARColors.Ink,
                    356f + index * 60f,
                    52f,
                    32f,
                    32f,
                    18f,
                    17f,
                    true);
                WireInt(answer, controller.SelectAnswer, index);
                answerButtons[index] = answer;
                answerLabels[index] = answer.GetComponentInChildren<TMP_Text>();
                answerBackgrounds[index] = answer.targetGraphic;
            }

            RectTransform feedbackPanel = _ui.Panel(
                "Detective Feedback", game, SolarARColors.Panel, 20f);
            SolarARUIFactory.Top(feedbackPanel, 544f, 108f, 24f, 24f);
            TMP_Text feedbackTitle = AddText(
                feedbackPanel, "Feedback Title", "Correto!", 16f, 10f, 30f, 88f, 16f,
                false, SolarARColors.Ink, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            TMP_Text feedbackExplanation = AddText(
                feedbackPanel, "Feedback Explanation", "Explicação educativa.", 14f, 38f,
                60f, 88f, 16f, false, SolarARColors.Ink, TextAlignmentOptions.TopLeft);
            GameObject revealedPlanetObject = new(
                "Revealed Planet Image",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(RawImage));
            RectTransform revealedPlanetRect = revealedPlanetObject.GetComponent<RectTransform>();
            revealedPlanetRect.SetParent(feedbackPanel, false);
            SolarARUIFactory.Place(
                revealedPlanetRect,
                12f,
                0f,
                64f,
                64f,
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f));
            RawImage revealedPlanetImage = revealedPlanetObject.GetComponent<RawImage>();
            revealedPlanetImage.raycastTarget = false;
            Button nextButton = AddButton(
                game, "Next Detective Challenge", "Próximo desafio", SolarARColors.Primary,
                SolarARColors.Ink, 664f, 52f, 48f, 48f, 20f, 17f, true);
            TMP_Text nextLabel = nextButton.GetComponentInChildren<TMP_Text>();
            nextButton.interactable = false;
            Wire(nextButton, controller.NextChallenge);

            RectTransform result = CreateScreen("Space Detective Result", root);
            AddMissionBackButton(result, hubController);
            AddText(result, "Title", "Mistério resolvido!", 32f, 72f, 48f, 24f, 24f, true);
            TMP_Text resultScore = AddText(
                result, "Result Score", "50/50 pontos", 30f, 190f, 48f, 32f, 32f,
                true, SolarARColors.Primary);
            TMP_Text[] resultStars = BuildTextStars(result, 270f);
            AddText(
                result,
                "Result Message",
                "Você investigou cinco planetas usando pistas científicas. Excelente trabalho, detetive!",
                18f,
                360f,
                88f,
                40f,
                40f,
                false,
                SolarARColors.Lavender);
            Button replay = AddButton(
                result, "Replay Detective Button", "Jogar novamente", SolarARColors.Primary,
                SolarARColors.Ink, 504f, 64f, 40f, 40f, 24f, 20f, true);
            Wire(replay, controller.StartNewGame);

            SerializedObject data = new(controller);
            SetObject(data, "_gameRoot", game.gameObject);
            SetObject(data, "_resultRoot", result.gameObject);
            SetObject(data, "_progressText", progress);
            SetObject(data, "_scoreText", score);
            SetObject(data, "_cluesText", clues);
            SetObject(data, "_clueCountText", clueCount);
            SetObject(data, "_moreClueButton", clueButton);
            SetObjectArray(data, "_answerButtons", answerButtons);
            SetObjectArray(data, "_answerLabels", answerLabels);
            SetObjectArray(data, "_answerBackgrounds", answerBackgrounds);
            SetObject(data, "_feedbackPanel", feedbackPanel.gameObject);
            SetObject(data, "_feedbackTitleText", feedbackTitle);
            SetObject(data, "_feedbackExplanationText", feedbackExplanation);
            SetObject(data, "_revealedPlanetImage", revealedPlanetImage);
            SetObject(data, "_nextButton", nextButton);
            SetObject(data, "_nextButtonLabel", nextLabel);
            SetObject(data, "_resultScoreText", resultScore);
            SetObjectArray(data, "_resultStars", resultStars);
            data.ApplyModifiedPropertiesWithoutUndo();

            game.gameObject.SetActive(true);
            result.gameObject.SetActive(false);
            feedbackPanel.gameObject.SetActive(false);
            return root.gameObject;
        }

        private static TMP_Text[] BuildTextStars(Transform parent, float top)
        {
            TMP_Text[] stars = new TMP_Text[3];
            for (int index = 0; index < stars.Length; index++)
            {
                TMP_Text star = _ui.Text(
                    $"Star {index + 1}",
                    parent,
                    "★",
                    46f,
                    SolarARColors.Primary,
                    TextAlignmentOptions.Center,
                    true,
                    FontStyles.Bold);
                SolarARUIFactory.Place(
                    star.rectTransform,
                    (index - 1) * 58f,
                    -top,
                    52f,
                    52f,
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f));
                stars[index] = star;
            }

            return stars;
        }

        private static RectTransform CreateScreen(string name, Transform parent)
        {
            RectTransform root = _ui.Rect(name, parent);
            SolarARUIFactory.Stretch(root);
            return root;
        }

        private static void BuildHeader(
            Transform parent,
            string title,
            UnityEngine.Events.UnityAction backAction,
            UnityEngine.Events.UnityAction helpAction)
        {
            RectTransform header = _ui.Rect("Header", parent);
            SolarARUIFactory.Top(header, 16f, 56f, 16f, 16f);

            Button back = _ui.IconButton(
                "Back Button",
                header,
                "‹",
                null,
                SolarARColors.Blue1,
                SolarARColors.OnDark,
                48f);
            SolarARUIFactory.Place(
                back.GetComponent<RectTransform>(),
                0f,
                0f,
                48f,
                48f,
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f));
            Wire(back, backAction);

            if (!string.IsNullOrEmpty(title))
            {
                TextMeshProUGUI titleText = _ui.Text(
                    "Title",
                    header,
                    title,
                    28f,
                    SolarARColors.OnDark,
                    TextAlignmentOptions.Center,
                    true,
                    FontStyles.Bold);
                SolarARUIFactory.Stretch(titleText.rectTransform, 64f, 0f, 64f, 0f);
            }

            if (helpAction != null)
            {
                Button help = _ui.IconButton(
                    "Help Button",
                    header,
                    "?",
                    null,
                    SolarARColors.Blue1,
                    SolarARColors.OnDark,
                    48f);
                SolarARUIFactory.Place(
                    help.GetComponent<RectTransform>(),
                    0f,
                    0f,
                    48f,
                    48f,
                    new Vector2(1f, 0.5f),
                    new Vector2(1f, 0.5f));
                Wire(help, helpAction);
            }
        }

        private static RectTransform CreateScanFrame(Transform parent, float top, float height)
        {
            RectTransform frame = _ui.Rect("Scan Frame", parent);
            SolarARUIFactory.Top(frame, top, height, 32f, 32f);

            AddFrameCorner(frame, "Top Left", new Vector2(0f, 1f), new Vector2(0f, 1f));
            AddFrameCorner(frame, "Top Right", new Vector2(1f, 1f), new Vector2(1f, 1f));
            AddFrameCorner(frame, "Bottom Left", new Vector2(0f, 0f), new Vector2(0f, 0f));
            AddFrameCorner(frame, "Bottom Right", new Vector2(1f, 0f), new Vector2(1f, 0f));
            return frame;
        }

        private static void AddFrameCorner(
            RectTransform parent,
            string name,
            Vector2 anchor,
            Vector2 pivot)
        {
            Image horizontal = _ui.Line(name + " Horizontal", parent, SolarARColors.Cyan);
            SolarARUIFactory.Place(horizontal.rectTransform, 0f, 0f, 44f, 3f, anchor, pivot);

            Image vertical = _ui.Line(name + " Vertical", parent, SolarARColors.Cyan);
            SolarARUIFactory.Place(vertical.rectTransform, 0f, 0f, 3f, 44f, anchor, pivot);
        }

        private static void CreateSimpleIcon(
            Transform parent,
            string name,
            string symbol,
            float top,
            float diameter)
        {
            RectTransform icon = _ui.Circle(name, parent, SolarARColors.Blue4, diameter);
            SolarARUIFactory.Place(
                icon,
                0f,
                -top,
                diameter,
                diameter,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f));

            RectTransform cameraBody = _ui.Panel(
                "Camera Body",
                icon,
                SolarARColors.Cyan,
                6f);
            SolarARUIFactory.Place(
                cameraBody,
                0f,
                0f,
                diameter * 0.48f,
                diameter * 0.34f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));

            RectTransform lens = _ui.Circle(
                "Camera Lens",
                cameraBody,
                SolarARColors.Blue4,
                diameter * 0.14f);
            SolarARUIFactory.Place(
                lens,
                0f,
                0f,
                diameter * 0.14f,
                diameter * 0.14f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));

            RectTransform cameraTop = _ui.Panel(
                "Camera Top",
                icon,
                SolarARColors.Cyan,
                3f);
            SolarARUIFactory.Place(
                cameraTop,
                -diameter * 0.1f,
                diameter * 0.16f,
                diameter * 0.2f,
                diameter * 0.1f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));

            if (name.Contains("Lost", StringComparison.Ordinal))
            {
                RectTransform badge = _ui.Circle(
                    "Lost Badge",
                    icon,
                    SolarARColors.Panel,
                    diameter * 0.28f);
                SolarARUIFactory.Place(
                    badge,
                    diameter * 0.2f,
                    -diameter * 0.2f,
                    diameter * 0.28f,
                    diameter * 0.28f,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f));
                AddCross(badge, SolarARColors.Blue1, diameter * 0.16f);
            }
        }

        private static void CreateCheckMark(Transform parent)
        {
            RectTransform root = _ui.Rect("Check Mark", parent);
            SolarARUIFactory.Place(
                root,
                20f,
                0f,
                24f,
                24f,
                new Vector2(0f, 0.5f),
                new Vector2(0.5f, 0.5f));

            Image shortLine = _ui.Line("Short", root, SolarARColors.Ink);
            SolarARUIFactory.Place(
                shortLine.rectTransform,
                -4f,
                -1f,
                10f,
                3f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            shortLine.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -45f);

            Image longLine = _ui.Line("Long", root, SolarARColors.Ink);
            SolarARUIFactory.Place(
                longLine.rectTransform,
                4f,
                2f,
                17f,
                3f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            longLine.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        }

        private static void AddCross(Transform parent, Color color, float length)
        {
            Image first = _ui.Line("Cross A", parent, color);
            SolarARUIFactory.Place(
                first.rectTransform,
                0f,
                0f,
                length,
                2f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            first.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);

            Image second = _ui.Line("Cross B", parent, color);
            SolarARUIFactory.Place(
                second.rectTransform,
                0f,
                0f,
                length,
                2f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            second.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -45f);
        }

        private static void CreateLogoArt(Transform parent, float top)
        {
            RectTransform art = _ui.Rect("SolarAR Illustration", parent);
            SolarARUIFactory.Place(
                art,
                0f,
                -top,
                190f,
                171f,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f));
            RawImage logo = art.gameObject.AddComponent<RawImage>();
            logo.texture = _logoTexture;
            logo.color = Color.white;
            logo.raycastTarget = false;
        }

        private static void BuildCameraIllustration(Transform parent, float top)
        {
            RectTransform halo = _ui.Circle(
                "Camera Illustration Halo",
                parent,
                SolarARColors.Lavender,
                152f);
            SolarARUIFactory.Place(
                halo,
                0f,
                -top,
                152f,
                152f,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f));

            RectTransform body = _ui.Panel(
                "Camera Illustration",
                halo,
                SolarARColors.Blue1,
                22f);
            SolarARUIFactory.Place(
                body,
                0f,
                -2f,
                112f,
                78f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));

            RectTransform grip = _ui.Panel(
                "Camera Grip",
                halo,
                SolarARColors.Blue1,
                8f);
            SolarARUIFactory.Place(
                grip,
                -25f,
                42f,
                44f,
                22f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));

            RectTransform lensRing = _ui.Circle(
                "Camera Lens Ring",
                body,
                SolarARColors.Cyan,
                54f);
            SolarARUIFactory.Place(
                lensRing,
                0f,
                0f,
                54f,
                54f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));

            RectTransform lens = _ui.Circle(
                "Camera Lens",
                lensRing,
                SolarARColors.Night,
                34f);
            SolarARUIFactory.Place(
                lens,
                0f,
                0f,
                34f,
                34f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));

            RectTransform lensLight = _ui.Circle(
                "Camera Lens Light",
                lens,
                SolarARColors.Panel,
                9f);
            SolarARUIFactory.Place(
                lensLight,
                -7f,
                7f,
                9f,
                9f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));

            RectTransform flash = _ui.Circle(
                "Camera Flash",
                body,
                SolarARColors.Primary,
                12f);
            SolarARUIFactory.Place(
                flash,
                38f,
                20f,
                12f,
                12f,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
        }

        private static RectTransform CreateCuriosityMoonArtwork(
            Transform parent,
            string name)
        {
            RectTransform artwork = _ui.Rect(name, parent);
            RawImage image = artwork.gameObject.AddComponent<RawImage>();
            image.texture = _curiosityMoonTexture;
            image.color = Color.white;
            image.raycastTarget = false;
            return artwork;
        }

        private static TMP_Text AddText(
            Transform parent,
            string name,
            string value,
            float fontSize,
            float top,
            float height,
            float left,
            float right,
            bool title,
            Color? color = null,
            TextAlignmentOptions alignment = TextAlignmentOptions.Center,
            FontStyles style = FontStyles.Normal)
        {
            TextMeshProUGUI text = _ui.Text(
                name,
                parent,
                value,
                fontSize,
                color ?? SolarARColors.OnDark,
                alignment,
                title,
                style);
            SolarARUIFactory.Top(text.rectTransform, top, height, left, right);
            return text;
        }

        private static Button AddButton(
            Transform parent,
            string name,
            string label,
            Color background,
            Color foreground,
            float top,
            float height,
            float left,
            float right,
            float radius = 24f,
            float fontSize = 19f,
            bool bold = true)
        {
            Button button = _ui.Button(
                name,
                parent,
                label,
                background,
                foreground,
                null,
                radius,
                fontSize);
            TextMeshProUGUI labelText = button.GetComponentInChildren<TextMeshProUGUI>();
            labelText.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
            SolarARUIFactory.Top(button.GetComponent<RectTransform>(), top, height, left, right);
            return button;
        }

        private static void Wire(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button != null && action != null)
            {
                UnityEventTools.AddPersistentListener(button.onClick, action);
            }
        }

        private static void WireInt(
            Button button,
            UnityEngine.Events.UnityAction<int> action,
            int value)
        {
            if (button != null && action != null)
            {
                UnityEventTools.AddIntPersistentListener(button.onClick, action, value);
            }
        }

        private static GameObject InstantiatePrefabForAuthoring(
            string path,
            Transform parent,
            string name)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                throw new InvalidOperationException($"UI prefab not found: {path}");
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.name = name;
            instance.transform.SetParent(parent, false);
            return instance;
        }

        private static T FindChildComponent<T>(Transform root, string name) where T : Component
        {
            foreach (T component in root.GetComponentsInChildren<T>(true))
            {
                if (component.gameObject.name == name)
                {
                    return component;
                }
            }

            throw new InvalidOperationException($"Component {typeof(T).Name} named '{name}' was not found.");
        }

        private static GameObject FindSceneObject(string name)
        {
            GameObject result = FindSceneObjectOrNull(name);
            if (result != null)
            {
                return result;
            }

            throw new InvalidOperationException($"Scene object not found: {name}");
        }

        private static GameObject FindSceneObjectOrNull(string name)
        {
            foreach (GameObject candidate in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (candidate.name == name && candidate.scene.IsValid())
                {
                    return candidate;
                }
            }

            return null;
        }

        private static void SavePrefab(GameObject source, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(source, path);
            UnityEngine.Object.DestroyImmediate(source);
        }

        private static void AssignControllerReferences(
            EarthMvpUI controller,
            EducationalContentManager education,
            TrackedImageHandler tracking,
            GameObject arSession,
            GameObject xrOrigin,
            SceneReferences references)
        {
            SerializedObject serialized = new(controller);
            SetObject(serialized, "_trackedImageHandler", tracking);
            SetObject(serialized, "_planetRegistry", tracking.GetComponent<PlanetRegistry>());
            SetObject(serialized, "_educationalContentManager", education);
            SetObject(serialized, "_arSessionRoot", arSession);
            SetObject(serialized, "_xrOriginRoot", xrOrigin);
            SetObject(serialized, "_planetPreviewController", references.PreviewController);
            SetObject(serialized, "_uiBackgroundCamera", references.BackgroundCamera);
            SetObject(serialized, "_nightBackground", references.NightBackground);
            SetObject(serialized, "_bottomNavigation", references.BottomNavigation);
            SetObject(serialized, "_exploreTabBackground", references.ExploreTabBackground);
            SetObject(serialized, "_planetsTabBackground", references.PlanetsTabBackground);
            SetObject(serialized, "_missionsTabBackground", references.MissionsTabBackground);
            SetObject(serialized, "_exploreTabText", references.ExploreTabText);
            SetObject(serialized, "_planetsTabText", references.PlanetsTabText);
            SetObject(serialized, "_missionsTabText", references.MissionsTabText);
            SetObject(serialized, "_permissionExplanation", references.PermissionExplanation);
            SetObject(serialized, "_onboardingScreen", references.Onboarding);
            SetObject(serialized, "_permissionScreen", references.Permission);
            SetObject(serialized, "_instructionsScreen", references.Instructions);
            SetObject(serialized, "_exploreScreen", references.Explore);
            SetObject(serialized, "_searchingScreen", references.Searching);
            SetObject(serialized, "_recognizedScreen", references.Recognized);
            SetObject(serialized, "_informationScreen", references.Information);
            SetObject(serialized, "_lostScreen", references.Lost);
            SetObject(serialized, "_planetsScreen", references.Planets);
            SetObject(serialized, "_planetDetailScreen", references.PlanetDetail);
            SetObject(serialized, "_missionsScreen", references.Missions);
            SetObject(serialized, "_missionHubController", references.MissionHubController);
            SetObject(serialized, "_planetNameText", references.PlanetName);
            SetObject(serialized, "_firstFactText", references.FirstFact);
            SetObject(serialized, "_secondFactText", references.SecondFact);
            SetObject(serialized, "_recognizedPlanetNameText", references.RecognizedPlanetName);
            SetObject(serialized, "_recognizedFirstFactText", references.RecognizedFirstFact);
            SetObject(serialized, "_recognizedSecondFactText", references.RecognizedSecondFact);
            SetObject(serialized, "_curiosityPanel", references.CuriosityPanel);
            SetObject(serialized, "_curiosityPrompt", references.CuriosityPrompt);
            SetObject(serialized, "_curiosityAccent", references.CuriosityAccent);
            SetObject(
                serialized,
                "_curiosityFirstFactBackground",
                references.CuriosityFirstFactBackground);
            SetObject(
                serialized,
                "_curiositySecondFactBackground",
                references.CuriositySecondFactBackground);
            SetObjectArray(serialized, "_planetGalleryImages", references.PlanetGalleryImages);
            SetObject(serialized, "_detailPlanetNameText", references.DetailPlanetName);
            SetObject(serialized, "_factCarouselController", references.FactCarouselController);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObject(SerializedObject serialized, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
            {
                throw new InvalidOperationException($"Serialized property not found: {propertyName}");
            }

            property.objectReferenceValue = value;
        }

        private static void SetObjectArray<T>(
            SerializedObject serialized,
            string propertyName,
            IReadOnlyList<T> values)
            where T : UnityEngine.Object
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null || !property.isArray)
            {
                throw new InvalidOperationException($"Serialized array property not found: {propertyName}");
            }

            property.arraySize = values.Count;
            for (int index = 0; index < values.Count; index++)
            {
                property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            }
        }

        private static void ValidateSingleComponent<T>(ICollection<string> errors)
            where T : Component
        {
            T[] components = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include);
            if (components.Length != 1)
            {
                errors.Add($"Expected one {typeof(T).Name}, found {components.Length}.");
            }
        }

        private static void SetInitialScreenStates(SceneReferences references)
        {
            references.BackgroundCamera.SetActive(true);
            references.NightBackground.SetActive(true);
            references.Onboarding.SetActive(true);
            references.Permission.SetActive(false);
            references.Instructions.SetActive(false);
            references.Explore.SetActive(false);
            references.Searching.SetActive(false);
            references.Recognized.SetActive(false);
            references.Information.SetActive(false);
            references.Lost.SetActive(false);
            references.Planets.SetActive(false);
            references.PlanetDetail.SetActive(false);
            references.Missions.SetActive(false);
            references.BottomNavigation.SetActive(false);
        }

        private static void EnsureSingleEventSystem()
        {
            EventSystem[] eventSystems = UnityEngine.Object.FindObjectsByType<EventSystem>(
                FindObjectsInactive.Include);

            if (eventSystems.Length > 1)
            {
                throw new InvalidOperationException("MainAR contains more than one EventSystem.");
            }

            if (eventSystems.Length == 1)
            {
                return;
            }

            new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(InputSystemUIInputModule));
        }

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];

            for (int index = 1; index < segments.Length; index++)
            {
                string next = current + "/" + segments[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[index]);
                }

                current = next;
            }
        }
    }
}
