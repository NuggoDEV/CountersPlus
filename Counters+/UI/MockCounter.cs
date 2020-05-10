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

        public static Dictionary<ConfigModel, MockCounterGroup> loadedMockCounters = new Dictionary<ConfigModel, MockCounterGroup>();
        private static List<MockCounterGroup> loadedStaticMockCounters = new List<MockCounterGroup>();
        public static ConfigModel HighlightedObject { get; private set; } = null;

        #region MockCounter Creation
        public static void Create<T>(T settings, string counterName, string counterData) where T : ConfigModel
        {
            if (!settings.Enabled) return;
            GameObject counter = new GameObject($"Counters+ | Mock {counterName} Counter");
            GameObject nameGO = new GameObject($"Counters+ | Mock {counterName} Label");
            Vector3 position = (CountersController.DeterminePosition(counter, settings.Position, settings.Distance) - new Vector3(0, 0.4f, 0));
            nameGO.transform.parent = counter.transform;
            TextHelper.CreateText(out TMP_Text name, position);
            name.text = counterName;
            name.fontSize = 3;
            name.color = Color.white;
            name.alignment = TextAlignmentOptions.Center;

            TextHelper.CreateText(out TMP_Text data, position - new Vector3(0, 0.4f, 0));
            data.text = counterData;
            data.fontSize = 4;
            data.color = Color.white;
            data.alignment = TextAlignmentOptions.Center;
            
            if (loadedMockCounters.ContainsKey(settings))
            {
                MockCounterGroup group = loadedMockCounters[settings];
                group.DestroyText();
                loadedMockCounters.Remove(settings);
            }
            loadedMockCounters.Add(settings, new MockCounterGroup(name, data));
            if (HighlightedObject != null)
            {
                if (settings.DisplayName == HighlightedObject.DisplayName) name.color = data.color = Color.yellow;
            }
        }

        public static void CreateStatic(string counterName, string counterData)
        {
            GameObject counter = new GameObject($"Counters+ | Static {counterName} Counter");
            GameObject nameGO = new GameObject($"Counters+ | Static {counterName} Label");
            Vector3 position = Vector3.zero;
            if (counterName == "Combo")
                position = new Vector3(-3.2f, 1.25f, 7);
            else if (counterName == "Multiplier")
                position = new Vector3(3.2f, 1.25f, 7);
            else if (counterName == "123 456")
                position = new Vector3(-3.2f, 0.35f, 7);
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


            loadedStaticMockCounters.Add(new MockCounterGroup(name, data));
        }
        #endregion

        #region MockCounter Editing
        public static void Update<T>(T settings) where T : ConfigModel
        {
            if (settings is null) return;

            if (!settings.Enabled && loadedMockCounters.ContainsKey(settings))
            {
                MockCounterGroup group = loadedMockCounters[settings];
                group.DestroyText();
                loadedMockCounters.Remove(settings);
            }

            Create(settings, "", settings.DisplayName);
        }

        public static void Highlight<T>(T settings) where T : ConfigModel
        {
            if (settings != null)
            {
                if (HighlightedObject != null && loadedMockCounters.ContainsKey(HighlightedObject))
                {
                    loadedMockCounters[HighlightedObject].UpdateColor(Color.white);
                }
                if (loadedMockCounters.ContainsKey(settings)) {
                    loadedMockCounters[settings].UpdateColor(Color.yellow);
                }
                HighlightedObject = settings;
            }
            else
            {
                HighlightedObject = null;
                foreach (var kvp in loadedMockCounters) kvp.Value.UpdateColor(Color.white);
            }
        }
        #endregion

        public static void ClearAllMockCounters()
        {
            foreach (var kvp in loadedMockCounters) kvp.Value.DestroyText();
            foreach (var group in loadedStaticMockCounters) group.DestroyText();
            loadedMockCounters.Clear();
            loadedStaticMockCounters.Clear();
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

        public void UpdateColor(Color color)
        {
            CounterName.color = color;
            CounterData.color = color;
        }

        public void DestroyText()
        {
            UnityEngine.Object.Destroy(CounterName);
            UnityEngine.Object.Destroy(CounterData);
        }
    }
}
