using GodhomeRandomizer.Interop;
using GodhomeRandomizer.Manager;
using GodhomeRandomizer.Settings;
using Modding;
using System;

namespace GodhomeRandomizer
{
    public class GodhomeRandomizer : Mod, IGlobalSettings<GodhomeRandomizerSettings>
    {
        new public string GetName() => "GodhomeRandomizer";
        public override string GetVersion() => "2.2.4.8";

        private static GodhomeRandomizer _instance;
        public GodhomeRandomizer() : base()
        {
            _instance = this;
        }
        internal static GodhomeRandomizer Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"{nameof(GodhomeRandomizer)} was never initialized");
                }
                return _instance;
            }
        }
        public GodhomeRandomizerSettings GS { get; set; } = new();
        public override void Initialize()
        {
            // Ignore completely if Randomizer 4 is inactive
            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                Log("Initializing");
                GodhomeManager.Hook();
                if (ModHooks.GetMod("FStatsMod") is Mod)
                {
                    FStats_Interop.Hook();
                }
                if (ModHooks.GetMod("MoreLocations") is Mod)
                {
                    MoreLocations_Interop.Hook();
                }
                if (ModHooks.GetMod("RandoSettingsManager") is Mod)
                {
                    RSM_Interop.Hook();
                }
                CondensedSpoilerLogger.AddCategory("Pantheon Completion", () => GodhomeManager.GlobalSettings.Enabled && GodhomeManager.GlobalSettings.Pantheons.Completion,
                    [
                        "Pantheon_Master-Completion", "Pantheon_Artist-Completion",
                        "Pantheon_Sage-Completion", "Pantheon_Knight-Completion",
                        "Pantheon_Hallownest-Completion"
                    ]
                );
                Log("Initialized");
            }
        }        

        public void OnLoadGlobal(GodhomeRandomizerSettings s) => GS = s;
        public GodhomeRandomizerSettings OnSaveGlobal() => GS;
    }   
}