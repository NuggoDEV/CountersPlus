using System;
using System.IO;
using System.Reflection;

namespace CountersPlus.Config
{
    public class ConfigLoader
    {
        public static MainConfigModel LoadSettings()
        {
            if (!File.Exists(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini"))
                File.Create(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            MainConfigModel model = new MainConfigModel();
            model = (MainConfigModel)DeserializeFromConfig(model, model.DisplayName);
            try
            {
                model.missedConfig = DeserializeFromConfig(model.missedConfig, model.missedConfig.DisplayName) as MissedConfigModel;
                model.noteConfig = DeserializeFromConfig(model.noteConfig, model.noteConfig.DisplayName) as NoteConfigModel;
                model.progressConfig = DeserializeFromConfig(model.progressConfig, model.progressConfig.DisplayName) as ProgressConfigModel;
                model.scoreConfig = DeserializeFromConfig(model.scoreConfig, model.scoreConfig.DisplayName) as ScoreConfigModel;
                model.speedConfig = DeserializeFromConfig(model.speedConfig, model.speedConfig.DisplayName) as SpeedConfigModel;
                model.cutConfig = DeserializeFromConfig(model.cutConfig, model.cutConfig.DisplayName) as CutConfigModel;
                model.spinometerConfig = DeserializeFromConfig(model.spinometerConfig, model.spinometerConfig.DisplayName) as SpinometerConfigModel;
                model.pbConfig = DeserializeFromConfig(model.pbConfig, model.pbConfig.DisplayName) as PBConfigModel;
                model.notesLeftConfig = DeserializeFromConfig(model.notesLeftConfig, model.notesLeftConfig.DisplayName) as NotesLeftConfigModel;
                model.failsConfig = DeserializeFromConfig(model.failsConfig, model.failsConfig.DisplayName) as FailConfigModel;
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(NullReferenceException)) Plugin.Log(e.ToString(), Plugin.LogInfo.Error);
            }
            Plugin.Log("Config loaded!", Plugin.LogInfo.Notice);
            return model;
        }

        public static object DeserializeFromConfig(object input, string DisplayName)
        {
            bool resetToDefaults = false;
            Type type = input.GetType();
            MemberInfo[] infos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (MemberInfo info in infos)
            {
                if (info.MemberType != MemberTypes.Field || info.Name.ToLower().Contains("config")) continue;
                FieldInfo finfo = (FieldInfo)info;
                string value = Plugin.config.GetString(DisplayName, info.Name, null);
                if (value == null)
                {
                    object defaults = null;
                    if (type.Name.Contains("Main")) value = finfo.GetValue(ConfigDefaults.MainDefaults).ToString();
                    else if (!type.Name.Contains("Custom")) defaults = finfo.GetValue(ConfigDefaults.Defaults[DisplayName]).ToString();
                    if (value == null) continue;
                    Plugin.Log($"Failed to load variable {info.Name} in {type.Name}. Resetting to defaults...", Plugin.LogInfo.Info);
                    resetToDefaults = true;
                }
                if (finfo.FieldType == typeof(ICounterMode))
                    input.SetPrivateField(info.Name, Enum.Parse(typeof(ICounterMode), value));
                else if (finfo.FieldType == typeof(ICounterPositions))
                    input.SetPrivateField(info.Name, Enum.Parse(typeof(ICounterPositions), value));
                else input.SetPrivateField(info.Name, Convert.ChangeType(value, finfo.FieldType));
                if (finfo.GetValue(input) == null) throw new Exception();
            }
            if (resetToDefaults)
            {
                if (type.Name.Contains("Main")) (input as MainConfigModel).Save();
                else if (!type.Name.Contains("Custom")) (input as ConfigModel).Save();
            }
            return input;
        }
    }
    
    public class MainConfigModel {
        public string DisplayName { get { return "Main"; } }
        public bool Enabled;
        public bool AdvancedCounterInfo;
        public bool FirstStart;
        public bool HideCombo;
        public bool HideMultiplier;
        public float ComboOffset;
        public float MultiplierOffset;
        public MissedConfigModel missedConfig = new MissedConfigModel();
        public NoteConfigModel noteConfig = new NoteConfigModel();
        public ProgressConfigModel progressConfig = new ProgressConfigModel();
        public ScoreConfigModel scoreConfig = new ScoreConfigModel();
        public PBConfigModel pbConfig = new PBConfigModel();
        public SpeedConfigModel speedConfig = new SpeedConfigModel();
        public CutConfigModel cutConfig = new CutConfigModel();
        public SpinometerConfigModel spinometerConfig = new SpinometerConfigModel();
        public NotesLeftConfigModel notesLeftConfig = new NotesLeftConfigModel();
        public FailConfigModel failsConfig = new FailConfigModel();

        public void Save()
        {
            Type type = GetType();
            MemberInfo[] infos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo info in infos)
            {
                if (info.MemberType == MemberTypes.Field)
                {
                    FieldInfo finfo = (FieldInfo)info;
                    if (finfo.Name.ToLower().Contains("config")) continue;
                    Plugin.config.SetString(DisplayName, info.Name, finfo.GetValue(this).ToString());
                }
            }
        }
    }

