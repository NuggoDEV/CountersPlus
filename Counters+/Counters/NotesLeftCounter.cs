using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using System.Linq;
using TMPro;
using Zenject;

namespace CountersPlus.Counters
{
    internal class NotesLeftCounter : Counter<NotesLeftConfigModel>, INoteEventHandler
    {
        [Inject] private IDifficultyBeatmap beatmap;
        [Inject] private GameplayCoreSceneSetupData setupData;

        private int notesLeft = 0;
        private TMP_Text counter;

        public override void CounterInit()
        {
            if (setupData.practiceSettings != null && setupData.practiceSettings.startInAdvanceAndClearNotes)
            {
                float startTime = setupData.practiceSettings.startSongTime; // in seconds, we need to convert to beats
                float beatsPerMinute = beatmap.level.beatsPerMinute;
                float startTimeInBeats = beatsPerMinute / 60 * startTime;

                // This complicated LINQ statement is to ensure compatibility with Practice Mode / Practice Plugin
                notesLeft = beatmap.beatmapData.beatmapLinesData.Sum(x => // Grab the sum of ...
                    x.beatmapObjectsData.Count(y =>                       // all beatmap objects in lines where:
                        y.beatmapObjectType == BeatmapObjectType.Note &&  // 1) It is a note
                        y.time > startTimeInBeats &&                      // 2) It has not spawned yet 
                        (y as NoteData).noteType != NoteType.Bomb));      // 3) It is not a Bomb (which is technically a Note)
            }
            else
            {
                notesLeft = beatmap.beatmapData.notesCount;
            }

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
