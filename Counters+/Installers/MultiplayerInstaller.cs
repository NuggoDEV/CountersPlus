using Zenject;

namespace CountersPlus.Installers
{
    internal class MultiplayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // We need a separate installer to bind the HUD Controller so counters can use it
            Container.Bind<CoreGameHUDController>()
                .FromInstance(GetComponentInChildren<CoreGameHUDController>())
                .AsSingle();
        }
    }
}
