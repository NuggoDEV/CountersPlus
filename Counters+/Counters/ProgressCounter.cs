using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using HMUI;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static CountersPlus.Utils.Accessors;

namespace CountersPlus.Counters
{
    internal class ProgressCounter : Counter<ProgressConfigModel>, ITickable
    {
        private readonly Vector3 ringSize = Vector3.one * 1.175f;
        private readonly string multiplierImageSpriteName = "Circle";

        [Inject] private AudioTimeSyncController atsc;
        [Inject(Optional = true)] private CoreGameHUDController coreGameHUD; // For getting multiplier image
        [Inject] private GameplayCoreSceneSetupData gcssd; // I hope this works

        private TMP_Text timeText;
        private ImageView progressRing;
        private float length = 0;
        private float songBPM = 100;

        public override void CounterInit()
        {
            length = atsc.songLength;
            songBPM = gcssd.difficultyBeatmap.level.beatsPerMinute;

            timeText = CanvasUtility.CreateTextFromSettings(Settings);
            timeText.fontSize = 4;

            // Set default text, which is visible in Multiplayer
            // Missing cases are covered by the default "_" case
            timeText.text = Settings.Mode switch
            {
                ProgressMode.Percent when Settings.ProgressTimeLeft => "100%",
                ProgressMode.Percent when !Settings.ProgressTimeLeft => "0%",

                ProgressMode.TimeInBeats when Settings.ProgressTimeLeft
                    => $"{Mathf.Round(songBPM / 60 * length / 0.25f) * 0.25f:F2}",
                ProgressMode.TimeInBeats when !Settings.ProgressTimeLeft => "0.00",

                _ when Settings.ProgressTimeLeft => $"{atsc.songLength:F2}",
                _ when !Settings.ProgressTimeLeft => "0:00"
            };

            if (coreGameHUD != null)
            {
                GameObject baseGameProgress = SongProgressPanelGO(ref coreGameHUD);
                UnityEngine.Object.Destroy(baseGameProgress); // I'm sorry, little one.
            }

            if (Settings.Mode != ProgressMode.Percent)
            {
                var canvas = CanvasUtility.GetCanvasFromID(Settings.CanvasID);
                if (canvas != null)
                {
                    ImageView backgroundImage = CreateRing(canvas);
                    backgroundImage.rectTransform.anchoredPosition = timeText.rectTransform.anchoredPosition;
                    backgroundImage.CrossFadeAlpha(0.05f, 1f, false);
                    backgroundImage.transform.localScale = ringSize / 10;
                    backgroundImage.type = Image.Type.Simple;

                    progressRing = CreateRing(canvas);
                    progressRing.rectTransform.anchoredPosition = timeText.rectTransform.anchoredPosition;
                    progressRing.transform.localScale = ringSize / 10;

                    // Start progress ring at 100% or 0%, depending on how the ring will behave
                    progressRing.fillAmount = (Settings.ProgressTimeLeft && Settings.IncludeRing) ? 1 : 0;
                }
            }
        }

        public void Tick()
        {
            var time = atsc.songTime;
            if (Settings.ProgressTimeLeft) time = length - time;
            if (time <= 0f) return;

            switch (Settings.Mode)
            {
                case ProgressMode.TimeInBeats:
                    float beats = Mathf.Round(songBPM / 60 * time / 0.25f) * 0.25f;
                    timeText.text = beats.ToString("F2");
                    break;
                case ProgressMode.Original:
                    timeText.text = $"{Math.Floor(time / 60):N0}:{Math.Floor(time % 60):00}";
                    break;
                default:
                    timeText.text = $"{time / length * 100:00}%";
                    return;
            }

            progressRing.fillAmount = (Settings.IncludeRing ? time : atsc.songTime) / length;
            progressRing.SetVerticesDirty();
        }

        private ImageView CreateRing(Canvas canvas)
        {
            // Unfortunately, there is no guarantee that I have the CoreGameHUDController, since No Text and Huds
            // completely disables it from spawning. So, to be safe, we recreate this all from scratch.
            GameObject imageGameObject = new GameObject("Ring Image", typeof(RectTransform));
            imageGameObject.transform.SetParent(canvas.transform, false);
            ImageView newImage = imageGameObject.AddComponent<ImageView>();
            newImage.enabled = false;
            newImage.material = Utilities.ImageResources.NoGlowMat;
            newImage.sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(x => x.name == multiplierImageSpriteName);
            newImage.type = Image.Type.Filled;
            newImage.fillClockwise = true;
            newImage.fillOrigin = 2;
            newImage.fillAmount = 1;
            newImage.fillMethod = Image.FillMethod.Radial360;
            newImage.enabled = true;
            return newImage;
        }
    }
}
