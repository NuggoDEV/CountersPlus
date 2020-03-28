using CountersPlus.Config;
using CountersPlus.UI.ViewControllers.ConfigModelControllers;
using HMUI;
using System;
using TMPro;
using UnityEngine;

namespace CountersPlus.UI.ViewControllers.SettingsGroups
{
    class HUDSettingsGroup : SettingsGroup
    {
        public override SettingsGroupType type => SettingsGroupType.HUD;

        public override TableCell CellForIdx(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings)
        {
            AnnotatedBeatmapLevelCollectionTableCell cell = view.DequeueReusableCellForIdentifier(settings.ReuseIdentifier) as AnnotatedBeatmapLevelCollectionTableCell;
            if (cell == null) //Dequeue the cell, and make an instance if it doesn't exist.
            {
                cell = UnityEngine.Object.Instantiate(settings.levelPackTableCellInstance);
                cell.reuseIdentifier = settings.ReuseIdentifier;
            }
            cell.showNewRibbon = false; //Dequeued cells will keep NEW ribbon value. Always change it to false.
            
            switch (row)
            {
                case 0:
                    SetCellInfo(ref cell, "General HUD Settings", "Change general settings, such as HUD size.", "HUD_General");
                    break;
                case 1:
                    SetCellInfo(ref cell, "Lock HUD to Camera", "Allows the HUD to be locked to various cameras.", "HUD_Camera");
                    break;
                case 2:
                    SetCellInfo(ref cell, "HUD Position Offset", "Changes the positional offset of the HUD.", "HUD_Position");
                    break;
                case 3:
                    SetCellInfo(ref cell, "HUD Rotation Offset", "Changes the rotational offset of the HUD.", "HUD_Rotation");
                    break;
            }
            return cell;
        }

        private void SetCellInfo(ref AnnotatedBeatmapLevelCollectionTableCell cell, string name, string info, string imageResource, bool ribbon = true)
        {
            TextMeshProUGUI packInfoText = cell.GetPrivateField<TextMeshProUGUI>("_infoText");
            packInfoText.richText = true; //Enable rich text for info text. Optional, but I use it for Counters+.
            UnityEngine.UI.Image packCoverImage = cell.GetPrivateField<UnityEngine.UI.Image>("_coverImage");
            packCoverImage.mainTexture.wrapMode = TextureWrapMode.Clamp; //Fixes bordering on images
            cell.showNewRibbon = ribbon;

            packInfoText.text = $"{name}\n\n{info}";
            packCoverImage.sprite = Images.Images.LoadSprite(imageResource);
        }

        public override int NumberOfCells() => 4;
        public override float CellSize() => 36;

        public override void OnCellSelect(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings)
        {
            CountersPlusEditViewController.ClearScreen(true);
            MockCounter.Highlight<ConfigModel>(null);
            switch (row)
            {
                case 0:
                    RefreshScreen("General", "General HUD Settings");
                    break;
                case 1:
                    RefreshScreen("Camera", "HUD Camera Settings");
                    break;
                case 2:
                    RefreshScreen("Position", "HUD Position Offset");
                    break;
                case 3:
                    RefreshScreen("Rotation", "HUD Rotation");
                    break;
            }
        }

        private void RefreshScreen(string name, string title)
        {
            CountersPlusEditViewController.UpdateTitle(title);
            Type controllerType = Type.GetType($"CountersPlus.UI.ViewControllers.ConfigModelControllers.HUD.{name}");
            ConfigModelController.GenerateController(controllerType,
                CountersPlusEditViewController.Instance.SettingsContainer, $"CountersPlus.UI.BSML.HUD.{name}.bsml");
        }
    }
}
