using System;
using UnityEngine;
using UnityEngine.UI;
using HMUI;
using System.Linq;
using UnityEngine.Events;

namespace CountersPlus.UI.ViewControllers
{
    class BackButton : NavigationController
    {
        public event Action DidFinishEvent;

        private NoTransitionsButton _backButton;
        private UnityAction _backButtonAction;
        private ScreenBackButtonAnimationController _backButtonController;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                _backButtonAction = new UnityAction(RaiseEventAndRemoveSelf);
                _backButtonController = Resources.FindObjectsOfTypeAll<ScreenBackButtonAnimationController>().FirstOrDefault();
                _backButton = _backButtonController.GetComponentInChildren<NoTransitionsButton>();
            }
            _backButton.onClick.AddListener(_backButtonAction);
            _backButtonController.StartAnimation(ScreenBackButtonAnimationController.AnimationType.FadeIn);
        }

        private void RaiseEventAndRemoveSelf()
        {
            DidFinishEvent?.Invoke();
            _backButtonController.StartAnimation(ScreenBackButtonAnimationController.AnimationType.FadeOut);
            _backButton.onClick.RemoveListener(_backButtonAction);
        }
    }
}
