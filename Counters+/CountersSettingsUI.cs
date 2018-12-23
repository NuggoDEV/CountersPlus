using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IllusionPlugin;
using CustomUI.Settings;
using CountersPlus.Config;
using System.IO;
using CountersPlus.Custom;
using Newtonsoft.Json;

namespace CountersPlus
{
    class CountersSettingsUI : MonoBehaviour
    {
        internal class PositionSettingsViewController : TupleViewController<Tuple<Config.ICounterPositions, string>> { }
        static List<Tuple<Config.ICounterPositions, string>> positions = new List<Tuple<Config.ICounterPositions, string>> {
            {Config.ICounterPositions.BelowCombo, "Below Combo" },
            {Config.ICounterPositions.AboveCombo, "Above Combo" },
            {Config.ICounterPositions.BelowMultiplier, "Below Multi." },
            {Config.ICounterPositions.AboveMultiplier, "Above Multi." },
            {Config.ICounterPositions.BelowEnergy, "Below Energy" },
            {Config.ICounterPositions.AboveHighway, "Over Highway" }
        };
        internal class ModeViewController : TupleViewController<Tuple<ICounterMode, string>> { }
        static List<Tuple<ICounterMode, string>> speedSettings = new List<Tuple<ICounterMode, string>> {
            {ICounterMode.Average, "Mean Speed" },
            {ICounterMode.Top5Sec, "Top (5 Sec.)" },
            {ICounterMode.Both, "Both" },
            {ICounterMode.SplitAverage, "Split Mean" },
            {ICounterMode.SplitBoth, "Both w/Split" }
        };
        static List<Tuple<ICounterMode, string>> progressSettings = new List<Tuple<ICounterMode, string>> {
            {ICounterMode.BaseGame, "Base Game" },
            {ICounterMode.Original, "Original" },
            {ICounterMode.Percent, "Percentage" }
        };

        static List<Tuple<ICounterMode, string>> creditSetting = new List<Tuple<ICounterMode, string>> {
            {ICounterMode.Original, "Hover" }
        };

        static List<CustomConfigModel> loadedCustoms = new List<CustomConfigModel>();

