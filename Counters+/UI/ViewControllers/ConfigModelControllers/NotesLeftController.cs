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
    class NotesLeftController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("label_above")]
        public bool LabelAboveCounter
        {
            get => (parentController?.ConfigModel as NotesLeftConfigModel).LabelAboveCount;
            set => (parentController?.ConfigModel as NotesLeftConfigModel).LabelAboveCount = value;
        }

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            parentController.ConfigChanged(obj);
        }
    }
}
