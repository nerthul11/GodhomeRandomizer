using MenuChanger.Attributes;
using System;

namespace GodhomeRandomizer.Settings
{
    [Flags]
    public enum AccessMode
    {
        /// <summary>
        /// Statues will be unlocked the same way they are in the original game.
        /// </summary>
        Vanilla,
        /// <summary>
        /// Statues will be unlocked after obtaining a Statue Mark for them.
        /// </summary>
        Randomized,
        /// <summary>
        /// Statues will always be unlocked.
        ///
        AllUnlocked
    }
    [Flags]
    public enum TierLimitMode
    {
        /// <summary>
        /// All Hall of Gods tiers will be randomized.
        /// </summary>
        IncludeAll = 3,
        /// <summary>
        /// Attuned and Ascended Hall of Gods marks will be randomized.
        /// </summary>
        ExcludeRadiant = 2,
        /// <summary>
        /// Attuned Hall of Gods marks will be randomized.
        /// </summary>
        ExcludeAscended = 1,
        /// <summary>
        /// All Hall of Gods tiers will have vanilla behaviour.
        /// </summary>
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
        public bool Enabled { get; set; } = true;
        public HOG HallOfGods { get; set;} = new();
        public Panth Pantheons { get; set;} = new();
        public class HOG
        {
            [MenuLabel("HOG Statue access")]
            public AccessMode RandomizeStatueAccess { get; set; } = AccessMode.Vanilla;
            [MenuLabel("HOG Battle randomization")]
            public TierLimitMode RandomizeTiers { get; set; } = TierLimitMode.Vanilla;
            public bool RandomizeOrdeal { get; set; } = false;
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