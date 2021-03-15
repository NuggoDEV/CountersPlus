using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CountersPlus.UI.SettingGroups;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using Zenject;

namespace CountersPlus.UI.ViewControllers
{
    public class CountersPlusSettingSectionSelectionViewController : BSMLResourceViewController
    {
        internal Action<int> OnSettingsGroupChanged;

        [Inject] private List<SettingsGroup> loadedSettingsGroups = new List<SettingsGroup>();

        [UIComponent("list")] private CustomCellListTableData tableList;

        private SettingsGroup selectedGroup;

        private ScrollView scrollView;

        public override string ResourceName => "CountersPlus.UI.BSML.SettingsSectionSelection.bsml";

        private readonly string cellTemplate = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "CountersPlus.UI.BSML.CountersPlusCell.bsml");

        [UIAction("#post-parse")]
        private void Parsed()
        {
            //tableList.cellTemplate = cellTemplate;

            scrollView = tableList.tableView.GetField<ScrollView, TableView>("_scrollView");

            // Need to do some hard reflection bullshittery to get the ScrollView to scroll horizontally
            var directionField = scrollView.GetType().GetField("_scrollViewDirection", BindingFlags.Instance | BindingFlags.NonPublic);

            var horizontalDirection = Enum.Parse(directionField.FieldType, "Horizontal");

            directionField.SetValue(scrollView, horizontalDirection);
        }

        [UIAction("option-selected")]
        protected void OptionSelected(TableView view, CountersPlusListTableCell cell)
        {
            selectedGroup.OnCellSelect(view, cell.CellIdx);
        }

        [UIAction("select-section")]
        private void HandleCellSelectedEvent(SegmentedControl _, int cellIdx)
        {
            selectedGroup?.OnDisable();
            
            selectedGroup = loadedSettingsGroups[cellIdx];

            selectedGroup.OnEnable();

            tableList.data.Clear();

            for (var i = 0; i < selectedGroup.NumberOfCells(); i++)
            {
                tableList.data.Add(selectedGroup.CellInfoForIdx(i));
            }

            tableList.tableView.ReloadData();

            tableList.tableView.ScrollToCellWithIdx(selectedGroup.CellToSelect(), TableView.ScrollPositionType.Beginning, true);
        }
    }
}
