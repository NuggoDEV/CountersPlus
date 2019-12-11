using HMUI;
using TMPro;
using UnityEngine;

namespace CountersPlus.UI.ViewControllers.SettingsGroups
{
    class HUDSettingsGroup : SettingsGroup
    {
        public override SettingsGroupType type => SettingsGroupType.HUD;

        public override TableCell CellForIdx(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings)
        {
            LevelPackTableCell cell = view.DequeueReusableCellForIdentifier(settings.ReuseIdentifier) as LevelPackTableCell;
            if (cell == null) //Dequeue the cell, and make an instance if it doesn't exist.
            {
                cell = Object.Instantiate(settings.levelPackTableCellInstance);
                cell.reuseIdentifier = settings.ReuseIdentifier;
            }
            cell.showNewRibbon = false; //Dequeued cells will keep NEW ribbon value. Always change it to false.
            
            switch (row)
            {
                case 0:
                    SetCellInfo(ref cell, "General HUD Settings", "Yep.", "Custom");
                    break;
                case 1:
                    SetCellInfo(ref cell, "Lock HUD to Camera", "Yep.", "Custom");
                    break;
                case 2:
                    SetCellInfo(ref cell, "HUD Position Offset", "Yep.", "Custom");
                    break;
                case 3:
                    SetCellInfo(ref cell, "HUD Rotation Offset", "Yep.", "Custom");
                    break;
            }
            return cell;
        }

        private void SetCellInfo(ref LevelPackTableCell cell, string name, string info, string imageResource, bool newRibbon = false)
        {
            TextMeshProUGUI packNameText = cell.GetPrivateField<TextMeshProUGUI>("_packNameText"); //Grab some TMP references.
            TextMeshProUGUI packInfoText = cell.GetPrivateField<TextMeshProUGUI>("_infoText");
            packInfoText.richText = true; //Enable rich text for info text. Optional, but I use it for Counters+.
            UnityEngine.UI.Image packCoverImage = cell.GetPrivateField<UnityEngine.UI.Image>("_coverImage");
            packCoverImage.mainTexture.wrapMode = TextureWrapMode.Clamp; //Fixes bordering on images (especially transparent ones)
            cell.showNewRibbon = newRibbon;

            packNameText.text = name;
            packInfoText.text = info;
            packCoverImage.sprite = Images.Images.LoadSprite(imageResource);
        }

        public override int NumberOfCells() => 4;
        public override float CellSize() => 36;

        public override void OnCellSelect(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings)
        {
        }
    }
}
