using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace CountersPlus.Config
{
    public class ConfigLoader
    {
        public static MainConfigModel loadSettings()
        {
            MainConfigModel model = new MainConfigModel();
            try
            {
                //Pings all the settings options so they are both:
                //A) assigned in CountersController.settings
                //B) created if they do not exist in CountersPlus.ini
                model.Enabled = model.Enabled;
                model.ComboOffset = model.ComboOffset;
                model.MultiplierOffset = model.MultiplierOffset;
                model.AdvancedCounterInfo = model.AdvancedCounterInfo;
                model.FirstStart = model.FirstStart;
                model.missedConfig = new MissedConfigModel();
                model.noteConfig = new NoteConfigModel();
                model.progressConfig = new ProgressConfigModel();
                model.scoreConfig = new ScoreConfigModel();
                model.speedConfig = new SpeedConfigModel();
                model.cutConfig = new CutConfigModel();
                if (new[] {
                    model.missedConfig.Index, model.noteConfig.Index, model.progressConfig.Index, model.scoreConfig.Index,
                    model.speedConfig.Index, model.cutConfig.Index
                }.All(x => x == 0))
                {
                    if (new[] {
                        model.missedConfig.Position, model.noteConfig.Position, model.progressConfig.Position, model.scoreConfig.Position,
                        model.speedConfig.Position, model.cutConfig.Position
                    }.All(x => x == ICounterPositions.BelowCombo))
                    {
                        ResetSettings(model);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(NullReferenceException))
                {
                    Plugin.Log(e.ToString());
                    ResetSettings(model);
                }
            }
            if (File.Exists(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json"))
            {
                try
                {
                    string json = File.ReadAllText(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json");
                    model = JsonConvert.DeserializeObject<MainConfigModel>(json);
                    File.Delete(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json");
                    Plugin.Log("Loaded JSON for the last time. Goodbye, JSON!");
                }
                catch
                {
                    Plugin.Log("Error loading JSON!", Plugin.LogInfo.Error);
                }
            }
            Plugin.Log("Config loaded.");
            return model;
        }

        //im not sure if I need this function anymore.
        internal static void ResetSettings(MainConfigModel settings)
        {
            ResetSetting(settings.missedConfig, true, ICounterPositions.BelowCombo, 0);
            ResetSetting(settings.noteConfig, true, ICounterPositions.BelowCombo, 1);
            ResetSetting(settings.progressConfig, true, ICounterPositions.BelowEnergy, 0);
            ResetSetting(settings.scoreConfig, true, ICounterPositions.BelowMultiplier, 0);
            ResetSetting(settings.speedConfig, false, ICounterPositions.AboveMultiplier, 0);
            ResetSetting(settings.cutConfig, false, ICounterPositions.AboveHighway, 0);
        }

        private static void ResetSetting<T>(T model, bool en, ICounterPositions pos, int index) where T : IConfigModel
        {
            model.Enabled = en;
            model.Position = pos;
            model.Index = index;
        }
    }
    
    public class MainConfigModel {
        public bool Enabled { get {
                return Plugin.config.GetBool("Main", "Enabled", true, true);
            } set {
                Plugin.config.SetBool("Main", "Enabled", value);
            } }
        public bool AdvancedCounterInfo
        {
            get
            {
                return Plugin.config.GetBool("Main", "AdvancedCounterInfo", true, true);
            }
            set
            {
                Plugin.config.SetBool("Main", "AdvancedCounterInfo", value);
            }
        }
        public bool FirstStart
        {
            get
            {
                return Plugin.config.GetBool("Main", "FirstStart", false, true);
            }
            set
            {
                Plugin.config.SetBool("Main", "FirstStart", value);
            }
        }
        public float ComboOffset {
            get
            {
                return Plugin.config.GetFloat("Main", "ComboOffset", 0.2f, true);
            }
            set
            {
                Plugin.config.SetFloat("Main", "ComboOffset", value);
            }
        }
        public float MultiplierOffset {
            get
            {
                return Plugin.config.GetFloat("Main", "MultiplierOffset", 0.2f, true);
            }
            set
            {
                Plugin.config.SetFloat("Main", "MultiplierOffset", value);
            }
        }
        public MissedConfigModel missedConfig;
        public NoteConfigModel noteConfig;
        public ProgressConfigModel progressConfig;
        public ScoreConfigModel scoreConfig;
        //public PBConfigModel pBConfig;
        public SpeedConfigModel speedConfig;
        public CutConfigModel cutConfig;
    }

    public abstract class IConfigModel {
        public string DisplayName { get; internal set; }
        public bool Enabled { get {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return Plugin.config.GetBool(DisplayName, "Enabled", true, true);
                else return true;
            }set{ if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetBool(DisplayName, "Enabled", value); } }
        public ICounterPositions Position { get {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return (ICounterPositions)Enum.Parse(typeof(ICounterPositions), Plugin.config.GetString(DisplayName, "Position", "BelowCombo", true));
                else return ICounterPositions.BelowCombo;
            } set {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetString(DisplayName, "Position", value.ToString());
            } }
        public int Index { get{
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return Plugin.config.GetInt(DisplayName, "Index", 0, true);
                else return 0;
            }
            set { if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetInt(DisplayName, "Index", value); }
        }
    }

    public sealed class MissedConfigModel : IConfigModel {
        public MissedConfigModel() { DisplayName = "Missed"; }
    }

    public sealed class NoteConfigModel : IConfigModel {
        public NoteConfigModel() { DisplayName = "Notes"; }
        public bool ShowPercentage
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return Plugin.config.GetBool(DisplayName, "ShowPercentage", true, true);
                else return true;
            }
            set { if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetBool("Notes", "ShowPercentage", value); }
        }
        public int DecimalPrecision
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return Plugin.config.GetInt(DisplayName, "DecimalPrecision", 2, true);
                else return 2;
            }
            set { if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetInt("Notes", "DecimalPrecision", value); }
        }
    }

    public sealed class ProgressConfigModel : IConfigModel {
        public ProgressConfigModel() { DisplayName = "Progress"; }
        public ICounterMode Mode
        {
            get
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                        return (ICounterMode)Enum.Parse(typeof(ICounterMode), Plugin.config.GetString("Progress", "Mode", "Original", true));
                    else return ICounterMode.Original;
                }
                catch
                {
                    Plugin.config.SetString(DisplayName, "Mode", ICounterMode.Original.ToString());
                    return ICounterMode.Original;
                }
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    Plugin.config.SetString("Progress", "Mode", value.ToString());
            }
        }
        public bool ProgressTimeLeft
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return Plugin.config.GetBool("Progress", "ProgressTimeLeft", false, true);
                else return false;
            }
            set { if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetBool("Progress", "ProgressTimeLeft", value); }
        }
    }

    public sealed class ScoreConfigModel : IConfigModel
    {
        public ScoreConfigModel() { DisplayName = "Score"; }
        public ICounterMode Mode
        {
            get
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                        return (ICounterMode)Enum.Parse(typeof(ICounterMode), Plugin.config.GetString("Score", "Mode", "Both", true));
                    else return ICounterMode.Both;
                }
                catch
                {
                    Plugin.config.SetString(DisplayName, "Mode", ICounterMode.Both.ToString());
                    return ICounterMode.Both;
                }
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    Plugin.config.SetString("Score", "Mode", value.ToString());
            }
        }
        public bool UseOld
        {
            get
            {
                return (Mode == ICounterMode.BaseGame) || (Mode == ICounterMode.BaseWithOutPoints);
            }
        }
        public int DecimalPrecision
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return Plugin.config.GetInt("Score", "DecimalPrecision", 2, true);
                else return 2;
            }
            set { if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetInt("Score", "DecimalPrecision", value); }
        }
        public bool DisplayRank
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return Plugin.config.GetBool("Score", "DisplayRank", true, true);
                else return true;
            }
            set { if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetBool("Score", "DisplayRank", value); }
        }
    }

    public sealed class PBConfigModel : IConfigModel{
        public PBConfigModel() { DisplayName = "PB"; }
        public int DecimalPrecision
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return Plugin.config.GetInt(DisplayName, "DecimalPrecision", 0, true);
                else return 0;
            }
            set { if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetInt(DisplayName, "DecimalPrecision", value); }
        }
    }

    public sealed class SpeedConfigModel : IConfigModel
    {
        public SpeedConfigModel() { DisplayName = "Speed"; }
        public int DecimalPrecision
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    return Plugin.config.GetInt("Speed", "DecimalPrecision", 2, true);
                else return 2;
            }
            set { if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main") Plugin.config.SetInt("Speed", "DecimalPrecision", value); }
        }
        public ICounterMode Mode
        {
            get
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                        return (ICounterMode)Enum.Parse(typeof(ICounterMode), Plugin.config.GetString("Speed", "Mode", "Average", true));
                    else return ICounterMode.Average;
                }
                catch
                {
                    Plugin.config.SetString(DisplayName, "Mode", ICounterMode.Average.ToString());
                    return ICounterMode.Average;
                }
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(DisplayName) && DisplayName != "Main")
                    Plugin.config.SetString("Speed", "Mode", value.ToString());
            }
        }
    }

    public class CutConfigModel : IConfigModel {
        public CutConfigModel() { DisplayName = "Cut"; }
    }
    
    public enum ICounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }

    public enum ICounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth, //Speed
                              BaseGame, Original, Percent, //Progress
                              ScoreOnly, LeavePoints, BaseWithOutPoints //Score (As well as BaseGame and Original)
    };
}
