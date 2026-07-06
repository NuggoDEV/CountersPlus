using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using TMPro;

namespace CountersPlus.Counters
{
    internal class MissedCounter : Counter<MissedConfigModel>, INoteEventHandler
    {
        private int notesMissed = 0;
        private TMP_Text counter;
        private TMP_Text label;

        public override void CounterInit()
        {
            GenerateBasicText("Misses", out counter);
            label = counter.transform.parent.GetComponentInChildren<TMP_Text>(true);
            if (label == counter) label = null;
            
            if (Settings.HideWhenNone && counter != null)
            {
                counter.enabled = false;
                if (label != null) label.enabled = false;
            }
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (Settings.CountBadCuts && !info.allIsOK && data.colorType != ColorType.None)
            {
                UpdateMissCount();
            }
        }

        public void OnNoteMiss(NoteData data)
        {
            if (data.colorType != ColorType.None && (Settings.CountChainNoteMisses || data.gameplayType != NoteData.GameplayType.BurstSliderElement))
            {
                UpdateMissCount();
            }
        }

        private void UpdateMissCount()
        {
            notesMissed++;
            counter.text = notesMissed.ToString();

            if (Settings.HideWhenNone)
            {
                if (!counter.enabled) counter.enabled = true;
                if (label != null && !label.enabled) label.enabled = true;
            }
        }
    }
}
