using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.ViewControllers;
using HMUI;
using System;
using UnityEngine;
using Zenject;

namespace CountersPlus.UI.SettingGroups
{
    // TODO implement
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

        public override float GetSize() => 49f;

        public override TableCell CellForIdx(TableView view, int idx)
        {
            GetCell(view, out var cell, out var packInfoText, out var packCoverImage);
            
            switch (idx)
            {
                case 0:
                    packCoverImage.sprite = LoadSprite("HUDs.Add");
                    packInfoText.text = "Add new Canvas";
                    break;
                case 1:
                    packCoverImage.sprite = LoadSprite("HUDs.Manage");
                    packInfoText.text = "Edit existing Canvases";
                    break;
                case 2:
                    packCoverImage.sprite = LoadSprite("HUDs.Remove");
                    packInfoText.text = "Remove a Canvas";
                    break;
            }

            return cell;
        }

        public override int NumberOfCells() => 3;

        public override void OnCellSelect(TableView view, int idx)
        {
            switch (idx)
            {
                case 0:
                    flowCoordinator.Value.SetRightViewController(null);
                    flowCoordinator.Value.SetMainScreenOffset(Vector3.zero);
                    hudList.Value.IsDeleting = false;
                    hudList.Value.CreateNewCanvasDialog();
                    break;
                case 1:
                    hudList.Value.DeactivateModals();
                    hudList.Value.ClearSelection();
                    flowCoordinator.Value.SetMainScreenOffset(Vector3.zero);
                    hudList.Value.IsDeleting = false;
                    break;
                case 2:
                    hudList.Value.DeactivateModals();
                    hudList.Value.ClearSelection();
                    flowCoordinator.Value.SetMainScreenOffset(Vector3.zero);
                    flowCoordinator.Value.SetRightViewController(null);
                    hudList.Value.IsDeleting = true;
                    break;
            }
        }
    }
}
