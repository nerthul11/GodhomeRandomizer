using ItemChanger;
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
        }
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.blueRoomActivated = true;
        }
    }
}