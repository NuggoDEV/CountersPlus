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

        public static Dictionary<ConfigModel, MockCounterGroup> loadedMockCounters = new Dictionary<ConfigModel, MockCounterGroup>();
        private static List<MockCounterGroup> loadedStaticMockCounters = new List<MockCounterGroup>();
        public static ConfigModel HighlightedObject { get; private set; } = null;

        #region MockCounter Creation
        public static void Create<T>(T settings, string counterName, string counterData) where T : ConfigModel
        {
            if (!settings.Enabled) return;
            Vector3 position = DeterminePosition(settings) - new Vector3(0, 0.4f, 0);
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
            Object.Destroy(CounterName.gameObject);
            Object.Destroy(CounterData.gameObject);
        }
    }
}
