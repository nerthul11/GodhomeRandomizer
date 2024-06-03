using GodhomeRandomizer.IC.Shop;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using Satchel;

namespace GodhomeRandomizer.IC
{
    public class GodhomeShop : CustomShopLocation
    {
        public GodhomeShop()
        {
            name = "Godhome_Shop";
            sceneName = "GG_Unlock_Wastes";
            objectName = "Godseeker Wall NPC";
            fsmName = "npc_control";
            flingType = FlingType.DirectDeposit;
            dungDiscount = false;
            costDisplayerSelectionStrategy = new MixedCostDisplayerSelectionStrategy()
            {
                Capabilities = 
                {
                    new StatueCostSupport()
                }
            };
            tags = [ShopTag()];
        }

        private static InteropTag ShopTag()
        {
            InteropTag tag = new();
            tag.Properties["ModSource"] = "GodhomeRandomizer";
            tag.Properties["PoolGroup"] = "Shops";
            tag.Properties["PinSprite"] = new GodhomeSprite("Ordeal");
            tag.Properties["MapLocations"] = new (string, float, float)[] {("GG_Waterways", 0.0f, 0.3f)};
            tag.Message = "RandoSupplementalMetadata";
            return tag;
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(SceneNames.GG_Waterways, new("Dream Enter", "Control"), ForceWorkshop);
            Events.AddFsmEdit(sceneName, new("Trigger Region", "FSM"), SkipDialogue);
            Events.AddLanguageEdit(new LanguageKey("GG_UNLOCK_TALK"), ShopDialogue);
            Events.AddLanguageEdit(new LanguageKey("GG_UNLOCK_TALK_REPEAT"), ShopDialogue);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(SceneNames.GG_Waterways, new("Dream Enter", "Control"), ForceWorkshop);
            Events.RemoveFsmEdit(sceneName, new("Trigger Region", "FSM"), SkipDialogue);
            Events.RemoveLanguageEdit(new LanguageKey("GG_UNLOCK_TALK"), ShopDialogue);
            Events.RemoveLanguageEdit(new LanguageKey("GG_UNLOCK_TALK_REPEAT"), ShopDialogue);
        }

        private void ShopDialogue(ref string value)
        {
            if (!Placement.AllObtained())
                value = "Empty one, show me your progress and you shall be rewarded.";
            else
                value = "You should abandon this place now.";
        }


        
        private void SkipDialogue(PlayMakerFSM fsm)
        {
            if (!Placement.AllObtained())
            {
                fsm.ChangeTransition("Fade", "FINISHED", "To Atrium");
                // Set the visited status to false
                SetBoolValue wastes = new()
                {
                    boolVariable = PlayerData.instance.seenGGWastes,
                    boolValue = false
                };
                fsm.AddAction("To Atrium", wastes);
            }
        }

        private void ForceWorkshop(PlayMakerFSM fsm)
        {
            if (!Placement.AllObtained())
                fsm.ChangeTransition("Dream Box Down", "FINISHED", "Wastes Cutscene");
        }
    }
}