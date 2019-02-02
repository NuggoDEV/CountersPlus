using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomUI.BeatSaber;
using CustomUI.MenuButton;
using CustomUI.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CountersPlus.UI
{
    public class MenuUI
    {
        static CustomMenu settings;
        static CountersPlusSettingsFlowCoordinator settingsFC;
        static CountersPlusSettingsListViewController list;
        public static void CreateUI()
        {
            MenuButtonUI.AddButton("Counters+", onClick);
        }

        internal static void onClick()
        {
            if (settingsFC == null)
                settingsFC = new GameObject("Counters+ | Settings Flow Coordinator").AddComponent<CountersPlusSettingsFlowCoordinator>();
            MainFlowCoordinator main = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            main.InvokeMethod("PresentFlowCoordinator", settingsFC, null, false, false);
        }
    }
}
