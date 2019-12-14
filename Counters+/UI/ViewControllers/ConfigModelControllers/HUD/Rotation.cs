using System.Collections;
using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;

namespace CountersPlus.UI.ViewControllers.ConfigModelControllers.HUD
{
    class Rotation : MonoBehaviour
    {
        [UIValue("rot_x")]
        public float rotX
        {
            get => CountersController.settings.hudConfig.HUDRotation_X;
            set => CountersController.settings.hudConfig.HUDRotation_X = value;
        }

        [UIValue("rot_y")]
        public float rotY
        {
            get => CountersController.settings.hudConfig.HUDRotation_Y;
            set => CountersController.settings.hudConfig.HUDRotation_Y = value;
        }

        [UIValue("rot_z")]
        public float rotZ
        {
            get => CountersController.settings.hudConfig.HUDRotation_Z;
            set => CountersController.settings.hudConfig.HUDRotation_Z = value;
        }

        [UIAction("update_model")]
        public void UpdateModel(object obj)
        {
            StartCoroutine(WaitBecauseFrickYou());
        }

        private IEnumerator WaitBecauseFrickYou()
        {
            yield return new WaitForSeconds(0.1f);
            TextHelper.CounterCanvas.transform.rotation = Quaternion.Euler(CountersController.settings.hudConfig.HUDRotation);
            CountersController.settings.hudConfig.Save();
        }
    }
}
