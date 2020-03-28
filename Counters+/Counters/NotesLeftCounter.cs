using TMPro;
using UnityEngine;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    class NotesLeftCounter : Counter<NotesLeftConfigModel>
    {
        private int notesLeft = 0;
        private TMP_Text counter;
        private BeatmapObjectManager beatmapObjectManager;

        internal override void Counter_Start() { }

        internal override void Init(CountersData data, Vector3 position)
        {
            beatmapObjectManager = data.BOM;
            beatmapObjectManager.noteWasCutEvent += OnNoteCut;
            beatmapObjectManager.noteWasMissedEvent += OnNoteMiss;
            notesLeft = data.GCSSD.difficultyBeatmap.beatmapData.notesCount;
            TextHelper.CreateText(out counter, position - new Vector3(0, 0.4f, 0));
            counter.text = $"Notes Remaining {notesLeft}";
            counter.fontSize = 3f;
            counter.color = Color.white;
            counter.alignment = TextAlignmentOptions.Center;

            if (settings.LabelAboveCount)
            {
                counter.fontSize = 4;
                counter.text = notesLeft.ToString();
                GameObject labelGO = new GameObject("Counters+ | Notes Left Label");
                labelGO.transform.parent = transform;
                TextHelper.CreateText(out TMP_Text label, position);
                label.text = "Notes Remaining";
                label.fontSize = 3;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;
            }
        }

        private void OnNoteCut(INoteController data, NoteCutInfo info)
        {
            if (data.noteData.noteType != NoteType.Bomb) DecrementCounter();
        }

        private void OnNoteMiss(INoteController data)
        {
            if (data.noteData.noteType != NoteType.Bomb) DecrementCounter();
        }

        private void DecrementCounter()
        {
            --notesLeft;
            if (settings.LabelAboveCount) counter.text = notesLeft.ToString();
            else counter.text = $"Notes Remaining {notesLeft}";
        }

        internal override void Counter_Destroy()
        {
            beatmapObjectManager.noteWasCutEvent -= OnNoteCut;
            beatmapObjectManager.noteWasMissedEvent -= OnNoteMiss;
        }
    }
}
