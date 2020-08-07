using Zenject;
using CountersPlus.ConfigModels;

namespace CountersPlus.Counters.Interfaces
{
    /// <summary>
    /// The base class for a counter, which also supplies a <see cref="ConfigModel"/> for the counter to use.
    /// This is barebones, and only contains events pertaining to the initialization and destruction of a counter.
    /// To access more events, inherit from event handlers such as <see cref="INoteEventHandler"/>.
    /// </summary>
    public class Counter<T> : ICounter where T : ConfigModel
    {
        [Inject] protected T Settings;

        public virtual void CounterInit() { }

        public virtual void CounterDestroy() { }
    }
}
