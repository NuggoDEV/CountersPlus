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
        static CountersPlusSettingsFlowCoordinator settingsFC;
        public static VRUIViewController left;
        public static VRUIViewController right;
        static bool secondClick = false;
        public static void CreateUI()
        {
            MenuButtonUI.AddButton("Counters+", onClick);
        }

        internal static void onClick()
        {
            if (settingsFC == null)
                settingsFC = new GameObject("Counters+ | Settings Flow Coordinator").AddComponent<CountersPlusSettingsFlowCoordinator>();
            MainFlowCoordinator main = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            left = main.leftScreenViewController;
            right = main.rightScreenViewController;
            main.InvokeMethod("PresentFlowCoordinator", settingsFC, null, false, false);
        }
    }
}
