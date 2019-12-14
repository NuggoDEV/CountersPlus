using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CountersPlus.Utils
{
    class AssignedFloatingWindow : MonoBehaviour
    {
        private Camera assignedCamera;

        private void Start()
        {
            List<Camera> cameras = Resources.FindObjectsOfTypeAll<Camera>().ToList();
            assignedCamera = cameras.Where(x => x.name == CountersController.settings.hudConfig.AttachedCamera).FirstOrDefault();
        }

        private void Update()
        {
            if (assignedCamera is null) return;
            transform.SetPositionAndRotation(assignedCamera.transform.position, assignedCamera.transform.rotation);
            Vector3 side = assignedCamera.transform.right * CountersController.settings.hudConfig.HUDPosition_X;
            Vector3 up = assignedCamera.transform.up * CountersController.settings.hudConfig.HUDPosition_Y;
            Vector3 forward = assignedCamera.transform.forward * CountersController.settings.hudConfig.HUDPosition_Z;
            Vector3 total = side + up + forward;
            transform.localPosition = assignedCamera.transform.position + total;
        }

        public void AssignCamera(Camera camera)
        {
            assignedCamera = camera;
        }
    }
}
