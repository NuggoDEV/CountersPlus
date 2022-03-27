using CountersPlus.ConfigModels;
using CountersPlus.Counters;
using CountersPlus.Counters.Event_Broadcasters;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Counters.NoteCountProcessors;
using CountersPlus.Utils;
using IPA.Utilities;
using Zenject;

namespace CountersPlus.Installers
{
    class MultiplayerInstaller : MonoInstaller
    {
        [Inject]
        private MultiplayerPlayersManager _playersManager;

        private readonly FieldAccessor<MultiplayerPlayersManager, MultiplayerLocalActivePlayerFacade>.Accessor _activeLocalPlayerFacade = FieldAccessor<MultiplayerPlayersManager, MultiplayerLocalActivePlayerFacade>.GetAccessor("_activeLocalPlayerFacade");

        public override void InstallBindings()
        {
            _playersManager.playerSpawningDidFinishEvent += InitializeCounters;
        }

        private void InitializeCounters()
        {
            var activeLocalPlayer = _activeLocalPlayerFacade(ref _playersManager);
            Container.Bind<CoreGameHUDController>().FromInstance(activeLocalPlayer.GetComponentInChildren<CoreGameHUDController>());

            var scoreCounter = Container.Resolve<MultiplayerScoreCounter>();
            Container.Inject(scoreCounter);
            scoreCounter.Init();

            _playersManager.playerSpawningDidFinishEvent -= InitializeCounters;
        }
    }
}
