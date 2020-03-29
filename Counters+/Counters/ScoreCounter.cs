using System.Collections;
using UnityEngine;
using TMPro;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    public class ScoreCounter : Counter<ScoreConfigModel>
    {
        internal TMP_Text ScoreMesh;
        internal TMP_Text RankText;
        internal TMP_Text PointsText;

        GameObject _RankObject;

        internal override void Counter_Start()
        {
            settings = CountersController.settings.scoreConfig;
            if (gameObject.name == "ScoreCanvas")
                PreInit();
            else
                StartCoroutine(YeetToBaseCounter());
        }
        internal override void Counter_Destroy() { }
        internal override void Init(CountersData data, Vector3 position) { }

        IEnumerator YeetToBaseCounter()
        {
            GameObject baseCounter;
            yield return new WaitUntil(() => GameObject.Find("ScoreCanvas") != null);
            baseCounter = GameObject.Find("ScoreCanvas");
            CountersController.LoadedCounters.Remove(gameObject);
            ScoreCounter newCounter = baseCounter.AddComponent<ScoreCounter>();
            newCounter.settings = settings;
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
            {
                transform.position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
                transform.SetParent(TextHelper.CounterCanvas.transform, true);
            }
        }

        private void CreateText()
        {
            transform.localScale = Vector3.one;
            PointsText.fontSize = 0.325f;
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
            TextHelper.CreateText(out ScoreMesh, position);
            ScoreMesh.text = "100.0%";
            ScoreMesh.fontSize = 3;
            ScoreMesh.color = Color.white;
            ScoreMesh.alignment = TextAlignmentOptions.Center;
            PointsText.rectTransform.SetParent(ScoreMesh.rectTransform, false);
            PointsText.rectTransform.localScale = PointsText.rectTransform.localScale * TextHelper.SizeScaleFactor;
            PointsText.rectTransform.localPosition = new Vector3(0, 77.7f, 0);
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
                PointsText.rectTransform.position = new Vector3(-3.2f,
                    0.35f + (settings.Mode == ICounterMode.LeavePoints ? 7.8f : 0), 7);
            }
            //BS way of getting Harmony patch to function but "if it works its not stupid" ~Caeden117
            GetComponent<ImmediateRankUIPanel>().Start();
        }
    }
}
