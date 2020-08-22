using CountersPlus.ConfigModels;
using TMPro;
using UnityEngine;
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

            Vector3 anchoredPos = (CanvasUtility.GetAnchoredPositionFromConfig(Settings, currentSettings.IsMainCanvas) + offset) * positionScale;

            pointsTextTransform.anchoredPosition = anchoredPos;
            scoreTransform.anchoredPosition = anchoredPos + baseGameScoreOffset;
            rankTransform.anchoredPosition = anchoredPos + baseGameRankOffset;

            Object.Destroy(coreGameHUD.GetComponentInChildren<ImmediateRankUIPanel>());

            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent += UpdateText;

            UpdateText();
        }

        private void UpdateText()
        {
            RankModel.Rank immediateRank = relativeScoreAndImmediateRank.immediateRank;
            if (immediateRank != prevImmediateRank)
            {
                rankText.text = RankModel.GetRankName(immediateRank);
                prevImmediateRank = immediateRank;

                Color RankColor = Color.white;
                if (Settings.CustomRankColors)
                {
                    RankColor = Settings.GetRankColorFromRank(immediateRank);
                }
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
