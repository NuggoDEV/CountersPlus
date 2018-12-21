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
        BeatmapObjectExecutionRatingsRecorder _objectRatingRecorder;

        private CutConfigModel settings;

        private List<float> cuts = new List<float>();

        GameObject _RankObject;
        TextMeshPro _RankText;

        IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                _objectRatingRecorder = FindObjectOfType<BeatmapObjectExecutionRatingsRecorder>();

                if (_scoreController == null || _objectRatingRecorder == null)
                    yield return new WaitForSeconds(0.1f);
                else
                    loaded = true;
            }

            Init();
        }

        void Awake()
        {
            settings = CountersController.settings.cutConfig;
            StartCoroutine(WaitForLoad());
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
        }

        public void UpdateScore(NoteData data, NoteCutInfo info, int score)
        {

            if (_objectRatingRecorder != null)
            {
                List<BeatmapObjectExecutionRating> _ratings = ReflectionUtil.GetPrivateField<List<BeatmapObjectExecutionRating>>(_objectRatingRecorder, "_beatmapObjectExecutionRatings");
                if (_ratings != null)
                {
                    float averageCut = 0;
                    foreach (BeatmapObjectExecutionRating rating in _ratings)
                    {
                        if (rating.beatmapObjectRatingType == BeatmapObjectExecutionRating.BeatmapObjectExecutionRatingType.Note)
                            averageCut += (rating as NoteExecutionRating).cutScore;
                    }
                    _RankText.text = Mathf.Round(averageCut / _ratings.Count).ToString();
                }
            }
        }
    }
}