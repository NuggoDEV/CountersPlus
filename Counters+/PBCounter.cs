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
    public class PBCounter : MonoBehaviour
    {
        ScoreController _scoreController;
        StandardLevelSceneSetupDataSO _objectRatingRecorder;

        private PBConfigModel settings;

        GameObject _PbTrackerObject;
        TextMeshPro _PbTrackerText;
        int _maxPossibleScore = 0;
        float roundMultiple;

        float pbPercent = CountersController.Instance.pbPercent;

        IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                _objectRatingRecorder = FindObjectOfType<StandardLevelSceneSetupDataSO>();

                if (_scoreController == null || _objectRatingRecorder == null)
                    yield return new WaitForSeconds(0.1f);
                else
                    loaded = true;
            }

            Init();
        }

        void Awake()
        {
            settings = CountersController.settings.pBConfig;
            StartCoroutine(WaitForLoad());
        }

        private void Init()
        {
            roundMultiple = (float)Math.Pow(100, settings.DecimalPrecision);
            SetPersonalBest(pbPercent);
            Plugin.Log(pbPercent.ToString());
        }

        //Sometimes a leaderboard request will run past creation of this object.
        //In that case, we'll need to be able to change the personal best from the outside
        public void SetPersonalBest(float pb)
        {
            //Force personal best percent to round down to decimal precision
            pb = (float)Math.Floor(pb * roundMultiple) / roundMultiple;
            if (_PbTrackerText == null)
            {
                _PbTrackerText = gameObject.AddComponent<TextMeshPro>();
                _PbTrackerText.fontSize = 2;
                _PbTrackerText.color = Color.white;
                _PbTrackerText.alignment = TextAlignmentOptions.Center;
            }
            if (_scoreController != null)
                _scoreController.scoreDidChangeEvent += UpdateScore;
            if (pb == 0) _PbTrackerText.text = "--";
            else _PbTrackerText.text = "PB: " + (Mathf.Clamp(pb, 0.0f, 1.0f) * 100.0f).ToString("F" + settings.DecimalPrecision) + "%";
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

            if (_PbTrackerText != null)
            {
                if (_maxPossibleScore != 0)
                {
                    float ratio = score / (float)_maxPossibleScore;
                    //Force percent to round down to decimal precision
                    ratio = (float)Math.Floor(ratio * roundMultiple) / roundMultiple;
                    if (pbPercent != 0 && pbPercent > ratio)
                        _PbTrackerText.color = Color.red;
                    else if (pbPercent != 0 && pbPercent < ratio)
                        _PbTrackerText.color = Color.white;
                }
            }
        }
    }
}