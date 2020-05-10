using CountersPlus.Config;
using BeatSaberMarkupLanguage;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HMUI;
using CountersPlus.UI.ViewControllers;
using CountersPlus.Utils;
using CountersPlus.Custom;
using CountersPlus.UI.ViewControllers.ConfigModelControllers;

namespace CountersPlus.UI
{
    class CountersPlusSettingsFlowCoordinator : FlowCoordinator
    {
        private Vector3 MainScreenPosition;
        private GameObject MainScreen;
        private CountersPlusFillerForMainViewController placeholder;
        private CountersPlusEditViewController editSettings;
        private CountersPlusCreditsViewController credits;
        private CountersPlusHorizontalSettingsListViewController horizSettingsList;
        private CountersPlusBottomSettingsSelectorViewController bottomSettings;

        internal static CountersPlusSettingsFlowCoordinator Instance;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            MainScreen = GameObject.Find("MainScreen");
            MainScreenPosition = MainScreen.transform.position;
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                Instance = this;
                title = "Counters+";
                showBackButton = true;
                
                editSettings = BeatSaberUI.CreateViewController<CountersPlusEditViewController>();
                placeholder = BeatSaberUI.CreateViewController<CountersPlusFillerForMainViewController>();
                horizSettingsList = BeatSaberUI.CreateViewController<CountersPlusHorizontalSettingsListViewController>();
                bottomSettings = BeatSaberUI.CreateViewController<CountersPlusBottomSettingsSelectorViewController>();
                credits = BeatSaberUI.CreateViewController<CountersPlusCreditsViewController>();
            }
            PushViewControllerToNavigationController(bottomSettings, horizSettingsList);
            ProvideInitialViewControllers(placeholder, credits, editSettings, bottomSettings);
            MainScreen.transform.position = new Vector3(0, -100, 0); //"If it works it's not stupid"
            
            CounterWarning.Create("Due to limitations, some counters may not reflect their true appearance in-game.", 7.5f);
            if (!Plugin.UpToDate) CounterWarning.Create("A new Counters+ update is available to download!", 5);
            InitMockCounters();
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
        }

        private void InitMockCounters()
        {
            MockCounter.CreateStatic("Combo", $"0");
            MockCounter.CreateStatic("Multiplier", "x1");
            UpdateMockCounters();
        }

        internal static void UpdateMockCounters()
        {
            MockCounter.ClearAllMockCounters();
            if (TextHelper.CounterCanvas != null) Destroy(TextHelper.CounterCanvas.gameObject);
            TextHelper.CounterCanvas = null;

            List<ConfigModel> loadedModels = TypesUtility.GetListOfType<ConfigModel>();
            loadedModels = loadedModels.Where(x => !(x is CustomConfigModel)).ToList();
            loadedModels.ForEach(x => x = ConfigLoader.DeserializeFromConfig(x, x.DisplayName) as ConfigModel);
            foreach (CustomCounter potential in CustomCounterCreator.LoadedCustomCounters)
                loadedModels.Add(potential.ConfigModel);
            loadedModels.RemoveAll(x => x is null);

            foreach (ConfigModel counter in loadedModels) MockCounter.Update(counter);
        }

        protected override void BackButtonWasPressed(ViewController controller)
        {
            MockCounter.ClearAllMockCounters();
            CountersPlusEditViewController.ClearScreen();
            CounterWarning.ClearAllWarnings();
            Destroy(TextHelper.CounterCanvas.gameObject);
            TextHelper.CounterCanvas = null;
            MainScreen.transform.position = MainScreenPosition;
            MainFlowCoordinator mainFlow = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            mainFlow.InvokePrivateMethod("DismissFlowCoordinator", new object[] { this, null, false });
            ConfigModelController.ClearAllControllers();

            //Reload settings from config
            CountersController.settings = ConfigLoader.LoadSettings();
        }
    }
}
