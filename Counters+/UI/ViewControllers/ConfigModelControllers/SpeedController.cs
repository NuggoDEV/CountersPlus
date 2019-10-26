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
    class SpeedController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("mode")]
        public ICounterMode Mode
        {
            get => (parentController?.ConfigModel as SpeedConfigModel).Mode;
            set => (parentController?.ConfigModel as SpeedConfigModel).Mode = value;
        }

        [UIValue("mode_values")]
        public List<object> ModeValues => AdvancedCounterSettings.SpeedSettings.Keys.Cast<object>().ToList();

        [UIAction("mode_formatter")]
        public string Format(ICounterMode mode) => AdvancedCounterSettings.SpeedSettings[mode];

        [UIValue("precision")]
        public int PercentagePrecision
        {
            get => (parentController?.ConfigModel as SpeedConfigModel).DecimalPrecision;
            set => (parentController?.ConfigModel as SpeedConfigModel).DecimalPrecision = value;
        }

        [UIValue("precision_values")]
        public List<object> PrecisionValues => AdvancedCounterSettings.PercentagePrecision.Cast<object>().ToList();

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            parentController.ConfigChanged(obj);
        }
    }
}
