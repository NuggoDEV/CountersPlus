using System;
using UnityEngine;
using TMPro;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    class AccuracyCounter : Counter<NoteConfigModel>
    {

        private BeatmapObjectManager beatmapObjectManager;
        private TMP_Text counterText;
        private int counter;
        private int total;

        internal override void Counter_Start() { }

        internal override void Init(CountersData data)
        {
            beatmapObjectManager = data.BOM;
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
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

            if (beatmapObjectManager != null)
            {
                beatmapObjectManager.noteWasCutEvent += OnNoteCut;
                beatmapObjectManager.noteWasMissedEvent += OnNoteMiss;
            }
        }

        internal override void Counter_Destroy()
        {
            beatmapObjectManager.noteWasCutEvent -= OnNoteCut;
            beatmapObjectManager.noteWasMissedEvent -= OnNoteMiss;
        }

        private void OnNoteCut(INoteController data, NoteCutInfo info)
        {
            if (data.noteData.noteType != NoteType.Bomb && info.allIsOK)
                Increment(true);
            else
                Increment(false);
        }

        private void OnNoteMiss(INoteController data)
        {
            if (data.noteData.noteType != NoteType.Bomb) Increment(false);
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
