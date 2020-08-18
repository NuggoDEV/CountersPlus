using CountersPlus.ConfigModels;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.Utils;
using Zenject;

namespace CountersPlus.Installers
{
    public class ConfigModelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Plugin.Logger.Warn("Binding config models");
            Container.Bind<VersionUtility>().AsSingle().NonLazy();

            Container.Bind<MainConfigModel>().FromInstance(MainConfigModel.Instance);
            Container.Bind<HUDConfigModel>().FromInstance(MainConfigModel.Instance.HUDConfig);

            BindConfig<MissedConfigModel>(MainConfigModel.Instance.MissedConfig);
            BindConfig<NoteConfigModel>(MainConfigModel.Instance.NoteConfig);
            BindConfig<ProgressConfigModel>(MainConfigModel.Instance.ProgressConfig);
            BindConfig<ScoreConfigModel>(MainConfigModel.Instance.ScoreConfig);
            BindConfig<SpeedConfigModel>(MainConfigModel.Instance.SpeedConfig);
            BindConfig<SpinometerConfigModel>(MainConfigModel.Instance.SpinometerConfig);
            BindConfig<PBConfigModel>(MainConfigModel.Instance.PBConfig);
            BindConfig<CutConfigModel>(MainConfigModel.Instance.CutConfig);
            BindConfig<FailConfigModel>(MainConfigModel.Instance.FailsConfig);
            BindConfig<NotesLeftConfigModel>(MainConfigModel.Instance.NotesLeftConfig);
        }

        // Helper function, allows easy modification to how configs are binded to zenject
        private void BindConfig<T>(T settings) where T : ConfigModel
        {
            Container.BindInterfacesAndSelfTo<T>().FromInstance(settings).AsCached();
            Container.Bind<ConfigModel>().To<T>().FromInstance(settings).AsCached();
        }
    }
}
