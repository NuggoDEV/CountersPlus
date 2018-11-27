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
    class AccuracyCounter : MonoBehaviour
    {

        private ScoreController scoreController;
        private AccuracyConfigModel settings;
        private TextMeshPro counterText;
        private int counter;
        private int total;

        void Awake()
        {
            settings = CountersController.settings.accuracyConfig;
            if (transform.parent == null)
                StartCoroutine(GetRequired());
            else
                Init();
        }

        IEnumerator GetRequired()
        {
            while (true)
            {
                scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                if (scoreController != null) break;
                yield return new WaitForSeconds(0.1f);
            }

            Init();
        }

        private void Init()
        {
            counterText = gameObject.AddComponent<TextMeshPro>();
            counterText.text = settings.ShowPercentage ? "0 / 0 - (100%)" : "0 / 0";
            counterText.fontSize = 4;
            counterText.color = Color.white;
            counterText.alignment = TextAlignmentOptions.Center;
            counterText.rectTransform.position = new Vector3(0, -0.4f, 0);

            GameObject labelGO = new GameObject("Counters+ | Accuracy Label");
            labelGO.transform.parent = transform;
            TextMeshPro label = labelGO.AddComponent<TextMeshPro>();
            label.text = "Accuracy";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

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
        }

        void Update()
        {
            if (transform.parent == null)
            {
                transform.position = CountersController.determinePosition(settings.Position, settings.Index);
            }
            else
            {
                transform.localPosition = CountersController.determinePosition(settings.Position, settings.Index);
            }
        }

        private void onNoteCut(NoteData data, NoteCutInfo info, int c)
        {
            if (data.noteType != NoteType.Bomb && info.allIsOK)
                increment(true);
            else
                increment(false);
        }

        private void onNoteMiss(NoteData data, int what)
        {
            if (data.noteType != NoteType.Bomb) increment(false);
        }

        private void increment(bool incCounter)
        {
            total++;
            if (incCounter) counter++;
            counterText.text = counter.ToString() + " / " + total.ToString();
            if (settings.ShowPercentage) counterText.text += string.Format(" - ({0}%)", Math.Round(((float)counter / (float)total) * 100, settings.DecimalPrecision));
        }
    }
}
