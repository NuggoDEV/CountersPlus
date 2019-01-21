using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CountersPlus.Custom;
using Newtonsoft.Json.Converters;
using BS_Utils.Utilities;
using System.Threading.Tasks;
using System.Threading;

namespace CountersPlus.Config
{
    public class Config
    {

        public static MainConfigModel loadSettings()
        {
            MainConfigModel model = new MainConfigModel();
            if (File.Exists(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json"))
            {
                try
                {
                    string json = File.ReadAllText(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json");
                    model = JsonConvert.DeserializeObject<MainConfigModel>(json);
                    File.Delete(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json");
                    Plugin.Log("Loaded JSON for the last time. Goodbye, JSON!");
                }
                catch (Exception e)
                {
                    Plugin.Log("Error loading JSON!", Plugin.LogInfo.Error);
                }
            }
            Plugin.Log("Config loaded.");
            return model;
        }
    }
    
    public class MainConfigModel {
        public bool Enabled { get {
                return new BS_Utils.Utilities.Config("CountersPlus").GetBool("Main", "Enabled", true, true);
            } set {
                new BS_Utils.Utilities.Config("CountersPlus").SetBool("Main", "Enabled", value);
            } }
        public bool RNG
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetBool("Main", "RNG", true, true);
            }
            set
            {
                new BS_Utils.Utilities.Config("CountersPlus").SetBool("Main", "RNG", value);
            }
        }
        public bool DisableMenus
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetBool("Main", "DisableMenus", true, true);
            }
            set
            {
                new BS_Utils.Utilities.Config("CountersPlus").SetBool("Main", "DisableMenus", value);
            }
        }
        public float ComboOffset {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetFloat("Main", "ComboOffset", 0.2f, true);
            }
            set
            {
                new BS_Utils.Utilities.Config("CountersPlus").SetFloat("Main", "ComboOffset", value);
            }
        }
        public float MultiplierOffset {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetFloat("Main", "MultiplierOffset", 0.2f, true);
            }
            set
            {
                new BS_Utils.Utilities.Config("CountersPlus").SetFloat("Main", "MultiplierOffset", value);
            }
        }
        public MissedConfigModel missedConfig;
        public NoteConfigModel noteConfig;
        public ProgressConfigModel progressConfig;
        public ScoreConfigModel scoreConfig;
        //public PBConfigModel pBConfig;
        public SpeedConfigModel speedConfig;
        public CutConfigModel cutConfig;

        public async void saveCustom(CustomConfigModel model)
        {
            using (StreamWriter file = File.CreateText(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters/{model.JSONName}.json"))
            {
                await file.WriteLineAsync(JsonConvert.SerializeObject(model));
            }
        }
    }

    public abstract class IConfigModel {
        public string DisplayName { get; protected set; }
        public virtual bool Enabled { get {
                return new BS_Utils.Utilities.Config("CountersPlus").GetBool(DisplayName, "Enabled", true, true);
            }set{  new BS_Utils.Utilities.Config("CountersPlus").SetBool(DisplayName, "Enabled", value); } }
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual ICounterPositions Position { get {
                return (ICounterPositions)Enum.Parse(typeof(ICounterPositions), new BS_Utils.Utilities.Config("CountersPlus").GetString(DisplayName, "Position", "BelowCombo", true));
            } set {
                new BS_Utils.Utilities.Config("CountersPlus").SetString(DisplayName, "Position", value.ToString());
            } }
        public virtual int Index { get{
                return new BS_Utils.Utilities.Config("CountersPlus").GetInt(DisplayName, "Index", 0, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetInt(DisplayName, "Index", value); }
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
                return new BS_Utils.Utilities.Config("CountersPlus").GetBool(DisplayName, "ShowPercentage", true, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetBool(DisplayName, "ShowPercentage", value); }
        }
        public int DecimalPrecision
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetInt(DisplayName, "DecimalPrecision", 0, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetInt(DisplayName, "DecimalPrecision", value); }
        }
    }

    public sealed class ProgressConfigModel : IConfigModel {
        public ProgressConfigModel() { DisplayName = "Progress"; }
        public ICounterMode Mode
        {
            get
            {
                return (ICounterMode)Enum.Parse(typeof(ICounterMode), new BS_Utils.Utilities.Config("CountersPlus").GetString(DisplayName, "Mode", "BelowCombo", true));
            }
            set
            {
                new BS_Utils.Utilities.Config("CountersPlus").SetString(DisplayName, "Position", value.ToString());
            }
        }
        public bool ProgressTimeLeft
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetBool(DisplayName, "ProgressTimeLeft", true, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetBool(DisplayName, "ProgressTimeLeft", value); }
        }
    }

    public sealed class ScoreConfigModel : IConfigModel
    {
        public ScoreConfigModel() { DisplayName = "Score"; }
        public bool UseOld
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetBool(DisplayName, "UseOld", true, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetBool(DisplayName, "UseOld", value); }
        }
        public int DecimalPrecision
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetInt(DisplayName, "DecimalPrecision", 0, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetInt(DisplayName, "DecimalPrecision", value); }
        }
        public bool DisplayRank
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetBool(DisplayName, "DisplayRank", true, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetBool(DisplayName, "DisplayRank", value); }
        }
    }

    public sealed class PBConfigModel : IConfigModel{
        public PBConfigModel() { DisplayName = "PB"; }
        public int DecimalPrecision
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetInt(DisplayName, "DecimalPrecision", 0, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetInt(DisplayName, "DecimalPrecision", value); }
        }
    }

    public sealed class SpeedConfigModel : IConfigModel
    {
        public SpeedConfigModel() { DisplayName = "Speed"; }
        public int DecimalPrecision
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetInt(DisplayName, "DecimalPrecision", 0, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetInt(DisplayName, "DecimalPrecision", value); }
        }
        public ICounterMode Mode
        {
            get
            {
                return (ICounterMode)Enum.Parse(typeof(ICounterMode), new BS_Utils.Utilities.Config("CountersPlus").GetString(DisplayName, "Mode", "BelowCombo", true));
            }
            set
            {
                new BS_Utils.Utilities.Config("CountersPlus").SetString(DisplayName, "Position", value.ToString());
            }
        }
    }

    public class CutConfigModel : IConfigModel {
        public CutConfigModel() { DisplayName = "Cut"; }
    }
    
    public enum ICounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }

    public enum ICounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth, //Speed
                              BaseGame, Original, Percent //Progress
    };
}
