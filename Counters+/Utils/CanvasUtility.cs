using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using HMUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;
using static CountersPlus.Utils.Accessors;

namespace CountersPlus.Utils
{
    public class CanvasUtility
    {
        // public GameplayCoreHUDInstaller.HudType HUDType = GameplayCoreHUDInstaller.HudType.Basic;

        private Dictionary<int, Canvas> CanvasIDToCanvas = new Dictionary<int, Canvas>();
        private Dictionary<Canvas, HUDCanvas> CanvasToSettings = new Dictionary<Canvas, HUDCanvas>();
        private Canvas energyCanvas = null;
        private MainConfigModel mainConfig;

        private float hudWidth = 3.2f;
        private float hudDepth = 7f;

        // Using the magical power of Zenject™, we magically find ourselves with an instance of
        // our HUDConfigModel and the CoreGameHUDController.
        internal CanvasUtility(HUDConfigModel hudConfig,
            MainConfigModel mainConfig,
            [Inject(Optional = true)] GameplayCoreSceneSetupData data,
            [Inject(Optional = true)] CoreGameHUDController coreGameHUD)
        {
            this.mainConfig = mainConfig;
            if (coreGameHUD != null)
            {
                hudWidth = Mathf.Abs(coreGameHUD.transform.Find("LeftPanel").transform.position.x);
                hudDepth = Mathf.Abs(coreGameHUD.transform.Find("LeftPanel").transform.position.z);

                energyCanvas = EnergyPanelGO(ref coreGameHUD).GetComponent<Canvas>();

                // Hide Canvas and Multiplier if needed
                if (mainConfig.HideCombo) HideBaseGameHUDElement<ComboUIController>(coreGameHUD);
                if (mainConfig.HideMultiplier) HideBaseGameHUDElement<ScoreMultiplierUIController>(coreGameHUD);
            }
            if (data != null)
            {
                // HUDType = GetGameplayCoreHUDTypeForEnvironmentSize(data.environmentInfo.environmentType);
            }

            RefreshAllCanvases(hudConfig, data, coreGameHUD);
        }

        public void RefreshAllCanvases(HUDConfigModel hudConfig, GameplayCoreSceneSetupData data = null, CoreGameHUDController coreGameHUD = null)
        {
            CanvasIDToCanvas.Clear();
            CanvasToSettings.Clear();
            CanvasIDToCanvas.Add(-1, CreateCanvasWithConfig(hudConfig.MainCanvasSettings));
            CanvasToSettings.Add(CanvasIDToCanvas[-1], hudConfig.MainCanvasSettings);
            if (coreGameHUD != null && hudConfig.MainCanvasSettings.ParentedToBaseGameHUD)
            {
                Transform parent = coreGameHUD.transform;
                //if (HUDType == GameplayCoreHUDInstaller.HudType.Flying) parent = coreGameHUD.transform.GetChild(0);
                SoftParent softParent = CanvasIDToCanvas[-1].gameObject.AddComponent<SoftParent>();
                softParent.AssignParent(parent);

                // Base Game HUD is rotated backwards, so we have to reflect our vector to match.
                Vector3 position = hudConfig.MainCanvasSettings.Position;

                if (hudConfig.MainCanvasSettings.MatchBaseGameHUDDepth) position.z = hudDepth;

                Vector3 posOofset = Vector3.Reflect(position, Vector3.back); // yknow what, fuck it, its posOofset now.
                Quaternion rotOofset = Quaternion.Euler(Vector3.Reflect(hudConfig.MainCanvasSettings.Rotation, Vector3.back));

                softParent.AssignOffsets(posOofset, rotOofset);
            }
            for (int i = 0; i < hudConfig.OtherCanvasSettings.Count; i++)
            {
                HUDCanvas canvasSettings = hudConfig.OtherCanvasSettings[i];
                RegisterNewCanvas(canvasSettings, i);

                if (coreGameHUD != null && hudConfig.OtherCanvasSettings[i].ParentedToBaseGameHUD)
                {
                    Transform parent = coreGameHUD.transform;
                    //if (HUDType == GameplayCoreHUDInstaller.HudType.Flying) parent = coreGameHUD.transform.GetChild(0);
                    SoftParent softParent = CanvasIDToCanvas[i].gameObject.AddComponent<SoftParent>();
                    softParent.AssignParent(parent);
                }
            }
        }

        public void RegisterNewCanvas(HUDCanvas canvasSettings, int id)
        {
            Canvas canvas = CreateCanvasWithConfig(canvasSettings);
            CanvasIDToCanvas.Add(id, canvas);
            CanvasToSettings.Add(canvas, canvasSettings);
        }

        public void UnregisterCanvas(int id)
        {
            Canvas canvas = CanvasIDToCanvas[id];

            CanvasIDToCanvas.Remove(id);
            CanvasToSettings.Remove(canvas);
            Object.Destroy(canvas.gameObject);
        }

