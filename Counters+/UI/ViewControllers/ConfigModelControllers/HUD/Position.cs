using System.Collections;
using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;

namespace CountersPlus.UI.ViewControllers.ConfigModelControllers.HUD
{
    class Position : MonoBehaviour
    {
        [UIValue("pos_x")]
        public float PosX
        {
            get => CountersController.settings.hudConfig.HUDPosition_X;
            set => CountersController.settings.hudConfig.HUDPosition_X = value;
        }

        [UIValue("pos_y")]
        public float PosY
        {
            get => CountersController.settings.hudConfig.HUDPosition_Y;
            set => CountersController.settings.hudConfig.HUDPosition_Y = value;
        }

        [UIValue("pos_z")]
        public float PosZ
        {
            get => CountersController.settings.hudConfig.HUDPosition_Z;
            set => CountersController.settings.hudConfig.HUDPosition_Z = value;
        }

        [UIAction("update_model")]
        public void UpdateModel(object obj)
        {
            StartCoroutine(WaitBecauseFrickYou());
        }

        private IEnumerator WaitBecauseFrickYou()
        {
            yield return new WaitForSeconds(0.1f);
            TextHelper.CounterCanvas.transform.position = CountersController.settings.hudConfig.HUDPosition;
            CountersController.settings.hudConfig.Save();
        }
    }
}
