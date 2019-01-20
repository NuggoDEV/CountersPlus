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
            if (!File.Exists(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json"))
            {
                Plugin.Log("Config JSON can not be found! Creating default JSON...", Plugin.LogInfo.Error);
            }
            else
            {
                try
                {
                    string json = File.ReadAllText(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json");
                    model = JsonConvert.DeserializeObject<MainConfigModel>(json);
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
                return new BS_Utils.Utilities.Config("CountersPus").GetFloat("Main", "ComboOffset", 0.2f, true);
            }
            set
            {
                new BS_Utils.Utilities.Config("CountersPus").SetFloat("Main", "ComboOffset", value);
            }
        }
        public float MultiplierOffset {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPus").GetFloat("Main", "MultiplierOffset", 0.2f, true);
            }
            set
            {
                new BS_Utils.Utilities.Config("CountersPus").SetFloat("Main", "MultiplierOffset", value);
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
        abstract public string DisplayName { get; set; }
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

    public class MissedConfigModel : IConfigModel
    {
        public override string DisplayName { get; set; } = "Missed";
        public override bool Enabled { get; set; } = true;
        public override ICounterPositions Position { get; set; } = ICounterPositions.BelowCombo;
        public override int Index { get; set; } = 0;
    }

    public class NoteConfigModel : IConfigModel {
        public override string DisplayName { get; set; } = "Notes";
        public override bool Enabled { get; set; } = true;
        public override ICounterPositions Position { get; set; } = ICounterPositions.BelowCombo;
        public override int Index { get; set; } = 1;
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

    public class ProgressConfigModel : IConfigModel {
        public override string DisplayName { get; set; } = "Progress";
        public override bool Enabled { get; set; } = true;
        public override ICounterPositions Position { get; set; } = ICounterPositions.BelowEnergy;
        public override int Index { get; set; } = 0;
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

    public class ScoreConfigModel : IConfigModel
    {
        public override string DisplayName { get; set; } = "Score";
        public override bool Enabled { get; set; } = true;
        public override ICounterPositions Position { get; set; } = ICounterPositions.BelowMultiplier;
        public override int Index { get; set; } = 0;
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

    public class PBConfigModel : IConfigModel{
        public override string DisplayName { get; set; } = "PB";
        public override bool Enabled { get; set; } = false;
        public override ICounterPositions Position { get; set; } = ICounterPositions.BelowMultiplier;
        public override int Index { get; set; } = 1;
        public int DecimalPrecision
        {
            get
            {
                return new BS_Utils.Utilities.Config("CountersPlus").GetInt(DisplayName, "DecimalPrecision", 0, true);
            }
            set { new BS_Utils.Utilities.Config("CountersPlus").SetInt(DisplayName, "DecimalPrecision", value); }
        }
    }

    public class SpeedConfigModel : IConfigModel
    {
        public override string DisplayName { get; set; } = "Speed";
        public override bool Enabled { get; set; } = false;
        public override ICounterPositions Position { get; set; } = ICounterPositions.AboveCombo;
        public override int Index { get; set; } = 0;
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

    public class CutConfigModel : IConfigModel
    {
        public override string DisplayName { get; set; } = "Cut";
        public override bool Enabled { get; set; } = false;
        public override ICounterPositions Position { get; set; } = ICounterPositions.AboveHighway;
        public override int Index { get; set; } = 0;
    }
    
    public enum ICounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }

    public enum ICounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth, //Speed
                              BaseGame, Original, Percent //Progress
    };
}
