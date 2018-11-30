using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    class RandomizePositions : MonoBehaviour
    {

        private StandardLevelDetailViewController sldvc;
        private TextMeshPro missedText;
        private float beatsPerSecond;
        private float delay;
        private float superSecretSauce = 4;

        void Awake()
        {
            Plugin.Log("Random Positions has been enabled!");
            StartCoroutine(GetRequired());
        }

        IEnumerator GetRequired()
        {
            while (true)
            {
                sldvc = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
                if (sldvc != null) break;
                yield return new WaitForSeconds(0.1f);
            }

            Init();
        }

        private void Init()
        {
            transform.position = Vector3.zero;
            if (sldvc != null)
            {
                CountersController.FlagConfigForReload(false);
                IDifficultyBeatmap beatmap = sldvc.GetPrivateField<IDifficultyBeatmap>("_difficultyBeatmap");
                beatsPerSecond = beatmap.level.beatsPerMinute / 60;
                delay = beatsPerSecond * superSecretSauce;
                StartCoroutine(CountDownRNG());
            }
        }

        /*
         * "CAEDEN OPTIMIZE YOUR CODE"
         * 
         * No.
         */
        static IEnumerator CountDownRNG()
        {
            CountersController.rng = false;
            while (true)
            {
                yield return new WaitForSeconds(10);
                CountersController.rng = true;
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                CountersController.rng = false;
            }
        }

        bool IsConflicting(CounterPositions position, int Index)
        {
            if (CountersController.settings.missedConfig.Position == position && CountersController.settings.missedConfig.Index == Index) return true;
            if (CountersController.settings.accuracyConfig.Position == position && CountersController.settings.accuracyConfig.Index == Index) return true;
            if (CountersController.settings.scoreConfig.Position == position && CountersController.settings.scoreConfig.Index == Index) return true;
            if (CountersController.settings.progressConfig.Position == position && CountersController.settings.progressConfig.Index == Index) return true;
            return false;
        }
    }
}
