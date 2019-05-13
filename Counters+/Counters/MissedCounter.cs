using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using CountersPlus.Config;

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
            Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
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

            if (settings.CustomMissTextIntegration)
            {
                #pragma warning disable CS0618 //Fuck off DaNike
                if (IPA.Loader.PluginManager.Plugins.Where((IPA.Old.IPlugin x) => x.Name == "CustomMissText").Any())
                    label.text = String.Join(" ", CustomMissText.Plugin.allEntries[UnityEngine.Random.Range(0, CustomMissText.Plugin.allEntries.Count)]);
            }

            if (scoreController != null)
            {
                scoreController.noteWasCutEvent += onNoteCut;
                scoreController.noteWasMissedEvent += onNoteMiss;
            }
        }

        void OnDestroy()
        {
            scoreController.noteWasCutEvent -= onNoteCut;
            scoreController.noteWasMissedEvent -= onNoteMiss;
            CountersController.ReadyToInit -= Init;
        }

        private void onNoteCut(NoteData data, NoteCutInfo info, int c)
        {
            if (data.noteType == NoteType.Bomb || !info.allIsOK) incrementCounter();
        }

        private void onNoteMiss(NoteData data, int c)
        {
            if (data.noteType != NoteType.Bomb)
            {
                incrementCounter();
                if (settings.CustomMissTextIntegration)
                {
                    #pragma warning disable CS0618 //Fuck off DaNike
                    if (IPA.Loader.PluginManager.Plugins.Where((IPA.Old.IPlugin x) => x.Name == "CustomMissText").Any())
                        label.text = String.Join(" ", CustomMissText.Plugin.allEntries[UnityEngine.Random.Range(0, CustomMissText.Plugin.allEntries.Count)]);
                }
            }
        }

        private void incrementCounter()
        {
            counter++;
            missedText.text = counter.ToString();
        }
    }
}
