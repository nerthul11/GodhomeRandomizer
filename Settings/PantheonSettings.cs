using MenuChanger.Attributes;

namespace GodhomeRandomizer.Settings
{
    public class PantheonSettings
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