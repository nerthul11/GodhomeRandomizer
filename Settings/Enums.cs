using System;
using MenuChanger.Attributes;

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
}