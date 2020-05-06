using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CountersPlus.Config;
using BeatSaberMarkupLanguage.Attributes;

namespace CountersPlus.UI.ViewControllers.ConfigModelControllers
{
    class CutController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("separate-sabers")]
        public bool SeparateSaberCuts
        {
            get => (parentController?.ConfigModel as CutConfigModel).SeparateSaberCounts;
            set => (parentController?.ConfigModel as CutConfigModel).SeparateSaberCounts = value;
        }

        [UIValue("separate-values")]
        public bool SeparateCutValues
        {
            get => (parentController?.ConfigModel as CutConfigModel).SeparateCutValues;
            set => (parentController?.ConfigModel as CutConfigModel).SeparateCutValues = value;
        }

        [UIValue("average-precision")]
        public int AveragePrecision
        {
            get => (parentController?.ConfigModel as CutConfigModel).AveragePrecision;
            set => (parentController?.ConfigModel as CutConfigModel).AveragePrecision = value;
        }

        [UIValue("average-precision_values")]
        public List<object> AveragePrecisionValues => AdvancedCounterSettings.AverageCutPrecision.Cast<object>().ToList();


        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            parentController.ConfigChanged(obj);
        }
    }
}
