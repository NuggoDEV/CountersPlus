using CountersPlus.ConfigModels;
using UnityEngine;
using Zenject;

namespace CountersPlus.Counters
{
    internal class ProgressBaseGameCounter : Counter<ProgressConfigModel> // Yeah, yeah, I use a completely separate counter
    {
        private readonly Vector3 offset = new Vector3(0, -0.25f, 0);

        [Inject] private CoreGameHUDController coreGameHUDController;

        public override void CounterInit()
        {
            Canvas canvas = coreGameHUDController.GetComponentInChildren<SongProgressUIController>(true).GetComponent<Canvas>();
            canvas.gameObject.SetActive(true);

            Canvas currentCanvas = CanvasUtility.GetCanvasFromID(Settings.CanvasID);
            HUDCanvas currentSettings = CanvasUtility.GetCanvasSettingsFromCanvas(currentCanvas);
            float positionScale = currentSettings?.PositionScale ?? 10;
            Vector2 anchoredPos = (CanvasUtility.GetAnchoredPositionFromConfig(Settings) + offset) * positionScale;

            canvas.transform.SetParent(currentCanvas.transform, true);

            RectTransform canvasRect = canvas.transform as RectTransform;
            canvasRect.anchoredPosition = anchoredPos;
            canvasRect.localPosition = new Vector3(canvasRect.localPosition.x, canvasRect.localPosition.y, 0);
            canvasRect.localEulerAngles = Vector3.zero;
        }
    }
}
