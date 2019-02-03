using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CountersPlus.Config;
using CustomUI.BeatSaber;
using CustomUI.Settings;
using UnityEngine;

namespace CountersPlus.UI
{
    public class AdvancedCounterSettings
    {
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

        private static HoverHint progressHover;
        private static HoverHint speedHover;

        internal static Dictionary<IConfigModel, Action<SubMenu>> counterUIItems = new Dictionary<IConfigModel, Action<SubMenu>>()
        {
            { CountersController.settings.missedConfig, v => { } },
            { CountersController.settings.noteConfig,
                sub => {
                    var accuracyPercentage = CountersPlusEditViewController.AddList(ref sub, "Show Percentage", "Toggles the percentage of notes hit over total notes.", 2);
                    accuracyPercentage.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                    accuracyPercentage.GetValue = () => CountersController.settings.noteConfig.ShowPercentage ? 1f : 0f;
                    accuracyPercentage.SetValue = (v) => CountersController.settings.noteConfig.ShowPercentage = v != 0f;

                    var accuracyPrecision = CountersPlusEditViewController.AddList(ref sub, "Percentage Precision", "How precise should the precentage be?", 6);
                    accuracyPrecision.GetTextForValue = (v) => Mathf.RoundToInt(v).ToString();
                    accuracyPrecision.GetValue = () => CountersController.settings.noteConfig.DecimalPrecision;
                    accuracyPrecision.SetValue = (v) => CountersController.settings.noteConfig.DecimalPrecision = Mathf.RoundToInt(v);
                } },
            { CountersController.settings.scoreConfig,
                sub => {
                        var scoreRank = CountersPlusEditViewController.AddList(ref sub, "Display Rank", "Displays the rank as you progress in a song.", 2);
                    scoreRank.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                    scoreRank.GetValue = () => CountersController.settings.scoreConfig.DisplayRank ? 1f : 0f;
                    scoreRank.SetValue = (v) => CountersController.settings.scoreConfig.DisplayRank = v != 0f;

                    var scoreOverride = CountersPlusEditViewController.AddList(ref sub, "Use Base Game Counter", "Uses the base game counter instead of a custom one.\n<color=#ff0000>Some settings will not apply in this mode.</color>", 2);
                    scoreOverride.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                    scoreOverride.GetValue = () => CountersController.settings.scoreConfig.UseOld ? 1f : 0f;
                    scoreOverride.SetValue = (v) => CountersController.settings.scoreConfig.UseOld = v != 0f;
                    
                    var scorePrecision = CountersPlusEditViewController.AddList(ref sub, "Percentage Precision", "How precise should the precentage be?", 6);
                    scorePrecision.GetTextForValue = (v) => Mathf.RoundToInt(v).ToString();
                    scorePrecision.GetValue = () => CountersController.settings.scoreConfig.DecimalPrecision;
                    scorePrecision.SetValue = (v) => CountersController.settings.scoreConfig.DecimalPrecision = Mathf.RoundToInt(v);
                } },
            { CountersController.settings.progressConfig,
                sub => {
                    var progressRank = CountersPlusEditViewController.AddList(ref sub, "Show Time Left", "Starts the counter from the end of the song and decreases while the song is played.", 2);
                    progressRank.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                    progressRank.GetValue = () => CountersController.settings.progressConfig.ProgressTimeLeft ? 1f : 0f;
                    progressRank.SetValue = (v) => CountersController.settings.progressConfig.ProgressTimeLeft = v != 0f;

                    var progressMode = CountersPlusEditViewController.AddList(ref sub, "Mode", "", progressSettings.Count());
                    progressMode.GetTextForValue = (v) => {
                        return progressSettings[Mathf.RoundToInt(v)].Item2;
                    };
                    progressMode.GetValue = () => {
                        if (progressHover == null) progressHover = BeatSaberUI.AddHintText(progressMode.transform as RectTransform, determineModeText(CountersController.settings.progressConfig.Mode));
                        return progressSettings.ToList().IndexOf(progressSettings.Where((Tuple<ICounterMode, string> x) => (x.Item1 == CountersController.settings.progressConfig.Mode)).First());
                    };
                    progressMode.SetValue = (v) => {
                        if (progressHover == null) progressHover = BeatSaberUI.AddHintText(progressMode.transform as RectTransform, determineModeText(CountersController.settings.progressConfig.Mode));
                        CountersController.settings.progressConfig.Mode = progressSettings[Mathf.RoundToInt(v)].Item1;
                        progressHover.text = determineModeText(CountersController.settings.progressConfig.Mode);
                    };
                } },
            { CountersController.settings.speedConfig,
                sub => {
                    var speedPrecision = CountersPlusEditViewController.AddList(ref sub, "Percentage Precision", "How precise should the precentage be?", 6);
                    speedPrecision.GetTextForValue = (v) => Mathf.RoundToInt(v).ToString();
                    speedPrecision.GetValue = () => CountersController.settings.speedConfig.DecimalPrecision;
                    speedPrecision.SetValue = (v) => CountersController.settings.speedConfig.DecimalPrecision = Mathf.RoundToInt(v);

                    var speedMode = CountersPlusEditViewController.AddList(ref sub, "Mode", "", speedSettings.Count());
                    speedMode.GetTextForValue = (v) => {
                        return speedSettings[Mathf.RoundToInt(v)].Item2;
                    };
                    speedMode.GetValue = () => {
                        if (speedHover == null) speedHover = BeatSaberUI.AddHintText(speedMode.transform as RectTransform, determineModeText(CountersController.settings.speedConfig.Mode));
                        return speedSettings.ToList().IndexOf(speedSettings.Where((Tuple<ICounterMode, string> x) => (x.Item1 == CountersController.settings.speedConfig.Mode)).First());
                    };
                    speedMode.SetValue = (v) => {
                        if (speedHover == null) speedHover = BeatSaberUI.AddHintText(speedMode.transform as RectTransform, determineModeText(CountersController.settings.speedConfig.Mode));
                        CountersController.settings.speedConfig.Mode = speedSettings[Mathf.RoundToInt(v)].Item1;
                        speedHover.text = determineModeText(CountersController.settings.speedConfig.Mode);
                    };
                } },
            { CountersController.settings.cutConfig, v => { } },
        };

        private static string determineModeText(ICounterMode Mode)
        {
            string mode = "Unavilable mode!";
            switch (Mode)
            {
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
