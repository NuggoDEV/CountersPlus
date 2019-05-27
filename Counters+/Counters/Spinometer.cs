using CountersPlus.Config;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CountersPlus.Counters
{
    class Spinometer : MonoBehaviour
    {
        private Saber leftSaber;
        private Saber rightSaber;
        private List<float> rightAngles = new List<float>();
        private List<float> leftAngles = new List<float>();
        private List<Quaternion> rightQuaternions = new List<Quaternion>();
        private List<Quaternion> leftQuaternions = new List<Quaternion>();
        private float highestSpin;
        private TMP_Text spinometer;
        private SpinometerConfigModel settings;
        private int settingsMode; //Prevents calls to config

        void Awake()
        {
            settings = CountersController.settings.spinometerConfig;
            settingsMode = (int)settings.Mode;
            StartCoroutine(GetRequired());
        }

        IEnumerator GetRequired()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());
            Saber[] sabers = Resources.FindObjectsOfTypeAll<Saber>();
            leftSaber = sabers.Where((Saber x) => x.name.ToLower().Contains("left")).First();
            rightSaber = sabers.Where((Saber x) => x.name.ToLower().Contains("right")).First();
            Init();
        }

        void Init()
        {
            Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            TextHelper.CreateText(out spinometer, position - new Vector3(0, 0.4f, 0));
            spinometer.text = settings.Mode == ICounterMode.SplitAverage ? "0 | 0" : "0";
            spinometer.fontSize = 4;
            spinometer.color = Color.white;
            spinometer.alignment = TextAlignmentOptions.Center;

            GameObject labelGO = new GameObject("Counters+ | Spinometer Label");
            labelGO.transform.parent = transform;
            TextHelper.CreateText(out TMP_Text label, position);
            label.text = "Spinometer";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;
            StartCoroutine(SecondTick());
        }

        void Update()
        {
            try
            {
                leftQuaternions.Add(leftSaber.transform.rotation);
                rightQuaternions.Add(rightSaber.transform.rotation);
                if (leftQuaternions.Count > 0 && rightQuaternions.Count > 0)
                {
                    leftAngles.Add(Quaternion.Angle(leftQuaternions.Last(), leftQuaternions[leftQuaternions.Count - 2]));
                    rightAngles.Add(Quaternion.Angle(rightQuaternions.Last(), rightQuaternions[rightQuaternions.Count - 2]));
                }
            }
            catch { }
        }

        IEnumerator SecondTick()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                leftQuaternions.Clear();
                rightQuaternions.Clear();
                float leftSpeed = TotalSpeed(leftAngles);
                float rightSpeed = TotalSpeed(rightAngles);
                float averageSpeed = (leftSpeed + rightSpeed) / 2;
                if (leftSpeed > highestSpin) highestSpin = leftSpeed;
                if (rightSpeed > highestSpin) highestSpin = rightSpeed;
                if (settingsMode == (int)ICounterMode.Original)
                    spinometer.text = $"<color=#{DetermineColor(averageSpeed)}>{Mathf.RoundToInt(averageSpeed)}</color>";
                else if (settingsMode == (int)ICounterMode.Highest)
                    spinometer.text = $"<color=#{DetermineColor(highestSpin)}>{Mathf.RoundToInt(highestSpin)}</color>";
                else if (settingsMode == (int)ICounterMode.SplitAverage)
                    spinometer.text = $"<color=#{DetermineColor(leftSpeed)}>{Mathf.RoundToInt(leftSpeed)}</color> | <color=#{DetermineColor(rightSpeed)}>{Mathf.RoundToInt(rightSpeed)}</color>";
                leftAngles.Clear();
                rightAngles.Clear();
            }
        }

        private float TotalSpeed(List<float> speeds)
        {
            float speed = 0;
            foreach (float f in speeds) speed += f;
            return speed;
        }

        private string DetermineColor(float speed)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString("#FFA500", out Color orange);
            color = Color.Lerp(Color.white, orange, speed / 3600);
            if (speed >= 3600) color = Color.red;
            return ColorUtility.ToHtmlStringRGB(color);
        }
    }
}
