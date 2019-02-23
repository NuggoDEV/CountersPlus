using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CountersPlus.Config;

namespace CountersPlus.UI
{
    /// <summary>
    /// A Mock Counter to be used in the Counters+ UI menu, used for real time editing.
    /// It has no function, it is just to display data that an actual Counter would represent.
    /// </summary>
    public class MockCounter
    {
        public static Dictionary<IConfigModel, GameObject> loadedMockCounters = new Dictionary<IConfigModel, GameObject>();

        public static void Create<T>(T settings, string counterName, string counterData, bool UseCounterPositioning = true) where T : IConfigModel
        {
            if (!settings.Enabled) return;
            GameObject counter = new GameObject($"Counters + | Mock {counterName} Counter");

            GameObject nameGO = new GameObject($"Counters+ | Mock {counterName} Label");
            nameGO.transform.parent = counter.transform;
            TextMeshPro name = nameGO.AddComponent<TextMeshPro>();
            name.text = counterName;
            name.fontSize = 3;
            name.color = Color.white;
            name.alignment = TextAlignmentOptions.Center;
            name.rectTransform.localPosition = new Vector3(0, 0.4f, 0);

            TextMeshPro data = counter.AddComponent<TextMeshPro>();
            data.text = counterData;
            data.fontSize = 4;
            data.color = Color.white;
            data.alignment = TextAlignmentOptions.Center;

            counter.transform.position = CountersController.determinePosition(counter, settings.Position, settings.Index) - new Vector3(0, 0.4f, 0);
            if (!loadedMockCounters.Where((KeyValuePair < IConfigModel, GameObject > x) => x.Key == settings).Any())
                loadedMockCounters.Add(settings, counter);
            CountersPlusSettingsFlowCoordinator.Instance.mockCounters.Add(counter);
        }

        public static void CreateStatic(string counterName, string counterData)
        {
            GameObject counter = new GameObject($"Counters + | Mock {counterName} Counter");

            GameObject nameGO = new GameObject($"Counters+ | Mock {counterName} Label");
            nameGO.transform.parent = counter.transform;
            TextMeshPro name = nameGO.AddComponent<TextMeshPro>();
            name.text = counterName;
            name.fontSize = 3;
            name.color = Color.white;
            name.alignment = TextAlignmentOptions.Center;
            name.rectTransform.localPosition = new Vector3(0, 0.4f, 0);

            TextMeshPro data = counter.AddComponent<TextMeshPro>();
            data.text = counterData;
            data.fontSize = 4;
            data.color = Color.white;
            data.alignment = TextAlignmentOptions.Center;

            name.color = new Color(0.35f, 0.35f, 0.35f);
            data.color = new Color(0.35f, 0.35f, 0.35f);
            if (counterName == "Combo")
                counter.transform.position = new Vector3(-3.2f, 0.9f, 7);
            else if (counterName == "Multiplier")
                counter.transform.position = new Vector3(3.2f, 0.9f, 7);
            else if (counterName == "123 456")
                counter.transform.position = new Vector3(-3.2f, -0.1f, 7);

            CountersPlusSettingsFlowCoordinator.Instance.mockCounters.Add(counter);
        }
    }

    /// <summary>
    /// MockCounterInfo is used to create random statistics for Mock Counters to display while in the Counters+ UI menu.
    /// </summary>
    public class MockCounterInfo
    {
        public int notesCut;
        public int notesMissed;
        public int totalTime;
        public int timeElapsed;
        public float score;
        public float leftSpeedAverage;
        public float rightSpeedAverage;
        public float averageCutScore;
        public float leftSpinAverage;
        public float rightSpinAverage;

        public MockCounterInfo()
        {
            notesCut = UnityEngine.Random.Range(100, 250);
            notesMissed = UnityEngine.Random.Range(0, 15);
            totalTime = UnityEngine.Random.Range(75, 185);
            timeElapsed = UnityEngine.Random.Range(35, totalTime);
            score = UnityEngine.Random.Range(0.65f, 0.95f) * 100;
            leftSpeedAverage = UnityEngine.Random.Range(30, 85);
            rightSpeedAverage = UnityEngine.Random.Range(30, 85);
            averageCutScore = UnityEngine.Random.Range(85, 105);
            leftSpinAverage = UnityEngine.Random.Range(1000, 2500);
            rightSpinAverage = UnityEngine.Random.Range(1000, 2500);
        }

        public string GetRank()
        {
            float prec = score / 100;
            if (prec > 0.9f) return "SS";
            if (prec > 0.8f) return "S";
            return "A";
        }
    }
}
