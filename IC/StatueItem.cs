using GodhomeRandomizer.Manager;
using GodhomeRandomizer.Modules;
using GodhomeRandomizer.Settings;
using ItemChanger;
using ItemChanger.UIDefs;
using System;

namespace GodhomeRandomizer.IC
{
    public class StatueItem : AbstractItem
    {
        public string statueStateName { get; set; }
        public string battleSceneName { get; set; }
        public string position { get; set; }
        public string dependency { get; set; }
        public bool isDreamBoss { get; set; }
        public int pantheonID { get; set; }
        public override void GiveImmediate(GiveInfo info)
        {
            StatueModule module = StatueModule.Instance;
            BossStatue.Completion statueCompletion = PlayerData.instance.GetVariable<BossStatue.Completion>(statueStateName);
            if (!statueCompletion.isUnlocked && module.Settings.RandomizeStatueAccess == AccessMode.Randomized)
            {
                module.UnlockedScenes.Add(battleSceneName);
                module.StatueOverride(statueStateName, battleSceneName);
            }
            else if (!statueCompletion.completedTier1 && module.Settings.RandomizeTiers > TierLimitMode.Vanilla)
            {
                module.AttunedStatues.Add(battleSceneName);
                module.StatueOverride(statueStateName, battleSceneName);
            }
            else if (!statueCompletion.completedTier2 && module.Settings.RandomizeTiers > TierLimitMode.ExcludeAscended)
            {
                module.AscendedStatues.Add(battleSceneName);
                module.StatueOverride(statueStateName, battleSceneName);
            }
            else if (!statueCompletion.completedTier3 && module.Settings.RandomizeTiers > TierLimitMode.ExcludeRadiant)
            {
                module.RadiantStatues.Add(battleSceneName);
                module.StatueOverride(statueStateName, battleSceneName);
            }
            else
            {
                throw new ArgumentException("The item had no effect due to logic inconsistencies.");
            }

            // Display current item obtention
            int current = module.CurrentMarks(battleSceneName);
            int total = module.TotalMarks;
            if (UIDef is MsgUIDef ui && total > 1)
                ui.name = new BoxedString($"{ui.name.Value} ({current} / {total})");
        }
    }
}