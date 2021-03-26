using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Linq;
using System.Collections.Generic;
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
        [UseConverter(typeof(HexColorConverter))]
        [UIValue(nameof(SSColor))]
        public virtual Color SSColor { get; set; } = Color.cyan;
        [UseConverter(typeof(HexColorConverter))]
        [UIValue(nameof(SColor))]
        public virtual Color SColor { get; set; } = Color.white;
        [UseConverter(typeof(HexColorConverter))]
        [UIValue(nameof(AColor))]
        public virtual Color AColor { get; set; } = Color.green;
        [UseConverter(typeof(HexColorConverter))]
        [UIValue(nameof(BColor))]
        public virtual Color BColor { get; set; } = Color.yellow;
        [UseConverter(typeof(HexColorConverter))]
        [UIValue(nameof(CColor))]
        public virtual Color CColor { get; set; } = new Color(1, 0.5f, 0);
        [UseConverter(typeof(HexColorConverter))]
        [UIValue(nameof(DColor))]
        public virtual Color DColor { get; set; } = Color.red;
        [UseConverter(typeof(HexColorConverter))]
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

        public Color GetRankColorFromRank(RankModel.Rank rank) => rank switch
            {
                RankModel.Rank.S => SColor,
                RankModel.Rank.A => AColor,
                RankModel.Rank.B => BColor,
                RankModel.Rank.C => CColor,
                RankModel.Rank.D => DColor,
                RankModel.Rank.E => EColor,
                _ => SSColor,
            };
    }

    public enum ScoreMode { Original, ScoreOnly, LeavePoints, RankOnly }
}
