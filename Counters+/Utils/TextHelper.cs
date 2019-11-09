using UnityEngine;
using TMPro;

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
        internal static readonly float ScaleFactor = 10;

        public static Canvas CreateCanvas(Vector3 Position, bool floatingHUD = false, float CanvasScaleFactor = 10)
        {
            Canvas canvas;
            GameObject CanvasGO = new GameObject("Counters+ | Counters Canvas");
            canvas = CanvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasGO.transform.localScale = Vector3.one / CanvasScaleFactor;
            CanvasGO.transform.position = Position;

            if (floatingHUD)
            {
                GameObject coreGameHUD = GameObject.Find("CoreGameHUD");
                if (coreGameHUD != null)
                {
                    coreGameHUD.transform.SetParent(CanvasGO.transform, true);
                    foreach(Transform children in coreGameHUD.transform)
                    {
                        if (children.Find("BG")) children.Find("BG").gameObject.SetActive(false);
                        if (children.Find("Top")) children.Find("Top").gameObject.SetActive(false);
                    }
                    coreGameHUD.transform.localScale = Vector3.one * ScaleFactor;
                    coreGameHUD.transform.localPosition = Vector3.back * 70;
                }
                CanvasGO.AddComponent<FloatingOverlayWindow>();
                CanvasGO.AddComponent<Utils.ResetCameraOnDestroy>();
            }
            
            return canvas;
        }

        public static void CreateText(out TMP_Text tmp_text, Vector3 anchoredPosition)
        {
            float scaleFactor = CountersController.settings.FloatingHUD ? 50 : ScaleFactor;
            if (CounterCanvas == null)
                CounterCanvas = CreateCanvas(Vector3.forward * 7, CountersController.settings.FloatingHUD, scaleFactor);
            CreateText(out tmp_text, CounterCanvas, anchoredPosition);
        }

        public static void CreateText(out TMP_Text tmp_text, Canvas canvas, Vector3 anchoredPosition)
        {
            var rectTransform = canvas.transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(100, 50);

            tmp_text = CustomUI.BeatSaber.BeatSaberUI.CreateText(rectTransform, "", anchoredPosition * ScaleFactor);
            tmp_text.alignment = TextAlignmentOptions.Center;
            tmp_text.fontSize = 4f;
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
            tmp_text.enableWordWrapping = false;
            tmp_text.overflowMode = TextOverflowModes.Overflow;
        }
    }
}
