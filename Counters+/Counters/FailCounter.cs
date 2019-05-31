using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CountersPlus.Config;
using UnityEngine;
using TMPro;
using System.Collections;

namespace CountersPlus.Counters
{
    class FailCounter : MonoBehaviour
    {
        private FailConfigModel settings;
        private TMP_Text failText;
        private GameEnergyCounter energy;
        private int fails = 0;

        private void Awake()
        {
            settings = CountersController.settings.failsConfig;
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            //Because CountersController.ReadyToInit relies on finding other objects via Resources.FindObjectsOfTypeAll<>()
            //before invoking, it is safe to assume that the objects we find do indeed exist.
            energy = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().First();
            fails = data.PlayerData.currentLocalPlayer.playerAllOverallStatsData.allOverallStatsData.failedLevelsCount;
            Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            TextHelper.CreateText(out failText, position - new Vector3(0, 0.4f, 0));
            failText.text = fails.ToString();
            failText.fontSize = 4;
            failText.color = Color.white;
            failText.alignment = TextAlignmentOptions.Center;

            GameObject labelGO = new GameObject("Counters+ | Fail Label");
            labelGO.transform.parent = transform;
            TextHelper.CreateText(out TMP_Text label, position);
            label.text = "Fails";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

            energy.gameEnergyDidReach0Event += SlowlyAnnoyThePlayerBecauseTheyFailed;
        }

        private void SlowlyAnnoyThePlayerBecauseTheyFailed()
        {
            failText.text = (fails + 1).ToString();
            StartCoroutine(ChangeTextColorToAnnoyThePlayerEvenMore());
        }

        private IEnumerator ChangeTextColorToAnnoyThePlayerEvenMore()
        {
            float t = 0;
            while (t <= 1)
            {
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
                failText.color = Color.Lerp(Color.white, Color.red, t);
            }
            failText.color = Color.red;
        }

        private void OnDestroy()
        {
            energy.gameEnergyDidReach0Event -= SlowlyAnnoyThePlayerBecauseTheyFailed;
            CountersController.ReadyToInit -= Init;
        }
    }
}
