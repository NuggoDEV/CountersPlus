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
            FlyingGameHUDRotation flyingGameHUD = Resources.FindObjectsOfTypeAll<FlyingGameHUDRotation>().FirstOrDefault(x => x.isActiveAndEnabled);
            bool attachToHUD = flyingGameHUD != null && CountersController.settings.hudConfig.AttachToBaseGameHUDFor360;
            if (CountersController.settings.hudConfig.AttachBaseGameHUD && !attachToHUD && coreGameHUD != null)
            {
                coreGameHUD.transform.SetParent(CanvasGO.transform, true);
                coreGameHUD.transform.localScale = Vector3.one * 10;
                coreGameHUD.transform.localPosition = Vector3.zero;
                coreGameHUD.transform.localRotation = Quaternion.identity;
                foreach (Transform children in coreGameHUD.transform)
                {
                    if (children.Find("BG")) children.Find("BG").gameObject.SetActive(false);
                    if (children.Find("Top")) children.Find("Top").gameObject.SetActive(false);
                    children.localPosition = new Vector3(children.localPosition.x, children.localPosition.y, 0);
                }
                if (flyingGameHUD != null)
                {
                    Object.Destroy(coreGameHUD.GetComponent<FlyingGameHUDRotation>());
                    Transform container = coreGameHUD.transform.GetChild(0);
                    container.localPosition = new Vector3(0, 1.8f, 0);
                    container.localRotation = Quaternion.identity;
                    foreach (Transform children in container)
                    {
                        switch (children.name)
                        {
                            default: break;
                            case "EnergyPanel":
                                children.localPosition = new Vector3(0, -120, 0);
                                break;
                            case "ComboPanel":
                                children.localPosition = new Vector3(-160, -57.5f, 0);
                                break;
                            case "MultiplierCanvas":
                                children.localPosition = new Vector3(160, 0, 0);
                                break;
                        }
                    }
                }
            }else if (attachToHUD)
            {
                CanvasGO.transform.SetParent(coreGameHUD.transform.GetChild(0), false);
                CanvasGO.transform.localPosition = Vector3.down * 70;
                CanvasGO.transform.localRotation = Quaternion.identity;
                CanvasGO.transform.localScale = Vector3.one * 4;
            }

            if (floatingHUD && !attachToHUD) CanvasGO.AddComponent<AssignedFloatingWindow>();
            
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
