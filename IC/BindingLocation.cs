using GodhomeRandomizer.Modules;
using GodhomeRandomizer.Settings;
using ItemChanger;
using ItemChanger.Locations;

namespace GodhomeRandomizer.IC 
{    
    public class BindingLocation : AutoLocation
    {
        public PantheonLimitMode pantheonID { get; set; }
        public string bindingType { get; set; }
        protected override void OnUnload()
        {
            On.BossSequenceController.FinishLastBossScene -= GrantItem;
            On.BossSequenceDoor.SetDisplayState -= GetDetails;
        }

        protected override void OnLoad()
        {
            On.BossSequenceController.FinishLastBossScene += GrantItem;
            On.BossSequenceDoor.SetDisplayState += GetDetails;
        }

        private void GetDetails(On.BossSequenceDoor.orig_SetDisplayState orig, BossSequenceDoor self, BossSequenceDoor.Completion completion)
        {
            PantheonModule.Instance.ManagePantheonState(pantheonID.ToString(), bindingType, false);
            orig(self, completion);
        }

        private void GrantItem(On.BossSequenceController.orig_FinishLastBossScene orig, BossSceneController self)
        {
            PantheonModule module = PantheonModule.Instance;
            if (module.CurrentPantheonRun.GetVariable<bool>(bindingType) && module.CurrentPantheon == (int)pantheonID)
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
            PantheonModule.Instance.ManagePantheonState(pantheonID.ToString(), bindingType, false);
            orig(self);
            PantheonModule.Instance.ManagePantheonState(pantheonID.ToString(), bindingType, false);
        }
    }
}