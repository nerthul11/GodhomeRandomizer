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
        private static bool CanProvideCosts() => GodhomeManager.GlobalSettings.Enabled && GodhomeManager.GlobalSettings.GodhomeShop.IncludeInJunkShop;
        private static StatueCostProvider CostProvider() => new();
    }

    public class StatueCostProvider : ICostProvider
    {
        public bool HasNonFreeCostsAvailable => true;

        public LogicCost Next(LogicManager lm, Random rng)
        {
            int multiplier = (int)GodhomeManager.GlobalSettings.HallOfGods.RandomizeTiers + 1;
            int minCost = (int)(44 * GodhomeManager.GlobalSettings.GodhomeShop.MinimumCost * multiplier);
            int maxCost = (int)(44 * GodhomeManager.GlobalSettings.GodhomeShop.MaximumCost * multiplier);
            return new StatueLogicCost(lm.GetTermStrict("STATUEMARKS"), rng.Next(minCost, maxCost + 1), amount => new StatueCost(amount));
        }

        public void PreRandomize(Random rng) { }
    }
}