using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;
using UnityEngine;

namespace CountersPlus.ConfigModels
{
    internal class PBConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Personal Best";

        public override bool Enabled { get; set; } = true;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowMultiplier;
        public override int Distance { get; set; } = 1;

        [UseConverter]
        [UIValue(nameof(Mode))]
        public virtual PBMode Mode { get; set; } = PBMode.Absolute;

        [UseConverter(typeof(HexColorConverter))]
        [UIValue(nameof(BetterColor))]
        public virtual Color BetterColor { get; set; } = Color.red;
        [UseConverter(typeof(HexColorConverter))]
        [UIValue(nameof(DefaultColor))]
        public virtual Color DefaultColor { get; set; } = new Color(1.0f, (float)0xa5 / 255, 0.0f, 1.0f);

        [UIValue(nameof(DecimalPrecision))]
        public virtual int DecimalPrecision { get; set; } = 2;
        [UIValue(nameof(TextSize))]
        public virtual int TextSize { get; set; } = 2;
        [UIValue(nameof(UnderScore))]
        public virtual bool UnderScore { get; set; } = true;
        [UIValue(nameof(HideFirstScore))]
        public virtual bool HideFirstScore { get; set; } = false;

        [UIValue(nameof(TextSizes))]
        public List<object> TextSizes => new List<object>() { 2, 3, 4, 5 };

        [UIValue(nameof(Modes))]
        public List<object> Modes => new List<object>() { PBMode.Absolute, PBMode.Relative };
    }

    public enum PBMode { Absolute, Relative }
}
