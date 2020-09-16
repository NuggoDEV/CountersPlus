using CountersPlus.Counters.Interfaces;
using CountersPlus.Custom;
using CountersPlus.Utils;
using Zenject;

namespace CountersPlus.Counters.Custom
{
    /// <summary>
    /// A barebones Custom Counter class that provides utilities for creating custom text.
    /// </summary>
    public abstract class BasicCustomCounter : ICounter
    {
        /// <summary>
        /// Helper class for creating text within Counters+'s system.
        /// Not recommended for creating text belonging outside of Counters+.
        /// </summary>
        [Inject] protected CanvasUtility CanvasUtility;

        /// <summary>
        /// The <see cref="CustomConfigModel"/> for your Custom Counter.
        /// Use it to help position your text with <see cref="CanvasUtility"/>.
        /// </summary>
        [Inject] protected CustomConfigModel Settings;

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
