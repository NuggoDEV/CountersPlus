using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.Utils
{
    public class AprilFools : IInitializable, ITickable, IDisposable, INoteEventHandler
    {
        private float t = 0;

        private TMP_FontAsset mainFont = BeatSaberUI.MainTextFont;

        private byte originalItalicStyle = 0;

        private IEnumerable<TMP_Text> allText = Enumerable.Empty<TMP_Text>();

        public void Initialize()
        {
            Plugin.Logger.Info("April Fools active.");
            originalItalicStyle = mainFont.italicStyle;
        }

        public void Dispose()
        {
            mainFont.italicStyle = originalItalicStyle;
        }

        public void Tick()
        {
            if (!allText.Any())
            {
                allText = Resources.FindObjectsOfTypeAll<CurvedTextMeshPro>().Where(x => x.isActiveAndEnabled);
            }

            t += Time.deltaTime;

            // TL;DR: animate italicizing and make it look good
            mainFont.italicStyle = (byte)Mathf.Clamp(Mathf.Abs(t / 5 * Mathf.Sin(t / 5) / 5), 0, byte.MaxValue);

            // THIS IS SUPER EXPENSIVE, PROBABLY FRAME KILLING, BUT THE OPPORTUNITY IS TOO GOOD TO PASS UP
            foreach (var tmp in allText)
            {
                tmp.fontStyle |= FontStyles.Italic; // WE ARE FORCING THESE GUYS TO BE ITALIC
                tmp.font = mainFont;
                tmp.SetAllDirty(); // holy shit this is so dirty
            }
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (!info.allIsOK) t += 30;
        }

        public void OnNoteMiss(NoteData data)
        {
            if (data.colorType != ColorType.None) t += 30;
        }
    }
}
