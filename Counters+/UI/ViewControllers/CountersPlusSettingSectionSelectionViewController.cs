using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using System;

namespace CountersPlus.UI.ViewControllers
{
    public class CountersPlusSettingSectionSelectionViewController : NavigationController
    {
        internal Action<int> OnSettingsGroupChanged;

        private TabSelector tab;

        public string ResourceName => "CountersPlus.UI.BSML.SettingsSectionSelection.bsml";

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            // I need to mimick the functionality of a BSMLResourceViewController but on a NavigationController.
            if (firstActivation)
            {
                BSMLParser.instance.Parse(Utilities.GetResourceContent(GetType().Assembly, ResourceName), gameObject, this);
            }
            tab = gameObject.GetComponentInChildren<TabSelector>();
            tab.textSegmentedControl.didSelectCellEvent += HandleCellSelectedEvent;
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
