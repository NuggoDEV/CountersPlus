using BeatSaberMarkupLanguage;
using CountersPlus.ConfigModels;
using CountersPlus.UI.ViewControllers;
using CountersPlus.Utils;
using HMUI;
using SiraUtil.Zenject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CountersPlus.UI.FlowCoordinators
{
    public class CountersPlusSettingsFlowCoordinator : FlowCoordinator
    {
        [Inject] private List<ConfigModel> allConfigModels;
        [Inject] private DiContainer menuDiContainer;
        [Inject] private CanvasUtility canvasUtility;

        private TweenPosition mainScreenTween;
        private Vector3 mainScreenOrigin;

        private CountersPlusCreditsViewController credits;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                Transform mainScreenTransform = GameObject.Find("MainScreen").transform;
                mainScreenTween = mainScreenTransform.gameObject.AddComponent<TweenPosition>();
                mainScreenTween._duration = 1f;
                mainScreenOrigin = mainScreenTransform.position;

                Plugin.Logger.Warn($"Loading options menu with {allConfigModels.Count} config models...");

                showBackButton = true;
                title = "Counters+";

                credits = BindViewController<CountersPlusCreditsViewController>();
            }
            SetMainScreenOffset(new Vector3(0, -4, 0));
            ProvideInitialViewControllers(credits);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            SetMainScreenOffset(Vector3.zero);
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
            canvasUtility.ClearAllText();
        }

        internal void SetMainScreenOffset(Vector3 offset, float duration = 1)
        {
            mainScreenTween._duration = duration;
            mainScreenTween.TargetPos = mainScreenOrigin + offset;
        }

        private T BindViewController<T>() where T : ViewController
        {
            T view = BeatSaberUI.CreateViewController<T>();
            menuDiContainer.InjectSpecialInstance<T>(view);
            return view;
        }
    }
}
