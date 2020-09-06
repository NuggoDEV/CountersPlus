using CountersPlus.ConfigModels;
using CountersPlus.Counters;
using CountersPlus.Counters.Event_Broadcasters;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Counters.NoteCountProcessors;
using CountersPlus.Utils;
using IPA.Loader;
using SiraUtil.Zenject;
using System;
using UnityEngine;
using Zenject;

namespace CountersPlus.Installers
{
    [RequiresInstaller(typeof(ConfigModelInstaller))]
    class CountersInstaller : MonoInstaller
    {
        private HUDConfigModel hudConfig;
        private PlayerDataModel dataModel;

        public override void InstallBindings()
        {
            MainConfigModel mainConfig = Plugin.MainConfig;
            
            hudConfig = Container.Resolve<HUDConfigModel>();
            dataModel = Container.Resolve<PlayerDataModel>();

            if (!mainConfig.Enabled) return;

            /// LOADING IMPORTANT SHIT LIKE CANVASES AND STUFF ///
            Container.Bind<CanvasUtility>().AsSingle().NonLazy();

            if (PluginManager.GetPlugin("CustomJSONData") != null)
            {
                Container.Bind<NoteCountProcessor>().To<CustomJSONDataNoteCountProcessor>().AsSingle().NonLazy();
            }
            else
            {
                Container.Bind<NoteCountProcessor>().To<GenericNoteCountProcessor>().AsSingle().NonLazy();
            }

            /// LOADING COUNTERS ///
            Plugin.Logger.Notice("Loading counters...");

            AddCounter<MissedConfigModel, MissedCounter>();
            AddCounter<NoteConfigModel, NotesCounter>();

            if (mainConfig.ProgressConfig.Mode != ProgressMode.BaseGame)
            {
                AddCounter<ProgressConfigModel, ProgressCounter>();
            }
            else
            {
                AddCounter<ProgressConfigModel, ProgressBaseGameCounter>();
            }

            AddCounter<ScoreConfigModel, ScoreCounter>();
            AddCounter<CutConfigModel, CutCounter>();
            AddCounter<FailConfigModel, FailCounter>();
            AddCounter<NotesLeftConfigModel, NotesLeftCounter>();
            AddCounter<SpeedConfigModel, SpeedCounter>();
            AddCounter<SpinometerConfigModel, Spinometer>();


            AddCounter<PBConfigModel, PBCounter>((settings) => {
                ScoreConfigModel scoreConfig = Container.Resolve<ScoreConfigModel>();
                HUDCanvas canvasSettings = scoreConfig.CanvasID == -1 ? hudConfig.MainCanvasSettings : hudConfig.OtherCanvasSettings[scoreConfig.CanvasID];
                return scoreConfig.Enabled && settings.UnderScore && (dataModel.playerData.playerSpecificSettings.noTextsAndHuds ? canvasSettings.IgnoreNoTextAndHUDOption : true);
                });

            foreach (Custom.CustomCounter customCounter in Plugin.LoadedCustomCounters.Values)
            {
                AddCustomCounter(customCounter, customCounter.CounterType);
            }

            /// LOADING BROADCASTERS WITH BROADCAST IN-GAME EVENTS TO COUNTERS AND STUFF ///
            Container.BindInterfacesAndSelfTo<CounterEventBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<NoteEventBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScoreEventBroadcaster>().AsSingle().NonLazy();
            Plugin.Logger.Notice("Counters loaded!");
        }

        private void AddCounter<T, R>() where T : ConfigModel where R : ICounter
        {
            AddCounter<T, R>(_ => true);
        }

        private void AddCounter<T, R>(Func<T, bool> additionalReasonToSpawn) where T : ConfigModel where R : ICounter
        {
            T settings = Container.Resolve<T>();

            HUDCanvas canvasSettings = settings.CanvasID == -1 ? hudConfig.MainCanvasSettings : hudConfig.OtherCanvasSettings[settings.CanvasID];

            if (!settings.Enabled || (!canvasSettings.IgnoreNoTextAndHUDOption && dataModel.playerData.playerSpecificSettings.noTextsAndHuds
                && !additionalReasonToSpawn(settings))) return;

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

        private void AddCustomCounter(Custom.CustomCounter customCounter, Type counterType)
        {
            ConfigModel settings = Container.TryResolveId<ConfigModel>(customCounter.Name);

            HUDCanvas canvasSettings = settings.CanvasID == -1 ? hudConfig.MainCanvasSettings : hudConfig.OtherCanvasSettings[settings.CanvasID];

            if (!settings.Enabled || (!canvasSettings.IgnoreNoTextAndHUDOption && dataModel.playerData.playerSpecificSettings.noTextsAndHuds)) return;

            Plugin.Logger.Debug($"Loading counter {customCounter.Name}...");

            if (counterType.BaseType == typeof(MonoBehaviour))
            {
                Container.BindInterfacesAndSelfTo(counterType).FromComponentOnRoot().NonLazy();
            }
            else
            {
                Container.BindInterfacesAndSelfTo(counterType).AsSingle().NonLazy();
            }
        }
    }
}
