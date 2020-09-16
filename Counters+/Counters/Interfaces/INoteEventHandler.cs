namespace CountersPlus.Counters.Interfaces
{
    /// <summary>
    /// An interface that exposes certain note-related events. Used with conjuction with a <see cref="ICounter"/>.
    /// </summary>
    public interface INoteEventHandler : IEventHandler
    {
        void OnNoteCut(NoteData data, NoteCutInfo info);

        void OnNoteMiss(NoteData data);
    }
}
