using CountersPlus.Custom;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using System.Linq;
using CountersPlus.Utils;
using IPA.Utilities;
using UnityEngine;

namespace CountersPlus.Config
{
    public class ConfigLoader
    {
        internal static BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config("CountersPlus");
        /// <summary>
        /// Load Counters+ settings from config.
        /// Automatically generates any missing settings with their defaults found in the ConfigDefaults class.
        /// </summary>
        public static MainConfigModel LoadSettings()
        {
            if (!File.Exists(Path.Combine(UnityGame.UserDataPath, "CountersPlus.ini")))
                File.Create(Path.Combine(UnityGame.UserDataPath, "CountersPlus.ini"));
            MainConfigModel model = new MainConfigModel();
            model = DeserializeFromConfig(model, model.DisplayName);
            try
            {   //For adding new Counters, assign your ConfigModel here, using the DeserializeFromConfig function.
                model.hudConfig = DeserializeFromConfig(model.hudConfig, model.hudConfig.DisplayName);
                model.missedConfig = DeserializeFromConfig(model.missedConfig, model.missedConfig.DisplayName);
                model.noteConfig = DeserializeFromConfig(model.noteConfig, model.noteConfig.DisplayName);
                model.progressConfig = DeserializeFromConfig(model.progressConfig, model.progressConfig.DisplayName);
                model.scoreConfig = DeserializeFromConfig(model.scoreConfig, model.scoreConfig.DisplayName);
                model.speedConfig = DeserializeFromConfig(model.speedConfig, model.speedConfig.DisplayName);
                model.cutConfig = DeserializeFromConfig(model.cutConfig, model.cutConfig.DisplayName);
                model.spinometerConfig = DeserializeFromConfig(model.spinometerConfig, model.spinometerConfig.DisplayName);
                model.pbConfig = DeserializeFromConfig(model.pbConfig, model.pbConfig.DisplayName);
                model.notesLeftConfig = DeserializeFromConfig(model.notesLeftConfig, model.notesLeftConfig.DisplayName);
                model.failsConfig = DeserializeFromConfig(model.failsConfig, model.failsConfig.DisplayName);
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(NullReferenceException)) Plugin.Log(e.ToString(), LogInfo.Error);
            }
            Plugin.Log("Config loaded!", LogInfo.Notice);
            return model;
        }

