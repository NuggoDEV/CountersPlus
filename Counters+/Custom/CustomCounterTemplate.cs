using CountersPlus.Counters;
using TMPro;
using UnityEngine;

namespace CountersPlus.Custom
{
    public abstract class CustomCounterTemplate : Counter<CustomConfigModel>
    {
        public abstract string DisplayName { get; }
        public abstract string FormattedText { get; }

        private TMP_Text label;
        private TMP_Text counter;

        internal override void Init(CountersData data)
        {
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);

            TextHelper.CreateText(out counter, position - new Vector3(0, 0.4f, 0));
            counter.text = "Data";
            counter.fontSize = 4;
            counter.color = Color.white;
            counter.alignment = TextAlignmentOptions.Center;

            TextHelper.CreateText(out label, position);
            label.text = $"{settings.CustomCounter.Name}";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

            RefreshCounter();
        }

        public void RefreshCounter()
        {
            label.text = DisplayName;
            counter.text = FormattedText;
        }
    }
}
