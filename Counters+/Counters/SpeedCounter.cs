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
        private TextMeshPro altCounterText;
        private Saber right;
        private Saber left;
        private List<float> rSpeedList = new List<float>();
        private List<float> lSpeedList = new List<float>();
        private List<float> fastest = new List<float>();
        private string precision = "00.";

        void Awake()
        {
            settings = CountersController.settings.speedConfig;
            transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            StartCoroutine(GetRequired());
            for (var i = 0; i < settings.DecimalPrecision; i++)
            {
                precision += "0";
            }
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
            if (settings.Mode == SpeedConfigModel.CounterMode.Average || settings.Mode == SpeedConfigModel.CounterMode.SplitAverage)
            {
                counterText = gameObject.AddComponent<TextMeshPro>();
                counterText.text = settings.Mode == SpeedConfigModel.CounterMode.Average ? "0" : "0 | 0";
                counterText.fontSize = 4;
                counterText.color = Color.white;
                counterText.alignment = TextAlignmentOptions.Center;
                counterText.rectTransform.position = new Vector3(0, -0.4f, 0);

                GameObject labelGO = new GameObject("Counters+ | Speed Label");
                labelGO.transform.parent = transform;
                TextMeshPro label = labelGO.AddComponent<TextMeshPro>();
                label.text = "Average Speed";
                label.fontSize = 3;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;
            }else if (settings.Mode == SpeedConfigModel.CounterMode.Top5Sec)
            {
                counterText = gameObject.AddComponent<TextMeshPro>();
                counterText.text = "00.00";
                counterText.fontSize = 4;
                counterText.color = Color.white;
                counterText.alignment = TextAlignmentOptions.Center;
                counterText.rectTransform.position = new Vector3(0, -0.4f, 0);

                GameObject labelGO = new GameObject("Counters+ | Highest Speed Label");
                labelGO.transform.parent = transform;
                TextMeshPro label = labelGO.AddComponent<TextMeshPro>();
                label.text = "Top Speed (5 Sec.)";
                label.fontSize = 3;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;

                StartCoroutine(FastestSpeed());
            }else if (settings.Mode == SpeedConfigModel.CounterMode.Both || settings.Mode == SpeedConfigModel.CounterMode.SplitBoth){
                counterText = gameObject.AddComponent<TextMeshPro>();
                counterText.text = settings.Mode == SpeedConfigModel.CounterMode.Both ? "0" : "0 | 0";
                counterText.fontSize = 4;
                counterText.color = Color.white;
                counterText.alignment = TextAlignmentOptions.Center;
                counterText.rectTransform.position = new Vector3(0, -0.4f, 0);

                GameObject labelGO = new GameObject("Counters+ | Speed Label");
                labelGO.transform.parent = transform;
                TextMeshPro label = labelGO.AddComponent<TextMeshPro>();
                label.text = "Average Speed";
                label.fontSize = 3;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;

                GameObject altGO = new GameObject("Counters+ | Highest Speed");
                altGO.transform.parent = transform;
                altCounterText = altGO.AddComponent<TextMeshPro>();
                altCounterText.text = "00.00";
                altCounterText.fontSize = 4;
                altCounterText.color = Color.white;
                altCounterText.alignment = TextAlignmentOptions.Center;
                altCounterText.rectTransform.position = new Vector3(0, -0.4f, 0);

                GameObject altLabelGO = new GameObject("Counters+ | Highest Speed Label");
                altLabelGO.transform.parent = transform;
                TextMeshPro altLabel = altLabelGO.AddComponent<TextMeshPro>();
                altLabel.text = "Top Speed (5 Sec.)";
                altLabel.fontSize = 3;
                altLabel.color = Color.white;
                altLabel.alignment = TextAlignmentOptions.Center;

                if (settings.Position == CounterPositions.AboveCombo || settings.Position == CounterPositions.AboveHighway || settings.Position == CounterPositions.AboveMultiplier)
                {
                    altCounterText.rectTransform.position += new Vector3(0, 1, 0);
                    altLabel.rectTransform.position += new Vector3(0, 1, 0);
                }
                else
                {
                    altCounterText.rectTransform.position += new Vector3(0, -0.75f, 0);
                    altLabel.rectTransform.position += new Vector3(0, -0.75f, 0);
                }
                StartCoroutine(FastestSpeed());
            }
        }

        IEnumerator FastestSpeed()
        {
            while (true)
            {
                yield return new WaitForSeconds(5);
                fastest.Add((this.right.bladeSpeed + this.left.bladeSpeed) / 2f);
                float top = 0;
                foreach (float speed in fastest)
                {
                    if (speed > top) top = speed;
                }
                fastest.Clear();
                if (settings.Mode == SpeedConfigModel.CounterMode.Both || settings.Mode == SpeedConfigModel.CounterMode.SplitBoth)
                    altCounterText.text = top.ToString(precision);
                else
                    counterText.text = top.ToString(precision);
            }
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
            if (settings.Mode == SpeedConfigModel.CounterMode.Average)
            {
                rSpeedList.Add((this.right.bladeSpeed + this.left.bladeSpeed) / 2f);
                float average = 0;
                foreach (float speed in rSpeedList)
                {
                    average += speed;
                }
                average /= rSpeedList.Count;
                counterText.text = average.ToString(precision);
            }
            else if (settings.Mode == SpeedConfigModel.CounterMode.SplitAverage)
            {
                rSpeedList.Add(right.bladeSpeed);
                float rAverage = 0;
                foreach (float speed in rSpeedList)
                {
                    rAverage += speed;
                }
                rAverage /= rSpeedList.Count;
                lSpeedList.Add(left.bladeSpeed);
                float lAverage = 0;
                foreach (float speed in lSpeedList)
                {
                    lAverage += speed;
                }
                lAverage /= lSpeedList.Count;
                counterText.text = string.Format("{0} | {1}", lAverage.ToString(precision), rAverage.ToString(precision));
            }else if (settings.Mode == SpeedConfigModel.CounterMode.Top5Sec)
            {
                fastest.Add((this.right.bladeSpeed + this.left.bladeSpeed) / 2f);
            }
            else
            {
                fastest.Add((this.right.bladeSpeed + this.left.bladeSpeed) / 2f);
                if (settings.Mode == SpeedConfigModel.CounterMode.Both)
                {
                    rSpeedList.Add((this.right.bladeSpeed + this.left.bladeSpeed) / 2f);
                    float average = 0;
                    foreach (float speed in rSpeedList)
                    {
                        average += speed;
                    }
                    average /= rSpeedList.Count;
                    counterText.text = average.ToString(precision);
                }
                else if (settings.Mode == SpeedConfigModel.CounterMode.SplitBoth)
                {
                    rSpeedList.Add(right.bladeSpeed);
                    float rAverage = 0;
                    foreach (float speed in rSpeedList)
                    {
                        rAverage += speed;
                    }
                    rAverage /= rSpeedList.Count;
                    lSpeedList.Add(left.bladeSpeed);
                    float lAverage = 0;
                    foreach (float speed in lSpeedList)
                    {
                        lAverage += speed;
                    }
                    lAverage /= lSpeedList.Count;
                    counterText.text = string.Format("{0} | {1}", lAverage.ToString(precision), rAverage.ToString(precision));
                }
            }
        }
    }
}
