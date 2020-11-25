using CountersPlus.ConfigModels;
using TMPro;
using UnityEngine;
using Zenject;
using static CountersPlus.Utils.Accessors;

namespace CountersPlus.Counters
{
    // REVIEW: Perhaps look into harmony patches to take control of the base game score counter more... sanely?
    internal class ScoreCounter : Counter<ScoreConfigModel>
    {
        private readonly Vector3 offset = new Vector3(0, 1.91666f, 0);

        [Inject] private CoreGameHUDController coreGameHUD;
        [Inject] private RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRank;
        [Inject] private MainConfigModel mainConfig;

        private RankModel.Rank prevImmediateRank = RankModel.Rank.SSS;
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
            baseGameScore.transform.SetParent(old.transform, true);
            baseGameRank.transform.SetParent(old.transform, true);

            if (!mainConfig.ItalicText)
            {
                old.fontStyle = relativeScoreText.fontStyle = rankText.fontStyle = FontStyles.Normal;
                Vector3 localPosition = relativeScoreText.rectTransform.localPosition;
                relativeScoreText.rectTransform.localPosition = new Vector3(0, localPosition.y, localPosition.z);
                localPosition = rankText.rectTransform.localPosition;
                rankText.rectTransform.localPosition = new Vector3(0, localPosition.y, localPosition.z);
            }

            switch (Settings.Mode)
            {
                case ScoreMode.RankOnly:
                    Object.Destroy(baseGameScore.gameObject);
                    break;
                case ScoreMode.ScoreOnly:
                    Object.Destroy(baseGameRank.gameObject);
                    break;
            }

            RectTransform pointsTextTransform = old.rectTransform;

            HUDCanvas currentSettings = CanvasUtility.GetCanvasSettingsFromID(Settings.CanvasID);

            Vector2 anchoredPos = CanvasUtility.GetAnchoredPositionFromConfig(Settings) + (offset * (3f / currentSettings.PositionScale));

            pointsTextTransform.localPosition = anchoredPos * currentSettings.PositionScale;
            pointsTextTransform.localPosition = new Vector3(pointsTextTransform.localPosition.x, pointsTextTransform.localPosition.y, 0);
            pointsTextTransform.localEulerAngles = Vector3.zero;

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
