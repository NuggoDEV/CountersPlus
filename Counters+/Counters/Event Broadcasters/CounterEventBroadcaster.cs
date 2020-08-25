using CountersPlus.Counters.Interfaces;

namespace CountersPlus.Counters.Event_Broadcasters
{
    /// <summary>
    /// A simple <see cref="EventBroadcaster{T}"/> that broadcasts the creation and destruction of counters.
    /// </summary>
    internal class CounterEventBroadcaster : EventBroadcaster<ICounter>
    {
        public override void Initialize()
        {
            foreach (ICounter counter in EventHandlers)
            {
                counter.CounterInit();
            }
        }

        public override void Dispose()
        {
            foreach (ICounter counter in EventHandlers)
            {
                counter.CounterDestroy();
            }
        }
    }
}
