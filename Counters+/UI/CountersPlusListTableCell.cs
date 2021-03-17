using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System.Linq;
using UnityEngine;

namespace CountersPlus.UI
{
    public class CountersPlusListTableCell : CustomListTableData.CustomCellInfo
    {
        public int CellIdx { get; private set; } = 0;

        [UIParams] private BSMLParserParams parserParams;

        public CountersPlusListTableCell(int idx, string text, Sprite icon = null) : base(text, null, icon)
        {
            CellIdx = idx;
        }

        [UIAction("#post-parse")]
        private void Parsed()
        {
            var coverImages = parserParams.GetObjectsWithTag("coverImage").Select(x => x.GetComponent<ImageView>());
            foreach (var image in coverImages) image.sprite = icon;

            var infoTexts = parserParams.GetObjectsWithTag("infoText").Select(x => x.GetComponent<CurvedTextMeshPro>());
            foreach (var infoText in infoTexts) infoText.text = text;
        }
    }
}
