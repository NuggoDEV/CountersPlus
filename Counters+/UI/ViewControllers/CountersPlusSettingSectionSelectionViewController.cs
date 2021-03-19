using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CountersPlus.UI.SettingGroups;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Zenject;

namespace CountersPlus.UI.ViewControllers
{
    public class CountersPlusSettingSectionSelectionViewController : BSMLResourceViewController
    {
        internal Action<int> OnSettingsGroupChanged;

        [Inject] private List<SettingsGroup> loadedSettingsGroups = new List<SettingsGroup>();

        [UIComponent("list")] private CustomCellListTableData tableList;
        [UIComponent("right-button")] private Button leftButton;
        [UIComponent("left-button")] private Button rightButton;

        private SettingsGroup selectedGroup;

        public override string ResourceName => "CountersPlus.UI.BSML.SettingsSectionSelection.bsml";

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            HandleCellSelectedEvent(null, (selectedGroup != null) ? loadedSettingsGroups.IndexOf(selectedGroup) : 0);
        }

        [UIAction("#post-parse")]
        private void Parsed()
        {
            var scrollView = tableList.tableView.GetField<ScrollView, TableView>("_scrollView");
            scrollView.SetField("_pageUpButton", rightButton);
            scrollView.SetField("_pageDownButton", leftButton);
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

            tableList.cellSize = selectedGroup.GetSize();

            for (var i = 0; i < selectedGroup.NumberOfCells(); i++)
            {
                tableList.data.Add(selectedGroup.CellInfoForIdx(i));
            }

            tableList.tableView.ReloadData();

            tableList.tableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, false);
            tableList.tableView.SelectCellWithIdx(selectedGroup.CellToSelect(), false);
        }
    }
}
