using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;
using System.Collections;

namespace CountersPlus.UI.ViewControllers.ConfigModelControllers
{
    class MainSettingsController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("enabled")]
        public bool Enabled
        {
            get => CountersController.settings.Enabled;
            set => CountersController.settings.Enabled = value;
        }

        [UIValue("advanced_mock")]
        public bool AdvancedMockCounters
        {
            get => CountersController.settings.AdvancedCounterInfo;
            set => CountersController.settings.AdvancedCounterInfo = value;
        }

        [UIValue("offset_values")]
        public List<object> OffsetValues => AdvancedCounterSettings.CounterOffsets.Cast<object>().ToList();

        [UIValue("combo_offset")]
        public float ComboOffset
        {
            get => CountersController.settings.ComboOffset;
            set => CountersController.settings.ComboOffset = value;
        }

        [UIValue("mult_offset")]
        public float MultiplierOffset
        {
            get => CountersController.settings.MultiplierOffset;
            set => CountersController.settings.MultiplierOffset = value;
        }


        [UIValue("hide_combo")]
        public bool HideCombo
        {
            get => CountersController.settings.HideCombo;
            set => CountersController.settings.HideCombo = value;
        }


        [UIValue("hide_mult")]
        public bool HideMultiplier
        {
            get => CountersController.settings.HideMultiplier;
            set => CountersController.settings.HideMultiplier = value;
        }

        [UIAction("update_model")]
        internal void UpdateMocksAndSaveModel(object obj)
        {
            if (gameObject.activeInHierarchy) StartCoroutine(WaitThenUpdate());
        }

        private IEnumerator WaitThenUpdate()
        {
            yield return new WaitForSeconds(0.1f);
            CountersPlusSettingsFlowCoordinator.UpdateMockCounters();
            CountersController.settings.Save();
        }
    }
}
