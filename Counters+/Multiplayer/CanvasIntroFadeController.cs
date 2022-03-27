using CountersPlus.ConfigModels;
using CountersPlus.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CountersPlus.Multiplayer
{
    internal class CanvasIntroFadeController : IInitializable, ITickable, IDisposable
    {
        [Inject] private CoreGameHUDController coreGameHUDController;
        [Inject] private HUDConfigModel hudConfig;
        [Inject] private CanvasUtility canvasUtility;
        
        [Inject] private MultiplayerController multiplayerController;

        private CanvasGroup coreGameHUDCanvasGroup;
        private List<CanvasGroup> countersPlusCanvasGroups = new List<CanvasGroup>();
        
        private bool tick = false;

        public void Initialize()
        {
            coreGameHUDCanvasGroup = coreGameHUDController.GetComponent<CanvasGroup>();

            for (int i = -1; i < hudConfig.OtherCanvasSettings.Count; i++)
            {
                var canvas = canvasUtility.GetCanvasFromID(i).gameObject;

                countersPlusCanvasGroups.Add(canvas.AddComponent<CanvasGroup>());
            }

            multiplayerController.stateChangedEvent += MultiplayerController_stateChangedEvent;
        }

        // Short circuit which stops Tick() from executing when in gameplay
        private void MultiplayerController_stateChangedEvent(MultiplayerController.State state)
            => tick = state != MultiplayerController.State.Gameplay;

        public void Tick()
        {
            if (!tick) return;

            var alpha = coreGameHUDCanvasGroup.alpha;

            countersPlusCanvasGroups.ForEach(canvas => canvas.alpha = alpha);
        }

        public void Dispose()
            => multiplayerController.stateChangedEvent -= MultiplayerController_stateChangedEvent;
    }
}
