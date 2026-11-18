using System;
using System.Collections.Generic;
using SolarAR.Games.Quiz;
using UnityEditor;
using UnityEngine;

namespace SolarAR.Editor
{
    public static class QuizQuestionBankSeeder
    {
        public const string BankPath =
            "Assets/SolarAr/Games/Quiz/Data/QuizQuestionBank.asset";

        [MenuItem("SolarAR/Quiz/Rebuild Question Bank")]
        public static void BuildFromMenu()
        {
            EnsureBank();
            Debug.Log("[Quiz] Question bank rebuilt with 35 questions.");
        }

        public static QuizQuestionBank EnsureBank()
        {
            EnsureFolder("Assets/SolarAr/Games");
            EnsureFolder("Assets/SolarAr/Games/Quiz");
            EnsureFolder("Assets/SolarAr/Games/Quiz/Data");

            QuizQuestionBank bank = AssetDatabase.LoadAssetAtPath<QuizQuestionBank>(BankPath);
            if (bank == null)
            {
                bank = ScriptableObject.CreateInstance<QuizQuestionBank>();
                AssetDatabase.CreateAsset(bank, BankPath);
            }

            bank.ReplaceQuestionsForEditor(CreateQuestions());
            EditorUtility.SetDirty(bank);
            AssetDatabase.SaveAssets();
            return bank;
        }

        public static void ValidateBank(QuizQuestionBank bank, List<string> errors)
        {
            if (bank == null)
            {
                errors.Add("QuizQuestionBank asset is missing.");
                return;
            }

            if (bank.Questions == null || bank.Questions.Count != 35)
            {
                errors.Add("QuizQuestionBank must contain exactly 35 questions.");
                return;
            }

            string[] planetKeys =
            {
                "mercury", "earth", "mars", "jupiter", "saturn", "uranus", "neptune"
            };
            HashSet<string> ids = new(StringComparer.Ordinal);
            Dictionary<string, int> counts = new(StringComparer.Ordinal);
            foreach (string key in planetKeys)
            {
                counts[key] = 0;
            }

            foreach (QuizQuestion question in bank.Questions)
            {
                string validationError = null;
                if (question == null || !question.TryValidate(out validationError))
                {
                    errors.Add(validationError ?? "QuizQuestionBank contains a null question.");
                    continue;
                }

                if (!ids.Add(question.Id))
                {
                    errors.Add($"Duplicate quiz question ID: {question.Id}.");
                }

                if (!counts.ContainsKey(question.PlanetKey))
                {
                    errors.Add($"Unsupported quiz planet key: {question.PlanetKey}.");
                    continue;
                }

                counts[question.PlanetKey]++;
            }

            foreach (string key in planetKeys)
            {
                if (counts[key] != 5)
                {
                    errors.Add($"Planet '{key}' must have exactly five quiz questions.");
                }
            }
        }

