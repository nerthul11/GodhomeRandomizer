using System;
using System.Collections.Generic;
using GodhomeRandomizer.Manager;
using GodhomeRandomizer.Settings;
using ItemChanger;

namespace GodhomeRandomizer.Modules
{
    public class StatueModule : ItemChanger.Modules.Module
    {
        public SaveSettings Settings { get; set; } = new();
        public class SaveSettings 
        {
            public AccessMode RandomizeStatueAccess { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.HallOfGods.RandomizeStatueAccess : AccessMode.Vanilla;
            public TierLimitMode RandomizeTiers { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.HallOfGods.RandomizeTiers : TierLimitMode.Vanilla;
            public bool ApplyAccessToPantheons { get; set; } = GodhomeManager.GlobalSettings.Enabled ? GodhomeManager.GlobalSettings.Pantheons.ApplyAccessToPantheons : false;
        }
        // Module properties
        public List<string> UnlockedScenes { get; set; } = [];
        public List<string> AttunedStatues { get; set; } = [];
        public List<string> AscendedStatues { get; set; } = [];
        public List<string> RadiantStatues { get; set; } = [];
        public int CurrentBossLevel { get; set; } = 0;
        public static StatueModule Instance => ItemChangerMod.Modules.GetOrAdd<StatueModule>();
        public override void Initialize() 
        {
            On.BossScene.IsUnlocked += UnlockCheck;
            On.BossSceneController.Awake += VanillaTracker;
            On.BossStatue.UpdateDetails += GetDetails;
        }
        public override void Unload() 
        {
            On.BossScene.IsUnlocked -= UnlockCheck;
            On.BossSceneController.Awake -= VanillaTracker;
            On.BossStatue.UpdateDetails -= GetDetails;
        }
        public int CurrentMarks(string battleScene)
        {
            int count = 0;
            if (UnlockedScenes.Contains(battleScene) && Settings.RandomizeStatueAccess == AccessMode.Randomized)
                count += 1;
            if (AttunedStatues.Contains(battleScene))
                count += 1;
            if (AscendedStatues.Contains(battleScene))
                count += 1;
            if (RadiantStatues.Contains(battleScene))
                count += 1;
            return count;
        }
        public int TotalMarks = !GodhomeManager.GlobalSettings.Enabled ? 0 : (GodhomeManager.GlobalSettings.HallOfGods.RandomizeStatueAccess == AccessMode.Randomized ? 1 : 0) + (int)GodhomeManager.GlobalSettings.HallOfGods.RandomizeTiers;
        public delegate void AchievedHallOfGods(List<string> marks);
        public event AchievedHallOfGods OnAchievedHallOfGods;

        // On Hook events
        private bool UnlockCheck(On.BossScene.orig_IsUnlocked orig, BossScene self, BossSceneCheckSource source)
        {
            List<string> exceptions = ["GG_Spa", "GG_Engine", "GG_Unn", "GG_Wyrm"];
            if (GameManager.instance.sceneName == SceneNames.GG_Workshop || (Settings.ApplyAccessToPantheons && GameManager.instance.sceneName == SceneNames.GG_Atrium))
            {
                if (Settings.RandomizeStatueAccess == AccessMode.AllUnlocked)
                {
                    if (!UnlockedScenes.Contains(self.Tier1Scene) && !exceptions.Contains(self.Tier1Scene))
                        UnlockedScenes.Add(self.Tier1Scene);
                    return true;
                }
                
                if (Settings.RandomizeStatueAccess == AccessMode.Randomized)
                    return UnlockedScenes.Contains(self.Tier1Scene);
            }
            
            if (!UnlockedScenes.Contains(self.Tier1Scene) && !exceptions.Contains(self.Tier1Scene) && orig(self, source))
                UnlockedScenes.Add(self.Tier1Scene);
            return orig(self, source);
        }
        public void CompletedChallenges()
        {
            List<string> completed = [];

            // Check for item state if randomized
            if (UnlockedScenes.Count == 44)
                completed.Add("All Statues Unlocked");
            if (AttunedStatues.Count == 44)
                completed.Add("All Attuned");
            if (AscendedStatues.Count == 44)
                completed.Add("All Ascended");
            if (RadiantStatues.Count == 44)
                completed.Add("All Radiant");
            if (PlayerData.instance.ordealAchieved)
                completed.Add("Eternal Ordeal Completed");
            OnAchievedHallOfGods?.Invoke(completed);
        }

        private void GetDetails(On.BossStatue.orig_UpdateDetails orig, BossStatue self)
        {
            StatueOverride(self.statueStatePD, self.bossScene.Tier1Scene);
            if (self.HasDreamVersion)
                StatueOverride(self.dreamStatueStatePD, self.dreamBossScene.Tier1Scene);
            PantheonModule.Instance.CurrentPantheon = 0;
            orig(self);
        }

        private void VanillaTracker(On.BossSceneController.orig_Awake orig, BossSceneController self)
        {
            string scene = self.gameObject.scene.name;

            // Force scene name to be equal to the Tier 1 Scene name
            if (scene.Contains("_V") && !scene.Contains("Mantis") && !scene.Contains("Broken_Vessel"))
            {
                // Failsafe for Vengefly
                scene = scene.Replace("GG_Vengefly", "GG_Wengefly");
                scene = scene.Replace("_V", "");
                scene = scene.Replace("GG_Wengefly", "GG_Vengefly");
            }
            
            if (PantheonModule.Instance.CurrentPantheon == 0)
            {
                self.OnBossesDead += delegate ()
                {
                    if (Settings.RandomizeTiers == TierLimitMode.Vanilla && self.BossLevel >= 0 && !AttunedStatues.Contains(scene))
                        AttunedStatues.Add(scene);
                    if (Settings.RandomizeTiers <= TierLimitMode.ExcludeAscended && self.BossLevel >= 1 && !AscendedStatues.Contains(scene))
                        AscendedStatues.Add(scene);
                    if (Settings.RandomizeTiers <= TierLimitMode.ExcludeRadiant && self.BossLevel >= 2 && !RadiantStatues.Contains(scene))
                        RadiantStatues.Add(scene);
                };
                self.OnBossSceneComplete += self.DoDreamReturn; 
            }
            orig(self);
        }
 
        public void StatueOverride(string statueStateName, string sceneName)
        {
            BossStatue.Completion orig = PlayerData.instance.GetVariable<BossStatue.Completion>(statueStateName);
            
            // Override: If settings enabled, orig is replaced. If not enabled, orig settings are copied.
            orig.isUnlocked = UnlockedScenes.Contains(sceneName);
            orig.completedTier1 = AttunedStatues.Contains(sceneName);
            orig.completedTier2 = AscendedStatues.Contains(sceneName);
            orig.completedTier3 = RadiantStatues.Contains(sceneName);

            // Save changes
            PlayerData.instance.SetVariable(statueStateName, orig);
            CompletedChallenges();
        }

        public T GetVariable<T>(string propertyName) {
            var property = typeof(StatueModule).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in StatueModule class.");
            }
            return (T)property.GetValue(this);
        }

        public void SetVariable<T>(string propertyName, T value) {
            var property = typeof(StatueModule).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' not found in StatueModule class.");
            }
            property.SetValue(this, value);
        }
    }
}