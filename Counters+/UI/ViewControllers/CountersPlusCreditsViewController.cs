using CustomUI.BeatSaber;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CountersPlus.UI.ViewControllers
{
    class CountersPlusCreditsViewController : VRUIViewController
    {
        private static CountersPlusCreditsViewController instance;

        static Action<RectTransform, float, float, float, float, float> setPositioning = delegate (RectTransform r, float x, float y, float w, float h, float pivotX)
        {
            r.anchorMin = new Vector2(x, y);
            r.anchorMax = new Vector2(x + w, y + h);
            r.pivot = new Vector2(pivotX, 1);
            r.sizeDelta = Vector2.zero;
            r.anchoredPosition = Vector2.zero;
        };

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (!firstActivation) return;
            instance = this;
            TextMeshProUGUI name, version, creator;
            Button github, issues, donate;
            
            name = BeatSaberUI.CreateText(rectTransform, "Counters+", Vector2.zero);
            name.fontSize = 11;
            name.alignment = TextAlignmentOptions.Center;
            name.characterSpacing = 2;
            setPositioning(name.rectTransform, 0, 0.7f, 1, 0.166f, 0.5f);

            version = BeatSaberUI.CreateText(rectTransform,
                $"Version <color={(Plugin.upToDate ? "#00FF00" : "#FF0000")}>{Plugin.PluginVersion}</color>", Vector2.zero);
            version.fontSize = 3;
            version.alignment = TextAlignmentOptions.Center;
            setPositioning(version.rectTransform, 0, 0.5f, 1, 0.166f, 0.5f);

            if (!Plugin.upToDate)
            {
                TextMeshProUGUI warning = BeatSaberUI.CreateText(rectTransform,
                $"<color=#FF0000>Version {Plugin.webVersion} available for download!</color>", Vector2.zero);
                warning.fontSize = 3;
                warning.alignment = TextAlignmentOptions.Center;
                setPositioning(warning.rectTransform, 0, 0.47f, 1, 0.166f, 0.5f);
            }

            creator = BeatSaberUI.CreateText(rectTransform, "Developed by: <color=#00c0ff>Caeden117</color>", Vector2.zero);
            creator.fontSize = 5;
            creator.alignment = TextAlignmentOptions.Center;
            setPositioning(creator.rectTransform, 0, 0.35f, 1, 0.166f, 0.5f);

            github = BeatSaberUI.CreateUIButton(rectTransform, "SettingsButton", Vector2.left, null, "GitHub");
            github.onClick.AddListener(() => { GoTo("https://github.com/Caeden117/CountersPlus", github); });
            setPositioning(github.transform as RectTransform, 0f, 0f, 0.39f, 0.125f, 0.5f);
            BeatSaberUI.AddHintText(github.transform as RectTransform, "Opens in a new browser tab on your desktop. Feel free to explore the source code! Maybe try out experimental versions?");

            issues = BeatSaberUI.CreateUIButton(rectTransform, "QuitButton", Vector2.right, null, "Report an Issue");
            issues.onClick.AddListener(() => { GoTo("https://github.com/Caeden117/CountersPlus/issues", issues); });
            setPositioning(issues.transform as RectTransform, 0.5f, 0f, 0.5f, 0.125f, 0.5f);
            BeatSaberUI.AddHintText(issues.transform as RectTransform, "Opens in a new browser tab on your desktop. Be sure to read the Issue template thoroughly!");

            donate = BeatSaberUI.CreateUIButton(rectTransform, "CreditsButton", Vector2.zero, null, "<3");
            donate.onClick.AddListener(() => { GoTo("https://ko-fi.com/Caeden117", donate); });
            BeatSaberUI.AddHintText(donate.transform as RectTransform, "Buy me a coffee if you feel like I'm deserving of one.");
            setPositioning(donate.transform as RectTransform, 0.36f, 0f, 0.17f, 0.125f, 0.5f);
        }

        private static void GoTo(string url, Button button)
        {
            Plugin.Log("Opened a link to: " + url);
            button.interactable = false;
            TextMeshProUGUI reminder = BeatSaberUI.CreateText(instance.rectTransform, "Link opened in your browser!", Vector2.zero);
            reminder.fontSize = 4;
            reminder.alignment = TextAlignmentOptions.Center;
            setPositioning(reminder.rectTransform, 0, 0.25f, 1, 0.166f, 0.5f);
            instance.StartCoroutine(instance.SecondRemove(reminder.gameObject, button));
            System.Diagnostics.Process.Start(url);
        }

        private IEnumerator SecondRemove(GameObject go, Button button)
        {
            yield return new WaitForSeconds(5);
            Destroy(go);
            button.interactable = true;
        }
    }
}
