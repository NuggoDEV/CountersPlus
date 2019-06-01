using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CountersPlus.Config;
using CustomUI.BeatSaber;
using CustomUI.Settings;
using UnityEngine;
using CPEVC = CountersPlus.UI.CountersPlusEditViewController;
using CC = CountersPlus.CountersController;

namespace CountersPlus.UI
{
    public class AdvancedCounterSettings
    {
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
        static List<Tuple<ICounterMode, string>> scoreSettings = new List<Tuple<ICounterMode, string>>
        {
            {ICounterMode.Original, "Original" }, //Counters+ Counter w/ Points
            {ICounterMode.BaseGame, "Base Game" }, //Base Game w/ Points
            //{ICounterMode.BaseWithOutPoints, "Base No Points" }, //Base Game w/ Points Under Combo
            {ICounterMode.LeavePoints, "No Points" }, //Counters+ Counter w/ Points Under Combo
            {ICounterMode.ScoreOnly, "Score Only" }, //Counters+ Counter w/ Points Removed Entirely
        };
        static List<Tuple<ICounterMode, string>> spinometerSettings = new List<Tuple<ICounterMode, string>>
        {
            {ICounterMode.Original, "Original" },
            {ICounterMode.SplitAverage, "Split" },
            {ICounterMode.Highest, "Highest" },
        };

        private static HoverHint progressHover;
        private static HoverHint speedHover;
        private static HoverHint scoreHover;
        private static HoverHint spinometerHover;