    public abstract class ConfigModel {
        public string DisplayName { get; internal set; }
        public bool Enabled;
        public ICounterPositions Position;
        public int Index;

        public void Save()
        {
            Type type = GetType();
            MemberInfo[] infos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (MemberInfo info in infos)
            {
                if (info.MemberType != MemberTypes.Field || info.Name.ToLower() == "restrictedpositions") continue;
                FieldInfo finfo = (FieldInfo)info;
                Plugin.config.SetString(DisplayName, info.Name, finfo.GetValue(this).ToString());
            }
        }
    }

    public sealed class MissedConfigModel : ConfigModel {
        public MissedConfigModel() { DisplayName = "Missed"; }
        public bool CustomMissTextIntegration;
    }

    public sealed class NoteConfigModel : ConfigModel {
        public NoteConfigModel() { DisplayName = "Notes"; }
        public bool ShowPercentage;
        public int DecimalPrecision;
    }

    public sealed class ProgressConfigModel : ConfigModel {
        public ProgressConfigModel() { DisplayName = "Progress"; }
        public ICounterMode Mode;
        public bool ProgressTimeLeft;
        public bool IncludeRing;
    }

    public sealed class ScoreConfigModel : ConfigModel
    {
        public ScoreConfigModel() { DisplayName = "Score"; }
        public ICounterMode Mode;
        public int DecimalPrecision;
        public bool DisplayRank;
    }

    public sealed class PBConfigModel : ConfigModel{
        public PBConfigModel() { DisplayName = "Personal Best"; }
        public int DecimalPrecision;
        public int TextSize;
        public bool UnderScore;
        public bool HideFirstScore;
    }

    public sealed class SpeedConfigModel : ConfigModel
    {
        public SpeedConfigModel() { DisplayName = "Speed"; }
        public int DecimalPrecision;
        public ICounterMode Mode;
    }

    public sealed class SpinometerConfigModel : ConfigModel
    {
        public SpinometerConfigModel() { DisplayName = "Spinometer"; }
        public ICounterMode Mode;
    }

    public sealed class CutConfigModel : ConfigModel {
        public CutConfigModel() { DisplayName = "Cut"; }
    }

    public sealed class NotesLeftConfigModel : ConfigModel
    {
        public NotesLeftConfigModel() { DisplayName = "Notes Left"; }
        public bool LabelAboveCount;
    }

    public sealed class FailConfigModel : ConfigModel
    {
        public FailConfigModel() { DisplayName = "Fail"; }
        public bool ShowRestartsInstead;
    }
    
    public enum ICounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }

    public enum ICounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth, //Speed
                              BaseGame, Original, Percent, //Progress
                              ScoreOnly, LeavePoints, BaseWithOutPoints, //Score (As well as BaseGame and Original)
                              Highest, //Spinometer (As well as Original and SplitAverage)
    };
}