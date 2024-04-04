using System;
using GodhomeRandomizer.Settings;
using Newtonsoft.Json;
using RandomizerMod.Logging;

namespace GodhomeRandomizer.Manager
{
    internal static class GodhomeManager
    {
        public static GodhomeRandomizerSettings GlobalSettings => GodhomeRandomizer.Instance.GS.Settings;
        public static LocalSettings SaveSettings => GodhomeRandomizer.Instance.LS;
        public static void Hook()
        {
            ItemHandler.Hook();
            LogicHandler.Hook();
            ConnectionMenu.Hook();
            SettingsLog.AfterLogSettings += AddFileSettings;
        }

        private static void AddFileSettings(LogArguments args, System.IO.TextWriter tw)
        {
            // Log settings into the settings file
            tw.WriteLine("Godhome Randomizer Settings:");
            using JsonTextWriter jtw = new(tw) { CloseOutput = false };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, GlobalSettings);
            tw.WriteLine();

            // Copy GlobalSettings into local to save settings snapshot for game logic use
            GodhomeRandomizerSettings.HOG hogSettings = GlobalSettings.HallOfGods;
            GodhomeRandomizerSettings.Panth panthSettings = GlobalSettings.Pantheons;
            SaveSettings.RandomizeTiers = hogSettings.RandomizeTiers;
            SaveSettings.RandomizeStatueAccess = hogSettings.RandomizeStatueAccess;
            SaveSettings.ApplyAccessToPantheons = panthSettings.ApplyAccessToPantheons;
            SaveSettings.PantheonsIncluded = panthSettings.PantheonsIncluded;
            SaveSettings.Nail = panthSettings.Nail;
            SaveSettings.Shell = panthSettings.Shell;
            SaveSettings.Charms = panthSettings.Charms;
            SaveSettings.Soul = panthSettings.Soul;
            SaveSettings.AllAtOnce = panthSettings.AllAtOnce;
            SaveSettings.Hitless = panthSettings.Hitless;
        }
    }
}