using System;

namespace CountersPlus.ConfigModels
{
    /// <summary>
    /// Main class for Counters+ config.
    /// For adding new Counters, add their ConfigModels as a field in this class, making sure that "Config" is in the name.
    /// </summary>
    public class MainConfigModel
    {
        public string DisplayName => "Main";
        public virtual bool Enabled { get; set; } = true;
        public virtual bool HideCombo { get; set; } = false;
        public virtual bool HideMultiplier { get; set; } = false;
        public virtual float ComboOffset { get; set; } = 0.2f;
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

        public event Action OnConfigChanged;

        public virtual void Changed()
        {
            OnConfigChanged?.Invoke();
        }
    }

    public enum CounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }
}
