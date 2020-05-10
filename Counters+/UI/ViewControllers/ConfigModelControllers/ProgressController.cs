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
    class ProgressController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("progresstimeleft")]
        public bool ProgressTimeLeft
        {
            get => (parentController?.ConfigModel as ProgressConfigModel).ProgressTimeLeft;
            set => (parentController?.ConfigModel as ProgressConfigModel).ProgressTimeLeft = value;
        }

        [UIValue("includering")]
        public bool IncludeRing
        {
            get => (parentController?.ConfigModel as ProgressConfigModel).IncludeRing;
            set => (parentController?.ConfigModel as ProgressConfigModel).IncludeRing = value;
        }

        [UIValue("show_time_in_beats")]
        public bool ShowTimeInBeats
        {
            get => (parentController?.ConfigModel as ProgressConfigModel).ShowTimeInBeats;
            set => (parentController?.ConfigModel as ProgressConfigModel).ShowTimeInBeats = value;
        }

        [UIValue("precision_values")]
        public List<object> PrecisionValues => AdvancedCounterSettings.PercentagePrecision.Cast<object>().ToList();

        [UIValue("mode")]
        public ICounterMode Mode
        {
            get => (parentController?.ConfigModel as ProgressConfigModel).Mode;
            set => (parentController?.ConfigModel as ProgressConfigModel).Mode = value;
        }

        [UIValue("mode_values")]
        public List<object> ModeValues => AdvancedCounterSettings.ProgressSettings.Keys.Cast<object>().ToList();

        [UIAction("mode_formatter")]
        public string Format(ICounterMode mode) => AdvancedCounterSettings.ProgressSettings[mode];

        [UIObject("includering")]
        private GameObject ringGO;

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            ringGO?.SetActive(Mode == ICounterMode.Original && ProgressTimeLeft);
            parentController.ConfigChanged(obj);
        }
    }
}
