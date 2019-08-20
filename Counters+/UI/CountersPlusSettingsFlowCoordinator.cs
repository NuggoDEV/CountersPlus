using CountersPlus.Config;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRUI;
using CountersPlus.UI.ViewControllers;

namespace CountersPlus.UI
{
    class CountersPlusSettingsFlowCoordinator : FlowCoordinator
    {
        private Vector3 MainScreenPosition;
        private GameObject MainScreen;
        private BackButton navigationController;
        private CountersPlusFillerForMainViewController placeholder;
        private CountersPlusEditViewController editSettings;
        //private CountersPlusSettingsListViewController settingsList;
        private CountersPlusCreditsViewController credits;
        private CountersPlusHorizontalSettingsListViewController horizSettingsList;
        internal static CountersPlusSettingsFlowCoordinator Instance;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            MainScreen = GameObject.Find("MainScreen");
            MainScreenPosition = MainScreen.transform.position;
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                Instance = this;
                title = "Counters+";

                navigationController = BeatSaberUI.CreateViewController<BackButton>();
                navigationController.DidFinishEvent += BackButton_DidFinish;
                
                editSettings = BeatSaberUI.CreateViewController<CountersPlusEditViewController>();
                placeholder = BeatSaberUI.CreateViewController<CountersPlusFillerForMainViewController>();
                //settingsList = BeatSaberUI.CreateViewController<CountersPlusSettingsListViewController>();
                horizSettingsList = BeatSaberUI.CreateViewController<CountersPlusHorizontalSettingsListViewController>();
                credits = BeatSaberUI.CreateViewController<CountersPlusCreditsViewController>();
            }
            SetViewControllersToNavigationConctroller(navigationController, new VRUIViewController[] { credits });
            ProvideInitialViewControllers(placeholder, navigationController, editSettings, horizSettingsList);
            MainScreen.transform.position = new Vector3(0, -100, 0); //"If it works it's not stupid"
            
            CounterWarning.CreateWarning("Due to limitations, some counters may not reflect their true appearance in-game.", 7.5f);
            if (!Plugin.UpToDate) CounterWarning.CreateWarning("A new Counters+ update is available to download!", 5);
            StartCoroutine(InitMockCounters());
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            if (deactivationType == DeactivationType.RemovedFromHierarchy)
                PopViewControllerFromNavigationController(navigationController);
        }

        private IEnumerator InitMockCounters()
        {
            yield return new WaitForEndOfFrame();
            MockCounterInfo info = new MockCounterInfo();
            MockCounter.CreateStatic("Combo", $"{info.notesCut}");
            MockCounter.CreateStatic("Multiplier", "x8");
            StartCoroutine(UpdateMockCountersRoutine());
        }

        internal static void UpdateMockCounters()
        {
            Instance.StartCoroutine(UpdateMockCountersRoutine());
        }

        private static IEnumerator UpdateMockCountersRoutine()
        {
            bool allCountersActive = false;
            while (!allCountersActive)
            {
                int loaded = 0;
                foreach (SettingsInfo counter in CountersPlusHorizontalSettingsListViewController.Instance.counterInfos)
                {
                    try
                    {
                        MockCounter.Update(counter.Model);
                        loaded++;
                    }
                    catch { } //Mainly from custom counters, no biggie.
                }
                if (loaded == CountersPlusHorizontalSettingsListViewController.Instance.counterInfos.Count) allCountersActive = true;
                yield return new WaitForEndOfFrame();
            }
        }

        private void BackButton_DidFinish()
        {
            foreach (KeyValuePair<MockCounterGroup, ConfigModel> kvp in MockCounter.loadedMockCounters)
            {
                Destroy(kvp.Key.CounterName);
                Destroy(kvp.Key.CounterData);
            }
            CountersPlusEditViewController.ClearScreen();
            MockCounter.loadedMockCounters.Clear();
            CounterWarning.ClearAllWarnings();
            Destroy(TextHelper.CounterCanvas.gameObject);
            TextHelper.CounterCanvas = null;
            MainScreen.transform.position = MainScreenPosition;
            MainFlowCoordinator mainFlow = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            mainFlow.InvokeMethod("DismissFlowCoordinator", this, null, false);
            foreach (CounterWarning warning in CounterWarning.existing) DestroyImmediate(warning.gameObject);
        }
    }
}
