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
            AnnotatedBeatmapLevelCollectionTableCell cell = view.DequeueReusableCellForIdentifier(settings.ReuseIdentifier) as AnnotatedBeatmapLevelCollectionTableCell;
            if (cell == null) //Dequeue the cell, and make an instance if it doesn't exist.
            {
                cell = Object.Instantiate(settings.levelPackTableCellInstance);
                cell.reuseIdentifier = settings.ReuseIdentifier;
            }
            cell.showNewRibbon = false; //Dequeued cells will keep NEW ribbon value. Always change it to false.
            TextMeshProUGUI packInfoText = cell.GetPrivateField<TextMeshProUGUI>("_infoText");
            packInfoText.richText = true; //Enable rich text for info text. Optional, but I use it for Counters+.
            UnityEngine.UI.Image packCoverImage = cell.GetPrivateField<UnityEngine.UI.Image>("_coverImage");
            packCoverImage.mainTexture.wrapMode = TextureWrapMode.Clamp; //Fixes bordering on images (especially transparent ones)
            switch (row)
            {
                case 0:
                    SetText("Main Settings", "Configure basic Counters+ settings.", Images.Images.LoadSprite("MainSettings"),
                        ref packInfoText, ref packCoverImage);
                    break;
                case 1:
                    SetText("Donators", "See who supported me on Ko-fi!", Images.Images.LoadSprite("Donators"),
                        ref packInfoText, ref packCoverImage);
                    break;
                case 2:
                    SetText("Contributors", "See who helped with Counters+!", Images.Images.LoadSprite("Contributors"),
                        ref packInfoText, ref packCoverImage);
                    break;
            }
            return cell;
        }


        private void SetText(string name, string desc, Sprite sprite, ref TextMeshProUGUI packInfo, ref UnityEngine.UI.Image packCover)
        {
            packInfo.text = $"{name}\n\n{desc}";
            packCover.sprite = sprite;
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