        internal static Dictionary<ConfigModel, Action<SubMenu, ConfigModel>> counterUIItems = new Dictionary<ConfigModel, Action<SubMenu, ConfigModel>>()
        {
            { CC.settings.missedConfig, (sub, config) => {
                #pragma warning disable CS0618 //Fuck off DaNike
                if (IPA.Loader.PluginManager.Plugins.Where((IPA.Old.IPlugin x) => x.Name == "CustomMissText").Any())
                {
                    var cmt = CPEVC.AddList(ref sub, config, "Custom Miss Text Integration", "Replaces the \"Misses\" label with one of the Custom Miss Text insults.\nAlso changes when you miss a note.", 2);
                    cmt.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                    cmt.GetValue = () => CC.settings.missedConfig.CustomMissTextIntegration ? 1f : 0f;
                    cmt.SetValue += (v) => CC.settings.missedConfig.CustomMissTextIntegration = v != 0f;
                }
            } },
            { CC.settings.noteConfig, (sub, config) => {
                var accuracyPercentage = CPEVC.AddList(ref sub, config, "Show Percentage", "Toggles the percentage of notes hit over total notes.", 2);
                accuracyPercentage.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                accuracyPercentage.GetValue = () => CC.settings.noteConfig.ShowPercentage ? 1f : 0f;
                accuracyPercentage.SetValue += (v) => CC.settings.noteConfig.ShowPercentage = v != 0f;

                var accuracyPrecision = CPEVC.AddList(ref sub, config, "Percentage Precision", "How precise should the precentage be?", 6);
                accuracyPrecision.GetTextForValue = (v) => Mathf.RoundToInt(v).ToString();
                accuracyPrecision.GetValue = () => CC.settings.noteConfig.DecimalPrecision;
                accuracyPrecision.SetValue += (v) => CC.settings.noteConfig.DecimalPrecision = Mathf.RoundToInt(v);
            } },
            { CC.settings.scoreConfig, (sub, config) => {
                var scoreRank = CPEVC.AddList(ref sub, config, "Display Rank", "Displays the rank as you progress in a song.", 2);
                scoreRank.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                scoreRank.GetValue = () => CC.settings.scoreConfig.DisplayRank ? 1f : 0f;
                scoreRank.SetValue += (v) => CC.settings.scoreConfig.DisplayRank = v != 0f;

                var scoreMode = CPEVC.AddList(ref sub, config, "Mode", "", scoreSettings.Count());
                scoreMode.GetTextForValue = (v) => {
                    return scoreSettings[Mathf.RoundToInt(v)].Item2;
                };
                scoreMode.GetValue = () => {
                    if (scoreHover == null) scoreHover = BeatSaberUI.AddHintText(scoreMode.transform as RectTransform, DetermineModeText(CC.settings.scoreConfig.Mode, true));
                    if (CC.settings.scoreConfig.Mode == ICounterMode.Both) CC.settings.scoreConfig.Mode = ICounterMode.Original;
                    return scoreSettings.ToList().IndexOf(scoreSettings.Where((Tuple<ICounterMode, string> x) => (x.Item1 == CC.settings.scoreConfig.Mode)).First());
                };
                scoreMode.SetValue += (v) => {
                    if (scoreHover == null) scoreHover = BeatSaberUI.AddHintText(scoreMode.transform as RectTransform, DetermineModeText(CC.settings.scoreConfig.Mode, true));
                    CC.settings.scoreConfig.Mode = scoreSettings[Mathf.RoundToInt(v)].Item1;
                    scoreHover.text = DetermineModeText(CC.settings.scoreConfig.Mode, true);
                };

                var scorePrecision = CPEVC.AddList(ref sub, config, "Percentage Precision", "How precise should the precentage be?", 6);
                scorePrecision.GetTextForValue = (v) => Mathf.RoundToInt(v).ToString();
                scorePrecision.GetValue = () => CC.settings.scoreConfig.DecimalPrecision;
                scorePrecision.SetValue += (v) => CC.settings.scoreConfig.DecimalPrecision = Mathf.RoundToInt(v);
            } },
            { CC.settings.pbConfig, (sub, config) =>
            {
                var pbPrecision = CPEVC.AddList(ref sub, config, "Percentage Precision", "How precise should the precentage be?", 6);
                pbPrecision.GetTextForValue = (v) => Mathf.RoundToInt(v).ToString();
                pbPrecision.GetValue = () => CC.settings.pbConfig.DecimalPrecision;
                pbPrecision.SetValue += (v) => CC.settings.pbConfig.DecimalPrecision = Mathf.RoundToInt(v);

                var textSize = CPEVC.AddList(ref sub, config, "Text Size", "How large should the text be?", 3);
                textSize.GetTextForValue = (v) => (Mathf.RoundToInt(v + 2)).ToString();
                textSize.GetValue = () => CC.settings.pbConfig.TextSize - 2;
                textSize.SetValue += (v) => CC.settings.pbConfig.TextSize = Mathf.RoundToInt(v) + 2;

                var underScore = CPEVC.AddList(ref sub, config, "Below Score Counter", "Will the Personal Best counter instead be positioned below the Score Counter?\n<color=#FF0000>Only works with Score Counter's <i>Original</i> Mode option.</color>", 2);
                underScore.GetTextForValue = (v) => (v != 0f) ? "YES" : "NO";
                underScore.GetValue = () => CC.settings.pbConfig.UnderScore ? 1f : 0f;
                underScore.SetValue += (v) => CC.settings.pbConfig.UnderScore = v != 0f;
            } },
            { CC.settings.progressConfig, (sub, config) => {
                var progressRank = CPEVC.AddList(ref sub, config, "Progress From End", "Starts the counter from the end of the song and decreases while the song is played.", 2);
                progressRank.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                progressRank.GetValue = () => {
                    if(CC.settings.progressConfig.ProgressTimeLeft) return 1f;
                    else return 0f;
                };
                progressRank.SetValue += (v) => {
                    CC.settings.progressConfig.ProgressTimeLeft = v != 0f;
                    };

                var includeRing = CPEVC.AddList(ref sub, CC.settings.progressConfig, "Include Progress Ring", "Whether or not the Progress Ring will also be effected by the \"Progress From End\" setting.\nOnly active in \"Original\" mode.", 2);
                includeRing.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                includeRing.GetValue = () => CC.settings.progressConfig.IncludeRing ? 1f : 0f;
                includeRing.SetValue += (v) => CC.settings.progressConfig.IncludeRing = v != 0f;

                var progressMode = CPEVC.AddList(ref sub, config, "Mode", "", progressSettings.Count());
                progressMode.GetTextForValue = (v) => {
                    return progressSettings[Mathf.RoundToInt(v)].Item2;
                };
                progressMode.GetValue = () => {
                    if (progressHover == null) progressHover = BeatSaberUI.AddHintText(progressMode.transform as RectTransform, DetermineModeText(CC.settings.progressConfig.Mode));
                    return progressSettings.ToList().IndexOf(progressSettings.Where((Tuple<ICounterMode, string> x) => (x.Item1 == CC.settings.progressConfig.Mode)).First());
                };
                progressMode.SetValue += (v) => {
                    if (progressHover == null) progressHover = BeatSaberUI.AddHintText(progressMode.transform as RectTransform, DetermineModeText(CC.settings.progressConfig.Mode));
                    CC.settings.progressConfig.Mode = progressSettings[Mathf.RoundToInt(v)].Item1;
                    progressHover.text = DetermineModeText(CC.settings.progressConfig.Mode);
                };
            } },
            { CC.settings.speedConfig, (sub, config) => {
                var speedPrecision = CPEVC.AddList(ref sub, config, "Percentage Precision", "How precise should the precentage be?", 6);
                speedPrecision.GetTextForValue = (v) => Mathf.RoundToInt(v).ToString();
                speedPrecision.GetValue = () => CC.settings.speedConfig.DecimalPrecision;
                speedPrecision.SetValue += (v) => CC.settings.speedConfig.DecimalPrecision = Mathf.RoundToInt(v);

                var speedMode = CPEVC.AddList(ref sub, config, "Mode", "", speedSettings.Count());
                speedMode.GetTextForValue = (v) => {
                    return speedSettings[Mathf.RoundToInt(v)].Item2;
                };
                speedMode.GetValue = () => {
                    if (speedHover == null) speedHover = BeatSaberUI.AddHintText(speedMode.transform as RectTransform, DetermineModeText(CC.settings.speedConfig.Mode));
                    return speedSettings.ToList().IndexOf(speedSettings.Where((Tuple<ICounterMode, string> x) => (x.Item1 == CC.settings.speedConfig.Mode)).First());
                };
                speedMode.SetValue += (v) => {
                    if (speedHover == null) speedHover = BeatSaberUI.AddHintText(speedMode.transform as RectTransform, DetermineModeText(CC.settings.speedConfig.Mode));
                    CC.settings.speedConfig.Mode = speedSettings[Mathf.RoundToInt(v)].Item1;
                    speedHover.text = DetermineModeText(CC.settings.speedConfig.Mode);
                };
            } },
            { CC.settings.cutConfig, (sub, config) => { } },
            { CC.settings.spinometerConfig, (sub, config) => {
                var spinometerMode = CPEVC.AddList(ref sub, config, "Mode", "", spinometerSettings.Count());
                spinometerMode.GetTextForValue = (v) => {
                    return spinometerSettings[Mathf.RoundToInt(v)].Item2;
                };
                spinometerMode.GetValue = () => {
                    if (spinometerHover == null) spinometerHover = BeatSaberUI.AddHintText(spinometerMode.transform as RectTransform, DetermineModeText(CC.settings.spinometerConfig.Mode, true));
                    return spinometerSettings.ToList().IndexOf(spinometerSettings.Where((Tuple<ICounterMode, string> x) => (x.Item1 == CC.settings.spinometerConfig.Mode)).First());
                };
                spinometerMode.SetValue += (v) => {
                    if (spinometerHover == null) spinometerHover = BeatSaberUI.AddHintText(spinometerMode.transform as RectTransform, DetermineModeText(CC.settings.spinometerConfig.Mode, true));
                    CC.settings.spinometerConfig.Mode = spinometerSettings[Mathf.RoundToInt(v)].Item1;
                    spinometerHover.text = DetermineModeText(CC.settings.spinometerConfig.Mode, true);
                };
            } },
            { CC.settings.notesLeftConfig, (sub, config) => { } },
            { CC.settings.failsConfig, (sub, config) => {
                var showRestarts = CPEVC.AddList(ref sub, CC.settings.failsConfig, "Track Restarts", "Instead of showing global fail count, show the amount of times you've restarted the same song.", 2);
                showRestarts.GetTextForValue = (v) => (v != 0f) ? "ON" : "OFF";
                showRestarts.GetValue = () => CC.settings.failsConfig.ShowRestartsInstead ? 1f : 0f;
                showRestarts.SetValue += (v) => CC.settings.failsConfig.ShowRestartsInstead = v != 0f;
            } },
        };

