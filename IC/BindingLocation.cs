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
            On.BossSequenceBindingsDisplay.CountCompletedBindings -= CompletedBindings;
        }

        protected override void OnLoad()
        {
            On.BossSequenceController.SetupNewSequence += StoreBindings;
            On.BossSequenceController.FinishLastBossScene += GrantItem;
            On.BossSequenceDoor.SetDisplayState += GetDetails;
            On.HeroController.TakeDamage += DisableHitless;
            On.BossSequenceBindingsDisplay.CountCompletedBindings += CompletedBindings;
        }

        private int CompletedBindings(On.BossSequenceBindingsDisplay.orig_CountCompletedBindings orig)
        {
            int bindingsObtained = 0;
            BossSequenceDoor.Completion master = PlayerData.instance.GetVariable<BossSequenceDoor.Completion>($"bossDoorStateTier1");
            BossSequenceDoor.Completion artist = PlayerData.instance.GetVariable<BossSequenceDoor.Completion>($"bossDoorStateTier2");
            BossSequenceDoor.Completion sage = PlayerData.instance.GetVariable<BossSequenceDoor.Completion>($"bossDoorStateTier3");
            BossSequenceDoor.Completion knight = PlayerData.instance.GetVariable<BossSequenceDoor.Completion>($"bossDoorStateTier4");
            BossSequenceDoor.Completion hallownest = PlayerData.instance.GetVariable<BossSequenceDoor.Completion>($"bossDoorStateTier5");
            bindingsObtained += master.boundNail ? 1 : 0;
            bindingsObtained += master.boundShell ? 1 : 0;
            bindingsObtained += master.boundCharms ? 1 : 0;
            bindingsObtained += master.boundSoul ? 1 : 0;
            bindingsObtained += artist.boundNail ? 1 : 0;
            bindingsObtained += artist.boundShell ? 1 : 0;
            bindingsObtained += artist.boundCharms ? 1 : 0;
            bindingsObtained += artist.boundSoul ? 1 : 0;
            bindingsObtained += sage.boundNail ? 1 : 0;
            bindingsObtained += sage.boundShell ? 1 : 0;
            bindingsObtained += sage.boundCharms ? 1 : 0;
            bindingsObtained += sage.boundSoul ? 1 : 0;
            bindingsObtained += knight.boundNail ? 1 : 0;
            bindingsObtained += knight.boundShell ? 1 : 0;
            bindingsObtained += knight.boundCharms ? 1 : 0;
            bindingsObtained += knight.boundSoul ? 1 : 0;
            bindingsObtained += hallownest.boundNail ? 1 : 0;
            bindingsObtained += hallownest.boundShell ? 1 : 0;
            bindingsObtained += hallownest.boundCharms ? 1 : 0;
            bindingsObtained += hallownest.boundSoul ? 1 : 0;
            return bindingsObtained;
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
            GodhomeRandomizer.Instance.ManagePantheonState(pantheonID.ToString(), bindingType, false);
            orig(self);
            GodhomeRandomizer.Instance.ManagePantheonState(pantheonID.ToString(), bindingType, false);
        }

        private void StoreBindings(On.BossSequenceController.orig_SetupNewSequence orig, BossSequence sequence, BossSequenceController.ChallengeBindings bindings, string playerData)
        {
            string activeBindings = bindings.ToString();
            GodhomeManager.SaveSettings.CurrentPantheon = int.Parse(playerData[playerData.Length-1].ToString());
            PantheonBindings settings = GodhomeManager.SaveSettings.CurrentPantheonRun;
            settings.SetVariable("Complete", true);
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