using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CountersPlus.Custom;
using Newtonsoft.Json.Converters;
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
                model = createDefaultJSON();
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
                    Plugin.Log("Error loading JSON! Overwriting with default JSON...", Plugin.LogInfo.Error);
                    Plugin.Log(e.Message, Plugin.LogInfo.Error);
                    model = createDefaultJSON();
                }
            }
            return model;
        }

        internal static MainConfigModel createDefaultJSON()
        {
            MainConfigModel def = new MainConfigModel { Enabled = true, RNG = false, DisableMenus = false,
                missedConfig = new MissedConfigModel()
                {
                    Enabled = true, Position = ICounterPositions.BelowCombo, Index = 0,
                },
                accuracyConfig = new AccuracyConfigModel
                {
                    Enabled = true, Position = ICounterPositions.BelowCombo, Index = 1,
                    ShowPercentage = true,
                    DecimalPrecision = 2,
                },
                progressConfig = new ProgressConfigModel
                {
                    Enabled = true, Position = ICounterPositions.BelowEnergy, Index = 0,
                    Mode = ICounterMode.Original,
                    ProgressTimeLeft = false,
                },
                scoreConfig = new ScoreConfigModel
                {
                    Enabled = true, Position = ICounterPositions.BelowMultiplier, Index = 0,
                    UseOld = false,
                    DecimalPrecision = 2,
                    DisplayRank = true,
                },
                /*pBConfig = new PBConfigModel
                {
                    Enabled = false, Position = ICounterPositions.BelowMultiplier, Index = 1,
                    DecimalPrecision = 2,
                },*/
                speedConfig = new SpeedConfigModel
                {
                    Enabled = false, Position = ICounterPositions.AboveHighway, Index = 0,
                    DecimalPrecision = 2,
                    Mode = ICounterMode.Average,
                },
                cutConfig = new CutConfigModel
                {
                    Enabled = false, Position = ICounterPositions.AboveCombo, Index = 0,
                },
            };
            using (StreamWriter file = File.CreateText(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() => {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.Formatting = Formatting.Indented;
                    return settings;
                });
                serializer.Serialize(file, def);
            }
            return def;
        }
    }
    
    public class MainConfigModel {
        public bool Enabled;
        public bool RNG;
        public bool DisableMenus;
        public MissedConfigModel missedConfig;
        public AccuracyConfigModel accuracyConfig;
        public ProgressConfigModel progressConfig;
        public ScoreConfigModel scoreConfig;
        //public PBConfigModel pBConfig;
        public SpeedConfigModel speedConfig;
        public CutConfigModel cutConfig;
        [JsonIgnore]
        public List<CustomConfigModel> CustomCounters = new List<CustomConfigModel>();
        [JsonIgnore]
        public bool isSaving = false;

        public async void save() //Give Config some time to save JSON without having it be malformed.
        {
            await Task.Run(() => {
                isSaving = true;
                using (StreamWriter file = File.CreateText(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, this);
                }
                if (!Directory.Exists(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters"))
                    Directory.CreateDirectory(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters");
                Plugin.Log("Settings saved!");
            });

            await Task.Run(() =>
            {
                Thread.Sleep(100);
                isSaving = false;
            });
        }

        public void saveCustom(CustomConfigModel model)
        {
            using (StreamWriter file = File.CreateText(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters/{model.JSONName}.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, model);
            }
        }
    }

    public interface IConfigModel {
        /// <summary>
        /// The name that will show in the Settings UI.
        /// </summary>
        string DisplayName { get; set; }
        /// <summary>
        /// Whether or not this counter will be enabled.
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// The relative position of the counter.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        ICounterPositions Position { get; set; }
        /// <summary>
        /// A multiplier variable. The higher this is, the farther away it'll be.
        /// </summary>
        int Index { get; set; }
    }

    public class MissedConfigModel : IConfigModel
    {
        public string DisplayName { get; set; } = "Missed";
        public bool Enabled { get; set; }
        public ICounterPositions Position { get; set; }
        public int Index { get; set; }
    }

    public class AccuracyConfigModel : IConfigModel {
        public string DisplayName { get; set; } = "Notes";
        public bool Enabled { get; set; }
        public ICounterPositions Position { get; set; }
        public int Index { get; set; }
        public bool ShowPercentage;
        public int DecimalPrecision;
    }

    public class ProgressConfigModel : IConfigModel {
        public string DisplayName { get; set; } = "Progress";
        public bool Enabled { get; set; }
        public ICounterPositions Position { get; set; }
        public int Index { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ICounterMode Mode;
        public bool ProgressTimeLeft;
    }

    public class ScoreConfigModel : IConfigModel
    {
        public string DisplayName { get; set; } = "Score";
        public bool Enabled { get; set; }
        public ICounterPositions Position { get; set; }
        public int Index { get; set; }
        public bool UseOld;
        public int DecimalPrecision;
        public bool DisplayRank;
    }

    public class PBConfigModel : IConfigModel{
        public string DisplayName { get; set; } = "PB";
        public bool Enabled { get; set; }
        public ICounterPositions Position { get; set; }
        public int Index { get; set; }
        public int DecimalPrecision;
    }

    public class SpeedConfigModel : IConfigModel
    {
        public string DisplayName { get; set; } = "Speed";
        public bool Enabled { get; set; }
        public ICounterPositions Position { get; set; }
        public int Index { get; set; }
        public int DecimalPrecision;
        [JsonConverter(typeof(StringEnumConverter))]
        public ICounterMode Mode;
    }

    public class CutConfigModel : IConfigModel
    {
        public string DisplayName { get; set; } = "Cut";
        public bool Enabled { get; set; }
        public ICounterPositions Position { get; set; }
        public int Index { get; set; }
    }
    
    public enum ICounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }

    public enum ICounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth, //Speed
                              BaseGame, Original, Percent //Progress
    };
}
