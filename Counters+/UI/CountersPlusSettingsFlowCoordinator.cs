using CustomUI.BeatSaber;
using CustomUI.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using VRUI;

namespace CountersPlus.UI
{
    class CountersPlusSettingsFlowCoordinator : FlowCoordinator
    {
        private BackButton navigationController;
        private BackButton filler;
        private CountersPlusSettingsListViewController settingsList;
        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                title = "Counters+";

                navigationController = BeatSaberUI.CreateViewController<BackButton>();
                navigationController.didFinishEvent += backButton_DidFinish;

                filler = BeatSaberUI.CreateViewController<BackButton>();

                settingsList = BeatSaberUI.CreateViewController<CountersPlusSettingsListViewController>();
            }

            SetViewControllersToNavigationConctroller(navigationController, new VRUIViewController[]
            {
                settingsList
            });
            ProvideInitialViewControllers(filler, navigationController, null);
            GameObject.Find("MainScreen").GetComponent<Canvas>().enabled = false;
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            if (deactivationType == DeactivationType.RemovedFromHierarchy)
            {
                PopViewControllerFromNavigationController(navigationController);
            }
        }

        private void backButton_DidFinish()
        {
            MainFlowCoordinator mainFlow = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            GameObject.Find("MainScreen").GetComponent<Canvas>().enabled = true;
            mainFlow.InvokeMethod("DismissFlowCoordinator", this, null, false);
        }
    }
}