        private static string DetermineModeText(ICounterMode Mode, bool alternateText = false)
        {
            string mode = "Invalid mode!";
            switch (Mode)
            {
                case ICounterMode.Average:
                    mode = "Mean Speed: Average speed of both sabers.";
                    break;
                case ICounterMode.Top5Sec:
                    mode = "Top: Fastest saber speed in the last 5 seconds.";
                    break;
                case ICounterMode.Both:
                    if (alternateText)
                        mode = "Original: Have Counters+ counter with the Points above the percentage.";
                    else
                        mode = "Both: A secondary Counter will be added so both Average and Top Speed will be displayed.";
                    break;
                case ICounterMode.SplitAverage:
                    if (alternateText)
                        mode = "Split: Updates your rotation speed every second for each saber separately.";
                    else
                        mode = "Split Mean: Displays averages for each saber, separately.";
                    break;
                case ICounterMode.SplitBoth:
                    mode = "Split Both: Displays both metrics, except the Average is split between two sabers.";
                    break;
                case ICounterMode.BaseGame:
                    mode = "Base Game: Uses the base game counter.\n<color=#FF0000>Some settings will not apply in this mode.</color>";
                    break;
                case ICounterMode.Original:
                    if (alternateText)
                        mode = "Original: Updates your average rotation speed every second.";
                    else
                        mode = "Original: Uses the original display mode, with a white circle bordering a time.";
                    break;
                case ICounterMode.Percent:
                    mode = "Percent: Displays a simple percent of the completed song.\n<color=#FF0000>Some settings will not apply in this mode.</color>";
                    break;
                case ICounterMode.BaseWithOutPoints:
                    mode = "Base No Points: Uses the base game counter, except the Points will be under the combo.\n<color=#FF0000>This might conflict with counters positioned below the combo.</color>";
                    break;
                case ICounterMode.LeavePoints:
                    mode = "No Points: Uses Counters+ counter, except the Points will be under the combo.\n<color=#FF0000>This might conflict with counters positioned below the combo.</color>";
                    break;
                case ICounterMode.ScoreOnly:
                    mode = "Score Only: Uses Counters+ counter, and completely removes the Points.";
                    break;
                case ICounterMode.Highest:
                    mode = "Highest: Displays your highest rotation speed throughout a song.";
                    break;
            }
            return "How should this Counter display data?\n" + mode;
        }
    }
}
