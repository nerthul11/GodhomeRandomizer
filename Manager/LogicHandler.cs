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

namespace GodhomeRandomizer.Manager
{
    public class LogicHandler
    {
        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(0f, AddTransitions);
            RCData.RuntimeLogicOverride.Subscribe(0f, AddMacros);
            RCData.RuntimeLogicOverride.Subscribe(1f, AddHOGLogic);

            // Edit Attuned Jewel logic
            if (ModHooks.GetMod("LostArtifacts") is Mod)
                RCData.RuntimeLogicOverride.Subscribe(2f, EditLostArtifacts);

            // Add alternative check for several journal entries
            if (ModHooks.GetMod("TheRealJournalRando") is Mod)
                RCData.RuntimeLogicOverride.Subscribe(11f, EditTRJR);
            
            // Handle Nail Upgrade requirements for Nail-modifying mods
            if (ModHooks.GetMod("RandoPlus") is Mod)
                RCData.RuntimeLogicOverride.Subscribe(1400f, EditRandoPlus);
            
            // Pantheon logic and waypoints to be added after ExtraRando
            RCData.RuntimeLogicOverride.Subscribe(1250f, AddWaypoints);
            RCData.RuntimeLogicOverride.Subscribe(2048f, AddPantheonLogic);
            RCData.RuntimeLogicOverride.Subscribe(4096f, EditPantheonAccessLogic);
        }

        private static void AddMacros(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;

            JsonLogicFormat fmt = new();
            lmb.DeserializeFile(LogicFileType.Macros, fmt, typeof(GodhomeRandomizer).Assembly.GetManifestResourceStream($"GodhomeRandomizer.Resources.Logic.macros.json"));
        }

        private static void AddWaypoints(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
            JsonLogicFormat fmt = new();
            lmb.DeserializeFile(LogicFileType.Waypoints, fmt, typeof(GodhomeRandomizer).Assembly.GetManifestResourceStream($"GodhomeRandomizer.Resources.Logic.waypoints.json"));
        }

        private static void AddTransitions(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream stream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Logic.transitions.json");
            StreamReader reader = new(stream);
            List<RawLogicDef> list = jsonSerializer.Deserialize<List<RawLogicDef>>(new JsonTextReader(reader));

            foreach (RawLogicDef def in list)
                lmb.AddTransition(def);
        }
        private static void AddHOGLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;

            // Read item definitions
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueItems.json");
            StreamReader itemReader = new(itemStream);
            List<StatueItem> itemList = jsonSerializer.Deserialize<List<StatueItem>>(new JsonTextReader(itemReader));

            HallOfGodsSettings settings = GodhomeManager.GlobalSettings.HallOfGods;
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
                string logicDef = $"GG_Workshop[left1] + GG_{boss}>0";
                if (dependency is not null)
                    logicDef += $" + GG_{dependency}>0";
                if (item.isDreamBoss)
                    logicDef += " + DREAMNAIL";
                if (item.position == "TOP")
                    logicDef = logicDef.Replace("GG_Workshop[left1]", "(GG_Workshop[left1] + (ANYCLAW | $SHRIEKPOGO[6]) | Bench-Hall_of_Gods)");
                if (settings.RandomizeStatueAccess == AccessMode.Vanilla)
                {
                    logicDef = logicDef.Replace($"GG_{boss}>0", $"Defeated_{boss}");
                    logicDef = logicDef.Replace($"GG_{dependency}>0", $"Defeated_{dependency}");
                }
                if (settings.RandomizeStatueAccess == AccessMode.AllUnlocked)
                {
                    logicDef = logicDef.Replace($"GG_{boss}>0", $"TRUE");
                    logicDef = logicDef.Replace($"GG_{dependency}>0", $"TRUE");
                }
                lmb.AddLogicDef(new($"Empty_Mark-{boss}", logicDef));

                // Bronze/Silver/Gold logic
                lmb.AddLogicDef(new($"Bronze_Mark-{boss}", $"{logicDef} + Attuned_Combat + COMBAT[{boss}]"));
                lmb.AddLogicDef(new($"Silver_Mark-{boss}", $"{logicDef} + Ascended_Combat + COMBAT[{boss}]"));

