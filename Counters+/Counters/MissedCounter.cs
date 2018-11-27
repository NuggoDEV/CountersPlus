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
        private StandardLevelSceneSetup sceneSetup;
        private MissedConfigModel settings;
        private GameObject missedObject;
        private TextMeshPro missedText;
        private int counter;

        void Awake()
        {
            settings = CountersController.settings.missedConfig;
            StartCoroutine(GetRequired());
        }

        IEnumerator GetRequired()
        {
            while (true)
            {
                scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                sceneSetup = Resources.FindObjectsOfTypeAll<StandardLevelSceneSetup>().FirstOrDefault();
                if (scoreController != null && sceneSetup != null) break;
                yield return new WaitForSeconds(0.1f);
            }

            Init();
        }

        private void Init()
        {
            missedText = this.gameObject.AddComponent<TextMeshPro>();
            missedText.text = "0";
            missedText.fontSize = 4;
            missedText.color = Color.white;
            missedText.alignment = TextAlignmentOptions.Center;
            missedText.rectTransform.position = new Vector3(0, -0.4f, 0);

            TextMeshPro label = this.gameObject.AddComponent<TextMeshPro>();
            label.text = "Misses";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;
            label.rectTransform.position = Vector3.zero;

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

        private void onNoteCut(NoteData data, NoteCutInfo info, int c)
        {
            if (data.noteType == NoteType.Bomb || !info.allIsOK) incrementCounter();
        }

        private void onNoteMiss(NoteData data, int c)
        {
            if (data.noteType != NoteType.Bomb) incrementCounter();
        }

        private void incrementCounter()
        {
            counter++;
            missedText.text = counter.ToString();
        }
    }
}
