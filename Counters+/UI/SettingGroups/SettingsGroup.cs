using HMUI;
using UnityEngine;
using CountersPlus.Utils;
using BeatSaberMarkupLanguage.Components;

namespace CountersPlus.UI.SettingGroups
{
    /// <summary>
    /// Helper class that makes it easy to dynamically the content of the bottom settings list.
    /// </summary>
    public abstract class SettingsGroup
    {
        public abstract int NumberOfCells();

        public virtual CustomListTableData.CustomCellInfo CellInfoForIdx(int idx)
            => new CountersPlusListTableCell(idx, "Unimplemented", LoadSprite("MainSettings"));

        public abstract void OnCellSelect(TableView view, int idx);

        public virtual void OnDisable() { }

        public virtual void OnEnable() { }

        public virtual float GetSize() => 20f;

        public virtual int CellToSelect() => 0;

        protected Sprite LoadSprite(string name) => ImagesUtility.LoadSpriteFromResources($"CountersPlus.UI.Images.{name}.png");
    }
}
