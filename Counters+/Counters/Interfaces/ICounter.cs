namespace CountersPlus.Counters.Interfaces
{
    /// <summary>
    /// The base interface for a counter.
    /// This is barebones, and only contains events pertaining to the initialization and destruction of a counter.
    /// To access more events, inherit from event handlers such as <see cref="INoteEventHandler"/>.
    /// </summary>
    public interface ICounter : IEventHandler
    {
        void CounterInit();

        void CounterDestroy();
    }
}
