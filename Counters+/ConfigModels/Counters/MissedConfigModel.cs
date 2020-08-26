using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    internal class MissedConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Missed";

        public override bool Enabled { get; set; } = true;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowCombo;
        public override int Distance { get; set; } = 0;

        [UIValue(nameof(CountBadCuts))]
        public virtual bool CountBadCuts { get; set; } = true;
    }
}
