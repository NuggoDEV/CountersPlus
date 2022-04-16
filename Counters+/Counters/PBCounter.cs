using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Counters.NoteCountProcessors;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.Counters
{
    internal class PBCounter : Counter<PBConfigModel>, IScoreEventHandler
    {
        private readonly Vector3 SCORE_COUNTER_OFFSET = new Vector3(0, -1.85f, 0); 

        [Inject] private GameplayCoreSceneSetupData data;
        [Inject] private PlayerDataModel playerDataModel;
        [Inject] private ScoreConfigModel scoreConfig;
        [Inject] private RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRank;
        [Inject] private IReadonlyBeatmapData beatmapData;

        private TMP_Text counter;
        private PlayerLevelStatsData stats;

        private Color white = Color.white; // Caching it beforehand, since calling the constant makes a new struct

        private int maxPossibleScore = 0;
        private int highScore;
        private float pbRatio;

        public override void CounterInit()
        {
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

            SetPersonalBest(pbRatio);
            ScoreUpdated(0);
        }

        public void MaxScoreUpdated(int maxModifiedScore) { }

        public void ScoreUpdated(int modifiedScore)
        {
            if (maxPossibleScore != 0 && modifiedScore > highScore)
            {
                SetPersonalBest(modifiedScore / (float)maxPossibleScore);
            }

            counter.color = Settings.Mode switch
            {
                // Show default color when setting the first PB and setting is enabled
                _ when Settings.HideFirstScore && stats.highScore == 0 => Settings.DefaultColor,

                // Relative % is above PB %
                PBMode.Relative when relativeScoreAndImmediateRank.relativeScore >= pbRatio
                    => Settings.BetterColor,

                // Relative % is approaching PB %
                PBMode.Relative when relativeScoreAndImmediateRank.relativeScore < pbRatio
                    => Color.Lerp(white, Settings.DefaultColor, relativeScoreAndImmediateRank.relativeScore / pbRatio),

                // New high score, show PB color
                PBMode.Absolute when modifiedScore >= highScore
                    => Settings.BetterColor,

                // Current score is approaching high school
                PBMode.Absolute when modifiedScore < highScore
                    => Color.Lerp(white, Settings.DefaultColor, modifiedScore / (float)highScore),

                // The "C# stop yelling at me" case
                _ => Settings.DefaultColor,
            };
        }

        private void SetPersonalBest(float pb)
        {
            if (Settings.HideFirstScore && stats.highScore == 0) counter.text = "PB: --";
            else counter.text = $"PB: {(pb * 100).ToString($"F{Settings.DecimalPrecision}")}%";
        }
    }
}
