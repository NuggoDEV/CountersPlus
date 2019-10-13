using System.Collections;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using System;

namespace CountersPlus.Counters
{
    public class ScoreCounter : MonoBehaviour
    {
        internal TMP_Text ScoreMesh;
        internal TMP_Text RankText;
        internal TMP_Text PointsText;
        
        private static ScoreConfigModel settings;


        GameObject _RankObject;
        int decimalPrecision;

        void Awake()
        {
            settings = CountersController.settings.scoreConfig;
            decimalPrecision = settings.DecimalPrecision;
            if (gameObject.name == "ScorePanel")
                PreInit();
            else
                StartCoroutine(YeetToBaseCounter());
        }

        IEnumerator YeetToBaseCounter()
        {
            GameObject baseCounter;
            yield return new WaitUntil(() => GameObject.Find("ScorePanel") != null);
            baseCounter = GameObject.Find("ScorePanel");
            CountersController.LoadedCounters.Remove(gameObject);
            baseCounter.AddComponent<ScoreCounter>();
            Destroy(gameObject);
            CountersController.LoadedCounters.Add(baseCounter);
        }

        private void PreInit()
        {
            if (!(settings.Mode == ICounterMode.BaseGame || settings.Mode == ICounterMode.BaseWithOutPoints))
            {
                for (var i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child.gameObject.name != "ScoreText")
                    {
                        if (child.GetComponent<TextMeshProUGUI>() != null) Destroy(child.GetComponent<TextMeshProUGUI>());
                        Destroy(child.gameObject);
                    }
                    else PointsText = child.GetComponent<TMP_Text>();
                }
                if (settings.Mode == ICounterMode.ScoreOnly) Destroy(GameObject.Find("ScoreText"));
                CreateText();
            }
            else
                transform.position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
        }

        private void CreateText()
        {
            transform.localScale = Vector3.one;
            transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().fontSize = 0.325f;
            GameObject scoreMesh = new GameObject("Counters+ | Score Percent");
            scoreMesh.transform.SetParent(transform, false);
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
            TextHelper.CreateText(out ScoreMesh, position);
            ScoreMesh.text = "100.0%";
            ScoreMesh.fontSize = 3;
            ScoreMesh.color = Color.white;
            ScoreMesh.alignment = TextAlignmentOptions.Center;
                
            //transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().rectTransform.position = position + new Vector3(-6.425f, 7.67f, 0);
            transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().rectTransform.position = position + new Vector3(-0.01f, 7.77f, 0);
            if (settings.DisplayRank)
            {
                _RankObject = new GameObject("Counters+ | Score Rank");
                _RankObject.transform.SetParent(transform, false);
                TextHelper.CreateText(out RankText, position);
                RankText.text = "\nSS";
                RankText.fontSize = 4;
                if (!settings.CustomRankColors)
                    RankText.color = Color.white; //if custom rank colors is disabled, just set color to white
                if (settings.CustomRankColors)
                {
                    ColorUtility.TryParseHtmlString(settings.SSColor, out Color defaultColor);
                    RankText.color = defaultColor;
                }
                RankText.alignment = TextAlignmentOptions.Center;
            }
            if (settings.Mode == ICounterMode.LeavePoints || settings.Mode == ICounterMode.BaseWithOutPoints)
            {
                transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().rectTransform.position = new Vector3(-3.2f,
                    0.35f + (settings.Mode == ICounterMode.LeavePoints ? 7.8f : 0), 7);
            }
            GetComponent<ImmediateRankUIPanel>().Start(); //BS way of getting Harmony patch to function but "if it works its not stupid" ~Caeden117
        }
        /*public void SetColor()
        {
            if (RankModel.GetRankName(relativeScoreAndImmediateRankCounter.immediateRank) == "SS" | RankModel.GetRankName(relativeScoreAndImmediateRankCounter.immediateRank) == "SSS")
            {
                ColorUtility.TryParseHtmlString(settings.SSColor, out Color RankColor);
                RankText.color = RankColor;
            }
            if (RankModel.GetRankName(relativeScoreAndImmediateRankCounter.immediateRank) == "S")
            {
                ColorUtility.TryParseHtmlString(settings.SColor, out Color RankColor);
                RankText.color = RankColor;
            }
            if (RankModel.GetRankName(relativeScoreAndImmediateRankCounter.immediateRank) == "A")
            {
                ColorUtility.TryParseHtmlString(settings.AColor, out Color RankColor);
                RankText.color = RankColor;
            }
            if (RankModel.GetRankName(relativeScoreAndImmediateRankCounter.immediateRank) == "B")
            {
                ColorUtility.TryParseHtmlString(settings.BColor, out Color RankColor);
                RankText.color = RankColor;
            }
            if (RankModel.GetRankName(relativeScoreAndImmediateRankCounter.immediateRank) == "C")
            {
                ColorUtility.TryParseHtmlString(settings.CColor, out Color RankColor);
                RankText.color = RankColor;
            }
            if (RankModel.GetRankName(relativeScoreAndImmediateRankCounter.immediateRank) == "D")
            {
                ColorUtility.TryParseHtmlString(settings.DColor, out Color RankColor);
                RankText.color = RankColor;
            }
            if (RankModel.GetRankName(relativeScoreAndImmediateRankCounter.immediateRank) == "E")
            {
                ColorUtility.TryParseHtmlString(settings.EColor, out Color RankColor);
                RankText.color = RankColor;
            }
            else
                RankText.color = Color.white;
            
        }*/
    }
}
