using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using CountersPlus.UI.ViewControllers;
using CountersPlus.Utils;
using HMUI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace CountersPlus.UI.FlowCoordinators
{
    public class CountersPlusSettingsFlowCoordinator : FlowCoordinator
    {
        private readonly Vector3 MAIN_SCREEN_OFFSET = new Vector3(0, -4, 0);

        [Inject] public List<ConfigModel> AllConfigModels;
        [Inject] private CanvasUtility canvasUtility;
        [Inject] private MockCounter mockCounter;

        [Inject] private CountersPlusCreditsViewController credits;
        [Inject] private CountersPlusBlankViewController blank;
        [Inject] private CountersPlusMainScreenNavigationController mainScreenNavigation;
        [Inject] private CountersPlusSettingSectionSelectionViewController settingsSelection;
        [Inject] private CountersPlusHorizontalSettingsListViewController horizontalSettingsList;

        private TweenPosition mainScreenTween;
        private Vector3 mainScreenOrigin;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                Transform mainScreenTransform = GameObject.Find("MainScreen").transform;
                mainScreenTween = mainScreenTransform.gameObject.AddComponent<TweenPosition>();
                mainScreenTween._duration = 1f;
                mainScreenOrigin = mainScreenTransform.position;

                Plugin.Logger.Warn($"Loading options menu with {AllConfigModels.Count} config models...");
                Plugin.Logger.Warn($"{Assembly.GetExecutingAssembly().GetName().Name}");

                showBackButton = true;
                title = "Counters+";
            }
            SetMainScreenOffset(MAIN_SCREEN_OFFSET);

            PushViewControllerToNavigationController(mainScreenNavigation, blank);
            PushViewControllerToNavigationController(settingsSelection, horizontalSettingsList);

            ProvideInitialViewControllers(mainScreenNavigation, credits, null, settingsSelection);

            foreach (ConfigModel settings in AllConfigModels)
            {
                mockCounter.UpdateMockCounter(settings);
            }
        }

        public void SetRightViewController(ViewController controller) => SetRightScreenViewController(controller);

        public void PushToMainScreen(ViewController controller)
        {
            SetViewControllerToNavigationController(mainScreenNavigation, controller);
            SetMainScreenOffset(Vector3.zero);
        }

        public void PopFromMainScreen()
        {
            SetViewControllerToNavigationController(mainScreenNavigation, blank);
            if (mainScreenNavigation.viewControllers.Count <= 2)
            {
                SetMainScreenOffset(MAIN_SCREEN_OFFSET);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            SetMainScreenOffset(Vector3.zero);
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
            canvasUtility.ClearAllText();
        }

        internal void SetMainScreenOffset(Vector3 offset, float duration = 1)
        {
            mainScreenTween._duration = duration;
            mainScreenTween.TargetPos = mainScreenOrigin + offset;
        }
    }
}
