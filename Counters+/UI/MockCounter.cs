using System.Collections.Generic;
using UnityEngine;
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

        public static Dictionary<string, MockCounterGroup> loadedMockCounters = new Dictionary<string, MockCounterGroup>();
        private static List<MockCounterGroup> loadedStaticMockCounters = new List<MockCounterGroup>();
        public static ConfigModel HighlightedObject { get; private set; } = null;

        #region MockCounter Creation
        public static void Create<T>(T settings, string counterName, string counterData) where T : ConfigModel
        {
            if (!settings.Enabled) return;
            Vector3 position = DeterminePosition(settings) - new Vector3(0, 0.4f, 0);
            TextHelper.CreateText(out TMP_Text name, position);
            name.gameObject.name = name.text = counterName;
            name.fontSize = 3;
            name.color = Color.white;
            name.alignment = TextAlignmentOptions.Center;

            TextHelper.CreateText(out TMP_Text data, position - new Vector3(0, 0.4f, 0));
            data.gameObject.name = data.text = counterData;
            data.fontSize = 4;
            data.color = Color.white;
            data.alignment = TextAlignmentOptions.Center;
            
            if (loadedMockCounters.ContainsKey(settings.DisplayName))
            {
                MockCounterGroup group = loadedMockCounters[settings.DisplayName];
                group.UpdatePosition(settings);
                if (HighlightedObject != null)
                {
                    group.UpdateColor(settings.DisplayName == HighlightedObject.DisplayName ? Color.yellow : Color.white);
                }
                return;
            }
            loadedMockCounters.Add(settings.DisplayName, new MockCounterGroup(name, data));
            if (HighlightedObject != null)
            {
                if (settings.DisplayName == HighlightedObject.DisplayName) name.color = data.color = Color.yellow;
            }
        }

        public static void CreateStatic(string counterName, string counterData)
        {
            Vector3 position = Vector3.zero;
            if (counterName == "Combo")
                position = new Vector3(-3.2f, 1.25f, 7);
            else if (counterName == "Multiplier")
                position = new Vector3(3.2f, 1.25f, 7);
            else if (counterName == "123 456")
                position = new Vector3(-3.2f, 0.35f, 7);

            TextHelper.CreateText(out TMP_Text name, position);
            name.gameObject.name = name.text = counterName;
            name.fontSize = 3;
            name.color = Color.gray;
            name.alignment = TextAlignmentOptions.Center;

            TextHelper.CreateText(out TMP_Text data, position - new Vector3(0, 0.4f, 0));
            data.gameObject.name = data.text = counterData;
            data.fontSize = 4;
            data.color = Color.gray;
            data.alignment = TextAlignmentOptions.Center;


            loadedStaticMockCounters.Add(new MockCounterGroup(name, data));
        }
        #endregion

        #region MockCounter Editing
        public static void Update<T>(T settings) where T : ConfigModel
        {
            if (settings is null) return;

            if (!settings.Enabled && loadedMockCounters.ContainsKey(settings.DisplayName))
            {
                MockCounterGroup group = loadedMockCounters[settings.DisplayName];
                group.DestroyText();
                loadedMockCounters.Remove(settings.DisplayName);
                return;
            }
            else if (settings.Enabled && !loadedMockCounters.ContainsKey(settings.DisplayName))
            {
                Create(settings, "", settings.DisplayName);
                return;
            }
            else if (settings.Enabled && loadedMockCounters.ContainsKey(settings.DisplayName))
            {
                loadedMockCounters[settings.DisplayName].UpdatePosition(settings);
            }
        }

        public static void Highlight<T>(T settings) where T : ConfigModel
        {
            if (settings != null)
            {
                if (HighlightedObject != null && loadedMockCounters.ContainsKey(HighlightedObject.DisplayName))
                {
                    loadedMockCounters[HighlightedObject.DisplayName].UpdateColor(Color.white);
                }
                if (loadedMockCounters.ContainsKey(settings.DisplayName)) {
                    loadedMockCounters[settings.DisplayName].UpdateColor(Color.yellow);
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
            HighlightedObject = null;
        }

        internal static Vector3 DeterminePosition<T>(T model) where T : ConfigModel
        {
            ICounterPositions position = model.Position;
            int index = model.Distance;
            Vector3 pos = new Vector3(); //Base position
            Vector3 offset = new Vector3(0, -0.75f * index, 0); //Offset for any overlapping, indexes, etc.
            float X = 3.2f;
            var settings = CountersController.settings;
            switch (position)
            {
                case ICounterPositions.BelowCombo:
                    pos = new Vector3(-X, 1.15f - settings.ComboOffset, 7);
                    break;
                case ICounterPositions.AboveCombo:
                    pos = new Vector3(-X, 2f + settings.ComboOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case ICounterPositions.BelowMultiplier:
                    pos = new Vector3(X, 1.05f - settings.MultiplierOffset, 7);
                    break;
                case ICounterPositions.AboveMultiplier:
                    pos = new Vector3(X, 2f + settings.MultiplierOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case ICounterPositions.BelowEnergy:
                    pos = new Vector3(0, -1.5f, 7);
                    break;
                case ICounterPositions.AboveHighway:
                    pos = new Vector3(0, 2.5f, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
            }
            return pos + offset;
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
            if (CounterName != null) Object.Destroy(CounterName.gameObject);
            if (CounterData != null) Object.Destroy(CounterData.gameObject);
        }

        public void UpdatePosition(ConfigModel settings)
        {
            if (settings is null) return;
            Vector3 position = MockCounter.DeterminePosition(settings) - new Vector3(0, 0.4f, 0);
            if (CounterName != null)
            {
                CounterName.rectTransform.anchoredPosition = position * TextHelper.PosScaleFactor;
            }
            if (CounterData != null)
            {
                CounterData.rectTransform.anchoredPosition = (position - new Vector3(0, 0.4f, 0)) * TextHelper.PosScaleFactor;
            }
        }
    }
}
