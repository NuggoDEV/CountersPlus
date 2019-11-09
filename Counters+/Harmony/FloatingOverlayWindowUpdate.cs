using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using UnityEngine.XR;

namespace CountersPlus.Harmony
{
    [HarmonyPatch(typeof(FloatingOverlayWindow))]
    [HarmonyPatch("Update")]
    class FloatingOverlayWindowUpdate
    {
        internal static SmoothCamera smooth = null;
        static bool Prefix(ref FloatingOverlayWindow __instance, float ____zDistance)
        {
            //if (!__instance.transform.name.Contains("Counters+")) return true; //Dont mess with non Counters+
            if (smooth is null)
            {
                smooth = Resources.FindObjectsOfTypeAll<SmoothCamera>().FirstOrDefault(x => x.gameObject.activeInHierarchy); //Find smooth camera component
                return true;
            }
            __instance.transform.SetPositionAndRotation(smooth.transform.position, smooth.transform.rotation);
            __instance.transform.localPosition = __instance.transform.localPosition + __instance.transform.forward * ____zDistance;
            return false;
        }

        internal static void ResetCamera()
        {
            smooth = null;
        }
    }
}
