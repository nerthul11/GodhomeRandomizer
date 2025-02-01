using GodhomeRandomizer.IC;
using GodhomeRandomizer.IC.Shop;
using GodhomeRandomizer.Settings;
using ItemChanger;
using ItemChanger.Locations;
using Newtonsoft.Json;
using RandomizerCore.Logic;
using RandomizerMod.RC;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using RandomizerMod.RandomizerData;
using System;
using RandomizerMod.Settings;

namespace GodhomeRandomizer.Manager {
    internal static class ItemHandler
    {
        internal static void Hook()
        {
            DefineObjects();
            ProgressionInitializer.OnCreateProgressionInitializer += AddTolerance;
            RequestBuilder.OnUpdate.Subscribe(-500f, DefineShopRef);
            RequestBuilder.OnUpdate.Subscribe(-100f, RandomizeShopCost);
            RequestBuilder.OnUpdate.Subscribe(0f, AddGodhomeShop);
            RequestBuilder.OnUpdate.Subscribe(10f, AddHOGObjects);
            RequestBuilder.OnUpdate.Subscribe(20f, AddPantheonObjects);
            RequestBuilder.OnUpdate.Subscribe(1100f, DefineTransitions);
        }

        public static void DefineObjects()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            
            using Stream itemStreamA = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueItems.json");
            StreamReader itemReaderA = new(itemStreamA);
            List<StatueItem> itemListA = jsonSerializer.Deserialize<List<StatueItem>>(new JsonTextReader(itemReaderA));
        
            foreach (StatueItem item in itemListA)
                Finder.DefineCustomItem(item);

            using Stream locationStreamA = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueLocations.json");
            StreamReader locationReaderA = new(locationStreamA);
            List<StatueLocation> locationListA = jsonSerializer.Deserialize<List<StatueLocation>>(new JsonTextReader(locationReaderA));
            foreach (StatueLocation location in locationListA)
                Finder.DefineCustomLocation(location);

            using Stream itemStreamB = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.BindingItems.json");
            StreamReader itemReaderB = new(itemStreamB);
            List<BindingItem> itemListB = jsonSerializer.Deserialize<List<BindingItem>>(new JsonTextReader(itemReaderB));
            foreach (BindingItem item in itemListB)
                Finder.DefineCustomItem(item);

            using Stream locationStreamB = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.BindingLocations.json");
            StreamReader locationReaderB = new(locationStreamB);
            List<BindingLocation> locationListB = jsonSerializer.Deserialize<List<BindingLocation>>(new JsonTextReader(locationReaderB));
            foreach (BindingLocation location in locationListB)
                Finder.DefineCustomLocation(location);

            Finder.DefineCustomItem(new LifebloodItem());
            Finder.DefineCustomLocation(new LifebloodLocation());

            Finder.DefineCustomItem(new OrdealItem());
            Finder.DefineCustomLocation(new OrdealLocation());

            Finder.DefineCustomLocation(new GodhomeShop());
        }

        private static void DefineShopRef(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled || !GodhomeManager.GlobalSettings.GodhomeShop.Enabled)
                return;

