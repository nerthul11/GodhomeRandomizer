using System.Linq;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Util;
using Satchel;

namespace GodhomeRandomizer.IC 
{    
    public class LifebloodLocation : AutoLocation
    {

        protected override void OnUnload()
        {
            On.DeactivateIfPlayerdataTrue.OnEnable -= ForceCocoon;
            Events.RemoveFsmEdit(SceneNames.GG_Blue_Room, new("gg_blue_core", "Dream React"), GrantItem);
        }

        protected override void OnLoad()
        {
            On.DeactivateIfPlayerdataTrue.OnEnable += ForceCocoon;
            Events.AddFsmEdit(SceneNames.GG_Blue_Room, new("gg_blue_core", "Dream React"), GrantItem);
        }

        private void ForceCocoon(On.DeactivateIfPlayerdataTrue.orig_OnEnable orig, DeactivateIfPlayerdataTrue self)
        {
            if (!Placement.AllObtained())
                return;
            orig(self);
        }

        private void GrantItem(PlayMakerFSM fsm)
        {
            if (Placement.Items.All(x => x.IsObtained()))
            {
                fsm.SetState("Disable");
            }
            fsm.AddState("GiveItem");
            fsm.AddCustomAction("GiveItem", () => {
                ItemUtility.GiveSequentially(Placement.Items, Placement, new GiveInfo()
                {
                    FlingType = FlingType.DirectDeposit,
                    MessageType = MessageType.Corner,
                });
            });
            fsm.RemoveAction("Play Anim", 1);
            fsm.ChangeTransition("Regain Control", "FINISHED", "GiveItem");
            fsm.AddTransition("GiveItem", "FINISHED", "Disable");
        }
    }
}