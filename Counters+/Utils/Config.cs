using IniParser;
using IniParser.Model;
using System;
using System.Linq;
using System.Reflection;

namespace CountersPlus.Config
{
    public class ConfigLoader
    {
        public static MainConfigModel LoadSettings()
        {
            MainConfigModel model = new MainConfigModel();
            model = (MainConfigModel)DeserializeFromConfig(model, model.DisplayName);
            try
            {
                MemberInfo[] infos = model.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                model.missedConfig = DeserializeFromConfig(model.missedConfig, model.missedConfig.DisplayName) as MissedConfigModel;
                model.noteConfig = DeserializeFromConfig(model.noteConfig, model.noteConfig.DisplayName) as NoteConfigModel;
                model.progressConfig = DeserializeFromConfig(model.progressConfig, model.progressConfig.DisplayName) as ProgressConfigModel;
                model.scoreConfig = DeserializeFromConfig(model.scoreConfig, model.scoreConfig.DisplayName) as ScoreConfigModel;
                model.speedConfig = DeserializeFromConfig(model.speedConfig, model.speedConfig.DisplayName) as SpeedConfigModel;
                model.cutConfig = DeserializeFromConfig(model.cutConfig, model.cutConfig.DisplayName) as CutConfigModel;
                model.spinometerConfig = DeserializeFromConfig(model.spinometerConfig, model.spinometerConfig.DisplayName) as SpinometerConfigModel;
                model.pbConfig = DeserializeFromConfig(model.pbConfig, model.pbConfig.DisplayName) as PBConfigModel;
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(NullReferenceException)) Plugin.Log(e.ToString(), Plugin.LogInfo.Error);
            }
            Plugin.Log("Config loaded!", Plugin.LogInfo.Notice);
            return model;
        }

        internal static object DeserializeFromConfig(object input, string DisplayName)
        {
            Type type = input.GetType();
            MemberInfo[] infos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (MemberInfo info in infos)
            {
                if (info.MemberType == MemberTypes.Field)
                {
                    FieldInfo finfo = (FieldInfo)info;
                    if (finfo.Name.ToLower().Contains("config")) continue;
                    try
                    {
                        if (finfo.GetValue(input) == null) throw new Exception(); //I guess I can just reset the singular variable to defaults, but eh, why not all of it to make sure the rest are available in config?
                        if (finfo.FieldType == typeof(ICounterMode))
                            input.SetPrivateField(info.Name, Enum.Parse(typeof(ICounterMode), Plugin.config.GetString(DisplayName, info.Name, null)));
                        else if (finfo.FieldType == typeof(ICounterPositions))
                            input.SetPrivateField(info.Name, Enum.Parse(typeof(ICounterPositions), Plugin.config.GetString(DisplayName, info.Name, null)));
                        else input.SetPrivateField(info.Name, Convert.ChangeType(Plugin.config.GetString(DisplayName, info.Name, null), finfo.FieldType));
                    }
                    catch
                    {
                        Plugin.Log($"Failed to load variable {info.Name} in {type.Name}. Resetting to defaults...", Plugin.LogInfo.Warning);
                        if (type.Namespace == "CountersPlus.Config")
                        {
                            if (type.Name.Contains("Main"))
                            {
                                ConfigDefaults.MainDefaults.Save();
                                return ConfigDefaults.MainDefaults;
                            }
                            else
                            {
                                input = ConfigDefaults.Defaults[DisplayName];
                                ConfigDefaults.Defaults[DisplayName].Save();
                            }
                        }
                        else Plugin.Log($"Attempting to load an unrecognised type ({type.Name}) from Config. WTF!?!?", Plugin.LogInfo.Error, "Open an Issue on the Counters+ GitHub.");
                    }
                }
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

    public abstract class IConfigModel {
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
                if (info.MemberType == MemberTypes.Field)
                {
                    FieldInfo finfo = (FieldInfo)info;
                    if (finfo.Name.ToLower() == "restrictedpositions") continue;
                    Plugin.config.SetString(DisplayName, info.Name, finfo.GetValue(this).ToString());
                }
            }
        }
    }

    public sealed class MissedConfigModel : IConfigModel {
        public MissedConfigModel() { DisplayName = "Missed"; }
    }

    public sealed class NoteConfigModel : IConfigModel {
        public NoteConfigModel() { DisplayName = "Notes"; }
        public bool ShowPercentage;
        public int DecimalPrecision;
    }

    public sealed class ProgressConfigModel : IConfigModel {
        public ProgressConfigModel() { DisplayName = "Progress"; }
        public ICounterMode Mode;
        public bool ProgressTimeLeft;
        public bool IncludeRing;
    }

    public sealed class ScoreConfigModel : IConfigModel
    {
        public ScoreConfigModel() { DisplayName = "Score"; }
        public ICounterMode Mode;
        public int DecimalPrecision;
        public bool DisplayRank;
    }

    public sealed class PBConfigModel : IConfigModel{
        public PBConfigModel() { DisplayName = "Personal Best"; }
        public int DecimalPrecision;
        public int TextSize;
        public bool UnderScore;
    }

    public sealed class SpeedConfigModel : IConfigModel
    {
        public SpeedConfigModel() { DisplayName = "Speed"; }
        public int DecimalPrecision;
        public ICounterMode Mode;
    }

    public sealed class SpinometerConfigModel : IConfigModel
    {
        public SpinometerConfigModel() { DisplayName = "Spinometer"; }
        public ICounterMode Mode;
    }

    public sealed class CutConfigModel : IConfigModel {
        public CutConfigModel() { DisplayName = "Cut"; }
    }
    
    public enum ICounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }

    public enum ICounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth, //Speed
                              BaseGame, Original, Percent, //Progress
                              ScoreOnly, LeavePoints, BaseWithOutPoints, //Score (As well as BaseGame and Original)
                              Highest, //Spinometer (As well as Original and SplitAverage)
    };
}