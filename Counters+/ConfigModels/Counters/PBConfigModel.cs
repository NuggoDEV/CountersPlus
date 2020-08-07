using IPA.Config.Stores.Attributes;

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

        public virtual int DecimalPrecision { get; set; } = 2;
        public virtual int TextSize { get; set; } = 2;
        public virtual bool UnderScore { get; set; } = true;
        public virtual bool HideFirstScore { get; set; } = false;
    }
}
