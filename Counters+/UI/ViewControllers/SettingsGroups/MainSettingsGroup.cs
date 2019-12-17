using HMUI;
using TMPro;
using UnityEngine;

namespace CountersPlus.UI.ViewControllers.SettingsGroups
{
    class MainSettingsGroup : SettingsGroup
    {
        public override SettingsGroupType type => SettingsGroupType.Main;

        public override TableCell CellForIdx(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings)
        {
            LevelPackTableCell cell = view.DequeueReusableCellForIdentifier(settings.ReuseIdentifier) as LevelPackTableCell;
            if (cell == null) //Dequeue the cell, and make an instance if it doesn't exist.
            {
                cell = Object.Instantiate(settings.levelPackTableCellInstance);
                cell.reuseIdentifier = settings.ReuseIdentifier;
            }
            cell.showNewRibbon = false; //Dequeued cells will keep NEW ribbon value. Always change it to false.
            TextMeshProUGUI packNameText = cell.GetPrivateField<TextMeshProUGUI>("_packNameText"); //Grab some TMP references.
            TextMeshProUGUI packInfoText = cell.GetPrivateField<TextMeshProUGUI>("_infoText");
            packInfoText.richText = true; //Enable rich text for info text. Optional, but I use it for Counters+.
            UnityEngine.UI.Image packCoverImage = cell.GetPrivateField<UnityEngine.UI.Image>("_coverImage");
            packCoverImage.mainTexture.wrapMode = TextureWrapMode.Clamp; //Fixes bordering on images (especially transparent ones)
            switch (row)
            {
                case 0:
                    packNameText.text = "Main Settings";
                    packInfoText.text = "Configure basic Counters+ settings.";
                    packCoverImage.sprite = Images.Images.LoadSprite("MainSettings");
                    break;
                case 1:
                    packNameText.text = "Donators";
                    packInfoText.text = "See who supported me on Ko-fi!";
                    packCoverImage.sprite = Images.Images.LoadSprite("Donators");
                    break;
                case 2:
                    packNameText.text = "Contributors";
                    packInfoText.text = "See who helped with Counters+!";
                    packCoverImage.sprite = Images.Images.LoadSprite("Contributors");
                    break;
            }
            return cell;
        }

        public override int NumberOfCells() => 3;

        public override float CellSize() => 49f;

        public override void OnCellSelect(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings)
        {
            switch (row)
            {
                case 0:
                    CountersPlusEditViewController.ShowMainSettings();
                    break;
                case 1:
                    CountersPlusEditViewController.ShowDonators();
                    break;
                case 2:
                    CountersPlusEditViewController.ShowContributors();
                    break;
            }
        }
    }
}
