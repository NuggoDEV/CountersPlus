using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.SettingGroups;
using CountersPlus.UI.ViewControllers;
using CountersPlus.Utils;
using HMUI;
using SiraUtil.Zenject;
using System.Linq;
using Zenject;

namespace CountersPlus.Installers
{
    public class MenuUIInstaller : MonoInstaller
    {
        private CountersPlusSettingsFlowCoordinator flowCoordinator;

        public override void InstallBindings()
        {
            // Using Zenject for UI lets goooooooooooo
            Container.BindInstance(Container).WhenInjectedInto<CountersPlusSettingsFlowCoordinator>();

            // CanvasUtility for UI
            Container.Bind<CanvasUtility>().AsSingle().NonLazy();

            BindSettingsGroup<CountersSettingsGroup>();

            BindViewController<CountersPlusCreditsViewController>();
            BindViewController<CountersPlusBlankViewController>();
            BindViewController<CountersPlusSettingSectionSelectionViewController>();
            BindViewController<CountersPlusHorizontalSettingsListViewController>();

            flowCoordinator = BeatSaberUI.CreateFlowCoordinator<CountersPlusSettingsFlowCoordinator>();
            Container.InjectSpecialInstance<CountersPlusSettingsFlowCoordinator>(flowCoordinator);

            Plugin.Logger.Warn(string.Join(",", Container.AllContracts.Select(x => x.Type.Name)));

            MenuButton button = new MenuButton("Counters+", "Configure Counters+ settings.", OnClick);
            MenuButtons.instance.RegisterButton(button);
        }

        private void BindViewController<T>() where T : ViewController
        {
            T view = BeatSaberUI.CreateViewController<T>();
            Container.InjectSpecialInstance<T>(view);
        }

        private void BindSettingsGroup<T>() where T : SettingsGroup
        {
            Container.Bind<SettingsGroup>().To<T>().AsCached().NonLazy();
        }

        private void OnClick()
        {
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator);
        }
    }
}
