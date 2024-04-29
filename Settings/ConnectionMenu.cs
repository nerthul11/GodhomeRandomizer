﻿using GodhomeRandomizer.Manager;
using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using UnityEngine;

namespace GodhomeRandomizer.Settings
{
    public class ConnectionMenu 
    {
        // Top-level definitions
        internal static ConnectionMenu Instance { get; private set; }
        private readonly SmallButton pageRootButton;

        // Menu page and elements
        private readonly MenuPage ghPage;
        private MenuElementFactory<GodhomeRandomizerSettings> topLevelElementFactory;
        private MenuElementFactory<GodhomeRandomizerSettings.HOG> hogElementFactory;
        private MenuElementFactory<GodhomeRandomizerSettings.Panth> pantheonElementFactory;

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += () => Instance = null;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance.pageRootButton;
            button.Text.color = GodhomeManager.GlobalSettings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            return true;
        }

        private static void ConstructMenu(MenuPage connectionPage)
        {
            Instance = new(connectionPage);
        }

        private ConnectionMenu(MenuPage connectionPage)
        {
            // Define connection page
            ghPage = new MenuPage("ghPage", connectionPage);
            topLevelElementFactory = new(ghPage, GodhomeManager.GlobalSettings);
            hogElementFactory = new(ghPage, GodhomeManager.GlobalSettings.HallOfGods);
            pantheonElementFactory = new(ghPage, GodhomeManager.GlobalSettings.Pantheons);
            VerticalItemPanel topLevelPanel = new(ghPage, new Vector2(0, 400), 75, true);
            VerticalItemPanel itemPanel = new(ghPage, new Vector2(0, 400), 240, false);
            
            // Define HOG Parameters
            topLevelElementFactory.ElementLookup["Enabled"].SelfChanged += EnableSwitch;
            MenuLabel hogLabel = new(ghPage, "Hall of Gods Randomizer");
            VerticalItemPanel hogSettingHolder = new(ghPage, Vector2.zero, 55, false, [
                hogElementFactory.ElementLookup["RandomizeTiers"],
                hogElementFactory.ElementLookup["RandomizeStatueAccess"],
                hogElementFactory.ElementLookup["RandomizeOrdeal"],
            ]);
            hogSettingHolder.Insert(0, hogLabel);

            // Define Pantheon Parameters
            MenuLabel panthLabel = new(ghPage, "Pantheon Randomizer");
            
            VerticalItemPanel panthSettingHolder = new(ghPage, Vector2.zero, 55, false, [
                pantheonElementFactory.ElementLookup["ApplyAccessToPantheons"],
                pantheonElementFactory.ElementLookup["PantheonsIncluded"]
            ]);
            VerticalItemPanel bindLeftSettingHolder = new(ghPage, Vector2.zero, 60, false, [
                pantheonElementFactory.ElementLookup["Completion"],
                pantheonElementFactory.ElementLookup["Nail"],
                pantheonElementFactory.ElementLookup["Charms"],
                pantheonElementFactory.ElementLookup["AllAtOnce"]
            ]);
            VerticalItemPanel bindRightSettingHolder = new(ghPage, Vector2.zero, 60, false, [
                pantheonElementFactory.ElementLookup["Lifeblood"],
                pantheonElementFactory.ElementLookup["Shell"],
                pantheonElementFactory.ElementLookup["Soul"],
                pantheonElementFactory.ElementLookup["Hitless"]
            ]);
            GridItemPanel bindSettingHolder = new(ghPage, Vector2.zero, 2, 100, 200, false, [bindLeftSettingHolder, bindRightSettingHolder]);
            panthSettingHolder.Insert(0, panthLabel);

            // Define top level
            itemPanel.Add(hogSettingHolder);
            itemPanel.Add(panthSettingHolder);
            itemPanel.Add(bindSettingHolder);
            topLevelPanel.Add(topLevelElementFactory.ElementLookup["Enabled"]);
            topLevelPanel.Add(itemPanel);
            topLevelPanel.ResetNavigation();
            topLevelPanel.SymSetNeighbor(Neighbor.Down, ghPage.backButton);
            pageRootButton = new SmallButton(connectionPage, "Godhome Randomizer");
            pageRootButton.AddHideAndShowEvent(connectionPage, ghPage);
        }
        // Define parameter changes
        private void EnableSwitch(IValueElement obj)
        {
            pageRootButton.Text.color = GodhomeManager.GlobalSettings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }

        // Apply proxy settings
        public void Disable()
        {
            IValueElement elem = topLevelElementFactory.ElementLookup[nameof(GodhomeRandomizerSettings.Enabled)];
            elem.SetValue(false);
        }

        public void Apply(GodhomeRandomizerSettings settings)
        {
            topLevelElementFactory.SetMenuValues(settings);
            hogElementFactory.SetMenuValues(settings.HallOfGods);
            pantheonElementFactory.SetMenuValues(settings.Pantheons);           
        }
    }
}