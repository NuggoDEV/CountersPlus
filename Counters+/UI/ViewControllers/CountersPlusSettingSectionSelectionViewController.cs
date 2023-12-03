using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CountersPlus.UI.SettingGroups;
using HMUI;
using IPA.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CountersPlus.UI.ViewControllers
{
    public class CountersPlusSettingSectionSelectionViewController : BSMLResourceViewController
    {
        [Inject] private List<SettingsGroup> loadedSettingsGroups = new List<SettingsGroup>();

        [UIComponent("list")] private CustomCellListTableData tableList;
        [UIComponent("left-button")] private Button leftButton;
        [UIComponent("right-button")] private Button rightButton;

        private ScrollView scrollView;

        private SettingsGroup selectedGroup;

        public override string ResourceName => "CountersPlus.UI.BSML.SettingsSectionSelection.bsml";

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

            scrollView = tableList.tableView.GetField<ScrollView, TableView>("_scrollView");
            scrollView.scrollPositionChangedEvent += ScrollView_scrollPositionChangedEvent;

            HandleCellSelectedEvent(null, (selectedGroup != null) ? loadedSettingsGroups.IndexOf(selectedGroup) : 0);
        }

        [UIAction("#post-parse")]
        private void Parsed()
        {
            var colors = rightButton.colors;
            colors.disabledColor = Color.black;
            rightButton.colors = colors;
            leftButton.colors = colors;
        }

        private void ScrollView_scrollPositionChangedEvent(float obj)
        {
            // TODO convert into accessors
            var destinationPos = scrollView.GetField<float, ScrollView>("_destinationPos");
            var contentSize = scrollView.GetProperty<float, ScrollView>("contentSize");
            var scrollPageSize = scrollView.GetProperty<float, ScrollView>("scrollPageSize");

            leftButton.interactable = destinationPos > 0.001f;
            rightButton.interactable = destinationPos < contentSize - scrollPageSize - 0.001f;
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
            scrollView.scrollPositionChangedEvent -= ScrollView_scrollPositionChangedEvent;
        }

        [UIAction("option-selected")]
        private void OptionSelected(TableView view, CountersPlusListTableCell cell)
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

            CustomListTableData.CustomCellInfo[] items = new CustomListTableData.CustomCellInfo[selectedGroup.NumberOfCells()];
            for (var i = 0; i < selectedGroup.NumberOfCells(); i++)
            {
                items[i] = selectedGroup.CellInfoForIdx(i);
            }
            tableList.data = items;

            tableList.tableView.ReloadData();

            tableList.tableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, false);
            tableList.tableView.SelectCellWithIdx(selectedGroup.CellToSelect(), false);

            ScrollView_scrollPositionChangedEvent(0f);
        }
    }
}
