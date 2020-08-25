using CountersPlus.ConfigModels;
using CountersPlus.Custom;
using CountersPlus.Utils;
using Zenject;

namespace CountersPlus.Installers
{
    public class ConfigModelInstaller : MonoInstaller
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
                BindConfigWithID<CustomConfigModel>(config, customCounter.Name);
            }
        }

        // Helper function, allows easy modification to how configs are binded to zenject
        private void BindConfig<T>(T settings) where T : ConfigModel
        {
            Container.BindInterfacesAndSelfTo<T>().FromInstance(settings).AsCached();
            Container.Bind<ConfigModel>().To<T>().FromInstance(settings).AsCached();
        }

        private void BindConfigWithID<T>(T settings, object id) where T : ConfigModel
        {
            Container.Bind<ConfigModel>().WithId(id).To<T>().FromInstance(settings).AsCached();
            Container.Bind<ConfigModel>().To<T>().FromInstance(settings).AsCached();
        }
    }
}
