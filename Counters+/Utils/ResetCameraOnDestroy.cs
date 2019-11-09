using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CountersPlus.Harmony;

namespace CountersPlus.Utils
{
    internal class ResetCameraOnDestroy : MonoBehaviour
    {
        private void OnDestroy()
        {
            if (CountersController.settings.FloatingHUD) FloatingOverlayWindowUpdate.ResetCamera();
        }
    }
}
