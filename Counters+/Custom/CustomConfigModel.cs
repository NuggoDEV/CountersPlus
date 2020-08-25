using CountersPlus.ConfigModels;
using IPA.Config.Stores.Attributes;
using Newtonsoft.Json;

namespace CountersPlus.Custom
{
    public class CustomConfigModel : ConfigModel
    {
        [JsonProperty(nameof(Enabled), Required = Required.DisallowNull)]
        public override bool Enabled { get; set; } = true;
        [UseConverter]
        [JsonProperty(nameof(Position), Required = Required.DisallowNull)]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowCombo;
        [JsonProperty(nameof(Distance), Required = Required.DisallowNull)]
        public override int Distance { get; set; } = 0;

        [Ignore]
        internal CustomCounter AttachedCustomCounter;
    }
}
