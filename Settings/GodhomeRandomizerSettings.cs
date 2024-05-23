using MenuChanger.Attributes;
using System;

namespace GodhomeRandomizer.Settings
{
    [Flags]
    public enum AccessMode
    {
        Vanilla,
        Randomized,
        AllUnlocked
    }
    [Flags]
    public enum TierLimitMode
    {
        IncludeAll = 3,
        ExcludeRadiant = 2,
        ExcludeAscended = 1,
        Vanilla = 0
    }

    public enum PantheonLimitMode
    {
        None = 0,
        Master = 1,
        Artist = 2,
        Sage = 3,
        Knight = 4,
        [MenuLabel("All")]
        Hallownest = 5
    }

    public class GodhomeRandomizerSettings
    {
        public bool Enabled { get; set; } = false;
        public HOG HallOfGods { get; set;} = new();
        public bool GodhomeShop { get; set; } = false;
        public Panth Pantheons { get; set;} = new();
        public class HOG
        {
            [MenuLabel("HOG Statue access")]
            public AccessMode RandomizeStatueAccess { get; set; } = AccessMode.Vanilla;
            [MenuLabel("HOG Battle randomization")]
            public TierLimitMode RandomizeTiers { get; set; } = TierLimitMode.Vanilla;
            public bool RandomizeOrdeal { get; set; } = false;
            public bool DuplicateMarks { get; set; } = false;
        }

        public class Panth
        {
            [MenuLabel("Apply HOG access rules to Pantheons")]
            public bool ApplyAccessToPantheons { get; set; }
            [MenuLabel("Pantheons included")]
            public PantheonLimitMode PantheonsIncluded { get; set; } = PantheonLimitMode.None;
            public bool Completion { get; set; } = false;
            public bool Lifeblood { get; set; } = false;
            public bool Nail { get; set; } = false;
            public bool Shell { get; set; } = false;
            public bool Charms { get; set; } = false;
            public bool Soul { get; set; } = false;
            public bool AllAtOnce { get; set; } = false;
            public bool Hitless { get; set; } = false;
        }
    }

   
}