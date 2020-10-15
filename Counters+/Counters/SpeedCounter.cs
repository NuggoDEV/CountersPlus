using CountersPlus.ConfigModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.Counters
{
    internal class SpeedCounter : Counter<SpeedConfigModel>, ITickable
    {
        [Inject] private SaberManager saberManager;

        private Saber right;
        private Saber left;
        private List<float> rSpeedList = new List<float>();
        private List<float> lSpeedList = new List<float>();
        private List<float> fastest = new List<float>();
        private TMP_Text averageCounter;
        private TMP_Text fastestCounter;

        public override void CounterInit()
        {
            right = saberManager.rightSaber;
            left = saberManager.leftSaber;

            switch (Settings.Mode)
            {
                case SpeedMode.Average:
                case SpeedMode.SplitAverage:
                    GenerateBasicText("Average Speed", out averageCounter);
                    break;
                case SpeedMode.Top5Sec:
                    GenerateBasicText("Recent Top Speed", out fastestCounter);
                    SharedCoroutineStarter.instance.StartCoroutine(FastestSpeed());
                    break;
                case SpeedMode.Both:
                case SpeedMode.SplitBoth:
                    GenerateBasicText("Average Speed", out averageCounter);
                    var label = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0, -1, 0));
                    label.fontSize = 3;
                    label.text = "Recent Top Speed";
                    fastestCounter = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0, -1.4f, 0));
                    SharedCoroutineStarter.instance.StartCoroutine(FastestSpeed());
                    break;
            }
        }

        private IEnumerator FastestSpeed()
        {
            while (true)
            {
                yield return new WaitForSeconds(5);
                float top = fastest.Max();
                fastest.Clear();
                fastestCounter.text = top.ToString($"F{Settings.DecimalPrecision}");
            }
        }

        public void Tick()
        {
            int precision = Settings.DecimalPrecision;
            switch (Settings.Mode)
            {
                case SpeedMode.Average:
                    rSpeedList.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    averageCounter.text = rSpeedList.Average().ToString($"F{precision}");
                    break;
                case SpeedMode.Top5Sec:
                    fastest.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    break;
                case SpeedMode.Both:
                    fastest.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    rSpeedList.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    averageCounter.text = rSpeedList.Average().ToString($"F{precision}");
                    break;
                case SpeedMode.SplitAverage:
                    rSpeedList.Add(right.bladeSpeed);
                    lSpeedList.Add(left.bladeSpeed);
                    averageCounter.text = $"{lSpeedList.Average().ToString($"F{precision}")} | {rSpeedList.Average().ToString($"F{precision}")}";
                    break;
                case SpeedMode.SplitBoth:
                    fastest.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    rSpeedList.Add(right.bladeSpeed);
                    lSpeedList.Add(left.bladeSpeed);
                    averageCounter.text = $"{lSpeedList.Average().ToString($"F{precision}")} | {rSpeedList.Average().ToString($"F{precision}")}";
                    break;
            }
        }
    }
}
