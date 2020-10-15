using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using TMPro;

namespace CountersPlus.Counters
{
    internal class MissedCounter : Counter<MissedConfigModel>, INoteEventHandler
    {
        private int notesMissed = 0;
        private TMP_Text counter;

        public override void CounterInit()
        {
            GenerateBasicText("Misses", out counter);
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (Settings.CountBadCuts && !info.allIsOK && data.colorType != ColorType.None) counter.text = (++notesMissed).ToString();
        }

        public void OnNoteMiss(NoteData data)
        {
            if (data.colorType != ColorType.None) counter.text = (++notesMissed).ToString();
        }
    }
}
