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
        private BeatmapObjectManager beatmapObjectManager;
        private GameObject _RankObject;
        private TMP_Text cutCounterLeft;
        private TMP_Text cutCounterRight;
        private int totalCutCountLeft = 0;
        private int totalCutCountRight = 0;
        private int[] totalScoresLeft = new int[] { 0, 0, 0 }; // [0]=beforeCut, [1]=afterCut, [2]=cutDistance
        private int[] totalScoresRight = new int[] { 0, 0, 0 };

        private Dictionary<SaberSwingRatingCounter, NoteCutInfo> noteCutInfos = new Dictionary<SaberSwingRatingCounter, NoteCutInfo>();

        internal override void Counter_Start() { }

        internal override void Init(CountersData data)
        {
            beatmapObjectManager = data.BOM;
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
            TextHelper.CreateText(out cutLabel, position);
            cutLabel.text = "Average Cut";
            cutLabel.fontSize = 3;
            cutLabel.color = Color.white;
            cutLabel.alignment = TextAlignmentOptions.Center;

            _RankObject = new GameObject("Counters+ | Cut Label");
            _RankObject.transform.parent = transform;
            TextHelper.CreateText(out cutCounterLeft, position - new Vector3(0, settings.SeparateCutValues ? 0.9f : 0.4f, 0));
            cutCounterLeft.fontSize = 4;
            cutCounterLeft.color = Color.white;
            cutCounterLeft.lineSpacing = -26;
            cutCounterLeft.text = settings.SeparateCutValues ? "0\n0\n0" : "0";
            cutCounterLeft.alignment = TextAlignmentOptions.Center;

            if (settings.SeparateSaberCounts)
            {
                TextHelper.CreateText(out cutCounterRight, position - new Vector3(-0.2f, settings.SeparateCutValues ? 0.9f : 0.4f, 0));
                cutCounterRight.fontSize = 4;
                cutCounterRight.color = Color.white;
                cutCounterRight.lineSpacing = -26;
                cutCounterRight.text = settings.SeparateCutValues ? "0\n0\n0" : "0";
                cutCounterRight.alignment = TextAlignmentOptions.Left;

                cutCounterLeft.alignment = TextAlignmentOptions.Right;
                cutCounterLeft.transform.position = position - new Vector3(0.2f, settings.SeparateCutValues ? 0.9f : 0.4f, 0);
            }

            if (beatmapObjectManager != null)
                beatmapObjectManager.noteWasCutEvent += UpdateScore;
        }

        internal override void Counter_Destroy()
        {
            beatmapObjectManager.noteWasCutEvent -= UpdateScore;
        }

        private void UpdateScore(INoteController data, NoteCutInfo info)
        {
            if (data.noteData.noteType == NoteType.Bomb || !info.allIsOK) return;
            noteCutInfos.Add(info.swingRatingCounter, info);
            info.swingRatingCounter.didFinishEvent -= SaberSwingRatingCounter_didFinishEvent;
            info.swingRatingCounter.didFinishEvent += SaberSwingRatingCounter_didFinishEvent;
        }

        private void SaberSwingRatingCounter_didFinishEvent(SaberSwingRatingCounter v)
        {
            ScoreModel.RawScoreWithoutMultiplier(noteCutInfos[v], out int beforeCut, out int afterCut, out int cutDistance);
            //"cutDistanceRawScore" is already calculated into "beforeCutRawScore"
            if (noteCutInfos[v].saberType == SaberType.SaberA)
            {
                totalScoresLeft[0] += beforeCut;
                totalScoresLeft[1] += afterCut;
                totalScoresLeft[2] += cutDistance;
                ++totalCutCountLeft;
            }
            else
            {
                totalScoresRight[0] += beforeCut;
                totalScoresRight[1] += afterCut;
                totalScoresRight[2] += cutDistance;
                ++totalCutCountRight;
            }

            UpdateLabels();

            noteCutInfos.Remove(v);
        }

        private void UpdateLabels()
        {
            double[] leftAverages = new double[3]
            {
                SafeDivideScore(totalScoresLeft[0], totalCutCountLeft),
                SafeDivideScore(totalScoresLeft[1], totalCutCountLeft),
                SafeDivideScore(totalScoresLeft[2], totalCutCountLeft)
            };

            double[] rightAverages = new double[3]
            {
                SafeDivideScore(totalScoresRight[0], (totalCutCountRight)),
                SafeDivideScore(totalScoresRight[1], (totalCutCountRight)),
                SafeDivideScore(totalScoresRight[2], (totalCutCountRight))
            };

            if (settings.SeparateCutValues && settings.SeparateSaberCounts)
            {
                cutCounterLeft.text = $"{leftAverages[0]}\n{leftAverages[1]}\n{leftAverages[2]}";
                cutCounterRight.text = $"{rightAverages[0]}\n{rightAverages[1]}\n{rightAverages[2]}";
            }
            else if (settings.SeparateCutValues)
            {
                cutCounterLeft.text = $"{(leftAverages[0] + rightAverages[0]) / 2}\n{(leftAverages[1] + rightAverages[1]) / 2}\n{(leftAverages[2] + rightAverages[2]) / 2}";
            }
            else if (settings.SeparateSaberCounts)
            {
                cutCounterLeft.text = $"{leftAverages[0] + leftAverages[1] + leftAverages[2]}";
                cutCounterRight.text = $"{rightAverages[0] + rightAverages[1] + rightAverages[2]}";
            }
            else
            {
                // Divide combined left and right score by 2 only if both sabers have cut something.
                int divisor = (totalCutCountRight * totalCutCountLeft == 0 ? 1 : 2);
                cutCounterLeft.text = $"{(leftAverages[0] + leftAverages[1] + leftAverages[2] + rightAverages[0] + rightAverages[1] + rightAverages[2]) / divisor}";
            }
        }

        private double SafeDivideScore(int total, int count)
        {
            double result = Math.Round(((double)(total)) / (count));
            return Double.IsNaN(result) ? 0 : result;
        }
    }
}