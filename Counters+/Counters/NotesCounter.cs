using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using TMPro;

namespace CountersPlus.Counters
{
    public class NotesCounter : Counter<NoteConfigModel>, INoteEventHandler
    {
        private int goodCuts = 0;
        private int allCuts = 0;
        private TMP_Text counter;

        public override void CounterInit()
        {
            GenerateBasicText("Notes", out counter);
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            allCuts++;
            if (data.noteType != NoteType.Bomb && info.allIsOK) goodCuts++;
            RefreshText();
        }

        public void OnNoteMiss(NoteData data)
        {
            if (data.noteType == NoteType.Bomb) return;
            allCuts++;
            RefreshText();
        }

        private void RefreshText()
        {
            counter.text = $"{goodCuts} / {allCuts}";
            if (Settings.ShowPercentage)
            {
                float percentage = (float)goodCuts / allCuts;
                counter.text += $" - {percentage.ToString($"F{Settings.DecimalPrecision}")}%";
            }
        }
    }
}
