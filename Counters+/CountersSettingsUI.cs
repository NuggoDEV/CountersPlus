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
        public class SpeedModeViewController : TupleViewController<Tuple<SpeedConfigModel.CounterMode, string>> { }
        static List<Tuple<SpeedConfigModel.CounterMode, string>> mode = new List<Tuple<SpeedConfigModel.CounterMode, string>> {
            {SpeedConfigModel.CounterMode.Average, "Mean Speed" },
            {SpeedConfigModel.CounterMode.Top5Sec, "Top (5 Sec.)" },
            {SpeedConfigModel.CounterMode.Both, "Both" },
            {SpeedConfigModel.CounterMode.SplitAverage, "Split Mean" },
            {SpeedConfigModel.CounterMode.SplitBoth, "Both w/Split" },
        };


        public static void CreateSettingsUI()
        {
            //Main
            var mainSub = SettingsUI.CreateSubMenu("Counters+ | Main");
            var mainEnabled = mainSub.AddBool("Enabled", "Toggles the plugin on or off.");
            mainEnabled.GetValue += delegate { return CountersController.settings.Enabled; };
            mainEnabled.SetValue += delegate (bool value) {
                CountersController.settings.Enabled = value;
                CountersController.FlagConfigForReload(true);
            };

            if (!CountersController.settings.Enabled || CountersController.settings.DisableMenus) return;

            var mainRNG = mainSub.AddBool("Random Counter Properties", "Add some RNG to the position and settings of some Counters.");
            mainRNG.GetValue += delegate { return CountersController.settings.RNG; };
            mainRNG.SetValue += delegate (bool value) {
                CountersController.settings.RNG = value;
            };

            var mainDisable = mainSub.AddBool("Disable Menus", "Removes clutter in the Settings option by removing all settings options, while keeping the Counters enabled.");
            mainRNG.GetValue += delegate { return CountersController.settings.DisableMenus; };
            mainRNG.SetValue += delegate (bool value) {
                CountersController.settings.DisableMenus = value;
            };

            //Missed
            var missedMenu = SettingsUI.CreateSubMenu("Counters+ | Missed");
            var enabled = missedMenu.AddBool("Enabled", "Toggles this counter on or off.");
            enabled.GetValue += () => CountersController.settings.missedConfig.Enabled;
            enabled.SetValue += v =>
            {
                CountersController.settings.missedConfig.Enabled = v;
            };
            var missedPosition = missedMenu.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            missedPosition.values = positions;
            missedPosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.missedConfig.Position)).FirstOrDefault();
            missedPosition.GetTextForValue = (value) => value.Item2;
            missedPosition.SetValue = v =>
            {
                CountersController.settings.missedConfig.Position = v.Item1;
            };
            var missedIndex = missedMenu.AddInt("Index", "How far from the position the counter will be. A higher number means farther away.", 0, 5, 1);
            missedIndex.GetValue += () => CountersController.settings.missedConfig.Index;
            missedIndex.SetValue += v =>
            {
                CountersController.settings.missedConfig.Index = v;
            };

            //Accuracy
            var accuracySub = SettingsUI.CreateSubMenu("Counters+ | Notes");
            var accuracyEnabled = accuracySub.AddBool("Enabled", "Toggles this counter on or off.");
            accuracyEnabled.GetValue += () => CountersController.settings.accuracyConfig.Enabled;
            accuracyEnabled.SetValue += v =>
            {
                CountersController.settings.accuracyConfig.Enabled = v;
            };
            var accuracyPosition = accuracySub.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            accuracyPosition.values = positions;
            accuracyPosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.accuracyConfig.Position)).FirstOrDefault();
            accuracyPosition.GetTextForValue = (value) => value.Item2;
            accuracyPosition.SetValue = v =>
            {
                CountersController.settings.accuracyConfig.Position = v.Item1;
            };
            var accuracyIndex = accuracySub.AddInt("Index", "How far from the position the counter will be. A higher number means farther away.", 0, 5, 1);
            accuracyIndex.GetValue += () => CountersController.settings.accuracyConfig.Index;
            accuracyIndex.SetValue += v =>
            {
                CountersController.settings.accuracyConfig.Index = v;
            };

            var accuracyPercentage = accuracySub.AddBool("Show Percentage", "Toggles the percentage of notes hit over total notes.");
            accuracyPercentage.GetValue += delegate { return CountersController.settings.accuracyConfig.ShowPercentage; };
            accuracyPercentage.SetValue += delegate (bool value) {
                CountersController.settings.accuracyConfig.ShowPercentage = value;
            };
            var accuracyPrecision = accuracySub.AddInt("Percentage Decimal Precision", "How accuracy the percentage will be.", 0, 5, 1);
            accuracyPrecision.GetValue += delegate { return CountersController.settings.accuracyConfig.DecimalPrecision; };
            accuracyPrecision.SetValue += delegate (int v) {
                CountersController.settings.accuracyConfig.DecimalPrecision = v;
            };

            //Score
            var scoreSub = SettingsUI.CreateSubMenu("Counters+ | Score");
            var scoreEnabled = scoreSub.AddBool("Enabled", "Toggles this counter on or off.");
            scoreEnabled.GetValue += () => CountersController.settings.scoreConfig.Enabled;
            scoreEnabled.SetValue += v =>
            {
                CountersController.settings.scoreConfig.Enabled = v;
            };
            var scorePosition = scoreSub.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            scorePosition.values = positions;
            scorePosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.scoreConfig.Position)).FirstOrDefault();
            scorePosition.GetTextForValue = (value) => value.Item2;
            scorePosition.SetValue = v =>
            {
                CountersController.settings.scoreConfig.Position = v.Item1;
            };
            var scoreIndex = scoreSub.AddInt("Index", "How far from the position the counter will be. A higher number means farther away.", 0, 5, 1);
            scoreIndex.GetValue += () => CountersController.settings.scoreConfig.Index;
            scoreIndex.SetValue += v =>
            {
                CountersController.settings.scoreConfig.Index = v;
            };

            var scoreRank = scoreSub.AddBool("Display Rank", "Displays the rank as you progress in a song.");
            scoreRank.GetValue += delegate { return CountersController.settings.scoreConfig.DisplayRank; };
            scoreRank.SetValue += delegate (bool value) {
                CountersController.settings.scoreConfig.DisplayRank = value;
            };
            var scoreOverride = scoreSub.AddBool("Override Base Game Counter");
            scoreOverride.GetValue += delegate { return CountersController.settings.scoreConfig.UseOld; };
            scoreOverride.SetValue += delegate (bool value) {
                CountersController.settings.scoreConfig.UseOld = value;
            };
            float[] scorePrec = new float[6] { 0, 1, 2, 3, 4, 5 };
            var scorePrecision = scoreSub.AddList("Percentage Decimal Precision", scorePrec, "How accuracy the percentage will be.");
            scorePrecision.GetValue += delegate { return CountersController.settings.scoreConfig.DecimalPrecision; };
            scorePrecision.SetValue += delegate (float v) {
                CountersController.settings.scoreConfig.DecimalPrecision = (int)Math.Floor(v);
            };
            scorePrecision.FormatValue += delegate (float v) { return v.ToString(); };

            //Progress
            var progressSub = SettingsUI.CreateSubMenu("Counters+ | Progress");
            var progressEnabled = progressSub.AddBool("Enabled", "Toggles this counter on or off.");
            progressEnabled.GetValue += () => CountersController.settings.progressConfig.Enabled;
            progressEnabled.SetValue += v =>
            {
                CountersController.settings.progressConfig.Enabled = v;
            };
            var progressPosition = progressSub.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            progressPosition.values = positions;
            progressPosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.progressConfig.Position)).FirstOrDefault();
            progressPosition.GetTextForValue = (value) => value.Item2;
            progressPosition.SetValue = v =>
            {
                CountersController.settings.progressConfig.Position = v.Item1;
            };
            var progressIndex = progressSub.AddInt("Index", "How far from the position the counter will be. A higher number means farther away.", 0, 5, 1);
            progressIndex.GetValue += () => CountersController.settings.progressConfig.Index;
            progressIndex.SetValue += v =>
            {
                CountersController.settings.progressConfig.Index = v;
            };

            var progressRank = progressSub.AddBool("Show Time Left", "Starts the counter from the end of the song and decreases while the song is played.");
            progressRank.GetValue += delegate { return CountersController.settings.progressConfig.ProgressTimeLeft; };
            progressRank.SetValue += delegate (bool value) {
                CountersController.settings.progressConfig.ProgressTimeLeft = value;
            };
            var progressOverride = progressSub.AddBool("Override Base Game Counter");
            progressOverride.GetValue += delegate { return CountersController.settings.progressConfig.UseOld; };
            progressOverride.SetValue += delegate (bool value) {
                CountersController.settings.progressConfig.UseOld = value;
            };

            //Speed
            var speedSub = SettingsUI.CreateSubMenu("Counters+ | Speed");
            var speedEnabled = speedSub.AddBool("Enabled", "Toggles this counter on or off.");
            speedEnabled.GetValue += () => CountersController.settings.speedConfig.Enabled;
            speedEnabled.SetValue += v =>
            {
                CountersController.settings.speedConfig.Enabled = v;
            };
            var speedPosition = speedSub.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            speedPosition.values = positions;
            speedPosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.speedConfig.Position)).FirstOrDefault();
            speedPosition.GetTextForValue = (value) => value.Item2;
            speedPosition.SetValue = v =>
            {
                CountersController.settings.speedConfig.Position = v.Item1;
            };
            var speedIndex = speedSub.AddInt("Index", "How far from the position the counter will be. A higher number means farther away.", 0, 5, 1);
            speedIndex.GetValue += () => CountersController.settings.speedConfig.Index;
            speedIndex.SetValue += v =>
            {
                CountersController.settings.speedConfig.Index = v;
            };
            var speedPrecision = speedSub.AddInt("Decimal Precision", "How precise will the counter go to?", 0, 5, 1);
            speedPrecision.GetValue += delegate { return CountersController.settings.speedConfig.DecimalPrecision; };
            speedPrecision.SetValue += delegate (int v) {
                CountersController.settings.speedConfig.DecimalPrecision = v;
            };
            var speedMode = speedSub.AddListSetting<SpeedModeViewController>("Mode", determineModeText());
            speedMode.values = mode;
            speedMode.GetValue = () => mode.Where((Tuple<SpeedConfigModel.CounterMode, string> x) => (x.Item1 == CountersController.settings.speedConfig.Mode)).FirstOrDefault();
            speedMode.GetTextForValue = (value) => value.Item2;
            speedMode.SetValue = v =>
            {
                CountersController.settings.speedConfig.Mode = v.Item1;
            };

            //Cut
            var cutMenu = SettingsUI.CreateSubMenu("Counters+ | Cut");
            var cutEnabled = cutMenu.AddBool("Enabled", "Toggles this counter on or off.");
            cutEnabled.GetValue += () => CountersController.settings.cutConfig.Enabled;
            cutEnabled.SetValue += v =>
            {
                CountersController.settings.cutConfig.Enabled = v;
            };
            var cutPosition = cutMenu.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            cutPosition.values = positions;
            cutPosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.cutConfig.Position)).FirstOrDefault();
            cutPosition.GetTextForValue = (value) => value.Item2;
            cutPosition.SetValue = v =>
            {
                CountersController.settings.cutConfig.Position = v.Item1;
            };
            var cutIndex = cutMenu.AddInt("Index", "How far from the position the counter will be. A higher number means farther away.", 0, 5, 1);
            cutIndex.GetValue += () => CountersController.settings.cutConfig.Index;
            cutIndex.SetValue += v =>
            {
                CountersController.settings.cutConfig.Index = v;
            };
        }

        private static string determineModeText(){
            string mode = "Unavilable mode!";
            switch(CountersController.settings.speedConfig.Mode){
                case SpeedConfigModel.CounterMode.Average:
                    mode = "Mean Speed: Average speed of both sabers.";
                    break;
                case SpeedConfigModel.CounterMode.Top5Sec:
                    mode = "Top: Fastest saber speed in the last 5 seconds.";
                    break;
                case SpeedConfigModel.CounterMode.Both:
                    mode = "Both: A secondary Counter will be added so both Average and Top will be displayed.";
                    break;
                case SpeedConfigModel.CounterMode.SplitAverage:
                    mode = "Split Mean: Displays averages for each saber, separately.";
                    break;
                case SpeedConfigModel.CounterMode.SplitBoth:
                    mode = "Split Both: Displays both metrics, except the Average is split between two sabers.";
                    break;
            }
            return "How should this Counter display data?\n" + mode;
        }
    }
}
