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
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            MainConfigModel model = new MainConfigModel();
            model = (MainConfigModel)DeserializeFromConfig(model, typeof(MainConfigModel), model.DisplayName);
            try
            {
                Type type = model.GetType();
                MemberInfo[] infos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                model.missedConfig = GrabFromConfig(ref model.missedConfig);
                model.noteConfig = GrabFromConfig(ref model.noteConfig);
                model.progressConfig = GrabFromConfig(ref model.progressConfig);
                model.scoreConfig = GrabFromConfig(ref model.scoreConfig);
                model.speedConfig = GrabFromConfig(ref model.speedConfig);
                model.cutConfig = GrabFromConfig(ref model.cutConfig);
                model.spinometerConfig = GrabFromConfig(ref model.spinometerConfig);
                model.pbConfig = GrabFromConfig(ref model.pbConfig);
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(NullReferenceException)) Plugin.Log(e.ToString(), Plugin.LogInfo.Error);
            }
            Plugin.Log("Config loaded!", Plugin.LogInfo.Notice);
            return model;
        }

        private static T GrabFromConfig<T>(ref T settings) where T : IConfigModel
        {
            return (T)DeserializeFromConfig(settings, typeof(T), settings.DisplayName);
        }

        private static object DeserializeFromConfig(object input, Type type, string DisplayName)
        {
            MemberInfo[] infos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            //DisplayName = ((PropertyInfo)infos.Where((MemberInfo x) => x.Name == "DisplayName").First()).GetValue(input).ToString();
            foreach (MemberInfo info in infos)
            {
                if (info.MemberType == MemberTypes.Field)
                {
                    try
                    {
                        if (info.DeclaringType == typeof(ICounterMode))
                            input.SetPrivateField(info.Name, Enum.Parse(typeof(ICounterMode), Plugin.config.GetString(DisplayName, info.Name)));
                        else if (info.DeclaringType == typeof(ICounterPositions))
                            input.SetPrivateField(info.Name, Enum.Parse(typeof(ICounterPositions), Plugin.config.GetString(DisplayName, info.Name)));
                        else input.SetPrivateField(info.Name, Convert.ChangeType(Plugin.config.GetString(DisplayName, info.Name), info.DeclaringType));
                    }
                    catch
                    {
                        Plugin.Log($"Failed to load variable {info.Name} in {type.Name}.", Plugin.LogInfo.Warning);
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
    }

    public abstract class IConfigModel {
        public string DisplayName { get; internal set; }
        public bool Enabled;
        public ICounterPositions Position;
        public int Index;
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
        public bool UseOld;
        public int DecimalPrecision;
        public bool DisplayRank;
    }

    public sealed class PBConfigModel : IConfigModel{
        public PBConfigModel() { DisplayName = "Personal Best"; }
        public int DecimalPrecision;
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
