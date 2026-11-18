using System;
using System.Collections.Generic;

namespace SolarAR.Games.Shared
{
    public static class PlanetCatalog
    {
        private static readonly PlanetId[] PlanetIds =
        {
            PlanetId.Mercury,
            PlanetId.Earth,
            PlanetId.Mars,
            PlanetId.Jupiter,
            PlanetId.Saturn,
            PlanetId.Uranus,
            PlanetId.Neptune
        };

        public static IReadOnlyList<PlanetId> All => PlanetIds;

        public static string GetDisplayName(PlanetId planet)
        {
            return planet switch
            {
                PlanetId.Mercury => "Mercúrio",
                PlanetId.Earth => "Terra",
                PlanetId.Mars => "Marte",
                PlanetId.Jupiter => "Júpiter",
                PlanetId.Saturn => "Saturno",
                PlanetId.Uranus => "Urano",
                PlanetId.Neptune => "Netuno",
                _ => throw new ArgumentOutOfRangeException(nameof(planet), planet, null)
            };
        }

        public static int GetIndex(PlanetId planet)
        {
            return (int)planet;
        }
    }
}