        /*
         * Coding help from my dad. This allows me to loop through every item in this list and create the settings submenu for it.
         * 
         * ConfigModel: The model to create the base settings SubMenu from (Enabled, Position, Index)
         * Action<SubMenu>: Easy way to pass in the SubMenu created into a function so we can add advanced options.
         * If we don't want any, we can easily just point it to an empty function.
         */
        internal static Dictionary<IConfigModel, Action<SubMenu>> counterUIItems = new Dictionary<IConfigModel, Action<SubMenu>>()
        {
            { CountersController.settings.missedConfig, v => { } },
            { CountersController.settings.accuracyConfig,
                v => {
                        var accuracyPercentage = v.AddBool("Show Percentage", "Toggles the percentage of notes hit over total notes.");
                    accuracyPercentage.GetValue += delegate { return CountersController.settings.accuracyConfig.ShowPercentage; };
                    accuracyPercentage.SetValue += delegate (bool value) {
                        CountersController.settings.accuracyConfig.ShowPercentage = value;
                    };
                    var accuracyPrecision = v.AddInt("Percentage Decimal Precision", "How precise should the percentage be?", 0, 5, 1);
                    accuracyPrecision.GetValue += delegate { return CountersController.settings.accuracyConfig.DecimalPrecision; };
                    accuracyPrecision.SetValue += delegate (int c) {
                        CountersController.settings.accuracyConfig.DecimalPrecision = c;
                    };
                } },
            { CountersController.settings.scoreConfig,
                v => {
                        var scoreRank = v.AddBool("Display Rank", "Displays the rank as you progress in a song.");
                    scoreRank.GetValue += delegate { return CountersController.settings.scoreConfig.DisplayRank; };
                    scoreRank.SetValue += delegate (bool value) {
                        CountersController.settings.scoreConfig.DisplayRank = value;
                    };
                    var scoreOverride = v.AddBool("Override Base Game Counter", "Uses the base game counter. Some settings will not apply in this mode.");
                    scoreOverride.GetValue += delegate { return CountersController.settings.scoreConfig.UseOld; };
                    scoreOverride.SetValue += delegate (bool value) {
                        CountersController.settings.scoreConfig.UseOld = value;
                    };
                    float[] scorePrec = new float[6] { 0, 1, 2, 3, 4, 5 };
                    var scorePrecision = v.AddList("Percentage Decimal Precision", scorePrec, "How precise should the percentage be?");
                    scorePrecision.GetValue += delegate { return CountersController.settings.scoreConfig.DecimalPrecision; };
                    scorePrecision.SetValue += delegate (float c) {
                        CountersController.settings.scoreConfig.DecimalPrecision = (int)Math.Floor(c);
                    };
                    scorePrecision.FormatValue += delegate (float c) { return c.ToString(); };
                } },
            { CountersController.settings.progressConfig,
                v => {
                        var progressRank = v.AddBool("Show Time Left", "Starts the counter from the end of the song and decreases while the song is played.");
                    progressRank.GetValue += delegate { return CountersController.settings.progressConfig.ProgressTimeLeft; };
                    progressRank.SetValue += delegate (bool value) {
                        CountersController.settings.progressConfig.ProgressTimeLeft = value;
                    };
                    var progressMode = v.AddListSetting<ModeViewController>("Mode", determineModeText(CountersController.settings.progressConfig.Mode));
                    progressMode.values = progressSettings;
                    progressMode.GetValue = () => progressSettings.Where((Tuple<ICounterMode, string> x) => (x.Item1 == CountersController.settings.progressConfig.Mode)).FirstOrDefault();
                    progressMode.GetTextForValue = (value) => value.Item2;
                    progressMode.SetValue = c =>
                    {
                        CountersController.settings.progressConfig.Mode = c.Item1;
                    };
                } },
            { CountersController.settings.speedConfig,
                v => {
                    var speedPrecision = v.AddInt("Decimal Precision", "How precise will the counter go to?", 0, 5, 1);
                    speedPrecision.GetValue += delegate { return CountersController.settings.speedConfig.DecimalPrecision; };
                    speedPrecision.SetValue += delegate (int c) {
                        CountersController.settings.speedConfig.DecimalPrecision = c;
                    };
                    var speedMode = v.AddListSetting<ModeViewController>("Mode", determineModeText(CountersController.settings.speedConfig.Mode));
                    speedMode.values = speedSettings;
                    speedMode.GetValue = () => speedSettings.Where((Tuple<ICounterMode, string> x) => (x.Item1 == CountersController.settings.speedConfig.Mode)).FirstOrDefault();
                    speedMode.GetTextForValue = (value) => value.Item2;
                    speedMode.SetValue = c =>
                    {
                        CountersController.settings.speedConfig.Mode = c.Item1;
                    };
                } },
            { CountersController.settings.cutConfig, v => { } },
        };

        internal static Dictionary<CustomConfigModel, Action<SubMenu>> customCounterUIItems = new Dictionary<CustomConfigModel, Action<SubMenu>>()
        {
        };

