using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IllusionPlugin;
using UnityEngine;

namespace CountersPlus.Config
{
    public class Config
    {
        public static MainConfigModel loadSettings()
        {
            MainConfigModel model = new MainConfigModel();
            if (ModPrefs.HasKey("Counters+ | Accuracy", "Enabled") && !ModPrefs.HasKey("Counters+ | Notes", "Enabled"))
            {
                AccuracyConfigModel newModel = new AccuracyConfigModel
                {
                    Enabled = ModPrefs.GetBool("Counters+ | Accuracy", "Enabled", true, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions), ModPrefs.GetString("Counters+ | Accuracy", "Position", "BelowCombo", true)),
                    ShowPercentage = ModPrefs.GetBool("Counters+ | Accuracy", "ShowPercentage", true, true),
                    Index = ModPrefs.GetInt("Counters+ | Accuracy", "Index", 1, true),
                    DecimalPrecision = ModPrefs.GetInt("Counters+ | Accuracy", "DecimalPrecision", 1, true),
                };
                model.save("Notes", newModel);
            }
            model = new MainConfigModel
            {
                Enabled = ModPrefs.GetBool("Counters+ | Main", "Enabled", true, true),
                RNG = ModPrefs.GetBool("Counters+ | Main", "RNG", false, true),
                DisableMenus = ModPrefs.GetBool("Counters+ | Main", "DisableMenus", false, true),
                missedConfig = new MissedConfigModel
                {
                    Enabled = ModPrefs.GetBool("Counters+ | Missed", "Enabled", true, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions),ModPrefs.GetString("Counters+ | Missed", "Position", "BelowCombo", true)),
                    Index = ModPrefs.GetInt("Counters+ | Missed", "Index", 0, true),
                },
                accuracyConfig = new AccuracyConfigModel
                {
                    Enabled = ModPrefs.GetBool("Counters+ | Notes", "Enabled", true, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions), ModPrefs.GetString("Counters+ | Notes", "Position", "BelowCombo", true)),
                    ShowPercentage = ModPrefs.GetBool("Counters+ | Notes", "ShowPercentage", true, true),
                    Index = ModPrefs.GetInt("Counters+ | Notes", "Index", 1, true),
                    DecimalPrecision = ModPrefs.GetInt("Counters+ | Notes", "DecimalPrecision", 1, true),
                },
                progressConfig = new ProgressConfigModel
                {
                    Enabled = ModPrefs.GetBool("Counters+ | Progress", "Enabled", true, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions), ModPrefs.GetString("Counters+ | Progress", "Position", "BelowEnergy", true)),
                    ProgressTimeLeft = ModPrefs.GetBool("Counters+ | Progress", "ProgressTimeLeft", false, true),
                    Index = ModPrefs.GetInt("Counters+ | Progress", "Index", 0, true),
                    UseOld = ModPrefs.GetBool("Counters+ | Progress", "UseOld", true, true),
                },
                scoreConfig = new ScoreConfigModel {
                    Enabled = ModPrefs.GetBool("Counters+ | Score", "Enabled", true, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions), ModPrefs.GetString("Counters+ | Score", "Position", "BelowMultiplier", true)),
                    DecimalPrecision = ModPrefs.GetInt("Counters+ | Score", "DecimalPrecision", 2, true),
                    DisplayRank = ModPrefs.GetBool("Counters+ | Score", "DisplayRank", true, true),
                    Index = ModPrefs.GetInt("Counters+ | Score", "Index", 0, true),
                    UseOld = ModPrefs.GetBool("Counters+ | Score", "UseOld", true, true),
                },
                pBConfig = new PBConfigModel
                {
                    Enabled = ModPrefs.GetBool("Counters+ | PB", "Enabled", true, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions), ModPrefs.GetString("Counters+ | PB", "Position", "BelowMultiplier", true)),
                    DecimalPrecision = ModPrefs.GetInt("Counters+ | PB", "DecimalPrecision", 2, true),
                    Index = ModPrefs.GetInt("Counters+ | PB", "Index", 1, true),
                },
                speedConfig = new SpeedConfigModel
                {
                    Enabled = ModPrefs.GetBool("Counters+ | Speed", "Enabled", false, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions), ModPrefs.GetString("Counters+ | Speed", "Position", "AboveHighway", true)),
                    DecimalPrecision = ModPrefs.GetInt("Counters+ | Speed", "DecimalPrecision", 2, true),
                    Index = ModPrefs.GetInt("Counters+ | Speed", "Index", 0, true),
                    Mode = (SpeedConfigModel.CounterMode)Enum.Parse(typeof(SpeedConfigModel.CounterMode), ModPrefs.GetString("Counters+ | Speed", "Mode", "Average", true)),
                },
                cutConfig = new CutConfigModel
                {
                    Enabled = ModPrefs.GetBool("Counters+ | Cut", "Enabled", true, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions), ModPrefs.GetString("Counters+ | Cut", "Position", "AboveHighway", true)),
                    Index = ModPrefs.GetInt("Counters+ | Cut", "Index", 0, true),
                }
            };
            
            return model;
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

        public void save()
        {
            ModPrefs.SetBool("Counters+ | Main", "Enabled", Enabled);
            ModPrefs.SetBool("Counters+ | Main", "RNG", RNG);
            ModPrefs.SetBool("Counters+ | Main", "DisableMenus", DisableMenus);
            save("Missed", missedConfig);
            save("Notes", accuracyConfig);
            save("Progress", progressConfig);
            save("Score", scoreConfig);
            save("Speed", speedConfig);
            save("Cut", cutConfig);
        }

        public void save<T>(string name, T settings) where T : ConfigModel
        {
            ModPrefs.SetBool("Counters+ | " + name, "Enabled", settings.Enabled);
            ModPrefs.SetString("Counters+ | " + name, "Position", settings.Position.ToString());
            ModPrefs.SetInt("Counters+ | " + name, "Index", settings.Index);
        }

        public void save(string name, AccuracyConfigModel settings)
        {
            save(name, settings as ConfigModel);
            ModPrefs.SetBool("Counters+ | " + name, "ShowPercentage", settings.ShowPercentage);
            ModPrefs.SetInt("Counters+ | " + name, "DecimalPrecision", settings.DecimalPrecision);
        }

        public void save(string name, ProgressConfigModel settings)
        {
            save(name, settings as ConfigModel);
            ModPrefs.SetBool("Counters+ | " + name, "UseOld", settings.UseOld);
            ModPrefs.SetBool("Counters+ | " + name, "ProgressTimeLeft", settings.ProgressTimeLeft);
        }

        public void save(string name, ScoreConfigModel settings)
        {
            save(name, settings as ConfigModel);
            ModPrefs.SetBool("Counters+ | " + name, "UseOld", settings.UseOld);
            ModPrefs.SetBool("Counters+ | " + name, "DisplayRank", settings.DisplayRank);
            ModPrefs.SetInt("Counters+ | " + name, "DecimalPrecision", settings.DecimalPrecision);
        }

        public void save(string name, SpeedConfigModel settings)
        {
            save(name, settings as ConfigModel);
            ModPrefs.SetString("Counters+ | " + name, "Mode", settings.Mode.ToString());
            ModPrefs.SetInt("Counters+ | " + name, "DecimalPrecision", settings.DecimalPrecision);
        }
    }

    public interface ConfigModel {
        bool Enabled { get; set; }
        CounterPositions Position { get; set; }
        int Index { get; set; }
    }

    public struct MissedConfigModel : ConfigModel
    {
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
    }

    public struct AccuracyConfigModel : ConfigModel {
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public bool ShowPercentage;
        public int DecimalPrecision;
    }

    public struct ProgressConfigModel : ConfigModel {
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public bool UseOld;
        public bool ProgressTimeLeft;
    }

    public struct ScoreConfigModel : ConfigModel
    {
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public bool UseOld;
        public int DecimalPrecision;
        public bool DisplayRank;
    }

    public struct PBConfigModel : ConfigModel{
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public int DecimalPrecision;
    }

    public struct SpeedConfigModel : ConfigModel
    {
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
        public int DecimalPrecision;
        public enum CounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth };
        public CounterMode Mode;
    }

    public struct CutConfigModel : ConfigModel
    {
        public bool Enabled { get; set; }
        public CounterPositions Position { get; set; }
        public int Index { get; set; }
    }

    public enum CounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }
}