            builder.EditLocationRequest("Godhome_Shop", info =>
            {
                info.getLocationDef = () => new()
                {
                    Name = "Godhome_Shop",
                    SceneName = SceneNames.GG_Workshop,
                    FlexibleCount = true,
                    AdditionalProgressionPenalty = true
                };
            });
        }

        private static void RandomizeShopCost(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled || !GodhomeManager.GlobalSettings.GodhomeShop.Enabled)
                return;
            
            builder.CostConverters.Subscribe(150f, RandomizeCost);
            builder.EditLocationRequest("Godhome_Shop", info =>
            {
                info.customPlacementFetch += (factory, rp) =>
                {
                    if (factory.TryFetchPlacement(rp.Location.Name, out AbstractPlacement plt))
                        return plt;
                    ShopLocation shop = (ShopLocation)factory.MakeLocation(rp.Location.Name);
                    plt = shop.Wrap();
                    factory.AddPlacement(plt);
                    return plt;
                };

                info.onRandoLocationCreation += (factory, rl) =>
                {
                    LogicManager lm = factory.lm;
                    Random rng = factory.rng;
                    int minCost = (int)(176 * GodhomeManager.GlobalSettings.GodhomeShop.MinimumCost);
                    int maxCost = (int)(176 * GodhomeManager.GlobalSettings.GodhomeShop.MaximumCost);
                    rl.AddCost(new StatueLogicCost(lm.GetTermStrict("STATUEMARKS"), rng.Next(minCost, maxCost), amount => new StatueCost(amount)));
                };
            });
        }

        private static bool RandomizeCost(LogicCost logicCost, out Cost cost)
        {
            if (logicCost is StatueLogicCost c)
            {
                cost = c.GetIcCost();
                return true;
            }
            else
            {
                cost = default;
                return false;
            }
        }

        private static void AddGodhomeShop(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
            if (GodhomeManager.GlobalSettings.GodhomeShop.Enabled)
                builder.AddLocationByName("Godhome_Shop");
        }

        public static void AddHOGObjects(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;

            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            HallOfGodsSettings settings = GodhomeManager.GlobalSettings.HallOfGods;
            int itemCount = (int)settings.RandomizeTiers;
            if (settings.RandomizeStatueAccess == AccessMode.Randomized)
                itemCount += 1;

            // Define items
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueItems.json");
            StreamReader itemReader = new(itemStream);
            List<StatueItem> itemList = jsonSerializer.Deserialize<List<StatueItem>>(new JsonTextReader(itemReader));
        
            foreach (StatueItem item in itemList)
            {
                builder.AddItemByName(item.name, itemCount);
                if (settings.DuplicateMarks)
                    builder.AddItemByName($"{PlaceholderItem.Prefix}{item.name}");
                builder.EditItemRequest(item.name, info =>
                {
                    info.getItemDef = () => new()
                    {
                        MajorItem = false,
                        Name = item.name,
                        Pool = "Statue Marks",
                        PriceCap = 500
                    };
                });
            }

            // Define locations
            using Stream locationStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueLocations.json");
            StreamReader locationReader = new(locationStream);
            List<StatueLocation> locationList = jsonSerializer.Deserialize<List<StatueLocation>>(new JsonTextReader(locationReader));
            List<StatueLocation> filteredLocationList = locationList.Where(location => location.name.Contains("Mark")).ToList();

            // Filter Gold, Silver and Bronze marks if TierLimitMode excludes them.
            if (settings.RandomizeTiers <= TierLimitMode.ExcludeRadiant)
                filteredLocationList = filteredLocationList.Where(location => !location.name.StartsWith("Gold")).ToList();

            if (settings.RandomizeTiers <= TierLimitMode.ExcludeAscended)
                filteredLocationList = filteredLocationList.Where(location => !location.name.StartsWith("Silver")).ToList();

            if (settings.RandomizeTiers == TierLimitMode.Vanilla)
                filteredLocationList = filteredLocationList.Where(location => !location.name.StartsWith("Bronze")).ToList();

            // Remove statue access locations if the Unlock Locations aren't included
            if (!settings.IncludeUnlockLocations)
                filteredLocationList = filteredLocationList.Where(location => !location.name.StartsWith("Empty")).ToList();

            foreach (StatueLocation location in locationList)
            {
                if (filteredLocationList.Contains(location))
                {
                    builder.AddLocationByName(location.name);
                    builder.EditLocationRequest(location.name, info =>
                    {
                        info.getLocationDef = () => new()
                        {
                            Name = location.name,
                            SceneName = SceneNames.GG_Workshop,
                            FlexibleCount = false,
                            AdditionalProgressionPenalty = false
                        };
                    });
                }
                else
                {
                    // If the location doesn't exist but the item does, we don't want a vanilla def for it as it's randomized.
                    if (!(location.name.StartsWith("Empty") && settings.RandomizeStatueAccess == AccessMode.Randomized))
                        builder.AddToVanilla($"Statue_Mark-{location.name.Split('-')[1]}", location.name);
                }

                // If the location exists but the item is not present in the pool, we need to add a vanilla def for logic to work.
                if (location.name.StartsWith("Empty") && settings.IncludeUnlockLocations && settings.RandomizeStatueAccess != AccessMode.Randomized)
                    builder.AddToVanilla($"Statue_Mark-{location.name.Split('-')[1]}", location.name);
            }
            
            // Add Eternal Ordeal if available
            if (settings.RandomizeOrdeal)
            {
                builder.AddItemByName("Eternal_Ordeal");
                builder.EditItemRequest("Eternal_Ordeal", info =>
                {
                    info.getItemDef = () => new()
                    {
                        MajorItem = false,
                        Name = "Eternal_Ordeal",
                        Pool = "Statue Marks",
                        PriceCap = 1
                    };
                });
                builder.AddLocationByName("Eternal_Ordeal");
                builder.EditLocationRequest("Eternal_Ordeal", info =>
                {
                    info.getLocationDef = () => new()
                    {
                        Name = "Eternal_Ordeal",
                        SceneName = SceneNames.GG_Workshop,
                        FlexibleCount = false,
                        AdditionalProgressionPenalty = false
                    };
                });
            }
            else
                builder.AddToVanilla("Eternal_Ordeal", "Eternal_Ordeal");
        }

        private static void AddPantheonObjects(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            PantheonSettings settings = GodhomeManager.GlobalSettings.Pantheons;
            List<string> availableSettings = [];
            if (settings.Completion)
                availableSettings.Add("Completion");
            if (settings.Nail)
                availableSettings.Add("Nail");
            if (settings.Shell)
                availableSettings.Add("Shell");
            if (settings.Charms)
                availableSettings.Add("Charms");
            if (settings.Soul)
                availableSettings.Add("Soul");
            if (settings.AllAtOnce)
                availableSettings.Add("AllAtOnce");
            if (settings.Hitless)
                availableSettings.Add("Hitless");

            // Define items
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.BindingItems.json");
            StreamReader itemReader = new(itemStream);
            List<BindingItem> itemList = jsonSerializer.Deserialize<List<BindingItem>>(new JsonTextReader(itemReader));
            foreach (BindingItem item in itemList)
            {
                if (item.pantheonID <= settings.PantheonsIncluded && availableSettings.Contains(item.bindingType))
                {    
                    builder.AddItemByName(item.name);
                    builder.EditItemRequest(item.name, info =>
                    {
                        info.getItemDef = () => new()
                        {
                            MajorItem = item.bindingType == "Completion",
                            Name = item.name,
                            Pool = "Pantheon Marks",
                            PriceCap = 500
                        };
                    });
                }
                else
                    builder.AddToVanilla(item.name, item.name);
            }

            // Define locations
            using Stream locationStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.BindingLocations.json");
            StreamReader locationReader = new(locationStream);
            List<BindingLocation> locationList = jsonSerializer.Deserialize<List<BindingLocation>>(new JsonTextReader(locationReader));
            foreach (BindingLocation location in locationList)
            {
                if (location.pantheonID <= settings.PantheonsIncluded && availableSettings.Contains(location.bindingType))
                {
                    builder.AddLocationByName(location.name);
                    builder.EditLocationRequest(location.name, info =>
                    {
                        info.getLocationDef = () => new()
                        {
                            Name = location.name,
                            SceneName = location.pantheonID < PantheonLimitMode.Hallownest ? SceneNames.GG_Atrium : SceneNames.GG_Atrium_Roof,
                            FlexibleCount = false,
                            AdditionalProgressionPenalty = false
                        };
                    });
                }
            }

            // Add lifeblood if available
            if (settings.Lifeblood)
            {
                builder.AddItemByName("Godhome_Lifeblood");
                builder.EditItemRequest("Godhome_Lifeblood", info =>
                    {
                        info.getItemDef = () => new()
                        {
                            MajorItem = false,
                            Name = "Godhome_Lifeblood",
                            Pool = "Lifeblood",
                            PriceCap = 500
                        };
                    });
                builder.AddLocationByName("Godhome_Lifeblood");
                builder.EditLocationRequest("Godhome_Lifeblood", info =>
                {
                    info.getLocationDef = () => new()
                    {
                        Name = "Godhome_Lifeblood",
                        SceneName = SceneNames.GG_Blue_Room,
                        FlexibleCount = false,
                        AdditionalProgressionPenalty = false
                    };
                });
            }
            else
                builder.AddToVanilla("Godhome_Lifeblood", "Godhome_Lifeblood");
        }

        private static void DefineTransitions(RequestBuilder rb)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            using Stream stream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.Transitions.json");
            StreamReader reader = new(stream);
            List<TransitionDef> list = jsonSerializer.Deserialize<List<TransitionDef>>(new JsonTextReader(reader));
            int group = 1;
            foreach (TransitionDef def in list)
            {
                bool shouldBeIncluded = def.IsMapAreaTransition && (rb.gs.TransitionSettings.Mode >= TransitionSettings.TransitionMode.MapAreaRandomizer);
                shouldBeIncluded |= def.IsTitledAreaTransition && (rb.gs.TransitionSettings.Mode >= TransitionSettings.TransitionMode.FullAreaRandomizer);
                shouldBeIncluded |= rb.gs.TransitionSettings.Mode >= TransitionSettings.TransitionMode.RoomRandomizer;
                if (shouldBeIncluded)
                {
                    rb.EditTransitionRequest($"{def.SceneName}[{def.DoorName}]", info => info.getTransitionDef = () => def);
                    bool uncoupled = rb.gs.TransitionSettings.TransitionMatching == TransitionSettings.TransitionMatchingSetting.NonmatchingDirections;
                    if (uncoupled)
                    {
                        SelfDualTransitionGroupBuilder tgb = rb.EnumerateTransitionGroups().First(x => x.label == RBConsts.TwoWayGroup) as SelfDualTransitionGroupBuilder;
                        tgb.Transitions.Add($"{def.SceneName}[{def.DoorName}]");
                    }
                    else
                    {
                        SymmetricTransitionGroupBuilder stgb = rb.EnumerateTransitionGroups().First(x => x.label == RBConsts.InLeftOutRightGroup) as SymmetricTransitionGroupBuilder;
                        if (group == 1)
                            stgb.Group1.Add($"{def.SceneName}[{def.DoorName}]");
                        else
                            stgb.Group2.Add($"{def.SceneName}[{def.DoorName}]");
                    }
                    group = group == 1 ? 2 : 1;
                }
                else
                {
                    rb.EditTransitionRequest($"{def.SceneName}[{def.DoorName}]", info => info.getTransitionDef = () => def);
                    rb.EnsureVanillaSourceTransition($"{def.SceneName}[{def.DoorName}]");
                }
            }
        }
        
        private static void AddTolerance(LogicManager lm, GenerationSettings settings, ProgressionInitializer initializer)
        {
            if (GodhomeManager.GlobalSettings.Enabled && GodhomeManager.GlobalSettings.GodhomeShop.Enabled)
            {
                int duplicates = GodhomeManager.GlobalSettings.HallOfGods.DuplicateMarks ? 44 : 0;
                int markCost = Math.Max((int)(176 * GodhomeManager.GlobalSettings.GodhomeShop.MaximumCost), 1);
                int markTolerance = Math.Min((int)(markCost * GodhomeManager.GlobalSettings.GodhomeShop.Tolerance + duplicates), 176 - markCost);
                initializer.Setters.Add(new(lm.GetTermStrict("STATUEMARKS"), -markTolerance));
            }
        }
    }
}