using UnityEngine;
using CountersPlus.Config;
using BeatSaberMarkupLanguage.Attributes;

namespace CountersPlus.UI.ViewControllers.ConfigModelControllers
{
    class FailController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("restarts")]
        public bool SeparateSaberCuts
        {
            get => (parentController?.ConfigModel as FailConfigModel).ShowRestartsInstead;
            set => (parentController?.ConfigModel as FailConfigModel).ShowRestartsInstead = value;
        }

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            parentController.ConfigChanged(obj);
        }
    }
}
