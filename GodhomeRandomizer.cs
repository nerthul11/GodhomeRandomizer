using GodhomeRandomizer.Manager;
using GodhomeRandomizer.Settings;
using Modding;
using System;

namespace GodhomeRandomizer
{
    public class GodhomeRandomizer : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        new public string GetName() => "GodhomeRandomizer";
        public override string GetVersion() => "2.1.2.2";

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
        public LocalSettings LS { get; set; } = new();
        public GlobalSettings GS { get; set; } = new();
        public override void Initialize()
        {
            // Ignore completely if Randomizer 4 is inactive
            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                Log("Initializing");
                GodhomeManager.Hook();
                if (ModHooks.GetMod("RandoSettingsManager") is Mod)
                {
                    RSM_Interop.Hook();
                }
                Log("Initialized");
            }
        }

        public void OnLoadGlobal(GlobalSettings s) => GS = s;
        public GlobalSettings OnSaveGlobal() => GS;
        public void OnLoadLocal(LocalSettings s) => LS = s;
        public LocalSettings OnSaveLocal() => LS;

        public void ManageStatueState(string statueName, string tier, bool setAsTrue)
        {
            BossStatue.Completion statue = Instance.LS.HallOfGodsCompletion.GetVariable<BossStatue.Completion>(statueName);
            Instance.LS.HallOfGodsCompletion.SetVariable(statueName, StatueOverride(statueName, statue, tier, setAsTrue));
        }

        public void ManagePantheonState(string pantheonName, string bindingType, bool setAsTrue)
        {
            int pantheonID;
            if (pantheonName == "Master")
                pantheonID = 1;
            else if (pantheonName == "Artist")
                pantheonID = 2;
            else if (pantheonName == "Sage")
                pantheonID = 3;
            else if (pantheonName == "Knight")
                pantheonID = 4;
            else if (pantheonName == "Hallownest")
                pantheonID = 5;
            else
                throw new ArgumentException("Invalid Pantheon Name.");
            PantheonBindings pantheon = Instance.LS.PantheonCompletion.GetVariable<PantheonBindings>(pantheonName);
            Instance.LS.PantheonCompletion.SetVariable(pantheonName, PantheonOverride(pantheonID, pantheon, bindingType, setAsTrue));
        }

        public PantheonBindings PantheonOverride(int pantheonID, PantheonBindings pantheon, string bindingType, bool setAsTrue)
        {
            BossSequenceDoor.Completion orig = PlayerData.instance.GetVariable<BossSequenceDoor.Completion>($"bossDoorStateTier{pantheonID}");
            if (setAsTrue)
                pantheon.SetVariable(bindingType, true);
                
            LocalSettings settings = GodhomeManager.SaveSettings;
            if (settings.Completion)
                orig.completed = pantheon.Complete;
            if (settings.Nail)
                orig.boundNail = pantheon.Nail;
            if (settings.Shell)
                orig.boundShell = pantheon.Shell;
            if (settings.Charms)
                orig.boundCharms = pantheon.Charms;
            if (settings.Soul)
                orig.boundSoul = pantheon.Soul;
            if (settings.AllAtOnce)
                orig.allBindings = pantheon.AllAtOnce;
            if (settings.Hitless)
                orig.noHits = pantheon.Hitless;

            // Save changes
            PlayerData.instance.SetVariable($"bossDoorStateTier{pantheonID}", orig);
            return pantheon;
        }

        public BossStatue.Completion StatueOverride(string statueStateName, BossStatue.Completion statue, string tier, bool setAsTrue)
        {
            BossStatue.Completion orig = PlayerData.instance.GetVariable<BossStatue.Completion>(statueStateName);
            // Set LocalSettings statue values if item is obtained
            if (tier == "isUnlocked" && setAsTrue)
                statue.isUnlocked = true;
            if (tier == "completedTier1" && setAsTrue)
                statue.completedTier1 = true;
            if (tier == "completedTier2" && setAsTrue)
                statue.completedTier2 = true;
            if (tier == "completedTier3" && setAsTrue)
                statue.completedTier3 = true;
            
            // Override: If settings enabled, orig is replaced. If not enabled, orig settings are copied.
            LocalSettings settings = GodhomeManager.SaveSettings;
            if (settings.RandomizeStatueAccess == AccessMode.Randomized)
                orig.isUnlocked = statue.isUnlocked;
            else
                statue.isUnlocked = orig.isUnlocked;
            
            if (settings.RandomizeTiers > TierLimitMode.Vanilla)
                orig.completedTier1 = statue.completedTier1;
            else
                statue.completedTier1 = orig.completedTier1;

            if (settings.RandomizeTiers > TierLimitMode.ExcludeAscended)
                orig.completedTier2 = statue.completedTier2;
            else
                statue.completedTier2 = orig.completedTier2;

            if (settings.RandomizeTiers > TierLimitMode.ExcludeRadiant)
                orig.completedTier3 = statue.completedTier3;
            else
                statue.completedTier3 = orig.completedTier3;

            // Other properties are unhandled by the mod and always take orig values       
            statue.hasBeenSeen = orig.hasBeenSeen;
            statue.seenTier3Unlock = orig.seenTier3Unlock;
            statue.usingAltVersion = orig.usingAltVersion;

            // Save changes
            PlayerData.instance.SetVariable(statueStateName, orig);
            return statue;
        }
    }   
}