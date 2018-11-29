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

namespace CountersPlus
{
    class CountersSettingsUI : MonoBehaviour
    {
        public static SettingsNavigationController settings;
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
            };

            
            //Missed
            var missedSub = createBase("Missed", CountersController.settings.missedConfig);

            //Accuracy
            var accuracySub = createBase("Accuracy", CountersController.settings.accuracyConfig);

            var accuracyPercentage = accuracySub.AddBool("Show Percentage");
            accuracyPercentage.GetValue += delegate { return CountersController.settings.accuracyConfig.ShowPercentage; };
            accuracyPercentage.SetValue += delegate (bool value) {
                CountersController.settings.accuracyConfig.ShowPercentage = value;
            };
            float[] accuracyPrec = new float[6] { 0, 1, 2, 3, 4, 5 };
            var accuracyPrecision = accuracySub.AddList("Percentage Decimal Precision", accuracyPrec);
            accuracyPrecision.GetValue += delegate { return CountersController.settings.accuracyConfig.DecimalPrecision; };
            accuracyPrecision.SetValue += delegate (float v) {
                CountersController.settings.accuracyConfig.DecimalPrecision = (int)Math.Floor(v);
            };
            accuracyPrecision.FormatValue += delegate (float v) { return v.ToString(); };

            //Score
            var scoreSub = createBase("Score", CountersController.settings.scoreConfig);

            var scoreRank = scoreSub.AddBool("Display Rank");
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
            var scorePrecision = scoreSub.AddList("Percentage Decimal Precision", scorePrec);
            scorePrecision.GetValue += delegate { return CountersController.settings.scoreConfig.DecimalPrecision; };
            scorePrecision.SetValue += delegate (float v) {
                CountersController.settings.scoreConfig.DecimalPrecision = (int)Math.Floor(v);
            };
            scorePrecision.FormatValue += delegate (float v) { return v.ToString(); };

            //Progress
            var progressSub = createBase("Progress", CountersController.settings.progressConfig);

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
            enabled.GetValue += () => configItem.Enabled;
            enabled.SetValue += v =>
            {
                configItem.Enabled = v;
                CountersController.settings.save(name, configItem);
            };
            var position = @base.AddListSetting<PositionSettingsViewController>("Position");
            position.values = positions;
            position.GetValue = () => positions.Where((Tuple<Config.CounterPositions, string> x) => (x.Item1 == configItem.Position)).FirstOrDefault();
            position.GetTextForValue = (value) => value.Item2;
            position.SetValue = v =>
            {
                configItem.Position = v.Item1;
                CountersController.settings.save(name, configItem);
            };
            var index = @base.AddInt("Index", 0, 5, 1);
            index.GetValue += () => configItem.Index;
            index.SetValue += v =>
            {
                configItem.Index = v;
                CountersController.settings.save(name, configItem);
            };
            return @base;
        }
    }
}
