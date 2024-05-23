using RandomizerCore.Logic;
using System;

namespace GodhomeRandomizer.IC.Shop
{
    public interface ICostProvider
    {
        bool HasNonFreeCostsAvailable { get; }

        LogicCost Next(LogicManager lm, Random rng);

        void PreRandomize(Random rng);
    }
}