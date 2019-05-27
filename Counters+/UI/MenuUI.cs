using System.Linq;
using CustomUI.MenuButton;
using CustomUI.Utilities;
using UnityEngine;

namespace CountersPlus.UI
{
    public class MenuUI
    {
        static CountersPlusSettingsFlowCoordinator settingsFC;
        public static void CreateUI()
        {
            MenuButtonUI.AddButton("Counters+", "Configure Counters+ settings.", OnClick);
        }

        internal static void OnClick()
        {
            if (settingsFC == null)
                settingsFC = new GameObject("Counters+ | Settings Flow Coordinator").AddComponent<CountersPlusSettingsFlowCoordinator>();
            MainFlowCoordinator main = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            main.InvokeMethod("PresentFlowCoordinator", settingsFC, null, false, false);
        }
    }
}
