using System.Collections.Generic;
using CountersPlus.Config;

namespace CountersPlus.UI
{
    public class AdvancedCounterSettings
    {
        public static readonly Dictionary<ICounterMode, string> SpeedSettings = new Dictionary<ICounterMode, string> {
            {ICounterMode.Average, "Average Speed" },
            {ICounterMode.Top5Sec, "Top (past 5 Seconds)" },
            {ICounterMode.Both, "Both Counters Visible" },
            {ICounterMode.SplitAverage, "Split Average" },
            {ICounterMode.SplitBoth, "Both Visible, with Split Average" }
        };
        public static readonly Dictionary<ICounterMode, string> ProgressSettings = new Dictionary<ICounterMode, string> {
            {ICounterMode.BaseGame, "Base Game" },
            {ICounterMode.Original, "Original" },
            {ICounterMode.Percent, "Percentage" }
        };
        public static readonly Dictionary<ICounterMode, string> ScoreSettings = new Dictionary<ICounterMode, string> {
            {ICounterMode.Original, "Original" }, //Counters+ Counter w/ Points
            {ICounterMode.BaseGame, "Base Game" }, //Base Game w/ Points
            //{ICounterMode.BaseWithOutPoints, "Base No Points" }, //Base Game w/ Points Under Combo
            {ICounterMode.LeavePoints, "Leave Points Under Combo" }, //Counters+ Counter w/ Points Under Combo
            {ICounterMode.ScoreOnly, "Delete Points Entirely" }, //Counters+ Counter w/ Points Removed Entirely
        };
        public static readonly Dictionary<ICounterMode, string> SpinometerSettings = new Dictionary<ICounterMode, string> {
            {ICounterMode.Original, "Original" },
            {ICounterMode.SplitAverage, "Split Average" },
            {ICounterMode.Highest, "Highest" },
        };
        public static readonly Dictionary<ICounterPositions, string> Positions = new Dictionary<ICounterPositions, string> {
            {ICounterPositions.BelowCombo, "Below Combo" },
            {ICounterPositions.AboveCombo, "Above Combo" },
            {ICounterPositions.BelowMultiplier, "Below Multiplier" },
            {ICounterPositions.AboveMultiplier, "Above Multiplier" },
            {ICounterPositions.BelowEnergy, "Below Energy" },
            {ICounterPositions.AboveHighway, "Over Highway" }
        };

        public static readonly List<int> Distances = new List<int> { -1, 0, 1, 2, 3, 4 };
        public static readonly List<int> PercentagePrecision = new List<int> { 0, 1, 2, 3, 4, 5 };
        public static readonly List<int> TextSize = new List<int> { 2, 3, 4 };
        public static readonly List<float> CounterOffsets = new List<float> { -1, -0.9f, -0.8f, -0.7f, -0.6f, -0.5f, -0.4f, -0.3f, -0.2f, -0.1f, 0, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1 };
        public static readonly List<int> AverageCutPrecision = new List<int> { 0, 1, 2, 3 };
    }
}
