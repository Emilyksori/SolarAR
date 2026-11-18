using SolarAR.Games.Shared;

namespace SolarAR.Games.Memory
{
    public sealed class MemoryCardState
    {
        public MemoryCardState(int id, PlanetId planet)
        {
            Id = id;
            Planet = planet;
        }

        public int Id { get; }
        public PlanetId Planet { get; }
        public bool IsRevealed { get; internal set; }
        public bool IsMatched { get; internal set; }
    }
}
