using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CountersPlus.Config;
using CountersPlus.Counters;
using CountersPlus.Custom;
using UnityEngine.SceneManagement;

namespace CountersPlus
{
    public class CountersController : MonoBehaviour
    {
        public static CountersController Instance { get; private set; }
        public static List<GameObject> loadedCounters { get; private set; } = new List<GameObject>();
        internal static MainConfigModel settings;
        internal static List<CustomCounter> customCounters { get; private set; } = new List<CustomCounter>();

        public float pbPercent { get; private set; }

        

        public static void OnLoad()
        {
            settings = Config.Config.loadSettings();
            if (Instance == null && settings.Enabled)
            {
                GameObject controller = new GameObject("Counters+ Controller");
                DontDestroyOnLoad(controller);
                Instance = controller.AddComponent<CountersController>();
                Plugin.Log("Counters+ Controller created.");
            }
        }

        void Awake()
        {
            SceneManager.activeSceneChanged += activeSceneChanged;
        }

        void activeSceneChanged(Scene arg, Scene arg1)
        {
            if (arg1.name == "Menu") {
                //StartCoroutine(GetStandardLevelDetailViewController());
                loadedCounters.Clear();
            }
        }

        /*IEnumerator GetStandardLevelDetailViewController()
        {
            while (true)
            {
                sldvc = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
                if (sldvc != null)
                {
                    StartCoroutine(HandleSLDVC());
                    break;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        IEnumerator HandleSLDVC()
        {
            while (true)
            {
                PlayerDataModelSO player = null;
                try
                {
                    player = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().FirstOrDefault();
                    IDifficultyBeatmap beatmap = sldvc.GetPrivateField<IDifficultyBeatmap>("_difficultyBeatmap");
                    
                    PlayerLevelStatsData data = player.currentLocalPlayer.GetPlayerLevelStatsData(beatmap.level.levelID, beatmap.difficulty);
                    if (data.validScore)
                    {
                        int songMaxScore = ScoreController.MaxScoreForNumberOfNotes(beatmap.beatmapData.notesCount);
                        float roundMultiple = 100 * (float)Math.Pow(10, settings.pBConfig.DecimalPrecision);
                        pbPercent = (float)Math.Floor(data.highScore / (float)songMaxScore * roundMultiple) / roundMultiple; 
                    }
                    else
                        pbPercent = 0;
                }
                catch { }
                yield return new WaitForSeconds(0.1f);
            }
        }*/
        
        static void LoadCounter<T, R>(string name, T settings) where T : IConfigModel
        {
            if (!settings.Enabled || GameObject.Find("Counters+ | " + name + " Counter")) return;
            GameObject counter = new GameObject("Counters+ | " + name + " Counter");
            counter.transform.position = determinePosition(counter, settings.Position, settings.Index);
            counter.AddComponent(typeof(R));
            Plugin.Log("Loaded Counter: " + name);
            loadedCounters.Add(counter);
        }

        static void LoadCustomCounter<T>(string name, T settings, MonoBehaviour mono) where T : IConfigModel
        {
            if (!settings.Enabled || GameObject.Find("Counters+ | " + name + " Counter")) return;
            GameObject counter = new GameObject("Counters+ | " + name + " Counter");
            counter.transform.position = determinePosition(counter, settings.Position, settings.Index);
            counter.AddComponent(mono.GetType());
            Plugin.Log("Loaded Custom Counter: " + name);
            loadedCounters.Add(counter);
        }

        public static void LoadCounters()
        {
            Plugin.Log("Loading Counters...");
            LoadCounter<MissedConfigModel, MissedCounter>("Missed", settings.missedConfig);
            LoadCounter<AccuracyConfigModel, AccuracyCounter>("Notes", settings.accuracyConfig);
            LoadCounter<ScoreConfigModel, ScoreCounter>("Score", settings.scoreConfig);
            LoadCounter<ProgressConfigModel, ProgressCounter>("Progress", settings.progressConfig);
            LoadCounter<SpeedConfigModel, SpeedCounter>("Speed", settings.speedConfig);
            LoadCounter<CutConfigModel, CutCounter>("Cut", settings.cutConfig);
            foreach(CustomCounter counter in customCounters)
            {
                CustomConfigModel model = settings.CustomCounters.Where((CustomConfigModel x) => x.JSONName == counter.JSONName).FirstOrDefault();
                MonoBehaviour type = counter.Counter;
                if (model != null) LoadCustomCounter(counter.Name, model, counter.Counter);
            }
            if (settings.RNG) new GameObject("Counters+ | Randomizer").AddComponent<RandomizePositions>();
        }

        public static bool rng;

        public static Vector3 determinePosition(GameObject counter, Config.ICounterPositions position, int index)
        {
            Vector3 pos = new Vector3(); //Base position
            Vector3 offset = new Vector3(0, -0.75f * (index), 0); //Offset for any overlapping, indexes, etc.
            switch (position)
            {
                case Config.ICounterPositions.BelowCombo:
                    pos = new Vector3(-3f, 0, 7);
                    break;
                case Config.ICounterPositions.AboveCombo:
                    pos = new Vector3(-3f, 1.5f, 7);
                    if (settings.progressConfig.Position == position && settings.progressConfig.Index == index - 1 && settings.progressConfig.Mode == ICounterMode.BaseGame)
                        offset += new Vector3(0, -0.75f, 0);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case Config.ICounterPositions.BelowMultiplier:
                    pos = new Vector3(3f, 0, 7);
                    if (GameObject.Find("FCDisplay")) offset += new Vector3(0, -0.25f, 0);
                    break;
                case Config.ICounterPositions.AboveMultiplier:
                    pos = new Vector3(3f, 1.5f, 7);
                    if (settings.progressConfig.Position == position && settings.progressConfig.Index == index - 1 && settings.progressConfig.Mode == ICounterMode.BaseGame)
                        offset += new Vector3(0, -0.75f, 0);
                    if (GameObject.Find("FCDisplay")) offset += new Vector3(0, -0.25f, 0);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case Config.ICounterPositions.BelowEnergy:
                    pos = new Vector3(0, -1.5f, 7);
                    break;
                case Config.ICounterPositions.AboveHighway:
                    pos = new Vector3(0, 2.25f, 7);
                    if (settings.progressConfig.Position == position && settings.progressConfig.Index == index - 1 && settings.progressConfig.Mode == ICounterMode.BaseGame)
                        offset += new Vector3(0, -0.75f, 0);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
            }
            if (Plugin.beatSaberVersion == "0.12.1") //Handles slight position changes from Beat Saber v0.12.1
            {
                if (position != ICounterPositions.AboveHighway && position != ICounterPositions.BelowEnergy)
                {
                    if ((pos.x / Math.Abs(pos.x)) == -1) //If Counter would be on the Combo side
                        pos -= new Vector3(0.2f, -0.3f, 0);
                    else                                 //If Counter would be on Multiplier side
                        pos += new Vector3(0.2f, 0.3f, 0);
                }
            }
            if (counter.GetComponent<ProgressCounter>() != null)
            {
                offset += new Vector3(0.25f, 0, 0);
                if (settings.progressConfig.Mode != ICounterMode.Original) offset -= new Vector3(0.25f, 0, 0);
            }
            return pos + offset;
        }
    }
}
