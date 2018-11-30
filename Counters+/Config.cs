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
            MainConfigModel model = new MainConfigModel
            {
                Enabled = ModPrefs.GetBool("Counters+ | Main", "Enabled", true, true),
                RNG = ModPrefs.GetBool("Counters+ | Main", "RNG", false, true),
                missedConfig = new MissedConfigModel
                {
                    Enabled = ModPrefs.GetBool("Counters+ | Missed", "Enabled", true, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions),ModPrefs.GetString("Counters+ | Missed", "Position", "BelowCombo", true)),
                    Index = ModPrefs.GetInt("Counters+ | Missed", "Index", 0, true),
                },
                accuracyConfig = new AccuracyConfigModel
                {
                    Enabled = ModPrefs.GetBool("Counters+ | Accuracy", "Enabled", true, true),
                    Position = (CounterPositions)Enum.Parse(typeof(CounterPositions), ModPrefs.GetString("Counters+ | Accuracy", "Position", "BelowCombo", true)),
                    ShowPercentage = ModPrefs.GetBool("Counters+ | Accuracy", "ShowPercentage", true, true),
                    Index = ModPrefs.GetInt("Counters+ | Accuracy", "Index", 1, true),
                    DecimalPrecision = ModPrefs.GetInt("Counters+ | Accuracy", "DecimalPrecision", 1, true),
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
                }
            };
            return model;
        }

        public static Vector3 ParseVector(string v)
        {
            string[] split = v.Split('|');
            if (split.Count() == 3)
            {
                return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
            }
            return Vector3.zero;
        }

        public static string ParseVector(Vector3 v)
        {
            return string.Format("{0}|{1}|{2}", v.x, v.y, v.z);
        }
    }

    public struct MainConfigModel {
        public bool Enabled;
        public bool RNG;
        public MissedConfigModel missedConfig;
        public AccuracyConfigModel accuracyConfig;
        public ProgressConfigModel progressConfig;
        public ScoreConfigModel scoreConfig;
        public PBConfigModel pBConfig;

        public void save()
        {
            ModPrefs.SetBool("Counters+ | Main", "Enabled", Enabled);
            ModPrefs.SetBool("Counters+ | Main", "RNG", RNG);
            save("Missed", missedConfig);
            save("Accuracy", accuracyConfig);
            save("Progress", progressConfig);
            save("Score", scoreConfig);
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

    public enum CounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy }
}
