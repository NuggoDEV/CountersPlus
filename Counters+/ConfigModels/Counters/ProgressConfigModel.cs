using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    public class ProgressConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Progress";

        public override bool Enabled { get; set; } = true;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowEnergy;
        public override int Distance { get; set; } = 0;

        [UseConverter]
        public virtual ProgressMode Mode { get; set; } = ProgressMode.Original;
        public virtual bool ProgressTimeLeft { get; set; } = false;
        public virtual bool IncludeRing { get; set; } = false;
    }

    public enum ProgressMode { Original, BaseGame, TimeInBeats, Percent }
}
