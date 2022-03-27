using CountersPlus.ConfigModels;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.Counters
{
    // REVIEW: Perhaps look into harmony patches to take control of the base game score counter more... sanely?
    internal class MultiplayerScoreCounter : ScoreCounter
    {
        [Inject] private CoreGameHUDController coreGameHUD;

        public override void CounterInit()
        {
            // Move multiplayer stuff to standard position
            GameEnergyUIPanel energyUIPanel = coreGameHUD.GetComponentInChildren<GameEnergyUIPanel>();
            energyUIPanel.transform.position = new Vector3(0, -0.64f, 7.75f);
            energyUIPanel.transform.rotation = Quaternion.Euler(0, 0, 0);
            ComboUIController comboUI = coreGameHUD.GetComponentInChildren<ComboUIController>();
            comboUI.transform.position = new Vector3(-3.2f, 1.83f, 7f);
            comboUI.transform.rotation = Quaternion.Euler(0, 0, 0);
            ScoreMultiplierUIController multiplierUI = coreGameHUD.GetComponentInChildren<ScoreMultiplierUIController>();
            multiplierUI.transform.position = new Vector3(3.2f, 1.7f, 7f);
            multiplierUI.transform.rotation = Quaternion.Euler(0, 0, 0);
            Object.Destroy(coreGameHUD.GetComponentInChildren<SongProgressUIController>().gameObject);

            base.CounterInit();
        }
    }
}
