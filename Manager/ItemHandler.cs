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
using System;

namespace GodhomeRandomizer.Manager {
    internal static class ItemHandler
    {
        private static StatueCostProvider costProvider;
        internal static void Hook()
        {
            DefineObjects();
            RequestBuilder.OnUpdate.Subscribe(float.NegativeInfinity, SetupShopCost);
            RequestBuilder.OnUpdate.Subscribe(-500f, DefineShopRef);
            RequestBuilder.OnUpdate.Subscribe(-100f, RandomizeShopCost);
            RequestBuilder.OnUpdate.Subscribe(0f, AddGodhomeShop);
            RequestBuilder.OnUpdate.Subscribe(100f, AddHOGObjects);
            RequestBuilder.OnUpdate.Subscribe(105f, AddPantheonObjects);
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
            if (!GodhomeManager.GlobalSettings.Enabled || !GodhomeManager.GlobalSettings.GodhomeShop)
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

        private static void SetupShopCost(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled || !GodhomeManager.GlobalSettings.GodhomeShop)
                return;

            int statues = 44;
            int multiplier = (int)GodhomeManager.GlobalSettings.HallOfGods.RandomizeTiers;
            multiplier += GodhomeManager.GlobalSettings.HallOfGods.RandomizeStatueAccess == AccessMode.Randomized ? 1 : 0;
            costProvider = new("STATUEMARKS", (int)(0.1 * statues * multiplier), (int)(0.2 * statues * multiplier), amount => new StatueCost(amount));
        }

        // Rightfully stolen from MoreLocations & GrassRando. I have literally no idea of what this does.
        private static void RandomizeShopCost(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled || !GodhomeManager.GlobalSettings.GodhomeShop)
                return;
            
            builder.CostConverters.Subscribe(150f, RandomizeCost);
            builder.EditLocationRequest("Godhome_Shop", info =>
            {
                info.customPlacementFetch += (factory, rp) =>
                {
                    if (factory.TryFetchPlacement(rp.Location.Name, out AbstractPlacement plt))
                        return plt;
                    ShopLocation shop = (ShopLocation)factory.MakeLocation(rp.Location.Name);
                    shop.costDisplayerSelectionStrategy = new MixedCostDisplayerSelectionStrategy()
                    {
                        Capabilities = {new StatueCostSupport()}
                    };
                    plt = shop.Wrap();
                    factory.AddPlacement(plt);
                    return plt;
                };

                info.onRandoLocationCreation += (factory, rl) =>
                {
                    if (costProvider == null)
                        return;
                    LogicCost nextCost = costProvider.Next(factory.lm, factory.rng);
                    rl.AddCost(nextCost);
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
            
            // Don't add unless settings randomize at least one statue mark type.
            int multiplier = (int)GodhomeManager.GlobalSettings.HallOfGods.RandomizeTiers;
            multiplier += GodhomeManager.GlobalSettings.HallOfGods.RandomizeStatueAccess == AccessMode.Randomized ? 1 : 0;
            if (multiplier == 0)
                return;
            
            if (GodhomeManager.GlobalSettings.GodhomeShop)
                builder.AddLocationByName("Godhome_Shop");
        }

        public static void AddHOGObjects(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;

            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            GodhomeRandomizerSettings.HOG settings = GodhomeManager.GlobalSettings.HallOfGods;
            int itemCount = (int)settings.RandomizeTiers;
            if (settings.RandomizeStatueAccess == AccessMode.Randomized)
                itemCount += 1;

            if (itemCount > 0)
            {
                // Add the shop only if there are statue marks available
                

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
                
                // Filter Gold, Silver and Bronze marks if TierLimitMode excludes them.
                if (settings.RandomizeTiers == TierLimitMode.ExcludeRadiant)
                {
                    locationList = locationList.Where(location => !location.name.StartsWith("Gold")).ToList();
                }
                else if (settings.RandomizeTiers == TierLimitMode.ExcludeAscended)
                {
                    locationList = locationList.Where(location => !location.name.StartsWith("Gold") && !location.name.StartsWith("Silver")).ToList();
                }
                else if (settings.RandomizeTiers == TierLimitMode.Vanilla)
                {
                    locationList = locationList.Where(location => location.name.StartsWith("Empty")).ToList();
                }

                // Remove statue access locations if StatueAccessMode isn't randomized
                if (settings.RandomizeStatueAccess != AccessMode.Randomized)
                {
                    locationList = locationList.Where(location => !location.name.StartsWith("Empty")).ToList();
                }

                foreach (StatueLocation location in locationList)
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
        }

        private static void AddPantheonObjects(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            GodhomeRandomizerSettings.Panth settings = GodhomeManager.GlobalSettings.Pantheons;
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
        }
    }
}