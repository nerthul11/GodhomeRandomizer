using ItemChanger;
using ItemChanger.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace GodhomeRandomizer.IC.Shop
{
    public class MixedCostDisplayerSelectionStrategy : ICostDisplayerSelectionStrategy
    {
        public List<IMixedCostSupport> Capabilities { get; set; } = new();

        public CostDisplayer GetCostDisplayer(AbstractItem item)
        {
            Cost c = item.GetTag<CostTag>()?.Cost;
            if (c == null)
            {
                return new GeoCostDisplayer();
            }

            Cost baseCost = c.GetBaseCost();
            if (baseCost is MultiCost mc)
            {
                return FindBestMatchForBaseCosts(mc.Select(cc => cc.GetBaseCost()));
            }
            else
            {
                return FindBestMatchForBaseCosts(baseCost.Yield());
            }
        }

        private CostDisplayer FindBestMatchForBaseCosts(IEnumerable<Cost> costs)
        {
            foreach (Cost c in costs)
            {
                CostDisplayer bestDisplayer = Capabilities
                    .Where(cap => cap.MatchesCost(c))
                    .Select(cap => cap.GetDisplayer(c))
                    .FirstOrDefault();
                if (bestDisplayer != null)
                {
                    return bestDisplayer;
                }
            }
            return new GeoCostDisplayer();
        }
    }
}