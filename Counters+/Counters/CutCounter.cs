using System;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using System.Collections.Generic;
using System.Globalization;
using CountersPlus.UI.ViewControllers.ConfigModelControllers;

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

        internal override void Init(CountersData data, Vector3 position)
        {
            string iniValue = FormatLabel(0, 0, settings.AveragePrecision);
            beatmapObjectManager = data.BOM;
            TextHelper.CreateText(out cutLabel, position);
            cutLabel.text = "Average Cut";
            cutLabel.fontSize = 3;
            cutLabel.color = Color.white;
            cutLabel.alignment = TextAlignmentOptions.Center;

            _RankObject = new GameObject("Counters+ | Cut Label");
            _RankObject.transform.parent = transform;
            TextHelper.CreateText(out cutCounterLeft, position - new Vector3(0, 0.2f, 0));
            cutCounterLeft.fontSize = 4;
            cutCounterLeft.color = Color.white;
            cutCounterLeft.lineSpacing = -26;
            cutCounterLeft.text = settings.SeparateCutValues ? $"{iniValue}\n{iniValue}\n{iniValue}" : $"{iniValue}";
            cutCounterLeft.alignment = TextAlignmentOptions.Top;

            if (settings.SeparateSaberCounts)
            {
                TextHelper.CreateText(out cutCounterRight, position - new Vector3(-0.2f, 0.2f, 0));
                cutCounterRight.fontSize = 4;
                cutCounterRight.color = Color.white;
                cutCounterRight.lineSpacing = -26;
                cutCounterRight.text = settings.SeparateCutValues ? $"{iniValue}\n{iniValue}\n{iniValue}" : $"{iniValue}";
                cutCounterRight.alignment = TextAlignmentOptions.TopLeft;

                cutCounterLeft.alignment = TextAlignmentOptions.TopRight;
                cutCounterLeft.transform.position = position - new Vector3(0.2f, 0.2f, 0);
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
            int shownDecimals = settings.AveragePrecision;

            if (settings.SeparateCutValues && settings.SeparateSaberCounts)
            {
                string[] leftAverages = new string[3]
                {
                    FormatLabel(totalScoresLeft[0], totalCutCountLeft, shownDecimals),
                    FormatLabel(totalScoresLeft[1], totalCutCountLeft, shownDecimals),
                    FormatLabel(totalScoresLeft[2], totalCutCountLeft, shownDecimals)
                };
                string[] rightAverages = new string[3]
                {
                    FormatLabel(totalScoresRight[0], totalCutCountRight, shownDecimals),
                    FormatLabel(totalScoresRight[1], totalCutCountRight, shownDecimals),
                    FormatLabel(totalScoresRight[2], totalCutCountRight, shownDecimals)
                };
                cutCounterLeft.text = $"{leftAverages[0]}\n{leftAverages[1]}\n{leftAverages[2]}";
                cutCounterRight.text = $"{rightAverages[0]}\n{rightAverages[1]}\n{rightAverages[2]}";
            }
            else if (settings.SeparateCutValues)
            {
                int totalCutCount = totalCutCountLeft + totalCutCountRight;
                string[] cutValueAverages = new string[3]
                {
                    FormatLabel(totalScoresLeft[0] + totalScoresRight[0], totalCutCount, shownDecimals),
                    FormatLabel(totalScoresLeft[1] + totalScoresRight[1], totalCutCount, shownDecimals),
                    FormatLabel(totalScoresLeft[2] + totalScoresRight[2], totalCutCount, shownDecimals)
                };
                cutCounterLeft.text = $"{cutValueAverages[0]}\n{cutValueAverages[1]}\n{cutValueAverages[2]}";
            }
            else if (settings.SeparateSaberCounts)
            {
                string[] saberAverages = new string[2]
                {
                    FormatLabel(totalScoresLeft[0] + totalScoresLeft[1] + totalScoresLeft[2], totalCutCountLeft, shownDecimals),
                    FormatLabel(totalScoresRight[0] + totalScoresRight[1] + totalScoresRight[2], totalCutCountRight, shownDecimals)
                };
                cutCounterLeft.text = $"{saberAverages[0]}";
                cutCounterRight.text = $"{saberAverages[1]}";
            }
            else
            {
                string averages = FormatLabel(totalScoresLeft[0] + totalScoresLeft[1] + totalScoresLeft[2] + totalScoresRight[0] + totalScoresRight[1] + totalScoresRight[2], totalCutCountLeft + totalCutCountRight, shownDecimals);
                cutCounterLeft.text = $"{averages}";
            }
        }

        private double SafeDivideScore(int total, int count)
        {
            double result = (double)(total) / (count);
            return Double.IsNaN(result) ? 0 : result;
        }

        private string FormatLabel(int totalScore, int totalCuts, int decimals)
        {
            return SafeDivideScore(totalScore, totalCuts).ToString($"F{decimals}", CultureInfo.InvariantCulture);
        }
    }
}