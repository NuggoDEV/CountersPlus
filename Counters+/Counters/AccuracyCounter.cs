using System;
using UnityEngine;
using TMPro;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    class AccuracyCounter : MonoBehaviour
    {

        private ScoreController scoreController;
        private NoteConfigModel settings;
        private TMP_Text counterText;
        private int counter;
        private int total;

        void Awake()
        {
            settings = CountersController.settings.noteConfig;
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            scoreController = data.ScoreController;
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Index);
            TextHelper.CreateText(out counterText, position - new Vector3(0, 0.4f, 0));
            counterText.text = settings.ShowPercentage ? "0 / 0 - (100%)" : "0 / 0";
            counterText.fontSize = 4;
            counterText.color = Color.white;
            counterText.alignment = TextAlignmentOptions.Center;

            GameObject labelGO = new GameObject("Counters+ | Notes Label");
            labelGO.transform.parent = transform;
            TextHelper.CreateText(out TMP_Text label, position);
            label.text = "Notes";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

            if (scoreController != null)
            {
                scoreController.noteWasCutEvent += OnNoteCut;
                scoreController.noteWasMissedEvent += OnNoteMiss;
            }
        }

        void OnDestroy()
        {
            scoreController.noteWasCutEvent -= OnNoteCut;
            scoreController.noteWasMissedEvent -= OnNoteMiss;
            CountersController.ReadyToInit -= Init;
        }

        private void OnNoteCut(NoteData data, NoteCutInfo info, int c)
        {
            if (data.noteType != NoteType.Bomb && info.allIsOK)
                Increment(true);
            else
                Increment(false);
        }

        private void OnNoteMiss(NoteData data, int what)
        {
            if (data.noteType != NoteType.Bomb) Increment(false);
        }

        private void Increment(bool incCounter)
        {
            total++;
            if (incCounter) counter++;
            counterText.text = counter.ToString() + " / " + total.ToString();
            if (settings.ShowPercentage) counterText.text += string.Format(" - ({0}%)", Math.Round(((float)counter / (float)total) * 100, settings.DecimalPrecision));
        }
    }
}
