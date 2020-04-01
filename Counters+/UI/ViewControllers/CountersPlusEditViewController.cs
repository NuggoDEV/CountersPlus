using System;
using System.Collections.Generic;
using System.Linq;
using CountersPlus.Config;
using UnityEngine;
using TMPro;
using CountersPlus.Custom;
using BS_Utils.Gameplay;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage;
using CountersPlus.UI.ViewControllers.ConfigModelControllers;

namespace CountersPlus.UI.ViewControllers
{
    class CountersPlusEditViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CountersPlus.UI.BSML.EditBase.bsml";
        public static CountersPlusEditViewController Instance;
        private static RectTransform rect;

        internal static List<GameObject> LoadedElements = new List<GameObject>(); //Mass clearing

        [UIObject("body")] internal GameObject SettingsContainer;
        [UIObject("settings_parent")] private GameObject SettingsParent;
        [UIComponent("name")] private TextMeshProUGUI SettingsName;

        private static ConfigModel SelectedConfigModel = null;

        private static bool wasInMainSettingsMenu = false;

        private static void SetPositioning(RectTransform r, float x, float y, float w, float h, float pivotX)
        {
            r.anchorMin = new Vector2(x, y);
            r.anchorMax = new Vector2(x + w, y + h);
            r.pivot = new Vector2(pivotX, 1);
            r.sizeDelta = Vector2.zero;
            r.anchoredPosition = Vector2.zero;
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);
            Instance = this;
            rect = rectTransform;
            if (!firstActivation && SelectedConfigModel != null)
                UpdateSettings(SelectedConfigModel);
            else if (!firstActivation && wasInMainSettingsMenu)
                ShowMainSettings();
        }

        internal static void ShowContributors()
        {
            Dictionary<string, string> contributors = new Dictionary<string, string>(ContributorsAndDonators.Contributors);
            string user = GetUserInfo.GetUserName();
            if (user == null) user = "You";
            if (contributors.ContainsKey(user))
                contributors.Add($"{user}, again!", "For enjoying this mod!");
            else contributors.Add(user, "For enjoying this mod!"); //Teehee :)

            ClearScreen();
            TextMeshProUGUI contributorLabel;
            contributorLabel = BeatSaberUI.CreateText(rect, "Thanks to these contributors for, directly or indirectly, helping make Counters+ what it is!", Vector2.zero);
            contributorLabel.fontSize = 3;
            contributorLabel.alignment = TextAlignmentOptions.Center;
            SetPositioning(contributorLabel.rectTransform, 0, 0.85f, 1, 0.166f, 0.5f);
            LoadedElements.Add(contributorLabel.gameObject);

            foreach (var kvp in contributors)
            {
                TextMeshProUGUI contributor = BeatSaberUI.CreateText(rect, $"<color=#00c0ff>{kvp.Key}</color> | {kvp.Value}", Vector2.zero);
                contributor.fontSize = 3;
                contributor.alignment = TextAlignmentOptions.Left;
                SetPositioning(contributor.rectTransform, 0.05f,
                    0.8f - (contributors.Keys.ToList().IndexOf(kvp.Key) * 0.05f), 1, 0.166f, 0.5f);
                LoadedElements.Add(contributor.gameObject);
            }
        }

        internal static void ShowDonators()
        {
            Dictionary<string, string> donators = new Dictionary<string, string>(ContributorsAndDonators.Donators);
            TextMeshProUGUI donatorLabel = BeatSaberUI.CreateText(rect, "Thanks to the <color=#FF0048>Ko-fi</color> donators who support me! <i>DM me on Discord for any corrections to these names.</i>", Vector2.zero);

            ClearScreen();
            donatorLabel.fontSize = 3;
            donatorLabel.alignment = TextAlignmentOptions.Center;
            SetPositioning(donatorLabel.rectTransform, 0, 0.85f, 1, 0.166f, 0.5f);
            LoadedElements.Add(donatorLabel.gameObject);

            foreach (var kvp in donators)
            {
                TextMeshProUGUI donator = BeatSaberUI.CreateText(rect, $"<color=#FF0048>{kvp.Key}</color> | {kvp.Value}", Vector2.zero);
                donator.fontSize = 3;
                donator.alignment = TextAlignmentOptions.Left;
                SetPositioning(donator.rectTransform, 0.05f,
                    0.8f - (donators.Keys.ToList().IndexOf(kvp.Key) * 0.05f), 1, 0.166f, 0.5f);
                LoadedElements.Add(donator.gameObject);
            }
        }

        internal static void ShowMainSettings()
        {
            ClearScreen(true);
            Instance.SettingsName.text = "Main Settings";
            Type controllerType = Type.GetType($"CountersPlus.UI.ViewControllers.ConfigModelControllers.MainSettingsController");
            ConfigModelController.GenerateController(controllerType, Instance.SettingsContainer,"CountersPlus.UI.BSML.MainSettings.bsml");
            MockCounter.Highlight<ConfigModel>(null);
            wasInMainSettingsMenu = true;
        }

        internal static void UpdateTitle(string title)
        {
            Instance.SettingsName.text = title;
        }

        public static void UpdateSettings<T>(T settings) where T : ConfigModel
        {
            try
            {
                if (settings is null) return;
                wasInMainSettingsMenu = false;
                SelectedConfigModel = settings;
                ClearScreen(true);
                MockCounter.Highlight(settings);
                string name = string.Join("", settings.DisplayName.Split(' '));
                if (settings is CustomConfigModel custom)
                {
                    ConfigModelController.GenerateController(custom.CustomCounter.CustomSettingsHandler,
                        Instance.SettingsContainer, custom.CustomCounter.CustomSettingsResource, true, settings);
                    name = custom.CustomCounter.Name;
                }
                else
                {
                    Type controllerType = Type.GetType($"CountersPlus.UI.ViewControllers.ConfigModelControllers.{name}Controller");
                    ConfigModelController controller = ConfigModelController.GenerateController(settings, controllerType, Instance.SettingsContainer);
                }
                Instance.SettingsName.text = $"{(settings is null ? "Oops!" : $"{settings.DisplayName} Settings")}";
            }
            catch (Exception e) { Plugin.Log(e.ToString(), LogInfo.Fatal, "Go to the Counters+ GitHub and open an Issue. This shouldn't happen!"); }
        }

        internal static void ClearScreen(bool enableSettings = false)
        {
            foreach (GameObject go in LoadedElements) Destroy(go);
            for (int i = 0; i < Instance.SettingsContainer.transform.childCount; i++) Destroy(Instance.SettingsContainer.transform.GetChild(i).gameObject);
            Instance.SettingsParent.SetActive(enableSettings);
        }
    }
}