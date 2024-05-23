using ItemChanger;
using GodhomeRandomizer.Modules;

namespace GodhomeRandomizer.IC.Shop
{

    public sealed record StatueCost(int amount) : Cost
    {
        public override bool CanPay() => StatueModule.Instance.TotalCount() >= amount;
        public override void OnPay() { }
        public override bool HasPayEffects() => false;
        public override string GetCostText() => $"{amount} Statue Marks";
    }
}