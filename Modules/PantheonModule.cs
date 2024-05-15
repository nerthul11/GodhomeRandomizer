using System;
using System.Collections.Generic;
using GlobalEnums;
using GodhomeRandomizer.Manager;
using GodhomeRandomizer.Settings;
using ItemChanger;
using UnityEngine;

namespace GodhomeRandomizer.Modules
{
    public class PantheonModule : ItemChanger.Modules.Module
    {
        public override void Initialize()
        {
            On.BossSequenceController.SetupNewSequence += StoreBindings;
            On.BossSequenceBindingsDisplay.CountCompletedBindings += CompletedBindings;
            On.HeroController.TakeDamage += DisableHitless;
            On.BossSequenceController.FinishLastBossScene += VanillaTracker;
        }

        public override void Unload()
        {
            On.BossSequenceController.SetupNewSequence -= StoreBindings;
            On.BossSequenceBindingsDisplay.CountCompletedBindings -= CompletedBindings;
            On.HeroController.TakeDamage -= DisableHitless;
            On.BossSequenceController.FinishLastBossScene -= VanillaTracker;
        }
        public static PantheonModule Instance => ItemChangerMod.Modules.GetOrAdd<PantheonModule>();
        public SaveSettings Settings { get; set; } = new();
        // Setting values
        public class SaveSettings {
            public PantheonLimitMode PantheonsIncluded { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.Pantheons.PantheonsIncluded : PantheonLimitMode.None;
            public bool Completion { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.Pantheons.Completion : false;
            public bool Nail { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.Pantheons.Nail : false;
            public bool Shell { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.Pantheons.Shell : false;
            public bool Charms { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.Pantheons.Charms : false;
            public bool Soul { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.Pantheons.Soul : false;
            public bool AllAtOnce { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.Pantheons.AllAtOnce : false;
            public bool Hitless { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.Pantheons.Hitless : false;
        }
        // Module properties
        public PantheonBindings Master { get; set; } = new();
        public PantheonBindings Artist { get; set; } = new();
        public PantheonBindings Sage { get; set; } = new();
        public PantheonBindings Knight { get; set; } = new();
        public PantheonBindings Hallownest { get; set; } = new();
        private int CompletedBindings(On.BossSequenceBindingsDisplay.orig_CountCompletedBindings orig)
        {
            return CompletedBindings();
        }

        private int CompletedBindings()
        {
            int completed = 0;
            completed += Master.Nail ? 1 : 0;
            completed += Master.Shell ? 1 : 0;
            completed += Master.Charms ? 1 : 0;
            completed += Master.Soul ? 1 : 0;
            completed += Artist.Nail ? 1 : 0;
            completed += Artist.Shell ? 1 : 0;
            completed += Artist.Charms ? 1 : 0;
            completed += Artist.Soul ? 1 : 0;
            completed += Sage.Nail ? 1 : 0;
            completed += Sage.Shell ? 1 : 0;
            completed += Sage.Charms ? 1 : 0;
            completed += Sage.Soul ? 1 : 0;
            completed += Knight.Nail ? 1 : 0;
            completed += Knight.Shell ? 1 : 0;
            completed += Knight.Charms ? 1 : 0;
            completed += Knight.Soul ? 1 : 0;
            completed += Hallownest.Nail ? 1 : 0;
            completed += Hallownest.Shell ? 1 : 0;
            completed += Hallownest.Charms ? 1 : 0;
            completed += Hallownest.Soul ? 1 : 0;
            return completed;
        }
        public class PantheonBindings
        {
            public bool Completion { get; set; }
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
        public int CurrentPantheon { get; set; } = 0;
        public PantheonBindings CurrentPantheonRun { get; set; } = new();
        public delegate void AchievedPantheons(List<string> marks);
        public event AchievedPantheons OnAchievedPantheons;
        public void CompletedChallenges()
        {
            List<string> completed = new();
            if (Master.Completion)
                completed.Add("Pantheon 1 Completed");
            if (Artist.Completion)
                completed.Add("Pantheon 2 Completed");
            if (Sage.Completion)
                completed.Add("Pantheon 3 Completed");
            if (Knight.Completion)
                completed.Add("Pantheon 4 Completed");
            if (Hallownest.Completion)
                completed.Add("Pantheon 5 Completed");
            if (CompletedBindings() >= 8)
                completed.Add("Blue Door Unlocked");
            if (CompletedBindings() >= 16)
                completed.Add("All Lifeblood Obtained");
            if (CompletedBindings() == 20)
                completed.Add("All Bindings Cleared");
            if (Master.AllAtOnce && Artist.AllAtOnce && Sage.AllAtOnce && Knight.AllAtOnce && Hallownest.AllAtOnce)
                completed.Add("Unleashed Pantheons");
            if (Master.Hitless && Artist.Hitless && Sage.Hitless && Knight.Hitless && Hallownest.Hitless)
                completed.Add("Unscarred Pantheons");
            OnAchievedPantheons?.Invoke(completed);
        }

        private void StoreBindings(On.BossSequenceController.orig_SetupNewSequence orig, BossSequence sequence, BossSequenceController.ChallengeBindings bindings, string playerData)
        {
            string activeBindings = bindings.ToString();
            CurrentPantheon = int.Parse(playerData[playerData.Length-1].ToString());
            CurrentPantheonRun.SetVariable("Completion", true);
            CurrentPantheonRun.SetVariable("Nail", activeBindings.Contains("Nail"));
            CurrentPantheonRun.SetVariable("Shell", activeBindings.Contains("Shell"));
            CurrentPantheonRun.SetVariable("Charms", activeBindings.Contains("Charms"));
            CurrentPantheonRun.SetVariable("Soul", activeBindings.Contains("Soul"));
            CurrentPantheonRun.SetVariable("Hitless", true);
            CurrentPantheonRun.SetVariable("AllAtOnce", CurrentPantheonRun.Nail && CurrentPantheonRun.Shell && CurrentPantheonRun.Charms && CurrentPantheonRun.Soul);
            orig(sequence, bindings, playerData);
        }

        private void DisableHitless(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            CurrentPantheonRun.SetVariable("Hitless", false);
            orig(self, go, damageSide, damageAmount, hazardType);
        }

        private void VanillaTracker(On.BossSequenceController.orig_FinishLastBossScene orig, BossSceneController self)
        {
            orig(self);
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Master || !Settings.Completion) && CurrentPantheon == 1)
                Master.Completion = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Master || !Settings.Nail) && CurrentPantheon == 1 && CurrentPantheonRun.Nail)
                Master.Nail = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Master || !Settings.Shell) && CurrentPantheon == 1 && CurrentPantheonRun.Shell)
                Master.Shell = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Master || !Settings.Charms) && CurrentPantheon == 1 && CurrentPantheonRun.Charms)
                Master.Charms = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Master || !Settings.Soul) && CurrentPantheon == 1 && CurrentPantheonRun.Soul)
                Master.Soul = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Master || !Settings.AllAtOnce) && CurrentPantheon == 1 && CurrentPantheonRun.AllAtOnce)
                Master.AllAtOnce = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Master || !Settings.Hitless) && CurrentPantheon == 1 && CurrentPantheonRun.Hitless)
                Master.Hitless = true;
            
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Artist || !Settings.Completion) && CurrentPantheon == 2)
                Artist.Completion = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Artist || !Settings.Nail) && CurrentPantheon == 2 && CurrentPantheonRun.Nail)
                Artist.Nail = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Artist || !Settings.Shell) && CurrentPantheon == 2 && CurrentPantheonRun.Shell)
                Artist.Shell = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Artist || !Settings.Charms) && CurrentPantheon == 2 && CurrentPantheonRun.Charms)
                Artist.Charms = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Artist || !Settings.Soul) && CurrentPantheon == 2 && CurrentPantheonRun.Soul)
                Artist.Soul = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Artist || !Settings.AllAtOnce) && CurrentPantheon == 2 && CurrentPantheonRun.AllAtOnce)
                Artist.AllAtOnce = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Artist || !Settings.Hitless) && CurrentPantheon == 2 && CurrentPantheonRun.Hitless)
                Artist.Hitless = true;
            
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Sage || !Settings.Completion) && CurrentPantheon == 3)
                Sage.Completion = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Sage || !Settings.Nail) && CurrentPantheon == 3 && CurrentPantheonRun.Nail)
                Sage.Nail = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Sage || !Settings.Shell) && CurrentPantheon == 3 && CurrentPantheonRun.Shell)
                Sage.Shell = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Sage || !Settings.Charms) && CurrentPantheon == 3 && CurrentPantheonRun.Charms)
                Sage.Charms = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Sage || !Settings.Soul) && CurrentPantheon == 3 && CurrentPantheonRun.Soul)
                Sage.Soul = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Sage || !Settings.AllAtOnce) && CurrentPantheon == 3 && CurrentPantheonRun.AllAtOnce)
                Sage.AllAtOnce = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Sage || !Settings.Hitless) && CurrentPantheon == 3 && CurrentPantheonRun.Hitless)
                Sage.Hitless = true;
            
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Knight || !Settings.Completion) && CurrentPantheon == 4)
                Knight.Completion = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Knight || !Settings.Nail) && CurrentPantheon == 4 && CurrentPantheonRun.Nail)
                Knight.Nail = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Knight || !Settings.Shell) && CurrentPantheon == 4 && CurrentPantheonRun.Shell)
                Knight.Shell = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Knight || !Settings.Charms) && CurrentPantheon == 4 && CurrentPantheonRun.Charms)
                Knight.Charms = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Knight || !Settings.Soul) && CurrentPantheon == 4 && CurrentPantheonRun.Soul)
                Knight.Soul = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Knight || !Settings.AllAtOnce) && CurrentPantheon == 4 && CurrentPantheonRun.AllAtOnce)
                Knight.AllAtOnce = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Knight || !Settings.Hitless) && CurrentPantheon == 4 && CurrentPantheonRun.Hitless)
                Knight.Hitless = true;
            
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Hallownest || !Settings.Completion) && CurrentPantheon == 5)
                Hallownest.Completion = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Hallownest || !Settings.Nail) && CurrentPantheon == 5 && CurrentPantheonRun.Nail)
                Hallownest.Nail = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Hallownest || !Settings.Shell) && CurrentPantheon == 5 && CurrentPantheonRun.Shell)
                Hallownest.Shell = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Hallownest || !Settings.Charms) && CurrentPantheon == 5 && CurrentPantheonRun.Charms)
                Hallownest.Charms = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Hallownest || !Settings.Soul) && CurrentPantheon == 5 && CurrentPantheonRun.Soul)
                Hallownest.Soul = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Hallownest || !Settings.AllAtOnce) && CurrentPantheon == 5 && CurrentPantheonRun.AllAtOnce)
                Hallownest.AllAtOnce = true;
            if ((Settings.PantheonsIncluded < PantheonLimitMode.Hallownest || !Settings.Hitless) && CurrentPantheon == 5 && CurrentPantheonRun.Hitless)
                Hallownest.Hitless = true;

            CompletedChallenges();            
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
            PantheonBindings pantheon = GetVariable<PantheonBindings>(pantheonName);
            SetVariable(pantheonName, PantheonOverride(pantheonID, pantheon, bindingType, setAsTrue));
            CompletedChallenges();
        }

        public PantheonBindings PantheonOverride(int pantheonID, PantheonBindings pantheon, string bindingType, bool setAsTrue)
        {
            BossSequenceDoor.Completion orig = PlayerData.instance.GetVariable<BossSequenceDoor.Completion>($"bossDoorStateTier{pantheonID}");
            if (setAsTrue)
                pantheon.SetVariable(bindingType, true);

            if (Settings.Completion)
                orig.completed = pantheon.Completion;
            if (Settings.Nail)
                orig.boundNail = pantheon.Nail;
            if (Settings.Shell)
                orig.boundShell = pantheon.Shell;
            if (Settings.Charms)
                orig.boundCharms = pantheon.Charms;
            if (Settings.Soul)
                orig.boundSoul = pantheon.Soul;
            if (Settings.AllAtOnce)
                orig.allBindings = pantheon.AllAtOnce;
            if (Settings.Hitless)
                orig.noHits = pantheon.Hitless;

            // Save changes
            PlayerData.instance.SetVariable($"bossDoorStateTier{pantheonID}", orig);
            return pantheon;
        }
        
        public T GetVariable<T>(string propertyName) {
            var property = typeof(PantheonModule).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in PantheonCompletion class.");
            }
            return (T)property.GetValue(this);
        }

        public void SetVariable<T>(string propertyName, T value) {
            var property = typeof(PantheonModule).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in PantheonCompletion class.");
            }
            property.SetValue(this, value);
        }
    }
}