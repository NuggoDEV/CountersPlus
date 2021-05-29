using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using CountersPlus.UI.ViewControllers;
using CountersPlus.UI.ViewControllers.Editing;
using CountersPlus.Utils;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRUIControls;
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
        [Inject] private MenuEnvironmentManager menuEnvironmentManager;
        [Inject] private GameScenesManager gameScenesManager;
        [Inject] private FadeInOutController fadeInOutController;
        [Inject] private VRInputModule vrInputModule;
        [Inject] private MenuShockwave menuShockwave;

        [Inject] private CountersPlusCreditsViewController credits;
        [Inject] private CountersPlusBlankViewController blank;
        [Inject] private CountersPlusMainScreenNavigationController mainScreenNavigation;
        [Inject] private CountersPlusSettingSectionSelectionViewController settingsSelection;
        [Inject] private SettingsFlowCoordinator settingsFlowCoordinator;
        [Inject] private SongPreviewPlayer songPreviewPlayer;
        
        private MainSettingsModelSO mainSettings;
        private HashSet<string> persistentScenes = new HashSet<string>();
        private bool hasTransitioned = false;

        private ScreenSystem screenSystem;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (addedToHierarchy)
            {
                showBackButton = true;
                SetTitle("Counters+");
            }

            screenSystem = IPA.Utilities.ReflectionUtil.GetField<ScreenSystem, FlowCoordinator>(this, "_screenSystem");
            //screenSystem.backButtonWasPressedEvent += HandleBackButtonWasPressed;

            ProvideInitialViewControllers(mainScreenNavigation, credits, null, settingsSelection);

            RefreshAllMockCounters();
        }

        public void DoSceneTransition(Action callback = null)
        {
            if (hasTransitioned) return;

            hasTransitioned = true;

            persistentScenes = GSMPersistentScenes(ref gameScenesManager); // Get our hashset of persistent scenes
            mainSettings = SFCMainSettingsModel(ref settingsFlowCoordinator);

            // Make sure our menu persists through the transition
            persistentScenes.Add("MenuCore");

            var tutorialSceneSetup = MTHTutorialScenesSetup(ref menuTransitionsHelper); // Grab the scene transition setup data
            tutorialSceneSetup.Init();

            menuEnvironmentManager.ShowEnvironmentType(MenuEnvironmentManager.MenuEnvironmentType.None);

            // We're actually transitioning to the Tutorial sequence, but disabling the tutorial itself from starting.
            gameScenesManager.PushScenes(tutorialSceneSetup, 0.25f, null, (_) =>
            {
                // god this makes me want to retire from beat saber modding
                static void DisableAllNonImportantObjects(Transform original, Transform source, IEnumerable<string> importantObjects)
                {
                    foreach (Transform child in source)
                    {
                        if (importantObjects.Contains(child.name))
                        {
                            Transform loopback = child;
                            while (loopback != original)
                            {
                                loopback.gameObject.SetActive(true);
                                loopback = loopback.parent;
                            }
                        }
                        else
                        {
                            child.gameObject.SetActive(false);
                            DisableAllNonImportantObjects(original, child, importantObjects);
                        }
                    }
                }


                // Disable the tutorial from actually starting. I dont think there's a better way to do this...
                Transform tutorial = GameObject.Find("TutorialGameplay").transform;

                // Gotta do some jank to re-activate the Root Container
                menuEnvironmentManager.transform.root.gameObject.SetActive(true);

                if (mainSettings.screenDisplacementEffectsEnabled)
                {
                    // Disable menu shockwave to forget about rendering order problems
                    menuShockwave.gameObject.SetActive(false);
                }

                // Reset menu audio to original state.
                songPreviewPlayer.CrossfadeToDefault();

                // When not in FPFC, disable the Menu input, and re-enable the Tutorial menu input
                if (!Environment.GetCommandLineArgs().Any(x => x.ToLowerInvariant() == "fpfc") &&
                    !Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>().Any(x => x.isActiveAndEnabled))
                {
                    vrInputModule.gameObject.SetActive(false);

                    DisableAllNonImportantObjects(tutorial, tutorial, new string[]
                    {
                            "EventSystem",
                            "ControllerLeft",
                            "ControllerRight"
                    });
                }
                else // If we're in FPFC, just disable everything as usual.
                {
                    DisableAllNonImportantObjects(tutorial, tutorial, Array.Empty<string>());
                    vrInputModule.enabled = true;
                }

                callback?.Invoke();
            });
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
            mainScreenNavigation.AssignViewController(controller);
            //if (hasTransitioned)
            //{
            //    PresentViewController(controller);
            //}
        }

        public void PopFromMainScreen()
        {
            mainScreenNavigation.AssignViewController(blank);
            //PresentViewController(blank, null, ViewController.AnimationDirection.Horizontal, true);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            hasTransitioned = false;
            canvasUtility.ClearAllText();
            screenSystem.backButtonWasPressedEvent -= HandleBackButtonWasPressed;
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this, TransitionToMenu, ViewController.AnimationDirection.Horizontal, true);
        }

        private void HandleBackButtonWasPressed() => BackButtonWasPressed(topViewController);

        private void TransitionToMenu()
        {
            vrInputModule.gameObject.SetActive(true);

            // This took a long time to figure out.
            if (mainSettings.screenDisplacementEffectsEnabled)
            {
                menuShockwave.gameObject.SetActive(true);
            }

            // Return back to the main menu.
            gameScenesManager.PopScenes(0.25f, null, (_) =>
            {
                // Unmark these scenes as persistent so they won't bother us in-game.
                persistentScenes.Remove("MenuCore");
                menuEnvironmentManager.ShowEnvironmentType(MenuEnvironmentManager.MenuEnvironmentType.Default);
                fadeInOutController.FadeIn();
                songPreviewPlayer.CrossfadeToDefault();
            });
        }
    }
}
