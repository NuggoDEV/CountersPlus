using CountersPlus.ConfigModels;
using CountersPlus.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.UI
{
    public class MockCounter
    {
        private Dictionary<ConfigModel, TMP_Text> activeMockCounters = new Dictionary<ConfigModel, TMP_Text>();

        [Inject] private CanvasUtility canvasUtility;

        private ConfigModel highlightedConfig = null;

        public void UpdateMockCounter(ConfigModel settings)
        {
            if (activeMockCounters.TryGetValue(settings, out TMP_Text old))
            {
                if (old != null)
                {
                    Object.Destroy(old.gameObject);
                }
                activeMockCounters.Remove(settings);
            }

            if (!settings.Enabled) return;

            TMP_Text @new = canvasUtility.CreateTextFromSettings(settings);
            @new.text = settings.DisplayName;
            @new.color = highlightedConfig == settings ? Color.yellow : Color.white;
            activeMockCounters.Add(settings, @new);
        }

        public void HighlightCounter(ConfigModel settings)
        {
            if (highlightedConfig != null && activeMockCounters.TryGetValue(highlightedConfig, out TMP_Text old))
            {
                old.color = Color.white;
            }
            highlightedConfig = settings;
            if (activeMockCounters.TryGetValue(settings, out TMP_Text highlighted))
            {
                highlighted.color = Color.yellow;
            }
        }
    }
}
