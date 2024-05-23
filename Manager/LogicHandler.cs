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
using System;


namespace GodhomeRandomizer.Manager
{
    public class LogicHandler
    {
        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(1f, AddHOGLogic);
            if (ModHooks.GetMod("LostArtifacts") is Mod)
                RCData.RuntimeLogicOverride.Subscribe(2f, EditLostArtifacts);
            if (ModHooks.GetMod("TheRealJournalRando") is Mod)
                RCData.RuntimeLogicOverride.Subscribe(11f, EditTRJR);
            if (ModHooks.GetMod("RandoPlus") is Mod)
                RCData.RuntimeLogicOverride.Subscribe(51f, EditRandoPlus);
            // Pantheon logic to be added after ExtraRando
            RCData.RuntimeLogicOverride.Subscribe(2048f, AddPantheonLogic);
            RCData.RuntimeLogicOverride.Subscribe(4096f, EditPantheonAccessLogic);
        }

        private static void AddHOGLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;

            // Add macros and waypoints.
            JsonLogicFormat fmt = new();
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
            lmb.GetOrAddTerm("STATUEMARKS", TermType.Int);
            foreach (StatueItem item in itemList)
            {
                string boss = item.name.Split('-').Last();
                string position = item.position;
                string dependency = item.dependency;

                // Add term and initialize value
                lmb.GetOrAddTerm($"GG_{boss}", TermType.Int);

                // Add logic items
                lmb.AddItem(new StringItemTemplate($"Statue_Mark-{boss}", $"STATUEMARKS++ >> GG_{boss}++"));

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
                if (settings.RandomizeStatueAccess == AccessMode.AllUnlocked)
                {
                    lmb.DoLogicEdit(new($"Bronze_Mark-{boss}", $"{position}_STATUE + Attuned_Combat + COMBAT[{boss}]"));
                    if (item.isDreamBoss)
                        lmb.DoLogicEdit(new($"Bronze_Mark-{boss}", $"ORIG + DREAMNAIL"));
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
                if (settings.RandomizeStatueAccess == AccessMode.AllUnlocked)
                {
                    lmb.DoLogicEdit(new($"Silver_Mark-{boss}", $"{position}_STATUE + Ascended_Combat + COMBAT[{boss}]"));
                    if (item.isDreamBoss)
                        lmb.DoLogicEdit(new($"Silver_Mark-{boss}", $"ORIG + DREAMNAIL"));
                }

                // Gold mark logic
                lmb.AddLogicDef(new($"Gold_Mark-{boss}", $"{position}_STATUE + Radiant_Combat + GG_{boss}>{1 + req} + COMBAT[{boss}]"));
                if (dependency is not null && settings.RandomizeStatueAccess < AccessMode.AllUnlocked)
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

            // Eternal Ordeal logic
            lmb.AddItem(new EmptyItem("Eternal_Ordeal"));
            lmb.AddLogicDef(new("Eternal_Ordeal", "GG_Workshop + (BOSS | SPICYCOMBATSKIPS) + UPWALLBREAK + RIGHTCLAW + WINGS + (LEFTSUPERDASH | LEFTSHARPSHADOW)"));
        }

        private static void AddPantheonLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
                      
            // Add logic term
            GodhomeRandomizerSettings.Panth settings = GodhomeManager.GlobalSettings.Pantheons;
            lmb.GetOrAddTerm($"Pantheon_Bindings", TermType.Int);
            if (settings.Completion)
            {
                if (settings.PantheonsIncluded >= PantheonLimitMode.Master)
                    lmb.AddItem(new BoolItem("Pantheon_Master-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_1")));
                if (settings.PantheonsIncluded >= PantheonLimitMode.Artist)
                    lmb.AddItem(new BoolItem("Pantheon_Artist-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_2")));
                if (settings.PantheonsIncluded >= PantheonLimitMode.Sage)
                    lmb.AddItem(new BoolItem("Pantheon_Sage-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_3")));
                if (settings.PantheonsIncluded >= PantheonLimitMode.Knight)
                    lmb.AddItem(new BoolItem("Pantheon_Knight-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_4")));
                if (settings.PantheonsIncluded >= PantheonLimitMode.Hallownest)
                    lmb.AddItem(new BoolItem("Pantheon_Hallownest-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_5")));
            }

            // Read item definitions
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.BindingItems.json");
            StreamReader itemReader = new(itemStream);
            List<BindingItem> itemList = jsonSerializer.Deserialize<List<BindingItem>>(new JsonTextReader(itemReader));
            
            foreach (BindingItem item in itemList)
            { 
                lmb.AddLogicDef(new(item.name, $"Defeated_Pantheon_{(int)item.pantheonID}"));
                if (item.bindingType == "Hitless" || item.bindingType == "AllAtOnce")
                {
                    lmb.AddItem(new EmptyItem(item.name));
                    lmb.DoLogicEdit(new(item.name, "ORIG + Hardcore_Combat"));
                }
                else if (item.bindingType != "Completion")
                {
                    lmb.AddItem(new StringItemTemplate(item.name, "Pantheon_Bindings++"));
                    lmb.DoLogicEdit(new(item.name, "ORIG + Radiant_Combat"));
                }
            }

            // Add lifeblood logic
            lmb.AddItem(new EmptyItem("Godhome_Lifeblood"));
            lmb.AddLogicDef(new("Godhome_Lifeblood", "GG_Blue_Room + DREAMNAIL"));
            
            int multiplier = (int)settings.PantheonsIncluded;
            int bindCount = (settings.Nail ? 1 : 0) + (settings.Shell ? 1 : 0) + (settings.Charms ? 1 : 0) + (settings.Soul ? 1 : 0);
            if (multiplier * bindCount > 12)
            {
                lmb.DoLogicEdit(new("GG_Blue_Room", $"ORIG + Pantheon_Bindings>{multiplier * bindCount - 13}"));
            }
        }

        private static void EditPantheonAccessLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled || !GodhomeManager.GlobalSettings.Pantheons.ApplyAccessToPantheons)
                return;
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueItems.json");
            StreamReader itemReader = new(itemStream);
            List<StatueItem> itemList = jsonSerializer.Deserialize<List<StatueItem>>(new JsonTextReader(itemReader));
            itemList = itemList.Where(item => !item.pantheonID.Equals(null)).ToList();
            GodhomeRandomizerSettings.HOG settings = GodhomeManager.GlobalSettings.HallOfGods;   

            if (settings.RandomizeStatueAccess == AccessMode.AllUnlocked)
            {
                lmb.DoLogicEdit(new("Opened_Pantheon_1", "GG_Atrium + PANTHEON_KEY_1 ? TRUE"));
                lmb.DoLogicEdit(new("Opened_Pantheon_2", "GG_Atrium + PANTHEON_KEY_2 ? TRUE"));
                lmb.DoLogicEdit(new("Opened_Pantheon_3", "GG_Atrium + PANTHEON_KEY_3 ? TRUE"));
                lmb.DoLogicEdit(new("Opened_Pantheon_4", "GG_Atrium + (PANTHEON_COMPLETION_1 ? Defeated_Pantheon_1 + PANTHEON_COMPLETION_2 ? Defeated_Pantheon_2 + PANTHEON_COMPLETION_3 ? Defeated_Pantheon_3 | (PANTHEON_KEY_4 ? FALSE))"));
            }
            if (settings.RandomizeStatueAccess == AccessMode.Randomized)
            {
                foreach (int panthID in Enumerable.Range(1, 4))
                {
                    string bossLogic = "";
                    foreach (StatueItem item in itemList)
                    {
                        if (item.pantheonID == panthID)
                        {
                            string boss = item.name.Split('-').Last();
                            if (bossLogic == "")
                                bossLogic += $"GG_{boss}>0";
                            else
                                bossLogic += $" + GG_{boss}>0";
                        }
                    }
                    if (panthID < 4)
                        lmb.DoLogicEdit(new($"Opened_Pantheon_{panthID}", $"GG_Atrium + ({bossLogic} | (PANTHEON_KEY_{panthID} ? FALSE))"));
                    else
                        lmb.DoLogicEdit(new($"Opened_Pantheon_{panthID}", $"GG_Atrium + (PANTHEON_COMPLETION_1 ? Defeated_Pantheon_1 + PANTHEON_COMPLETION_2 ? Defeated_Pantheon_2 + PANTHEON_COMPLETION_3 ? Defeated_Pantheon_3 + {bossLogic} | (PANTHEON_KEY_{panthID} ? FALSE))"));
                }
            }
        }
        private static void EditTRJR(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
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
                    "Nosk", "Flukemarm", "Soul_Master", "God_Tamer",
                    "Uumuu", "Dung_Defender", "White_Defender", "Hive_Knight",
                    "Traitor_Lord", "Grimm", "Pure_Vessel",
                    "Elder_Hu", "Galien", "Markoth", "Marmu", "No_Eyes", "Xero", "Gorb",
                    "Radiance", "Soul_Warrior", "Paintmaster_Sheo"
                ];
                bool regularEntriesIncluded = lmb.LogicLookup.TryGetValue("Defeated_Any_Folly", out _);
                bool bonusEntriesIncluded = lmb.LogicLookup.TryGetValue("Defeated_Any_Volatile_Zoteling", out _);
                bool regularBossIncluded = lmb.LogicLookup.TryGetValue("Defeated_Any_Gruz_Mother", out _);
                bool pantheonBossIncluded = lmb.LogicLookup.TryGetValue("Defeated_Any_Pure_Vessel", out _);
                foreach (string entry in entries)
                {
                    bool entryIncluded = lmb.LogicLookup.TryGetValue($"Defeated_Any_{entry}", out _);
                    if (entryIncluded)
                        lmb.DoLogicEdit(new($"Defeated_Any_{entry}", $"ORIG | *Bronze_Mark-{entry}"));
                }
                if (regularEntriesIncluded)
                {
                    lmb.DoLogicEdit(new("Defeated_Any_Vengefly", "ORIG | *Bronze_Mark-Collector"));
                    lmb.DoLogicEdit(new("Defeated_Any_Baldur", "ORIG | *Bronze_Mark-Collector"));
                    lmb.DoLogicEdit(new("Defeated_Any_Aspid_Hunter", "ORIG | *Bronze_Mark-Collector"));
                    lmb.DoLogicEdit(new("Defeated_Any_Armoured_Squit", "ORIG | *Silver_Mark-Collector"));
                    lmb.DoLogicEdit(new("Defeated_Any_Sharp_Baldur", "ORIG | *Silver_Mark-Collector"));
                    lmb.DoLogicEdit(new("Defeated_Any_Primal_Aspid", "ORIG | *Silver_Mark-Collector"));
                    lmb.DoLogicEdit(new("Defeated_Any_Folly", "ORIG | *Silver_Mark-Soul_Warrior"));
                }
                if (bonusEntriesIncluded)
                {
                    lmb.DoLogicEdit(new("Defeated_Any_Winged_Zoteling", "ORIG | *Bronze_Mark-Grey_Prince_Zote"));
                    lmb.DoLogicEdit(new("Defeated_Any_Hopping_Zoteling", "ORIG | *Bronze_Mark-Grey_Prince_Zote"));
                    lmb.DoLogicEdit(new("Defeated_Any_Volatile_Zoteling", "ORIG | *Bronze_Mark-Grey_Prince_Zote"));
                }
                if (regularBossIncluded)
                {
                    lmb.DoLogicEdit(new("Defeated_Any_Hornet", "ORIG | *Bronze_Mark-Hornet_1"));
                    lmb.DoLogicEdit(new("Defeated_Any_Oblobble", "ORIG | *Bronze_Mark-Oblobbles"));
                    lmb.DoLogicEdit(new("Defeated_Any_The_Collector", "ORIG | *Bronze_Mark-Collector"));
                    lmb.DoLogicEdit(new("Defeated_Any_Watcher_Knight", "ORIG | *Bronze_Mark-Watcher_Knights"));
                }
                if (pantheonBossIncluded)
                {
                    lmb.DoLogicEdit(new("Defeated_Any_Great_Nailsage_Sly", "ORIG | *Bronze_Mark-Sly"));
                    lmb.DoLogicEdit(new("Defeated_Any_Nailmasters_Oro_And_Mato", "ORIG | *Bronze_Mark-Nailmaster_Brothers"));
                }
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
                lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_2", "ATTUNED_IDOL"));
                lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_3", "ATTUNED_IDOL"));     
            
                if (hogSettings.RandomizeTiers > TierLimitMode.ExcludeAscended)
                {
                    foreach (StatueItem item in itemList)
                    {
                        string boss = item.name.Split('-').Last();
                        bossLogic += $" + GG_{boss}>{req + 1}";
                    }
                    lmb.DoMacroEdit(new("ASCENDED_IDOL", bossLogic));
                    lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_2", "ASCENDED_IDOL"));
                    lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_3", "ASCENDED_IDOL"));                
                }
                else
                if (hogSettings.RandomizeTiers == TierLimitMode.IncludeAll)
                {
                    foreach (StatueItem item in itemList)
                    {
                        string boss = item.name.Split('-').Last();
                        bossLogic += $" + GG_{boss}>{req + 2}";
                    }
                    lmb.DoMacroEdit(new("RADIANT_IDOL", bossLogic));
                    lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_3", "RADIANT_IDOL"));
                }

                // Override GG_Atrium_Roof's definition to fit Godhome Rando requirements
                lmb.DoLogicEdit(new("GG_Atrium_Roof", "GG_Atrium + LEFTCLAW + PANTHEON_COMPLETION_1 ? Defeated_Pantheon_1 + PANTHEON_COMPLETION_2 ? Defeated_Pantheon_2 + PANTHEON_COMPLETION_3 ? Defeated_Pantheon_3 + PANTHEON_COMPLETION_4 ? Defeated_Pantheon_4"));
            }

            // Add bindings requirement to Weathered Mask entry
            GodhomeRandomizerSettings.Panth panthSettings = GodhomeRandomizer.Instance.GS.Settings.Pantheons;
            if (panthSettings.PantheonsIncluded > PantheonLimitMode.None)
            {
                int multiplier = (int)panthSettings.PantheonsIncluded;
                int bindCount = (panthSettings.Nail ? 1 : 0) + (panthSettings.Shell ? 1 : 0) + (panthSettings.Charms ? 1 : 0) + (panthSettings.Soul ? 1 : 0);
                bool weatheredMask = lmb.LogicLookup.TryGetValue("Journal_Entry-Weathered_Mask", out _);
                if (multiplier * bindCount > 0 && weatheredMask)
                    lmb.DoLogicEdit(new("Journal_Entry-Weathered_Mask", $"ORIG + Pantheon_Bindings > {(multiplier * bindCount) - 1}"));
            }
        }

        private static void EditLostArtifacts(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
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

        private static void EditRandoPlus(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            try
            {
                lmb.GetTerm("NAILUPGRADE");
                lmb.DoLogicEdit(new("Nail_Upgradable_1", "NAILUPGRADE>0"));
                lmb.DoLogicEdit(new("Nail_Upgradable_2", "NAILUPGRADE>1"));
                lmb.DoLogicEdit(new("Nail_Upgradable_3", "NAILUPGRADE>2"));
                lmb.DoLogicEdit(new("Nail_Upgradable_4", "NAILUPGRADE>3"));
            }
            catch (KeyNotFoundException)
            {
            }
        }
    }
}