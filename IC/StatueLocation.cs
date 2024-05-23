using GodhomeRandomizer.Modules;
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
            On.BossStatue.SetPlaqueState -= GrantUnlockItem;
            On.BossSceneController.Awake -= GrantCombatItem;
        }

        protected override void OnLoad()
        {
            On.BossStatue.SetPlaqueState += GrantUnlockItem;
            On.BossSceneController.Awake += GrantCombatItem;
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

        private void GrantCombatItem(On.BossSceneController.orig_Awake orig, BossSceneController self)
        {
            string scene = self.gameObject.scene.name;

            // Validate we're fighting in Hall of Gods
            bool isPantheon = PantheonModule.Instance.CurrentPantheon > 0;

            // Validate the boss fight is equal or higher than the location requirement
            bool levelValidation = StatueModule.Instance.CurrentBossLevel >= (int)statueTier;

            // Validate the scene name matches the current boss
            bool exceptions = battleScene.Contains("Nosk_Hornet") || battleScene.Contains("Mantis_Lords");
            bool sceneValidation = (exceptions && battleScene == scene) || (!exceptions && scene.Contains(battleScene));
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
    }
}