        internal static void CreateSettingsUI()
        {
            if (Directory.Exists(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters"))
            {
                foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory.Replace('\\', '/') + $"/UserData/Custom Counters"))
                {
                    try
                    {
                        CustomConfigModel potential = JsonConvert.DeserializeObject<CustomConfigModel>(File.ReadAllText(file));
                        if (loadedCustoms.Where((CustomConfigModel x) => x.JSONName == potential.JSONName).Count() != 0) continue;
                        customCounterUIItems.Add(potential, (SubMenu v) =>
                        {
                            var delete = v.AddBool("Delete?", "Deletes the Custom Counter from the system.\nRelaunching the credited mod will regenerate the file.");
                            delete.GetValue += delegate { return false; };
                            delete.SetValue += delegate (bool c)
                            {
                                if (c) File.Delete(file);
                                CountersController.settings.saveCustom(potential);
                            };
                            var credits = v.AddListSetting<ModeViewController>("Credits", $"Created by: {potential.ModCreator}");
                            credits.values = creditSetting;
                            credits.GetValue = () => creditSetting.First();
                            credits.GetTextForValue = (value) => "Hover";
                            credits.SetValue = c => { };
                        });
                        loadedCustoms.Add(potential);
                    }catch{ Plugin.Log("Error loading Custom Counter. Ignoring..."); }
                }
            }

            //Main
            var mainSub = SettingsUI.CreateSubMenu("Counters+ | Main");
            var mainEnabled = mainSub.AddBool("Enabled", "Toggles the plugin on or off.");
            mainEnabled.GetValue += delegate { return CountersController.settings.Enabled; };
            mainEnabled.SetValue += delegate (bool value) {
                CountersController.settings.Enabled = value;
                Plugin.FlagConfigForReload(true);
            };

            if (!CountersController.settings.Enabled) return;

            var mainRNG = mainSub.AddBool("Random Counter Properties", "Add some RNG to the position and settings of some Counters.");
            mainRNG.GetValue += delegate { return CountersController.settings.RNG; };
            mainRNG.SetValue += delegate (bool value) {
                CountersController.settings.RNG = value;
            };

            var mainMenus = mainSub.AddBool("Disable Menus", "Removes clutter by removing all other Counters+ submenus while keeping Counters+ enabled.");
            mainMenus.GetValue += delegate { return CountersController.settings.DisableMenus; };
            mainMenus.SetValue += delegate (bool value) {
                CountersController.settings.DisableMenus = value;
            };

            if (CountersController.settings.DisableMenus) return;
            
            foreach(var kvp in counterUIItems)
            {
                kvp.Value(createBase(kvp.Key.DisplayName, kvp.Key));
            }
            foreach (var kvp in customCounterUIItems)
            {
                kvp.Value(createBase(kvp.Key.DisplayName, kvp.Key, kvp.Key.RestrictedPositions));
            }
        }

        internal static SubMenu createBase<T>(string name, T configItem, ICounterPositions[] restricted) where T : Config.IConfigModel
        {
            Plugin.Log("Creating base for: " + name);
            List<Tuple<ICounterPositions, string>> restrictedList = new List<Tuple<ICounterPositions, string>>();
            foreach(ICounterPositions pos in restricted)
            {
                restrictedList.Add(Tuple.Create(pos, positions.Where((Tuple<ICounterPositions, string> x) => x.Item1 == pos ).First().Item2));
            }
            var @base = SettingsUI.CreateSubMenu("Counters+ | " + name);
            var enabled = @base.AddBool("Enabled", "Toggles this counter on or off.");
            enabled.GetValue += () => configItem.Enabled;
            enabled.SetValue += v => configItem.Enabled = v;
            var position = @base.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            position.values = (restricted.Count() == 0) ? positions : restrictedList;
            position.GetValue = () => positions.Where((Tuple<Config.ICounterPositions, string> x) => (x.Item1 == configItem.Position)).FirstOrDefault();
            position.GetTextForValue = (value) => value.Item2;
            position.SetValue = v => configItem.Position = v.Item1;
            var index = @base.AddInt("Index", "How far from the position the counter will be. A higher number means farther away.", 0, 5, 1);
            index.GetValue += () => configItem.Index;
            index.SetValue += v => configItem.Index = v;
            return @base;
        }

        internal static SubMenu createBase<T>(string name, T configItem) where T : Config.IConfigModel
        {
            return createBase(name, configItem, new ICounterPositions[] { });
        }

        private static string determineModeText(ICounterMode Mode){
            string mode = "Unavilable mode!";
            switch(Mode){
                case ICounterMode.Average:
                    mode = "Mean Speed: Average speed of both sabers.";
                    break;
                case ICounterMode.Top5Sec:
                    mode = "Top: Fastest saber speed in the last 5 seconds.";
                    break;
                case ICounterMode.Both:
                    mode = "Both: A secondary Counter will be added so both Average and Top will be displayed.";
                    break;
                case ICounterMode.SplitAverage:
                    mode = "Split Mean: Displays averages for each saber, separately.";
                    break;
                case ICounterMode.SplitBoth:
                    mode = "Split Both: Displays both metrics, except the Average is split between two sabers.";
                    break;
                case ICounterMode.BaseGame:
                    mode = "Base Game: Uses the base game counter. Some settings will not apply in this mode.";
                    break;
                case ICounterMode.Original:
                    mode = "Original: Uses the original display mode, with a white circle bordering a time.";
                    break;
                case ICounterMode.Percent:
                    mode = "Percent : Displays a simple percent of the completed song. Some settings will not apply in this mode.";
                    break;
            }
            return "How should this Counter display data?\n" + mode;
        }
    }
}
