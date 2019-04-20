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
using System.Collections;
using IPA.Loader;

namespace CountersPlus.UI
{
    class CountersPlusEditViewController : VRUIViewController
    {
        public static CountersPlusEditViewController Instance;
        private static RectTransform rect;
        private static TextMeshProUGUI settingsTitle;
        private static SubMenu container;
        
        internal static List<GameObject> loadedElements = new List<GameObject>(); //Mass clearing
        private static List<ListSettingsController> loadedSettings = new List<ListSettingsController>(); //Mass initialization
        internal static int settingsCount = 0; //Spacing

        internal class PositionSettingsViewController : TupleViewController<Tuple<ICounterPositions, string>> { }
        static List<Tuple<ICounterPositions, string>> positions = new List<Tuple<ICounterPositions, string>> {
            {ICounterPositions.BelowCombo, "Below Combo" },
            {ICounterPositions.AboveCombo, "Above Combo" },
            {ICounterPositions.BelowMultiplier, "Below Multi." },
            {ICounterPositions.AboveMultiplier, "Above Multi." },
            {ICounterPositions.BelowEnergy, "Below Energy" },
            {ICounterPositions.AboveHighway, "Over Highway" }
        };

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
            rect = rectTransform;
            if (firstActivation)
            {
                Instance = this;
                CreateCredits();
            }
        }

