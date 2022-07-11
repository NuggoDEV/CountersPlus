using CountersPlus.ConfigModels;
using CountersPlus.Counters;
using CountersPlus.Custom;
using CountersPlus.Multiplayer;
using UnityEngine;

namespace CountersPlus.Installers
{
    internal class MultiplayerCountersInstaller : CountersInstaller
    {
        public override void InstallBindings()
        {
            // haha i made goobie beg me for this one line of code
            if (!Plugin.MainConfig.Enabled) return;

            // If CoreGameHUD isn't bound already (by MultEx or others), we need to bind it ourselves
            var coreGameHUD = Container.TryResolve<CoreGameHUDController>();
            if (coreGameHUD == null)
            {
                // Thankfully the CoreGameHUD is just a child of the installer GameObject
                coreGameHUD = GetComponentInChildren<CoreGameHUDController>();
                Container.Bind<CoreGameHUDController>().FromInstance(coreGameHUD).AsSingle();

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
            }

            // For the multiplayer rank counter, we also need to bind the Multiplayer Position
            Container.Bind<MultiplayerPositionHUDController>()
                .FromInstance(GetComponentInChildren<MultiplayerPositionHUDController>())
                .AsSingle();

            Container.BindInterfacesAndSelfTo<CanvasIntroFadeController>().AsSingle();

            // Install counters like normal
            base.InstallBindings();
            
            // Then add our multiplayer rank counter
            AddCounter<MultiplayerRankConfigModel, MultiplayerRankCounter>();
        }

        protected override void InstallCustomCounters()
        {
            // Only load multiplayer-ready custom counters
            foreach (CustomCounter customCounter in Plugin.LoadedCustomCounters.FindAll(x => x.MultiplayerReady))
            {
                AddCustomCounter(customCounter, customCounter.CounterType);
            }
        }
    }
}
