using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS_Utils.Gameplay;
using UnityEngine;
using TMPro;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    public class PBCounter : MonoBehaviour
    {
        ScoreController _scoreController;

        private PBConfigModel settings;
        private LevelData levelData;

        GameObject _PbTrackerObject;
        TMP_Text _PbTrackerText;
        int _maxPossibleScore = 0;
        float roundMultiple;
        float pbAtBeginningOfSong = 0;
        
        void Awake()
        {
            settings = CountersController.settings.pbConfig;
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            roundMultiple = (float)Math.Pow(100, settings.DecimalPrecision);
            levelData = BS_Utils.Plugin.LevelData;
            PlayerDataModelSO player = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().First();
            IDifficultyBeatmap beatmap = levelData.GameplayCoreSceneSetupData.difficultyBeatmap;
            PlayerLevelStatsData stats = player.currentLocalPlayer.GetPlayerLevelStatsData(
                beatmap.level.levelID, beatmap.difficulty, beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic);
            _maxPossibleScore = stats.highScore;
            pbAtBeginningOfSong = stats.highScore / ScoreController.MaxScoreForNumberOfNotes(beatmap.beatmapData.notesCount);
            SetPersonalBest(pbAtBeginningOfSong);
        }

        //Sometimes a leaderboard request will run past creation of this object.
        //In that case, we'll need to be able to change the personal best from the outside
        public void SetPersonalBest(float pb)
        {
            //Force personal best percent to round down to decimal precision
            pb = (float)Math.Floor(pb * roundMultiple) / roundMultiple;
            if (_PbTrackerText == null)
            {
                Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
                TextHelper.CreateText(out _PbTrackerText, position);
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
            transform.position = Vector3.Lerp(
                transform.position,
                CountersController.determinePosition(gameObject, settings.Position, settings.Index),
                Time.deltaTime);
        }

        public void UpdateScore(int score)
        {
            if (_PbTrackerText != null)
            {
                if (_maxPossibleScore != 0)
                {
                    float ratio = score / (float)_maxPossibleScore;
                    //Force percent to round down to decimal precision
                    ratio = (float)Math.Floor(ratio * roundMultiple) / roundMultiple;

                    if (ratio != 0 && ratio > pbAtBeginningOfSong)
                    {
                        _PbTrackerText.color = Color.red;
                        _PbTrackerText.text = "PB: " + (Mathf.Clamp(ratio, 0.0f, 1.0f) * 100.0f).ToString("F" + settings.DecimalPrecision) + "%";
                    }
                    else if (ratio != 0 && ratio < pbAtBeginningOfSong)
                        _PbTrackerText.color = Color.white;
                }
            }
        }
    }
}