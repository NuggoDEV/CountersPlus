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
        private GameplayModifiersModelSO gameplayMods;
        private GameplayCoreSceneSetupData gcssd;

        private Color orange;
        private TMP_Text _PbTrackerText;
        private int _maxPossibleScore = 0;
        private int decimalPrecision = 2;
        private float beginningPB = 0;
        
        void Awake()
        {
            settings = CountersController.settings.pbConfig;
            decimalPrecision = settings.DecimalPrecision;
            CountersController.ReadyToInit += Init;
            ColorUtility.TryParseHtmlString("#FFA500", out orange);
        }

        private void Init(CountersData data)
        {
            _scoreController = data.ScoreController;
            gcssd = data.GCSSD;
            PlayerDataModelSO player = data.PlayerData;
            gameplayMods = data.ModifiersData;
            IDifficultyBeatmap beatmap = data.GCSSD.difficultyBeatmap;
            PlayerLevelStatsData stats = player.currentLocalPlayer.GetPlayerLevelStatsData(
                beatmap.level.levelID, beatmap.difficulty, beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic);
            _maxPossibleScore = ScoreController.MaxScoreForNumberOfNotes(beatmap.beatmapData.notesCount);
            beginningPB = (float)stats.highScore / ((float)ScoreController.MaxScoreForNumberOfNotes(beatmap.beatmapData.notesCount));

            Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            TextHelper.CreateText(out _PbTrackerText, position);
            _PbTrackerText.fontSize = 2;
            _PbTrackerText.color = Color.white;
            _PbTrackerText.alignment = TextAlignmentOptions.Center;
            
            _scoreController.scoreDidChangeEvent += UpdateScore;

            SetPersonalBest(beginningPB);
        }
        
        public void SetPersonalBest(float pb)
        {
            //Force personal best percent to round down to decimal precision
            pb = (float)Math.Round((decimal)pb, decimalPrecision + 2);
            if (pb == 0) _PbTrackerText.text = "--";
            else _PbTrackerText.text = "PB: " + (pb * 100.0f).ToString("F" + settings.DecimalPrecision) + "%";
        }

        public void UpdateScore(int score)
        {
            if (_maxPossibleScore != 0)
            {
                float ratio = ScoreController.GetScoreForGameplayModifiersScoreMultiplier(score, gameplayMods.GetTotalMultiplier(gcssd.gameplayModifiers)) / (float)_maxPossibleScore;
                if (ratio > beginningPB)
                {
                    SetPersonalBest(ratio);
                    _PbTrackerText.color = Color.red;
                }
                else _PbTrackerText.color = Color.Lerp(Color.white, orange, ratio / beginningPB);
            }
        }
    }
}