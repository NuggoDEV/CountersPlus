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
            // Yeah this is required.
            // If the Score Counter is all alone on its own Canvas with nothing to accompany them,
            // I need to give 'em a friend or else they get shy and hide away in the void.
            _ = CanvasUtility.CreateTextFromSettings(Settings, null);

            ScoreUIController scoreUIController = coreGameHUD.GetComponentInChildren<ScoreUIController>();
            TextMeshProUGUI old = ScoreUIText(ref scoreUIController);
            
            GameObject baseGameScore = RelativeScoreGO(ref coreGameHUD);
            baseGameScore.SetActive(true);
            relativeScoreText = baseGameScore.GetComponent<TextMeshProUGUI>();
            relativeScoreText.enabled = true;
            relativeScoreText.color = Color.white;

            GameObject baseGameRank = ImmediateRankGO(ref coreGameHUD);
            baseGameRank.SetActive(true);
            rankText = baseGameRank.GetComponent<TextMeshProUGUI>();
            rankText.enabled = true;
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
        }

        private void UpdateText()
        {
            RankModel.Rank immediateRank = relativeScoreAndImmediateRank.immediateRank;
            if (immediateRank != prevImmediateRank)
            {
                rankText.text = RankModel.GetRankName(immediateRank);
                prevImmediateRank = immediateRank;

                rankText.color = Settings.CustomRankColors ? Settings.GetRankColorFromRank(immediateRank) : Color.white;
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
