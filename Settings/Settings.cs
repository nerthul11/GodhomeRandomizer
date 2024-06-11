namespace GodhomeRandomizer.Settings
{
    public class GodhomeRandomizerSettings
    {
        public bool Enabled { get; set; } = new();
        public GodhomeShopSettings GodhomeShop { get; set; } = new();
        public HallOfGodsSettings HallOfGods { get; set;} = new();
        public PantheonSettings Pantheons { get; set;} = new();
    }
}