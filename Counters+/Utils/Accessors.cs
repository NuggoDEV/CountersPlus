using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CountersPlus.Utils
{
    /// <summary>
    /// Class that contains various <see cref="FieldAccessor{T, U}"/>s that see use within Counters+.
    /// </summary>
    static class Accessors
    {
        #region Core Game HUD Components
        public static FieldAccessor<CoreGameHUDController, GameObject>.Accessor SongProgressPanelGO = FieldAccessor<CoreGameHUDController, GameObject>.GetAccessor("_songProgressPanelGO");
        public static FieldAccessor<CoreGameHUDController, GameObject>.Accessor RelativeScoreGO = FieldAccessor<CoreGameHUDController, GameObject>.GetAccessor("_relativeScoreGO");
        public static FieldAccessor<CoreGameHUDController, GameObject>.Accessor ImmediateRankGO = FieldAccessor<CoreGameHUDController, GameObject>.GetAccessor("_immediateRankGO");
        public static FieldAccessor<CoreGameHUDController, GameObject>.Accessor EnergyPanelGO = FieldAccessor<CoreGameHUDController, GameObject>.GetAccessor("_energyPanelGO");
        #endregion

        public static FieldAccessor<ScoreMultiplierUIController, Image>.Accessor MultiplierImage = FieldAccessor<ScoreMultiplierUIController, Image>.GetAccessor("_multiplierProgressImage");

        public static FieldAccessor<ScoreUIController, TextMeshProUGUI>.Accessor ScoreUIText = FieldAccessor<ScoreUIController, TextMeshProUGUI>.GetAccessor("_scoreText");
    }
}
