using CountersPlus.Config;
using UnityEngine;

namespace CountersPlus.Counters
{
    public abstract class Counter<T> : MonoBehaviour where T : ConfigModel
    {
        internal T settings;

        private void Start()
        {
            Counter_Start();
            CountersController.ReadyToInit += Init;
        }

        internal abstract void Init(CountersData data);
        internal abstract void Counter_Start();
        internal abstract void Counter_Destroy();

        internal void OnDestroy()
        {
            Counter_Destroy();
            try
            {
                CountersController.ReadyToInit -= Init;
            }
            catch
            {
                Plugin.Log($"Could not properly destroy Counter {GetType().Name}!", LogInfo.Warning);
            }
        }
    }
}
