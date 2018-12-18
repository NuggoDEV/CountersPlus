using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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
                    Enabled = true, Position = CounterPositions.BelowCombo, Index = 0,
                },
                accuracyConfig = new AccuracyConfigModel
                {
                    Enabled = true, Position = CounterPositions.BelowCombo, Index = 1,
                    ShowPercentage = true,
                    DecimalPrecision = 2,
                },
                progressConfig = new ProgressConfigModel
                {
                    Enabled = true, Position = CounterPositions.BelowEnergy, Index = 0,
                    Mode = CounterMode.Original,
                    ProgressTimeLeft = false,
                },
                scoreConfig = new ScoreConfigModel
                {
                    Enabled = true, Position = CounterPositions.BelowMultiplier, Index = 0,
                    UseOld = false,
                    DecimalPrecision = 2,
                    DisplayRank = true,
                },
                pBConfig = new PBConfigModel
                {
                    Enabled = false, Position = CounterPositions.BelowMultiplier, Index = 1,
                    DecimalPrecision = 2,
                },
                speedConfig = new SpeedConfigModel
                {
                    Enabled = false, Position = CounterPositions.AboveHighway, Index = 0,
                    DecimalPrecision = 2,
                    Mode = CounterMode.Average,
                },
                cutConfig = new CutConfigModel
                {
                    Enabled = false, Position = CounterPositions.AboveCombo, Index = 0,
                }
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

    public struct MainConfigModel {
        public bool Enabled;
        public bool RNG;
        public bool DisableMenus;
        public MissedConfigModel missedConfig;
        public AccuracyConfigModel accuracyConfig;
        public ProgressConfigModel progressConfig;
        public ScoreConfigModel scoreConfig;
        public PBConfigModel pBConfig;
        public SpeedConfigModel speedConfig;
        public CutConfigModel cutConfig;
        public List<ConfigModel> customCounters;

        public void save()
        {
            StreamWriter writer = File.CreateText(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json");
            writer.Write(JsonConvert.SerializeObject(this));
            Plugin.Log("Settings saved!");
        }
    }

    public interface ConfigModel {
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
        CounterPositions Position { get; set; }
        /// <summary>
        /// A multiplier variable. The higher this is, the farther away it'll be.
        /// </summary>
        int Index { get; set; }
    }

    public class MissedConfigModel : ConfigModel
    {
        public string DisplayName { get; set; } = "Missed";
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
    }

    public class AccuracyConfigModel : ConfigModel {
        public string DisplayName { get; set; } = "Notes";
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public bool ShowPercentage;
        public int DecimalPrecision;
    }

    public class ProgressConfigModel : ConfigModel {
        public string DisplayName { get; set; } = "Progress";
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public CounterMode Mode;
        public bool ProgressTimeLeft;
    }

    public class ScoreConfigModel : ConfigModel
    {
        public string DisplayName { get; set; } = "Score";
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public bool UseOld;
        public int DecimalPrecision;
        public bool DisplayRank;
    }

    public class PBConfigModel : ConfigModel{
        public string DisplayName { get; set; } = "PB";
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public int DecimalPrecision;
    }

    public class SpeedConfigModel : ConfigModel
    {
        public string DisplayName { get; set; } = "Speed";
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public int DecimalPrecision;
        public CounterMode Mode;
    }

    public class CutConfigModel : ConfigModel
    {
        public string DisplayName { get; set; } = "Cut";
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
    }

    public enum CounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }

    public enum CounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth, //Speed
                              BaseGame, Original, Percent //Progress
    };
}
