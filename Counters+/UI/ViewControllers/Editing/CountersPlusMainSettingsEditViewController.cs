using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using CountersPlus.UI.FlowCoordinators;
using HMUI;
using System.Reflection;
using Zenject;

namespace CountersPlus.UI.ViewControllers.Editing
{
    public class CountersPlusMainSettingsEditViewController : ViewController
    {
        private readonly string SettingsBase = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "CountersPlus.UI.BSML.MainSettings.bsml");

        [Inject] private MainConfigModel mainConfig;
        [Inject] private LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (firstActivation) BSMLParser.instance.Parse(SettingsBase, gameObject, mainConfig);
            mainConfig.OnConfigChanged += MainConfig_OnConfigChanged;
        }

        private void MainConfig_OnConfigChanged()
        {
            flowCoordinator.Value.RefreshAllMockCounters();
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemEnabling)
        {
            mainConfig.OnConfigChanged -= MainConfig_OnConfigChanged;
        }
    }
}
