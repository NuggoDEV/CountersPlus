using CountersPlus.ConfigModels;
using Zenject;

namespace CountersPlus.Installers
{
    public class ConfigModelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MainConfigModel>().FromInstance(MainConfigModel.Instance);
            Container.Bind<HUDConfigModel>().FromInstance(MainConfigModel.Instance.HUDConfig);

            Container.Bind<MissedConfigModel>().FromInstance(MainConfigModel.Instance.MissedConfig);
            Container.Bind<NoteConfigModel>().FromInstance(MainConfigModel.Instance.NoteConfig);
            Container.Bind<ProgressConfigModel>().FromInstance(MainConfigModel.Instance.ProgressConfig);
            Container.Bind<ScoreConfigModel>().FromInstance(MainConfigModel.Instance.ScoreConfig);
            Container.Bind<SpeedConfigModel>().FromInstance(MainConfigModel.Instance.SpeedConfig);
            Container.Bind<SpinometerConfigModel>().FromInstance(MainConfigModel.Instance.SpinometerConfig);
            Container.Bind<PBConfigModel>().FromInstance(MainConfigModel.Instance.PBConfig);
            Container.Bind<CutConfigModel>().FromInstance(MainConfigModel.Instance.CutConfig);
            Container.Bind<FailConfigModel>().FromInstance(MainConfigModel.Instance.FailsConfig);
            Container.Bind<NotesLeftConfigModel>().FromInstance(MainConfigModel.Instance.NotesLeftConfig);
        }
    }
}
