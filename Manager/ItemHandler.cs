using GodhomeRandomizer.IC;
using GodhomeRandomizer.Settings;
using ItemChanger;
using Newtonsoft.Json;
using RandomizerMod.RC;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GodhomeRandomizer.Manager {
    internal static class ItemHandler
    {
        internal static void Hook()
        {
            DefineObjects();
            RequestBuilder.OnUpdate.Subscribe(100f, AddHOGObjects);
            RequestBuilder.OnUpdate.Subscribe(100f, AddPantheonObjects);
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
        }
        private static void AddPantheonObjects(RequestBuilder builder)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            GodhomeRandomizerSettings.Panth settings = GodhomeManager.GlobalSettings.Pantheons;
            List<string> availableSettings = [];
            if (settings.Completion == true)
                availableSettings.Add("Complete");
            if (settings.Nail == true || settings.AllAtOnce == true)
                availableSettings.Add("Nail");
            if (settings.Shell == true || settings.AllAtOnce == true)
                availableSettings.Add("Shell");
            if (settings.Charms == true || settings.AllAtOnce == true)
                availableSettings.Add("Charms");
            if (settings.Soul == true || settings.AllAtOnce == true)
                availableSettings.Add("Soul");
            if (settings.AllAtOnce == true)
                availableSettings.Add("AllAtOnce");
            if (settings.Hitless == true)
                availableSettings.Add("Hitless");

            // Define items
            using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.BindingItems.json");
            StreamReader itemReader = new(itemStream);
            List<BindingItem> itemList = jsonSerializer.Deserialize<List<BindingItem>>(new JsonTextReader(itemReader));
            foreach (BindingItem item in itemList)
            {
                if (item.pantheonID <= settings.PantheonsIncluded && availableSettings.Contains(item.bindingType))
                    builder.AddItemByName(item.name);
            }

            // Define locations
            using Stream locationStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.BindingLocations.json");
            StreamReader locationReader = new(locationStream);
            List<BindingLocation> locationList = jsonSerializer.Deserialize<List<BindingLocation>>(new JsonTextReader(locationReader));
            foreach (BindingLocation location in locationList)
            {
                if (location.pantheonID <= settings.PantheonsIncluded && availableSettings.Contains(location.bindingType))
                    builder.AddLocationByName(location.name);
            }

            // Add lifeblood if available
            if (settings.Lifeblood)
            {
                builder.AddItemByName("Godhome_Lifeblood");
                builder.AddLocationByName("Godhome_Lifeblood");
            }
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
                // Define items
                using Stream itemStream = assembly.GetManifestResourceStream("GodhomeRandomizer.Resources.Data.StatueItems.json");
                StreamReader itemReader = new(itemStream);
                List<StatueItem> itemList = jsonSerializer.Deserialize<List<StatueItem>>(new JsonTextReader(itemReader));
            
                foreach (StatueItem item in itemList)
                    builder.AddItemByName(item.name, itemCount);

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
                    builder.AddLocationByName(location.name);
                
                // Add Eternal Ordeal if available
                if (settings.RandomizeOrdeal)
                {
                    builder.AddItemByName("Eternal_Ordeal");
                    builder.AddLocationByName("Eternal_Ordeal");
                }
            }
        }
    }
}