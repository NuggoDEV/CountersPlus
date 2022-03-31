using CountersPlus.ConfigModels;
using CountersPlus.Counters;
using CountersPlus.Multiplayer;
using UnityEngine;

namespace CountersPlus.Installers
{
    internal class MultiplayerCountersInstaller : CountersInstaller
    {
        public override void InstallBindings()
        {
            var coreGameHUD = GetComponentInChildren<CoreGameHUDController>();

            // We need a separate installer to bind the HUD Controller so counters can use it
            Container.Bind<CoreGameHUDController>().FromInstance(coreGameHUD).AsSingle();

            // For the multiplayer rank counter, we also need to bind the Multiplayer Position
            Container.Bind<MultiplayerPositionHUDController>()
                .FromInstance(GetComponentInChildren<MultiplayerPositionHUDController>())
                .AsSingle();

            Container.BindInterfacesAndSelfTo<CanvasIntroFadeController>().AsSingle();

            // Change base game HUD elements to standard position
            var energyUIPanel = coreGameHUD.GetComponentInChildren<GameEnergyUIPanel>();
            energyUIPanel.transform.position = new Vector3(0, -0.64f, 7.75f);
            energyUIPanel.transform.rotation = Quaternion.Euler(0, 0, 0);

            var comboUI = coreGameHUD.GetComponentInChildren<ComboUIController>();
            comboUI.transform.position = new Vector3(-3.2f, 1.83f, 7f);
            comboUI.transform.rotation = Quaternion.Euler(0, 0, 0);

            var multiplierUI = coreGameHUD.GetComponentInChildren<ScoreMultiplierUIController>();
            multiplierUI.transform.position = new Vector3(3.2f, 1.7f, 7f);
            multiplierUI.transform.rotation = Quaternion.Euler(0, 0, 0);

            // Install counters like normal
            base.InstallBindings();
            
            // Then add our multiplayer rank counter
            AddCounter<MultiplayerRankConfigModel, MultiplayerRankCounter>();
        }
    }
}
