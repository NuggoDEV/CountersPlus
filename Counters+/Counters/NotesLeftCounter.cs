using TMPro;
using UnityEngine;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    class NotesLeftCounter : MonoBehaviour
    {
        private int notesLeft = 0;
        private TMP_Text counter;
        private NotesLeftConfigModel settings;
        private ScoreController SC;

        void Awake()
        {
            settings = CountersController.settings.notesLeftConfig;
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            SC = data.ScoreController;
            SC.noteWasCutEvent += OnNoteCut;
            SC.noteWasMissedEvent += OnNoteMiss;
            notesLeft = data.GCSSD.difficultyBeatmap.beatmapData.notesCount;
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Index);
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

        private void OnNoteCut(NoteData data, NoteCutInfo info, int cutScore)
        {
            if (data.noteType != NoteType.Bomb) DecrementCounter();
        }

        private void OnNoteMiss(NoteData data, int score)
        {
            if (data.noteType != NoteType.Bomb) DecrementCounter();
        }

        private void DecrementCounter()
        {
            --notesLeft;
            if (settings.LabelAboveCount) counter.text = notesLeft.ToString();
            else counter.text = $"Notes Remaining {notesLeft}";
        }

        void OnDestroy()
        {
            CountersController.ReadyToInit -= Init;
            SC.noteWasCutEvent -= OnNoteCut;
            SC.noteWasMissedEvent -= OnNoteMiss;
        }
    }
}
