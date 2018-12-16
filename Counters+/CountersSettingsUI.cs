using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HMUI;
using VRUI;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;
using VRUIControls;
using CustomUI.Settings;
using CountersPlus.Config;
using System.Threading;

namespace CountersPlus
{
    class CountersSettingsUI : MonoBehaviour
    {
        public class PositionSettingsViewController : TupleViewController<Tuple<Config.CounterPositions, string>> { }
        static List<Tuple<Config.CounterPositions, string>> positions = new List<Tuple<Config.CounterPositions, string>> {
            {Config.CounterPositions.BelowCombo, "Below Combo" },
            {Config.CounterPositions.AboveCombo, "Above Combo" },
            {Config.CounterPositions.BelowMultiplier, "Below Multi." },
            {Config.CounterPositions.AboveMultiplier, "Above Multi." },
            {Config.CounterPositions.BelowEnergy, "Below Energy" },
            {Config.CounterPositions.AboveHighway, "Over Highway" }
        };
        public class ModeViewController : TupleViewController<Tuple<CounterMode, string>> { }
        static List<Tuple<CounterMode, string>> speedSettings = new List<Tuple<CounterMode, string>> {
            {CounterMode.Average, "Mean Speed" },
            {CounterMode.Top5Sec, "Top (5 Sec.)" },
            {CounterMode.Both, "Both" },
            {CounterMode.SplitAverage, "Split Mean" },
            {CounterMode.SplitBoth, "Both w/Split" }
        };
        static List<Tuple<CounterMode, string>> progressSettings = new List<Tuple<CounterMode, string>> {
            {CounterMode.BaseGame, "Base Game" },
            {CounterMode.Original, "Original" },
            {CounterMode.Percent, "Percentage" }
        };

        public static void CreateSettingsUI()
        {
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

            var items = new Dictionary<string, Tuple<ConfigModel, Action<SubMenu>>>()
            {
                {
                    "Missed",
                    Tuple.Create<ConfigModel,Action<SubMenu>>(
                        CountersController.settings.missedConfig,
                        v =>{ }) },
                {
                    "Notes",
                    Tuple.Create<ConfigModel,Action<SubMenu>>(
                        CountersController.settings.accuracyConfig,
                        v => {
                             var accuracyPercentage = v.AddBool("Show Percentage", "Toggles the percentage of notes hit over total notes.");
                            accuracyPercentage.GetValue += delegate { return CountersController.settings.accuracyConfig.ShowPercentage; };
                            accuracyPercentage.SetValue += delegate (bool value) {
                                CountersController.settings.accuracyConfig.ShowPercentage = value;
                            };
                            var accuracyPrecision = v.AddInt("Percentage Decimal Precision", "How accuracy the percentage will be.", 0, 5, 1);
                            accuracyPrecision.GetValue += delegate { return CountersController.settings.accuracyConfig.DecimalPrecision; };
                            accuracyPrecision.SetValue += delegate (int c) {
                                CountersController.settings.accuracyConfig.DecimalPrecision = c;
                            };
                        }) },
                {
                    "Score",
                    Tuple.Create<ConfigModel,Action<SubMenu>>(
                        CountersController.settings.scoreConfig,
                        v => {
                             var scoreRank = v.AddBool("Display Rank", "Displays the rank as you progress in a song.");
                            scoreRank.GetValue += delegate { return CountersController.settings.scoreConfig.DisplayRank; };
                            scoreRank.SetValue += delegate (bool value) {
                                CountersController.settings.scoreConfig.DisplayRank = value;
                            };
                            var scoreOverride = v.AddBool("Override Base Game Counter");
                            scoreOverride.GetValue += delegate { return CountersController.settings.scoreConfig.UseOld; };
                            scoreOverride.SetValue += delegate (bool value) {
                                CountersController.settings.scoreConfig.UseOld = value;
                            };
                            float[] scorePrec = new float[6] { 0, 1, 2, 3, 4, 5 };
                            var scorePrecision = v.AddList("Percentage Decimal Precision", scorePrec, "How accuracy the percentage will be.");
                            scorePrecision.GetValue += delegate { return CountersController.settings.scoreConfig.DecimalPrecision; };
                            scorePrecision.SetValue += delegate (float c) {
                                CountersController.settings.scoreConfig.DecimalPrecision = (int)Math.Floor(c);
                            };
                            scorePrecision.FormatValue += delegate (float c) { return c.ToString(); };
                        }) },
                {
                    "Progress",
                    Tuple.Create<ConfigModel,Action<SubMenu>>(
                        CountersController.settings.progressConfig,
                        v => {
                             var progressRank = v.AddBool("Show Time Left", "Starts the counter from the end of the song and decreases while the song is played.");
                            progressRank.GetValue += delegate { return CountersController.settings.progressConfig.ProgressTimeLeft; };
                            progressRank.SetValue += delegate (bool value) {
                                CountersController.settings.progressConfig.ProgressTimeLeft = value;
                            };
                            var progressMode = v.AddListSetting<ModeViewController>("Mode", determineModeText(CountersController.settings.progressConfig.Mode));
                            progressMode.values = progressSettings;
                            progressMode.GetValue = () => progressSettings.Where((Tuple<CounterMode, string> x) => (x.Item1 == CountersController.settings.progressConfig.Mode)).FirstOrDefault();
                            progressMode.GetTextForValue = (value) => value.Item2;
                            progressMode.SetValue = c =>
                            {
                                CountersController.settings.progressConfig.Mode = c.Item1;
                            };
                        }) },
                {
                    "Speed",
                    Tuple.Create<ConfigModel,Action<SubMenu>>(
                        CountersController.settings.speedConfig,
                        v => {
                            var speedPrecision = v.AddInt("Decimal Precision", "How precise will the counter go to?", 0, 5, 1);
                            speedPrecision.GetValue += delegate { return CountersController.settings.speedConfig.DecimalPrecision; };
                            speedPrecision.SetValue += delegate (int c) {
                                CountersController.settings.speedConfig.DecimalPrecision = c;
                            };
                            var speedMode = v.AddListSetting<ModeViewController>("Mode", determineModeText(CountersController.settings.speedConfig.Mode));
                            speedMode.values = speedSettings;
                            speedMode.GetValue = () => speedSettings.Where((Tuple<CounterMode, string> x) => (x.Item1 == CountersController.settings.speedConfig.Mode)).FirstOrDefault();
                            speedMode.GetTextForValue = (value) => value.Item2;
                            speedMode.SetValue = c =>
                            {
                                CountersController.settings.speedConfig.Mode = c.Item1;
                            };
                        }) },
                {
                    "Cut",
                    Tuple.Create<ConfigModel,Action<SubMenu>>(
                        CountersController.settings.cutConfig,
                        v =>{ }) },
            };
            foreach(var kvp in items)
            {
                kvp.Value.Item2(createBase(kvp.Key, kvp.Value.Item1));
            }
        }

