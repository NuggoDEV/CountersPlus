using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    public class CutConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Cut";

        public override bool Enabled { get; set; } = false;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.AboveHighway;
        public override int Distance { get; set; } = 1;

        [UIValue(nameof(SeparateSaberCounts))]
        public virtual bool SeparateSaberCounts { get; set; } = false;
        [UIValue(nameof(SeparateCutValues))]
        public virtual bool SeparateCutValues { get; set; } = false;
        [UIValue(nameof(AveragePrecision))]
        public virtual int AveragePrecision { get; set; } = 1;
    }
}
