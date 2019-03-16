using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CountersPlus.Config;
using CountersPlus.Custom;

namespace CountersPlus.UI
{
    /// <summary>
    /// A Mock Counter to be used in the Counters+ UI menu, used for real time editing.
    /// It has no function, it is just to display data that an actual Counter would represent.
    /// </summary>
    public class MockCounter
    {

        public static Dictionary<MockCounterGroup, IConfigModel> loadedMockCounters = new Dictionary<MockCounterGroup, IConfigModel>();
        private static MockCounterInfo info = new MockCounterInfo();
        public static string highlightedObject { get; private set; } = null;

        #region MockCounter Creation
        public static void Create<T>(T settings, string counterName, string counterData, bool UseCounterPositioning = true) where T : IConfigModel
        {
            if (!settings.Enabled) return;
            GameObject counter = new GameObject($"Counters+ | Mock {counterName} Counter");
            GameObject nameGO = new GameObject($"Counters+ | Mock {counterName} Label");
            Vector3 position = (CountersController.determinePosition(counter, settings.Position, settings.Index) - new Vector3(0, 0.4f, 0));
            nameGO.transform.parent = counter.transform;
            //TextMeshPro name = nameGO.AddComponent<TextMeshPro>();
            TextHelper.CreateText(out TMP_Text name, position);
            name.text = counterName;
            name.fontSize = 3;
            name.color = Color.white;
            name.alignment = TextAlignmentOptions.Center;
            //name.rectTransform.localPosition = new Vector3(0, 0.4f, 0);

            //TextMeshPro data = counter.AddComponent<TextMeshPro>();
            TextHelper.CreateText(out TMP_Text data, position - new Vector3(0, 0.4f, 0));
            data.text = counterData;
            data.fontSize = 4;
            data.color = Color.white;
            data.alignment = TextAlignmentOptions.Center;
            
            if (loadedMockCounters.Where((KeyValuePair <MockCounterGroup, IConfigModel> x) => x.Value == settings).Any())
            {
                MockCounterGroup group = loadedMockCounters.First((KeyValuePair<MockCounterGroup, IConfigModel> x) => x.Value == settings).Key;
                UnityEngine.Object.Destroy(group.CounterName);
                UnityEngine.Object.Destroy(group.CounterData);
                loadedMockCounters.Remove(group);
            }
            if (settings.DisplayName == highlightedObject)
            {
                name.color = Color.yellow;
                data.color = Color.yellow;
            }
            loadedMockCounters.Add(new MockCounterGroup(name, data), settings);
        }

        public static void CreateStatic(string counterName, string counterData)
        {
            GameObject counter = new GameObject($"Counters+ | Static {counterName} Counter");
            GameObject nameGO = new GameObject($"Counters+ | Static {counterName} Label");
            Vector3 position = Vector3.zero;
            if (counterName == "Combo")
                position = new Vector3(-3.2f, 0.9f, 7);
            else if (counterName == "Multiplier")
                position = new Vector3(3.2f, 0.9f, 7);
            else if (counterName == "123 456")
                position = new Vector3(-3.2f, -0.1f, 7);
            nameGO.transform.parent = counter.transform;
            //TextMeshPro name = nameGO.AddComponent<TextMeshPro>();
            TextHelper.CreateText(out TMP_Text name, position);
            name.text = counterName;
            name.fontSize = 3;
            name.color = Color.white;
            name.alignment = TextAlignmentOptions.Center;

            //TextMeshPro data = counter.AddComponent<TextMeshPro>();
            TextHelper.CreateText(out TMP_Text data, position - new Vector3(0, 0.4f, 0));
            data.text = counterData;
            data.fontSize = 4;
            data.color = Color.white;
            data.alignment = TextAlignmentOptions.Center;

            name.color = new Color(0.35f, 0.35f, 0.35f);
            data.color = new Color(0.35f, 0.35f, 0.35f);
            loadedMockCounters.Add(new MockCounterGroup(name, data), null as IConfigModel);
        }
        #endregion

