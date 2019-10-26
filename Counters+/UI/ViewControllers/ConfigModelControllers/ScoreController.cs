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
    class ScoreController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("display_rank")]
        public bool DisplayRank
        {
            get => (parentController?.ConfigModel as ScoreConfigModel).DisplayRank;
            set => (parentController?.ConfigModel as ScoreConfigModel).DisplayRank = value;
        }

        [UIValue("mode")]
        public ICounterMode Mode
        {
            get => (parentController?.ConfigModel as ScoreConfigModel).Mode;
            set => (parentController?.ConfigModel as ScoreConfigModel).Mode = value;
        }

        [UIValue("mode_values")]
        public List<object> ModeValues => AdvancedCounterSettings.ScoreSettings.Keys.Cast<object>().ToList();

        [UIAction("mode_formatter")]
        public string Format(ICounterMode mode) => AdvancedCounterSettings.ScoreSettings[mode];

        [UIValue("precision")]
        public int PercentagePrecision
        {
            get => (parentController?.ConfigModel as ScoreConfigModel).DecimalPrecision;
            set => (parentController?.ConfigModel as ScoreConfigModel).DecimalPrecision = value;
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
