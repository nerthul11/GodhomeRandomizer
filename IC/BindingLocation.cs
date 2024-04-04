using System;
using GlobalEnums;
using GodhomeRandomizer.Manager;
using GodhomeRandomizer.Settings;
using ItemChanger;
using ItemChanger.Locations;
using UnityEngine;

namespace GodhomeRandomizer.IC 
{    
    public class BindingLocation : AutoLocation
    {
        public PantheonLimitMode pantheonID { get; set; }
        public string bindingType { get; set; }

        protected override void OnUnload()
        {
            On.BossSequenceController.SetupNewSequence -= StoreBindings;
            On.BossSequenceController.FinishLastBossScene -= GrantItem;
            On.BossSequenceDoor.SetDisplayState -= GetDetails;
            On.HeroController.TakeDamage -= DisableHitless;
        }

        protected override void OnLoad()
        {
            On.BossSequenceController.SetupNewSequence += StoreBindings;
            On.BossSequenceController.FinishLastBossScene += GrantItem;
            On.BossSequenceDoor.SetDisplayState += GetDetails;
            On.HeroController.TakeDamage += DisableHitless;
        }

        private void DisableHitless(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            PantheonBindings settings = GodhomeManager.SaveSettings.CurrentPantheonRun;
            settings.SetVariable("Hitless", false);
            orig(self, go, damageSide, damageAmount, hazardType);
        }

        private void GetDetails(On.BossSequenceDoor.orig_SetDisplayState orig, BossSequenceDoor self, BossSequenceDoor.Completion completion)
        {
            GodhomeRandomizer.Instance.ManagePantheonState(pantheonID.ToString(), bindingType, false);
            orig(self, completion);
        }

        private void GrantItem(On.BossSequenceController.orig_FinishLastBossScene orig, BossSceneController self)
        {
            LocalSettings settings = GodhomeManager.SaveSettings;
            if (settings.CurrentPantheonRun.GetVariable<bool>(bindingType) && settings.CurrentPantheon == (int)pantheonID)
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
            }
            orig(self);
        }

        private void StoreBindings(On.BossSequenceController.orig_SetupNewSequence orig, BossSequence sequence, BossSequenceController.ChallengeBindings bindings, string playerData)
        {
            string activeBindings = bindings.ToString();
            GodhomeManager.SaveSettings.CurrentPantheon = int.Parse(playerData[playerData.Length-1].ToString());
            PantheonBindings settings = GodhomeManager.SaveSettings.CurrentPantheonRun;
            settings.SetVariable("Nail", activeBindings.Contains("Nail"));
            settings.SetVariable("Shell", activeBindings.Contains("Shell"));
            settings.SetVariable("Charms", activeBindings.Contains("Charms"));
            settings.SetVariable("Soul", activeBindings.Contains("Soul"));
            settings.SetVariable("Hitless", true);
            settings.SetVariable("AllAtOnce", settings.Nail && settings.Shell && settings.Charms && settings.Soul);
            orig(sequence, bindings, playerData);
        }
    }
}