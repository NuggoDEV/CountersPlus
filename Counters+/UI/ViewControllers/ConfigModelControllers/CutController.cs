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

        [UIValue("separate")]
        public bool SeparateSaberCuts
        {
            get => (parentController?.ConfigModel as CutConfigModel).SeparateSaberCounts;
            set => (parentController?.ConfigModel as CutConfigModel).SeparateSaberCounts = value;
        }

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            parentController.ConfigChanged(obj);
        }
    }
}