        static SubMenu createBase<T>(string name, T configItem) where T : Config.ConfigModel
        {
            Plugin.Log("Creating base for: " + name);
            var @base = SettingsUI.CreateSubMenu("Counters+ | " + name);
            var enabled = @base.AddBool("Enabled", "Toggles this counter on or off.");
            enabled.GetValue += () => configItem.Enabled;
            enabled.SetValue += v => configItem.Enabled = v;
            var position = @base.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            position.values = positions;
            position.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == configItem.Position)).FirstOrDefault();
            position.GetTextForValue = (value) => value.Item2;
            position.SetValue = v => configItem.Position = v.Item1;
            var index = @base.AddInt("Index", "How far from the position the counter will be. A higher number means farther away.", 0, 5, 1);
            index.GetValue += () => configItem.Index;
            index.SetValue += v => configItem.Index = v;
            return @base;
        }

        private static string determineModeText(CounterMode Mode){
            string mode = "Unavilable mode!";
            switch(Mode){
                case CounterMode.Average:
                    mode = "Mean Speed: Average speed of both sabers.";
                    break;
                case CounterMode.Top5Sec:
                    mode = "Top: Fastest saber speed in the last 5 seconds.";
                    break;
                case CounterMode.Both:
                    mode = "Both: A secondary Counter will be added so both Average and Top will be displayed.";
                    break;
                case CounterMode.SplitAverage:
                    mode = "Split Mean: Displays averages for each saber, separately.";
                    break;
                case CounterMode.SplitBoth:
                    mode = "Split Both: Displays both metrics, except the Average is split between two sabers.";
                    break;
                case CounterMode.BaseGame:
                    mode = "Base Game: Uses the base game counter. Some settings will not apply in this mode.";
                    break;
                case CounterMode.Original:
                    mode = "Original: Uses the original display mode, with a white circle bordering a time.";
                    break;
                case CounterMode.Percent:
                    mode = "Percent : Displays a simple percent of the completed song. Some settings will not apply in this mode.";
                    break;
            }
            return "How should this Counter display data?\n" + mode;
        }
    }
}
