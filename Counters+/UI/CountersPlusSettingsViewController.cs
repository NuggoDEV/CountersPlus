using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomUI.Utilities;
using CustomUI.BeatSaber;
using VRUI;
using UnityEngine;
using UnityEngine.UI;
using CustomUI.Settings;
using System.Collections;

namespace CountersPlus.UI
{
    class CountersPlusSettingsViewController : VRUIViewController
    {
        public Button apply;
        public Button back;
        public Action backPressed;

        private SettingsNavigationController nav;
        private SimpleSettingsController[] settings;
        private static int something;
        private GameObject uiGO;
        private SettingsUI ui;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                Plugin.Log("Custom Settings created.");
                if (GameObject.Find("SettingsUI"))
                {
                    uiGO = new GameObject("Counters+ | Settings UI");
                    ui = GameObject.Find("SettingsUI").GetComponent<SettingsUI>();
                    StartCoroutine(WaitForInit());
                }
            }
        }

        IEnumerator WaitForInit()
        {
            while (true)
            {
                if (ui.GetPrivateField<bool>("initialized") == true)
                {
                    ReflectionUtil.CopyComponent(GameObject.Find("SettingsUI").GetComponent<SettingsUI>(), typeof(SettingsUI), typeof(SettingsUI), uiGO);
                    ui = uiGO.GetComponent<SettingsUI>();
                    ModifySettings();
                    break;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void ModifySettings()
        {
            Plugin.Log("Copied SettingsUI. Modifying...");
            Transform others = ui.GetPrivateField<Transform>("othersSubmenu");
            others.SetPrivateField("_viewController", this);
        }
    }
}
