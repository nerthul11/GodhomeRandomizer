﻿using System;
using GodhomeRandomizer.Manager;
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
        private SmallButton godhomeShopPage;
        private MenuElementFactory<GodhomeShopSettings> shopElementFactory;
        private MenuElementFactory<HallOfGodsSettings> hogElementFactory;
        private MenuElementFactory<PantheonSettings> pantheonElementFactory;

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
            // Define main connection page
            ghPage = new MenuPage("ghPage", connectionPage);
            topLevelElementFactory = new(ghPage, GodhomeManager.GlobalSettings);
            hogElementFactory = new(ghPage, GodhomeManager.GlobalSettings.HallOfGods);
            pantheonElementFactory = new(ghPage, GodhomeManager.GlobalSettings.Pantheons);
            VerticalItemPanel topLevelPanel = new(ghPage, new Vector2(0, 400), 75, true);
            VerticalItemPanel itemPanel = new(ghPage, new Vector2(0, 400), 180, false);

            // Define shop page
            godhomeShopPage = new(ghPage, "Godhome Shop");
            godhomeShopPage.AddHideAndShowEvent(DisplayGodhomeShop());
            
            // Define HOG Parameters
            topLevelElementFactory.ElementLookup["Enabled"].SelfChanged += EnableSwitch;
            MenuLabel hogLabel = new(ghPage, "Hall of Gods Randomizer");
            VerticalItemPanel hogLeftSettingHolder1 = new(ghPage, Vector2.zero, 60, false, [
                hogElementFactory.ElementLookup[nameof(HallOfGodsSettings.RandomizeTiers)]
            ]);
            VerticalItemPanel hogRightSettingHolder1 = new(ghPage, Vector2.zero, 60, false, [
                hogElementFactory.ElementLookup[nameof(HallOfGodsSettings.RandomizeStatueAccess)]
            ]);
            VerticalItemPanel hogLeftSettingHolder2 = new(ghPage, Vector2.zero, 60, false, [
                hogElementFactory.ElementLookup[nameof(HallOfGodsSettings.RandomizeOrdeal)]
            ]);
            VerticalItemPanel hogMiddleSettingHolder = new(ghPage, Vector2.zero, 60, false, [
                hogElementFactory.ElementLookup[nameof(HallOfGodsSettings.IncludeUnlockLocations)]
            ]);
            VerticalItemPanel hogRightSettingHolder2 = new(ghPage, Vector2.zero, 60, false, [
                hogElementFactory.ElementLookup[nameof(HallOfGodsSettings.DuplicateMarks)]
            ]);
            GridItemPanel hogSettingHolder1 = new(ghPage, Vector2.zero, 2, 0, 800, false, [hogLeftSettingHolder1, hogRightSettingHolder1]);
            GridItemPanel hogSettingHolder2 = new(ghPage, Vector2.zero, 3, 0, 500, false, [hogLeftSettingHolder2, hogMiddleSettingHolder, hogRightSettingHolder2]);
            VerticalItemPanel hogSettingHolder = new(ghPage, Vector2.zero, 60, false, [hogSettingHolder1, hogSettingHolder2]);
            // Define Pantheon Parameters
            MenuLabel panthLabel = new(ghPage, "Pantheon Randomizer");
            
            VerticalItemPanel panthSettingHolder = new(ghPage, Vector2.zero, 55, false, [
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.ApplyAccessToPantheons)],
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.PantheonsIncluded)]
            ]);
            VerticalItemPanel bindLeftSettingHolder = new(ghPage, Vector2.zero, 60, false, [
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.Completion)],
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.Nail)],
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.Charms)],
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.AllAtOnce)]
            ]);
            VerticalItemPanel bindRightSettingHolder = new(ghPage, Vector2.zero, 60, false, [
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.Lifeblood)],
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.Shell)],
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.Soul)],
                pantheonElementFactory.ElementLookup[nameof(PantheonSettings.Hitless)]
            ]);
            GridItemPanel bindSettingHolder = new(ghPage, Vector2.zero, 2, 100, 200, false, [bindLeftSettingHolder, bindRightSettingHolder]);
            panthSettingHolder.Insert(0, panthLabel);

            // Define top level
            itemPanel.Add(hogSettingHolder);
            itemPanel.Add(panthSettingHolder);
            itemPanel.Add(bindSettingHolder);
            topLevelPanel.Add(topLevelElementFactory.ElementLookup[nameof(GodhomeRandomizerSettings.Enabled)]);
            topLevelPanel.Add(godhomeShopPage);
            topLevelPanel.Add(hogLabel);
            topLevelPanel.Add(itemPanel);
            topLevelPanel.ResetNavigation();
            topLevelPanel.SymSetNeighbor(Neighbor.Up, ghPage.backButton);
            topLevelPanel.SymSetNeighbor(Neighbor.Down, ghPage.backButton);
            pageRootButton = new SmallButton(connectionPage, "Godhome Randomizer");
            pageRootButton.AddHideAndShowEvent(connectionPage, ghPage);
        }

        private MenuPage DisplayGodhomeShop()
        {
            MenuPage godhomePage = new("Godhome Shop", ghPage);
            MenuLabel header = new(godhomePage, "Godhome Shop");
            shopElementFactory = new(godhomePage, GodhomeManager.GlobalSettings.GodhomeShop);

            VerticalItemPanel itemPanel = new(godhomePage, new Vector2(0, 350), 75, false);
            GridItemPanel costPanel = new(godhomePage, Vector2.zero, 2, 100, 250, false,
                [
                shopElementFactory.ElementLookup[nameof(GodhomeShopSettings.MinimumCost)], 
                shopElementFactory.ElementLookup[nameof(GodhomeShopSettings.MaximumCost)]
                ]);
            
            itemPanel.Add(header);
            itemPanel.Add(shopElementFactory.ElementLookup[nameof(GodhomeShopSettings.Enabled)]);
            itemPanel.Add(shopElementFactory.ElementLookup[nameof(GodhomeShopSettings.IncludeInJunkShop)]);
            itemPanel.Add(costPanel);
            itemPanel.Add(shopElementFactory.ElementLookup[nameof(GodhomeShopSettings.Tolerance)]);

            itemPanel.ResetNavigation();
            itemPanel.SymSetNeighbor(Neighbor.Up, godhomePage.backButton);
            itemPanel.SymSetNeighbor(Neighbor.Down, godhomePage.backButton);

            SetButtonColor(godhomeShopPage, () => GodhomeManager.GlobalSettings.GodhomeShop.Enabled);
            return godhomePage;
        }

        private void SetButtonColor(SmallButton target, Func<bool> condition)
        {
            target.Parent.BeforeShow += () =>
            {
                target.Text.color = condition() ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            };
        }

        // Define parameter changes
        private void EnableSwitch(IValueElement obj)
        {
            pageRootButton.Text.color = GodhomeManager.GlobalSettings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }

        // Apply proxy settings
        public void Disable()
        {
            IValueElement enabled = topLevelElementFactory.ElementLookup[nameof(GodhomeRandomizerSettings.Enabled)];
            enabled.SetValue(false);
        }

        public void Apply(GodhomeRandomizerSettings settings)
        {
            topLevelElementFactory.SetMenuValues(settings);
            shopElementFactory.SetMenuValues(settings.GodhomeShop);
            hogElementFactory.SetMenuValues(settings.HallOfGods);
            pantheonElementFactory.SetMenuValues(settings.Pantheons);           
        }
    }
}