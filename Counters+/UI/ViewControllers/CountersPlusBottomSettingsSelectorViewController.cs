using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using CountersPlus.UI.ViewControllers.SettingsGroups;
using HMUI;

namespace CountersPlus.UI.ViewControllers
{
    class CountersPlusBottomSettingsSelectorViewController : NavigationController
    {
        internal static Action<SettingsGroupType> SettingsGroupChanged;

        private TabSelector tab;

        public string ResourceName => "CountersPlus.UI.BSML.BottomSettingsList.bsml";

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            if (firstActivation)
                BSMLParser.instance.Parse(Utilities.GetResourceContent(GetType().Assembly, ResourceName), gameObject, this);
            tab = gameObject.GetComponentInChildren<TabSelector>();
            tab.textSegmentedControl.didSelectCellEvent += HandleCellSelectedEvent;
        }

        private void HandleCellSelectedEvent(SegmentedControl control, int cell)
        {
            SettingsGroupChanged?.Invoke((SettingsGroupType)cell);
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            tab.textSegmentedControl.didSelectCellEvent -= HandleCellSelectedEvent;
            base.DidDeactivate(deactivationType);
        }
    }
}
