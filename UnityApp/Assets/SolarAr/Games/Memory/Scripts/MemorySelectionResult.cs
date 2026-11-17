namespace SolarAR.Games.Memory
{
    public enum MemorySelectionStatus
    {
        Invalid,
        FirstCardRevealed,
        Match,
        Mismatch,
        Completed
    }

    public readonly struct MemorySelectionResult
    {
        public MemorySelectionResult(
            MemorySelectionStatus status,
            int firstCardIndex,
            int secondCardIndex)
        {
            Status = status;
            FirstCardIndex = firstCardIndex;
            SecondCardIndex = secondCardIndex;
        }

        public MemorySelectionStatus Status { get; }
        public int FirstCardIndex { get; }
        public int SecondCardIndex { get; }
        public bool IsValid => Status != MemorySelectionStatus.Invalid;
    }
}
