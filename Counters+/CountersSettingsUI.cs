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
using System.Collections;
using IniParser;
using IniParser.Model;
using IllusionInjector;

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
            { CountersController.settings.noteConfig,
                v => {
                        var accuracyPercentage = v.AddBool("Show Percentage", "Toggles the percentage of notes hit over total notes.");
                    accuracyPercentage.GetValue += delegate { return CountersController.settings.noteConfig.ShowPercentage; };
                    accuracyPercentage.SetValue += delegate (bool value) {
                        CountersController.settings.noteConfig.ShowPercentage = value;
                    };
                    var accuracyPrecision = v.AddInt("Percentage Decimal Precision", "How precise should the percentage be?", 0, 5, 1);
                    accuracyPrecision.GetValue += delegate { return CountersController.settings.noteConfig.DecimalPrecision; };
                    accuracyPrecision.SetValue += delegate (int c) {
                        CountersController.settings.noteConfig.DecimalPrecision = c;
                    };
                } },
            { CountersController.settings.scoreConfig,
                v => {
                        var scoreRank = v.AddBool("Display Rank", "Displays the rank as you progress in a song.");
                    scoreRank.GetValue += delegate { return CountersController.settings.scoreConfig.DisplayRank; };
                    scoreRank.SetValue += delegate (bool value) {
                        CountersController.settings.scoreConfig.DisplayRank = value;
                    };
                    var scoreOverride = v.AddBool("Use Base Game Counter", "Uses the base game counter instead of a custom one.\n<color=#ff0000>Some settings will not apply in this mode.</color>");
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
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            customCounterUIItems.Clear();
            loadedCustoms.Clear();
            foreach (SectionData section in data.Sections)
            {
                if (section.Keys.Any((KeyData x) => x.KeyName == "SectionName"))
                {
                    Plugin.Log(section.SectionName);
                    CustomConfigModel potential = new CustomConfigModel(section.SectionName);
                    if (!PluginManager.Plugins.Any((IPlugin x) => x.Name == section.Keys["ModCreator"])) continue;
                    customCounterUIItems.Add(potential, (SubMenu v) =>
                    {
                        var delete = v.AddBool("Delete?", "Deletes the Custom Counter from the system when you apply settings.\n<color=#FF0000>Relaunching the credited mod will regenerate the file.</color>\nAnother way to remove the effects of a Custom Counter is by simply disabling it.");
                        delete.GetValue += delegate { return false; };
                        delete.SetValue += delegate (bool c)
                        {
                            if (c) data.Sections.RemoveSection(section.SectionName);
                        };
                        var credits = v.AddListSetting<ModeViewController>("Credits", $"Created by: {potential.ModCreator}");
                        credits.values = creditSetting;
                        credits.GetValue = () => creditSetting.First();
                        credits.GetTextForValue = (value) => "Hover";
                        credits.SetValue = c => { };
                    });
                    loadedCustoms.Add(potential);
                }
            }

            //Main
            var mainSub = SettingsUI.CreateSubMenu("Counters+ | Main");
            var mainEnabled = mainSub.AddBool("Enabled", "Toggles the plugin on or off.");
            mainEnabled.GetValue += delegate { return CountersController.settings.Enabled; };
            mainEnabled.SetValue += delegate (bool value) {
                CountersController.settings.Enabled = value;
            };

            if (!CountersController.settings.Enabled) return;

            var mainMenus = mainSub.AddBool("Disable Menus", "Removes clutter by removing all other Counters+ submenus while keeping Counters+ enabled.");
            mainMenus.GetValue += delegate { return CountersController.settings.DisableMenus; };
            mainMenus.SetValue += delegate (bool value) {
                CountersController.settings.DisableMenus = value;
            };


            float[] offsetValues = new float[] { 0, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1};
            var comboOffset = mainSub.AddList("Combo Offset", offsetValues, "Before Index is taken into account, how far should counters be from the Combo counter.");
            comboOffset.GetValue += delegate { return CountersController.settings.ComboOffset; };
            comboOffset.SetValue += delegate (float f) { CountersController.settings.ComboOffset = f; };
            comboOffset.GetTextForValue += delegate (float f) { return f.ToString(); };

            var multiplierOffset = mainSub.AddList("Multiplier Offset", offsetValues, "Before Index is taken into account, how far should counters be from the Multiplier counter.");
            multiplierOffset.GetValue += delegate { return CountersController.settings.MultiplierOffset; };
            multiplierOffset.SetValue += delegate (float f) { CountersController.settings.MultiplierOffset = f; };
            multiplierOffset.GetTextForValue += delegate (float f) { return f.ToString(); };

            if (CountersController.settings.DisableMenus) return;

            foreach (var kvp in counterUIItems)
            {
                try
                {
                    kvp.Value(createBase(kvp.Key.DisplayName, kvp.Key));
                }
                catch { }
            }
            foreach (var kvp in customCounterUIItems)
            {
                try
                {
                    kvp.Value(createBase(kvp.Key.DisplayName, kvp.Key, kvp.Key.RestrictedPositions));
                }
                catch(Exception e) { Plugin.Log(e.ToString()); }
            }
        }

        internal static SubMenu createBase<T>(string name, T configItem, params ICounterPositions[] restricted) where T : Config.IConfigModel
        {
            Plugin.Log("Creating base for: " + name);
            List<Tuple<ICounterPositions, string>> restrictedList = new List<Tuple<ICounterPositions, string>>();
            try
            {
                foreach (ICounterPositions pos in restricted)
                {
                    restrictedList.Add(Tuple.Create(pos, positions.Where((Tuple<ICounterPositions, string> x) => x.Item1 == pos).First().Item2));
                }
            }
            catch { } //It most likely errors here. If it does, well no problem.
            var @base = SettingsUI.CreateSubMenu("Counters+ | " + name);
            var enabled = @base.AddBool("Enabled", "Toggles this counter on or off.");
            enabled.GetValue += () => configItem.Enabled;
            enabled.SetValue += v => configItem.Enabled = v;
            var position = @base.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            position.values = (restrictedList.Count() == 0) ? positions : restrictedList;
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
                    mode = "Both: A secondary Counter will be added so both Average and Top Speed will be displayed.";
                    break;
                case ICounterMode.SplitAverage:
                    mode = "Split Mean: Displays averages for each saber, separately.";
                    break;
                case ICounterMode.SplitBoth:
                    mode = "Split Both: Displays both metrics, except the Average is split between two sabers.";
                    break;
                case ICounterMode.BaseGame:
                    mode = "Base Game: Uses the base game counter.\n<color=#FF0000>Some settings will not apply in this mode.</color>";
                    break;
                case ICounterMode.Original:
                    mode = "Original: Uses the original display mode, with a white circle bordering a time.";
                    break;
                case ICounterMode.Percent:
                    mode = "Percent: Displays a simple percent of the completed song.\n<color=#FF0000>Some settings will not apply in this mode.</color>";
                    break;
            }
            return "How should this Counter display data?\n" + mode;
        }
    }
}
