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
        private static Canvas internalCounterCanvas;
        public static Canvas CounterCanvas
        {
            get
            {
                if (internalCounterCanvas is null)
                {
                    bool useFloatingHUD = CountersController.settings.hudConfig.AttachHUDToCamera;
                    float scale = CountersController.settings.hudConfig.HUDSize;
                    internalCounterCanvas = CreateCanvas(CountersController.settings.hudConfig.HUDPosition, useFloatingHUD, scale);
                }
                return internalCounterCanvas;
            }
            set
            {
                internalCounterCanvas = value;
            }
        }

        public static float PosScaleFactor => CountersController.settings.hudConfig.HUDPositionScaleFactor;
        public static float SizeScaleFactor => CountersController.settings.hudConfig.HUDSize;

        public static Canvas CreateCanvas(Vector3 Position, bool floatingHUD = false, float CanvasScaleFactor = 10)
        {
            Canvas canvas;
            GameObject CanvasGO = new GameObject("Counters+ | Counters Canvas");
            canvas = CanvasGO.AddComponent<Canvas>();
            System.DateTime date = System.DateTime.Now;
            if (date.Month == 4 && date.Day == 1 && CountersController.settings.AprilFoolsTomfoolery)
            {
                CanvasGO.AddComponent<OopsAllAprilFools>();
            }
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasGO.transform.localScale = Vector3.one / CanvasScaleFactor;
            CanvasGO.transform.position = Position;
            CanvasGO.transform.rotation = Quaternion.Euler(CountersController.settings.hudConfig.HUDRotation);

            GameObject coreGameHUD = Resources.FindObjectsOfTypeAll<CoreGameHUDController>()?.FirstOrDefault(x => x.isActiveAndEnabled)?.gameObject ?? null;
            FlyingGameHUDRotation flyingGameHUD = Resources.FindObjectsOfTypeAll<FlyingGameHUDRotation>().FirstOrDefault(x => x.isActiveAndEnabled);
            bool attachToHUD = flyingGameHUD != null && CountersController.settings.hudConfig.AttachToBaseGameHUDFor360;

            //We inherit canvas properties from the Energy Bar as to exclude the Counters+ HUD from the debris distortion effects
            //(See https://github.com/Caeden117/CountersPlus/issues/51)
            if (coreGameHUD != null)
            {
                Canvas energyCanvas = coreGameHUD.GetComponentInChildren<GameEnergyUIPanel>(true).GetComponent<Canvas>();
                canvas.overrideSorting = energyCanvas.overrideSorting;
                canvas.sortingLayerID = energyCanvas.sortingLayerID;
                canvas.sortingLayerName = energyCanvas.sortingLayerName;
                canvas.sortingOrder = energyCanvas.sortingOrder;
                canvas.gameObject.layer = energyCanvas.gameObject.layer;
            }

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
            CreateText(out tmp_text, CounterCanvas, anchoredPosition);
        }

        public static void CreateText(out TMP_Text tmp_text, Canvas canvas, Vector3 anchoredPosition)
        {
            var rectTransform = canvas.transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(100, 50);

            tmp_text = BeatSaberUI.CreateText(rectTransform, "", anchoredPosition * PosScaleFactor);
            tmp_text.alignment = TextAlignmentOptions.Center;
            tmp_text.fontSize = 4f;
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
            tmp_text.enableWordWrapping = false;
            tmp_text.overflowMode = TextOverflowModes.Overflow;
        }
    }
}
