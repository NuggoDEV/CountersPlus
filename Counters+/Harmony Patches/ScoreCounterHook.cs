using System;
using HarmonyLib;
using UnityEngine;
using TMPro;
using CountersPlus.Counters;
using CountersPlus.Config;

namespace CountersPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(ImmediateRankUIPanel))]
    [HarmonyPatch("Start", MethodType.Normal)]
    class ScoreCounterStartHook
    {
        static bool Prefix(ref ImmediateRankUIPanel __instance, ref TextMeshProUGUI ____rankText, ref TextMeshProUGUI ____relativeScoreText)
        {
            if (!CountersController.settings.scoreConfig.Enabled || !CountersController.settings.Enabled) return true;
            if (__instance.gameObject.GetComponent<ScoreCounter>() == null) return true;
            __instance.gameObject.GetComponent<ScoreCounter>().UpdateTextMeshes(ref ____rankText, ref ____relativeScoreText);
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
            if (model.DisplayRank) //Dont mind me im dumdum and forgot to check for this setting
            {
                RankModel.Rank rank = ____relativeScoreAndImmediateRankCounter.immediateRank;
                if (rank != ____prevImmediateRank)
                {
                    string rankName = RankModel.GetRankName(rank);
                    ____rankText.text = rankName;
                    ____prevImmediateRank = rank;
                    //I am moving this code down here so that it only runs if the rank changes instead of every time the game refreshes the UI.
                    //Because of how cosmic brain Beat Games is, this code should run on game startup, because SSS != SS, so should work fine.
                    if (model.CustomRankColors)
                    {
                        string color = "#FFFFFF"; //Blank white shall be used for the default color in case like some SSS shit happens
                        switch (rankName) //Using PogU switch case instead of Pepega If chain
                        {
                            case "SS": color = model.SSColor; break; //Even compressing this shit down to one liners, look at me!
                            case "S": color = model.SColor; break;
                            case "A": color = model.AColor; break;
                            case "B": color = model.BColor; break;
                            case "C": color = model.CColor; break;
                            case "D": color = model.DColor; break;
                            case "E": color = model.EColor; break;
                        }
                        ColorUtility.TryParseHtmlString(color, out Color RankColor); //converts config hex color to unity RGBA value
                        ____rankText.color = RankColor; //sets color of ranktext
                    }
                }
            }
            float score = ____relativeScoreAndImmediateRankCounter.relativeScore;
            float roundedScore = (float)Math.Round((decimal)score * 100, model.DecimalPrecision);
            ____relativeScoreText.text = $"{roundedScore.ToString($"F{model.DecimalPrecision}")}%";
            ____prevRelativeScore = score;
            return false;
        }
    }
}
