using CustomUI.BeatSaber;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Attributes;

namespace CountersPlus.UI.ViewControllers
{
    class CountersPlusCreditsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CountersPlus.UI.BSML.Credits.bsml";

        [UIComponent("versionText")] private TextMeshProUGUI version = null;
        [UIComponent("creatorText")] private TextMeshProUGUI creator = null;
        [UIComponent("updateText")] private TextMeshProUGUI update = null;
        [UIComponent("linkOpenedText")] private TextMeshProUGUI linkOpened = null;
        [UIComponent("github")] private Button github = null;
        [UIComponent("donate")] private Button donate = null;
        [UIComponent("issues")] private Button issues = null;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);
            if (!firstActivation) return;

            version.transform.parent.localPosition = new Vector3(0, 2.5f, 0);
            version.text = $"Version <color={(Plugin.UpToDate ? "#00FF00" : "#FF0000")}>{Plugin.PluginVersion}</color>";
            creator.text = "Developed by: <color=#00c0ff>Caeden117</color>";
            update.text = $"<color=#FF0000>Version {Plugin.WebVersion} available for download!</color>";

            if (Plugin.UpToDate) update.gameObject.SetActive(false);
            linkOpened.gameObject.SetActive(false);

            github.onClick.AddListener(() => GoTo("https://github.com/Caeden117/CountersPlus", github));
            issues.onClick.AddListener(() => GoTo("https://github.com/Caeden117/CountersPlus/issues", issues));
            donate.onClick.AddListener(() => GoTo("https://ko-fi.com/Caeden117", donate));
        }

        private void GoTo(string url, Button button)
        {
            Plugin.Log("Opened a link to: " + url);
            button.interactable = false;
            linkOpened.gameObject.SetActive(true);
            StartCoroutine(SecondRemove(button));
            System.Diagnostics.Process.Start(url);
        }

        private IEnumerator SecondRemove(Button button)
        {
            yield return new WaitForSeconds(5);
            linkOpened.gameObject.SetActive(false);
            button.interactable = true;
        }
    }
}
