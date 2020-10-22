using UnityEngine;

namespace CountersPlus.UI
{
    public class CountersPlusListTableCell : AnnotatedBeatmapLevelCollectionTableCell
    {
        protected override void SelectionDidChange(TransitionType transitionType)
        {
            _infoText.gameObject.SetActive(selected || highlighted);
            RefreshColors();
            base.RefreshVisuals();
        }

        protected override void HighlightDidChange(TransitionType transitionType)
        {
            _infoText.gameObject.SetActive(selected || highlighted);
            RefreshColors();
            base.RefreshVisuals();
        }

        private void RefreshColors()
        {
            _coverImage.color = Color.white;
            if (highlighted) _coverImage.color = new Color(0.25f, 0.25f, 0.25f);
            if (selected) _coverImage.color = (Color.yellow / 4f).ColorWithAlpha(1);
        }
    }
}
