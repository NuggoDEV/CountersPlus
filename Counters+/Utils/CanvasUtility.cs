using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
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
        public GameplayCoreHUDInstaller.HudType HUDType = GameplayCoreHUDInstaller.HudType.Basic;

        private Dictionary<int, Canvas> CanvasIDToCanvas = new Dictionary<int, Canvas>();
        private Dictionary<Canvas, HUDCanvas> CanvasToSettings = new Dictionary<Canvas, HUDCanvas>();
        private Canvas energyCanvas = null;
        private MainConfigModel mainConfig;

        // Using the magical power of Zenject™, we magically find ourselves with an instance of
        // our HUDConfigModel and the CoreGameHUDController.
        public CanvasUtility(HUDConfigModel hudConfig,
            MainConfigModel mainConfig,
            [Inject(Optional = true)] GameplayCoreSceneSetupData data,
            [Inject(Optional = true)] CoreGameHUDController coreGameHUD)
        {
            this.mainConfig = mainConfig;
            if (coreGameHUD != null)
            {
                energyCanvas = EnergyPanelGO(ref coreGameHUD).GetComponent<Canvas>();
            }
            if (data != null)
            {
                HUDType = GetGameplayCoreHUDTypeForEnvironmentSize(data.environmentInfo.environmentSizeData.width);
            }

            CanvasIDToCanvas.Add(-1, CreateCanvasWithConfig(hudConfig.MainCanvasSettings));
            CanvasToSettings.Add(CanvasIDToCanvas[-1], hudConfig.MainCanvasSettings);
            if (coreGameHUD != null && hudConfig.MainCanvasSettings.ParentedToBaseGameHUD)
            {
                Transform parent = coreGameHUD.transform;
                if (HUDType == GameplayCoreHUDInstaller.HudType.Flying) parent = coreGameHUD.transform.GetChild(0);
                SoftParent softParent = CanvasIDToCanvas[-1].gameObject.AddComponent<SoftParent>();
                softParent.AssignParent(parent);
                Vector3 posOofset = Vector3.zero; // yknow what, fuck it, its posOofset now.
                if (HUDType == GameplayCoreHUDInstaller.HudType.Flying) posOofset = parent.up;
                softParent.AssignOffsets(posOofset, Quaternion.identity);
            }
            for (int i = 0; i < hudConfig.OtherCanvasSettings.Count; i++)
            {
                HUDCanvas canvasSettings = hudConfig.OtherCanvasSettings[i];
                RegisterNewCanvas(canvasSettings, i);

                if (coreGameHUD != null && hudConfig.OtherCanvasSettings[i].ParentedToBaseGameHUD)
                {
                    Transform parent = coreGameHUD.transform;
                    if (HUDType == GameplayCoreHUDInstaller.HudType.Flying) parent = coreGameHUD.transform.GetChild(0);
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
            GameObject CanvasGameObject = new GameObject($"Counters+ | {canvasSettings.Name} Canvas");

            Vector3 CanvasPos = canvasSettings.Position;
            Vector3 CanvasRot = canvasSettings.Rotation;
            float CanvasSize = canvasSettings.Size;

            Canvas canvas = CanvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            CanvasGameObject.transform.localScale = Vector3.one / CanvasSize;
            CanvasGameObject.transform.position = CanvasPos;
            CanvasGameObject.transform.rotation = Quaternion.Euler(CanvasRot);

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

            // TODO handle interactions between Base Game and Counters+
            // (Attach Counters+ to Base Game HUD? Move Base Game HUD elements in between?)

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
            return CreateText(canvasToApply, GetAnchoredPositionFromConfig(settings, hudSettings.IsMainCanvas), offset);
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

        public Vector3 GetAnchoredPositionFromConfig(ConfigModel settings, bool isMainCanvas = true)
        {
            float comboOffset = mainConfig.ComboOffset;
            float multOffset = mainConfig.MultiplierOffset;
            CounterPositions position = settings.Position;
            int index = settings.Distance;
            Vector3 pos = new Vector3(); // Base position
            Vector3 offset = new Vector3(0, -0.75f * index, 0); // Offset 

            float X = 3.2f;
            float belowEnergyOffset = -1.5f;
            float aboveHighwayOffset = 0.75f;
            if (isMainCanvas)
            {
                switch (HUDType)
                {
                    case GameplayCoreHUDInstaller.HudType.Narrow:
                        X = 2f;
                        break;
                    case GameplayCoreHUDInstaller.HudType.Flying:
                        X = 1.6f;
                        belowEnergyOffset = -0.25f;
                        aboveHighwayOffset = 0.25f;
                        break;
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

        private GameplayCoreHUDInstaller.HudType GetGameplayCoreHUDTypeForEnvironmentSize(EnvironmentSizeData.Width environmentWidth)
        {
            switch (environmentWidth)
            {
                case EnvironmentSizeData.Width.Narrow:
                    return GameplayCoreHUDInstaller.HudType.Narrow;
                case EnvironmentSizeData.Width.Circle:
                    return GameplayCoreHUDInstaller.HudType.Flying;
            }
            return GameplayCoreHUDInstaller.HudType.Basic;
        }
    }
}
