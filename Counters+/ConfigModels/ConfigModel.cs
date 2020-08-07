using IPA.Config.Stores.Attributes;

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

        public virtual bool Enabled { get; set; } = false;
        [UseConverter]
        public virtual CounterPositions Position { get; set; } = CounterPositions.BelowCombo;
        public virtual int Distance { get; set; } = 2;

        // Default Canvas will be the main Counters+ one
        // Oh yeah baby, we're gonna support multiple canvases
        public virtual int CanvasID { get; set; } = -1;
    }
}
