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
            notesLeft = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.beatmapData.notesCount;
            Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            TextHelper.CreateText(out counter, position);
            counter.text = $"Notes Remaining {notesLeft}";
            counter.fontSize = 2.5f;
            counter.color = Color.white;
            counter.alignment = TextAlignmentOptions.Center;
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
            counter.text = $"Notes Remaining {notesLeft}";
        }

        void OnDestroy()
        {
            CountersController.ReadyToInit -= Init;
            SC.noteWasCutEvent -= OnNoteCut;
            SC.noteWasMissedEvent -= OnNoteMiss;
        }
    }
}
