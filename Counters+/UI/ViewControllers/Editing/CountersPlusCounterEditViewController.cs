using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CountersPlus.ConfigModels;
using System.Collections;
using System.Reflection;
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

        [UIObject("body")] private GameObject settingsContainer;
        [UIComponent("ScrollContent")] private BSMLScrollableContainer scrollView;
        [UIComponent("name")] private TextMeshProUGUI settingsHeader;

        private ConfigModel editingConfigModel = null;

        public void ApplySettings(ConfigModel model)
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

            // Loading settings base
            BSMLParser.instance.Parse(SettingsBase, settingsContainer, model);

            // Loading counter-specific settings
            string resourceLocation = $"CountersPlus.UI.BSML.Config.{model.DisplayName}.bsml";

            string resourceContent = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), resourceLocation);

            BSMLParser.instance.Parse(resourceContent, settingsContainer, model);

            StartCoroutine(WaitThenDirtyTheFuckingScrollView());
        }

        private void MainConfig_OnConfigChanged()
        {
            mockCounter.UpdateMockCounter(editingConfigModel);
        }

        private IEnumerator WaitThenDirtyTheFuckingScrollView()
        {
            yield return new WaitUntil(() => scrollView != null && scrollView.ContentRect != null);
            scrollView.ContentRect.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
            scrollView.ContentRect.gameObject.SetActive(true);
        }

        private void ClearScreen()
        {
            for (int i = 0; i < settingsContainer.transform.childCount; i++)
                Destroy(settingsContainer.transform.GetChild(i).gameObject);
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            mainConfig.OnConfigChanged -= MainConfig_OnConfigChanged;
        }
    }
}
