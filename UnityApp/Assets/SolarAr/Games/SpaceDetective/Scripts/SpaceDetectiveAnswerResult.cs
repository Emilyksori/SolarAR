using SolarAR.Games.Shared;

namespace SolarAR.Games.SpaceDetective
{
    public readonly struct SpaceDetectiveAnswerResult
    {
        public SpaceDetectiveAnswerResult(
            bool accepted,
            bool isCorrect,
            int points,
            PlanetId correctPlanet,
            string explanation)
        {
            Accepted = accepted;
            IsCorrect = isCorrect;
            Points = points;
            CorrectPlanet = correctPlanet;
            Explanation = explanation;
        }

        public bool Accepted { get; }
        public bool IsCorrect { get; }
        public int Points { get; }
        public PlanetId CorrectPlanet { get; }
        public string Explanation { get; }
    }
}
