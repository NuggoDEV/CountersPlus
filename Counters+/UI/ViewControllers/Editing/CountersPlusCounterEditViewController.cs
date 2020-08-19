using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CountersPlus.ConfigModels;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace CountersPlus.UI.ViewControllers.Editing
{
    public class CountersPlusCounterEditViewController : BSMLResourceViewController
    {
        public override string ResourceName => $"CountersPlus.UI.BSML.EditBase.bsml";

        private string SettingsBase = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "CountersPlus.UI.BSML.SettingsBase.bsml");

        [UIObject("body")] private GameObject settingsContainer;
        [UIComponent("ScrollContent")] private BSMLScrollableContainer scrollView;
        [UIComponent("name")] private TextMeshProUGUI settingsHeader;

        public void ApplySettings(ConfigModel model)
        {
            settingsHeader.name = $"{model.DisplayName} Settings";
            
            ClearScreen();

            Plugin.Logger.Warn("Loading settings base");
            BSMLParser.instance.Parse(SettingsBase, settingsContainer, model);

            Plugin.Logger.Warn("Loading model specific settings for " + model.DisplayName);
            string resourceLocation = $"CountersPlus.UI.BSML.Config.{model.DisplayName}.bsml";

            string resourceContent = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), resourceLocation);

            BSMLParser.instance.Parse(resourceContent, settingsContainer, model);

            StartCoroutine(WaitThenDirtyTheFuckingScrollView());
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
    }
}
