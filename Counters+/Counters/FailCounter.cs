using System.Linq;
using CountersPlus.Config;
using UnityEngine;
using TMPro;
using System.Collections;

namespace CountersPlus.Counters
{
    class FailCounter : Counter<FailConfigModel>
    {
        private static int Restarts = 0;
        private static IBeatmapLevel CurrentLevel = null;
        private static IDifficultyBeatmap BeatmapDiff = null;
        private TMP_Text failText;
        private GameEnergyCounter energy;
        private int fails = 0;

        internal override void Counter_Start() { }

        internal override void Init(CountersData data, Vector3 position)
        {
            //Because CountersController.ReadyToInit relies on finding other objects via Resources.FindObjectsOfTypeAll<>()
            //before invoking, it is safe to assume that the objects we find do indeed exist.
            energy = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().First();
            fails = data.PlayerData.playerData.playerAllOverallStatsData.allOverallStatsData.failedLevelsCount;

            if (settings.ShowRestartsInstead)
            {
                bool SameLevel = false;
                if (CurrentLevel != null) {
                    SameLevel = data.GCSSD.difficultyBeatmap.level.songName == CurrentLevel.songName && //I mean,
                        data.GCSSD.difficultyBeatmap.level.songSubName == CurrentLevel.songSubName && //Probably not the best way to compare levels,
                        data.GCSSD.difficultyBeatmap.level.songAuthorName == CurrentLevel.songAuthorName && //But that means I have more lines to spend
                        data.GCSSD.difficultyBeatmap.beatmapData.notesCount == BeatmapDiff.beatmapData.notesCount && //Making snarky comments like these
                        data.GCSSD.difficultyBeatmap.beatmapData.obstaclesCount == BeatmapDiff.beatmapData.obstaclesCount && //For you to find
                        data.GCSSD.difficultyBeatmap.beatmapData.bombsCount == BeatmapDiff.beatmapData.bombsCount; //And to @ me on Discord.
                }
                if (SameLevel) Restarts++;
                else
                {
                    CurrentLevel = data.GCSSD.difficultyBeatmap.level;
                    BeatmapDiff = data.GCSSD.difficultyBeatmap;
                    Restarts = 0;
                }
            }

            TextHelper.CreateText(out failText, position - new Vector3(0, 0.4f, 0));
            failText.text = settings.ShowRestartsInstead ? Restarts.ToString() : fails.ToString();
            failText.fontSize = 4;
            failText.color = Color.white;
            failText.alignment = TextAlignmentOptions.Center;

            GameObject labelGO = new GameObject("Counters+ | Fail Label");
            labelGO.transform.parent = transform;
            TextHelper.CreateText(out TMP_Text label, position);
            label.text = settings.ShowRestartsInstead ? "Restarts" : "Fails";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

            if (!settings.ShowRestartsInstead) energy.gameEnergyDidReach0Event += SlowlyAnnoyThePlayerBecauseTheyFailed;
        }

        private void SlowlyAnnoyThePlayerBecauseTheyFailed()
        {
            failText.text = (fails + 1).ToString();
            StartCoroutine(ChangeTextColorToAnnoyThePlayerEvenMore());
        }

        private IEnumerator ChangeTextColorToAnnoyThePlayerEvenMore()
        {
            float t = 0;
            while (t <= 1)
            {
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
                failText.color = Color.Lerp(Color.white, Color.red, t);
            }
            failText.color = Color.red;
        }

        internal override void Counter_Destroy()
        {
            if (!settings.ShowRestartsInstead) energy.gameEnergyDidReach0Event -= SlowlyAnnoyThePlayerBecauseTheyFailed;
        }
    }
}