        /// <summary>
        /// Grabs a list of all Custom Counter references in CountersPlus.ini, if their mod is loaded by BSIPA.
        /// </summary>
        public static List<CustomConfigModel> LoadCustomCounters()
        {
            List<CustomConfigModel> counters = new List<CustomConfigModel>();
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Path.Combine(UnityGame.UserDataPath, "CountersPlus.ini"));
            foreach (SectionData section in data.Sections)
            {
                if (!TypesUtility.GetListOfType<ConfigModel>().Any(y => y.DisplayName == section.SectionName))
                {
                    CustomConfigModel unloadedModel = new CustomConfigModel(section.SectionName);
                    CustomConfigModel loadedModel = DeserializeFromConfig(unloadedModel, section.SectionName);
                    counters.Add(loadedModel);
                }
            }
            return counters;
        }

        /// <summary>
        /// Automatically assigns fields of an input from the Config file, and attempts to assign defaults.
        /// If you want to use this code, it is recommended to copy this yourself.
        /// </summary>
        public static T DeserializeFromConfig<T>(T input, string DisplayName)
        {
            Type type = input.GetType();
            MemberInfo[] infos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo info in infos)
            {
                if (info.MemberType != MemberTypes.Field || info.Name.ToLower().Contains("config")) continue;
                FieldInfo finfo = (FieldInfo)info;
                string value = config.GetString(DisplayName, info.Name, null);
                if (value == null)
                {
                    Plugin.Log($"Failed to load variable {info.Name} in {type.Name}. Using defaults...", LogInfo.Info);
                    continue;
                }
                if (finfo.FieldType == typeof(ICounterMode))
                    input.SetPrivateField(info.Name, Enum.Parse(typeof(ICounterMode), value));
                else if (finfo.FieldType == typeof(ICounterPositions))
                    input.SetPrivateField(info.Name, Enum.Parse(typeof(ICounterPositions), value));
                else input.SetPrivateField(info.Name, Convert.ChangeType(value, finfo.FieldType));
                if (finfo.GetValue(input) == null) throw new Exception();
            }
            return input;
        }
    }
    
    /// <summary>
    /// Main class for Counters+ config.
    /// For adding new Counters, add their ConfigModels as a field in this class, making sure that "Config" is in the name.
    /// </summary>
    public class MainConfigModel {
        public string DisplayName { get { return "Main"; } }
        public bool Enabled = true;
        public bool AdvancedCounterInfo = true;
        public bool HideCombo = false;
        public bool HideMultiplier = false;
        public float ComboOffset = 0.2f;
        public float MultiplierOffset = 0.4f;
        public bool AprilFoolsTomfoolery = true;
        public HUDConfigModel hudConfig = new HUDConfigModel();
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
            MemberInfo[] infos = GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo info in infos)
            {
                if (info.MemberType != MemberTypes.Field) continue;
                FieldInfo finfo = (FieldInfo)info;
                if (finfo.Name.ToLower().Contains("config"))
                {
                    finfo.FieldType.GetMethod("Save").Invoke(finfo.GetValue(this), null);
                    continue;
                }
                ConfigLoader.config.SetString(DisplayName, info.Name, finfo.GetValue(this).ToString());
            }
            hudConfig.Save();
        }
    }

    public sealed class HUDConfigModel
    {
        public string DisplayName => "HUD Settings";
        public bool AttachBaseGameHUD = true;
        public bool AttachToBaseGameHUDFor360 = true;
        public float HUDSize = 10;
        public float HUDPositionScaleFactor = 10;
        public Vector3 HUDPosition => new Vector3(HUDPosition_X, HUDPosition_Y, HUDPosition_Z);
        public float HUDPosition_X = 0;
        public float HUDPosition_Y = 0;
        public float HUDPosition_Z = 7;
        public Vector3 HUDRotation => new Vector3(HUDRotation_X, HUDRotation_Y, HUDRotation_Z);
        public float HUDRotation_X = 0;
        public float HUDRotation_Y = 0;
        public float HUDRotation_Z = 0;
        public bool AttachHUDToCamera = false;
        public string AttachedCamera = "Main Camera";
        public void Save()
        {
            MemberInfo[] infos = GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo info in infos)
            {
                if (info.MemberType != MemberTypes.Field) continue;
                FieldInfo finfo = (FieldInfo)info;
                ConfigLoader.config.SetString(DisplayName, info.Name, finfo.GetValue(this).ToString());
            }
        }
    }

    /// <summary>
    /// The base config class for every single Counter in Counters+.
    /// As part of creating a new Counter, you will need to make a class that inherits ConfigModel.
    /// For adding new options to an existing Counter, add them to their respective ConfigModel.
    /// Add defaults to ConfigModels in the ConfigDefaults class.
    /// </summary>
    public abstract class ConfigModel {
        public string DisplayName { get; internal set; } //DisplayName and VersionAdded should not be changed once set.
        internal SemVer.Version VersionAdded { get; set; } = null;
        public bool Enabled;
        public ICounterPositions Position;
        public int Distance;

        public void Save()
        {
            MemberInfo[] infos = GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo info in infos)
            {
                if (info.MemberType != MemberTypes.Field) continue;
                FieldInfo finfo = (FieldInfo)info;
                ConfigLoader.config.SetString(DisplayName, info.Name, finfo.GetValue(this).ToString());
            }
        }
    }

    public sealed class MissedConfigModel : ConfigModel {
        public MissedConfigModel() { DisplayName = "Missed"; VersionAdded = new SemVer.Version("1.0.0");
            Enabled = true; Position = ICounterPositions.BelowCombo; Distance = 0; } //Default values
        public bool CustomMissTextIntegration = false;
    }

    public sealed class NoteConfigModel : ConfigModel {
        public NoteConfigModel() { DisplayName = "Notes"; VersionAdded = new SemVer.Version("1.0.0");
            Enabled = false; Position = ICounterPositions.BelowCombo; Distance = 1; } //Default values
        public bool ShowPercentage = false;
        public int DecimalPrecision = 2;
    }

    public sealed class ProgressConfigModel : ConfigModel {
        public ProgressConfigModel() { DisplayName = "Progress"; VersionAdded = new SemVer.Version("1.0.0");
            Enabled = true; Position = ICounterPositions.BelowEnergy; Distance = 0; } //Default values
        public ICounterMode Mode = ICounterMode.Original;
        public bool ProgressTimeLeft = false;
        public bool IncludeRing = false;
        public bool ShowTimeInBeats = false;
    }

    public sealed class ScoreConfigModel : ConfigModel
    {
        public ScoreConfigModel()
        {
            DisplayName = "Score"; VersionAdded = new SemVer.Version("1.0.0");
            Enabled = true; Position = ICounterPositions.BelowMultiplier; Distance = 0;
        } //Default values
        public ICounterMode Mode = ICounterMode.Original;
        public int DecimalPrecision = 2;
        public bool DisplayRank = true;
        public bool CustomRankColors = true;
        public string SSColor = "#00FFFF";
        public string SColor = "#FFFFFF";
        public string AColor = "#00FF00";
        public string BColor = "#FFFF00";
        public string CColor = "#FFA700";
        public string DColor = "#FF0000";
        public string EColor = "#FF0000";
    }

    public sealed class PBConfigModel : ConfigModel{
        public PBConfigModel() { DisplayName = "Personal Best"; VersionAdded = new SemVer.Version("1.5.5");
            Enabled = true; Position = ICounterPositions.BelowMultiplier; Distance = 1; } //Default values
        public int DecimalPrecision = 2;
        public int TextSize = 2;
        public bool UnderScore = true;
        public bool HideFirstScore = false;
    }

    public sealed class SpeedConfigModel : ConfigModel
    {
        public SpeedConfigModel() { DisplayName = "Speed"; VersionAdded = new SemVer.Version("1.1.0");
            Enabled = false; Position = ICounterPositions.BelowMultiplier; Distance = 2; } //Default values
        public int DecimalPrecision = 2;
        public ICounterMode Mode = ICounterMode.Average;
    }

    public sealed class SpinometerConfigModel : ConfigModel
    {
        public SpinometerConfigModel() { DisplayName = "Spinometer"; VersionAdded = new SemVer.Version("1.4.1");
            Enabled = false; Position = ICounterPositions.AboveMultiplier; Distance = 0; } //Default values
        public ICounterMode Mode = ICounterMode.Highest;
    }

    public sealed class CutConfigModel : ConfigModel {
        public CutConfigModel() { DisplayName = "Cut"; VersionAdded = new SemVer.Version("1.1.0");
            Enabled = false; Position = ICounterPositions.AboveHighway; Distance = 1; } //Default values
        public bool SeparateSaberCounts = false;
        public bool SeparateCutValues = false;
        public int AveragePrecision = 1;
    }

    public sealed class NotesLeftConfigModel : ConfigModel
    {
        public NotesLeftConfigModel() { DisplayName = "Notes Left"; VersionAdded = new SemVer.Version("1.5.8");
            Enabled = false; Position = ICounterPositions.AboveHighway; Distance = -1; } //Default values
        public bool LabelAboveCount = false;
    }

    public sealed class FailConfigModel : ConfigModel
    {
        public FailConfigModel() { DisplayName = "Fail"; VersionAdded = new SemVer.Version("1.5.8");
            Enabled = false; Position = ICounterPositions.AboveCombo; Distance = 0; } //Default values
        public bool ShowRestartsInstead = false;
    }
    
    public enum ICounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }

    public enum ICounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth, //Speed
                              BaseGame, Original, Percent, //Progress
                              ScoreOnly, LeavePoints, BaseWithOutPoints, //Score (As well as BaseGame and Original)
                              Highest, //Spinometer (As well as Original and SplitAverage)
    };
}