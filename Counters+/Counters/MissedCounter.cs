using System;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using CountersPlus.Utils;

namespace CountersPlus.Counters
{
    class MissedCounter : MonoBehaviour
    {
        private ScoreController scoreController;
        private MissedConfigModel settings;
        private TMP_Text missedText;
        private TMP_Text label;
        private int counter;

        void Awake()
        {
            settings = CountersController.settings.missedConfig;
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            scoreController = data.ScoreController;
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
            TextHelper.CreateText(out missedText, position - new Vector3(0, 0.4f, 0));
            missedText.text = "0";
            missedText.fontSize = 4;
            missedText.color = Color.white;
            missedText.alignment = TextAlignmentOptions.Center;

            GameObject labelGO = new GameObject("Counters+ | Missed Label");
            labelGO.transform.parent = transform;
            TextHelper.CreateText(out label, position);
            label.text = "Misses";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

            if (settings.CustomMissTextIntegration) UpdateCustomMissText();

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
            if (data.noteType == NoteType.Bomb || !info.allIsOK) IncrementCounter();
        }

        private void OnNoteMiss(NoteData data, int c)
        {
            if (data.noteType != NoteType.Bomb)
            {
                IncrementCounter();
                if (settings.CustomMissTextIntegration) UpdateCustomMissText();
            }
        }

        private void UpdateCustomMissText()
        {
            try
            {
                if (PluginUtility.IsPluginPresent("CustomMissText"))
                    label.text = String.Join(" ", CustomMissText.Plugin.allEntries[UnityEngine.Random.Range(0, CustomMissText.Plugin.allEntries.Count)]);
            }
            catch { }
        }

        private void IncrementCounter()
        {
            counter++;
            missedText.text = counter.ToString();
        }
    }
}
