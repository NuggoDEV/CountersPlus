using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;

namespace CountersPlus.UI.ViewControllers
{
    public class CountersPlusSettingSectionSelectionViewController : BSMLResourceViewController
    {
        internal Action<int> OnSettingsGroupChanged;

        private TabSelector tab;

        [UIComponent("list")]
        private CustomListTableData tableList;

        [UIValue("#text")]
        private string text => "A";

        public override string ResourceName => "CountersPlus.UI.BSML.SettingsSectionSelection.bsml";

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            //tab = gameObject.GetComponentInChildren<TabSelector>();
            //tab.textSegmentedControl.didSelectCellEvent += HandleCellSelectedEvent;
        }

        [UIAction("#post-parse")]
        private void Parsed()
        {
            Plugin.Logger.Info("william gay");
            tableList.data.Add(new CustomListTableData.CustomCellInfo("Hi Twitch!"));
        }

        [UIAction("option-selected")]
        protected void OptionSelected(TableView _, int id)
        {
        }

        private void HandleCellSelectedEvent(SegmentedControl control, int cellIdx)
        {
            OnSettingsGroupChanged?.Invoke(cellIdx);
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemEnabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemEnabling);
            tab.textSegmentedControl.didSelectCellEvent -= HandleCellSelectedEvent;
        }
    }
}
