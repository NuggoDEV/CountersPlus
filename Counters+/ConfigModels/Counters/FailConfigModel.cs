using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    public class FailConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Fail";

        public override bool Enabled { get; set; } = false;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.AboveCombo;
        public override int Distance { get; set; } = 0;

        [UIValue(nameof(ShowRestartsInstead))]
        public virtual bool ShowRestartsInstead { get; set; } = false;
    }
}
