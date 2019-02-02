using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using VRUI;

namespace CountersPlus.UI
{
    class CountersPlusSettingsListViewController : CustomListViewController
    {
        internal static void DidActivateLeft(bool firstActivation, ActivationType type)
        {
            try
            {
                if (firstActivation)
                {
                    
                }
            }
            catch (Exception e){
                Plugin.Log(e.ToString(), Plugin.LogInfo.Fatal);
            }
        }
    }
}
