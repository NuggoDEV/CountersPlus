using System;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using System.Collections.Generic;

namespace CountersPlus.Counters
{
    public class CutCounter : Counter<CutConfigModel>
    {
        private TMP_Text cutLabel;
        private BeatmapObjectSpawnController beatmapObjectSpawnController;
        private GameObject _RankObject;
        private TMP_Text cutCounter;
        private int totalCutCountLeft = 0;
        private int totalCutCountRight = 0;
        private int totalScoreLeft = 0; // MaxScoreForNumberOfNotes() is an int. Don't expect scores over 2 billion.
        private int totalScoreRight = 0;

        private Dictionary<SaberSwingRatingCounter, NoteCutInfo> noteCutInfos = new Dictionary<SaberSwingRatingCounter, NoteCutInfo>();

        internal override void Counter_Start() { }

        internal override void Init(CountersData data)
        {
            beatmapObjectSpawnController = data.BOSC;
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
            
            if (beatmapObjectSpawnController != null)
                beatmapObjectSpawnController.noteWasCutEvent += UpdateScore;
        }

        internal override void Counter_Destroy()
        {
            beatmapObjectSpawnController.noteWasCutEvent -= UpdateScore;
        }

        private void UpdateScore(BeatmapObjectSpawnController bosc, INoteController data, NoteCutInfo info)
        {
            if (data.noteData.noteType == NoteType.Bomb || !info.allIsOK) return;
            noteCutInfos.Add(info.swingRatingCounter, info);
            info.swingRatingCounter.didFinishEvent -= SaberSwingRatingCounter_didFinishEvent;
            info.swingRatingCounter.didFinishEvent += SaberSwingRatingCounter_didFinishEvent;
        }

        private void SaberSwingRatingCounter_didFinishEvent(SaberSwingRatingCounter v)
        {
            ScoreController.RawScoreWithoutMultiplier(noteCutInfos[v], out int beforeCut, out int afterCut, out int cutDistance);
            //"cutDistanceRawScore" is already calculated into "beforeCutRawScore"
            if (noteCutInfos[v].saberType == Saber.SaberType.SaberA)
            {
                totalScoreLeft += beforeCut + afterCut + cutDistance;
                ++totalCutCountLeft;
            }
            else
            {
                totalScoreRight += beforeCut + afterCut + cutDistance;
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