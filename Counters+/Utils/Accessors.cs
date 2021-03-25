using HMUI;
using IPA.Utilities;
using System.Collections.Generic;
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

        #region Counters+ UI
        public static FieldAccessor<GameScenesManager, HashSet<string>>.Accessor GSMPersistentScenes = FieldAccessor<GameScenesManager, HashSet<string>>.GetAccessor("_neverUnloadScenes");
        public static FieldAccessor<MenuTransitionsHelper, TutorialScenesTransitionSetupDataSO>.Accessor MTHTutorialScenesSetup = FieldAccessor<MenuTransitionsHelper, TutorialScenesTransitionSetupDataSO>.GetAccessor("_tutorialScenesTransitionSetupData");
        public static FieldAccessor<SettingsFlowCoordinator, MainSettingsModelSO>.Accessor SFCMainSettingsModel = FieldAccessor<SettingsFlowCoordinator, MainSettingsModelSO>.GetAccessor("_mainSettingsModel");
        #endregion

        public static FieldAccessor<ScoreUIController, TextMeshProUGUI>.Accessor ScoreUIText = FieldAccessor<ScoreUIController, TextMeshProUGUI>.GetAccessor("_scoreText");
        public static FieldAccessor<ScoreController, GameplayModifiersModelSO>.Accessor SCGameplayModsModel = FieldAccessor<ScoreController, GameplayModifiersModelSO>.GetAccessor("_gameplayModifiersModel");
    }
}
