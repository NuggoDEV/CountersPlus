using CountersPlus.ConfigModels;
using CountersPlus.ConfigModels.SettableSettings;
using CountersPlus.Custom;
using CountersPlus.Utils;
using IPA.Loader;
using Zenject;

namespace CountersPlus.Installers
{
    public class CoreInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<VersionUtility>().AsSingle().NonLazy();

            MainConfigModel mainConfig = Plugin.MainConfig;

            Container.Bind<MainConfigModel>().FromInstance(mainConfig);
            mainConfig.HUDConfig.MainCanvasSettings.IsMainCanvas = true;
            Container.Bind<HUDConfigModel>().FromInstance(mainConfig.HUDConfig);

            BindConfig(mainConfig.MissedConfig);
            BindConfig(mainConfig.NoteConfig);
            BindConfig(mainConfig.ProgressConfig);
            BindConfig(mainConfig.ScoreConfig);
            BindConfig(mainConfig.SpeedConfig);
            BindConfig(mainConfig.SpinometerConfig);
            BindConfig(mainConfig.PBConfig);
            BindConfig(mainConfig.CutConfig);
            BindConfig(mainConfig.FailsConfig);
            BindConfig(mainConfig.NotesLeftConfig);
            BindConfig(mainConfig.MultiplayerRankConfig);

            foreach (CustomCounter customCounter in Plugin.LoadedCustomCounters)
            {
                if (!mainConfig.CustomCounters.TryGetValue(customCounter.Name, out CustomConfigModel config))
                {
                    config = customCounter.ConfigDefaults;
                    mainConfig.CustomCounters.Add(customCounter.Name, config);
                }

                config.DisplayName = customCounter.Name;
                config.AttachedCustomCounter = customCounter;
                customCounter.Config = config;
                BindCustomCounter(customCounter, config);
            }

            if (PluginManager.GetPlugin("Heck") != null)
            {
                InstallHeckCompatibility();
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

        private void InstallHeckCompatibility()
        {
            Container.BindInterfacesAndSelfTo<CountersPlusSettableSettings>().AsSingle().NonLazy();
        }
    }
}
