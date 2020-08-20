using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;
using System.Collections.Generic;

namespace CountersPlus.ConfigModels
{
    public class PBConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Personal Best";

        public override bool Enabled { get; set; } = true;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowMultiplier;
        public override int Distance { get; set; } = 1;

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
    }
}