        private static List<QuizQuestion> CreateQuestions()
        {
            return new List<QuizQuestion>
            {
                Q("mercury-01", "mercury", QuizDifficulty.Easy,
                    "Qual planeta fica mais perto do Sol?",
                    "Mercúrio", "Marte", "Netuno", 0,
                    "Mercúrio é o planeta mais próximo do Sol."),
                Q("mercury-02", "mercury", QuizDifficulty.Easy,
                    "Como Mercúrio se compara aos outros planetas em tamanho?",
                    "É o maior", "É o menor", "Tem o mesmo tamanho da Terra", 1,
                    "Mercúrio é o menor planeta do Sistema Solar."),
                Q("mercury-03", "mercury", QuizDifficulty.Medium,
                    "Um ano em Mercúrio dura aproximadamente quantos dias da Terra?",
                    "365 dias", "687 dias", "88 dias", 2,
                    "Mercúrio completa uma volta ao redor do Sol em cerca de 88 dias terrestres."),
                Q("mercury-04", "mercury", QuizDifficulty.Medium,
                    "Como é a camada de gases ao redor de Mercúrio?",
                    "É espessa e respirável", "É extremamente fina", "É cheia de nuvens de água", 1,
                    "Mercúrio tem uma camada de gases muito fina, chamada exosfera."),
                Q("mercury-05", "mercury", QuizDifficulty.Hard,
                    "O que pode existir em crateras muito escuras nos polos de Mercúrio?",
                    "Gelo de água", "Florestas", "Oceanos quentes", 0,
                    "Algumas crateras polares nunca recebem luz direta e podem guardar gelo de água."),

                Q("earth-01", "earth", QuizDifficulty.Easy,
                    "O que cobre a maior parte da superfície da Terra?",
                    "Areia", "Gelo", "Água líquida", 2,
                    "Oceanos de água líquida cobrem a maior parte da superfície terrestre."),
                Q("earth-02", "earth", QuizDifficulty.Easy,
                    "Qual característica torna a Terra especial entre os mundos conhecidos?",
                    "Tem vida conhecida", "É o maior planeta", "Não possui atmosfera", 0,
                    "Até hoje, a Terra é o único mundo onde sabemos que existe vida."),
                Q("earth-03", "earth", QuizDifficulty.Medium,
                    "Quantas luas naturais a Terra possui?",
                    "Nenhuma", "Uma", "Duas", 1,
                    "A Terra possui uma lua natural, chamada Lua."),
                Q("earth-04", "earth", QuizDifficulty.Medium,
                    "Quanto tempo a Terra leva, aproximadamente, para dar uma volta em torno de si mesma?",
                    "Uma semana", "Um mês", "Um dia", 2,
                    "A rotação da Terra dura cerca de 24 horas e forma o dia e a noite."),
                Q("earth-05", "earth", QuizDifficulty.Hard,
                    "O que o campo magnético da Terra ajuda a fazer?",
                    "Proteger o planeta de partículas do Sol", "Produzir a luz do Sol", "Parar a rotação da Terra", 0,
                    "O campo magnético desvia muitas partículas carregadas que vêm do Sol."),

                Q("mars-01", "mars", QuizDifficulty.Easy,
                    "Por que Marte é chamado de planeta vermelho?",
                    "Por causa de plantas vermelhas", "Por causa do ferro oxidado no solo", "Por causa de lava cobrindo tudo", 1,
                    "Minerais com ferro oxidado dão ao solo marciano sua cor avermelhada."),
                Q("mars-02", "mars", QuizDifficulty.Easy,
                    "Quantas luas naturais Marte possui?",
                    "Nenhuma", "Uma", "Duas", 2,
                    "Marte possui duas pequenas luas: Fobos e Deimos."),
                Q("mars-03", "mars", QuizDifficulty.Medium,
                    "Qual formação gigante existe em Marte?",
                    "O vulcão Monte Olimpo", "Uma floresta de gelo", "Um oceano de água líquida", 0,
                    "O Monte Olimpo é um enorme vulcão e uma das maiores montanhas conhecidas do Sistema Solar."),
                Q("mars-04", "mars", QuizDifficulty.Medium,
                    "Como é a atmosfera de Marte?",
                    "Espessa e rica em oxigênio", "Fina e rica em gás carbônico", "Igual à atmosfera da Terra", 1,
                    "A atmosfera marciana é fina e formada principalmente por dióxido de carbono."),
                Q("mars-05", "mars", QuizDifficulty.Hard,
                    "O que canais e minerais encontrados em Marte indicam?",
                    "Que Marte sempre foi seco", "Que há cidades escondidas", "Que água líquida existiu no passado", 2,
                    "Formas do terreno e minerais mostram que água líquida correu em Marte há muito tempo."),

                Q("jupiter-01", "jupiter", QuizDifficulty.Easy,
                    "Qual é o maior planeta do Sistema Solar?",
                    "Júpiter", "Terra", "Saturno", 0,
                    "Júpiter é o maior planeta do Sistema Solar."),
                Q("jupiter-02", "jupiter", QuizDifficulty.Easy,
                    "Que tipo de planeta é Júpiter?",
                    "Planeta rochoso", "Gigante gasoso", "Planeta-anão", 1,
                    "Júpiter é um gigante gasoso, formado principalmente por hidrogênio e hélio."),
                Q("jupiter-03", "jupiter", QuizDifficulty.Medium,
                    "O que é a Grande Mancha Vermelha de Júpiter?",
                    "Um continente", "Um vulcão", "Uma enorme tempestade", 2,
                    "A Grande Mancha Vermelha é uma tempestade gigantesca observada há muito tempo."),
                Q("jupiter-04", "jupiter", QuizDifficulty.Medium,
                    "Aproximadamente quanto dura um dia em Júpiter?",
                    "10 horas", "24 horas", "100 horas", 0,
                    "Júpiter gira muito rápido e completa uma rotação em cerca de 10 horas."),
                Q("jupiter-05", "jupiter", QuizDifficulty.Hard,
                    "Além de muitas luas, o que Júpiter também possui?",
                    "Uma superfície sólida para caminhar", "Anéis finos e pouco brilhantes", "Oceanos de água na superfície", 1,
                    "Júpiter possui anéis tênues, bem menos visíveis que os de Saturno."),

                Q("saturn-01", "saturn", QuizDifficulty.Easy,
                    "Do que são feitos principalmente os anéis de Saturno?",
                    "Fumaça", "Luz sólida", "Gelo e rocha", 2,
                    "Os anéis de Saturno são formados por incontáveis pedaços de gelo e rocha."),
                Q("saturn-02", "saturn", QuizDifficulty.Easy,
                    "Que tipo de planeta é Saturno?",
                    "Gigante gasoso", "Planeta rochoso", "Planeta-anão", 0,
                    "Saturno é um gigante gasoso, assim como Júpiter."),
                Q("saturn-03", "saturn", QuizDifficulty.Medium,
                    "O que é curioso sobre a densidade de Saturno?",
                    "É menor que a da água", "É igual à do ferro", "É a maior entre os planetas", 0,
                    "Saturno é menos denso que a água, embora não exista um oceano grande o bastante para testá-lo."),
                Q("saturn-04", "saturn", QuizDifficulty.Medium,
                    "Qual destas é uma grande lua de Saturno?",
                    "Fobos", "Tritão", "Titã", 2,
                    "Titã é a maior lua de Saturno e possui uma atmosfera espessa."),
                Q("saturn-05", "saturn", QuizDifficulty.Hard,
                    "Os anéis de Saturno formam uma única placa inteira?",
                    "Não, são muitas faixas com incontáveis pedaços", "Sim, são uma placa sólida", "Sim, são uma única nuvem", 0,
                    "Os anéis são divididos em muitas faixas e feitos de incontáveis partículas."),

                Q("uranus-01", "uranus", QuizDifficulty.Easy,
                    "O que torna a rotação de Urano diferente?",
                    "Ele não gira", "Ele gira quase de lado", "Ele gira ao redor da Terra", 1,
                    "O eixo de Urano é muito inclinado, por isso ele parece girar de lado."),
                Q("uranus-02", "uranus", QuizDifficulty.Easy,
                    "Que tipo de planeta é Urano?",
                    "Planeta rochoso", "Gigante gasoso quente", "Gigante de gelo", 2,
                    "Urano é classificado como gigante de gelo por sua composição e temperatura."),
                Q("uranus-03", "uranus", QuizDifficulty.Medium,
                    "Qual gás ajuda a dar a Urano sua cor azul-esverdeada?",
                    "Metano", "Oxigênio", "Vapor de ferro", 0,
                    "O metano na atmosfera absorve parte da luz vermelha e ajuda a criar a cor azul-esverdeada."),
                Q("uranus-04", "uranus", QuizDifficulty.Medium,
                    "Urano possui anéis?",
                    "Não possui", "Sim, possui anéis discretos", "Possui apenas um anel de fogo", 1,
                    "Urano possui um sistema de anéis escuros e menos visíveis que os de Saturno."),
                Q("uranus-05", "uranus", QuizDifficulty.Hard,
                    "Por que as estações de Urano são tão longas?",
                    "Porque ele não recebe luz", "Porque não gira", "Porque sua órbita é longa e seu eixo é muito inclinado", 2,
                    "A longa volta ao redor do Sol e a grande inclinação fazem cada estação durar muitos anos terrestres."),

                Q("neptune-01", "neptune", QuizDifficulty.Easy,
                    "Qual é o planeta mais distante do Sol entre os oito planetas?",
                    "Netuno", "Saturno", "Marte", 0,
                    "Netuno é o planeta mais distante do Sol na classificação atual dos oito planetas."),
                Q("neptune-02", "neptune", QuizDifficulty.Easy,
                    "Que tipo de planeta é Netuno?",
                    "Planeta rochoso", "Gigante gasoso quente", "Gigante de gelo", 2,
                    "Netuno é um gigante de gelo, assim como Urano."),
                Q("neptune-03", "neptune", QuizDifficulty.Medium,
                    "O que se destaca na atmosfera de Netuno?",
                    "Ausência de ventos", "Ventos extremamente rápidos", "Ar respirável", 1,
                    "Netuno possui alguns dos ventos mais rápidos observados no Sistema Solar."),
                Q("neptune-04", "neptune", QuizDifficulty.Medium,
                    "Qual é a maior lua de Netuno?",
                    "Tritão", "Titã", "Deimos", 0,
                    "Tritão é a maior lua de Netuno e percorre sua órbita em sentido incomum."),
                Q("neptune-05", "neptune", QuizDifficulty.Hard,
                    "Aproximadamente quanto tempo Netuno leva para dar uma volta ao redor do Sol?",
                    "Um ano terrestre", "Doze anos terrestres", "Cerca de 165 anos terrestres", 2,
                    "Por estar muito distante do Sol, Netuno leva cerca de 165 anos terrestres para completar sua órbita.")
            };
        }

        private static QuizQuestion Q(
            string id,
            string planetKey,
            QuizDifficulty difficulty,
            string prompt,
            string first,
            string second,
            string third,
            int correctIndex,
            string explanation)
        {
            return new QuizQuestion(
                id,
                planetKey,
                difficulty,
                prompt,
                new[] { first, second, third },
                correctIndex,
                explanation);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            int separator = path.LastIndexOf('/');
            string parent = path[..separator];
            string name = path[(separator + 1)..];
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
