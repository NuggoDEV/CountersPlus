using System;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using System.Collections.Generic;

namespace CountersPlus.Counters
{
    public class CutCounter : MonoBehaviour
    {
        private TMP_Text cutLabel;
        private ScoreController _scoreController;
        private GameObject _RankObject;
        private TMP_Text cutCounter;
        private CutConfigModel settings;
        private int totalCutCountLeft = 0;
        private int totalCutCountRight = 0;
        private int totalScoreLeft = 0; // MaxScoreForNumberOfNotes() is an int. Don't expect scores over 2 billion.
        private int totalScoreRight = 0;

        private Dictionary<SaberSwingRatingCounter, NoteCutInfo> noteCutInfos = new Dictionary<SaberSwingRatingCounter, NoteCutInfo>();

        void Awake()
        {
            settings = CountersController.settings.cutConfig;
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            _scoreController = data.ScoreController;
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
            TextHelper.CreateText(out cutLabel, position);
            cutLabel.text = "Average Cut";
            cutLabel.fontSize = 3;
            cutLabel.color = Color.white;
            cutLabel.alignment = TextAlignmentOptions.Center;

            _RankObject = new GameObject("Counters+ | Cut Label");
            _RankObject.transform.parent = transform;
            TextHelper.CreateText(out cutCounter, position - new Vector3(0, 0.4f, 0));
            cutCounter.text = settings.SeparateSaberCounts ? "0  0" : "0";
            cutCounter.fontSize = 4;
            cutCounter.color = Color.white;
            cutCounter.alignment = TextAlignmentOptions.Center;
            
            if (_scoreController != null)
                _scoreController.noteWasCutEvent += UpdateScore;
        }

        void OnDestroy()
        {
            _scoreController.noteWasCutEvent -= UpdateScore;
            CountersController.ReadyToInit -= Init;
        }

        private void UpdateScore(NoteData data, NoteCutInfo info, int score)
        {
            if (data.noteType == NoteType.Bomb || !info.allIsOK) return;
            noteCutInfos.Add(info.swingRatingCounter, info);
            info.swingRatingCounter.didFinishEvent -= SaberSwingRatingCounter_didFinishEvent;
            info.swingRatingCounter.didFinishEvent += SaberSwingRatingCounter_didFinishEvent;
        }

        private void SaberSwingRatingCounter_didFinishEvent(SaberSwingRatingCounter v)
        {
            ScoreController.RawScoreWithoutMultiplier(noteCutInfos[v], out int beforeCut, out int afterCut, out _);
            //"cutDistanceRawScore" is already calculated into "beforeCutRawScore"
            if (noteCutInfos[v].saberType == Saber.SaberType.SaberA)
            {
                totalScoreLeft += beforeCut + afterCut;
                ++totalCutCountLeft;
            }
            else
            {
                totalScoreRight += beforeCut + afterCut;
                ++totalCutCountRight;
            }

            if (settings.SeparateSaberCounts)
            {
                double leftAverage = Math.Round(((double)(totalScoreLeft)) / (totalCutCountLeft));
                double rightAverage = Math.Round(((double)(totalScoreRight)) / (totalCutCountRight));
                leftAverage = Double.IsNaN(leftAverage) ? 0 : leftAverage;
                rightAverage = Double.IsNaN(rightAverage) ? 0 : rightAverage;
                cutCounter.text = $"{leftAverage}";
                cutCounter.text += $"  {rightAverage}";
            }
            else
                cutCounter.text = $"{Math.Round(((double)(totalScoreRight + totalScoreLeft)) / (totalCutCountRight + totalCutCountLeft))}";

            noteCutInfos.Remove(v);
        }
    }
}