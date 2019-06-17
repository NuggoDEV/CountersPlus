using System;
using System.Collections;
using System.Linq;
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
        private GameplayModifiers modifiers;
        private PlayerLevelStatsData stats;

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
            modifiers = data.PlayerData.sharedGameplayModifiers;
            IDifficultyBeatmap beatmap = data.GCSSD.difficultyBeatmap;
            stats = player.currentLocalPlayer.GetPlayerLevelStatsData(
                beatmap.level.levelID, beatmap.difficulty, beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic);
            _maxPossibleScore = ScoreController.MaxRawScoreForNumberOfNotes(beatmap.beatmapData.notesCount);
            beginningPB = (float)stats.highScore / ((float)ScoreController.MaxRawScoreForNumberOfNotes(beatmap.beatmapData.notesCount));

            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Index);
            TextHelper.CreateText(out _PbTrackerText, position);
            _PbTrackerText.fontSize = settings.TextSize;
            _PbTrackerText.color = Color.white;
            _PbTrackerText.alignment = TextAlignmentOptions.Center;
            
            _scoreController.scoreDidChangeEvent += UpdateScore;
            
            SetPersonalBest(beginningPB);

            if (settings.UnderScore) StartCoroutine(WaitForScoreCounter());
        }

        private IEnumerator WaitForScoreCounter()
        {
            ScoreCounter counter = CountersController.LoadedCounters.Where((GameObject x) => x?.GetComponent<ScoreCounter>() != null).FirstOrDefault()?.GetComponent<ScoreCounter>();
            if (counter == null) yield break;
            float offset = 0;
            yield return new WaitUntil(() => counter.PointsText != null);
            if (!(CountersController.settings.scoreConfig.Mode == ICounterMode.BaseGame || CountersController.settings.scoreConfig.Mode == ICounterMode.BaseWithOutPoints))
            {
                if (CountersController.settings.scoreConfig.DisplayRank)
                    offset = 3.35f;
                else
                    offset = 3.1f;
            }

            _PbTrackerText.rectTransform.SetParent(counter.PointsText.rectTransform);
            _PbTrackerText.rectTransform.localPosition = new Vector2(0, (TextHelper.ScaleFactor / 2) + (settings.TextSize / 10) + offset) * -1;
        }

        void OnDestroy()
        {
            _scoreController.scoreDidChangeEvent -= UpdateScore;
            CountersController.ReadyToInit -= Init;
        }
        
        public void SetPersonalBest(float pb)
        {
            //Force personal best percent to round down to decimal precision
            pb = (float)Math.Round((decimal)pb * 100, decimalPrecision);
            if (settings.HideFirstScore && stats.highScore == 0) _PbTrackerText.text = "--";
            else _PbTrackerText.text = $"PB: {pb.ToString($"F{settings.DecimalPrecision}")}%";
        }

        public void UpdateScore(int score, int modifiedScore)
        {
            if (_maxPossibleScore != 0)
            {
                float ratio = ScoreController.MaxModifiedScoreForMaxRawScore(score, modifiers, gameplayMods) / (float)_maxPossibleScore;
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