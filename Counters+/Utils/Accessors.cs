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
        public static FieldAccessor<TableView, TableViewScroller>.Accessor TVTableViewScroller = FieldAccessor<TableView, TableViewScroller>.GetAccessor("scroller");
        public static FieldAccessor<TableView, TableView.TableType>.Accessor TVTableType = FieldAccessor<TableView, TableView.TableType>.GetAccessor("_tableType");
        public static FieldAccessor<TableView, TableView.CellsGroup[]>.Accessor TVPreallocCells = FieldAccessor<TableView, TableView.CellsGroup[]>.GetAccessor("_preallocatedCells");
        public static FieldAccessor<TableView, bool>.Accessor TVIsInitialized = FieldAccessor<TableView, bool>.GetAccessor("_isInitialized");
        public static FieldAccessor<TableView, Button>.Accessor TVPageUpButton = FieldAccessor<TableView, Button>.GetAccessor("_pageUpButton");
        public static FieldAccessor<TableView, Button>.Accessor TVPageDownButton = FieldAccessor<TableView, Button>.GetAccessor("_pageDownButton");
        public static FieldAccessor<TableView, RectTransform>.Accessor TVScrollRect = FieldAccessor<TableView, RectTransform>.GetAccessor("_viewportTransform");


        public static FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, TextMeshProUGUI>.Accessor PackInfoTextAccessor = FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, TextMeshProUGUI>.GetAccessor("_infoText");
        public static FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, Image>.Accessor CoverImageAccessor = FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, Image>.GetAccessor("_coverImage");
        public static FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, ImageView>.Accessor SelectionImageAccessor = FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, ImageView>.GetAccessor("_selectionImage");
        public static FieldAccessor<ImageView, float>.Accessor ImageViewSkewAccessor = FieldAccessor<ImageView, float>.GetAccessor("_skew");
        public static FieldAccessor<LevelListTableCell, TextMeshProUGUI>.Accessor LevelListNameAccessor = FieldAccessor<LevelListTableCell, TextMeshProUGUI>.GetAccessor("_songNameText");

        public static FieldAccessor<GameScenesManager, HashSet<string>>.Accessor GSMPersistentScenes = FieldAccessor<GameScenesManager, HashSet<string>>.GetAccessor("_neverUnloadScenes");
        public static FieldAccessor<MenuTransitionsHelper, TutorialScenesTransitionSetupDataSO>.Accessor MTHTutorialScenesSetup = FieldAccessor<MenuTransitionsHelper, TutorialScenesTransitionSetupDataSO>.GetAccessor("_tutorialScenesTransitionSetupData");
        public static FieldAccessor<SettingsFlowCoordinator, MainSettingsModelSO>.Accessor SFCMainSettingsModel = FieldAccessor<SettingsFlowCoordinator, MainSettingsModelSO>.GetAccessor("_mainSettingsModel");
        #endregion

        public static FieldAccessor<ScoreMultiplierUIController, Image>.Accessor MultiplierImage = FieldAccessor<ScoreMultiplierUIController, Image>.GetAccessor("_multiplierProgressImage");
        public static FieldAccessor<ScoreUIController, TextMeshProUGUI>.Accessor ScoreUIText = FieldAccessor<ScoreUIController, TextMeshProUGUI>.GetAccessor("_scoreText");
        public static FieldAccessor<ScoreController, GameplayModifiersModelSO>.Accessor SCGameplayModsModel = FieldAccessor<ScoreController, GameplayModifiersModelSO>.GetAccessor("_gameplayModifiersModel");
    }
}
