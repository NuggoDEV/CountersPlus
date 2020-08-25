using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace CountersPlus.ConfigModels
{
    internal class SpeedConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Speed";

        public override bool Enabled { get; set; } = false;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowMultiplier;
        public override int Distance { get; set; } = 2;

        [UIValue(nameof(DecimalPrecision))]
        public virtual int DecimalPrecision { get; set; } = 2;
        [UseConverter]
        [UIValue(nameof(Mode))]
        public virtual SpeedMode Mode { get; set; } = SpeedMode.Average;

        [UIValue(nameof(Modes))]
        public List<object> Modes => ModeToNames.Keys.Cast<object>().ToList();

        [UIAction(nameof(ModeFormat))]
        public string ModeFormat(SpeedMode pos) => ModeToNames[pos];

        private static Dictionary<SpeedMode, string> ModeToNames = new Dictionary<SpeedMode, string>()
        {
            { SpeedMode.Average, "Average" },
            { SpeedMode.Top5Sec, "Top from 5 Seconds" },
            { SpeedMode.Both, "Both Metrics" },
            { SpeedMode.SplitAverage, "Split Average" },
            { SpeedMode.SplitBoth, "Split Both Metrics" },
        };
    }

    public enum SpeedMode { Average, Top5Sec, Both, SplitAverage, SplitBoth }
}
