using CountersPlus.UI.ViewControllers;
using HMUI;
using TMPro;
using Zenject;
using UnityEngine.UI;
using UnityEngine;
using CountersPlus.Utils;
using static CountersPlus.Utils.Accessors;
using IPA.Utilities;
using BeatSaberMarkupLanguage.Components;

namespace CountersPlus.UI.SettingGroups
{
    /// <summary>
    /// Helper class that makes it easy to dynamically the content of a <see cref="CountersPlusHorizontalSettingsListViewController"/>
    /// </summary>
    public abstract class SettingsGroup
    {
        [Inject] protected LazyInject<CountersPlusHorizontalSettingsListViewController> settingsList;

        private CountersPlusListTableCell countersPlusTableCellInstance = null;

        public abstract int NumberOfCells();

        public abstract TableCell CellForIdx(TableView view, int idx);

        public virtual CustomListTableData.CustomCellInfo CellInfoForIdx(int idx)
            => new CustomListTableData.CustomCellInfo("Unimplemented", "If you see this, lists work!");

        public abstract void OnCellSelect(TableView view, int idx);

        public virtual void OnDisable() { }

        public virtual void OnEnable() { }

        public virtual float GetSize() => 30f;

        public virtual int CellToSelect() => 0;

        protected void GetCell(TableView view, out CountersPlusListTableCell cell, out TextMeshProUGUI packText, out ImageView image)
        {
            cell = view.DequeueReusableCellForIdentifier(settingsList.Value.ReuseIdentifier) as CountersPlusListTableCell;
            if (cell == null) //Dequeue the cell, and make an instance if it doesn't exist.
            {
                if (countersPlusTableCellInstance == null)
                {
                    var original = Object.Instantiate(settingsList.Value.levelPackTableCellInstance);
                    countersPlusTableCellInstance = original.CopyComponent<CountersPlusListTableCell>(original.gameObject);
                    Object.Destroy(original.GetComponent<AnnotatedBeatmapLevelCollectionTableCell>());
                    cell = countersPlusTableCellInstance;
                    cell.name = $"{nameof(CountersPlusListTableCell)}";
                }
                else
                {
                    cell = Object.Instantiate(countersPlusTableCellInstance);
                }
                cell.reuseIdentifier = settingsList.Value.ReuseIdentifier;
            }
            cell.showNewRibbon = false; //Dequeued cells will keep NEW ribbon value. Always change it to false.

            var annotatedCell = cell as AnnotatedBeatmapLevelCollectionTableCell;

            packText = PackInfoTextAccessor(ref annotatedCell);
            packText.richText = true; //Enable rich text for info text. Optional, but I use it for Counters+.
            image = CoverImageAccessor(ref annotatedCell) as ImageView;
            image.rectTransform.localPosition = Vector3.up * 0.25f;
            image.rectTransform.localScale = Vector3.one * 0.8f;
            image.preserveAspect = true; // oh boy beat games why must you make me do this
            ImageViewSkewAccessor(ref image) = 0;
            ImageViewSkewAccessor(ref SelectionImageAccessor(ref annotatedCell)) = 0;
            image.mainTexture.wrapMode = TextureWrapMode.Clamp; //Fixes bordering on images (especially transparent ones)
        }

        protected Texture2D LoadTexture(string name) => ImagesUtility.LoadTextureFromResources($"CountersPlus.UI.Images.{name}.png");

        protected Sprite LoadSprite(string name) => ImagesUtility.LoadSpriteFromResources($"CountersPlus.UI.Images.{name}.png");
    }
}
