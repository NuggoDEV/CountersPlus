using CountersPlus.ConfigModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.Counters
{
    internal class Spinometer : Counter<SpinometerConfigModel>, ITickable
    {
        [Inject] private SaberManager saberManager;

        private Saber leftSaber = null;
        private Saber rightSaber = null;
        private List<float> rightAngles = new List<float>();
        private List<float> leftAngles = new List<float>();
        private List<Quaternion> rightQuaternions = new List<Quaternion>();
        private List<Quaternion> leftQuaternions = new List<Quaternion>();
        private float highestSpin;
        private TMP_Text spinometer;

        public override void CounterInit()
        {
            leftSaber = saberManager.leftSaber;
            rightSaber = saberManager.rightSaber;
            GenerateBasicText("Spinometer", out spinometer);
            SharedCoroutineStarter.instance.StartCoroutine(SecondTick());
        }

        public void Tick()
        {
            if (leftSaber != saberManager.leftSaber)
            {
                leftSaber = saberManager.leftSaber;
            }
            if (rightSaber != saberManager.rightSaber)
            {
                rightSaber = saberManager.rightSaber;
            }
            leftQuaternions.Add(leftSaber.transform.rotation);
            rightQuaternions.Add(rightSaber.transform.rotation);
            if (leftQuaternions.Count >= 2 && rightQuaternions.Count >= 2)
            {
                leftAngles.Add(Quaternion.Angle(leftQuaternions.Last(), leftQuaternions[leftQuaternions.Count - 2]));
                rightAngles.Add(Quaternion.Angle(rightQuaternions.Last(), rightQuaternions[rightQuaternions.Count - 2]));
            }
        }

        private IEnumerator SecondTick()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                leftQuaternions.Clear();
                rightQuaternions.Clear();
                float leftSpeed = leftAngles.Sum();
                float rightSpeed = rightAngles.Sum();
                leftAngles.Clear();
                rightAngles.Clear();
                float averageSpeed = (leftSpeed + rightSpeed) / 2;
                if (leftSpeed > highestSpin) highestSpin = leftSpeed;
                if (rightSpeed > highestSpin) highestSpin = rightSpeed;

                switch (Settings.Mode)
                {
                    case SpinometerMode.Average:
                        spinometer.text = $"<color=#{DetermineColor(averageSpeed)}>{Mathf.RoundToInt(averageSpeed)}</color>";
                        break;
                    case SpinometerMode.Highest:
                        spinometer.text = $"<color=#{DetermineColor(highestSpin)}>{Mathf.RoundToInt(highestSpin)}</color>";
                        break;
                    case SpinometerMode.SplitAverage:
                        spinometer.text = $"<color=#{DetermineColor(leftSpeed)}>{Mathf.RoundToInt(leftSpeed)}</color> | <color=#{DetermineColor(rightSpeed)}>{Mathf.RoundToInt(rightSpeed)}</color>";       
                        break;
                }
            }
        }

        private string DetermineColor(float speed)
        {
            ColorUtility.TryParseHtmlString("#FFA500", out Color orange);
            Color color = Color.Lerp(Color.white, orange, speed / 3600);
            if (speed >= 3600) color = Color.red;
            return ColorUtility.ToHtmlStringRGB(color);
        }
    }
}
