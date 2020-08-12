using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static CountersPlus.Utils.Accessors;

namespace CountersPlus.Counters
{
    // REVIEW: Perhaps look into harmony patches to take control of the base game score counter more... sanely?
    public class ScoreCounter : Counter<ScoreConfigModel>
    {
        private readonly Vector3 offset = new Vector3(0, -2, 0);

        [Inject] private CoreGameHUDController coreGameHUD;
        [Inject] private RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRank;

        private RankModel.Rank prevImmediateRank = RankModel.Rank.SS;
        private TextMeshProUGUI rankText;
        private TextMeshProUGUI relativeScoreText;

        public override void CounterInit()
        {
            ScoreUIController scoreUIController = coreGameHUD.GetComponentInChildren<ScoreUIController>();
            TextMeshProUGUI old = ScoreUIText(ref scoreUIController);
            GameObject baseGameScore = RelativeScoreGO(ref coreGameHUD);
            relativeScoreText = baseGameScore.GetComponent<TextMeshProUGUI>();
            relativeScoreText.color = Color.white;
            GameObject baseGameRank = ImmediateRankGO(ref coreGameHUD);
            rankText = baseGameRank.GetComponent<TextMeshProUGUI>();
            rankText.color = Color.white;

            Canvas currentCanvas = CanvasUtility.GetCanvasFromID(Settings.CanvasID);

            old.rectTransform.SetParent(currentCanvas.transform, true);
            baseGameScore.transform.SetParent(currentCanvas.transform, true);
            baseGameRank.transform.SetParent(currentCanvas.transform, true);

            RectTransform pointsTextTransform = old.rectTransform;
            RectTransform scoreTransform = baseGameScore.transform as RectTransform;
            RectTransform rankTransform = baseGameRank.transform as RectTransform;

            Vector3 baseGameScoreOffset = scoreTransform.anchoredPosition - pointsTextTransform.anchoredPosition;
            Vector3 baseGameRankOffset = rankTransform.anchoredPosition - pointsTextTransform.anchoredPosition;

            HUDCanvas currentSettings = CanvasUtility.GetCanvasSettingsFromCanvas(currentCanvas);
            float positionScale = currentSettings?.PositionScale ?? 10;

            Vector3 anchoredPos = (CanvasUtility.GetAnchoredPositionFromConfig(Settings) + offset) * positionScale;

            pointsTextTransform.anchoredPosition = anchoredPos;
            scoreTransform.anchoredPosition = anchoredPos + baseGameScoreOffset;
            rankTransform.anchoredPosition = anchoredPos + baseGameRankOffset;

            UnityEngine.Object.Destroy(coreGameHUD.GetComponentInChildren<ImmediateRankUIPanel>());

            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent += UpdateText;

            UpdateText();
        }

        private void UpdateText()
        {
            RankModel.Rank immediateRank = relativeScoreAndImmediateRank.immediateRank;
            if (immediateRank != prevImmediateRank)
            {
                string rankName = RankModel.GetRankName(immediateRank);
                rankText.text = RankModel.GetRankName(immediateRank);
                prevImmediateRank = immediateRank;

                string color = "#FFFFFF";
                if (Settings.CustomRankColors)
                {
                    switch (rankName)
                    {
                        case "SS": color = Settings.SSColor; break;
                        case "S": color = Settings.SColor; break;
                        case "A": color = Settings.AColor; break;
                        case "B": color = Settings.BColor; break;
                        case "C": color = Settings.CColor; break;
                        case "D": color = Settings.DColor; break;
                        case "E": color = Settings.EColor; break;
                    }
                }
                ColorUtility.TryParseHtmlString(color, out Color RankColor); //converts config hex color to unity RGBA value
                rankText.color = RankColor;
            }
            float relativeScore = relativeScoreAndImmediateRank.relativeScore * 100;
            relativeScoreText.text = $"{relativeScore.ToString($"F{Settings.DecimalPrecision}")}%";
        }

        public override void CounterDestroy()
        {
            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent -= UpdateText;
        }
    }
}
