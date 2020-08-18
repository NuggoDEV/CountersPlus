using CountersPlus.ConfigModels;
using CountersPlus.UI.FlowCoordinators;
using HMUI;
using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.UI.SettingGroups
{
    public class CountersSettingsGroup : SettingsGroup
    {
        [Inject] protected LazyInject<CountersPlusSettingsFlowCoordinator> flowCoordinator;

        public override TableCell CellForIdx(TableView view, int idx)
        {
            AnnotatedBeatmapLevelCollectionTableCell cell = view.DequeueReusableCellForIdentifier(settingsList.Value.ReuseIdentifier) as AnnotatedBeatmapLevelCollectionTableCell;
            if (cell == null) //Dequeue the cell, and make an instance if it doesn't exist.
            {
                cell = UnityEngine.Object.Instantiate(settingsList.Value.levelPackTableCellInstance);
                cell.reuseIdentifier = settingsList.Value.ReuseIdentifier;
            }
            cell.showNewRibbon = false; //Dequeued cells will keep NEW ribbon value. Always change it to false.
            TextMeshProUGUI packInfoText = packInfoTextAccessor(ref cell);
            packInfoText.richText = true; //Enable rich text for info text. Optional, but I use it for Counters+.
            UnityEngine.UI.Image packCoverImage = coverImageAccessor(ref cell);
            packCoverImage.mainTexture.wrapMode = TextureWrapMode.Clamp; //Fixes bordering on images (especially transparent ones)

            ConfigModel model = flowCoordinator.Value.AllConfigModels[idx];
            packInfoText.text = $"{model.DisplayName}";
            try
            {
                packCoverImage.sprite = LoadSprite($"Counters.{model.DisplayName}");
            }
            catch { }
            return cell;
        }

        public override int NumberOfCells() => flowCoordinator.Value.AllConfigModels.Count;

        public override void OnCellSelect(TableView view, int idx)
        {
            Plugin.Logger.Warn($"Selected {flowCoordinator.Value.AllConfigModels[idx].DisplayName}!");
        }
    }
}
