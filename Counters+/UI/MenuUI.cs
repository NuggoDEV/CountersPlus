using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;

namespace CountersPlus.UI
{
    public class MenuUI
    {
        private static CountersPlusSettingsFlowCoordinator settingsFC;

        public static void CreateUI()
        {
            MenuButton button = new MenuButton("Counters+", "Configure Counters+ settings.", OnClick);
            MenuButtons.instance.RegisterButton(button);
        }

        internal static void OnClick()
        {
            if (settingsFC == null)
                settingsFC = BeatSaberUI.CreateFlowCoordinator<CountersPlusSettingsFlowCoordinator>();
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(settingsFC, null, false, false);
        }
    }
}
