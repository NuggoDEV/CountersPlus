using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountersPlus.Config
{
    internal class ConfigDefaults
    {
        internal static Dictionary<string, IConfigModel> Defaults = new Dictionary<string, IConfigModel>()
        {
            { "Missed", new MissedConfigModel(){ Enabled = true, Position = ICounterPositions.BelowCombo, Index = 0 } },
            { "Notes", new NoteConfigModel() { Enabled = true, Position = ICounterPositions.BelowCombo, Index = 1, DecimalPrecision = 2, ShowPercentage = true} },
            { "Progress", new ProgressConfigModel() { Enabled = true, Position = ICounterPositions.BelowEnergy, Index = 0, Mode = ICounterMode.Original, ProgressTimeLeft = false, IncludeRing = false } },
            { "Score", new ScoreConfigModel() { Enabled = true, Position = ICounterPositions.BelowMultiplier, Index = 0, DecimalPrecision = 2, DisplayRank = true, Mode = ICounterMode.Both} },
            { "Speed", new SpeedConfigModel() { Enabled = false, Position = ICounterPositions.BelowMultiplier, Index = 1, DecimalPrecision = 2, Mode = ICounterMode.Average} },
            { "Cut", new CutConfigModel() { Enabled = false, Position = ICounterPositions.AboveHighway, Index = 0} },
            { "Spinometer", new SpinometerConfigModel() { Enabled = false, Position = ICounterPositions.AboveMultiplier, Index = 0, Mode = ICounterMode.Highest} },
            { "Personal Best", new PBConfigModel() { Enabled = false, Position = ICounterPositions.BelowMultiplier, Index = 1} }
        };
    }
}
