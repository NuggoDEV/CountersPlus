using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    public class SpinometerConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Spinometer";

        public override bool Enabled { get; set; } = false;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.AboveMultiplier;
        public override int Distance { get; set; } = 0;

        [UseConverter]
        public virtual SpinometerMode Mode { get; set; } = SpinometerMode.Highest;
    }

    public enum SpinometerMode { Average, SplitAverage, Highest }
}