        #region MockCounter Editing
        public static void Update<T>(T settings) where T : IConfigModel
        {
            if (settings is null) return;

            //Mock Counter creation is here instead of CountersPlusSettingsFlowCoordinator.
            if (CountersController.settings.AdvancedCounterInfo)
            {
                if (settings is MissedConfigModel)
                    Create(settings, "Misses", info.notesMissed.ToString());
                else if (settings is NoteConfigModel)
                    Create(settings, "Notes",
                                $"{info.notesCut - info.notesMissed} / {info.notesCut} {(((settings as NoteConfigModel).ShowPercentage) ? $"- ({Math.Round((((double)(info.notesCut - info.notesMissed) / info.notesCut) * 100), (settings as NoteConfigModel).DecimalPrecision)}%)" : "")}");
                else if (settings is ScoreConfigModel)
                {
                    if ((settings as ScoreConfigModel).Mode == ICounterMode.BaseWithOutPoints || (settings as ScoreConfigModel).Mode == ICounterMode.LeavePoints || !(settings as ScoreConfigModel).Enabled)
                        CreateStatic("123 456", "");
                    else UnityEngine.Object.Destroy(GameObject.Find("Counters+ | Static 123 456 Counter"));
                    Create(settings, $"<size=50%>{(settings as ScoreConfigModel).Mode}</size> {Math.Round(info.score, (settings as ScoreConfigModel).DecimalPrecision).ToString()}%", (settings as ScoreConfigModel).DisplayRank ? info.GetRank() : "");
                }
                else if (settings is SpeedConfigModel)
                {
                    if ((settings as SpeedConfigModel).Mode == ICounterMode.Average || (settings as SpeedConfigModel).Mode == ICounterMode.Both)
                        Create(settings,
                                $"{((settings as SpeedConfigModel).Mode == ICounterMode.Both ? "Average (Both)" : "Average Speed")}", $"{Math.Round((info.leftSpeedAverage + info.rightSpeedAverage) / 2, (settings as SpeedConfigModel).DecimalPrecision)}");
                    else if ((settings as SpeedConfigModel).Mode == ICounterMode.SplitAverage || (settings as SpeedConfigModel).Mode == ICounterMode.SplitBoth)
                        Create(settings,
                                $"{((settings as SpeedConfigModel).Mode == ICounterMode.SplitBoth ? "Split Average (Both)" : "Split Average")}", $"{Math.Round(info.leftSpeedAverage, (settings as SpeedConfigModel).DecimalPrecision)} | {Math.Round(info.rightSpeedAverage, (settings as SpeedConfigModel).DecimalPrecision)}");
                    else if ((settings as SpeedConfigModel).Mode == ICounterMode.Top5Sec)
                        Create(settings, "Top Speed (5 Sec.)", $"{Math.Round(info.leftSpeedAverage + 10, (settings as SpeedConfigModel).DecimalPrecision)}");
                }
                else if (settings is CutConfigModel)
                    Create(settings, "Average Cut", $"{Mathf.RoundToInt(info.averageCutScore)}");
                else if (settings is ProgressConfigModel)
                    Create(settings,
                                $"{(settings as ProgressConfigModel).Mode.ToString()} Progress",
                                    $"{(((((settings as ProgressConfigModel).ProgressTimeLeft ? 1f : 0f) - ((float)info.timeElapsed / info.totalTime)) * 100f) * ((settings as ProgressConfigModel).ProgressTimeLeft ? 1f : -1f)).ToString("00")}%");
                else if (settings is SpinometerConfigModel)
                    Create(settings, "Spinometer",
                                $"{((settings as SpinometerConfigModel).Mode == ICounterMode.SplitAverage ? $"{info.leftSpinAverage} | {info.rightSpinAverage}" : $"{(info.leftSpinAverage + info.rightSpinAverage) / 2}")}");
            }
            else //Reduces calls to config. However with the new system I'm not sure if it matters all that much anymore.
            {
                if (settings is MissedConfigModel) Create(settings, "Missed", "0");
                else if (settings is NoteConfigModel) Create(settings, "Notes", "0 / 0");
                else if (settings is ScoreConfigModel) Create(settings, "100%", "SSS");
                else if (settings is SpeedConfigModel) Create(settings, "Average Speed", "0");
                else if (settings is CutConfigModel) Create(settings, "Average Cut", "0");
                else if (settings is ProgressConfigModel) Create(settings, "Progress", "0%");
                else if (settings is SpinometerConfigModel) Create(settings, "Spinometer", "0");
            }
            if (settings is CustomConfigModel) Create(settings, "", settings.DisplayName);
        }

        public static void Highlight<T>(T settings) where T : IConfigModel
        {
            foreach (KeyValuePair<MockCounterGroup, IConfigModel> kvp in loadedMockCounters)
            {
                if (kvp.Value is null) continue;
                if (settings.DisplayName == kvp.Value.DisplayName) highlightedObject = settings.DisplayName;
                kvp.Key.CounterName.color = (settings.DisplayName == kvp.Value.DisplayName) ? Color.yellow : Color.white;
                kvp.Key.CounterData.color = (settings.DisplayName == kvp.Value.DisplayName) ? Color.yellow : Color.white;
            }
        }
        #endregion
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

    public class MockCounterGroup
    {
        public TMP_Text CounterName;
        public TMP_Text CounterData;

        public MockCounterGroup(TMP_Text name, TMP_Text data)
        {
            CounterName = name;
            CounterData = data;
        }
    }
}
