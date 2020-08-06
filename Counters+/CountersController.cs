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
using System.CodeDom;

namespace CountersPlus
{
    public class CountersController : MonoBehaviour
    {
        public static CountersController Instance { get; private set; }
        /// <summary>
        /// A Dictionary of all loaded Counters+ <see cref="Counter{T}"/>, mapped by the type of their <see cref="Counter{T}.settings"/>
        /// </summary>
        internal static Dictionary<Type, MonoBehaviour> LoadedCounters = new Dictionary<Type, MonoBehaviour>();
        /// <summary>
        /// A reference to the main Counters+ config.
        /// </summary>
        internal static MainConfigModel settings;

        /// <summary>
        /// Fired when Counters+ has obtained all necessary objects the counters require, and is ready to hand them out. 
        /// This fires <see cref="Counter{T}.Init(CountersData, Vector3)"/>
        /// </summary>
        public static Action<CountersData> ReadyToInit;

        /// <summary>
        /// Load our settings from the config file, and creates an instance of <see cref="CountersController"/> if it doesn't exist.
        /// Should only be called when the plugin has been enabled.
        /// </summary>
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
        /// Loads a counter given types to a <see cref="ConfigModel"/> and a <see cref="Counter{T}"/>.
        /// If enabled, a new <see cref="GameObject"/> will be created and the provided <see cref="Counter{T}"/> will be attached.
        /// </summary>
        /// <typeparam name="T">Type of a <see cref="ConfigModel"/> to use for settings.</typeparam>
        /// <typeparam name="R">Type of a <see cref="Counter{T}"/> to attach to the new GameObject.</typeparam>
        /// <param name="settings">Settings object to be passed into the newly created <see cref="Counter{T}"/> component.</param>
        internal static void LoadCounter<T, R>(T settings) where T : ConfigModel where R : Counter<T>
        {
            if (!settings.Enabled || LoadedCounters.ContainsKey(typeof(R))) return;
            R counter = new GameObject($"Counters+ | {settings.DisplayName} Counter").AddComponent(typeof(R)) as R;
            counter.settings = settings;
            Plugin.Log($"Loaded Counter: {settings.DisplayName}");
            LoadedCounters.Add(typeof(R), counter);
        }

        /// <summary>
        /// Retrieves a loaded <see cref="Counter{T}"/> when given the <see cref="ConfigModel"/> that is assigned to it. 
        /// Should only be used to obtain the built in Counters+ counters. Custom counters should be handled per mod.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="ConfigModel"/> bound to a <see cref="Counter{T}"/></typeparam>
        /// <returns>A <see cref="Counter{T}"/> component.</returns>
        public static T GetLoadedCounter<T>() where T : MonoBehaviour, ICounter
        {
            if (LoadedCounters.TryGetValue(typeof(T), out MonoBehaviour counter))
            {
                return counter as T;
            }
            return null;
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
        }

        private void HideUIElementWithComponent<T>() where T : MonoBehaviour
        {
            GameObject gameObject = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault().gameObject;
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

        /// <summary>
        /// Loads all built-in Counters+ counters. Should only be called once when the game scene is loaded.
        /// </summary>
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
