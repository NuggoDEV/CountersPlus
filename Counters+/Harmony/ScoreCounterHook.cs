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
        static void Postfix(ref ImmediateRankUIPanel __instance, ref TextMeshProUGUI ____rankText, ref TextMeshProUGUI ____relativeScoreText,
            ref RelativeScoreAndImmediateRankCounter ____relativeScoreAndImmediateRankCounter)
        {
            if (!CountersController.settings.scoreConfig.Enabled || !CountersController.settings.Enabled) return;
            if (__instance.gameObject.GetComponent<ScoreCounter>() == null)
            {
                ____relativeScoreAndImmediateRankCounter.relativeScoreOrImmediateRankDidChangeEvent -= __instance.HandleRelativeScoreAndImmediateRankCounterRelativeScoreOrImmediateRankDidChange;
                return;
            }
            ____rankText = (TextMeshProUGUI)__instance.gameObject.GetComponent<ScoreCounter>().RankText;
            ____relativeScoreText = (TextMeshProUGUI)__instance.gameObject.GetComponent<ScoreCounter>().ScoreMesh;
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
