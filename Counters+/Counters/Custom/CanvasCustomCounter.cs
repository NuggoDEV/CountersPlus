using CountersPlus.ConfigModels;
using System.Collections;
using UnityEngine;

namespace CountersPlus.Counters.Custom
{
    /// <summary>
    /// A custom counter that re-parents an existing <see cref="Canvas"/> into the Custom Counter system.
    /// </summary>
    public class CanvasCustomCounter : BasicCustomCounter
    {
        /// <summary>
        /// The name of the Canvas that this Custom Counter will look for.
        /// Use this if Counters+ does not immediately find the Canvas on startup.
        /// </summary>
        public virtual string CanvasObjectName => null;

        /// <summary>
        /// A direct reference to the Canvas object. You should try using this first.
        /// </summary>
        public virtual Canvas CanvasReference => null;

        public override void CounterInit()
        {
            PreReparent();
            if (CanvasReference != null)
            {
                ReparentCanvas(CanvasReference);
            }
            else if (!string.IsNullOrEmpty(CanvasObjectName))
            {
                SharedCoroutineStarter.instance.StartCoroutine(FindCanvasByString());
            }
            else
            {
                Plugin.Logger.Warn($"Custom Counter ({Settings.AttachedCustomCounter.Name}) has no method of obtaining a Canvas to reparent.");
            }
        }

        /// <summary>
        /// Called before Counters+ attempts to reparent the defined <see cref="Canvas"/>.
        /// </summary>
        public virtual void PreReparent() { }

        /// <summary>
        /// Called after Counters+ has successfully reparented the defined <see cref="Canvas"/>.
        /// </summary>
        public virtual void PostReparent() { }

        private void ReparentCanvas(Canvas canvas)
        {
            Canvas currentCanvas = CanvasUtility.GetCanvasFromID(Settings.CanvasID);
            HUDCanvas currentSettings = CanvasUtility.GetCanvasSettingsFromCanvas(currentCanvas);
            float positionScale = currentSettings?.PositionScale ?? 10;
            Vector2 anchoredPos = CanvasUtility.GetAnchoredPositionFromConfig(Settings) * positionScale;

            canvas.transform.SetParent(currentCanvas.transform, true);

            RectTransform canvasRect = canvas.transform as RectTransform;
            canvasRect.anchoredPosition = anchoredPos;
            canvasRect.localPosition = new Vector3(canvasRect.localPosition.x, canvasRect.localPosition.y, 0);
            canvasRect.localEulerAngles = Vector3.zero;

            PostReparent();
        }

        private IEnumerator FindCanvasByString()
        {
            int tries = 1;
            Canvas canvas = null;
            while (tries <= 10)
            {
                yield return new WaitForSeconds(tries * 0.1f);
                canvas = GameObject.Find(CanvasObjectName)?.GetComponent<Canvas>();
                if (canvas != null) break;
                tries++;
            }
            if (canvas != null) ReparentCanvas(canvas);
            else Plugin.Logger.Warn($"Custom Counter ({Settings.AttachedCustomCounter.Name}) could not find its Canvas in 10 tries.");
        }

        public override void CounterDestroy() { }
    }
}
