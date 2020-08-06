using CountersPlus.Config;
using UnityEngine;

namespace CountersPlus.Counters
{
    public abstract class Counter<T> : MonoBehaviour, ICounter where T : ConfigModel
    {
        internal T settings;

        private void Start()
        {
            Counter_Start();
            CountersController.ReadyToInit += InternalInit;
        }

        private void InternalInit(CountersData data)
        {
            Init(data, DeterminePosition(this, data));
        }

        internal abstract void Init(CountersData data, Vector3 textPosition);
        internal abstract void Counter_Start();
        internal abstract void Counter_Destroy();

        //We have a local copy here so that we can do some optimizing that isn't available in MenuCore.
        protected Vector3 DeterminePosition(Counter<T> counter, CountersData data)
        {
            float comboOffset = CountersController.settings.ComboOffset;
            float multOffset = CountersController.settings.MultiplierOffset;
            ICounterPositions position = settings.Position;
            int index = settings.Distance;
            Vector3 pos = new Vector3(); //Base position
            Vector3 offset = new Vector3(0, -0.75f * (index), 0); //Offset for any overlapping, indexes, etc.
            bool hud360 = CountersController.settings.hudConfig.AttachToBaseGameHUDFor360 && data.Is360Or90Level;
            float X = hud360 ? 2f : 3.2f;
            switch (position)
            {
                case ICounterPositions.BelowCombo:
                    pos = new Vector3(-X, 1.15f - comboOffset, 7);
                    break;
                case ICounterPositions.AboveCombo:
                    pos = new Vector3(-X, 2f + comboOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case ICounterPositions.BelowMultiplier:
                    pos = new Vector3(X, 1.05f - multOffset, 7);
                    break;
                case ICounterPositions.AboveMultiplier:
                    pos = new Vector3(X, 2f + multOffset, 7);
                    offset = new Vector3(0, (offset.y * -1) + 0.75f, 0);
                    break;
                case ICounterPositions.BelowEnergy:
                    pos = new Vector3(0, hud360 ? -0.25f : -1.5f, 7);
                    break;
                case ICounterPositions.AboveHighway:
                    pos = new Vector3(0, 2.5f, 7);
                    offset = new Vector3(0, (offset.y * -1) + (hud360 ? 0.25f : 0.75f), 0);
                    break;
            }
            if (counter is ProgressCounter)
            {
                if ((settings as ProgressConfigModel).Mode == ICounterMode.Original) offset += new Vector3(0.25f, 0, 0);
            }
            return pos + offset;
        }

        internal void OnDestroy()
        {
            Counter_Destroy();
            try
            {
                CountersController.ReadyToInit -= InternalInit;
            }
            catch
            {
                Plugin.Log($"Could not properly destroy Counter {GetType().Name}!", LogInfo.Warning);
            }
        }
    }
}
