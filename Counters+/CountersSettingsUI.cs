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

            if (!CountersController.settings.Enabled) return;

            var mainRNG = mainSub.AddBool("Random Counter Properties", "Add some RNG to the position and settings of some Counters.");
            mainRNG.GetValue += delegate { return CountersController.settings.RNG; };
            mainRNG.SetValue += delegate (bool value) {
                CountersController.settings.RNG = value;
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
            /*var scoreOverride = scoreSub.AddBool("Override Base Game Counter");
            scoreOverride.GetValue += delegate { return CountersController.settings.scoreConfig.UseOld; };
            scoreOverride.SetValue += delegate (bool value) {
                CountersController.settings.scoreConfig.UseOld = value;
                CountersController.settings.save();
            };*/
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
            /*var progressOverride = progressSub.AddBool("Override Base Game Counter");
            progressOverride.GetValue += delegate { return CountersController.settings.progressConfig.UseOld; };
            progressOverride.SetValue += delegate (bool value) {
                CountersController.settings.progressConfig.UseOld = value;
                CountersController.settings.save();
            };*/

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

            var speedRank = speedSub.AddBool("Combine Saber Speeds", "Combine the speeds of the left and right saber into one average.");
            speedRank.GetValue += delegate { return CountersController.settings.speedConfig.CombinedSpeed; };
            speedRank.SetValue += delegate (bool value) {
                CountersController.settings.speedConfig.CombinedSpeed = value;
            };
            var speedPrecision = speedSub.AddInt("Decimal Precision", "How precise will the counter go to?", 0, 5, 1);
            speedPrecision.GetValue += delegate { return CountersController.settings.speedConfig.DecimalPrecision; };
            speedPrecision.SetValue += delegate (int v) {
                CountersController.settings.speedConfig.DecimalPrecision = v;
            };
        }

        /*
         *
         * So you may be thinking, why am I not using this perfectly good helper function?
         * 
         * Well, as it turns out, using this base function breaks settings saving, as I assume the T variable messes some things up.
         * Either way, the three main options would not save at all. The extras I added on after, however, did.
         * RIP trying to condense code. RIP readability.
         * 
         */
        static SubMenu createBase<T>(string name, T configItem) where T : Config.ConfigModel
        {
            var @base = SettingsUI.CreateSubMenu("Counters+ | " + name);
            var enabled = @base.AddBool("Enabled", "Toggles this counter on or off.");
            T item = configItem;
            enabled.GetValue += () => item.Enabled;
            enabled.SetValue += v =>
            {
                UpdateEnabled(name, configItem, v);
            };
            var position = @base.AddListSetting<PositionSettingsViewController>("Position", "The relative positions of common UI elements of which to go off of.");
            position.values = positions;
            position.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == item.Position)).FirstOrDefault();
            position.GetTextForValue = (value) => value.Item2;
            position.SetValue = v =>
            {
                UpdatePosition(name, configItem, v.Item1);
            };
            var index = @base.AddInt("Index", "How far from the position the counter will be. A higher number means farther away.", 0, 5, 1);
            index.GetValue += () => item.Index;
            index.SetValue += v =>
            {
                UpdateIndex(name, configItem, v);
            };
            configItem = item;
            return @base;
        }

        static async void UpdateIndex<T>(string name, T item, int v) where T : Config.ConfigModel
        {
            item.Index = v;
            CountersController.settings.save(name, item);
            CountersController.FlagConfigForReload(true);
        }
        static async void UpdateEnabled<T>(string name, T item, bool v) where T : Config.ConfigModel
        {
            item.Enabled = v;
            CountersController.settings.save(name, item);
        }
        static async void UpdatePosition<T>(string name, T item, Config.CounterPositions v) where T : Config.ConfigModel
        {
            item.Position = v;
            CountersController.settings.save(name, item);
        }
    }
}
