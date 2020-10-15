using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using CountersPlus.ConfigModels;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.ViewControllers.Editing;
using CountersPlus.Utils;
using HMUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CountersPlus.UI.ViewControllers
{
    class CountersPlusHUDListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CountersPlus.UI.BSML.HUDs.HUDList.bsml";

        public bool IsDeleting = false;
        public int SelectedCanvas { get; private set; } = -1;

        [UIComponent("list")] private CustomListTableData data;
        [UIComponent("new-canvas-name")] private ModalKeyboard newCanvasKeyboard;
        [UIComponent("delete-canvas")] private ModalView deleteCanvas;
        [UIComponent("canvas-error")] private ModalView canvasError;
        [UIParams] private BSMLParserParams parserParams;

        [Inject] private HUDConfigModel hudConfig;
        [Inject] private MainConfigModel mainConfig;
        [Inject] private CanvasUtility canvasUtility;
        [Inject] private LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;
        [Inject] private LazyInject<CountersPlusHUDEditViewController> hudEdit;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            RefreshData();
            IsDeleting = false;
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemEnabling)
        {
            DeactivateModals();
        }

        protected void RefreshData()
        {
            data.data.Clear();
            for (int i = -1; i < hudConfig.OtherCanvasSettings.Count; i++)
            {
                HUDCanvas settings = canvasUtility.GetCanvasSettingsFromID(i);
                int countersUsingCanvas = flowCoordinator.Value.AllConfigModels.Count(x => x.CanvasID == i);
                var info = new CustomListTableData.CustomCellInfo(
                    settings?.Name ?? "Unknown",
                    $"<i>{countersUsingCanvas} counter(s) use this Canvas.</i>", Utilities.LoadSpriteFromTexture(Texture2D.blackTexture));
                data.data.Add(info);
            }
            data.tableView.ReloadData();
        }

        public void CreateNewCanvasDialog()
        {
            flowCoordinator.Value.SetRightViewController(null);
            DeactivateModals();
            newCanvasKeyboard.modalView.Show(true);
        }

        public void DeactivateModals() => parserParams.EmitEvent("on-deactivate");

        public void ClearSelection() => data.tableView.ClearSelection();

        [UIAction("cell-selected")]
        private void CellSelected (TableView view, int idx)
        {
            SelectedCanvas = idx - 1;
            if (IsDeleting)
            {
                flowCoordinator.Value.SetRightViewController(null);
                DeactivateModals();
                if (idx == 0)
                {
                    canvasError.Show(true);
                }
                else
                {
                    deleteCanvas.Show(true);
                }
            }
            else
            {
                flowCoordinator.Value.SetRightViewController(hudEdit.Value);
                hudEdit.Value.ApplyCanvasForEditing(SelectedCanvas);
            }
        }

        [UIAction("create-new-canvas")]
        private void CreateNewCanvas(string name)
        {
            HUDCanvas settings = new HUDCanvas();
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name)) name = "New Canvas";
            settings.Name = name;
            canvasUtility.RegisterNewCanvas(settings, hudConfig.OtherCanvasSettings.Count);
            hudConfig.OtherCanvasSettings.Add(settings);
            mainConfig.HUDConfig = hudConfig;
            RefreshData();
        }

        [UIAction("delete-selected-canvas")]
        private void DeleteSelectedCanvas()
        {
            DeactivateModals();
            if (SelectedCanvas == -1) return;
            IEnumerable<ConfigModel> needToUpdate = flowCoordinator.Value.AllConfigModels.Where(x => x.CanvasID == SelectedCanvas);
            for (int i = 0; i < needToUpdate.Count(); i++)
            {
                needToUpdate.ElementAt(i).CanvasID = -1;
            }
            canvasUtility.UnregisterCanvas(SelectedCanvas);
            hudConfig.OtherCanvasSettings.RemoveAt(SelectedCanvas);
            mainConfig.HUDConfig = hudConfig;
            SelectedCanvas--;
            RefreshData();
            flowCoordinator.Value.RefreshAllMockCounters();
        }

        [UIAction("cancel-deletion")]
        private void CancelCanvasDeletion()
        {
            DeactivateModals();
        }
    }
}
