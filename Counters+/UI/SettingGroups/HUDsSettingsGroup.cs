using BeatSaberMarkupLanguage.Components;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.ViewControllers;
using HMUI;
using Zenject;

namespace CountersPlus.UI.SettingGroups
{
    public class HUDsSettingsGroup : SettingsGroup
    {
        [Inject] private LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;
        [Inject] private LazyInject<CountersPlusHUDListViewController> hudList;

        public override void OnEnable()
        {
            flowCoordinator.Value.PushToMainScreen(hudList.Value);
        }

        public override void OnDisable()
        {
            flowCoordinator.Value.SetRightViewController(null);
            flowCoordinator.Value.PopFromMainScreen();
        }

        public override float GetSize() => 100 / 3f;

        public override int CellToSelect() => 1;

        public override CustomListTableData.CustomCellInfo CellInfoForIdx(int idx) => idx switch
            {
                0 => new CountersPlusListTableCell(idx, "Add new Canvas", LoadSprite("HUDs.Add")),
                1 => new CountersPlusListTableCell(idx, "Edit existing Canvases", LoadSprite("HUDs.Manage")),
                2 => new CountersPlusListTableCell(idx, "Remove a Canvas", LoadSprite("HUDs.Remove")),
                _ => base.CellInfoForIdx(idx),
            };

        public override int NumberOfCells() => 3;

        public override void OnCellSelect(TableView view, int idx)
        {
            switch (idx)
            {
                case 0:
                    flowCoordinator.Value.SetRightViewController(null);
                    hudList.Value.IsDeleting = false;
                    hudList.Value.CreateNewCanvasDialog();
                    break;
                case 1:
                    hudList.Value.DeactivateModals();
                    hudList.Value.ClearSelection();
                    hudList.Value.IsDeleting = false;
                    break;
                case 2:
                    hudList.Value.DeactivateModals();
                    hudList.Value.ClearSelection();
                    flowCoordinator.Value.SetRightViewController(null);
                    hudList.Value.IsDeleting = true;
                    break;
            }
        }
    }
}
