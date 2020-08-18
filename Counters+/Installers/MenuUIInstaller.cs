using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.Utils;
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

            Container.Bind<CanvasUtility>().AsSingle().NonLazy();

            flowCoordinator = BeatSaberUI.CreateFlowCoordinator<CountersPlusSettingsFlowCoordinator>();
            Container.InjectSpecialInstance<CountersPlusSettingsFlowCoordinator>(flowCoordinator);

            Plugin.Logger.Warn(string.Join(",", Container.AllContracts.Select(x => x.Type.Name)));

            MenuButton button = new MenuButton("Counters+", "Configure Counters+ settings.", OnClick);
            MenuButtons.instance.RegisterButton(button);
        }

        private void OnClick()
        {
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator);
        }
    }
}