        public Canvas CreateCanvasWithConfig(HUDCanvas canvasSettings)
        {
            GameObject canvasGameObject = new GameObject($"Counters+ | {canvasSettings.Name} Canvas");

            Vector3 canvasPos = canvasSettings.Position;

            if (canvasSettings.MatchBaseGameHUDDepth)
            {
                canvasPos = new Vector3(canvasPos.x, canvasPos.y, hudDepth);
            }

            Vector3 canvasRot = canvasSettings.Rotation;
            float canvasSize = canvasSettings.Size;

            Canvas canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            canvasGameObject.transform.localScale = Vector3.one / canvasSize;
            canvasGameObject.transform.position = canvasPos;
            canvasGameObject.transform.rotation = Quaternion.Euler(canvasRot);

            CurvedCanvasSettings curvedCanvasSettings = canvasGameObject.AddComponent<CurvedCanvasSettings>();
            curvedCanvasSettings.SetRadius(canvasSettings.CurveRadius);

            // Inherit canvas properties from the Energy Bar to ignore the shockwave effect.
            // However, a caveat as that, when viewing through walls, UI elements will not appear.
            if (canvasSettings.IgnoreShockwaveEffect && energyCanvas != null)
            {
                canvas.overrideSorting = energyCanvas.overrideSorting;
                canvas.sortingLayerID = energyCanvas.sortingLayerID;
                canvas.sortingLayerName = energyCanvas.sortingLayerName;
                canvas.sortingOrder = energyCanvas.sortingOrder;
                canvas.gameObject.layer = energyCanvas.gameObject.layer;
            }

            return canvas;
        }

#nullable enable
        public Canvas? GetCanvasFromID(int id)
        {
            if (CanvasIDToCanvas.TryGetValue(id, out Canvas canvas)) return canvas;
            return null;
        }

        public HUDCanvas? GetCanvasSettingsFromID(int id)
        {
            if (id == -1) return mainConfig.HUDConfig.MainCanvasSettings;
            return mainConfig.HUDConfig.OtherCanvasSettings.ElementAtOrDefault(id);
        }

        public HUDCanvas? GetCanvasSettingsFromCanvas(Canvas canvas)
        {
            if (CanvasToSettings.TryGetValue(canvas, out HUDCanvas settings)) return settings;
            return null;

        }
#nullable restore

        public TMP_Text CreateTextFromSettings(ConfigModel settings, Vector3? offset = null)
        {
            Canvas canvasToApply = CanvasIDToCanvas[settings.CanvasID];
            HUDCanvas hudSettings = CanvasToSettings[canvasToApply];
            if (canvasToApply == null)
            {
                hudSettings = GetCanvasSettingsFromID(settings.CanvasID);
                canvasToApply = CreateCanvasWithConfig(hudSettings);
                CanvasIDToCanvas[settings.CanvasID] = canvasToApply;
                CanvasToSettings.Add(canvasToApply, hudSettings);
            }
            return CreateText(canvasToApply, GetAnchoredPositionFromConfig(settings), offset);
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
            
            if (mainConfig.ItalicText)
            {
                tmp_text.fontStyle = FontStyles.Italic;
                if (offset != null)
                {
                    tmp_text.rectTransform.anchoredPosition += new Vector2(Mathf.Abs(offset.Value.x) * -1, 0)
                        * posScaleFactor
                        * 0.18f
                        * tmp_text.fontSize;
                }
            }

            return tmp_text;
        }

        public Vector3 GetAnchoredPositionFromConfig(ConfigModel settings)
        {
            float comboOffset = mainConfig.ComboOffset;
            float multOffset = mainConfig.MultiplierOffset;
            CounterPositions position = settings.Position;
            int index = settings.Distance;
            Vector3 pos = new Vector3(); // Base position
            Vector3 offset = new Vector3(0, -0.75f * index, 0); // Offset 

            float belowEnergyOffset = -1.5f;
            float aboveHighwayOffset = 0.75f;

            float X = 3.2f;

            var canvasSettings = GetCanvasSettingsFromID(settings.CanvasID);

            if (canvasSettings != null)
            {
                if (canvasSettings.ParentedToBaseGameHUD && (canvasSettings.MatchBaseGameHUDDepth || canvasSettings.IsMainCanvas))
                {
                    X = hudWidth;
                }
            }

            switch (position)
            {
                case CounterPositions.BelowCombo:
                    pos = new Vector3(-X, 1.15f - comboOffset, 0);
                    break;
                case CounterPositions.AboveCombo:
                    pos = new Vector3(-X, 2f + comboOffset, 0);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case CounterPositions.BelowMultiplier:
                    pos = new Vector3(X, 1.05f - multOffset, 0);
                    break;
                case CounterPositions.AboveMultiplier:
                    pos = new Vector3(X, 2f + multOffset, 0);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case CounterPositions.BelowEnergy:
                    pos = new Vector3(0, belowEnergyOffset, 0);
                    break;
                case CounterPositions.AboveHighway:
                    pos = new Vector3(0, 2.5f, 0);
                    offset = new Vector3(0, (offset.y * -1) + aboveHighwayOffset, 0);
                    break;
            }
            return pos + offset;
        }
        
        public void ClearAllText()
        {
            foreach (Canvas canvas in CanvasIDToCanvas.Values)
            {
                foreach (Transform child in canvas.transform)
                {
                    Object.Destroy(child.gameObject);
                }
            }
        }

        private void HideBaseGameHUDElement<T>(CoreGameHUDController coreGameHUD) where T : MonoBehaviour
        {
            GameObject gameObject = coreGameHUD.GetComponentInChildren<T>().gameObject;
            if (gameObject != null && gameObject.activeInHierarchy)
                RecurseFunctionOverGameObjectTree(gameObject, (child) => child.SetActive(false));
            gameObject.SetActive(false);
        }

        private void RecurseFunctionOverGameObjectTree(GameObject go, System.Action<GameObject> func)
        {
            foreach (Transform child in go.transform)
            {
                RecurseFunctionOverGameObjectTree(child.gameObject, func);
                func?.Invoke(child.gameObject);
            }
        }
    }
}
