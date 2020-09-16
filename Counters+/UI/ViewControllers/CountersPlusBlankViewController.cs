using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System.Collections;
using UnityEngine;

namespace CountersPlus.UI.ViewControllers
{
    public class CountersPlusBlankViewController : BSMLResourceViewController
    {
        [UIObject("easter-egg")] private GameObject hidden;

        private CanvasGroup group;

        public override string ResourceName => "CountersPlus.UI.BSML.BlankScreen.bsml";

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            if (firstActivation) group = hidden.AddComponent<CanvasGroup>();
            StartCoroutine(WaitThenPopUp());
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
            group.alpha = 0;
        }

        private IEnumerator WaitThenPopUp()
        {
            group.alpha = 0;
            yield return new WaitForSeconds(2f);
            group.alpha = 1;
            (transform as RectTransform).anchoredPosition = Vector2.zero;
        }
    }
}
