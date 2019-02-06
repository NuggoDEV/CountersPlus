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
using CountersPlus.Custom;
using System.Threading;
using BS_Utils.Gameplay;

namespace CountersPlus.UI
{
    class CountersPlusEditViewController : VRUIViewController
    {
        public static CountersPlusEditViewController Instance;
        private static RectTransform rect;
        private static TextMeshProUGUI settingsTitle;
        private static SubMenu container;
        
        private static List<GameObject> loadedElements = new List<GameObject>(); //Mass clearing
        private static List<ListSettingsController> loadedSettings = new List<ListSettingsController>(); //Mass initialization
        private static int settingsCount = 0; //Spacing

        internal class PositionSettingsViewController : TupleViewController<Tuple<ICounterPositions, string>> { }
        static List<Tuple<ICounterPositions, string>> positions = new List<Tuple<ICounterPositions, string>> {
            {ICounterPositions.BelowCombo, "Below Combo" },
            {ICounterPositions.AboveCombo, "Above Combo" },
            {ICounterPositions.BelowMultiplier, "Below Multi." },
            {ICounterPositions.AboveMultiplier, "Above Multi." },
            {ICounterPositions.BelowEnergy, "Below Energy" },
            {ICounterPositions.AboveHighway, "Over Highway" }
        };

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
            }
        }

        private static void CreateCredits()
        {
            ClearScreen();
            TextMeshProUGUI name, version, creator, contributorLabel;
            Dictionary<string, string> contributors = new Dictionary<string, string>()
            {
                { "Moon", "Bug fixing and code optimization" },
                { "Shoko84", "Bug fixing" },
                { "xhuytox", "Big helper in bug hunting - thanks man!" },
                { "Brian", "Saving Beat Saber Modding with CustomUI" },
                { "Assistant", "Stole some Custom Avatars UI code to help with settings" },
                { "Kyle1413", "Beat Saber Utils and for Progress/Score Counter code" },
                { "Ragesaq", "Speed Counter idea <i>(and some bug fixing on stream)</i>" },
                { "SkyKiwiTV", "Original version checking code"},
                { "Stackeror", "Creator of the original Progress Counter mod" },
                { GetUserInfo.GetUserName(), "For enjoying this mod!"}, //Teehee :)
            }; 
            name = BeatSaberUI.CreateText(rect, "Counters+", Vector2.zero);
            name.fontSize = 10;
            name.alignment = TextAlignmentOptions.Center;
            name.characterWidthAdjustment = 2;
            setStuff(name.rectTransform, 0, 0.8f, 1, 0.166f, 0.5f);

            version = BeatSaberUI.CreateText(rect,
                $"Version <color={(Plugin.upToDate ? "#00FF00" : "#FF0000")}>{Plugin.Instance.Version}</color>", Vector2.zero);
            version.fontSize = 3;
            version.alignment = TextAlignmentOptions.Center;
            setStuff(version.rectTransform, 0, 0.73f, 1, 0.166f, 0.5f);

            if (!Plugin.upToDate)
            {
                TextMeshProUGUI warning = BeatSaberUI.CreateText(rect,
                $"<color=#FF0000>Version {Plugin.webVersion} available for download!</color>", Vector2.zero);
                warning.fontSize = 3;
                warning.alignment = TextAlignmentOptions.Center;
                setStuff(warning.rectTransform, 0, 0.7f, 1, 0.166f, 0.5f);
            }

            creator = BeatSaberUI.CreateText(rect, "Developed by: <color=#00c0ff>Caeden117</color>", Vector2.zero);
            creator.fontSize = 5;
            creator.alignment = TextAlignmentOptions.Center;
            setStuff(creator.rectTransform, 0, 0.64f, 1, 0.166f, 0.5f);

            contributorLabel = BeatSaberUI.CreateText(rect, "Thanks to these contributors for, directly or indirectly, helping make Counters+ what it is!", Vector2.zero);
            contributorLabel.fontSize = 3;
            contributorLabel.alignment = TextAlignmentOptions.Center;
            setStuff(contributorLabel.rectTransform, 0, 0.55f, 1, 0.166f, 0.5f);

            foreach(var kvp in contributors)
            {
                TextMeshProUGUI contributor = BeatSaberUI.CreateText(rect, $"<color=#00c0ff>{kvp.Key}</color> | {kvp.Value}", Vector2.zero);
                contributor.fontSize = 3;
                contributor.alignment = TextAlignmentOptions.Left;
                setStuff(contributor.rectTransform, 0.15f,
                    0.5f - (contributors.Keys.ToList().IndexOf(kvp.Key) * 0.05f), 1, 0.166f, 0.5f);
                loadedElements.Add(contributor.gameObject);
            }

            loadedElements.AddRange(new GameObject[] { name.gameObject, version.gameObject, creator.gameObject, contributorLabel.gameObject});
        }

        public static void UpdateSettings<T>(T settings, SettingsInfo info, bool isMain = false, bool isCredits = false) where T : IConfigModel
        {
            try
            {
                ClearScreen();
                if (info.IsCustom)
                {
                    container = CreateBase(settings, (settings as CustomConfigModel).RestrictedPositions);
                }
                else if (!isMain)
                {
                    SubMenu sub = CreateBase(settings);
                    AdvancedCounterSettings.counterUIItems.Where(
                        (KeyValuePair<IConfigModel, Action<SubMenu>> x) => (x.Key.DisplayName == settings.DisplayName)
                        ).First().Value(sub);
                }
                if (!isCredits)
                {
                    settingsTitle = BeatSaberUI.CreateText(rect, $"{(isMain ? "Main" : settings.DisplayName)} Settings", Vector2.zero);
                    settingsTitle.fontSize = 6;
                    settingsTitle.alignment = TextAlignmentOptions.Center;
                    setStuff(settingsTitle.rectTransform, 0, 0.85f, 1, 0.166f, 0.5f);
                    loadedElements.Add(settingsTitle.gameObject);
                    if (isMain)
                    {
                        SubMenu sub = new SubMenu(rect);
                        var enabled = AddList(ref sub, "Enabled", "Toggles Counters+ on or off.", 2);
                        enabled.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                        enabled.GetValue = () => CountersController.settings.Enabled ? 1f : 0f;
                        enabled.SetValue = (v) => CountersController.settings.Enabled = v != 0f;

                        var toggleCounters = AddList(ref sub, "Advanced Mock Counters", "Allows the mock counters to display more settings. To increase preformance, and reduce chances of bugs, disable this option.", 2);
                        toggleCounters.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                        toggleCounters.GetValue = () => CountersController.settings.RefreshCounterInfo ? 1f : 0f;
                        toggleCounters.SetValue = (v) => CountersController.settings.RefreshCounterInfo = v != 0f;

                        var comboOffset = AddList(ref sub, "Combo Offset", "How far from the Combo counters should be before Index is taken into account.", 10);
                        comboOffset.GetTextForValue = (v) => (v / 10).ToString();
                        comboOffset.GetValue = () => CountersController.settings.ComboOffset * 10;
                        comboOffset.SetValue = (v) => CountersController.settings.ComboOffset = (v / 10);

                        var multiOffset = AddList(ref sub, "Multiplier Offset", "How far from the Multiplier counters should be before Index is taken into account.", 10);
                        multiOffset.GetTextForValue = (v) => (v / 10).ToString();
                        multiOffset.GetValue = () => CountersController.settings.MultiplierOffset * 10;
                        multiOffset.SetValue = (v) => CountersController.settings.MultiplierOffset = (v / 10);

                        foreach (ListViewController list in loadedSettings)
                        {
                            list.Init();
                        }
                    }
                }
                else
                {
                    CreateCredits();
                }
            }
            catch { }
        }

        private static SubMenu CreateBase<T>(T settings, params ICounterPositions[] restricted) where T : IConfigModel
        {
            SubMenu sub = new SubMenu(rect);
            List<Tuple<ICounterPositions, string>> restrictedList = new List<Tuple<ICounterPositions, string>>();
            try
            {
                foreach (ICounterPositions pos in restricted)
                {
                    restrictedList.Add(Tuple.Create(pos, positions.Where((Tuple<ICounterPositions, string> x) => x.Item1 == pos).First().Item2));
                }
            }
            catch { } //It most likely errors here. If it does, well no problem.

            var enabled = AddList(ref sub, "Enabled", "Toggles this counter on or off.", 2);
            enabled.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
            enabled.GetValue = () => settings.Enabled ? 1f : 0f;
            enabled.SetValue = (v) => settings.Enabled = v != 0f;

            var position = AddList(ref sub, "Position", "The relative position of common UI elements", (restrictedList.Count() == 0) ? positions.Count() : restrictedList.Count());
            position.GetTextForValue = (v) => {
                if (restrictedList.Count() == 0)
                    return positions[Mathf.RoundToInt(v)].Item2;
                else
                    return restrictedList[Mathf.RoundToInt(v)].Item2;
            };
            position.GetValue = () => {
                return positions.ToList().IndexOf(positions.Where((Tuple<ICounterPositions, string> x) => (x.Item1 == settings.Position)).First());
            };
            position.SetValue = (v) => {
                if (restrictedList.Count() == 0)
                    settings.Position = positions[Mathf.RoundToInt(v)].Item1;
                else
                    settings.Position = restrictedList[Mathf.RoundToInt(v)].Item1;
            };

            var index = AddList(ref sub, "Index", "How far from the position the counter will be. A higher number means farther way.", 6);
            index.GetTextForValue = (v) => Mathf.RoundToInt(v).ToString();
            index.GetValue = () => settings.Index;
            index.SetValue = (v) => settings.Index = Mathf.RoundToInt(v);

            Task.Run(() => { //Give AdvancedCountersSettings time to queue up other settings before init
                Thread.Sleep(25);
                foreach (ListViewController list in loadedSettings)
                {
                    list.Init();
                }
            });
            return sub;
        }

        internal static ListViewController AddList(ref SubMenu sub, string Label, string HintText, int sizeCount)
        {
            List<float> values = new List<float>() { };
            for (var i = 0; i < sizeCount; i++)
            {
                values.Add(i);
            }
            var list = sub.AddList(Label, values.ToArray(), HintText);
            list.applyImmediately = true;
            PositionElement(list.gameObject);
            loadedSettings.Add(list);
            return list;
        }

        private static void ClearScreen()
        {
            foreach (GameObject element in loadedElements)
            {
                Destroy(element);
            }
            loadedElements.Clear();
            loadedSettings.Clear();
            settingsCount = 0;
        }

        private static void PositionElement(GameObject element)
        {
            loadedElements.Add(element);
            setStuff(element.transform as RectTransform, 0.05f, 0.75f - (settingsCount * 0.1f), 0.9f, 0.166f, 0f);
            settingsCount++;
        }

        static bool TestGet(ref bool variable)
        {
            return variable;
        }
    }
}