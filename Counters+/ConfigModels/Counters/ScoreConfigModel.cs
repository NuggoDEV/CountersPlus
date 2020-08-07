using IPA.Config.Stores.Attributes;

namespace CountersPlus.ConfigModels
{
    public class ScoreConfigModel : ConfigModel
    {
        [Ignore]
        public override string DisplayName => "Score";

        public override bool Enabled { get; set; } = true;
        [UseConverter]
        public override CounterPositions Position { get; set; } = CounterPositions.BelowMultiplier;
        public override int Distance { get; set; } = 0;

        [UseConverter]
        public virtual ScoreMode Mode { get; set; } = ScoreMode.Original;
        public virtual int DecimalPrecision { get; set; } = 2;
        public virtual bool DisplayRank { get; set; } = true;
        public virtual bool CustomRankColors { get; set; } = true;
        public virtual string SSColor { get; set; } = "#00FFFF";
        public virtual string SColor { get; set; } = "#FFFFFF";
        public virtual string AColor { get; set; } = "#00FF00";
        public virtual string BColor { get; set; } = "#FFFF00";
        public virtual string CColor { get; set; } = "#FFA700";
        public virtual string DColor { get; set; } = "#FF0000";
        public virtual string EColor { get; set; } = "#FF0000";
    }

    public enum ScoreMode { Original, BaseGame, ScoreOnly, LeavePoints, BaseGameLeavePoints }
}
