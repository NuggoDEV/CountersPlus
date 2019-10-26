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
    class NotesController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("showpercentage")]
        public bool ShowPercentage
        {
            get => (parentController?.ConfigModel as NoteConfigModel).ShowPercentage;
            set => (parentController?.ConfigModel as NoteConfigModel).ShowPercentage = value;
        }

        [UIValue("precision")]
        public int PercentagePrecision
        {
            get => (parentController?.ConfigModel as NoteConfigModel).DecimalPrecision;
            set => (parentController?.ConfigModel as NoteConfigModel).DecimalPrecision = value;
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
