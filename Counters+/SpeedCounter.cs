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
    class SpeedCounter : MonoBehaviour
    {

        private PlayerController playerController;
        private SpeedConfigModel settings;
        private TextMeshPro counterText;
        private Saber right;
        private Saber left;
        private int counter;
        private int total;

        void Awake()
        {
            settings = CountersController.settings.speedConfig;
            transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            if (transform.parent == null)
                StartCoroutine(GetRequired());
            else
                Init();
        }

        IEnumerator GetRequired()
        {
            while (true)
            {
                playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
                if (playerController != null) break;
                yield return new WaitForSeconds(0.1f);
            }
            Init();
        }

        private void Init()
        {
            right = playerController.rightSaber;
            left = playerController.leftSaber;
            counterText = gameObject.AddComponent<TextMeshPro>();
            counterText.text = settings.CombinedSpeed ? "0" : "0 | 0";
            counterText.fontSize = 4;
            counterText.color = Color.white;
            counterText.alignment = TextAlignmentOptions.Center;
            counterText.rectTransform.position = new Vector3(0, -0.4f, 0);

            GameObject labelGO = new GameObject("Counters+ | Speed Label");
            labelGO.transform.parent = transform;
            TextMeshPro label = labelGO.AddComponent<TextMeshPro>();
            label.text = "Saber Speed";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;
        }

        void Update()
        {
            if (CountersController.rng)
            {
                settings.Index = UnityEngine.Random.Range(0, 5);
                settings.Position = (CounterPositions)UnityEngine.Random.Range(0, 4);
                settings.DecimalPrecision = UnityEngine.Random.Range(0, 5);
            }
            else
            {
                if (CountersController.settings.RNG)
                {
                    transform.position = Vector3.Lerp(
                    transform.position,
                    CountersController.determinePosition(gameObject, settings.Position, settings.Index),
                    Time.deltaTime);
                }else
                    transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            }
            if (settings.CombinedSpeed)
            {
                counterText.text = ((right.bladeSpeed + left.bladeSpeed) / 2).ToString("00.00");
            }
            else
            {
                counterText.text = string.Format("{0} | {1}", left.bladeSpeed.ToString("00.00"), right.bladeSpeed.ToString("00.00"));
                if (settings.ShowUnit) counterText.text += "\n<size=50%>m/s</size>";
            }
        }
    }
}
