using CountersPlus.ConfigModels.Converters;
using IPA.Config.Stores.Attributes;
using UnityEngine;

namespace CountersPlus.ConfigModels
{
    public class ScoreConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Score";

        public override bool Enabled { get; set; } = true;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowMultiplier;
        public override int Distance { get; set; } = 0;

        [UseConverter]
        public virtual ScoreMode Mode { get; set; } = ScoreMode.Original;
        public virtual int DecimalPrecision { get; set; } = 2;
        public virtual bool DisplayRank { get; set; } = true;
        public virtual bool CustomRankColors { get; set; } = true;
        [UseConverter(typeof(ColorConverter))]
        public virtual Color SSColor { get; set; } = Color.cyan;
        [UseConverter(typeof(ColorConverter))]
        public virtual Color SColor { get; set; } = Color.white;
        [UseConverter(typeof(ColorConverter))]
        public virtual Color AColor { get; set; } = Color.green;
        [UseConverter(typeof(ColorConverter))]
        public virtual Color BColor { get; set; } = Color.yellow;
        [UseConverter(typeof(ColorConverter))]
        public virtual Color CColor { get; set; } = new Color(1, 0.5f, 0);
        [UseConverter(typeof(ColorConverter))]
        public virtual Color DColor { get; set; } = Color.red;
        [UseConverter(typeof(ColorConverter))]
        public virtual Color EColor { get; set; } = Color.red;

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

    public enum ScoreMode { Original, ScoreOnly, LeavePoints }
}
