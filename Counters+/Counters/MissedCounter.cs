using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using TMPro;

namespace CountersPlus.Counters
{
    public class MissedCounter : Counter<MissedConfigModel>, INoteEventHandler
    {
        private int notesMissed = 0;
        private TMP_Text counter;

        public override void CounterInit()
        {
            GenerateBasicText("Misses", out TMP_Text count);
            counter = count;
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info) { }

        public void OnNoteMiss(NoteData data)
        {
            if (data.noteType == NoteType.Bomb) return;
            notesMissed++;
            counter.text = notesMissed.ToString();
        }
    }
}
