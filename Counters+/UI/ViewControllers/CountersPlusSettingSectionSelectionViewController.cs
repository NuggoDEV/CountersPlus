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

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
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
            Plugin.Logger.Warn("Changed to section " + cellIdx);
            OnSettingsGroupChanged?.Invoke(cellIdx);
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
            tab.textSegmentedControl.didSelectCellEvent -= HandleCellSelectedEvent;
        }
    }
}
