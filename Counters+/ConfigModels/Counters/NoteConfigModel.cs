using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    internal class NoteConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Notes";

        public override bool Enabled { get; set; } = false;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowCombo;
        public override int Distance { get; set; } = 1;

        [UIValue(nameof(ShowPercentage))]
        public virtual bool ShowPercentage { get; set; } = false;
        [UIValue(nameof(DecimalPrecision))]
        public virtual int DecimalPrecision { get; set; } = 2;
    }
}
