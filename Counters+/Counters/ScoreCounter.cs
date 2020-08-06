using System.Collections;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using CountersPlus.Counters;

namespace CountersPlus.Counters
{
    public class ScoreCounter : Counter<ScoreConfigModel>
    {
        private CountersData localData; //Used for transferring data
        private Vector3 localPosition; //Used for transferring data

        internal override void Counter_Start() { }

        internal override void Counter_Destroy() { }

        internal override void Init(CountersData data, Vector3 position)
        {
            localData = data;
            localPosition = position;
            if (gameObject.name != "ScoreCanvas")
            {
                StartCoroutine(YeetToBaseCounter());
                return;
            }
        }

        IEnumerator YeetToBaseCounter()
        {
            GameObject baseCounter;
            yield return new WaitUntil(() => GameObject.Find("ScoreCanvas") != null);
            baseCounter = GameObject.Find("ScoreCanvas");
            ScoreCounter newCounter = baseCounter.AddComponent<ScoreCounter>();
            newCounter.settings = settings;
            newCounter.Init(localData, localPosition);
            CountersController.LoadedCounters[typeof(ScoreConfigModel)] = newCounter;

            Vector3 position = DeterminePosition(this, localData);
            position.x *= 2;
            position.z = 0;
            baseCounter.transform.localPosition = position - new Vector3(0, 0.4f, 0);

            baseCounter.GetComponent<ImmediateRankUIPanel>().Start();
            Destroy(gameObject);
        }

        public void UpdateTextMeshes(ref TextMeshProUGUI rank, ref TextMeshProUGUI score)
        {
            if (settings.Mode != ICounterMode.BaseGame && settings.Mode != ICounterMode.BaseWithOutPoints)
            {
                score.text = "100.0%";
                score.fontSize = 1.75f * TextHelper.SizeScaleFactor;
                score.color = Color.white;
                score.alignment = TextAlignmentOptions.Center;

                rank.text = "SS";
                rank.fontSize = 2.25f * TextHelper.SizeScaleFactor;

                if (!settings.CustomRankColors)
                {
                    rank.color = Color.white; //if custom rank colors is disabled, just set color to white
                }
                else
                {
                    ColorUtility.TryParseHtmlString(settings.SSColor, out Color defaultColor);
                    rank.color = defaultColor;
                }
                rank.alignment = TextAlignmentOptions.Center;
                rank.overflowMode = TextOverflowModes.Overflow;
            }
            PBCounter personalBest = GameObject.Find("Counters+ | Personal Best Counter").GetComponent<PBCounter>();
            Plugin.Log($"{personalBest == null}", LogInfo.Error);
            if (personalBest != null)
            {
                personalBest.UpdatePBCounterToScorePosition(score);
            }
        }
    }
}
