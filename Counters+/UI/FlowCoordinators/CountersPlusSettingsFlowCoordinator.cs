using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using CountersPlus.UI.ViewControllers;
using CountersPlus.Utils;
using HMUI;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static CountersPlus.Utils.Accessors;

namespace CountersPlus.UI.FlowCoordinators
{
    public class CountersPlusSettingsFlowCoordinator : FlowCoordinator
    {
        public readonly Vector3 MAIN_SCREEN_OFFSET = new Vector3(0, -4, 0);

        [Inject] public List<ConfigModel> AllConfigModels;
        [Inject] private CanvasUtility canvasUtility;
        [Inject] private MockCounter mockCounter;
        [Inject] private MenuTransitionsHelper menuTransitionsHelper;
        [Inject] private GameScenesManager gameScenesManager;
        [Inject] private FadeInOutController fadeInOutController;

        [Inject] private CountersPlusCreditsViewController credits;
        [Inject] private CountersPlusBlankViewController blank;
        [Inject] private CountersPlusMainScreenNavigationController mainScreenNavigation;
        [Inject] private CountersPlusSettingSectionSelectionViewController settingsSelection;
        [Inject] private CountersPlusHorizontalSettingsListViewController horizontalSettingsList;

        private HashSet<string> persistentScenes = new HashSet<string>();

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (addedToHierarchy)
            {
                showBackButton = true;
                SetTitle("Counters+");

                persistentScenes = GSMPersistentScenes(ref gameScenesManager); // Get our hashset of persistent scenes

                persistentScenes.Add("MenuViewControllers"); // Make sure our menu persists through the transition
                persistentScenes.Add("MenuCore");
                persistentScenes.Add("Menu");

                var tutorialSceneSetup = MTHTutorialScenesSetup(ref menuTransitionsHelper); // Grab the scene transition setup data
                tutorialSceneSetup.Init();

                // We're actually transitioning to the Tutorial sequence, but disabling the tutorial itself from starting.
                gameScenesManager.PushScenes(tutorialSceneSetup, 0.25f, null, (_) =>
                {
                    // Disable the tutorial from actually starting. I dont think there's a better way to do this...
                    GameObject.Find("TutorialGameplay").SetActive(false);
                });
            }

            //fadeInOutController.FadeIn(1f, () => FadeInCallback(MenuEnvironmentManager.MenuEnvironmentType.Lobby));

            PushViewControllerToNavigationController(mainScreenNavigation, blank);
            PushViewControllerToNavigationController(settingsSelection, horizontalSettingsList);

            ProvideInitialViewControllers(mainScreenNavigation, credits, null, settingsSelection);

            RefreshAllMockCounters();
        }

        public void RefreshAllMockCounters()
        {
            foreach (ConfigModel settings in AllConfigModels)
            {
                mockCounter.UpdateMockCounter(settings);
            }
        }

        public void SetRightViewController(ViewController controller) => SetRightScreenViewController(controller, ViewController.AnimationType.In);

        public void PushToMainScreen(ViewController controller)
        {
            SetViewControllerToNavigationController(mainScreenNavigation, controller);
        }

        public void PopFromMainScreen()
        {
            SetViewControllerToNavigationController(mainScreenNavigation, blank);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
            canvasUtility.ClearAllText();

            // Return back to the main menu.
            gameScenesManager.PopScenes(0.25f, null, (_) =>
            { 
                // Unmark these scenes as persistent so they won't bother us in-game.
                persistentScenes.Remove("MenuViewControllers");
                persistentScenes.Remove("MenuCore");
                persistentScenes.Remove("Menu");
                fadeInOutController.FadeIn();
            });
        }
    }
}
