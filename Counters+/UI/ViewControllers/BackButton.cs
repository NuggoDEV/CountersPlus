using CustomUI.BeatSaber;
using System;
using UnityEngine.UI;
using VRUI;

namespace CountersPlus.UI.ViewControllers
{
    class BackButton : VRUINavigationController
    {
        public event Action DidFinishEvent;

        private Button _backButton;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                _backButton = BeatSaberUI.CreateBackButton(rectTransform, DidFinishEvent.Invoke);
            }
        }
    }
}
