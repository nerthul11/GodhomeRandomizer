using ItemChanger;
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
        }
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.ordealAchieved = true;
        }
    }
}