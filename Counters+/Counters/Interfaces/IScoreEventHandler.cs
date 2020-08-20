namespace CountersPlus.Counters.Interfaces
{
    /// <summary>
    /// An interface that exposes certain scoring-related events. Used with conjuction with a <see cref="ICounter"/>.
    /// </summary>
    public interface IScoreEventHandler : IEventHandler
    {
        void ScoreUpdated(int modifiedScore);

        void MaxScoreUpdated(int maxModifiedScore);
    }
}
