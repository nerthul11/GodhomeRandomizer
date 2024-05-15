using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using ItemChanger.Util;
using Satchel;

namespace GodhomeRandomizer.IC 
{    
    public class OrdealLocation : AutoLocation
    {
        public OrdealLocation()
        {
            name = "Eternal_Ordeal";
            sceneName = "GG_Workshop";
            flingType = FlingType.DirectDeposit;
            tags = [OrdealTag()];
        }

        private static InteropTag OrdealTag()
        {
            InteropTag tag = new();
            tag.Properties["ModSource"] = "GodhomeRandomizer";
            tag.Properties["PoolGroup"] = "Statue Marks";
            tag.Properties["PinSprite"] = new GodhomeSprite("Ordeal");
            tag.Properties["VanillaItem"] = "Eternal_Ordeal";
            tag.Properties["MapLocations"] = new (string, float, float)[] {("GG_Waterways", 0.6f, 0.3f)};
            tag.Message = "RandoSupplementalMetadata";
            return tag;
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit("GG_Mighty_Zote", new("Battle Control", "Control"), GrantItem);
        }

        protected override void OnLoad()
        {
            Events.AddFsmEdit("GG_Mighty_Zote", new("Battle Control", "Control"), GrantItem);
        }

        private void GrantItem(PlayMakerFSM fsm)
        {
            fsm.AddState("GiveItem");
            fsm.AddCustomAction("GiveItem", () => {
                ItemUtility.GiveSequentially(Placement.Items, Placement, new GiveInfo()
                {
                    FlingType = FlingType.DirectDeposit,
                    MessageType = MessageType.Corner,
                });
            });
            fsm.ChangeTransition("Add Zoteling 2", "NEXT", "GiveItem");
            fsm.AddTransition("GiveItem", "FINISHED", PlayerData.instance.ordealAchieved ? "Menu Unlock" : "Add Warrior 3");
        }
    }
}