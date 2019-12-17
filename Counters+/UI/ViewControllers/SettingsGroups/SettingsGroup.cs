using HMUI;

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
        public abstract SettingsGroupType type { get; }

        public abstract int NumberOfCells();
        public virtual float CellSize() => 30f;
        public abstract TableCell CellForIdx(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings);
        public abstract void OnCellSelect(TableView view, int row, CountersPlusHorizontalSettingsListViewController settings);
    }
}
