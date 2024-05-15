using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;

namespace GodhomeRandomizer.IC
{
    public class LifebloodItem : AbstractItem
    {
        public LifebloodItem()
        {
            name = "Godhome_Lifeblood";
            UIDef = new MsgUIDef {
                name = new BoxedString("Godhome Lifeblood"),
                shopDesc = new BoxedString("It sparks with the lifeblood of attuned beings."),
                sprite = new GodhomeSprite("Lifeblood")
            };
            tags = [LifebloodTag()];
        }

        private static InteropTag LifebloodTag()
        {
            InteropTag tag = new();
            tag.Properties["ModSource"] = "GodhomeRandomizer";
            tag.Properties["PinSprite"] = new GodhomeSprite("Lifeblood");
            tag.Properties["PoolGroup"] = "Lifeblood Cocoons";
            tag.Message = "RandoSupplementalMetadata";
            return tag;
        }
        
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.blueRoomActivated = true;
        }
    }
}