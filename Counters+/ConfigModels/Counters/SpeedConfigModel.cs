using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    public class SpeedConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Speed";

        public override bool Enabled { get; set; } = false;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowMultiplier;
        public override int Distance { get; set; } = 2;

        public virtual int DecimalPrecision { get; set; } = 2;
        [UseConverter]
        public virtual SpeedMode Mode { get; set; } = SpeedMode.Average;
    }

    public enum SpeedMode { Average, Top5Sec, Both, SplitAverage, SplitBoth }
}
