using BeatSaberMarkupLanguage.Attributes;
using CountersPlus.Custom;
using IPA.Config.Data;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System;
using System.Collections.Generic;

namespace CountersPlus.ConfigModels
{
    /// <summary>
    /// Main class for Counters+ config.
    /// For adding new Counters, add their ConfigModels as a field in this class, making sure that "Config" is in the name.
    /// </summary>
    public class MainConfigModel
    {
        public string DisplayName => "Main";
        [UIValue(nameof(Enabled))]
        public virtual bool Enabled { get; set; } = true;
        [UIValue(nameof(HideCombo))]
        public virtual bool HideCombo { get; set; } = false;
        [UIValue(nameof(HideMultiplier))]
        public virtual bool HideMultiplier { get; set; } = false;
        [UIValue(nameof(ComboOffset))]
        public virtual float ComboOffset { get; set; } = 0.2f;
        [UIValue(nameof(MultiplierOffset))]
        public virtual float MultiplierOffset { get; set; } = 0.4f;
        public virtual HUDConfigModel HUDConfig { get; set; } = new HUDConfigModel();
        public virtual MissedConfigModel MissedConfig { get; set; } = new MissedConfigModel();
        public virtual NoteConfigModel NoteConfig { get; set; } = new NoteConfigModel();
        public virtual ProgressConfigModel ProgressConfig { get; set; } = new ProgressConfigModel();
        public virtual ScoreConfigModel ScoreConfig { get; set; } = new ScoreConfigModel();
        public virtual PBConfigModel PBConfig { get; set; } = new PBConfigModel();
        public virtual SpeedConfigModel SpeedConfig { get; set; } = new SpeedConfigModel();
        public virtual CutConfigModel CutConfig { get; set; } = new CutConfigModel();
        public virtual SpinometerConfigModel SpinometerConfig { get; set; } = new SpinometerConfigModel();
        public virtual NotesLeftConfigModel NotesLeftConfig { get; set; } = new NotesLeftConfigModel();
        public virtual FailConfigModel FailsConfig { get; set; } = new FailConfigModel();

        [UseConverter]
        public virtual Dictionary<string, CustomConfigModel> CustomCounters { get; set; } = new Dictionary<string, CustomConfigModel>();

        public event Action OnConfigChanged;

        public virtual void Changed()
        {
            OnConfigChanged?.Invoke();
        }

        public List<object> Offsets => new List<object> { 0, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1 };
    }

    public enum CounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }
}
