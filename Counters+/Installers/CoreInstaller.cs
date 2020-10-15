using CountersPlus.ConfigModels;
using CountersPlus.Custom;
using CountersPlus.Utils;
using Zenject;

namespace CountersPlus.Installers
{
    public class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<VersionUtility>().AsSingle().NonLazy();

            MainConfigModel mainConfig = Plugin.MainConfig;

            Container.Bind<MainConfigModel>().FromInstance(mainConfig);
            mainConfig.HUDConfig.MainCanvasSettings.IsMainCanvas = true;
            Container.Bind<HUDConfigModel>().FromInstance(mainConfig.HUDConfig);

            BindConfig<MissedConfigModel>(mainConfig.MissedConfig);
            BindConfig<NoteConfigModel>(mainConfig.NoteConfig);
            BindConfig<ProgressConfigModel>(mainConfig.ProgressConfig);
            BindConfig<ScoreConfigModel>(mainConfig.ScoreConfig);
            BindConfig<SpeedConfigModel>(mainConfig.SpeedConfig);
            BindConfig<SpinometerConfigModel>(mainConfig.SpinometerConfig);
            BindConfig<PBConfigModel>(mainConfig.PBConfig);
            BindConfig<CutConfigModel>(mainConfig.CutConfig);
            BindConfig<FailConfigModel>(mainConfig.FailsConfig);
            BindConfig<NotesLeftConfigModel>(mainConfig.NotesLeftConfig);

            foreach (CustomCounter customCounter in Plugin.LoadedCustomCounters.Values)
            {
                if (!mainConfig.CustomCounters.TryGetValue(customCounter.Name, out CustomConfigModel config))
                {
                    config = customCounter.ConfigDefaults;
                    mainConfig.CustomCounters.Add(customCounter.Name, config);
                }
                config.AttachedCustomCounter = customCounter;
                customCounter.Config = config;
                BindCustomCounter(customCounter, config);
            }
        }

        // Helper function, allows easy modification to how configs are binded to zenject
        private void BindConfig<T>(T settings) where T : ConfigModel
        {
            Container.BindInterfacesAndSelfTo<T>().FromInstance(settings).AsCached();
            Container.Bind<ConfigModel>().To<T>().FromInstance(settings).AsCached();
        }

        // Is this too much? Probably.
        private void BindCustomCounter(CustomCounter counter, CustomConfigModel settings)
        {
            Container.Bind<ConfigModel>().WithId(counter.Name).To<CustomConfigModel>().FromInstance(settings).AsCached();
            Container.Bind<ConfigModel>().To<CustomConfigModel>().FromInstance(settings).AsCached();
            Container.BindInterfacesAndSelfTo<CustomConfigModel>().FromInstance(settings).WhenInjectedInto(counter.CounterType);
        }
    }
}
