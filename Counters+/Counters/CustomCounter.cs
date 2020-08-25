using CountersPlus.Counters.Interfaces;
using CountersPlus.Custom;
using CountersPlus.Utils;
using System.Reflection;
using Zenject;

namespace CountersPlus.Counters
{
    public abstract class CustomCounter : ICounter
    {
        /// <summary>
        /// Helper class for creating text within Counters+'s system.
        /// Not recommended for creating text belonging outside of Counters+.
        /// </summary>
        [Inject] protected CanvasUtility CanvasUtility;

        /// <summary>
        /// Obtains your <see cref="CustomConfigModel"/> by searching for one with a matching <see cref="Assembly"/>.
        /// It is recommended to cache this config model for later reuse.
        /// </summary>
        /// <returns>A matching <see cref="CustomConfigModel"/></returns>
        protected CustomConfigModel GetConfig()
        {
            Assembly callingAssembly = GetType().Assembly;
            if (Plugin.LoadedCustomCounters.TryGetValue(callingAssembly, out Custom.CustomCounter customCounter))
            {
                return customCounter.Config;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Called when the Counter has initialized; it is ready to create text and subscribe to events.
        /// </summary>
        public abstract void CounterInit();

        /// <summary>
        /// Called when a Counter is destroyed; it is ready to cleanup and unsubscribe from events.
        /// </summary>
        public abstract void CounterDestroy();
    }
}
