using CustomUI.BeatSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using VRUI;
using CountersPlus.Config;
using UnityEngine;
using TMPro;
using CustomUI.Settings;

namespace CountersPlus.UI
{
    class CountersPlusEditViewController : VRUIViewController
    {
        public static CountersPlusEditViewController Instance;
        public event Action didFinishEvent;
        private static RectTransform rect;
        private static TextMeshProUGUI settingsTitle;
        private static SubMenu container;
        private static IConfigModel model;
        private static List<GameObject> loadedElements = new List<GameObject>();

        private Button _backButton;


        static Action<RectTransform, float, float, float, float, float> setStuff = delegate (RectTransform r, float x, float y, float w, float h, float pivotX)
        {
            r.anchorMin = new Vector2(x, y);
            r.anchorMax = new Vector2(x + w, y + h);
            r.pivot = new Vector2(pivotX, 1);
            r.sizeDelta = Vector2.zero;
            r.anchoredPosition = Vector2.zero;
        };

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            rect = rectTransform;
            if (firstActivation)
            {
                Instance = this;
                CreateCredits();


                _backButton = BeatSaberUI.CreateBackButton(rect, didFinishEvent.Invoke);
            }
        }

        private static void CreateCredits()
        {
            TextMeshProUGUI name, version, creator, contributorLabel;
            Dictionary<string, string> contributors = new Dictionary<string, string>()
            {
                { "Moon", "Bug fixing and code optimization" },
                { "Shoko84", "Bug fixing" },
                { "xhuytox", "Big helper in bug hunting - thanks man!" },
                { "Brian", "Saving Beat Saber Modding with CustomUI" },
                { "Kyle1413", "Beat Saber Utils and for Progress/Score Counter code" },
                { "Ragesaq", "Speed Counter idea <i>(and some bug fixing on stream)</i>" },
                { "Stackeror", "Creator of the original Progress Counter mod" },
                { "BSMG", "Getting me into Modding" },
                { "You", "For enjoying this mod!"},
            }; 
            name = BeatSaberUI.CreateText(rect, "Counters+", Vector2.zero);
            name.fontSize = 10;
            name.alignment = TextAlignmentOptions.Center;
            Plugin.Log(name.characterSpacing.ToString());
            setStuff(name.rectTransform, 0, 0.8f, 1, 0.166f, 0.5f);

            version = BeatSaberUI.CreateText(rect, $"Version {Plugin.Instance.Version}", Vector2.zero);
            version.fontSize = 3;
            version.alignment = TextAlignmentOptions.Center;
            setStuff(version.rectTransform, 0, 0.73f, 1, 0.166f, 0.5f);

            creator = BeatSaberUI.CreateText(rect, "Developed by: <color=#00c0ff>Caeden117</color>", Vector2.zero);
            creator.fontSize = 5;
            creator.alignment = TextAlignmentOptions.Center;
            setStuff(creator.rectTransform, 0, 0.585f, 1, 0.166f, 0.5f);

            contributorLabel = BeatSaberUI.CreateText(rect, "Thanks to these contributors for helping make Counters+ what it is!", Vector2.zero);
            contributorLabel.fontSize = 3;
            contributorLabel.alignment = TextAlignmentOptions.Center;
            setStuff(contributorLabel.rectTransform, 0, 0.45f, 1, 0.166f, 0.5f);

            foreach(var kvp in contributors)
            {
                TextMeshProUGUI contributor = BeatSaberUI.CreateText(rect, $"<color=#00c0ff>{kvp.Key}</color> | {kvp.Value}", Vector2.zero);
                contributor.fontSize = 3;
                contributor.alignment = TextAlignmentOptions.Left;
                setStuff(contributor.rectTransform, 0.15f,
                    0.4f - (contributors.Keys.ToList().IndexOf(kvp.Key) * 0.05f), 1, 0.166f, 0.5f);
                loadedElements.Add(contributor.gameObject);
            }

            loadedElements.AddRange(new GameObject[] { name.gameObject, version.gameObject, creator.gameObject, contributorLabel.gameObject});
        }

        public static void UpdateSettings<T>(T settings, bool isMain = false) where T : IConfigModel
        {
            foreach (GameObject element in loadedElements)
            {
                Destroy(element);
            }
            loadedElements.Clear();
            model = settings;
            container = new SubMenu(rect);
            settingsTitle = BeatSaberUI.CreateText(rect, $"{(isMain ? "Main" : settings.DisplayName)} Settings", Vector2.zero);
            settingsTitle.fontSize = 6;
            settingsTitle.alignment = TextAlignmentOptions.Center;
            setStuff(settingsTitle.rectTransform, 0, 0.85f, 1, 0.166f, 0.5f);

            loadedElements.Add(settingsTitle.gameObject);
        }
    }
}