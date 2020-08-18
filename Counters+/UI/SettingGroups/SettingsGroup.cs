using CountersPlus.UI.ViewControllers;
using HMUI;
using TMPro;
using Zenject;
using UnityEngine.UI;
using IPA.Utilities;

namespace CountersPlus.UI.SettingGroups
{
    /// <summary>
    /// Helper class that makes it easy to dynamically the content of a <see cref="CountersPlusHorizontalSettingsListViewController"/>
    /// </summary>
    public abstract class SettingsGroup
    {
        protected static FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, TextMeshProUGUI>.Accessor packInfoTextAccessor = FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, TextMeshProUGUI>.GetAccessor("_infoText");
        protected static FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, Image>.Accessor coverImageAccessor = FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, Image>.GetAccessor("_coverImage");
        
        [Inject] protected LazyInject<CountersPlusHorizontalSettingsListViewController> settingsList;

        public virtual float GetSize() => 30f;

        public abstract int NumberOfCells();

        public abstract TableCell CellForIdx(TableView view, int idx);

        public abstract void OnCellSelect(TableView view, int idx);
    }
}
