using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.ViewControllers;
using HMUI;
using System;
using Zenject;

namespace CountersPlus.UI.SettingGroups
{
    // TODO implement
    public class HUDsSettingsGroup : SettingsGroup
    {
        [Inject] private LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;
        [Inject] private LazyInject<CountersPlusUnimplementedViewController> unimplemented;

        public override void OnEnable()
        {
            flowCoordinator.Value.PushToMainScreen(unimplemented.Value);
        }

        public override void OnDisable()
        {
            flowCoordinator.Value.PopFromMainScreen();
        }

        public override TableCell CellForIdx(TableView view, int idx)
        {
            throw new NotImplementedException();
        }

        public override int NumberOfCells() => 0;

        public override void OnCellSelect(TableView view, int idx)
        {
        }
    }
}
