namespace CountersPlus.Counters.Interfaces
{
    /// <summary>
    /// The base interface for a counter.
    /// This is barebones, and only contains events pertaining to the initialization and destruction of a counter.
    /// </summary>
    public interface ICounter : IEventHandler
    {
        void CounterInit();

        void CounterDestroy();
    }
}
