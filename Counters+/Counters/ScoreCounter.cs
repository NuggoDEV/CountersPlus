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
        int _currentScore;

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
            if (gameObject.name == "ScorePanel")
                PreInit();
            else
                StartCoroutine(YeetToBaseCounter());
        }

        IEnumerator YeetToBaseCounter()
        {
            GameObject baseCounter;
            while (true)
            {
                baseCounter = GameObject.Find("ScorePanel");
                Console.WriteLine(baseCounter != null);
                if (baseCounter != null) break;
                yield return new WaitForSeconds(0.1f);
            }
            baseCounter.AddComponent<ScoreCounter>();
            Plugin.Log("Score Counter has been moved to the base game counter!");
            Destroy(gameObject);
        }

        private void PreInit()
        {
            if (settings.Mode == ICounterMode.BaseGame || settings.Mode == ICounterMode.BaseWithOutScore)
            {
                StartCoroutine(UpdatePosition());
            }
            else
            {
                Destroy(GetComponent<ImmediateRankUIPanel>());
                transform.Find("ScoreText").transform.position += new Vector3(0, 0f, 0);
                for (var i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child.gameObject.name != "ScoreText")
                    {
                        if (child.GetComponent<TextMeshProUGUI>() != null) Destroy(child.GetComponent<TextMeshProUGUI>());
                        Destroy(child);
                    }
                }
                //if (settings.Mode == ICounterMode.LeaveScore) transform.Find("ScoreText").SetParent(new GameObject("Counters+ | Points Container").transform, true);
                if (settings.Mode == ICounterMode.ScoreOnly) Destroy(GameObject.Find("ScoreText"));
                StartCoroutine(WaitForLoad());
            }
        }

        private void Init()
        {
            Plugin.Log("Creating Score Counter stuff");
            roundMultiple = (float)Math.Pow(10, settings.DecimalPrecision + 2);

            GameObject scoreMesh = new GameObject("Counters+ | Score Percent");
            scoreMesh.transform.parent = transform;
            _scoreMesh = scoreMesh.AddComponent<TextMeshPro>();
            _scoreMesh.text = "100.0%";
            _scoreMesh.fontSize = 3;
            _scoreMesh.color = Color.white;
            _scoreMesh.alignment = TextAlignmentOptions.Center;
            _scoreMesh.rectTransform.localPosition = new Vector3(0f, 0f, 0f);

            if (settings.DisplayRank)
            {
                _RankObject = new GameObject("Counters+ | Score Rank");
                _RankObject.transform.parent = transform;
                _RankText = _RankObject.AddComponent<TextMeshPro>();
                _RankText.text = "\nSSS";
                _RankText.fontSize = 4;
                _RankText.color = Color.white;
                _RankText.alignment = TextAlignmentOptions.Center;
            }
            if (_scoreController != null)
            {
                _scoreController.scoreDidChangeEvent += UpdateScore;
                _scoreController.noteWasMissedEvent += _OnNoteWasMissed;
            }
            StartCoroutine(UpdatePosition());
        }

        void Update()
        {
            _RankText.rectTransform.localPosition = new Vector3(0, -0.4f, 0);
            if (settings.Mode == ICounterMode.LeaveScore || settings.Mode == ICounterMode.BaseWithOutScore)
            {
                GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>().rectTransform.position = new Vector3(-3.2f, 0.9f, 7);
                _RankText.rectTransform.localPosition = new Vector3(0, 0f, 0);
                _scoreMesh.rectTransform.localPosition = new Vector3(0, 0.4f, 0);
            }
            else if (settings.Mode == ICounterMode.ScoreOnly)
            {
                _RankText.rectTransform.localPosition = new Vector3(0, 0f, 0);
                _scoreMesh.rectTransform.localPosition = new Vector3(0, 0.4f, 0);
            }
        }

        IEnumerator UpdatePosition()
        {
            while (true)
            {
                transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
                yield return new WaitForSeconds(10);
            }
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

        private void _OnNoteWasMissed(NoteData noteData, int score)
        {
            UpdateScore(_currentScore);
        }

        public void UpdateScore(int score)
        {
            _currentScore = score;
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
                    if (settings.DisplayRank) _RankText.text = "\nSSS";
                }
                else
                {
                    float ratio = score / (float)_maxPossibleScore;
                    //Force percent to round down to decimal precision
                    ratio = (float)Math.Floor(ratio * roundMultiple) / roundMultiple;
                    _scoreMesh.text = (Mathf.Clamp(ratio, 0.0f, 1.0f) * 100.0f).ToString("F" + settings.DecimalPrecision) + "%";
                    if (settings.DisplayRank) _RankText.text = "\n" + GetRank(score, ratio);
                }
            }
        }
    }
}
