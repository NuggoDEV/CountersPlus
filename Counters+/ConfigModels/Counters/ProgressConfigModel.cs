using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;
using System.Linq;
using System.Collections.Generic;

namespace CountersPlus.ConfigModels
{
    internal class ProgressConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Progress";

        public override bool Enabled { get; set; } = true;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowEnergy;
        public override int Distance { get; set; } = 0;

        [UseConverter]
        [UIValue(nameof(Mode))]
        public virtual ProgressMode Mode { get; set; } = ProgressMode.Original;
        [UIValue(nameof(ProgressTimeLeft))]
        public virtual bool ProgressTimeLeft { get; set; } = false;
        [UIValue(nameof(IncludeRing))]
        public virtual bool IncludeRing { get; set; } = false;

        [UIValue(nameof(Modes))]
        public List<object> Modes => ModeToNames.Keys.Cast<object>().ToList();

        [UIAction(nameof(ModeFormat))]
        public string ModeFormat(ProgressMode pos) => ModeToNames[pos];

        private static Dictionary<ProgressMode, string> ModeToNames = new Dictionary<ProgressMode, string>()
        {
            { ProgressMode.Original, "Original" },
            { ProgressMode.BaseGame, "Base Game" },
            { ProgressMode.TimeInBeats, "Time in Beats" },
            { ProgressMode.Percent, "Percentage" },
        };
    }

    public enum ProgressMode { Original, BaseGame, TimeInBeats, Percent }
}
