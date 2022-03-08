using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Counters.NoteCountProcessors;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;
using static CountersPlus.Utils.Accessors;

namespace CountersPlus.Counters
{
    internal class PBCounter : Counter<PBConfigModel>, IScoreEventHandler
    {
        private readonly Vector3 SCORE_COUNTER_OFFSET = new Vector3(0, -1.85f, 0); 

        [Inject] private GameplayCoreSceneSetupData data;
        [Inject] private ScoreController scoreController;
        [Inject] private PlayerDataModel playerDataModel;
        [Inject] private ScoreConfigModel scoreConfig;
        [Inject] private NoteCountProcessor noteCountProcessor;
        [Inject] private RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRank;
        [Inject] private IReadonlyBeatmapData beatmapData;

        private GameplayModifiersModelSO modifiersModel;
        private TMP_Text counter;
        private PlayerLevelStatsData stats;

        private Color white = Color.white; // Caching it beforehand, since calling the constant makes a new struct

        private int maxPossibleScore = 0;
        private int highScore;
        private float pbRatio;

        public override void CounterInit()
        {
            modifiersModel = SCGameplayModsModel(ref scoreController);
            IDifficultyBeatmap beatmap = data.difficultyBeatmap;
            
            maxPossibleScore = ScoreModel.ComputeMaxMultipliedScoreForBeatmap(beatmapData);

            stats = playerDataModel.playerData.GetPlayerLevelStatsData(beatmap);
            highScore = stats.highScore;

            if (scoreConfig.Enabled && Settings.UnderScore)
            {
                HUDCanvas scoreCanvas = CanvasUtility.GetCanvasSettingsFromID(scoreConfig.CanvasID);
                counter = CanvasUtility.CreateTextFromSettings(scoreConfig, SCORE_COUNTER_OFFSET * (3f / scoreCanvas.PositionScale));
            }
            else
            {
                counter = CanvasUtility.CreateTextFromSettings(Settings);
            }
            counter.alignment = TextAlignmentOptions.Top;
            counter.fontSize = Settings.TextSize;

            pbRatio = (float)highScore / maxPossibleScore;

            SetPersonalBest((float)highScore / maxPossibleScore);
            ScoreUpdated(0);
        }

        public void MaxScoreUpdated(int maxModifiedScore) { }

        public void ScoreUpdated(int modifiedScore)
        {

            if (maxPossibleScore != 0)
            {
                if (modifiedScore > highScore)
                {
                    float ratio = modifiedScore / (float)maxPossibleScore;
                    SetPersonalBest(ratio);
                }
            }

            if (Settings.Mode == PBMode.Relative)
            {
                if (relativeScoreAndImmediateRank.relativeScore > pbRatio)
                {
                    counter.color = Settings.BetterColor;
                }
                else
                {
                    counter.color = Settings.DefaultColor;
                }
            }
            else
            {
                if (modifiedScore > highScore)
                {
                    if (!(Settings.HideFirstScore && stats.highScore == 0)) counter.color = Settings.BetterColor;
                }
                else
                {
                    counter.color = Color.Lerp(white, Settings.DefaultColor, modifiedScore / (float)highScore);
                }
            }
        }

        private void SetPersonalBest(float pb)
        {
            if (Settings.HideFirstScore && stats.highScore == 0) counter.text = "PB: --";
            else counter.text = $"PB: {(pb * 100).ToString($"F{Settings.DecimalPrecision}")}%";
        }
    }
}
