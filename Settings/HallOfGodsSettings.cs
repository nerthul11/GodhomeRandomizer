using MenuChanger.Attributes;

namespace GodhomeRandomizer.Settings
{
    public class HallOfGodsSettings
    {
        [MenuLabel("HOG Statue access")]
        public AccessMode RandomizeStatueAccess { get; set; } = AccessMode.Vanilla;
        [MenuLabel("HOG Battle randomization")]
        public TierLimitMode RandomizeTiers { get; set; } = TierLimitMode.Vanilla;
        public bool RandomizeOrdeal { get; set; } = false;
        public bool DuplicateMarks { get; set; } = false;
    }
}