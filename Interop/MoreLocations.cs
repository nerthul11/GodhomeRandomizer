using System;
using GodhomeRandomizer.IC.Shop;
using GodhomeRandomizer.Manager;
using MoreLocations.Rando.Costs;
using RandomizerCore.Logic;

namespace GodhomeRandomizer.Interop
{
    internal static class MoreLocations_Interop
    {
        public static void Hook()
        {
            MoreLocations.Rando.ConnectionInterop.AddRandoCostProviderToJunkShop(CanProvideCosts, CostProvider);
        }
        private static bool CanProvideCosts() => GodhomeManager.GlobalSettings.Enabled && GodhomeManager.GlobalSettings.GodhomeShop.Enabled && GodhomeManager.GlobalSettings.GodhomeShop.IncludeInJunkShop;
        private static StatueCostProvider CostProvider() => new();
    }

    public class StatueCostProvider : ICostProvider
    {
        public bool HasNonFreeCostsAvailable => true;

        public LogicCost Next(LogicManager lm, Random rng)
        {
            int minCost = (int)(176 * GodhomeManager.GlobalSettings.GodhomeShop.MinimumCost);
            int maxCost = (int)(176 * GodhomeManager.GlobalSettings.GodhomeShop.MaximumCost);
            return new StatueLogicCost(lm.GetTermStrict("STATUEMARKS"), rng.Next(minCost, maxCost + 1), amount => new StatueCost(amount));
        }

        public void PreRandomize(Random rng) { }
    }
}