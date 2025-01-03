using GodhomeRandomizer.Settings;
using Newtonsoft.Json;
using RandomizerMod.Logging;

namespace GodhomeRandomizer.Manager
{
    internal static class GodhomeManager
    {
        public static GodhomeRandomizerSettings GlobalSettings => GodhomeRandomizer.Instance.GS;
        public static void Hook()
        {
            LogicHandler.Hook();
            ItemHandler.Hook();
            ConnectionMenu.Hook();
            SettingsLog.AfterLogSettings += AddFileSettings;
        }

        private static void AddFileSettings(LogArguments args, System.IO.TextWriter tw)
        {
            if (!GlobalSettings.Enabled)
                return;

            // Log settings into the settings file
            tw.WriteLine("Godhome Randomizer Settings:");
            using JsonTextWriter jtw = new(tw) { CloseOutput = false };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, GlobalSettings);
            tw.WriteLine();
        }
    }
}