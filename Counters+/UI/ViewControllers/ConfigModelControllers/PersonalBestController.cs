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
    class PersonalBestController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("below_score")]
        public bool BelowScoreCounter
        {
            get => (parentController?.ConfigModel as PBConfigModel).UnderScore;
            set => (parentController?.ConfigModel as PBConfigModel).UnderScore = value;
        }

        [UIValue("hide_first_score")]
        public bool HideFirstScore
        {
            get => (parentController?.ConfigModel as PBConfigModel).HideFirstScore;
            set => (parentController?.ConfigModel as PBConfigModel).HideFirstScore = value;
        }

        [UIValue("precision")]
        public int PercentagePrecision
        {
            get => (parentController?.ConfigModel as PBConfigModel).DecimalPrecision;
            set => (parentController?.ConfigModel as PBConfigModel).DecimalPrecision = value;
        }

        [UIValue("precision_values")]
        public List<object> PrecisionValues => AdvancedCounterSettings.PercentagePrecision.Cast<object>().ToList();


        [UIValue("text_size")]
        public int TextSize
        {
            get => (parentController?.ConfigModel as PBConfigModel).TextSize;
            set => (parentController?.ConfigModel as PBConfigModel).TextSize = value;
        }

        [UIValue("text_size_values")]
        public List<object> TextSizeValues => AdvancedCounterSettings.TextSize.Cast<object>().ToList();

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            parentController.ConfigChanged(obj);
        }
    }
}
