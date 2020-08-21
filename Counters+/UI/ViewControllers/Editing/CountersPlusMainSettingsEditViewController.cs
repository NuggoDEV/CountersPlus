using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using HMUI;
using System.Reflection;
using Zenject;

namespace CountersPlus.UI.ViewControllers.Editing
{
    public class CountersPlusMainSettingsEditViewController : ViewController
    {
        private readonly string SettingsBase = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "CountersPlus.UI.BSML.MainSettings.bsml");

        [Inject] private MainConfigModel mainConfig;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation) BSMLParser.instance.Parse(SettingsBase, gameObject, mainConfig);
        }
    }
}
