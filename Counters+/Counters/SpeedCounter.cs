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

        private float t;

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
                    break;
                case SpeedMode.Both:
                case SpeedMode.SplitBoth:
                    GenerateBasicText("Average Speed", out averageCounter);
                    var label = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0, -1, 0));
                    label.fontSize = 3;
                    label.text = "Recent Top Speed";
                    fastestCounter = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0, -1.4f, 0));
                    fastestCounter.text = "0";
                    break;
            }
        }

        public void Tick()
        {
            int precision = Settings.DecimalPrecision;
            // YES I AM USING GOTO TO LIMIT CODE DUPLICATION NOW STOP FLAMING ME
            switch (Settings.Mode)
            {
                case SpeedMode.Top5Sec:
                    TickFastestSpeed();
                    break;

                case SpeedMode.Both:
                    TickFastestSpeed();
                    goto case SpeedMode.Average;
                case SpeedMode.Average:
                    rSpeedList.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
                    averageCounter.text = rSpeedList.Average().ToString($"F{precision}");
                    break;

                case SpeedMode.SplitBoth:
                    TickFastestSpeed();
                    goto case SpeedMode.SplitAverage;
                case SpeedMode.SplitAverage:
                    rSpeedList.Add(right.bladeSpeed);
                    lSpeedList.Add(left.bladeSpeed);
                    averageCounter.text = $"{lSpeedList.Average().ToString($"F{precision}")} | {rSpeedList.Average().ToString($"F{precision}")}";
                    break;
            }
        }

        // Ticked function instead of IEnumerator because its legit just better
        private void TickFastestSpeed()
        {
            fastest.Add((right.bladeSpeed + left.bladeSpeed) / 2f);
            t += Time.deltaTime;
            if (t >= 5)
            {
                t = 0;
                var top = fastest.Max();
                fastest.Clear();
                fastestCounter.text = top.ToString($"F{Settings.DecimalPrecision}");
            }
        }
    }
}
