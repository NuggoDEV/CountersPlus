using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    class SpeedCounter : MonoBehaviour
    {
        private SpeedConfigModel settings;
        private TMP_Text counterText;
        private TMP_Text altCounterText;
        private PlayerController pc = null;
        private Saber right;
        private Saber left;
        private List<float> rSpeedList = new List<float>();
        private List<float> lSpeedList = new List<float>();
        private List<float> fastest = new List<float>();
        private string precision = "00.";

        private int settingsMode;

        void Awake()
        {
            settings = CountersController.settings.speedConfig;
            settingsMode = (int)settings.Mode;
            for (var i = 0; i < settings.DecimalPrecision; i++) precision += "0";
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            pc = data.PlayerController;
            right = data.PlayerController.rightSaber;
            left = data.PlayerController.leftSaber;
            if (settings.Mode == ICounterMode.Average || settings.Mode == ICounterMode.SplitAverage)
            {
                TextHelper.CreateText(out counterText, position - new Vector3(0, 0.4f, 0));
                counterText.text = settings.Mode == ICounterMode.Average ? "0" : "0 | 0";
                counterText.fontSize = 4;
                counterText.color = Color.white;
                counterText.alignment = TextAlignmentOptions.Center;

                GameObject labelGO = new GameObject("Counters+ | Speed Label");
                labelGO.transform.parent = transform;
                TextHelper.CreateText(out TMP_Text label, position);
                label.text = "Average Speed";
                label.fontSize = 3;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;
            }else if (settings.Mode == ICounterMode.Top5Sec)
            {
                TextHelper.CreateText(out counterText, position - new Vector3(0, 0.4f, 0));
                counterText.text = "00.00";
                counterText.fontSize = 4;
                counterText.color = Color.white;
                counterText.alignment = TextAlignmentOptions.Center;

                GameObject labelGO = new GameObject("Counters+ | Highest Speed Label");
                labelGO.transform.parent = transform;
                TextHelper.CreateText(out TMP_Text label, position);
                label.text = "Top Speed (5 Sec.)";
                label.fontSize = 3;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;

                StartCoroutine(FastestSpeed());
            }else if (settings.Mode == ICounterMode.Both || settings.Mode == ICounterMode.SplitBoth){
                TextHelper.CreateText(out counterText, position - new Vector3(0, 0.4f, 0));
                counterText.text = settings.Mode == ICounterMode.Both ? "0" : "0 | 0";
                counterText.fontSize = 4;
                counterText.color = Color.white;
                counterText.alignment = TextAlignmentOptions.Center;

                GameObject labelGO = new GameObject("Counters+ | Speed Label");
                labelGO.transform.parent = transform;
                TextHelper.CreateText(out TMP_Text label, position);
                label.text = "Average Speed";
                label.fontSize = 3;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;

                GameObject altGO = new GameObject("Counters+ | Highest Speed");
                altGO.transform.parent = transform;
                TextHelper.CreateText(out altCounterText, position - new Vector3(0, 1.1f, 0));
                altCounterText.text = "00.00";
                altCounterText.fontSize = 4;
                altCounterText.color = Color.white;
                altCounterText.alignment = TextAlignmentOptions.Center;

                GameObject altLabelGO = new GameObject("Counters+ | Highest Speed Label");
                altLabelGO.transform.parent = altGO.transform;
                TextHelper.CreateText(out TMP_Text altLabel, position - new Vector3(0, 0.7f, 0));
                altLabel.text = "Top Speed (5 Sec.)";
                altLabel.fontSize = 3;
                altLabel.color = Color.white;
                altLabel.alignment = TextAlignmentOptions.Center;

                if (settings.Position == ICounterPositions.AboveCombo || settings.Position == ICounterPositions.AboveHighway || settings.Position == ICounterPositions.AboveMultiplier)
                    altGO.transform.position += new Vector3(0, 1f, 0);
                else
                    altGO.transform.position += new Vector3(0, -0.75f, 0);
                StartCoroutine(FastestSpeed());
            }
        }

        void OnDestroy()
        {
            CountersController.ReadyToInit -= Init;
        }

        IEnumerator FastestSpeed()
        {
            while (true)
            {
                yield return new WaitForSeconds(5);
                fastest.Add((this.right.bladeSpeed + this.left.bladeSpeed) / 2f);
                float top = 0;
                foreach (float speed in fastest) if (speed > top) top = speed;
                fastest.Clear();
                if (settings.Mode == ICounterMode.Both || settings.Mode == ICounterMode.SplitBoth)
                    altCounterText.text = top.ToString(precision);
                else
                    counterText.text = top.ToString(precision);
            }
        }

        void Update()
        {
            if (pc == null) return;
            switch (settingsMode)
            {
                case (int)ICounterMode.Average:
                    rSpeedList.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    counterText.text = rSpeedList.Average().ToString(precision);
                    break;
                case (int)ICounterMode.Top5Sec:
                    fastest.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    break;
                case (int)ICounterMode.Both:
                    fastest.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    rSpeedList.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    counterText.text = rSpeedList.Average().ToString(precision);
                    break;
                case (int)ICounterMode.SplitAverage:
                    rSpeedList.Add(right.bladeSpeed);
                    lSpeedList.Add(left.bladeSpeed);
                    counterText.text = string.Format("{0} | {1}", lSpeedList.Average().ToString(precision), rSpeedList.Average().ToString(precision));
                    break;
                case (int)ICounterMode.SplitBoth:
                    fastest.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    rSpeedList.Add(right.bladeSpeed);
                    lSpeedList.Add(left.bladeSpeed);
                    counterText.text = string.Format("{0} | {1}", lSpeedList.Average().ToString(precision), rSpeedList.Average().ToString(precision));
                    break;
            }
        }
    }
}
