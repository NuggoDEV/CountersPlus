using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using TMPro;
using Zenject;

namespace CountersPlus.Counters
{
    public class NotesLeftCounter : Counter<NotesLeftConfigModel>, INoteEventHandler
    {
        [Inject] private IDifficultyBeatmap beatmap;

        private int notesLeft = 0;
        private TMP_Text counter;

        public override void CounterInit()
        {
            notesLeft = beatmap.beatmapData.notesCount;
            if (Settings.LabelAboveCount)
            {
                GenerateBasicText("Notes Remaining", out counter);
                counter.text = notesLeft.ToString();
            }
            else
            {
                counter = CanvasUtility.CreateTextFromSettings(Settings);
                counter.text = $"Notes Remaining: {notesLeft}";
                counter.fontSize = 2;
            }
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (data.noteType != NoteType.Bomb) DecrementCounter();
        }

        public void OnNoteMiss(NoteData data)
        {
            if (data.noteType != NoteType.Bomb) DecrementCounter();
        }

        private void DecrementCounter()
        {
            --notesLeft;
            if (Settings.LabelAboveCount) counter.text = notesLeft.ToString();
            else counter.text = $"Notes Remaining: {notesLeft}";
        }
    }
}
