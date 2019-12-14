using UnityEngine;
using TMPro;
using BeatSaberMarkupLanguage;
using CountersPlus.Utils;
using System.Linq;

namespace CountersPlus
{
    class TextHelper
    {
        /*
         * Thank the god himself Viscoci for providing me with the code to fix displaying text with Counters+.
         * 
         * I cannot thank him enough.
         */
        public static Canvas CounterCanvas;
        public static float PosScaleFactor => CountersController.settings.hudConfig.HUDPositionScaleFactor;
        public static float SizeScaleFactor => CountersController.settings.hudConfig.HUDSize;

        public static Canvas CreateCanvas(Vector3 Position, bool floatingHUD = false, float CanvasScaleFactor = 10)
        {
            Canvas canvas;
            GameObject CanvasGO = new GameObject("Counters+ | Counters Canvas");
            canvas = CanvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasGO.transform.localScale = Vector3.one / CanvasScaleFactor;
            CanvasGO.transform.position = Position;
            CanvasGO.transform.rotation = Quaternion.Euler(CountersController.settings.hudConfig.HUDRotation);

            GameObject coreGameHUD = Resources.FindObjectsOfTypeAll<CoreGameHUDController>()?.FirstOrDefault()?.gameObject ?? null;
            if (CountersController.settings.hudConfig.AttachBaseGameHUD && coreGameHUD != null)
            {
                coreGameHUD.transform.SetParent(CanvasGO.transform, true);
                coreGameHUD.transform.localScale = Vector3.one * 10;
                coreGameHUD.transform.localPosition = Position;
                coreGameHUD.transform.localRotation = Quaternion.identity;
                foreach (Transform children in coreGameHUD.transform)
                {
                    if (children.Find("BG")) children.Find("BG").gameObject.SetActive(false);
                    if (children.Find("Top")) children.Find("Top").gameObject.SetActive(false);
                    children.localPosition = new Vector3(children.localPosition.x, children.localPosition.y, 0);
                }
            }

            if (floatingHUD) CanvasGO.AddComponent<AssignedFloatingWindow>();
            
            return canvas;
        }

        public static void CreateText(out TMP_Text tmp_text, Vector3 anchoredPosition)
        {
            if (CounterCanvas == null)
            {
                bool useFloatingHUD = CountersController.settings.hudConfig.AttachHUDToCamera;
                float scale = CountersController.settings.hudConfig.HUDSize;
                CounterCanvas = CreateCanvas(CountersController.settings.hudConfig.HUDPosition, useFloatingHUD, scale);
            }
            CreateText(out tmp_text, CounterCanvas, anchoredPosition);
        }

        public static void CreateText(out TMP_Text tmp_text, Canvas canvas, Vector3 anchoredPosition)
        {
            var rectTransform = canvas.transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(100, 50);
            float scale = CountersController.settings.hudConfig.HUDPositionScaleFactor;

            tmp_text = BeatSaberUI.CreateText(rectTransform, "", anchoredPosition * scale);
            tmp_text.alignment = TextAlignmentOptions.Center;
            tmp_text.fontSize = 4f;
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
            tmp_text.enableWordWrapping = false;
            tmp_text.overflowMode = TextOverflowModes.Overflow;
        }
    }
}
