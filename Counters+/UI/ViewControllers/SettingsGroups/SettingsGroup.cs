using IPA.Utilities;
using TMPro;
using HMUI;
using UnityEngine.UI;

namespace CountersPlus.UI.ViewControllers.SettingsGroups
{
    public enum SettingsGroupType
    {
        Main,
        HUD,
        Counters
    }

    public abstract class SettingsGroup
    {
        protected static FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, TextMeshProUGUI>.Accessor packInfoTextAccessor = FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, TextMeshProUGUI>.GetAccessor("_infoText");
        protected static FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, Image>.Accessor coverImageAccessor = FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, Image>.GetAccessor("_coverImage");

        public abstract SettingsGroupType type { get; }

        public abstract int NumberOfCells();
        public virtual float CellSize() => 30f;
        public abstract TableCell CellForIdx(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings);
        public abstract void OnCellSelect(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings);
    }
}
