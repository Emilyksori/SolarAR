using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SolarAR.Education.Editor
{
    public static class PlanetFactsSeeder
    {
        public const string FactsFolder = "Assets/SolarAr/Education/Facts";

        private readonly struct FactDefinition
        {
            public FactDefinition(
                string id,
                string title,
                string description,
                PlanetFactCategory category)
            {
                Id = id;
                Title = title;
                Description = description;
                Category = category;
            }

            public string Id { get; }
            public string Title { get; }
            public string Description { get; }
            public PlanetFactCategory Category { get; }
        }

        private static readonly (string Key, string AssetName, FactDefinition[] Facts)[] Definitions =
        {
            ("mercury", "MercuryFacts", new[]
            {
                Fact("mercury-01-fast-year", "Ano veloz", "Mercúrio completa uma volta ao redor do Sol em apenas 88 dias da Terra.", PlanetFactCategory.OrbitAndRotation),
                Fact("mercury-02-long-solar-day", "Dia muito longo", "Do nascer do Sol ao próximo passam 176 dias terrestres.", PlanetFactCategory.OrbitAndRotation),
                Fact("mercury-03-cratered-world", "Mundo de crateras", "Sua superfície rochosa tem muitas crateras formadas por impactos antigos.", PlanetFactCategory.Surface),
                Fact("mercury-04-polar-ice", "Gelo perto do Sol", "Crateras sempre escuras nos polos conseguem guardar gelo.", PlanetFactCategory.LifeAndWater),
                Fact("mercury-05-thin-exosphere", "Quase sem ar", "Mercúrio não tem uma atmosfera espessa para segurar o calor.", PlanetFactCategory.Atmosphere),
                Fact("mercury-06-hot-and-cold", "Calor e frio", "Os dias podem ser muito quentes, mas as noites ficam extremamente frias.", PlanetFactCategory.Temperature),
                Fact("mercury-07-smallest-planet", "O menor planeta", "Mercúrio é o menor dos oito planetas do Sistema Solar.", PlanetFactCategory.Structure),
                Fact("mercury-08-no-moons", "Sem luas", "Mercúrio não possui luas nem anéis conhecidos.", PlanetFactCategory.MoonsAndRings)
            }),
            ("earth", "EarthFacts", new[]
            {
                Fact("earth-01-water-world", "Planeta azul", "A água cobre a maior parte da superfície da Terra.", PlanetFactCategory.LifeAndWater),
                Fact("earth-02-known-life", "Casa da vida", "A Terra é o único mundo onde sabemos que existe vida.", PlanetFactCategory.LifeAndWater),
                Fact("earth-03-moving-plates", "Planeta inquieto", "Grandes placas da crosta se movem lentamente e mudam a superfície.", PlanetFactCategory.Surface),
                Fact("earth-04-magnetic-shield", "Escudo invisível", "O campo magnético ajuda a proteger o planeta de partículas vindas do Sol.", PlanetFactCategory.Structure),
                Fact("earth-05-one-moon", "Nossa Lua", "A Lua ajuda a produzir as marés nos oceanos da Terra.", PlanetFactCategory.MoonsAndRings),
                Fact("earth-06-air-mixture", "Ar para respirar", "A atmosfera é formada principalmente por nitrogênio e oxigênio.", PlanetFactCategory.Atmosphere),
                Fact("earth-07-one-day", "Um giro por dia", "A Terra leva cerca de 24 horas para completar uma rotação.", PlanetFactCategory.OrbitAndRotation),
                Fact("earth-08-one-year", "Caminho anual", "Uma volta ao redor do Sol dura aproximadamente 365 dias.", PlanetFactCategory.OrbitAndRotation)
            }),
            ("mars", "MarsFacts", new[]
            {
                Fact("mars-01-red-dust", "Poeira vermelha", "O ferro presente no solo enferrujou e deu a Marte sua cor avermelhada.", PlanetFactCategory.Surface),
                Fact("mars-02-olympus-mons", "Vulcão gigante", "O Monte Olimpo é o maior vulcão conhecido do Sistema Solar.", PlanetFactCategory.Surface),
                Fact("mars-03-valles-marineris", "Cânion enorme", "Marte possui um sistema de cânions que atravessaria muitos países da Terra.", PlanetFactCategory.Surface),
                Fact("mars-04-two-moons", "Duas pequenas luas", "Fobos e Deimos são as duas luas pequenas e irregulares de Marte.", PlanetFactCategory.MoonsAndRings),
                Fact("mars-05-familiar-day", "Dia parecido", "Um dia marciano dura cerca de 24 horas e 39 minutos.", PlanetFactCategory.OrbitAndRotation),
                Fact("mars-06-ice-caps", "Calotas de gelo", "Os polos marcianos têm gelo de água e gelo de gás carbônico.", PlanetFactCategory.LifeAndWater),
                Fact("mars-07-ancient-water", "Rios do passado", "Marcas no terreno indicam que rios e lagos existiram há muito tempo.", PlanetFactCategory.LifeAndWater),
                Fact("mars-08-blue-sunset", "Pôr do sol azul", "A poeira fina faz o céu perto do Sol parecer azulado no entardecer.", PlanetFactCategory.Atmosphere)
            }),
            ("jupiter", "JupiterFacts", new[]
            {
                Fact("jupiter-01-largest", "Planeta gigante", "Júpiter é o maior planeta do Sistema Solar.", PlanetFactCategory.Structure),
                Fact("jupiter-02-gas-giant", "Sem chão sólido", "Ele é um gigante gasoso e não possui uma superfície sólida como a Terra.", PlanetFactCategory.Structure),
                Fact("jupiter-03-red-spot", "Grande Mancha Vermelha", "Uma tempestade enorme gira em sua atmosfera há muito tempo.", PlanetFactCategory.Atmosphere),
                Fact("jupiter-04-fast-day", "Dia relâmpago", "Mesmo sendo enorme, Júpiter gira em cerca de 10 horas.", PlanetFactCategory.OrbitAndRotation),
                Fact("jupiter-05-cloud-bands", "Faixas de nuvens", "Ventos em direções diferentes formam as faixas claras e escuras.", PlanetFactCategory.Atmosphere),
                Fact("jupiter-06-faint-rings", "Anéis discretos", "Júpiter possui anéis fracos feitos principalmente de poeira.", PlanetFactCategory.MoonsAndRings),
                Fact("jupiter-07-strong-magnet", "Ímã poderoso", "Seu campo magnético é muito mais forte que o da Terra.", PlanetFactCategory.Structure),
                Fact("jupiter-08-galilean-moons", "Quatro luas famosas", "Io, Europa, Ganimedes e Calisto foram observadas por Galileu em 1610.", PlanetFactCategory.Observation)
            }),
            ("saturn", "SaturnFacts", new[]
            {
                Fact("saturn-01-icy-rings", "Anéis de gelo", "Os anéis são formados por incontáveis pedaços de gelo e rocha.", PlanetFactCategory.MoonsAndRings),
                Fact("saturn-02-many-ringlets", "Muitos anéis", "O que parece um único conjunto é dividido em milhares de faixas estreitas.", PlanetFactCategory.MoonsAndRings),
                Fact("saturn-03-low-density", "Planeta que boiaria", "Saturno é menos denso que a água e boiaria em um oceano grande o bastante.", PlanetFactCategory.Structure),
                Fact("saturn-04-gas-giant", "Gigante gasoso", "Ele é composto principalmente por hidrogênio e hélio.", PlanetFactCategory.Structure),
                Fact("saturn-05-fast-spin", "Giro rápido", "Saturno completa uma rotação em pouco mais de 10 horas.", PlanetFactCategory.OrbitAndRotation),
                Fact("saturn-06-polar-hexagon", "Hexágono no polo", "Uma corrente de ar forma um desenho de seis lados perto do polo norte.", PlanetFactCategory.Atmosphere),
                Fact("saturn-07-flattened-shape", "Um pouco achatado", "A rotação rápida deixa o planeta mais largo no meio.", PlanetFactCategory.Structure),
                Fact("saturn-08-titan", "Lua com atmosfera", "Titã, uma de suas luas, possui uma atmosfera espessa.", PlanetFactCategory.MoonsAndRings)
            }),
            ("uranus", "UranusFacts", new[]
            {
                Fact("uranus-01-sideways", "Planeta deitado", "Urano gira quase de lado, como uma bola rolando ao redor do Sol.", PlanetFactCategory.OrbitAndRotation),
                Fact("uranus-02-long-year", "Ano distante", "Uma volta de Urano ao redor do Sol dura cerca de 84 anos terrestres.", PlanetFactCategory.OrbitAndRotation),
                Fact("uranus-03-long-seasons", "Estações gigantes", "Cada estação pode durar aproximadamente 21 anos da Terra.", PlanetFactCategory.OrbitAndRotation),
                Fact("uranus-04-ice-giant", "Gigante de gelo", "Seu interior contém materiais como água, metano e amônia sob grande pressão.", PlanetFactCategory.Structure),
                Fact("uranus-05-blue-green", "Cor azul-esverdeada", "O gás metano da atmosfera ajuda a produzir sua cor.", PlanetFactCategory.Atmosphere),
                Fact("uranus-06-faint-rings", "Anéis escuros", "Urano possui anéis estreitos e pouco brilhantes.", PlanetFactCategory.MoonsAndRings),
                Fact("uranus-07-very-cold", "Atmosfera gelada", "Urano possui algumas das temperaturas atmosféricas mais baixas entre os planetas.", PlanetFactCategory.Temperature),
                Fact("uranus-08-tilted-magnet", "Campo magnético inclinado", "Seu campo magnético é inclinado e não fica bem no centro do planeta.", PlanetFactCategory.Structure)
            }),
            ("neptune", "NeptuneFacts", new[]
            {
                Fact("neptune-01-farthest", "O mais distante", "Netuno é o planeta mais distante do Sol.", PlanetFactCategory.OrbitAndRotation),
                Fact("neptune-02-long-year", "Ano de 165 anos", "Netuno leva cerca de 165 anos terrestres para completar sua órbita.", PlanetFactCategory.OrbitAndRotation),
                Fact("neptune-03-fast-winds", "Ventos velozes", "Sua atmosfera possui os ventos mais rápidos observados entre os planetas.", PlanetFactCategory.Atmosphere),
                Fact("neptune-04-blue-world", "Mundo azul", "O metano e outros efeitos da atmosfera contribuem para sua aparência azul.", PlanetFactCategory.Atmosphere),
                Fact("neptune-05-ice-giant", "Gigante de gelo", "Netuno pertence ao grupo dos gigantes de gelo, assim como Urano.", PlanetFactCategory.Structure),
                Fact("neptune-06-triton", "Lua ao contrário", "Tritão percorre sua órbita no sentido contrário ao da rotação de Netuno.", PlanetFactCategory.MoonsAndRings),
                Fact("neptune-07-dark-storms", "Tempestades escuras", "Grandes tempestades podem surgir e desaparecer em sua atmosfera.", PlanetFactCategory.Atmosphere),
                Fact("neptune-08-short-day", "Dia de 16 horas", "Apesar da longa órbita, Netuno gira em torno de si mesmo em cerca de 16 horas.", PlanetFactCategory.OrbitAndRotation)
            })
        };

        [MenuItem("SolarAR/Education/Seed 56 Planet Facts")]
        public static void EnsureAll()
        {
            foreach ((string key, string assetName, FactDefinition[] facts) in Definitions)
            {
                string assetPath = $"{FactsFolder}/{assetName}.asset";
                PlanetFacts asset = AssetDatabase.LoadAssetAtPath<PlanetFacts>(assetPath);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<PlanetFacts>();
                    AssetDatabase.CreateAsset(asset, assetPath);
                }

                SerializedObject serialized = new(asset);
                serialized.FindProperty("_planetKey").stringValue = key;
                SerializedProperty factList = serialized.FindProperty("_facts");
                factList.arraySize = facts.Length;

                for (int index = 0; index < facts.Length; index++)
                {
                    FactDefinition definition = facts[index];
                    SerializedProperty fact = factList.GetArrayElementAtIndex(index);
                    fact.FindPropertyRelative("_id").stringValue = definition.Id;
                    fact.FindPropertyRelative("_title").stringValue = definition.Title;
                    fact.FindPropertyRelative("_description").stringValue = definition.Description;
                    fact.FindPropertyRelative("_category").enumValueIndex = (int)definition.Category;
                    fact.FindPropertyRelative("_order").intValue = index + 1;
                    fact.FindPropertyRelative("_visual").objectReferenceValue = null;
                }

                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(asset);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[SolarAR Education] Seeded 56 facts for 7 planets.");
        }

        public static void ValidateAll(ICollection<string> errors)
        {
            HashSet<string> ids = new(StringComparer.Ordinal);
            int total = 0;

            foreach ((string expectedKey, string assetName, FactDefinition[] _) in Definitions)
            {
                string assetPath = $"{FactsFolder}/{assetName}.asset";
                PlanetFacts asset = AssetDatabase.LoadAssetAtPath<PlanetFacts>(assetPath);
                if (asset == null)
                {
                    errors.Add($"Missing PlanetFacts asset: {assetPath}.");
                    continue;
                }

                if (!string.Equals(asset.PlanetKey, expectedKey, StringComparison.Ordinal))
                {
                    errors.Add($"Unexpected planet key in {assetPath}: '{asset.PlanetKey}'.");
                }

                if (asset.Facts == null || asset.Facts.Count != 8)
                {
                    errors.Add($"{assetPath} must contain exactly 8 facts.");
                    continue;
                }

                for (int index = 0; index < asset.Facts.Count; index++)
                {
                    PlanetFact fact = asset.Facts[index];
                    total++;
                    if (fact == null || string.IsNullOrWhiteSpace(fact.Id) ||
                        string.IsNullOrWhiteSpace(fact.Title) ||
                        string.IsNullOrWhiteSpace(fact.Description))
                    {
                        errors.Add($"Invalid fact at {assetPath} index {index}.");
                        continue;
                    }

                    if (!ids.Add(fact.Id))
                    {
                        errors.Add($"Duplicate fact id: {fact.Id}.");
                    }

                    if (fact.Order != index + 1)
                    {
                        errors.Add($"Unexpected order for fact {fact.Id}: {fact.Order}.");
                    }
                }
            }

            if (total != 56)
            {
                errors.Add($"Expected 56 educational facts, found {total}.");
            }
        }

        private static FactDefinition Fact(
            string id,
            string title,
            string description,
            PlanetFactCategory category)
        {
            return new FactDefinition(id, title, description, category);
        }
    }
}
