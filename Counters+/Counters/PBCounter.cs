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
        private ScoreController _scoreController;

        private PBConfigModel settings;
        private LevelData levelData;

        private TMP_Text _PbTrackerText;
        private int _maxPossibleScore = 0;
        private int decimalPrecision = 2;
        private float roundMultiple;
        private float pbAtBeginningOfSong = 0;
        
        void Awake()
        {
            settings = CountersController.settings.pbConfig;
            decimalPrecision = settings.DecimalPrecision;
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            _scoreController = data.ScoreController;
            roundMultiple = (float)Math.Pow(100, settings.DecimalPrecision);
            levelData = BS_Utils.Plugin.LevelData;
            PlayerDataModelSO player = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().First();
            IDifficultyBeatmap beatmap = levelData.GameplayCoreSceneSetupData.difficultyBeatmap;
            PlayerLevelStatsData stats = player.currentLocalPlayer.GetPlayerLevelStatsData(
                beatmap.level.levelID, beatmap.difficulty, beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic);
            _maxPossibleScore = stats.highScore;
            pbAtBeginningOfSong = (float)stats.highScore / (float)ScoreController.MaxScoreForNumberOfNotes(beatmap.beatmapData.notesCount);

            Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            TextHelper.CreateText(out _PbTrackerText, position);
            _PbTrackerText.fontSize = 2;
            _PbTrackerText.color = Color.white;
            _PbTrackerText.alignment = TextAlignmentOptions.Center;
            
            _scoreController.scoreDidChangeEvent += UpdateScore;

            SetPersonalBest(pbAtBeginningOfSong);
        }
        
        public void SetPersonalBest(float pb)
        {
            //Force personal best percent to round down to decimal precision
            pb = (float)Math.Round((decimal)pb, decimalPrecision);
            if (pb == 0) _PbTrackerText.text = "--";
            else _PbTrackerText.text = "PB: " + (Mathf.Clamp(pb, 0.0f, 1.0f) * 100.0f).ToString("F" + settings.DecimalPrecision) + "%";
        }

        public void UpdateScore(int score)
        {
            if (_maxPossibleScore != 0)
            {
                float ratio = score / (float)_maxPossibleScore;
                if (ratio > pbAtBeginningOfSong)
                {
                    _PbTrackerText.color = Color.red;
                    SetPersonalBest(ratio);
                }
            }
        }
    }
}