using GodhomeRandomizer.Manager;
using GodhomeRandomizer.Settings;
using ItemChanger;
using ItemChanger.Locations;

namespace GodhomeRandomizer.IC 
{    
    public class StatueLocation : AutoLocation
    {
        public enum Tier
        {
            Unlock = -1,
            Attuned = 0,
            Ascended = 1,
            Radiant = 2
        }
        public string battleScene { get; set; }
        public string statueStateName { get; set; }
        public Tier statueTier { get; set; }


        protected override void OnUnload()
        {
            On.BossScene.IsUnlocked -= UnlockCheck;
            On.BossStatue.SetPlaqueState -= GrantUnlockItem;
            On.BossChallengeUI.LoadBoss_int_bool -= StoreBossLevel;
            On.BossSceneController.Awake -= GrantCombatItem;
            On.BossStatue.UpdateDetails -= GetDetails;
        }

        protected override void OnLoad()
        {
            On.BossScene.IsUnlocked += UnlockCheck;
            On.BossStatue.SetPlaqueState += GrantUnlockItem;
            On.BossChallengeUI.LoadBoss_int_bool += StoreBossLevel;
            On.BossSceneController.Awake += GrantCombatItem;
            On.BossStatue.UpdateDetails += GetDetails;
        }

        private void GetDetails(On.BossStatue.orig_UpdateDetails orig, BossStatue self)
        {
            if (statueTier == Tier.Unlock)
            {
            GodhomeRandomizer.Instance.ManageStatueState(statueStateName, "isUnlocked", false);
            }
            if (statueTier == Tier.Attuned)
            {
            GodhomeRandomizer.Instance.ManageStatueState(statueStateName, "completedTier1", false);
            }
            if (statueTier == Tier.Ascended)
            {
            GodhomeRandomizer.Instance.ManageStatueState(statueStateName, "completedTier2", false);
            }
            if (statueTier == Tier.Radiant)
            {
            GodhomeRandomizer.Instance.ManageStatueState(statueStateName, "completedTier3", false);
            }
            GodhomeRandomizer.Instance.LS.CurrentPantheon = 0;
            orig(self);
        }

        private void GrantUnlockItem(On.BossStatue.orig_SetPlaqueState orig, BossStatue self, BossStatue.Completion statueState, BossStatueTrophyPlaque plaque, string playerDataKey)
        {
            bool locationMatch = self.UsingDreamVersion && self.dreamStatueStatePD == statueStateName || !self.UsingDreamVersion && self.statueStatePD == statueStateName;
            if (self.StatueState.isUnlocked == true && statueTier == Tier.Unlock && locationMatch)
            {
                if (!Placement.AllObtained())
                {
                    HeroController.instance.RelinquishControl();
                    Placement.GiveAll(new()
                    {
                        FlingType = FlingType.DirectDeposit,
                        MessageType = MessageType.Any
                    }, HeroController.instance.RegainControl);
                }
            }
            orig(self, statueState, plaque, playerDataKey);
        }
        

        private bool UnlockCheck(On.BossScene.orig_IsUnlocked orig, BossScene self, BossSceneCheckSource source)
        {
            if (GameManager.instance.sceneName == SceneNames.GG_Workshop || GodhomeManager.SaveSettings.ApplyAccessToPantheons)
            {
                if (GodhomeManager.SaveSettings.RandomizeStatueAccess == AccessMode.AllUnlocked)
                    return true;
                
                if (GodhomeManager.SaveSettings.RandomizeStatueAccess == AccessMode.Randomized && battleScene == self.Tier1Scene)
                {
                    BossStatue.Completion completion = PlayerData.instance.GetVariable<BossStatue.Completion>(statueStateName);
                    return completion.isUnlocked;
                }   
            }
            return orig(self, source);
        }

        private void GrantCombatItem(On.BossSceneController.orig_Awake orig, BossSceneController self)
        {
            string scene = self.gameObject.scene.name;
            bool isPantheon = GodhomeRandomizer.Instance.LS.CurrentPantheon > 0;
            bool levelValidation = (int)statueTier <= GodhomeRandomizer.Instance.LS.CurrentBossLevel;
            bool exceptions = battleScene.Contains("Nosk_Hornet") || battleScene.Contains("Mantis_Lords");
            bool sceneValidation = (exceptions && battleScene == scene) || scene.Contains(battleScene);
            if (!isPantheon && levelValidation && sceneValidation)
            {
                self.OnBossesDead += delegate ()
                {
                    if (!Placement.AllObtained())
                    {
                        HeroController.instance.RelinquishControl();
                        Placement.GiveAll(new()
                        {
                            FlingType = FlingType.DirectDeposit,
                            MessageType = MessageType.Corner
                        }, HeroController.instance.RegainControl);
                    };
                };
                self.OnBossSceneComplete += self.DoDreamReturn;  
            };
            orig(self);
        }

        private void StoreBossLevel(On.BossChallengeUI.orig_LoadBoss_int_bool orig, BossChallengeUI self, int level, bool doHideAnim)
        {
            GodhomeRandomizer.Instance.LS.CurrentBossLevel = level;
            orig(self, level, doHideAnim);
        }
    }
}