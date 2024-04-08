using GodhomeRandomizer.Settings;
using ItemChanger;

namespace GodhomeRandomizer.IC
{
    public class BindingItem : AbstractItem
    {
        public PantheonLimitMode pantheonID { get; set; }
        public string bindingType { get; set; }
        public override void GiveImmediate(GiveInfo info)
        {
            GodhomeRandomizer.Instance.ManagePantheonState(pantheonID.ToString(), bindingType, true);
        }
    }
}