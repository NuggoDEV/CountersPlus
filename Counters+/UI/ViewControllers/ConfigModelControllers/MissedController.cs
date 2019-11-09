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
    class MissedController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("custommisstext")]
        public bool CustomMissTextIntegration {
            get => (parentController?.ConfigModel as MissedConfigModel).CustomMissTextIntegration;
            set => (parentController?.ConfigModel as MissedConfigModel).CustomMissTextIntegration = value;
        }

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            parentController.ConfigChanged(obj);
        }
    }
}
