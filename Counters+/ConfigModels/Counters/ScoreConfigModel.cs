using BeatSaberMarkupLanguage.Attributes;
using CountersPlus.ConfigModels.Converters;
using IPA.Config.Stores.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CountersPlus.ConfigModels
{
    internal class ScoreConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Score";

        public override bool Enabled { get; set; } = true;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowMultiplier;
        public override int Distance { get; set; } = 0;

        [UseConverter]
        [UIValue(nameof(Mode))]
        public virtual ScoreMode Mode { get; set; } = ScoreMode.Original;
        [UIValue(nameof(DecimalPrecision))]
        public virtual int DecimalPrecision { get; set; } = 2;
        [UIValue(nameof(DisplayRank))]
        public virtual bool DisplayRank { get; set; } = true;
        [UIValue(nameof(CustomRankColors))]
        public virtual bool CustomRankColors { get; set; } = true;
        [UseConverter(typeof(ColorConverter))]
        [UIValue(nameof(SSColor))]
        public virtual Color SSColor { get; set; } = Color.cyan;
        [UseConverter(typeof(ColorConverter))]
        [UIValue(nameof(SColor))]
        public virtual Color SColor { get; set; } = Color.white;
        [UseConverter(typeof(ColorConverter))]
        [UIValue(nameof(AColor))]
        public virtual Color AColor { get; set; } = Color.green;
        [UseConverter(typeof(ColorConverter))]
        [UIValue(nameof(BColor))]
        public virtual Color BColor { get; set; } = Color.yellow;
        [UseConverter(typeof(ColorConverter))]
        [UIValue(nameof(CColor))]
        public virtual Color CColor { get; set; } = new Color(1, 0.5f, 0);
        [UseConverter(typeof(ColorConverter))]
        [UIValue(nameof(DColor))]
        public virtual Color DColor { get; set; } = Color.red;
        [UseConverter(typeof(ColorConverter))]
        [UIValue(nameof(EColor))]
        public virtual Color EColor { get; set; } = Color.red;

        [UIValue(nameof(Modes))]
        public List<object> Modes => ModeToNames.Keys.Cast<object>().ToList();

        [UIAction(nameof(ModeFormat))]
        public string ModeFormat(ScoreMode pos) => ModeToNames[pos];

        private static Dictionary<ScoreMode, string> ModeToNames = new Dictionary<ScoreMode, string>()
        {
            { ScoreMode.Original, "Original" },
            { ScoreMode.LeavePoints, "Dont Move Points" },
            { ScoreMode.ScoreOnly, "Remove Rank" },
            { ScoreMode.RankOnly, "Remove Percentage" },
        };

        public Color GetRankColorFromRank(RankModel.Rank rank)
        {
            switch (rank)
            {
                case RankModel.Rank.S: return SColor;
                case RankModel.Rank.A: return AColor;
                case RankModel.Rank.B: return BColor;
                case RankModel.Rank.C: return CColor;
                case RankModel.Rank.D: return DColor;
                case RankModel.Rank.E: return EColor;
                default: return SSColor;
            }
        }
    }

    public enum ScoreMode { Original, ScoreOnly, LeavePoints, RankOnly }
}
