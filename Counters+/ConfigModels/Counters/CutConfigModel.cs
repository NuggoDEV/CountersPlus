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

        public virtual bool SeparateSaberCounts { get; set; } = false;
        public virtual bool SeparateCutValues { get; set; } = false;
        public virtual int AveragePrecision { get; set; } = 1;
    }
}
