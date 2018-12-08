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
        void Awake()
        {
            Plugin.Log("Random Positions has been enabled!");
            StartCoroutine(CountDownRNG());
        }

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
    }
}
