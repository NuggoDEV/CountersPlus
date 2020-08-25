using CountersPlus.UI.ViewControllers;
using HMUI;
using TMPro;
using Zenject;
using UnityEngine.UI;
using UnityEngine;
using CountersPlus.Utils;
using static CountersPlus.Utils.Accessors;

namespace CountersPlus.UI.SettingGroups
{
    /// <summary>
    /// Helper class that makes it easy to dynamically the content of a <see cref="CountersPlusHorizontalSettingsListViewController"/>
    /// </summary>
    public abstract class SettingsGroup
    {
        [Inject] protected LazyInject<CountersPlusHorizontalSettingsListViewController> settingsList;

        public abstract int NumberOfCells();

        public abstract TableCell CellForIdx(TableView view, int idx);

        public abstract void OnCellSelect(TableView view, int idx);

        public virtual void OnDisable() { }

        public virtual void OnEnable() { }

        public virtual float GetSize() => 30f;

        public virtual int CellToSelect() => 0;

        protected void GetCell(TableView view, out AnnotatedBeatmapLevelCollectionTableCell cell, out TextMeshProUGUI packText, out Image image)
        {
            cell = view.DequeueReusableCellForIdentifier(settingsList.Value.ReuseIdentifier) as AnnotatedBeatmapLevelCollectionTableCell;
            if (cell == null) //Dequeue the cell, and make an instance if it doesn't exist.
            {
                cell = Object.Instantiate(settingsList.Value.levelPackTableCellInstance);
                cell.reuseIdentifier = settingsList.Value.ReuseIdentifier;
            }
            cell.showNewRibbon = false; //Dequeued cells will keep NEW ribbon value. Always change it to false.
            packText = PackInfoTextAccessor(ref cell);
            packText.richText = true; //Enable rich text for info text. Optional, but I use it for Counters+.
            image = CoverImageAccessor(ref cell);
            image.mainTexture.wrapMode = TextureWrapMode.Clamp; //Fixes bordering on images (especially transparent ones)
        }

        protected Texture2D LoadTexture(string name) => ImagesUtility.LoadTextureFromResources($"CountersPlus.UI.Images.{name}.png");

        protected Sprite LoadSprite(string name) => ImagesUtility.LoadSpriteFromResources($"CountersPlus.UI.Images.{name}.png");
    }
}
