using System;

namespace GodhomeRandomizer.Settings
{
    public class GlobalSettings
    {
        public GodhomeRandomizerSettings Settings { get; set; } = new();
    }
    public class LocalSettings
    {
        public AccessMode RandomizeStatueAccess { get; set; }
        public TierLimitMode RandomizeTiers { get; set; }
        public bool RandomizeOrdeal { get; set; }
        public bool ApplyAccessToPantheons { get; set; }
        public PantheonLimitMode PantheonsIncluded { get; set; } = PantheonLimitMode.None;
        public bool Completion { get; set; } = false;
        public bool Nail { get; set; } = false;
        public bool Shell { get; set; } = false;
        public bool Charms { get; set; } = false;
        public bool Soul { get; set; } = false;
        public bool AllAtOnce { get; set; } = false;
        public bool Hitless { get; set; } = false;
        public HallOfGodsCompletion HallOfGodsCompletion { get; set; } = new();
        public PantheonCompletion PantheonCompletion { get; set; } = new();
        public int CurrentPantheon { get; set; } = 0;
        public int CurrentBossLevel { get; set; } = 0;
        public PantheonBindings CurrentPantheonRun { get; set; } = new();   
    }

    public class HallOfGodsCompletion {
        public BossStatue.Completion statueStateGruzMother { get; set; }
        public BossStatue.Completion statueStateVengefly { get; set; }
        public BossStatue.Completion statueStateBroodingMawlek { get; set; }
        public BossStatue.Completion statueStateFalseKnight { get; set; }
        public BossStatue.Completion statueStateFailedChampion { get; set; }
        public BossStatue.Completion statueStateHornet1 { get; set; }
        public BossStatue.Completion statueStateHornet2 { get; set; }
        public BossStatue.Completion statueStateMegaMossCharger { get; set; }
        public BossStatue.Completion statueStateMantisLords { get; set; }
        public BossStatue.Completion statueStateOblobbles { get; set; }
        public BossStatue.Completion statueStateGreyPrince { get; set; }
        public BossStatue.Completion statueStateBrokenVessel { get; set; }
        public BossStatue.Completion statueStateLostKin { get; set; }
        public BossStatue.Completion statueStateNosk { get; set; }
        public BossStatue.Completion statueStateFlukemarm { get; set; }
        public BossStatue.Completion statueStateCollector { get; set; }
        public BossStatue.Completion statueStateWatcherKnights { get; set; }
        public BossStatue.Completion statueStateSoulMaster { get; set; }
        public BossStatue.Completion statueStateSoulTyrant { get; set; }
        public BossStatue.Completion statueStateGodTamer { get; set; }
        public BossStatue.Completion statueStateCrystalGuardian1 { get; set; }
        public BossStatue.Completion statueStateCrystalGuardian2 { get; set; }
        public BossStatue.Completion statueStateUumuu { get; set; }
        public BossStatue.Completion statueStateDungDefender { get; set; }
        public BossStatue.Completion statueStateWhiteDefender { get; set; }
        public BossStatue.Completion statueStateHiveKnight { get; set; }
        public BossStatue.Completion statueStateTraitorLord { get; set; }
        public BossStatue.Completion statueStateGrimm { get; set; }
        public BossStatue.Completion statueStateNightmareGrimm { get; set; }
        public BossStatue.Completion statueStateHollowKnight { get; set; }
        public BossStatue.Completion statueStateElderHu { get; set; }
        public BossStatue.Completion statueStateGalien { get; set; }
        public BossStatue.Completion statueStateMarkoth { get; set; }
        public BossStatue.Completion statueStateMarmu { get; set; }
        public BossStatue.Completion statueStateNoEyes { get; set; }
        public BossStatue.Completion statueStateXero { get; set; }
        public BossStatue.Completion statueStateGorb { get; set; }
        public BossStatue.Completion statueStateRadiance { get; set; }
        public BossStatue.Completion statueStateSly { get; set; }
        public BossStatue.Completion statueStateNailmasters { get; set; }
        public BossStatue.Completion statueStateMageKnight { get; set; }
        public BossStatue.Completion statueStatePaintmaster { get; set; }
        public BossStatue.Completion statueStateNoskHornet { get; set; }
        public BossStatue.Completion statueStateMantisLordsExtra { get; set; }
        public T GetVariable<T>(string propertyName) {
            var property = typeof(HallOfGodsCompletion).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in HallOfGodsCompletion class.");
            }
            return (T)property.GetValue(this);
        }

        public void SetVariable<T>(string propertyName, T value) {
            var property = typeof(HallOfGodsCompletion).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in HallOfGodsCompletion class.");
            }
            property.SetValue(this, value);
        }
    }

    public class PantheonBindings
    {
        public bool Complete { get; set; }
        public bool Nail { get; set; }
        public bool Shell { get; set; }
        public bool Charms { get; set; }
        public bool Soul { get; set; }
        public bool AllAtOnce { get; set; }
        public bool Hitless { get; set; }
        public T GetVariable<T>(string propertyName) {
            var property = typeof(PantheonBindings).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in PantheonBindings class.");
            }
            return (T)property.GetValue(this);
        }

        public void SetVariable<T>(string propertyName, T value) {
            var property = typeof(PantheonBindings).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in PantheonBindings class.");
            }
            property.SetValue(this, value);
        }
    }
    public class PantheonCompletion
    {
        public PantheonBindings Master { get; set; } = new();
        public PantheonBindings Artist { get; set; } = new();
        public PantheonBindings Sage { get; set; } = new();
        public PantheonBindings Knight { get; set; } = new();
        public PantheonBindings Hallownest { get; set; } = new();
        public T GetVariable<T>(string propertyName) {
            var property = typeof(PantheonCompletion).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in PantheonCompletion class.");
            }
            return (T)property.GetValue(this);
        }

        public void SetVariable<T>(string propertyName, T value) {
            var property = typeof(PantheonCompletion).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in PantheonCompletion class.");
            }
            property.SetValue(this, value);
        }
    }
}