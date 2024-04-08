using ItemChanger;

namespace GodhomeRandomizer.IC
{
    public class LifebloodItem : AbstractItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.blueRoomActivated = true;
        }
    }
}