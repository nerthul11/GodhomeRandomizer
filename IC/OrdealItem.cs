using System;
using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;

namespace GodhomeRandomizer.IC
{
    public class OrdealItem : AbstractItem
    {
        public OrdealItem()
        {
            name = "Eternal_Ordeal";
            UIDef = new MsgUIDef {
                name = new BoxedString("Eternal Ordeal"),
                shopDesc = new BoxedString("Obey All Precepts!"),
                sprite = new GodhomeSprite("Ordeal")
            };
            tags = [OrdealTag(), CurseTag()];
        }

        private static InteropTag OrdealTag()
        {
            InteropTag tag = new();
            tag.Properties["ModSource"] = "GodhomeRandomizer";
            tag.Properties["PinSprite"] = new GodhomeSprite("Ordeal");
            tag.Properties["PoolGroup"] = "Statue Marks";
            tag.Message = "RandoSupplementalMetadata";
            return tag;
        }

        private InteropTag CurseTag()
        {
            InteropTag tag = new();
            tag.Properties["CanMimic"] = new BoxedBool(true);
            tag.Properties["MimicNames"] = new string[] {"Zoteeeeee"};
            tag.Message = "CurseData";
            return tag;
        }
        
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.ordealAchieved = true;
        }
    }
}