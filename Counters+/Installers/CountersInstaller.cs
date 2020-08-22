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
        private HUDConfigModel hudConfig;
        private PlayerDataModel dataModel;

        public override void InstallBindings()
        {
            MainConfigModel mainConfig = Plugin.MainConfig;
            
            hudConfig = Container.Resolve<HUDConfigModel>();
            dataModel = Container.Resolve<PlayerDataModel>();

            if (!mainConfig.Enabled || dataModel.playerData.playerSpecificSettings.noTextsAndHuds) return;

            /// LOADING IMPORTANT SHIT LIKE CANVASES AND STUFF ///
            Container.Bind<CanvasUtility>().AsSingle().NonLazy();

            /// LOADING COUNTERS ///
            Plugin.Logger.Notice("Loading counters...");

            AddCounter<MissedConfigModel, MissedCounter>();
            AddCounter<NoteConfigModel, NotesCounter>();

            AddCounter<ProgressConfigModel, ProgressCounter>();
            /*if (mainConfig.ProgressConfig.Mode != ProgressMode.BaseGame)
            {
                AddCounter<ProgressConfigModel, ProgressCounter>();
            }*/ // TODO add base game variant for progress counter

            AddCounter<ScoreConfigModel, ScoreCounter>();
            AddCounter<CutConfigModel, CutCounter>();
            AddCounter<FailConfigModel, FailCounter>();
            AddCounter<NotesLeftConfigModel, NotesLeftCounter>();
            AddCounter<PBConfigModel, PBCounter>();
            AddCounter<SpeedConfigModel, SpeedCounter>();
            AddCounter<SpinometerConfigModel, Spinometer>();

            /// LOADING BROADCASTERS WITH BROADCAST IN-GAME EVENTS TO COUNTERS AND STUFF ///
            Container.BindInterfacesAndSelfTo<CounterEventBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<NoteEventBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScoreEventBroadcaster>().AsSingle().NonLazy();
            Plugin.Logger.Notice("Counters loaded!");
        }

        private void AddCounter<T, R>() where T : ConfigModel where R : ICounter
        {
            T settings = Container.Resolve<T>();

            HUDCanvas canvasSettings = settings.CanvasID == -1 ? hudConfig.MainCanvasSettings : hudConfig.OtherCanvasSettings[settings.CanvasID];

            if (!settings.Enabled || (!canvasSettings.IgnoreNoTextAndHUDOption && dataModel.playerData.playerSpecificSettings.noTextsAndHuds)) return;

            Plugin.Logger.Debug($"Loading counter {settings.DisplayName}...");

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
