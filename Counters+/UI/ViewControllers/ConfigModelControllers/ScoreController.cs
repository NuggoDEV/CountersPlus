using BeatSaberMarkupLanguage.Attributes;
using CountersPlus.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace CountersPlus.UI.ViewControllers.ConfigModelControllers
{
    class ScoreController : MonoBehaviour
    {
        public ConfigModelController parentController;

        [UIValue("display_rank")]
        public bool DisplayRank
        {
            get => (parentController?.ConfigModel as ScoreConfigModel).DisplayRank;
            set => (parentController?.ConfigModel as ScoreConfigModel).DisplayRank = value;
        }

        [UIValue("mode")]
        public ICounterMode Mode
        {
            get => (parentController?.ConfigModel as ScoreConfigModel).Mode;
            set => (parentController?.ConfigModel as ScoreConfigModel).Mode = value;
        }

        [UIValue("mode_values")]
        public List<object> ModeValues => AdvancedCounterSettings.ScoreSettings.Keys.Cast<object>().ToList();

        [UIAction("mode_formatter")]
        public string Format(ICounterMode mode) => AdvancedCounterSettings.ScoreSettings[mode];

        [UIValue("precision")]
        public int PercentagePrecision
        {
            get => (parentController?.ConfigModel as ScoreConfigModel).DecimalPrecision;
            set => (parentController?.ConfigModel as ScoreConfigModel).DecimalPrecision = value;
        }

        [UIValue("ss-color")]
        public Color SSColor
        {
            get => GetColorFromHTML((parentController?.ConfigModel as ScoreConfigModel).SSColor);
            set => (parentController?.ConfigModel as ScoreConfigModel).SSColor = ConvertToHTML(value);
        }

        [UIValue("s-color")]
        public Color SColor
        {
            get => GetColorFromHTML((parentController?.ConfigModel as ScoreConfigModel).SColor);
            set => (parentController?.ConfigModel as ScoreConfigModel).SColor = ConvertToHTML(value);
        }

        [UIValue("a-color")]
        public Color AColor
        {
            get => GetColorFromHTML((parentController?.ConfigModel as ScoreConfigModel).AColor);
            set => (parentController?.ConfigModel as ScoreConfigModel).AColor = ConvertToHTML(value);
        }

        [UIValue("b-color")]
        public Color BColor
        {
            get => GetColorFromHTML((parentController?.ConfigModel as ScoreConfigModel).BColor);
            set => (parentController?.ConfigModel as ScoreConfigModel).BColor = ConvertToHTML(value);
        }

        [UIValue("c-color")]
        public Color CColor
        {
            get => GetColorFromHTML((parentController?.ConfigModel as ScoreConfigModel).CColor);
            set => (parentController?.ConfigModel as ScoreConfigModel).CColor = ConvertToHTML(value);
        }

        [UIValue("d-color")]
        public Color DColor
        {
            get => GetColorFromHTML((parentController?.ConfigModel as ScoreConfigModel).DColor);
            set => (parentController?.ConfigModel as ScoreConfigModel).DColor = ConvertToHTML(value);
        }

        [UIValue("e-color")]
        public Color EColor
        {
            get => GetColorFromHTML((parentController?.ConfigModel as ScoreConfigModel).EColor);
            set => (parentController?.ConfigModel as ScoreConfigModel).EColor = ConvertToHTML(value);
        }

        [UIValue("precision_values")]
        public List<object> PrecisionValues => AdvancedCounterSettings.PercentagePrecision.Cast<object>().ToList();

        [UIValue("rank_colors")]
        public bool RankColors
        {
            get => (parentController?.ConfigModel as ScoreConfigModel).CustomRankColors;
            set => (parentController?.ConfigModel as ScoreConfigModel).CustomRankColors = value;
        }

        [UIAction("update_model")]
        internal void ConfigChanged(object obj)
        {
            parentController.ConfigChanged(obj);
        }

        private Color GetColorFromHTML(string html)
        {
            ColorUtility.TryParseHtmlString(html, out Color value);
            return value;
        }

        private string ConvertToHTML(Color color) => $"#{ColorUtility.ToHtmlStringRGB(color)}";
    }
}
