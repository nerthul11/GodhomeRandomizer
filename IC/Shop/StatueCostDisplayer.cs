﻿using ItemChanger;

namespace GodhomeRandomizer.IC.Shop
{
    public class StatueCostDisplayer : CostDisplayer
    {
        public override ISprite CustomCostSprite { get; set; } = new GodhomeSprite("Ordeal");

        public override bool Cumulative => true;

        protected override bool SupportsCost(Cost cost) => cost is StatueCost;

        protected override int GetSingleCostDisplayAmount(Cost cost) => ((StatueCost)cost).amount;
    }
}