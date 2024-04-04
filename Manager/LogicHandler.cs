using GodhomeRandomizer.IC;
using GodhomeRandomizer.Settings;
using Modding;
using Newtonsoft.Json;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerCore.StringItems;
using RandomizerMod.Settings;
using RandomizerMod.RC;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using MenuChanger.MenuElements;


namespace GodhomeRandomizer.Manager
{
    public class LogicHandler
    {
        public static void Hook()
        {
            GodhomeRandomizerSettings settings = GodhomeManager.GlobalSettings;
            if (settings.Enabled)
            {
                RCData.RuntimeLogicOverride.Subscribe(0f, AddHOGLogic);
                RCData.RuntimeLogicOverride.Subscribe(0f, AddBindingLogic);
                if (ModHooks.GetMod("LostArtifacts") is Mod)
                    RCData.RuntimeLogicOverride.Subscribe(1f, EditLostArtifacts);
                if (ModHooks.GetMod("TheRealJournalRando") is Mod)
                    RCData.RuntimeLogicOverride.Subscribe(11f, EditTRJR);
                if (settings.Pantheons.ApplyAccessToPantheons)
                    RCData.RuntimeLogicOverride.Subscribe(1024f, EditPantheonAccessLogic);
            }
        }

        private static void AddHOGLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            JsonLogicFormat fmt = new();
            // Add macros and waypoints.
            lmb.DeserializeFile(LogicFileType.Macros, fmt, typeof(GodhomeRandomizer).Assembly.GetManifestResourceStream($"GodhomeRandomizer.Resources.Logic.macros.json"));
            lmb.DeserializeFile(LogicFileType.Waypoints, fmt, typeof(GodhomeRandomizer).Assembly.GetManifestResourceStream($"GodhomeRandomizer.Resources.Logic.waypoints.json"));

            // Read item definitions
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueItems.json");
            StreamReader itemReader = new(itemStream);
            List<StatueItem> itemList = jsonSerializer.Deserialize<List<StatueItem>>(new JsonTextReader(itemReader));

            GodhomeRandomizerSettings.HOG settings = GodhomeManager.GlobalSettings.HallOfGods;
            int req = settings.RandomizeStatueAccess == AccessMode.Randomized ? 1 : 0;
            foreach (StatueItem item in itemList)
            {
                string boss = item.name.Split('-').Last();
                string position = item.position;
                string dependency = item.dependency;

                // Add term and initialize value
                lmb.GetOrAddTerm($"GG_{boss}", TermType.Int);

                // Add logic items
                lmb.AddItem(new StringItemTemplate($"Statue_Mark-{boss}", $"GG_{boss}++"));

                // Empty mark logic
                lmb.AddLogicDef(new($"Empty_Mark-{boss}", $"{position}_STATUE + GG_{boss}>0"));
                if (dependency is not null)
                    lmb.DoLogicEdit(new($"Empty_Mark-{boss}", $"ORIG + GG_{dependency}>0"));
                if (item.isDreamBoss)
                    lmb.DoLogicEdit(new($"Empty_Mark-{boss}", $"ORIG + DREAMNAIL"));

                // Bronze mark logic
                lmb.AddLogicDef(new($"Bronze_Mark-{boss}", $"{position}_STATUE + Attuned_Combat + GG_{boss}>0 + COMBAT[{boss}]"));
                if (dependency is not null)
                    lmb.DoLogicEdit(new($"Bronze_Mark-{boss}", $"ORIG + GG_{dependency}>0"));
                if (item.isDreamBoss)
                    lmb.DoLogicEdit(new($"Bronze_Mark-{boss}", $"ORIG + DREAMNAIL"));
                
                if (settings.RandomizeStatueAccess == AccessMode.Vanilla)
                {
                    lmb.DoLogicEdit(new($"Bronze_Mark-{boss}", $"{position}_STATUE + Attuned_Combat + Defeated_{boss}"));
                    if (dependency is not null)
                        lmb.DoLogicEdit(new($"Bronze_Mark-{boss}", $"ORIG + Defeated_{dependency}"));
                }

                // Silver mark logic
                lmb.AddLogicDef(new($"Silver_Mark-{boss}", $"{position}_STATUE + Ascended_Combat + GG_{boss}>0 + COMBAT[{boss}]"));
                if (dependency is not null)
                    lmb.DoLogicEdit(new($"Silver_Mark-{boss}", $"ORIG + GG_{dependency}>0"));
                if (item.isDreamBoss)
                    lmb.DoLogicEdit(new($"Silver_Mark-{boss}", $"ORIG + DREAMNAIL"));
                
                if (settings.RandomizeStatueAccess == AccessMode.Vanilla)
                {
                    lmb.DoLogicEdit(new($"Silver_Mark-{boss}", $"{position}_STATUE + Ascended_Combat + Defeated_{boss}"));
                    if (dependency is not null)
                        lmb.DoLogicEdit(new($"Silver_Mark-{boss}", $"ORIG + Defeated_{dependency}"));                        
                }

                // Gold mark logic
                lmb.AddLogicDef(new($"Gold_Mark-{boss}", $"{position}_STATUE + Radiant_Combat + GG_{boss}>{1 + req} + COMBAT[{boss}]"));
                if (dependency is not null)
                    lmb.DoLogicEdit(new($"Gold_Mark-{boss}", $"ORIG + GG_{dependency}>0"));
                if (item.isDreamBoss)
                    lmb.DoLogicEdit(new($"Gold_Mark-{boss}", $"ORIG + DREAMNAIL"));
                
                if (settings.RandomizeStatueAccess == AccessMode.Vanilla)
                {
                    lmb.DoLogicEdit(new($"Gold_Mark-{boss}", $"{position}_STATUE + Radiant_Combat + GG_{boss}>{1 + req} + Defeated_{boss}"));
                    if (dependency is not null)
                        lmb.DoLogicEdit(new($"Gold_Mark-{boss}", $"ORIG + Defeated_{dependency}"));
                }
            }
        }

