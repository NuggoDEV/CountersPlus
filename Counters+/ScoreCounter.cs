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
    public class ScoreCounter : MonoBehaviour
    {
        TextMeshPro _scoreMesh;
        ScoreController _scoreController;
        BeatmapObjectExecutionRatingsRecorder _objectRatingRecorder;

        private ScoreConfigModel settings;

        GameObject _RankObject;
        TextMeshPro _RankText;
        int _maxPossibleScore = 0;
        float roundMultiple;

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
            settings = CountersController.settings.scoreConfig;
            if (settings.UseOld && gameObject.name != "ScorePanel")
                StartCoroutine(YeetToBaseCounter());
            else if (!settings.UseOld)
                StartCoroutine(WaitForLoad());
        }

        IEnumerator YeetToBaseCounter()
        {
            GameObject baseCounter;
            while (true)
            {
                baseCounter = GameObject.Find("ScorePanel");
                if (baseCounter != null) break;
                yield return new WaitForSeconds(0.1f);
            }
            baseCounter.AddComponent<ScoreCounter>();
            Plugin.Log("Score Counter has been moved to the base game counter!");
            Destroy(gameObject);
        }

        void Update()
        {
            if (CountersController.rng)
            {
                settings.Index = UnityEngine.Random.Range(0, 5);
                settings.Position = (CounterPositions)UnityEngine.Random.Range(0, 4);
                settings.DecimalPrecision = UnityEngine.Random.Range(0, 5);
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
            if (GameObject.Find("ScorePanel") != null)
            {
                for(int i = 0; i < GameObject.Find("ScorePanel").transform.childCount; i++)
                {
                    Transform child = GameObject.Find("ScorePanel").transform.GetChild(i);
                    if (child.name != "RelativeScoreText") Destroy(child);
                }
            }
            roundMultiple = (float)Math.Pow(100, settings.DecimalPrecision);

            _scoreMesh = this.gameObject.AddComponent<TextMeshPro>();
            _scoreMesh.text = "100.0%";
            _scoreMesh.fontSize = 3;
            _scoreMesh.color = Color.white;
            _scoreMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _scoreMesh.alignment = TextAlignmentOptions.Center;

            if (settings.DisplayRank)
            {
                _RankObject = new GameObject("Counters+ | Score Label");
                _RankObject.transform.parent = transform;
                _RankText = _RankObject.AddComponent<TextMeshPro>();
                _RankText.text = "SSS";
                _RankText.fontSize = 4;
                _RankText.color = Color.white;
                _RankText.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
                _RankText.alignment = TextAlignmentOptions.Center;
                _RankText.rectTransform.localPosition = new Vector3(0f, -0.4f, 0f);
            }
            if (_scoreController != null)
                _scoreController.scoreDidChangeEvent += UpdateScore;
        }

        public string GetRank(int score, float prec)
        {
            if (score >= _maxPossibleScore)
            {
                return "SSS";
            }
            if (prec > 0.9f)
            {
                return "SS";
            }
            if (prec > 0.8f)
            {
                return "S";
            }
            if (prec > 0.65f)
            {
                return "A";
            }
            if (prec > 0.5f)
            {
                return "B";
            }
            if (prec > 0.35f)
            {
                return "C";
            }
            if (prec > 0.2f)
            {
                return "D";
            }
            return "E";
        }

        public void UpdateScore(int score)
        {

            if (_objectRatingRecorder != null)
            {
                List<BeatmapObjectExecutionRating> _ratings = ReflectionUtil.GetPrivateField<List<BeatmapObjectExecutionRating>>(_objectRatingRecorder, "_beatmapObjectExecutionRatings");
                if (_ratings != null)
                {
                    int notes = 0;
                    foreach (BeatmapObjectExecutionRating rating in _ratings)
                    {
                        if (rating.beatmapObjectRatingType == BeatmapObjectExecutionRating.BeatmapObjectExecutionRatingType.Note)
                            notes++;
                    }
                    _maxPossibleScore = ScoreController.MaxScoreForNumberOfNotes(notes);
                }
            }

            if (_scoreMesh != null)
            {
                if (_maxPossibleScore == 0)
                {
                    _scoreMesh.text = "100.0%";
                    if (settings.DisplayRank) _RankText.text = "SSS";
                }
                else
                {
                    float ratio = score / (float)_maxPossibleScore;
                    //Force percent to round down to decimal precision
                    ratio = (float)Math.Floor(ratio * roundMultiple) / roundMultiple;
                    _scoreMesh.text = (Mathf.Clamp(ratio, 0.0f, 1.0f) * 100.0f).ToString("F" + settings.DecimalPrecision) + "%";
                    if (settings.DisplayRank) _RankText.text = GetRank(score, ratio);
                }
            }
        }
    }
}