using CountersPlus.ConfigModels;
using CountersPlus.Counters;
using CountersPlus.Counters.Event_Broadcasters;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Utils;
using UnityEngine;
using Zenject;

namespace CountersPlus.Installers
{
    class CountersInstaller : MonoInstaller
    {
        [Inject] private MainConfigModel mainConfig;

        public override void InstallBindings()
        {
            // TODO re-add check for No Text and HUD option
            if (!mainConfig.Enabled) return;

            /// LOADING IMPORTANT SHIT LIKE CANVASES AND STUFF ///
            Container.Bind<CanvasUtility>().AsSingle().NonLazy();

            /// LOADING COUNTERS ///
            Plugin.Logger.Notice("Loading counters...");

            AddCounter<MissedConfigModel, MissedCounter>();
            AddCounter<NoteConfigModel, NotesCounter>();

            if (mainConfig.ProgressConfig.Mode != ProgressMode.BaseGame)
            {
                AddCounter<ProgressConfigModel, ProgressCounter>();
            } // TODO add base game variant for progress counter

            AddCounter<ScoreConfigModel, ScoreCounter>();
            AddCounter<CutConfigModel, CutCounter>();
            AddCounter<FailConfigModel, FailCounter>();
            AddCounter<NotesLeftConfigModel, NotesLeftCounter>();

            /// LOADING BROADCASTERS WITH BROADCAST IN-GAME EVENTS TO COUNTERS AND STUFF ///
            Container.BindInterfacesAndSelfTo<CounterEventBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<NoteEventBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScoreEventBroadcaster>().AsSingle().NonLazy();
            Plugin.Logger.Notice("Counters loaded!");
        }

        private void AddCounter<T, R>() where T : ConfigModel where R : ICounter
        {
            T settings = Container.Resolve<T>();

            if (!settings.Enabled) return;

            Plugin.Logger.Warn($"Loading counter {settings.DisplayName}...");

            if (typeof(R).BaseType == typeof(MonoBehaviour))
            {
                Container.BindInterfacesAndSelfTo<R>().FromComponentOnRoot().NonLazy();
            }
            else
            {
                Container.BindInterfacesAndSelfTo<R>().AsSingle().NonLazy();
            }
        }
    }
}
