using Newtonsoft.Json;
using System;

namespace CountersPlus.Custom
{
    // Not to be confused with CountersPlus.Counters.CustomCounter
    internal class CustomCounter
    {
        [JsonProperty(nameof(Name), Required = Required.Always)]
        public string Name;
        [JsonProperty(nameof(Description), Required = Required.DisallowNull)]
        public string Description;
        [JsonProperty(nameof(BSML), Required = Required.DisallowNull)]
        public BSMLSettings BSML = null;
        [JsonProperty(nameof(CounterLocation), Required = Required.Always)]
        internal string CounterLocation;
        [JsonProperty(nameof(ConfigDefaults), Required = Required.DisallowNull)]
        internal CustomConfigModel ConfigDefaults = new CustomConfigModel();

        public CustomConfigModel Config;

        public Type CounterType;

        public class BSMLSettings
        {
            [JsonProperty(nameof(Resource), Required = Required.AllowNull)]
            public string Resource;
            [JsonProperty(nameof(Host), Required = Required.AllowNull)]
            internal string Host;
            [JsonProperty(nameof(Icon), Required = Required.DisallowNull)]
            public string Icon;

            public Type HostType;

            public bool HasType => !string.IsNullOrEmpty(Host);
        }
    }
}
