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
        public static MissedConfigModel missed = CountersController.settings.missedConfig;
        public static AccuracyConfigModel accuracy = CountersController.settings.accuracyConfig;
        public static ProgressConfigModel progress = CountersController.settings.progressConfig;
        public static ScoreConfigModel score = CountersController.settings.scoreConfig;
        public class PositionSettingsViewController : TupleViewController<Tuple<Config.CounterPositions, string>> { }
        static List<Tuple<Config.CounterPositions, string>> positions = new List<Tuple<Config.CounterPositions, string>> {
            {Config.CounterPositions.BelowCombo, "Below Combo" },
            {Config.CounterPositions.AboveCombo, "Above Combo" },
            {Config.CounterPositions.BelowMultiplier, "Below Multi." },
            {Config.CounterPositions.AboveMultiplier, "Above Multi." },
            {Config.CounterPositions.BelowEnergy, "Below Energy" },
        };


        public static void CreateSettingsUI()
        {
            //Main
            var mainSub = SettingsUI.CreateSubMenu("Counters+ | Main");
            var mainEnabled = mainSub.AddBool("Enabled");
            mainEnabled.GetValue += delegate { return CountersController.settings.Enabled; };
            mainEnabled.SetValue += delegate (bool value) {
                CountersController.settings.Enabled = value;
                CountersController.FlagConfigForReload();
            };

            if (!CountersController.settings.Enabled) return;

            //Missed
            var missedMenu = SettingsUI.CreateSubMenu("Counters+ | Missed");
            var enabled = missedMenu.AddBool("Enabled");
            enabled.GetValue += () => CountersController.settings.missedConfig.Enabled;
            enabled.SetValue += v =>
            {
                CountersController.settings.missedConfig.Enabled = v;
            };
            var missedPosition = missedMenu.AddListSetting<PositionSettingsViewController>("Position");
            missedPosition.values = positions;
            missedPosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.missedConfig.Position)).FirstOrDefault();
            missedPosition.GetTextForValue = (value) => value.Item2;
            missedPosition.SetValue = v =>
            {
                CountersController.settings.missedConfig.Position = v.Item1;
            };
            var missedIndex = missedMenu.AddInt("Index", 0, 5, 1);
            missedIndex.GetValue += () => CountersController.settings.missedConfig.Index;
            missedIndex.SetValue += v =>
            {
                CountersController.settings.missedConfig.Index = v;
            };

            //Accuracy
            var accuracySub = SettingsUI.CreateSubMenu("Counters+ | Accuracy");
            var accuracyEnabled = accuracySub.AddBool("Enabled");
            accuracyEnabled.GetValue += () => CountersController.settings.accuracyConfig.Enabled;
            accuracyEnabled.SetValue += v =>
            {
                CountersController.settings.accuracyConfig.Enabled = v;
            };
            var accuracyPosition = accuracySub.AddListSetting<PositionSettingsViewController>("Position");
            accuracyPosition.values = positions;
            accuracyPosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.accuracyConfig.Position)).FirstOrDefault();
            accuracyPosition.GetTextForValue = (value) => value.Item2;
            accuracyPosition.SetValue = v =>
            {
                CountersController.settings.accuracyConfig.Position = v.Item1;
            };
            var accuracyIndex = accuracySub.AddInt("Index", 0, 5, 1);
            accuracyIndex.GetValue += () => CountersController.settings.accuracyConfig.Index;
            accuracyIndex.SetValue += v =>
            {
                CountersController.settings.accuracyConfig.Index = v;
            };

            var accuracyPercentage = accuracySub.AddBool("Show Percentage");
            accuracyPercentage.GetValue += delegate { return accuracy.ShowPercentage; };
            accuracyPercentage.SetValue += delegate (bool value) {
                CountersController.settings.accuracyConfig.ShowPercentage = value;
            };
            float[] accuracyPrec = new float[6] { 0, 1, 2, 3, 4, 5 };
            var accuracyPrecision = accuracySub.AddList("Percentage Decimal Precision", accuracyPrec);
            accuracyPrecision.GetValue += delegate { return accuracy.DecimalPrecision; };
            accuracyPrecision.SetValue += delegate (float v) {
                CountersController.settings.accuracyConfig.DecimalPrecision = (int)Math.Floor(v);
            };
            accuracyPrecision.FormatValue += delegate (float v) { return v.ToString(); };

            //Score
            var scoreSub = SettingsUI.CreateSubMenu("Counters+ | Score");
            var scoreEnabled = scoreSub.AddBool("Enabled");
            scoreEnabled.GetValue += () => CountersController.settings.scoreConfig.Enabled;
            scoreEnabled.SetValue += v =>
            {
                CountersController.settings.scoreConfig.Enabled = v;
            };
            var scorePosition = scoreSub.AddListSetting<PositionSettingsViewController>("Position");
            scorePosition.values = positions;
            scorePosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.scoreConfig.Position)).FirstOrDefault();
            scorePosition.GetTextForValue = (value) => value.Item2;
            scorePosition.SetValue = v =>
            {
                CountersController.settings.scoreConfig.Position = v.Item1;
            };
            var scoreIndex = scoreSub.AddInt("Index", 0, 5, 1);
            scoreIndex.GetValue += () => CountersController.settings.scoreConfig.Index;
            scoreIndex.SetValue += v =>
            {
                CountersController.settings.scoreConfig.Index = v;
            };

            var scoreRank = scoreSub.AddBool("Display Rank");
            scoreRank.GetValue += delegate { return score.DisplayRank; };
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
            var scorePrecision = scoreSub.AddList("Percentage Decimal Precision", scorePrec);
            scorePrecision.GetValue += delegate { return score.DecimalPrecision; };
            scorePrecision.SetValue += delegate (float v) {
                CountersController.settings.scoreConfig.DecimalPrecision = (int)Math.Floor(v);
            };
            scorePrecision.FormatValue += delegate (float v) { return v.ToString(); };

            //Progress
            var progressSub = SettingsUI.CreateSubMenu("Counters+ | Progress");
            var progressEnabled = progressSub.AddBool("Enabled");
            progressEnabled.GetValue += () => CountersController.settings.progressConfig.Enabled;
            progressEnabled.SetValue += v =>
            {
                CountersController.settings.progressConfig.Enabled = v;
            };
            var progressPosition = progressSub.AddListSetting<PositionSettingsViewController>("Position");
            progressPosition.values = positions;
            progressPosition.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == CountersController.settings.progressConfig.Position)).FirstOrDefault();
            progressPosition.GetTextForValue = (value) => value.Item2;
            progressPosition.SetValue = v =>
            {
                CountersController.settings.progressConfig.Position = v.Item1;
            };
            var progressIndex = progressSub.AddInt("Index", 0, 5, 1);
            progressIndex.GetValue += () => CountersController.settings.progressConfig.Index;
            progressIndex.SetValue += v =>
            {
                CountersController.settings.progressConfig.Index = v;
            };

            var progressRank = progressSub.AddBool("Show Time Left");
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
        }

        static SubMenu createBase<T>(string name, T configItem) where T : Config.ConfigModel
        {
            var @base = SettingsUI.CreateSubMenu("Counters+ | " + name);
            var enabled = @base.AddBool("Enabled");
            T item = configItem;
            enabled.GetValue += () => item.Enabled;
            enabled.SetValue += v =>
            {
                UpdateEnabled(name, configItem, v);
            };
            var position = @base.AddListSetting<PositionSettingsViewController>("Position");
            position.values = positions;
            position.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == item.Position)).FirstOrDefault();
            position.GetTextForValue = (value) => value.Item2;
            position.SetValue = v =>
            {
                UpdatePosition(name, configItem, v.Item1);
            };
            var index = @base.AddInt("Index", 0, 5, 1);
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
            CountersController.FlagConfigForReload();
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
