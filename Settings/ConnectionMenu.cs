﻿﻿using System;
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
            VerticalItemPanel hogLeftSettingHolder = new(ghPage, Vector2.zero, 60, false, [
                hogElementFactory.ElementLookup[nameof(HallOfGodsSettings.RandomizeTiers)],
                hogElementFactory.ElementLookup[nameof(HallOfGodsSettings.RandomizeOrdeal)],
            ]);
            VerticalItemPanel hogRightSettingHolder = new(ghPage, Vector2.zero, 60, false, [
                hogElementFactory.ElementLookup[nameof(HallOfGodsSettings.RandomizeStatueAccess)],
                hogElementFactory.ElementLookup[nameof(HallOfGodsSettings.DuplicateMarks)],
            ]);
            GridItemPanel hogSettingHolder = new(ghPage, Vector2.zero, 2, 100, 800, false, [hogLeftSettingHolder, hogRightSettingHolder]);

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
            topLevelPanel.SymSetNeighbor(Neighbor.Down, ghPage.backButton);
            pageRootButton = new SmallButton(connectionPage, "Godhome Randomizer");
            pageRootButton.AddHideAndShowEvent(connectionPage, ghPage);
        }

        private MenuPage DisplayGodhomeShop()
        {
            MenuPage godhomePage = new("Godhome Shop", ghPage);
            MenuLabel header = new(godhomePage, "Godhome Shop");
            shopElementFactory = new(godhomePage, GodhomeManager.GlobalSettings.GodhomeShop);
            VerticalItemPanel rootPanel = new(ghPage, new Vector2(0, 400), 75, true);

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
            rootPanel.Add(itemPanel);

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