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
        //private StandardLevelSceneSetup sceneSetup;
        private MissedConfigModel settings;
        private TextMeshPro missedText;
        private int counter;

        void Awake()
        {
            settings = CountersController.settings.missedConfig;
            transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            if (transform.parent == null)
                StartCoroutine(GetRequired());
            else
                Init();
        }

        IEnumerator GetRequired()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<ScoreController>().Any());
            scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
            Init();
        }

        private void Init()
        {
            missedText = gameObject.AddComponent<TextMeshPro>();
            missedText.text = "0";
            missedText.fontSize = 4;
            missedText.color = Color.white;
            missedText.alignment = TextAlignmentOptions.Center;
            missedText.rectTransform.localPosition = new Vector3(0, -0.4f, 0);

            GameObject labelGO = new GameObject("Counters+ | Missed Label");
            labelGO.transform.parent = transform;
            TextMeshPro label = labelGO.AddComponent<TextMeshPro>();
            label.text = "Misses";
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
            if (CountersController.rng)
            {
                settings.Index = UnityEngine.Random.Range(0, 5);
                settings.Position = (ICounterPositions)UnityEngine.Random.Range(0, 4);
            }
            else
            {
                if (CountersController.settings.RNG)
                {
                    transform.position = Vector3.Lerp(
                    transform.position,
                    CountersController.determinePosition(gameObject, settings.Position, settings.Index),
                    Time.deltaTime);
                }
                else
                    transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            }

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
