using System.Collections;
using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;

namespace CountersPlus.UI.ViewControllers.ConfigModelControllers.HUD
{
    class General : MonoBehaviour
    {
        [UIValue("attach_base_game")]
        public bool AttachBaseGame
        {
            get => CountersController.settings.hudConfig.AttachBaseGameHUD;
            set => CountersController.settings.hudConfig.AttachBaseGameHUD = value;
        }

        [UIValue("attach_to_base_game")]
        public bool AttachToBaseGame
        {
            get => CountersController.settings.hudConfig.AttachToBaseGameHUDFor360;
            set => CountersController.settings.hudConfig.AttachToBaseGameHUDFor360 = value;
        }

        [UIValue("hud_size")]
        public float HUDSize
        {
            get => CountersController.settings.hudConfig.HUDSize;
            set => CountersController.settings.hudConfig.HUDSize = value;
        }

        [UIValue("hud_scalar")]
        public float HUDScalar
        {
            get => CountersController.settings.hudConfig.HUDPositionScaleFactor;
            set => CountersController.settings.hudConfig.HUDPositionScaleFactor = value;
        }

        [UIAction("update_model")]
        public void UpdateModel(object obj)
        {
            StartCoroutine(WaitBecauseFrickYou());
        }

        private IEnumerator WaitBecauseFrickYou()
        {
            yield return new WaitForSeconds(0.1f);
            CountersController.settings.hudConfig.Save();
            CountersPlusSettingsFlowCoordinator.UpdateMockCounters();
        }
    }
}
