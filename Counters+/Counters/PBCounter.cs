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
        private readonly Vector3 SCORE_COUNTER_OFFSET = new Vector3(0, -0.7f, 0); 

        [Inject] private GameplayCoreSceneSetupData data;
        [Inject] private ScoreController scoreController;
        [Inject] private PlayerDataModel playerDataModel;
        [Inject] private ScoreConfigModel scoreConfig;
        [Inject] private NoteCountProcessor noteCountProcessor;

        private GameplayModifiersModelSO modifiersModel;
        private TMP_Text counter;
        private PlayerLevelStatsData stats;


        private Color orange;
        private Color white = Color.white; // Caching it beforehand, since calling the constant makes a new struct
        private Color red = Color.red;

        private int maxPossibleScore = 0;
        private int highScore;

        public override void CounterInit()
        {
            ColorUtility.TryParseHtmlString("#FFA500", out orange);

            modifiersModel = SCGameplayModsModel(ref scoreController);
            IDifficultyBeatmap beatmap = data.difficultyBeatmap;
            int maxRawScore = ScoreModel.MaxRawScoreForNumberOfNotes(noteCountProcessor.NoteCount);
            maxPossibleScore = ScoreModel.GetModifiedScoreForGameplayModifiersScoreMultiplier(maxRawScore,
                modifiersModel.GetTotalMultiplier(data.gameplayModifiers));
            stats = playerDataModel.playerData.GetPlayerLevelStatsData(beatmap);
            highScore = stats.highScore;

            if (scoreConfig.Enabled && Settings.UnderScore)
            {
                counter = CanvasUtility.CreateTextFromSettings(scoreConfig, SCORE_COUNTER_OFFSET);
            }
            else
            {
                counter = CanvasUtility.CreateTextFromSettings(Settings);
            }
            counter.alignment = TextAlignmentOptions.Top;
            counter.fontSize = Settings.TextSize;

            SetPersonalBest((float)highScore / maxPossibleScore);
        }

        public void MaxScoreUpdated(int maxModifiedScore) { }

        public void ScoreUpdated(int modifiedScore)
        {
            if (maxPossibleScore != 0)
            {
                float ratio = modifiedScore / (float)maxPossibleScore;
                if (modifiedScore > highScore)
                {
                    SetPersonalBest(ratio);
                    if (!(Settings.HideFirstScore && stats.highScore == 0)) counter.color = red;
                }
                else counter.color = Color.Lerp(white, orange, modifiedScore / (float)highScore);
            }
        }

        private void SetPersonalBest(float pb)
        {
            if (Settings.HideFirstScore && stats.highScore == 0) counter.text = "PB: --";
            else counter.text = $"PB: {(pb * 100).ToString($"F{Settings.DecimalPrecision}")}%";
        }
    }
}
