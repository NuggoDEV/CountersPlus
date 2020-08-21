using CountersPlus.ConfigModels;
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
        }

        // Helper function, allows easy modification to how configs are binded to zenject
        private void BindConfig<T>(T settings) where T : ConfigModel
        {
            Container.BindInterfacesAndSelfTo<T>().FromInstance(settings).AsCached();
            Container.Bind<ConfigModel>().To<T>().FromInstance(settings).AsCached();
        }
    }
}
