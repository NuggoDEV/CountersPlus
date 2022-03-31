using CountersPlus.ConfigModels;
using HMUI;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.Counters
{
    internal class MultiplayerRankCounter : Counter<MultiplayerRankConfigModel>
    {
        private readonly Vector3 offset = new Vector3(0, -0.25f, 0);

        [Inject] private MainConfigModel mainConfig;
        [Inject] private MultiplayerPositionHUDController multiplayerPositionHUDController;

        public override void CounterInit()
        {
            // Reposition/reparent rank counter to destination canvas
            var currentCanvas = CanvasUtility.GetCanvasFromID(Settings.CanvasID);
            var currentSettings = CanvasUtility.GetCanvasSettingsFromCanvas(currentCanvas);
            var positionScale = currentSettings?.PositionScale ?? 10;
            var anchoredPos = (CanvasUtility.GetAnchoredPositionFromConfig(Settings) + offset) * positionScale;

            multiplayerPositionHUDController.transform.SetParent(currentCanvas.transform, true);

            var rect = multiplayerPositionHUDController.transform as RectTransform;
            rect.anchoredPosition = anchoredPos;
            rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0);
            rect.localEulerAngles = Vector3.zero;

            // Iterate existing text and set italic shit
            if (!mainConfig.ItalicText)
            {
                foreach (var childText in multiplayerPositionHUDController.GetComponentsInChildren<CurvedTextMeshPro>(true))
                {
                    childText.fontStyle = FontStyles.Normal;
                }
            }
        }
    }
}
