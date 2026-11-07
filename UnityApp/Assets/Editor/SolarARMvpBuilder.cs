using System;
using System.Collections.Generic;
using SolarAR.AR;
using SolarAR.Education;
using SolarAR.Interaction;
using SolarAR.Planets;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SolarAR.Editor
{
    public static class SolarARMvpBuilder
    {
        private const string ScenePath = "Assets/SolarAr/Scenes/MainAR.unity";
        private const string LibraryPath =
            "Assets/SolarAr/AR/ReferenceImages/SolarARReferenceImageLibrary.asset";
        private const string CardFolder = "Assets/SolarAr/AR/ReferenceImages";
        private const string ContentFolder = "Assets/SolarAr/Education/Data";
        private const string FactsFolder = "Assets/SolarAr/Education/Facts";
        private const string PrefabFolder = "Assets/SolarAr/Planets/Prefabs";
        private const string SolarSystemModelPath =
            "Assets/SolarAr/Planets/Models/SolarSystem.glb";
        private const string EarthModelPath = "Assets/SolarAr/Planets/Models/terra.glb";
        private const float CardWidthMeters = 0.06f;
        private const float CardHeightMeters = 0.08f;
        private const float DisplayDiameterMeters = 0.048f;

        private sealed class PlanetSpec
        {
            public string Key;
            public string DisplayName;
            public string EnglishName;
            public string ReferenceImageName;
            public string CardFile;
            public int OrderFromSun;
            public string ShortDescription;
            public string Summary;
            public float DiameterKm;
            public float DistanceMillionKm;
            public float OrbitalDays;
            public float RotationHours;
            public int Moons;
            public float TemperatureCelsius;
            public string FirstFactTitle;
            public string FirstFact;
            public string SecondFactTitle;
            public string SecondFact;

            public string PrefabPath => $"{PrefabFolder}/{EnglishName}Prefab.prefab";
            public string ContentPath => $"{ContentFolder}/{EnglishName}Content.asset";
            public string FactsPath => $"{FactsFolder}/{EnglishName}Facts.asset";
            public string CardPath => $"{CardFolder}/{CardFile}";
        }

        private static readonly PlanetSpec[] Planets =
        {
            new()
            {
                Key = "mercury", DisplayName = "Mercúrio", EnglishName = "Mercury",
                ReferenceImageName = "MercuryCard", CardFile = "MercuryCard.png", OrderFromSun = 1,
                ShortDescription = "O menor planeta e o mais próximo do Sol.",
                Summary = "Mercúrio é um mundo rochoso coberto por crateras. Ele completa uma volta ao redor do Sol mais rápido que qualquer outro planeta.",
                DiameterKm = 4879f, DistanceMillionKm = 58f, OrbitalDays = 88f,
                RotationHours = 1407.6f, Moons = 0, TemperatureCelsius = 167f,
                FirstFactTitle = "Ano veloz", FirstFact = "Seu ano dura apenas 88 dias terrestres.",
                SecondFactTitle = "Sem luas", SecondFact = "Mercúrio não possui luas nem anéis."
            },
            new()
            {
                Key = "earth", DisplayName = "Terra", EnglishName = "Earth",
                ReferenceImageName = "EarthCard", CardFile = "EarthCard.png", OrderFromSun = 3,
                ShortDescription = "Nosso planeta, coberto principalmente por água e cheio de vida.",
                Summary = "A Terra é o terceiro planeta a partir do Sol e o único mundo conhecido com vida. Sua atmosfera e água líquida ajudam a manter muitos seres vivos.",
                DiameterKm = 12756f, DistanceMillionKm = 150.2f, OrbitalDays = 365.25f,
                RotationHours = 23.9f, Moons = 1, TemperatureCelsius = 15f,
                FirstFactTitle = "Vida", FirstFact = "É o único planeta conhecido que tem vida.",
                SecondFactTitle = "Nossa Lua", SecondFact = "Tem 1 lua, chamada Lua."
            },
            new()
            {
                Key = "mars", DisplayName = "Marte", EnglishName = "Mars",
                ReferenceImageName = "MarsCard", CardFile = "MarsCard.png", OrderFromSun = 4,
                ShortDescription = "Um planeta rochoso e frio, conhecido como planeta vermelho.",
                Summary = "Marte tem solo rico em ferro, grandes vulcões e sinais de que água líquida correu por sua superfície no passado.",
                DiameterKm = 6780f, DistanceMillionKm = 228f, OrbitalDays = 687f,
                RotationHours = 24.6f, Moons = 2, TemperatureCelsius = -53f,
                FirstFactTitle = "Planeta vermelho", FirstFact = "O ferro do solo dá a Marte sua cor avermelhada.",
                SecondFactTitle = "Duas luas", SecondFact = "Suas luas pequenas se chamam Fobos e Deimos."
            },
            new()
            {
                Key = "jupiter", DisplayName = "Júpiter", EnglishName = "Jupiter",
                ReferenceImageName = "JupiterCard", CardFile = "JupiterCard.png", OrderFromSun = 5,
                ShortDescription = "O maior planeta do Sistema Solar.",
                Summary = "Júpiter é um gigante gasoso com faixas de nuvens, tempestades intensas e um enorme conjunto de luas.",
                DiameterKm = 142984f, DistanceMillionKm = 778f, OrbitalDays = 4333f,
                RotationHours = 9.9f, Moons = 115, TemperatureCelsius = -110f,
                FirstFactTitle = "Grande Mancha", FirstFact = "A Grande Mancha Vermelha é uma tempestade gigantesca.",
                SecondFactTitle = "Dia curto", SecondFact = "Um dia em Júpiter dura cerca de 10 horas."
            },
            new()
            {
                Key = "saturn", DisplayName = "Saturno", EnglishName = "Saturn",
                ReferenceImageName = "SaturnCard", CardFile = "SaturnCard.png", OrderFromSun = 6,
                ShortDescription = "Um gigante gasoso cercado por anéis brilhantes.",
                Summary = "Saturno possui milhares de pequenos anéis feitos principalmente de gelo e rocha. É o planeta menos denso do Sistema Solar.",
                DiameterKm = 120536f, DistanceMillionKm = 1434f, OrbitalDays = 10759f,
                RotationHours = 10.7f, Moons = 293, TemperatureCelsius = -140f,
                FirstFactTitle = "Muitos anéis", FirstFact = "Seus anéis são formados por incontáveis pedaços de gelo e rocha.",
                SecondFactTitle = "Muitas luas", SecondFact = "Saturno possui centenas de luas confirmadas."
            },
            new()
            {
                Key = "uranus", DisplayName = "Urano", EnglishName = "Uranus",
                ReferenceImageName = "UranusCard", CardFile = "UranusCard.png", OrderFromSun = 7,
                ShortDescription = "Um gigante de gelo azul-esverdeado que gira de lado.",
                Summary = "Urano tem metano em sua atmosfera, anéis discretos e uma inclinação tão grande que parece rolar ao redor do Sol.",
                DiameterKm = 51118f, DistanceMillionKm = 2900f, OrbitalDays = 30687f,
                RotationHours = -17.2f, Moons = 29, TemperatureCelsius = -195f,
                FirstFactTitle = "Gira de lado", FirstFact = "Seu eixo é inclinado em quase 98 graus.",
                SecondFactTitle = "Gigante gelado", SecondFact = "O metano ajuda a dar sua cor azul-esverdeada."
            },
            new()
            {
                Key = "neptune", DisplayName = "Netuno", EnglishName = "Neptune",
                ReferenceImageName = "NeptuneCard", CardFile = "NeptuneCard.png", OrderFromSun = 8,
                ShortDescription = "O planeta mais distante do Sol nesta experiência.",
                Summary = "Netuno é um gigante de gelo azul, muito frio e com os ventos mais rápidos observados entre os planetas.",
                DiameterKm = 49528f, DistanceMillionKm = 4495f, OrbitalDays = 59800f,
                RotationHours = 16.1f, Moons = 16, TemperatureCelsius = -200f,
                FirstFactTitle = "Ventos velozes", FirstFact = "Seus ventos podem ser mais rápidos que o som.",
                SecondFactTitle = "Lua Tritão", SecondFact = "Tritão, sua maior lua, orbita no sentido contrário."
            }
        };

        [MenuItem("SolarAR/MVP/Build Seven Planet MVP")]
        public static void BuildFromMenu()
        {
            BuildAll();
        }

        public static void BuildAllAndExit()
        {
            try
            {
                BuildAll();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        public static void BuildAll()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            ConfigureCardTextures();
            ConfigureReferenceImageLibrary();
            CreateEducationalAssets();
            CreatePlanetPrefabs();
            ConfigureSceneData();
            SolarARUIBuilder.Build();
            ValidateAll();
            AssetDatabase.SaveAssets();
            Debug.Log("[SolarAR MVP] Seven-planet Editor MVP built and validated successfully.");
        }

        private static void ConfigureCardTextures()
        {
            foreach (PlanetSpec planet in Planets)
            {
                TextureImporter importer = AssetImporter.GetAtPath(planet.CardPath) as TextureImporter;
                if (importer == null)
                {
                    throw new InvalidOperationException($"Card texture missing: {planet.CardPath}");
                }

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = 2048;
                importer.SaveAndReimport();
            }

            const string logoPath = "Assets/SolarAr/Branding/SolarARLogo.png";
            TextureImporter logoImporter = AssetImporter.GetAtPath(logoPath) as TextureImporter;
            if (logoImporter == null)
            {
                throw new InvalidOperationException($"Official logo missing: {logoPath}");
            }

            logoImporter.textureType = TextureImporterType.Sprite;
            logoImporter.spriteImportMode = SpriteImportMode.Single;
            logoImporter.mipmapEnabled = false;
            logoImporter.SaveAndReimport();
        }

        private static void ConfigureReferenceImageLibrary()
        {
            XRReferenceImageLibrary library =
                AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(LibraryPath);
            if (library == null)
            {
                throw new InvalidOperationException($"Reference image library missing: {LibraryPath}");
            }

            while (library.count > Planets.Length)
            {
                library.RemoveAt(library.count - 1);
            }

            while (library.count < Planets.Length)
            {
                library.Add();
            }

            for (int index = 0; index < Planets.Length; index++)
            {
                PlanetSpec planet = Planets[index];
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(planet.CardPath);
                if (texture == null)
                {
                    throw new InvalidOperationException($"Card texture missing: {planet.CardPath}");
                }

                float aspect = (float)texture.width / texture.height;
                float expectedAspect = CardWidthMeters / CardHeightMeters;
                if (Mathf.Abs(aspect - expectedAspect) > 0.001f)
                {
                    throw new InvalidOperationException(
                        $"Unexpected card aspect ratio for {planet.CardFile}: {aspect:0.####}");
                }

                library.SetName(index, planet.ReferenceImageName);
                library.SetTexture(index, texture, true);
                library.SetSpecifySize(index, true);
                library.SetSize(index, new Vector2(CardWidthMeters, CardHeightMeters));
            }

            EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssets();
        }

        private static void CreateEducationalAssets()
        {
            foreach (PlanetSpec planet in Planets)
            {
                PlanetContent content = LoadOrCreate<PlanetContent>(planet.ContentPath);
                SerializedObject contentData = new(content);
                SetString(contentData, "_planetKey", planet.Key);
                SetString(contentData, "_displayName", planet.DisplayName);
                SetInt(contentData, "_orderFromSun", planet.OrderFromSun);
                SetString(contentData, "_shortDescription", planet.ShortDescription);
                SetString(contentData, "_educationalSummary", planet.Summary);
                SetFloat(contentData, "_diameterKm", planet.DiameterKm);
                SetFloat(contentData, "_distanceFromSunMillionKm", planet.DistanceMillionKm);
                SetFloat(contentData, "_orbitalPeriodDays", planet.OrbitalDays);
                SetFloat(contentData, "_rotationPeriodHours", planet.RotationHours);
                SetInt(contentData, "_moonCount", planet.Moons);
                SetFloat(contentData, "_averageTemperatureCelsius", planet.TemperatureCelsius);
                SetObject(
                    contentData,
                    "_thumbnail",
                    AssetDatabase.LoadAssetAtPath<Sprite>(planet.CardPath));
                contentData.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(content);

                PlanetFacts facts = LoadOrCreate<PlanetFacts>(planet.FactsPath);
                SerializedObject factsData = new(facts);
                SetString(factsData, "_planetKey", planet.Key);
                SerializedProperty factList = factsData.FindProperty("_facts");
                factList.arraySize = 2;
                SetFact(factList.GetArrayElementAtIndex(0), planet.FirstFactTitle, planet.FirstFact);
                SetFact(factList.GetArrayElementAtIndex(1), planet.SecondFactTitle, planet.SecondFact);
                factsData.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(facts);
            }

            AssetDatabase.SaveAssets();
        }

        private static void CreatePlanetPrefabs()
        {
            GameObject solarSystem = AssetDatabase.LoadAssetAtPath<GameObject>(SolarSystemModelPath);
            GameObject earth = AssetDatabase.LoadAssetAtPath<GameObject>(EarthModelPath);
            if (solarSystem == null || earth == null)
            {
                throw new InvalidOperationException("Required local glTF planet models were not imported.");
            }

            float previousEarthDiameter = MeasurePrefabDiameter(
                AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/EarthPrefab.prefab"));

            foreach (PlanetSpec planet in Planets)
            {
                GameObject visualSource = planet.Key == "earth"
                    ? earth
                    : FindPlanetNode(solarSystem.transform, planet.Key)?.gameObject;
                if (visualSource == null)
                {
                    throw new InvalidOperationException(
                        $"Visual node for '{planet.Key}' was not found in {SolarSystemModelPath}.");
                }

                CreatePlanetPrefab(planet, visualSource);
            }

            Debug.Log(
                $"[SolarAR MVP] Earth visual diameter: previous={previousEarthDiameter:0.####} m, " +
                $"normalized={DisplayDiameterMeters:0.###} m.");
        }

        private static Transform FindPlanetNode(Transform root, string planetKey)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                string lowerName = child.name.ToLowerInvariant();
                if (lowerName.StartsWith(planetKey + "_beziercircle_", StringComparison.Ordinal) &&
                    !lowerName.Contains(".001"))
                {
                    return child;
                }
            }

            return null;
        }

        private static void CreatePlanetPrefab(PlanetSpec planet, GameObject visualSource)
        {
            GameObject root = new(planet.EnglishName + "Prefab");
            GameObject visualRootObject = new("VisualRoot");
            visualRootObject.transform.SetParent(root.transform, false);
            Transform visualRoot = visualRootObject.transform;

            GameObject model = UnityEngine.Object.Instantiate(visualSource, visualRoot);
            model.name = planet.EnglishName + "Model";

            Bounds bounds = CalculateBounds(model);
            Vector3 localCenter = visualRoot.InverseTransformPoint(bounds.center);
            model.transform.localPosition -= localCenter;
            bounds = CalculateBounds(model);
            float sourceDiameter = MaxDimension(bounds.size);
            if (sourceDiameter <= Mathf.Epsilon)
            {
                UnityEngine.Object.DestroyImmediate(root);
                throw new InvalidOperationException($"No renderer bounds found for {planet.DisplayName}.");
            }

            visualRoot.localScale = Vector3.one * (DisplayDiameterMeters / sourceDiameter);

            PlanetDisplay display = root.AddComponent<PlanetDisplay>();
            SerializedObject displayData = new(display);
            SetString(displayData, "_planetKey", planet.Key);
            SetObject(displayData, "_visualRoot", visualRoot);
            SetFloat(displayData, "_displayDiameterMeters", DisplayDiameterMeters);
            displayData.ApplyModifiedPropertiesWithoutUndo();

            RotationController rotation = root.AddComponent<RotationController>();
            SerializedObject rotationData = new(rotation);
            SetObject(rotationData, "_rotationTarget", visualRoot);
            rotationData.FindProperty("_rotationAxis").vector3Value = Vector3.up;
            SetFloat(rotationData, "_rotationSpeedDegreesPerSecond", 10f);
            rotationData.FindProperty("_autoRotate").boolValue = true;
            rotationData.ApplyModifiedPropertiesWithoutUndo();

            EarthGestureController gestures = root.AddComponent<EarthGestureController>();
            SerializedObject gestureData = new(gestures);
            SetObject(gestureData, "_visualTarget", visualRoot);
            SetObject(gestureData, "_rotationController", rotation);
            SetFloat(gestureData, "_minScaleMultiplier", 0.65f);
            SetFloat(gestureData, "_maxScaleMultiplier", 1.75f);
            gestureData.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, planet.PrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
        }

        private static void ConfigureSceneData()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject xrOrigin = FindSceneObject(scene, "XR Origin");
            GameObject educationObject = FindSceneObject(scene, "EducationalContentManager");

            PlanetRegistry registry = xrOrigin.GetComponent<PlanetRegistry>();
            TrackedImageHandler handler = xrOrigin.GetComponent<TrackedImageHandler>();
            ARTrackedImageManager imageManager = xrOrigin.GetComponent<ARTrackedImageManager>();
            EducationalContentManager education = educationObject.GetComponent<EducationalContentManager>();
            if (registry == null || handler == null || imageManager == null || education == null)
            {
                throw new InvalidOperationException("MainAR is missing a required MVP component.");
            }

            SerializedObject registryData = new(registry);
            SerializedProperty entries = registryData.FindProperty("_planets");
            entries.arraySize = Planets.Length;
            for (int index = 0; index < Planets.Length; index++)
            {
                PlanetSpec planet = Planets[index];
                SerializedProperty entry = entries.GetArrayElementAtIndex(index);
                entry.FindPropertyRelative("_referenceImageName").stringValue = planet.ReferenceImageName;
                entry.FindPropertyRelative("_planetKey").stringValue = planet.Key;
                entry.FindPropertyRelative("_planetPrefab").objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<GameObject>(planet.PrefabPath);
            }
            registryData.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject handlerData = new(handler);
            SetObject(handlerData, "_planetRegistry", registry);
            handlerData.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject educationData = new(education);
            SetAssetList<PlanetContent>(educationData.FindProperty("_planetContents"),
                planet => planet.ContentPath);
            SetAssetList<PlanetFacts>(educationData.FindProperty("_planetFacts"),
                planet => planet.FactsPath);
            educationData.ApplyModifiedPropertiesWithoutUndo();

            imageManager.referenceLibrary =
                AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(LibraryPath);
            imageManager.requestedMaxNumberOfMovingImages = 1;

            EditorUtility.SetDirty(registry);
            EditorUtility.SetDirty(handler);
            EditorUtility.SetDirty(education);
            EditorUtility.SetDirty(imageManager);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
        }

        private static void ValidateAll()
        {
            List<string> errors = new();
            ValidateLibrary(errors);
            ValidatePrefabs(errors);
            ValidateSceneData(errors);
            SolarARUIBuilder.ValidateScene();

            if (errors.Count > 0)
            {
                throw new InvalidOperationException(
                    "SolarAR MVP validation failed:\n- " + string.Join("\n- ", errors));
            }

            Debug.Log(
                "[SolarAR MVP Validation] PASS: 7 cards, 7 normalized prefabs, 7 educational entries, " +
                "registry mappings, preview UI and MainAR references are valid.");
        }

        private static void ValidateLibrary(List<string> errors)
        {
            XRReferenceImageLibrary library =
                AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(LibraryPath);
            if (library == null || library.count != Planets.Length)
            {
                errors.Add($"Reference library must contain exactly {Planets.Length} images.");
                return;
            }

            for (int index = 0; index < Planets.Length; index++)
            {
                XRReferenceImage image = library[index];
                PlanetSpec planet = Planets[index];
                if (image.name != planet.ReferenceImageName)
                {
                    errors.Add($"Reference image {index} is '{image.name}', expected '{planet.ReferenceImageName}'.");
                }

                if (!image.specifySize ||
                    Vector2.Distance(image.size, new Vector2(CardWidthMeters, CardHeightMeters)) > 0.0001f)
                {
                    errors.Add($"{planet.ReferenceImageName} must use physical size 0.06 x 0.08 m.");
                }

                string texturePath = AssetDatabase.GetAssetPath(image.texture);
                if (texturePath != planet.CardPath)
                {
                    errors.Add($"{planet.ReferenceImageName} uses unexpected texture '{texturePath}'.");
                }
            }
        }

        private static void ValidatePrefabs(List<string> errors)
        {
            foreach (PlanetSpec planet in Planets)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(planet.PrefabPath);
                if (prefab == null)
                {
                    errors.Add($"Missing prefab: {planet.PrefabPath}");
                    continue;
                }

                PlanetDisplay display = prefab.GetComponent<PlanetDisplay>();
                EarthGestureController gestures = prefab.GetComponent<EarthGestureController>();
                RotationController rotation = prefab.GetComponent<RotationController>();
                if (display == null || display.VisualRoot == null || gestures == null || rotation == null)
                {
                    errors.Add($"{prefab.name} is missing display, gesture, rotation, or VisualRoot configuration.");
                }

                float diameter = MeasurePrefabDiameter(prefab);
                if (Mathf.Abs(diameter - DisplayDiameterMeters) > 0.001f)
                {
                    errors.Add($"{prefab.name} visual bounds are {diameter:0.####} m, expected about 0.048 m.");
                }
            }
        }

        private static void ValidateSceneData(List<string> errors)
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            PlanetRegistry registry = FindSceneObject(scene, "XR Origin").GetComponent<PlanetRegistry>();
            EducationalContentManager education =
                FindSceneObject(scene, "EducationalContentManager")
                    .GetComponent<EducationalContentManager>();
            ARTrackedImageManager imageManager =
                FindSceneObject(scene, "XR Origin").GetComponent<ARTrackedImageManager>();

            foreach (PlanetSpec planet in Planets)
            {
                if (!registry.TryGetPrefab(planet.ReferenceImageName, out GameObject prefab) || prefab == null)
                {
                    errors.Add($"Missing registry mapping: {planet.ReferenceImageName} -> {planet.EnglishName}Prefab.");
                }
            }

            SerializedObject educationData = new(education);
            if (educationData.FindProperty("_planetContents").arraySize != Planets.Length ||
                educationData.FindProperty("_planetFacts").arraySize != Planets.Length)
            {
                errors.Add("EducationalContentManager must reference seven content and seven facts assets.");
            }

            if (imageManager.requestedMaxNumberOfMovingImages != 1)
            {
                errors.Add("ARTrackedImageManager must keep Max Moving Images set to 1.");
            }

            GameObject planetsScreen = FindSceneObject(scene, "03_Planetas");
            int galleryCardCount = 0;
            foreach (Button button in planetsScreen.GetComponentsInChildren<Button>(true))
            {
                if (!button.name.EndsWith(" Card", StringComparison.Ordinal))
                {
                    continue;
                }

                galleryCardCount++;
                if (button.onClick.GetPersistentEventCount() != 1 ||
                    button.onClick.GetPersistentMethodName(0) != "ShowPlanetDetail")
                {
                    errors.Add($"Gallery button '{button.name}' is not wired to ShowPlanetDetail.");
                }
            }

            if (galleryCardCount != Planets.Length)
            {
                errors.Add($"Planet gallery must contain exactly seven selectable cards, found {galleryCardCount}.");
            }

            GameObject detailScreen = FindSceneObject(scene, "04_PlanetDetail");
            ValidateButtonMethod(detailScreen, "Previous Button", "ShowPreviousPlanet", errors);
            ValidateButtonMethod(detailScreen, "Next Button", "ShowNextPlanet", errors);

            PlanetPreviewController preview =
                FindSceneObject(scene, "PlanetPreviewSystem").GetComponent<PlanetPreviewController>();
            SerializedObject previewData = new(preview);
            if (previewData.FindProperty("_previewCamera").objectReferenceValue == null ||
                previewData.FindProperty("_previewRoot").objectReferenceValue == null ||
                previewData.FindProperty("_outputImage").objectReferenceValue == null)
            {
                errors.Add("PlanetPreviewController has a missing required reference.");
            }

            RawImage logo = FindSceneObject(scene, "SolarAR Illustration").GetComponent<RawImage>();
            if (logo == null ||
                AssetDatabase.GetAssetPath(logo.texture) != "Assets/SolarAr/Branding/SolarARLogo.png")
            {
                errors.Add("Onboarding must reference the official SolarAR logo.");
            }

            foreach (TMP_Text text in UnityEngine.Object.FindObjectsByType<TMP_Text>(
                         FindObjectsInactive.Include))
            {
                if (text.text.Contains("Ouvir (opcional)", StringComparison.Ordinal) ||
                    text.text.Contains("Use com a câmera do celular", StringComparison.Ordinal) ||
                    text.text.Contains("Vênus", StringComparison.Ordinal))
                {
                    errors.Add($"Forbidden or out-of-scope UI text found on '{text.name}': {text.text}");
                }
            }

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
                {
                    foreach (Component component in transform.GetComponents<Component>())
                    {
                        if (component == null)
                        {
                            errors.Add($"Missing Script on {transform.name}.");
                        }
                    }
                }
            }
        }

        private static void ValidateButtonMethod(
            GameObject root,
            string buttonName,
            string methodName,
            List<string> errors)
        {
            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                if (button.name != buttonName)
                {
                    continue;
                }

                if (button.onClick.GetPersistentEventCount() != 1 ||
                    button.onClick.GetPersistentMethodName(0) != methodName)
                {
                    errors.Add($"Button '{buttonName}' is not wired to {methodName}.");
                }

                return;
            }

            errors.Add($"Button '{buttonName}' was not found.");
        }

        private static float MeasurePrefabDiameter(GameObject prefab)
        {
            if (prefab == null)
            {
                return 0f;
            }

            GameObject instance = UnityEngine.Object.Instantiate(prefab);
            float diameter = MaxDimension(CalculateBounds(instance).size);
            UnityEngine.Object.DestroyImmediate(instance);
            return diameter;
        }

        private static Bounds CalculateBounds(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return new Bounds(root.transform.position, Vector3.zero);
            }

            Bounds bounds = renderers[0].bounds;
            for (int index = 1; index < renderers.Length; index++)
            {
                bounds.Encapsulate(renderers[index].bounds);
            }

            return bounds;
        }

        private static float MaxDimension(Vector3 size)
        {
            return Mathf.Max(size.x, Mathf.Max(size.y, size.z));
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void SetAssetList<T>(
            SerializedProperty list,
            Func<PlanetSpec, string> pathSelector) where T : UnityEngine.Object
        {
            list.arraySize = Planets.Length;
            for (int index = 0; index < Planets.Length; index++)
            {
                list.GetArrayElementAtIndex(index).objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<T>(pathSelector(Planets[index]));
            }
        }

        private static void SetFact(SerializedProperty fact, string title, string description)
        {
            fact.FindPropertyRelative("_title").stringValue = title;
            fact.FindPropertyRelative("_description").stringValue = description;
        }

        private static GameObject FindSceneObject(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
                {
                    if (transform.name == name)
                    {
                        return transform.gameObject;
                    }
                }
            }

            throw new InvalidOperationException($"Scene object not found: {name}");
        }

        private static void SetString(SerializedObject serialized, string name, string value)
        {
            serialized.FindProperty(name).stringValue = value;
        }

        private static void SetInt(SerializedObject serialized, string name, int value)
        {
            serialized.FindProperty(name).intValue = value;
        }

        private static void SetFloat(SerializedObject serialized, string name, float value)
        {
            serialized.FindProperty(name).floatValue = value;
        }

        private static void SetObject(
            SerializedObject serialized,
            string name,
            UnityEngine.Object value)
        {
            serialized.FindProperty(name).objectReferenceValue = value;
        }
    }
}
