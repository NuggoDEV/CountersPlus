using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    public class NoteConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Notes";

        public override bool Enabled { get; set; } = false;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowCombo;
        public override int Distance { get; set; } = 1;

        public virtual bool ShowPercentage { get; set; } = false;
        public virtual int DecimalPrecision { get; set; } = 2;
    }
}
