using GodhomeRandomizer.Manager;
using GodhomeRandomizer.Settings;
using ItemChanger;
using System;

namespace GodhomeRandomizer.IC
{
    public class StatueItem : AbstractItem
    {
        public string statueStateName { get; set; }
        public string position { get; set; }
        public string dependency { get; set; }
        public bool isDreamBoss { get; set; }
        public int pantheonID { get; set; }
        public override void GiveImmediate(GiveInfo info)
        {
            BossStatue.Completion statueCompletion = PlayerData.instance.GetVariable<BossStatue.Completion>(statueStateName);
            if (!statueCompletion.isUnlocked && GodhomeManager.SaveSettings.RandomizeStatueAccess == AccessMode.Randomized)
            {
                GodhomeRandomizer.Instance.ManageStatueState(statueStateName, "isUnlocked", true);
            }
            else if (!statueCompletion.completedTier1 && GodhomeManager.SaveSettings.RandomizeTiers > TierLimitMode.Vanilla)
            {
                GodhomeRandomizer.Instance.ManageStatueState(statueStateName, "completedTier1", true);
            }
            else if (!statueCompletion.completedTier2 && GodhomeManager.SaveSettings.RandomizeTiers > TierLimitMode.ExcludeAscended)
            {
                GodhomeRandomizer.Instance.ManageStatueState(statueStateName, "completedTier2", true);
            }
            else if (!statueCompletion.completedTier3 && GodhomeManager.SaveSettings.RandomizeTiers > TierLimitMode.ExcludeRadiant)
            {
                GodhomeRandomizer.Instance.ManageStatueState(statueStateName, "completedTier3", true);
            }
            else
            {
                throw new ArgumentException("The item had no effect due to logic inconsistencies.");
            }
        }
    }
}