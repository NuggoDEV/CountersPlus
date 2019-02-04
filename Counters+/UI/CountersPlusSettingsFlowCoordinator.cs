using CountersPlus.Config;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRUI;

namespace CountersPlus.UI
{
    class CountersPlusSettingsFlowCoordinator : FlowCoordinator
    {
        private Vector3 MainScreenPosition;
        private GameObject MainScreen;
        private BackButton navigationController;
        private CountersPlusFillerForMainViewController placeholder;
        private CountersPlusEditViewController editSettings;
        private CountersPlusSettingsListViewController settingsList;
        internal static CountersPlusSettingsFlowCoordinator Instance;

        //For mock counters
        public List<GameObject> mockCounters = new List<GameObject>();
        private MockCounterInfo info = new MockCounterInfo();
        private bool createMocks = true;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            MainScreen = GameObject.Find("MainScreen");
            MainScreenPosition = MainScreen.transform.position;
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                Instance = this;
                title = "Counters+";

                navigationController = BeatSaberUI.CreateViewController<BackButton>();
                navigationController.didFinishEvent += backButton_DidFinish;

                editSettings = BeatSaberUI.CreateViewController<CountersPlusEditViewController>();
                placeholder = BeatSaberUI.CreateViewController<CountersPlusFillerForMainViewController>();
                settingsList = BeatSaberUI.CreateViewController<CountersPlusSettingsListViewController>();
            }
            SetViewControllersToNavigationConctroller(navigationController, new VRUIViewController[]
            {
                settingsList
            });
            ProvideInitialViewControllers(placeholder, navigationController, editSettings);
            MainScreen.transform.position = new Vector3(0, -100, 0); //"If it works it's not stupid"

            //For mock counters
            createMocks = true;
            new GameObject("Counters+ | Counters Warning").AddComponent<CounterWarning>();
            StartCoroutine(UpdateMockCounters());
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            if (deactivationType == DeactivationType.RemovedFromHierarchy)
            {
                PopViewControllerFromNavigationController(navigationController);
            }
        }

        private IEnumerator UpdateMockCounters()
        {
            while (createMocks)
            {
                foreach (GameObject counter in mockCounters)
                {
                    Destroy(counter);
                }
                MockCounter.Create(CountersController.settings.missedConfig, "Combo", "0", false);
                MockCounter.Create(CountersController.settings.missedConfig, "Multiplier", "x8", false);
                MockCounter.Create(CountersController.settings.missedConfig, "Misses", info.notesMissed.ToString());
                MockCounter.Create(CountersController.settings.noteConfig, "Notes", //Sorry for this mess of a line
                    $"{info.notesCut - info.notesMissed} / {info.notesCut} {((CountersController.settings.noteConfig.ShowPercentage) ? $"- ({Math.Round((((double)(info.notesCut - info.notesMissed) / info.notesCut) * 100), CountersController.settings.noteConfig.DecimalPrecision)}%)" : "")}");
                MockCounter.Create(CountersController.settings.scoreConfig, $"{Math.Round(info.score, CountersController.settings.scoreConfig.DecimalPrecision).ToString()}%", CountersController.settings.scoreConfig.DisplayRank ? info.GetRank() : "");
                if (CountersController.settings.scoreConfig.Mode == ICounterMode.BaseWithOutScore || CountersController.settings.scoreConfig.Mode == ICounterMode.LeaveScore)
                    MockCounter.Create(CountersController.settings.scoreConfig, "123456", "", false);
                if (CountersController.settings.speedConfig.Mode == ICounterMode.Average || CountersController.settings.speedConfig.Mode == ICounterMode.Both)
                {
                    MockCounter.Create(CountersController.settings.speedConfig,
                        $"{(CountersController.settings.speedConfig.Mode == ICounterMode.Both ? "Average (Both)" : "Average Speed")}", $"{Math.Round((info.leftSpeedAverage + info.rightSpeedAverage) / 2, CountersController.settings.speedConfig.DecimalPrecision)}");
                }else if (CountersController.settings.speedConfig.Mode == ICounterMode.SplitAverage || CountersController.settings.speedConfig.Mode == ICounterMode.SplitBoth)
                {
                    MockCounter.Create(CountersController.settings.speedConfig,
                        $"{(CountersController.settings.speedConfig.Mode == ICounterMode.SplitBoth ? "Split Average (Both)" : "Split Average")}", $"{Math.Round(info.leftSpeedAverage, CountersController.settings.speedConfig.DecimalPrecision)} | {Math.Round(info.rightSpeedAverage, CountersController.settings.speedConfig.DecimalPrecision)}");
                }else if (CountersController.settings.speedConfig.Mode == ICounterMode.Top5Sec)
                {
                    MockCounter.Create(CountersController.settings.speedConfig, "Top Speed (5 Sec.)", $"{Math.Round(info.leftSpeedAverage + 10, CountersController.settings.speedConfig.DecimalPrecision)}");
                }
                MockCounter.Create(CountersController.settings.cutConfig, "Average Cut", $"{Mathf.RoundToInt(info.averageCutScore)}");
                MockCounter.Create(CountersController.settings.progressConfig,
                    $"{CountersController.settings.progressConfig.Mode.ToString()} Progress",
                        $"{((((CountersController.settings.progressConfig.ProgressTimeLeft ? 1f : 0f) - ((float)info.timeElapsed / info.totalTime)) * 100f) * (CountersController.settings.progressConfig.ProgressTimeLeft ? 1f : -1f)).ToString("00")}%");
                try
                {
                    foreach (SettingsInfo info in CountersPlusSettingsListViewController.Instance.counterInfos)
                    {
                        if (info.IsCustom) MockCounter.Create(info.Model, "", info.Name);
                    }
                }
                catch { }
                yield return new WaitForSeconds(0.25f);
            }
        }

        private void backButton_DidFinish()
        {
            createMocks = false;
            StopCoroutine(UpdateMockCounters());
            foreach(GameObject counter in mockCounters)
            {
                Destroy(counter);
            }
            MainScreen.transform.position = MainScreenPosition;
            MainFlowCoordinator mainFlow = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            mainFlow.InvokeMethod("DismissFlowCoordinator", this, null, false);
        }
    }
}
