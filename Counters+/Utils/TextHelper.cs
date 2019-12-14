using UnityEngine;
using TMPro;
using BeatSaberMarkupLanguage;
using CountersPlus.Utils;

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

            GameObject coreGameHUD = GameObject.Find("CoreGameHUD"); //Attach base game HUD to Counters+. Why? Why not.
            if (coreGameHUD != null) coreGameHUD.transform.SetParent(CanvasGO.transform, true);

            if (floatingHUD)
            {
                if (coreGameHUD != null)
                {
                    coreGameHUD.transform.localScale = Vector3.one * CanvasScaleFactor;
                    coreGameHUD.transform.localPosition = Vector3.back * 70;
                }

                if (coreGameHUD != null)
                {
                    foreach (Transform children in coreGameHUD.transform)
                    {
                        if (children.Find("BG")) children.Find("BG").gameObject.SetActive(false);
                        if (children.Find("Top")) children.Find("Top").gameObject.SetActive(false);
                    }
                }
                CanvasGO.AddComponent<AssignedFloatingWindow>();
            }
            
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