        private static void AddBindingLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            // Read item definitions
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.BindingItems.json");
            StreamReader itemReader = new(itemStream);
            List<BindingItem> itemList = jsonSerializer.Deserialize<List<BindingItem>>(new JsonTextReader(itemReader));
            lmb.GetOrAddTerm($"Pantheon_Bindings", TermType.Int);
            foreach (BindingItem item in itemList)
            {
                lmb.AddItem(new StringItemTemplate(item.name, "Pantheon_Bindings++"));
                lmb.AddLogicDef(new(item.name, $"Defeated_Pantheon_{(int)item.pantheonID}"));
                if (item.bindingType == "Hitless" || item.bindingType == "AllAtOnce")
                {
                    lmb.DoLogicEdit(new(item.name, "ORIG + Radiant_Combat"));
                }
                else
                {
                    lmb.DoLogicEdit(new(item.name, "ORIG + Ascended_Combat"));
                }
            }
        }

        private static void EditPantheonAccessLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueItems.json");
            StreamReader itemReader = new(itemStream);
            List<StatueItem> itemList = jsonSerializer.Deserialize<List<StatueItem>>(new JsonTextReader(itemReader));
            itemList = itemList.Where(item => !item.pantheonID.Equals(null)).ToList();
            GodhomeRandomizerSettings.HOG settings = GodhomeManager.GlobalSettings.HallOfGods;   

            if (settings.RandomizeStatueAccess == AccessMode.AllUnlocked)
            {
                lmb.DoLogicEdit(new("Defeated_Pantheon_1", "GG_Atrium + PANTHEON_KEY_1 ? TRUE + COMBAT[Pantheon_1]"));
                lmb.DoLogicEdit(new("Defeated_Pantheon_2", "GG_Atrium + PANTHEON_KEY_2 ? TRUE + COMBAT[Pantheon_2]"));
                lmb.DoLogicEdit(new("Defeated_Pantheon_3", "GG_Atrium + PANTHEON_KEY_3 ? TRUE + COMBAT[Pantheon_3]"));
                lmb.DoLogicEdit(new("Defeated_Pantheon_4", "GG_Atrium + PANTHEON_KEY_4 ? TRUE + COMBAT[Pantheon_4]"));
            }
            if (settings.RandomizeStatueAccess == AccessMode.Randomized)
            {
                string bossLogic = "";
                foreach (StatueItem item in itemList)
                {
                    if (item.pantheonID == 1)
                    {
                        string boss = item.name.Split('-').Last();
                        if (bossLogic == "")
                            bossLogic += $"GG_{boss}>0";
                        else
                            bossLogic += $" + GG_{boss}>0";
                    }
                }
                lmb.DoLogicEdit(new("Defeated_Pantheon_1", $"GG_Atrium + (({bossLogic}) | (PANTHEON_KEY_1 ? FALSE)) + COMBAT[Pantheon_1]"));
                
                bossLogic = "";
                foreach (StatueItem item in itemList)
                {
                    if (item.pantheonID == 2)
                    {
                        string boss = item.name.Split('-').Last();
                        if (bossLogic == "")
                            bossLogic += $"GG_{boss}>0";
                        else
                            bossLogic += $" + GG_{boss}>0";
                    }
                }
                lmb.DoLogicEdit(new("Defeated_Pantheon_2", $"GG_Atrium + (({bossLogic}) | (PANTHEON_KEY_2 ? FALSE)) + COMBAT[Pantheon_2]"));
                
                bossLogic = "";
                foreach (StatueItem item in itemList)
                {
                    if (item.pantheonID == 3)
                    {
                        string boss = item.name.Split('-').Last();
                        if (bossLogic == "")
                            bossLogic += $"GG_{boss}>0";
                        else
                            bossLogic += $" + GG_{boss}>0";
                    }
                }
                lmb.DoLogicEdit(new("Defeated_Pantheon_3", $"GG_Atrium + (({bossLogic}) | (PANTHEON_KEY_3 ? FALSE)) + COMBAT[Pantheon_3]"));
                
                bossLogic = "";
                foreach (StatueItem item in itemList)
                {
                    if (item.pantheonID == 4)
                    {
                        string boss = item.name.Split('-').Last();
                        if (bossLogic == "")
                            bossLogic += $"GG_{boss}>0";
                        else
                            bossLogic += $" + GG_{boss}>0";
                    }
                }
                lmb.DoLogicEdit(new("Defeated_Pantheon_4", $"GG_Atrium + Opened_Pantheon_4 + (({bossLogic}) | (PANTHEON_KEY_4 ? FALSE)) + COMBAT[Pantheon_4]"));
            }
        }
        private static void EditTRJR(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            // Read item definitions
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueItems.json");
            StreamReader itemReader = new(itemStream);
            List<StatueItem> itemList = jsonSerializer.Deserialize<List<StatueItem>>(new JsonTextReader(itemReader));

            GodhomeRandomizerSettings.HOG hogSettings = GodhomeManager.GlobalSettings.HallOfGods;
            int req = hogSettings.RandomizeStatueAccess == AccessMode.Randomized ? 1 : 0;

            // For boss journal entries, add HOG clearance as an option if access is altered
            if (hogSettings.RandomizeStatueAccess > AccessMode.Vanilla)
            {
                string[] entries = [
                    "Gruz_Mother", "Vengefly_King", "Brooding_Mawlek", "False_Knight",
                    "Massive_Moss_Charger", "Mantis_Lords", "Grey_Prince_Zote", "Broken_Vessel",
                    "Nosk", "Flukemarm", "Soul_Master", "God_Tamer", "Crystal_Guardian",
                    "Uumuu", "Dung_Defender", "White_Defender", "Hive_Knight",
                    "Traitor_Lord", "Grimm", "Nightmare_King", "Pure_Vessel",
                    "Elder_Hu", "Galien", "Markoth", "Marmu", "No_Eyes", "Xero", "Gorb",
                    "Radiance", "Soul_Warrior", "Paintmaster_Sheo"
                ];
                foreach (string entry in entries)
                {
                    lmb.DoLogicEdit(new($"Defeated_Any_{entry}", $"ORIG | *Bronze_Mark-{entry}"));
                }
                lmb.DoLogicEdit(new("Defeated_Any_Hornet", "ORIG | *Bronze_Mark-Hornet_1"));
                lmb.DoLogicEdit(new("Defeated_Any_Oblobble", "ORIG | *Bronze_Mark-Oblobbles"));
                lmb.DoLogicEdit(new("Defeated_Any_The_Collector", "ORIG | *Bronze_Mark-Collector"));
                lmb.DoLogicEdit(new("Defeated_Any_Watcher_Knight", "ORIG | *Bronze_Mark-Watcher_Knights"));
                lmb.DoLogicEdit(new("Defeated_Any_Great_Nailsage_Sly", "ORIG | *Bronze_Mark-Sly"));
                lmb.DoLogicEdit(new("Defeated_Any_Nailmasters_Oro_And_Mato", "ORIG | *Bronze_Mark-Nailmaster_Brothers"));
            }

            // Add HOG mark requirements to Void Idol logic.
            if (hogSettings.RandomizeTiers > TierLimitMode.Vanilla)
            {
                string bossLogic = "GG_Workshop";
                foreach (StatueItem item in itemList)
                {
                    string boss = item.name.Split('-').Last();
                    bossLogic += $" + GG_{boss}>{req}";
                }
                lmb.DoMacroEdit(new("ATTUNED_IDOL", bossLogic));
                lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_1", "ATTUNED_IDOL"));
            }

            if (hogSettings.RandomizeTiers > TierLimitMode.ExcludeAscended)
            {
                string bossLogic = "GG_Workshop";
                foreach (StatueItem item in itemList)
                {
                    string boss = item.name.Split('-').Last();
                    bossLogic += $" + GG_{boss}>{req + 1}";
                }
                lmb.DoMacroEdit(new("ASCENDED_IDOL", bossLogic));
                lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_2", "ASCENDED_IDOL"));                
            }
            if (hogSettings.RandomizeTiers == TierLimitMode.IncludeAll)
            {
                string bossLogic = "GG_Workshop";
                foreach (StatueItem item in itemList)
                {
                    string boss = item.name.Split('-').Last();
                    bossLogic += $" + GG_{boss}>{req + 2}";
                }
                lmb.DoMacroEdit(new("RADIANT_IDOL", bossLogic));
                lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_3", "RADIANT_IDOL"));
            }

            // Add bindings requirement to Weathered Mask entry
            GodhomeRandomizerSettings.Panth panthSettings = GodhomeRandomizer.Instance.GS.Settings.Pantheons;
            if (panthSettings.PantheonsIncluded > PantheonLimitMode.None)
            {
                int multiplier = (int)panthSettings.PantheonsIncluded;
                int bindCount = (panthSettings.Nail ? 1 : 0) + (panthSettings.Shell ? 1 : 0) + (panthSettings.Charms ? 1 : 0) + (panthSettings.Soul ? 1 : 0);
                if (multiplier * bindCount > 0)
                    lmb.DoLogicEdit(new("Journal_Entry-Weathered_Mask", $"ORIG + Pantheon_Bindings > {(multiplier * bindCount) - 1}"));
            }
        }

        private static void EditLostArtifacts(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            // Read item definitions
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueItems.json");
            StreamReader itemReader = new(itemStream);
            List<StatueItem> itemList = jsonSerializer.Deserialize<List<StatueItem>>(new JsonTextReader(itemReader));

            GodhomeRandomizerSettings.HOG settings = GodhomeManager.GlobalSettings.HallOfGods;
            int req = settings.RandomizeStatueAccess == AccessMode.Randomized ? 1 : 0;

            if (settings.RandomizeTiers > TierLimitMode.Vanilla)
            {
                string logic = "GG_Workshop";
                foreach (StatueItem item in itemList)
                {
                    string boss = item.name.Split('-').Last();
                    logic += $" + GG_{boss}>{req}";
                }
                lmb.DoMacroEdit(new("ATTUNED_IDOL", logic));
                lmb.DoLogicEdit(new("AttunedJewel", "ATTUNED_IDOL"));
            }
        }
    }
}