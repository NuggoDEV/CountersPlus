using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using System.Linq.Expressions;

namespace CountersPlus.Counters
{
    public class PBCounter : Counter<PBConfigModel>
    {
        private ScoreController _scoreController;

        private GameplayModifiersModelSO gameplayModsModel;
        private GameplayModifiers gameplayMods;
        private PlayerLevelStatsData stats;

        private Color orange;
        private TMP_Text _PbTrackerText;
        private float beginningPB = 0;

        private int _maxPossibleScore = 0;
        private int highScore;

        internal override void Counter_Start()
        {
            ColorUtility.TryParseHtmlString("#FFA500", out orange);
        }

        internal override void Init(CountersData data, Vector3 position)
        {
            _scoreController = data.ScoreController;
            PlayerDataModel player = data.PlayerData;
            gameplayModsModel = data.ModifiersData;
            gameplayMods = data.PlayerData.playerData.gameplayModifiers;
            IDifficultyBeatmap beatmap = data.GCSSD.difficultyBeatmap;
            stats = player.playerData.GetPlayerLevelStatsData(
                beatmap.level.levelID, beatmap.difficulty, beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic);
            int maxRawScore = ScoreModel.MaxRawScoreForNumberOfNotes(beatmap.beatmapData.notesCount);
            _maxPossibleScore = Mathf.RoundToInt(maxRawScore * gameplayModsModel.GetTotalMultiplier(gameplayMods));
            beginningPB = stats.highScore / (float)_maxPossibleScore;
            highScore = stats.highScore;

            TextHelper.CreateText(out _PbTrackerText, position);
            _PbTrackerText.fontSize = settings.TextSize;
            _PbTrackerText.color = Color.white;
            _PbTrackerText.alignment = TextAlignmentOptions.Center;
            
            _scoreController.scoreDidChangeEvent += UpdateScore;
            
            SetPersonalBest(beginningPB);
        }

        public void UpdatePBCounterToScorePosition(TextMeshProUGUI score)
        {
            if (!settings.UnderScore) return;
            Plugin.Log("Fuck me", LogInfo.Error);
            float size = score.fontSize * score.fontScale;
            Vector3 position = score.rectTransform.position - (Vector3.up * size);
            _PbTrackerText.rectTransform.localPosition = TextHelper.CounterCanvas.transform.InverseTransformPoint(position);
        }

        internal override void Counter_Destroy()
        {
            _scoreController.scoreDidChangeEvent -= UpdateScore;
        }
        
        public void SetPersonalBest(float pb)
        {
            //Force personal best percent to round down to decimal precision'
            try
            {
                pb = (float)Math.Round((decimal)pb * 100, settings.DecimalPrecision);
            }
            catch { pb = 0; } //yea something can go wrong here, like if you have all of the negative modifiers
            if (settings.HideFirstScore && stats.highScore == 0) _PbTrackerText.text = "PB: --";
            else _PbTrackerText.text = $"PB: {pb.ToString($"F{settings.DecimalPrecision}")}%";
        }

        public void UpdateScore(int score, int modifiedScore)
        {
            if (_maxPossibleScore != 0)
            {
                float ratio = modifiedScore / (float)_maxPossibleScore;
                if (modifiedScore > highScore)
                {
                    SetPersonalBest(ratio);
                    if (!(settings.HideFirstScore && stats.highScore == 0)) _PbTrackerText.color = Color.red;
                }
                else _PbTrackerText.color = Color.Lerp(Color.white, orange, modifiedScore / (float)highScore);
            }
        }
    }
}