        internal static void CreateCredits()
        {
            ClearScreen();
            TextMeshProUGUI name, version, creator;
            Button github, issues, donate;

            //name = BeatSaberUI.CreateText(rect, "Temporary Name LMAO", Vector2.zero);
            name = BeatSaberUI.CreateText(rect, "Counters+", Vector2.zero);
            name.fontSize = 9;
            name.alignment = TextAlignmentOptions.Center;
            name.characterSpacing = 2;
            setPositioning(name.rectTransform, 0, 0.7f, 1, 0.166f, 0.5f);

            version = BeatSaberUI.CreateText(rect,
                $"Version <color={(Plugin.upToDate ? "#00FF00" : "#FF0000")}>{PluginManager.GetPlugin("Counters+").Metadata.Version.ToString()}</color>", Vector2.zero);
            version.fontSize = 3;
            version.alignment = TextAlignmentOptions.Center;
            setPositioning(version.rectTransform, 0, 0.5f, 1, 0.166f, 0.5f);

            if (!Plugin.upToDate)
            {
                TextMeshProUGUI warning = BeatSaberUI.CreateText(rect,
                $"<color=#FF0000>Version {Plugin.webVersion} available for download!</color>", Vector2.zero);
                warning.fontSize = 3;
                warning.alignment = TextAlignmentOptions.Center;
                setPositioning(warning.rectTransform, 0, 0.47f, 1, 0.166f, 0.5f);
                loadedElements.Add(warning.gameObject);
            }

            creator = BeatSaberUI.CreateText(rect, "Developed by: <color=#00c0ff>Caeden117</color>", Vector2.zero);
            creator.fontSize = 5;
            creator.alignment = TextAlignmentOptions.Center;
            setPositioning(creator.rectTransform, 0, 0.35f, 1, 0.166f, 0.5f);

            github = BeatSaberUI.CreateUIButton(rect, "QuitButton", Vector2.zero, null, "GitHub", Images.Images.Load("GitHub"));
            github.onClick.AddListener(() => { GoTo("https://github.com/Caeden117/CountersPlus", github); });
            setPositioning(github.transform as RectTransform, 0.1f, 0.1f, 0.25f, 0.166f, 0.5f);
            BeatSaberUI.AddHintText(github.transform as RectTransform, "Opens in a new browser tab on your desktop. Feel free to explore the source code! Maybe try out experimental versions?");

            issues = BeatSaberUI.CreateUIButton(rect, "QuitButton", Vector2.zero, null, "Report an Issue", null);
            issues.onClick.AddListener(() => { GoTo("https://github.com/Caeden117/CountersPlus/issues", issues); });
            setPositioning(issues.transform as RectTransform, 0.55f, 0.1f, 0.35f, 0.166f, 0.5f);
            BeatSaberUI.AddHintText(issues.transform as RectTransform, "Opens in a new browser tab on your desktop. Be sure to read the Issue template thoroughly!");

            donate = BeatSaberUI.CreateUIButton(rect, "PlayButton", Vector2.zero, null, "<3", null);
            ColorUtility.TryParseHtmlString("#FF0048", out Color color);
            donate.gameObject.GetComponentInChildren<Image>().color = color;
            donate.onClick.AddListener(() => { GoTo("https://ko-fi.com/Caeden117", donate); });
            BeatSaberUI.AddHintText(donate.transform as RectTransform, "Buy me a coffee if you feel like I'm deserving of one.");
            setPositioning(donate.transform as RectTransform, 0.375f, 0.075f, 0.15f, 0.166f, 0.5f);

            loadedElements.AddRange(new GameObject[] { name.gameObject, version.gameObject, creator.gameObject, github.gameObject, issues.gameObject, donate.gameObject });
        }

        private static void GoTo(string url, Button button)
        {
            Plugin.Log("Opened a link to: " + url);
            button.interactable = false;
            TextMeshProUGUI reminder = BeatSaberUI.CreateText(rect, "Link opened in your browser!", Vector2.zero);
            reminder.fontSize = 4;
            reminder.alignment = TextAlignmentOptions.Center;
            setPositioning(reminder.rectTransform, 0, 0.25f, 1, 0.166f, 0.5f);
            loadedElements.Add(reminder.gameObject);
            Instance.StartCoroutine(Instance.SecondRemove(reminder.gameObject, button));
            System.Diagnostics.Process.Start(url);
        }

        private IEnumerator SecondRemove(GameObject go, Button button) {
            yield return new WaitForSeconds(5);
            loadedElements.Remove(go);
            Destroy(go);
            button.interactable = true;
        }

        internal static void ShowContributors()
        {
            Dictionary<string, string> contributors = new Dictionary<string, string>()
            {
                { "Moon", "Bug fixing and code optimization" },
                { "Shoko84", "Bug fixing" },
                { "xhuytox", "Big helper in bug hunting - thanks man!" },
                { "Brian", "Saving Beat Saber Modding with CustomUI" },
                { "Viscoci", "Helper code that fixed displaying text and saved my bacon" },
                { "Assistant", "Stole some Custom Avatars UI code to help with settings" },
                { "Kyle1413", "Beat Saber Utils and for Progress/Score Counter code" },
                { "Ragesaq", "Speed Counter and Spinometer idea <i>(and some bug fixing on stream)</i>" },
                { "SkyKiwiTV", "Original version checking code" },
                { "Dracrius", "Bug fixing and beta testing" },
                { "Stackeror", "Creator of the original Progress Counter mod" },
            };
            string user = GetUserInfo.GetUserName();
            if (user == null) user = "Channel Monitor Bot";
            if (contributors.ContainsKey(user))
                contributors.Add($"<i>\"{user}\"</i>", "For enjoying this mod!");
            else contributors.Add(user, "For enjoying this mod!"); //Teehee :)

            ClearScreen();
            TextMeshProUGUI contributorLabel;
            contributorLabel = BeatSaberUI.CreateText(rect, "Thanks to these contributors for, directly or indirectly, helping make Counters+ what it is!", Vector2.zero);
            contributorLabel.fontSize = 3;
            contributorLabel.alignment = TextAlignmentOptions.Center;
            setPositioning(contributorLabel.rectTransform, 0, 0.85f, 1, 0.166f, 0.5f);
            loadedElements.Add(contributorLabel.gameObject);

            foreach (var kvp in contributors)
            {
                TextMeshProUGUI contributor = BeatSaberUI.CreateText(rect, $"<color=#00c0ff>{kvp.Key}</color> | {kvp.Value}", Vector2.zero);
                contributor.fontSize = 3;
                contributor.alignment = TextAlignmentOptions.Left;
                setPositioning(contributor.rectTransform, 0.15f,
                    0.8f - (contributors.Keys.ToList().IndexOf(kvp.Key) * 0.05f), 1, 0.166f, 0.5f);
                loadedElements.Add(contributor.gameObject);
            }
        }

        internal static void ShowMainSettings()
        {
            ClearScreen();
            settingsTitle = BeatSaberUI.CreateText(rect, "Main Settings", Vector2.zero);
            settingsTitle.fontSize = 6;
            settingsTitle.alignment = TextAlignmentOptions.Center;
            setPositioning(settingsTitle.rectTransform, 0, 0.85f, 1, 0.166f, 0.5f);
            loadedElements.Add(settingsTitle.gameObject);

            SubMenu sub = new SubMenu(rect);
            var enabled = AddList(ref sub, null as IConfigModel, "Enabled", "Toggles Counters+ on or off.", 2);
            enabled.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
            enabled.GetValue = () => CountersController.settings.Enabled ? 1f : 0f;
            enabled.SetValue = (v) => CountersController.settings.Enabled = v != 0f;

            var toggleCounters = AddList(ref sub, null as IConfigModel, "Advanced Mock Counters", "Allows the mock counters to display more settings. To increase preformance, and reduce chances of bugs, disable this option.", 2);
            toggleCounters.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
            toggleCounters.GetValue = () => CountersController.settings.AdvancedCounterInfo ? 1f : 0f;
            toggleCounters.SetValue = (v) => CountersController.settings.AdvancedCounterInfo = v != 0f;

            var comboOffset = AddList(ref sub, null as IConfigModel, "Combo Offset", "How far from the Combo counters should be before Distance is taken into account.", 20);
            comboOffset.GetTextForValue = (v) => ((v - 10) / 10).ToString();
            comboOffset.GetValue = () => (CountersController.settings.ComboOffset * 10) + 10;
            comboOffset.SetValue = (v) => CountersController.settings.ComboOffset = ((v - 10) / 10);

            var multiOffset = AddList(ref sub, null as IConfigModel, "Multiplier Offset", "How far from the Multiplier counters should be before Distance is taken into account.", 20);
            multiOffset.GetTextForValue = (v) => ((v - 10) / 10).ToString();
            multiOffset.GetValue = () => (CountersController.settings.MultiplierOffset * 10) + 10;
            multiOffset.SetValue = (v) => CountersController.settings.MultiplierOffset = ((v - 10) / 10);

            var hideCombo = AddList(ref sub, null as IConfigModel, "Hide Combo In-Game", "The combo counter wasn't good enough anyways.", 2);
            hideCombo.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
            hideCombo.GetValue = () => CountersController.settings.HideCombo ? 1f : 0f;
            hideCombo.SetValue = (v) => CountersController.settings.HideCombo = v != 0f;

            var hideMultiplier = AddList(ref sub, null as IConfigModel, "Hide Multiplier In-Game", "The multiplier wasn't good enough anyways.", 2);
            hideMultiplier.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
            hideMultiplier.GetValue = () => CountersController.settings.HideMultiplier ? 1f : 0f;
            hideMultiplier.SetValue = (v) => CountersController.settings.HideMultiplier = v != 0f;

            toggleCounters.SetValue += (v) => CountersPlusSettingsFlowCoordinator.UpdateMockCounters();
            comboOffset.SetValue += (v) => CountersPlusSettingsFlowCoordinator.UpdateMockCounters();
            multiOffset.SetValue += (v) => CountersPlusSettingsFlowCoordinator.UpdateMockCounters();
            InitSettings();
        }

        public static void UpdateSettings<T>(T settings, SettingsInfo info) where T : IConfigModel
        {
            try
            {
                if (!(settings is null)) MockCounter.Highlight(settings);
                ClearScreen();
                if (!(info is null))
                {
                    if (info.IsCustom) container = CreateBase(settings, (settings as CustomConfigModel).RestrictedPositions);
                    else
                    {
                        SubMenu sub = CreateBase(settings);
                        AdvancedCounterSettings.counterUIItems.Where(
                            (KeyValuePair<IConfigModel, Action<SubMenu, IConfigModel>> x) => (x.Key.DisplayName == settings.DisplayName)
                            ).First().Value(sub, settings);
                    }
                }
                settingsTitle = BeatSaberUI.CreateText(rect, $"{settings.DisplayName} Settings", Vector2.zero);
                settingsTitle.fontSize = 6;
                settingsTitle.alignment = TextAlignmentOptions.Center;
                setPositioning(settingsTitle.rectTransform, 0, 0.85f, 1, 0.166f, 0.5f);
                loadedElements.Add(settingsTitle.gameObject);
                InitSettings();
            }
            catch(Exception e) { Plugin.Log(e.ToString(), Plugin.LogInfo.Fatal, "Go to the Counters+ GitHub and open an Issue. This shouldn't happen!"); }
        }

        private static SubMenu CreateBase<T>(T settings, params ICounterPositions[] restricted) where T : IConfigModel
        {
            SubMenu sub = new SubMenu(rect);
            List<Tuple<ICounterPositions, string>> restrictedList = new List<Tuple<ICounterPositions, string>>();
            try
            {
                foreach (ICounterPositions pos in restricted)
                    restrictedList.Add(Tuple.Create(pos, positions.Where((Tuple<ICounterPositions, string> x) => x.Item1 == pos).First().Item2));
            }
            catch { } //It most likely errors here. If it does, well no problem.

            var enabled = AddList(ref sub, settings, "Enabled", "Toggles this counter on or off.", 2);
            enabled.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
            enabled.GetValue = () => settings.Enabled ? 1f : 0f;
            enabled.SetValue += (v) => settings.Enabled = v != 0f;

            var position = AddList(ref sub, settings, "Position", "The relative position of common UI elements", (restrictedList.Count() == 0) ? positions.Count() : restrictedList.Count());
            position.GetTextForValue = (v) => {
                Plugin.Log(v.ToString());
                if (restrictedList.Count() == 0)
                    return positions[Mathf.RoundToInt(v)].Item2;
                else
                    return restrictedList[Mathf.RoundToInt(v)].Item2;
            };
            position.GetValue = () => {
                return positions.ToList().IndexOf(positions.Where((Tuple<ICounterPositions, string> x) => (x.Item1 == settings.Position)).First());
            };
            position.SetValue += (v) => {
                if (restrictedList.Count() == 0)
                    settings.Position = positions[Mathf.RoundToInt(v)].Item1;
                else
                    settings.Position = restrictedList[Mathf.RoundToInt(v)].Item1;
            };

            var index = AddList(ref sub, settings, "Distance", "How far from the position the counter will be. A higher number means farther way.", 7);
            index.GetTextForValue = (v) => Mathf.RoundToInt(v - 1).ToString();
            index.GetValue = () => settings.Index + 1;
            index.SetValue += (v) => settings.Index = Mathf.RoundToInt(v - 1);
            return sub;
        }

        internal static ListViewController AddList<T>(ref SubMenu sub, T settings, string Label, string HintText, int sizeCount) where T : IConfigModel
        {
            List<float> values = new List<float>() { };
            for (var i = 0; i < sizeCount; i++) values.Add(i);
            var list = sub.AddList(Label, values.ToArray(), HintText);
            list.applyImmediately = true;
            PositionElement(list.gameObject);
            loadedSettings.Add(list);
            if (!(settings is null)) list.SetValue = (v) => Instance.StartCoroutine(DelayedMockCounterUpdate(settings));
            return list;
        }

        private static IEnumerator DelayedMockCounterUpdate<T>(T settings) where T : IConfigModel
        {
            yield return new WaitForEndOfFrame();
            MockCounter.Update(settings);
        }

        private static void ClearScreen()
        {
            foreach (GameObject element in loadedElements) Destroy(element);
            loadedElements.Clear();
            loadedSettings.Clear();
            settingsCount = 0;
        }

        private static void PositionElement(GameObject element)
        {
            loadedElements.Add(element);
            setPositioning(element.transform as RectTransform, 0.05f, 0.75f - (settingsCount * 0.1f), 0.9f, 0.166f, 0f);
            settingsCount++;
        }

        private static void InitSettings()
        {
            foreach (ListViewController list in loadedSettings)
            {
                list.InvokePrivateMethod("OnDisable", new object[] { });
                list.InvokePrivateMethod("OnEnable", new object[] { });
            }
        }
    }
}