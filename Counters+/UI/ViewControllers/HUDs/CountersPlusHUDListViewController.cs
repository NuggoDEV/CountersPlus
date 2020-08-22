using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using CountersPlus.ConfigModels;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.SettingGroups;
using CountersPlus.Utils;
using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static CountersPlus.Utils.Accessors;

namespace CountersPlus.UI.ViewControllers.HUDs
{
    class CountersPlusHUDListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CountersPlus.UI.BSML.HUDs.HUDList.bsml";

        [UIComponent("list")] private CustomListTableData data;
        [UIComponent("new-canvas-name")] private ModalKeyboard newCanvasKeyboard;
        [UIComponent("delete-canvas")] private ModalView deleteCanvas;
        [UIParams] private BSMLParserParams parserParams;

        [Inject] private HUDConfigModel hudConfig;
        [Inject] private MainConfigModel mainConfig;
        [Inject] private CanvasUtility canvasUtility;
        [Inject] private LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            RefreshData();
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            parserParams.EmitEvent("on-deactivate");
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
                    $"<i>{countersUsingCanvas} counters use this Canvas.</i>", Texture2D.blackTexture);
                data.data.Add(info);
            }
            data.tableView.ReloadData();
        }

        public void CreateNewCanvasDialog()
        {
            parserParams.EmitEvent("on-deactivate");
            newCanvasKeyboard.modalView.Show(true);
        }

        [UIAction("cell-selected")]
        private void CellSelected (TableView view, int idx)
        {
            Plugin.Logger.Warn("WOOT");
        }

        [UIAction("create-new-canvas")]
        private void CreateNewCanvas(string name)
        {
            HUDCanvas settings = new HUDCanvas();
            settings.Name = name;
            canvasUtility.RegisterNewCanvas(settings, hudConfig.OtherCanvasSettings.Count);
            hudConfig.OtherCanvasSettings.Add(settings);
            mainConfig.HUDConfig = hudConfig;
            RefreshData();
        }
    }
}
