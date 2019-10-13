using System;
using Harmony;
using UnityEngine;
using TMPro;
using CountersPlus.Counters;
using CountersPlus.Config;

namespace CountersPlus.Harmony
{

    [HarmonyPatch(typeof(ImmediateRankUIPanel))]
    [HarmonyPatch("Start", MethodType.Normal)]
    class ScoreCounterStartHook
    {
        static bool Prefix(ref ImmediateRankUIPanel __instance, ref TextMeshProUGUI ____rankText, ref TextMeshProUGUI ____relativeScoreText)
        {
            if (!CountersController.settings.scoreConfig.Enabled || !CountersController.settings.Enabled) return true;
            if (__instance.gameObject.GetComponent<ScoreCounter>() == null) return true;
            ____rankText = (TextMeshProUGUI)__instance.gameObject.GetComponent<ScoreCounter>().RankText;
            ____relativeScoreText = (TextMeshProUGUI)__instance.gameObject.GetComponent<ScoreCounter>().ScoreMesh;
            return false;
        }
    }

    [HarmonyPatch(typeof(ImmediateRankUIPanel))]
    [HarmonyPatch("RefreshUI", MethodType.Normal)]
    class ScoreCounterRefreshUIHook
    {
        static ScoreConfigModel model = null;
        
        static bool Prefix(ref ImmediateRankUIPanel __instance, ref RelativeScoreAndImmediateRankCounter ____relativeScoreAndImmediateRankCounter,
            ref RankModel.Rank ____prevImmediateRank, ref float ____prevRelativeScore, ref TextMeshProUGUI ____rankText,
            ref TextMeshProUGUI ____relativeScoreText)
        {
            if (!CountersController.settings.Enabled) return true; //Dont use Score Counters decimal precision if the plugin is disabled
            if (model == null) model = CountersController.settings.scoreConfig;
            RankModel.Rank rank = ____relativeScoreAndImmediateRankCounter.immediateRank;
            if (rank != ____prevImmediateRank)
            {
                ____rankText.text = model.Mode != ICounterMode.BaseGame ? $"\n{RankModel.GetRankName(rank)}" : RankModel.GetRankName(rank);
                ____prevImmediateRank = rank;
            }
            if (model.CustomRankColors) //checks if custom rank colors is enabled
            {
                if (RankModel.GetRankName(rank) == "SS") 
                {
                    ColorUtility.TryParseHtmlString(model.SSColor, out Color RankColor); //converts config hex color to unity RGBA value
                    ____rankText.color = RankColor; //sets color of ranktext
                }
                if (RankModel.GetRankName(rank) == "S")
                {
                    ColorUtility.TryParseHtmlString(model.SColor, out Color RankColor);
                    ____rankText.color = RankColor;
                }
                if (RankModel.GetRankName(rank) == "A")
                {
                    ColorUtility.TryParseHtmlString(model.AColor, out Color RankColor);
                    ____rankText.color = RankColor;
                }
                if (RankModel.GetRankName(rank) == "B")
                {
                    ColorUtility.TryParseHtmlString(model.BColor, out Color RankColor);
                    ____rankText.color = RankColor;
                }
                if (RankModel.GetRankName(rank) == "C")
                {
                    ColorUtility.TryParseHtmlString(model.CColor, out Color RankColor);
                    ____rankText.color = RankColor;
                }
                if (RankModel.GetRankName(rank) == "D")
                {
                    ColorUtility.TryParseHtmlString(model.DColor, out Color RankColor);
                    ____rankText.color = RankColor;
                }
                if (RankModel.GetRankName(rank) == "E")
                {
                    ColorUtility.TryParseHtmlString(model.EColor, out Color RankColor);
                    ____rankText.color = RankColor;
                }
            }
            float score = ____relativeScoreAndImmediateRankCounter.relativeScore;
            if (Mathf.Abs(____prevRelativeScore - score) >= 0.001f)
            {
                float roundedScore = (float)Math.Round((decimal)score * 100, model.DecimalPrecision);
                ____relativeScoreText.text = $"{roundedScore.ToString($"F{model.DecimalPrecision}")}%";
            }
            return false;
        }
    }
}
