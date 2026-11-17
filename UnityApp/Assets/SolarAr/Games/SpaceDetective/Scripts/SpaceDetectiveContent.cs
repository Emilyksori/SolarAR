using System.Collections.Generic;
using SolarAR.Games.Shared;

namespace SolarAR.Games.SpaceDetective
{
    public static class SpaceDetectiveContent
    {
        public static IReadOnlyList<SpaceDetectivePlanet> CreateDefault()
        {
            return new[]
            {
                new SpaceDetectivePlanet(
                    PlanetId.Mercury,
                    "Mercúrio é o planeta mais próximo do Sol e completa sua volta em apenas 88 dias terrestres.",
                    new[]
                    {
                        "Sou o planeta que fica mais perto do Sol.",
                        "Meu ano é o mais curto entre todos os planetas.",
                        "Minha superfície rochosa tem muitas crateras e quase não possui atmosfera."
                    }),
                new SpaceDetectivePlanet(
                    PlanetId.Earth,
                    "A Terra tem água líquida abundante, uma atmosfera protetora e condições conhecidas para a vida.",
                    new[]
                    {
                        "Tenho oceanos de água líquida cobrindo grande parte da superfície.",
                        "Sou o único planeta onde sabemos que existe vida.",
                        "Tenho um satélite natural chamado Lua."
                    }),
                new SpaceDetectivePlanet(
                    PlanetId.Mars,
                    "Marte parece avermelhado por causa de minerais de ferro oxidados presentes em seu solo.",
                    new[]
                    {
                        "Sou conhecido como o planeta vermelho.",
                        "Em mim fica o Monte Olimpo, o maior vulcão conhecido do Sistema Solar.",
                        "Tenho duas pequenas luas chamadas Fobos e Deimos."
                    }),
                new SpaceDetectivePlanet(
                    PlanetId.Jupiter,
                    "Júpiter é o maior planeta do Sistema Solar e é formado principalmente por gases.",
                    new[]
                    {
                        "Sou o maior planeta do Sistema Solar.",
                        "Tenho uma enorme tempestade chamada Grande Mancha Vermelha.",
                        "Sou um gigante gasoso e Ganimedes é uma de minhas luas."
                    }),
                new SpaceDetectivePlanet(
                    PlanetId.Saturn,
                    "Saturno é um gigante gasoso cercado por um amplo sistema de anéis feitos de gelo e rocha.",
                    new[]
                    {
                        "Meus grandes anéis são os mais visíveis entre os planetas.",
                        "Sou o segundo maior planeta do Sistema Solar.",
                        "Sou um gigante gasoso com densidade menor que a da água."
                    }),
                new SpaceDetectivePlanet(
                    PlanetId.Uranus,
                    "Urano é um gigante de gelo que gira quase deitado em relação ao caminho de sua órbita.",
                    new[]
                    {
                        "Giro quase deitado, como se rolasse ao redor do Sol.",
                        "Sou um gigante de gelo com aparência azul-esverdeada.",
                        "Levo cerca de 84 anos terrestres para completar uma volta ao redor do Sol."
                    }),
                new SpaceDetectivePlanet(
                    PlanetId.Neptune,
                    "Netuno é o planeta mais distante do Sol e possui ventos extremamente rápidos em sua atmosfera.",
                    new[]
                    {
                        "Sou o planeta mais distante do Sol entre os oito planetas.",
                        "Minha atmosfera possui os ventos mais rápidos já medidos em um planeta.",
                        "Sou um gigante de gelo de cor azul intensa."
                    })
            };
        }
    }
}
