using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;

namespace CountersPlus.UI.ViewControllers.ConfigModelControllers.HUD
{
    class Camera : MonoBehaviour
    {
        private static readonly List<string> filteredCameras = new List<string>()
        {
            "MainMenuCamera"
        };

        [UIValue("attach_to_camera")]
        public bool AttachBaseGame
        {
            get => CountersController.settings.hudConfig.AttachHUDToCamera;
            set => CountersController.settings.hudConfig.AttachHUDToCamera = value;
        }

        [UIValue("camera")]
        public string AttachedCamera
        {
            get => CountersController.settings.hudConfig.AttachedCamera;
            set => CountersController.settings.hudConfig.AttachedCamera = value;
        }

        [UIValue("camera_values")]
        public List<object> CameraValues
        {
            get
            {
                List<string> cameras = Resources.FindObjectsOfTypeAll<UnityEngine.Camera>().Select(x => x.name).ToList();
                cameras.Add("MainCamera");
                cameras.RemoveAll(x => filteredCameras.Contains(x));
                return cameras.Cast<object>().ToList();
            }
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
