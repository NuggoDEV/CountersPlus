using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    public class NotesLeftConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Notes Left";

        public override bool Enabled { get; set; } = false;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.AboveHighway;
        public override int Distance { get; set; } = -1;

        public virtual bool LabelAboveCount { get; set; } = false;
    }
}
