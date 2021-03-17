using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.ViewControllers.Editing;
using HMUI;
using Zenject;

namespace CountersPlus.UI.SettingGroups
{
    public class MainSettingsGroup : SettingsGroup
    {
        [Inject] private LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;
        [Inject] private LazyInject<CountersPlusMainSettingsEditViewController> mainSettings;

        public override void OnEnable() => flowCoordinator.Value.PushToMainScreen(mainSettings.Value);

        public override void OnDisable() => flowCoordinator.Value.PopFromMainScreen();

        public override int NumberOfCells() => 0;

        public override void OnCellSelect(TableView view, int idx) { }
    }
}
