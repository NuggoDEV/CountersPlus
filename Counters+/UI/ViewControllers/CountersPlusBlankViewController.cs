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

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (firstActivation) group = hidden.AddComponent<CanvasGroup>();
            StartCoroutine(WaitThenPopUp());
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemEnabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemEnabling);
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
