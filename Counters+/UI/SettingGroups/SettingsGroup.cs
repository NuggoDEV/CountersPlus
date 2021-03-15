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

        public abstract int NumberOfCells();

        public abstract TableCell CellForIdx(TableView view, int idx);

        public virtual CustomListTableData.CustomCellInfo CellInfoForIdx(int idx)
            => new CountersPlusListTableCell(idx, "Unimplemented", LoadSprite("MainSettings"));

        public abstract void OnCellSelect(TableView view, int idx);

        public virtual void OnDisable() { }

        public virtual void OnEnable() { }

        public virtual float GetSize() => 30f;

        public virtual int CellToSelect() => 0;

        protected void GetCell(TableView view, out CountersPlusListTableCell cell, out TextMeshProUGUI packText, out ImageView image)
        {
            cell = null;
            packText = null;
            image = null;
        }

        protected Texture2D LoadTexture(string name) => ImagesUtility.LoadTextureFromResources($"CountersPlus.UI.Images.{name}.png");

        protected Sprite LoadSprite(string name) => ImagesUtility.LoadSpriteFromResources($"CountersPlus.UI.Images.{name}.png");
    }
}
