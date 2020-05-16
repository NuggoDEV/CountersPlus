using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CountersPlus.Utils;
using CountersPlus.Config;
using CountersPlus.Counters;
using CountersPlus.Custom;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CountersPlus
{
    public class CountersController : MonoBehaviour
    {
        public static CountersController Instance { get; private set; }
        public static List<GameObject> LoadedCounters { get; private set; } = new List<GameObject>();
        internal static MainConfigModel settings;

        public static Action<CountersData> ReadyToInit;

        public static void OnLoad()
        {
            settings = ConfigLoader.LoadSettings();
            if (Instance == null)
            {
                GameObject controller = new GameObject("Counters+ | Controller");
                DontDestroyOnLoad(controller);
                Instance = controller.AddComponent<CountersController>();
                controller.AddComponent<VersionChecker>();
                Plugin.Log("Counters Controller created.", LogInfo.Notice);
            }
        }

        /// <summary>
        /// Loads a counter given a ConfigModel and a MonoBehaviour. If enabled, a new GameObject will be created and the provided MonoBehaviour will be attached.
        /// </summary>
        /// <typeparam name="T">ConfigModel to use for settings.</typeparam>
        /// <typeparam name="R">MonoBehaviour to attach to the new GameObject.</typeparam>
        /// <param name="settings">ConfigModel settings reference.</param>
        internal static void LoadCounter<T, R>(T settings) where T : ConfigModel where R : Counter<T>
        {
            if (!settings.Enabled || GameObject.Find($"Counters+ | {settings.DisplayName} Counter")) return;
            R counter = new GameObject($"Counters+ | {settings.DisplayName} Counter").AddComponent(typeof(R)) as R;
            counter.settings = settings;
            Plugin.Log($"Loaded Counter: {settings.DisplayName}");
            LoadedCounters.Add(counter.gameObject);
        }

        private IEnumerator ObtainRequiredData()
        {
            Plugin.Log("Obtaining required counter data...");
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<BeatmapObjectManager>().Any());
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<PlayerController>().Any());
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().Any());
            CountersData data = new CountersData();
            ReadyToInit.Invoke(data);
            Plugin.Log("Obtained data!");
            if (settings.HideCombo) HideUIElementWithComponent<ComboUIController>();
            if (settings.HideMultiplier) HideUIElementWithComponent<ScoreMultiplierUIController>();

            //Bullshit warning of detecting ScoreSaber replays but uhhhh i cant do shit about false positives during them.
            //Yell at Umby to expose events that Counters+ can use during ScoreSaber replays.
            if (GameObject.Find("InGameReplayUI") != null && settings.ScoreSaberReplayWarning)
            {
                UI.CounterWarning.Create("Counters+ is not fully compatible with ScoreSaber replays.");
            }
        }

        private void HideUIElementWithComponent<T>() where T : MonoBehaviour
        {
            GameObject gameObject = (Resources.FindObjectsOfTypeAll<T>().FirstOrDefault() as MonoBehaviour).gameObject;
            if (gameObject != null && gameObject.activeInHierarchy)
                RecurseFunctionOverGameObjectTree(gameObject, (child) => child.SetActive(false));
            else Plugin.Log($"Can't remove a GameObject with the attached component {typeof(T).Name}!", LogInfo.Warning);
        }

        private void RecurseFunctionOverGameObjectTree(GameObject go, Action<GameObject> func)
        {
            foreach (Transform child in go.transform)
            {
                RecurseFunctionOverGameObjectTree(child.gameObject, func);
                func?.Invoke(go);
            }
        }

        public static void LoadCounters()
        {
            Plugin.Log("Loading Counters...", LogInfo.Notice);
            LoadCounter<MissedConfigModel, MissedCounter>(settings.missedConfig);
            LoadCounter<NoteConfigModel, AccuracyCounter>(settings.noteConfig);
            LoadCounter<ScoreConfigModel, ScoreCounter>(settings.scoreConfig);
            LoadCounter<PBConfigModel, PBCounter>(settings.pbConfig);
            LoadCounter<ProgressConfigModel, ProgressCounter>(settings.progressConfig);
            LoadCounter<SpeedConfigModel, SpeedCounter>(settings.speedConfig);
            LoadCounter<CutConfigModel, CutCounter>(settings.cutConfig);
            LoadCounter<SpinometerConfigModel, Spinometer>(settings.spinometerConfig);
            LoadCounter<NotesLeftConfigModel, NotesLeftCounter>(settings.notesLeftConfig);
            LoadCounter<FailConfigModel, FailCounter>(settings.failsConfig);
            foreach (CustomCounter potential in CustomCounterCreator.LoadedCustomCounters)
            {
                if (potential.TemplateCounter != null)
                    LoadCounter<CustomConfigModel, CustomCounterTemplate>(potential.ConfigModel);
                else
                    LoadCounter<CustomConfigModel, CustomCounterHook>(potential.ConfigModel);
            }
            Plugin.Log("Counters loaded!", LogInfo.Notice);
            Instance.StartCoroutine(Instance.ObtainRequiredData());
        }
    }

    public class CountersData
    {
        public BeatmapObjectManager BOM;
        public ScoreController ScoreController;
        public PlayerController PlayerController;
        public AudioTimeSyncController AudioTimeSyncController;
        public PlayerDataModel PlayerData;
        public GameplayModifiersModelSO ModifiersData;
        public GameplayCoreSceneSetupData GCSSD;
        public bool Is360Or90Level = false;

        public CountersData()
        {
            BOM = Resources.FindObjectsOfTypeAll<BeatmapObjectManager>().First();
            ScoreController = Resources.FindObjectsOfTypeAll<ScoreController>().First();
            PlayerController = Resources.FindObjectsOfTypeAll<PlayerController>().First();
            AudioTimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().First();
            PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First();
            ModifiersData = Resources.FindObjectsOfTypeAll<GameplayModifiersModelSO>().First();
            GCSSD = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData; //By the time all of these load, so should GCSSD.
            Is360Or90Level = Resources.FindObjectsOfTypeAll<FlyingGameHUDRotation>().Any(x => x.isActiveAndEnabled);
        }
    }
}
