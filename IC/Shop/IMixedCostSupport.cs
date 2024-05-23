using ItemChanger;

namespace GodhomeRandomizer.IC.Shop
{
    public interface IMixedCostSupport
    {
        /// <summary>
        /// Whether a given unwrapped cost is matched by this.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool MatchesCost(Cost c);
        /// <summary>
        /// Gets a cost displayer for a given matching cost
        /// </summary>
        public CostDisplayer GetDisplayer(Cost c);
    }
}