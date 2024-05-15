using GodhomeRandomizer.Modules;
using GodhomeRandomizer.Settings;
using ItemChanger;
using ItemChanger.Tags;

namespace GodhomeRandomizer.IC
{
    public class BindingItem : AbstractItem
    {
        public PantheonLimitMode pantheonID { get; set; }
        public string bindingType { get; set; }
        public override void GiveImmediate(GiveInfo info)
        {
            PantheonModule.Instance.ManagePantheonState(pantheonID.ToString(), bindingType, true);
        }
    }
}