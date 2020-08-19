using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CountersPlus.ConfigModels
{
    /// <summary>
    /// The base config class for every single Counter in Counters+.
    /// As part of creating a new Counter, you will need to make a class that inherits ConfigModel.
    /// For adding new options to an existing Counter, add them to their respective ConfigModel.
    /// Add defaults to ConfigModels in the ConfigDefaults class.
    /// </summary>
    public abstract class ConfigModel
    {
        [Ignore]
        public virtual string DisplayName { get; set; } = "Unknown";

        [UIValue(nameof(Enabled))]
        public virtual bool Enabled { get; set; } = false;
        [UseConverter]
        [UIValue(nameof(Position))]
        public virtual CounterPositions Position { get; set; } = CounterPositions.BelowCombo;
        [UIValue(nameof(Distance))]
        public virtual int Distance { get; set; } = 2;

        // Default Canvas will be the main Counters+ one
        // Oh yeah baby, we're gonna support multiple canvases
        [UIValue(nameof(CanvasID))]
        public virtual int CanvasID { get; set; } = -1;

        [UIValue(nameof(Distances))]
        public List<object> Distances = new List<object> { -1, 0, 1, 2, 3, 4 };

        [UIValue(nameof(Positions))]
        public List<object> Positions => PositionToNames.Keys.Cast<object>().ToList();

        [UIAction(nameof(PositionsFormat))]
        public string PositionsFormat(CounterPositions pos) => PositionToNames[pos];

        public static Dictionary<CounterPositions, string> PositionToNames = new Dictionary<CounterPositions, string>() {
            {CounterPositions.BelowCombo, "Below Combo" },
            {CounterPositions.AboveCombo, "Above Combo" },
            {CounterPositions.BelowMultiplier, "Below Multiplier" },
            {CounterPositions.AboveMultiplier, "Above Multiplier" },
            {CounterPositions.BelowEnergy, "Below Energy" },
            {CounterPositions.AboveHighway, "Over Highway" }
        };
    }
}
