using BeatSaberMarkupLanguage.Attributes;
using CountersPlus.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace CountersPlus.UI.ViewControllers.ConfigModelControllers
{
    class SpinometerController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("mode")]
        public ICounterMode Mode
        {
            get => (parentController?.ConfigModel as SpinometerConfigModel).Mode;
            set => (parentController?.ConfigModel as SpinometerConfigModel).Mode = value;
        }

        [UIValue("mode_values")]
        public List<object> ModeValues => AdvancedCounterSettings.SpinometerSettings.Keys.Cast<object>().ToList();

        [UIAction("mode_formatter")]
        public string Format(ICounterMode mode) => AdvancedCounterSettings.SpinometerSettings[mode];

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            parentController.ConfigChanged(obj);
        }
    }
}
