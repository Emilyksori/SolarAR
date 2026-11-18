namespace SolarAR.Education
{
    public enum PlanetFactCategory
    {
        OrbitAndRotation,
        Surface,
        Atmosphere,
        Temperature,
        MoonsAndRings,
        Structure,
        Observation,
        LifeAndWater
    }

    internal static class PlanetFactCategoryLabels
    {
        public static string GetLabel(PlanetFactCategory category)
        {
            return category switch
            {
                PlanetFactCategory.OrbitAndRotation => "Órbita e rotação",
                PlanetFactCategory.Surface => "Superfície",
                PlanetFactCategory.Atmosphere => "Atmosfera",
                PlanetFactCategory.Temperature => "Temperatura",
                PlanetFactCategory.MoonsAndRings => "Luas e anéis",
                PlanetFactCategory.Structure => "Estrutura",
                PlanetFactCategory.Observation => "Descobertas",
                PlanetFactCategory.LifeAndWater => "Vida e água",
                _ => "Curiosidade"
            };
        }
    }
}
