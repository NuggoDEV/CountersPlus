using HMUI;
using UnityEngine;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;

namespace CountersPlus.UI
{
    public class CountersPlusListTableCell : CustomListTableData.CustomCellInfo
    {
        public int CellIdx { get; private set; } = 0;

        [UIComponent("selectedImage")] private ImageView selectionImage;
        [UIComponent("coverImage")] private ImageView coverImage;
        [UIComponent("infoText")] private CurvedTextMeshPro infoText;

        public CountersPlusListTableCell(int idx, string text, Sprite icon = null) : base(text, null, icon)
        {
            CellIdx = idx;
        }

        [UIAction("#post-parse")]
        private void Parsed()
        {
            coverImage.sprite = icon;
            infoText.text = text;
        }
    }
}
