using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static CountersPlus.Utils.Accessors;

namespace CountersPlus.Utils
{
    public class CanvasUtility
    {
        private Dictionary<int, Canvas> CanvasIDToCanvas = new Dictionary<int, Canvas>();
        private Dictionary<Canvas, HUDCanvas> CanvasToSettings = new Dictionary<Canvas, HUDCanvas>();
        private Canvas energyCanvas;

        public CanvasUtility(HUDConfigModel hudConfig, CoreGameHUDController coreGameHUD)
        {
            energyCanvas = EnergyPanelGO(ref coreGameHUD).GetComponent<Canvas>();

            CanvasIDToCanvas.Add(-1, CreateCanvasWithConfig(hudConfig.MainCanvasSettings));
            for (int i = 0; i < hudConfig.OtherCanvasSettings.Count; i++)
            {
                HUDCanvas canvasSettings = hudConfig.OtherCanvasSettings[i];
                Canvas canvas = CreateCanvasWithConfig(canvasSettings);
                CanvasIDToCanvas.Add(i, canvas);
                CanvasToSettings.Add(canvas, canvasSettings);
            }
        }

        public Canvas CreateCanvasWithConfig(HUDCanvas canvasSettings)
        {
            GameObject CanvasGameObject = new GameObject("Counters+ | Counters Canvas");

            Vector3 CanvasPos = canvasSettings.Position;
            Vector3 CanvasRot = canvasSettings.Rotation;
            float CanvasPosScale = canvasSettings.PositionScale;
            float CanvasSize = canvasSettings.Size;

            Canvas canvas = CanvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            CanvasGameObject.transform.localScale = Vector3.one / CanvasSize;
            CanvasGameObject.transform.position = CanvasPos;
            CanvasGameObject.transform.rotation = Quaternion.Euler(CanvasRot);

            // Inherit canvas properties from the Energy Bar to ignore the shockwave effect.
            // However, a caveat as that, when viewing through walls, UI elements will not appear.
            if (canvasSettings.IgnoreShockwaveEffect)
            {
                canvas.overrideSorting = energyCanvas.overrideSorting;
                canvas.sortingLayerID = energyCanvas.sortingLayerID;
                canvas.sortingLayerName = energyCanvas.sortingLayerName;
                canvas.sortingOrder = energyCanvas.sortingOrder;
                canvas.gameObject.layer = energyCanvas.gameObject.layer;
            }

            // TODO handle interactions between Base Game and Counters+
            // (Attach Counters+ to Base Game HUD? Move Base Game HUD elements in between?)

            return canvas;
        }

        public TMP_Text CreateTextFromSettings(ConfigModel settings, Vector3? offset = null)
        {
            Canvas canvasToApply = CanvasIDToCanvas[settings.CanvasID];
            return CreateText(canvasToApply, DeterminePosition(settings), offset);
        }

        public TMP_Text CreateText(Canvas canvas, Vector3 anchoredPosition, Vector3? offset = null)
        {

            var rectTransform = canvas.transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(100, 50);

            float posScaleFactor = 10;
            if (CanvasToSettings.TryGetValue(canvas, out HUDCanvas settings))
            {
                posScaleFactor = settings.PositionScale;
            }

            if (offset != null)
            {
                anchoredPosition += offset.Value;
            }

            TMP_Text tmp_text = BeatSaberUI.CreateText(rectTransform, "", anchoredPosition * posScaleFactor);
            tmp_text.alignment = TextAlignmentOptions.Center;
            tmp_text.fontSize = 4f;
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
            tmp_text.enableWordWrapping = false;
            tmp_text.overflowMode = TextOverflowModes.Overflow;
            
            return tmp_text;
        }

        private Vector3 DeterminePosition(ConfigModel settings)
        {
            float comboOffset = MainConfigModel.Instance.ComboOffset;
            float multOffset = MainConfigModel.Instance.MultiplierOffset;
            CounterPositions position = settings.Position;
            int index = settings.Distance;
            Vector3 pos = new Vector3(); // Base position
            Vector3 offset = new Vector3(0, -0.75f * index, 0); // Offset 
            bool hud360 = false; // TODO re-implement 360 HUD
            float X = hud360 ? 2 : 3.2f;
            switch (position)
            {
                case CounterPositions.BelowCombo:
                    pos = new Vector3(-X, 1.15f - comboOffset, 7);
                    break;
                case CounterPositions.AboveCombo:
                    pos = new Vector3(-X, 2f + comboOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case CounterPositions.BelowMultiplier:
                    pos = new Vector3(X, 1.05f - multOffset, 7);
                    break;
                case CounterPositions.AboveMultiplier:
                    pos = new Vector3(X, 2f + multOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case CounterPositions.BelowEnergy:
                    pos = new Vector3(0, hud360 ? -0.25f : -1.5f, 7);
                    break;
                case CounterPositions.AboveHighway:
                    pos = new Vector3(0, 2.5f, 7);
                    offset = new Vector3(0, (offset.y * -1) + (hud360 ? 0.25f : 0.75f), 0);
                    break;
            }
            return pos + offset;
        }
    }
}