                logicDef = logicDef.Replace($"GG_{boss}>0 + ", "");
                lmb.AddLogicDef(new($"Gold_Mark-{boss}", $"{logicDef} + Radiant_Combat + GG_{boss}>2 + COMBAT[{boss}]"));
            }

            // Eternal Ordeal logic
            lmb.AddItem(new EmptyItem("Eternal_Ordeal"));
            lmb.AddLogicDef(new("Eternal_Ordeal", "GG_Workshop + (BOSS + FOCUS | SPICYCOMBATSKIPS) + (Wall-Godhome_Workshop?TRUE + Wall-Eternal_Ordeal?TRUE + UPWALLBREAK) + RIGHTCLAW + WINGS + (LEFTSUPERDASH | LEFTSHARPSHADOW)"));
        }

        private static void AddPantheonLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
             
            // Add logic term
            lmb.GetOrAddTerm($"Pantheon_Bindings", TermType.Int);
            lmb.AddItem(new BoolItem("Pantheon_Master-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_1")));
            lmb.AddItem(new BoolItem("Pantheon_Artist-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_2")));
            lmb.AddItem(new BoolItem("Pantheon_Sage-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_3")));
            lmb.AddItem(new BoolItem("Pantheon_Knight-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_4")));
            lmb.AddItem(new BoolItem("Pantheon_Hallownest-Completion", lmb.GetOrAddTerm($"PANTHEON_COMPLETION_5")));

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
            HallOfGodsSettings settings = GodhomeManager.GlobalSettings.HallOfGods;   

            if (settings.RandomizeStatueAccess == AccessMode.AllUnlocked)
            {
                lmb.DoLogicEdit(new("Opened_Pantheon_1", "GG_Atrium + PANTHEON_KEY_1 ? TRUE"));
                lmb.DoLogicEdit(new("Opened_Pantheon_2", "GG_Atrium + PANTHEON_KEY_2 ? TRUE"));
                lmb.DoLogicEdit(new("Opened_Pantheon_3", "GG_Atrium + PANTHEON_KEY_3 ? TRUE"));
                lmb.DoLogicEdit(new("Opened_Pantheon_4", "GG_Atrium + (PANTHEON_COMPLETION_1 + PANTHEON_COMPLETION_2 + PANTHEON_COMPLETION_3 | (PANTHEON_KEY_4 ? FALSE))"));
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
                        lmb.DoLogicEdit(new($"Opened_Pantheon_{panthID}", $"GG_Atrium + (PANTHEON_COMPLETION_1 + PANTHEON_COMPLETION_2 + PANTHEON_COMPLETION_3 + {bossLogic} | (PANTHEON_KEY_{panthID} ? FALSE))"));
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

            HallOfGodsSettings hogSettings = GodhomeManager.GlobalSettings.HallOfGods;

            // For boss journal entries, add HOG clearance as an option if access is altered
            if (hogSettings.RandomizeStatueAccess > AccessMode.Vanilla)
            {
                string[] entries = [
                    "Gruz_Mother", "Vengefly_King", "Brooding_Mawlek", "False_Knight",
                    "Massive_Moss_Charger", "Mantis_Lords", "Grey_Prince_Zote", "Broken_Vessel",
                    "Nosk", "Flukemarm", "Soul_Master", "God_Tamer", "Uumuu", "Dung_Defender", 
                    "White_Defender", "Hive_Knight", "Traitor_Lord", "Grimm", "Pure_Vessel",
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
                    lmb.DoLogicEdit(new("Defeated_Any_Winged_Zoteling", "ORIG | *Bronze_Mark-Grey_Prince_Zote | *Eternal_Ordeal"));
                    // Hopping Zotelings aren't guaranteed to appear on HoG battles.
                    lmb.DoLogicEdit(new("Defeated_Any_Hopping_Zoteling", "ORIG | *Eternal_Ordeal"));
                    lmb.DoLogicEdit(new("Defeated_Any_Volatile_Zoteling", "ORIG | *Bronze_Mark-Grey_Prince_Zote | *Eternal_Ordeal"));
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
            string bossLogic = "GG_Workshop[left1]";
            foreach (StatueItem item in itemList)
            {
                string boss = item.name.Split('-').Last();
                bossLogic += $" + GG_{boss}>1";
            }
            lmb.DoMacroEdit(new("ATTUNED_IDOL", bossLogic));
            lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_1", "ATTUNED_IDOL"));    
            
            bossLogic = "GG_Workshop[left1]";
            foreach (StatueItem item in itemList)
            {
                string boss = item.name.Split('-').Last();
                bossLogic += $" + GG_{boss}>2";
            }
            lmb.DoMacroEdit(new("ASCENDED_IDOL", bossLogic));
            lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_2", "ASCENDED_IDOL"));
            
            bossLogic = "GG_Workshop[left1]";
            foreach (StatueItem item in itemList)
            {
                string boss = item.name.Split('-').Last();
                bossLogic += $" + GG_{boss}>3";
            }
            lmb.DoMacroEdit(new("RADIANT_IDOL", bossLogic));
            lmb.DoLogicEdit(new("Journal_Entry-Void_Idol_3", "RADIANT_IDOL"));

            // Add bindings requirement to Weathered Mask entry
            bool weatheredMask = lmb.LogicLookup.TryGetValue("Journal_Entry-Weathered_Mask", out _);
            if (weatheredMask)
                lmb.DoSubst(new("Journal_Entry-Weathered_Mask", "Defeated_Pantheon_5", "Pantheon_Bindings>19"));
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

            HallOfGodsSettings settings = GodhomeManager.GlobalSettings.HallOfGods;

            if (settings.RandomizeTiers > TierLimitMode.Vanilla)
            {
                string logic = "GG_Workshop[left1]";
                foreach (StatueItem item in itemList)
                {
                    string boss = item.name.Split('-').Last();
                    logic += $" + GG_{boss}>1";
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
            catch (KeyNotFoundException) {} // Ignore if NAILUPGRADE isn't present
        }
    }
}