namespace CountersPlus.UI
{
    public class CountersPlusListTableCell : AnnotatedBeatmapLevelCollectionTableCell
    {
        public override void RefreshVisuals()
        {
            _infoText.gameObject.SetActive(selected || highlighted);
            base.RefreshVisuals();
        }
    }
}
