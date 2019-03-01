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
    public class CutCounter : MonoBehaviour
    {
        TextMeshPro _scoreMesh;
        ScoreController _scoreController;

        private CutConfigModel settings;

        private List<int> cuts = new List<int>();

        GameObject _RankObject;
        TextMeshPro _RankText;

        IEnumerator GetRequired()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<ScoreController>().Any());
            _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
            Init();
        }

        void Awake()
        {
            settings = CountersController.settings.cutConfig;
            StartCoroutine(GetRequired());
        }

        private void Init()
        {

            _scoreMesh = this.gameObject.AddComponent<TextMeshPro>();
            _scoreMesh.text = "Average Cut";
            _scoreMesh.fontSize = 3;
            _scoreMesh.color = Color.white;
            _scoreMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _scoreMesh.alignment = TextAlignmentOptions.Center;

            _RankObject = new GameObject("Counters+ | Cut Label");
            _RankObject.transform.parent = transform;
            _RankText = _RankObject.AddComponent<TextMeshPro>();
            _RankText.text = "0";
            _RankText.fontSize = 4;
            _RankText.color = Color.white;
            _RankText.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _RankText.alignment = TextAlignmentOptions.Center;
            _RankText.rectTransform.localPosition = new Vector3(0f, -0.4f, 0f);

            if (_scoreController != null)
                _scoreController.noteWasCutEvent += UpdateScore;
            StartCoroutine(UpdatePosition());
        }

        IEnumerator UpdatePosition()
        {
            while (true)
            {
                transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
                yield return new WaitForSeconds(10);
            }
        }

        public void UpdateScore(NoteData data, NoteCutInfo info, int score)
        {
            int a, b, c;
            if (data.noteType == NoteType.Bomb || !info.allIsOK) return;
            info.afterCutSwingRatingCounter.didFinishEvent += (v) =>
            {
                ScoreController.ScoreWithoutMultiplier(info, info.afterCutSwingRatingCounter, out a, out b, out c);
                cuts.Add(a+b);
                _RankText.text = $"{Math.Round(cuts.Average())}";
            };
        }
    }
}