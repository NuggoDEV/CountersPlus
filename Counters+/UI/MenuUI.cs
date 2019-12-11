using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using UnityEngine;

namespace CountersPlus.UI
{
    public class MenuUI
    {
        static CountersPlusSettingsFlowCoordinator settingsFC;
        public static void CreateUI()
        {
            MenuButton button = new MenuButton("Counters+", "Configure Counters+ settings.", OnClick);
            MenuButtons.instance.RegisterButton(button);
        }

        internal static void OnClick()
        {
            if (settingsFC == null)
                settingsFC = BeatSaberUI.CreateFlowCoordinator<CountersPlusSettingsFlowCoordinator>();
            MainFlowCoordinator main = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            main.InvokePrivateMethod("PresentFlowCoordinator", new object[] { settingsFC, null, false, false });
        }
    }
}
