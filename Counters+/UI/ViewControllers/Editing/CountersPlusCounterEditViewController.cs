using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CountersPlus.Utils;
using CountersPlus.Custom;
using CountersPlus.ConfigModels;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.UI.ViewControllers.Editing
{
    public class CountersPlusCounterEditViewController : BSMLResourceViewController
    {
        public override string ResourceName => $"CountersPlus.UI.BSML.EditBase.bsml";

        private readonly string SettingsBase = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "CountersPlus.UI.BSML.SettingsBase.bsml");

        [Inject] private MainConfigModel mainConfig;
        [Inject] private MockCounter mockCounter;
        [Inject] private CanvasUtility canvasUtility;
        [Inject] private DiContainer diContainer;

        [UIObject("body")] private GameObject settingsContainer;
        [UIComponent("ScrollContent")] private BSMLScrollableContainer scrollView;
        [UIComponent("name")] private TextMeshProUGUI settingsHeader;

        private Dictionary<ConfigModel, HashSet<GameObject>> cachedSettings = new Dictionary<ConfigModel, HashSet<GameObject>>();

        private ConfigModel editingConfigModel = null;

        internal void ApplySettings(ConfigModel model)
        {
            ClearScreen();

            if (editingConfigModel != null)
            {
                mainConfig.OnConfigChanged -= MainConfig_OnConfigChanged;
            }

            settingsHeader.text = $"{model.DisplayName} Settings";
            mainConfig.OnConfigChanged += MainConfig_OnConfigChanged;
            editingConfigModel = model;
            mockCounter.HighlightCounter(editingConfigModel);

            // Setup helper functions for the config model to hook off of.
            model.GetCanvasFromID = (v) => canvasUtility.GetCanvasSettingsFromID(v);
            model.GetCanvasIDFromCanvasSettings = (v) => mainConfig.HUDConfig.OtherCanvasSettings.IndexOf(v);
            model.GetAllCanvases = () => GetAllCanvases();

            if (cachedSettings.TryGetValue(model, out var cache))
            {
                if (model is CustomConfigModel customConfig)
                {
                    CustomCounter customCounter = customConfig.AttachedCustomCounter;
                    settingsHeader.text = $"{customCounter.Name} Settings";
                }
                foreach (GameObject obj in cache)
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                // Loading settings base
                var settingsBaseParams = BSMLParser.Instance.Parse(SettingsBase, settingsContainer, model);
                var multiplayerWarning = settingsBaseParams.GetObjectsWithTag("multiplayer-warning")[0];

                // Only show multiplayer warning if this is a Custom Counter that is not enabled in multiplayer

                // Loading counter-specific settings
                if (model is not CustomConfigModel customConfig)
                {
                    string resourceLocation = $"CountersPlus.UI.BSML.Config.{model.DisplayName}.bsml";
                    string resourceContent = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), resourceLocation);
                    BSMLParser.Instance.Parse(resourceContent, settingsContainer, model);

                    // All base Counters+ counters are (or should be) multiplayer ready.
                    multiplayerWarning.SetActive(false);
                }
                else
                {
                    CustomCounter customCounter = customConfig.AttachedCustomCounter;
                    settingsHeader.text = $"{customCounter.Name} Settings";
                    if (customCounter.BSML != null && !string.IsNullOrEmpty(customCounter.BSML.Resource))
                    {
                        string resourceLocation = customCounter.BSML.Resource;
                        string resourceContent = Utilities.GetResourceContent(customCounter.CounterType.Assembly, resourceLocation);

                        object host = null;
                        if (customCounter.BSML.HasType)
                        {
                            host = diContainer.TryResolveId(customCounter.BSML.HostType, customCounter.Name);
                        }
                        BSMLParser.Instance.Parse(resourceContent, settingsContainer, host);
                    }

                    // Show multiplayer warning if the custom counter is not multiplayer ready.
                    multiplayerWarning.SetActive(!customCounter.MultiplayerReady);
                }
            }

            StartCoroutine(WaitThenDirtyTheFuckingScrollView());
        }

        private List<HUDCanvas> GetAllCanvases()
        {
            List<HUDCanvas> allCanvases = new List<HUDCanvas>() { mainConfig.HUDConfig.MainCanvasSettings };
            allCanvases.AddRange(mainConfig.HUDConfig.OtherCanvasSettings);
            return allCanvases;
        }

        private void MainConfig_OnConfigChanged()
        {
            mockCounter.UpdateMockCounter(editingConfigModel);
        }

        private IEnumerator WaitThenDirtyTheFuckingScrollView() // I'm still sad I have to do this.
        {
            yield return new WaitUntil(() => scrollView != null);
            scrollView.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
            scrollView.gameObject.SetActive(true);
        }

        private void ClearScreen()
        {
            for (int i = 0; i < settingsContainer.transform.childCount; i++)
            {
                GameObject child = settingsContainer.transform.GetChild(i).gameObject;
                if (child.activeSelf)
                {
                    if (!cachedSettings.TryGetValue(editingConfigModel, out var cache))
                    {
                        cache = new HashSet<GameObject>() { };
                        cachedSettings.Add(editingConfigModel, cache);
                    }
                    cache.Add(child);
                    child.SetActive(false);
                }
            }
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemEnabling)
        {
            mainConfig.OnConfigChanged -= MainConfig_OnConfigChanged;
        }
    }
}
