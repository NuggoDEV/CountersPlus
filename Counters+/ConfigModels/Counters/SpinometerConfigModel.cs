using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;
using System.Collections.Generic;
using System.Linq;

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

        [UIValue(nameof(Modes))]
        public List<object> Modes => ModeToNames.Keys.Cast<object>().ToList();

        [UIAction(nameof(ModeFormat))]
        public string ModeFormat(SpinometerMode pos) => ModeToNames[pos];

        private static Dictionary<SpinometerMode, string> ModeToNames = new Dictionary<SpinometerMode, string>()
        {
            { SpinometerMode.Average, "Average" },
            { SpinometerMode.SplitAverage, "Split Average" },
            { SpinometerMode.Highest, "Highest" },
        };
    }

    public enum SpinometerMode { Average, SplitAverage, Highest }
}
