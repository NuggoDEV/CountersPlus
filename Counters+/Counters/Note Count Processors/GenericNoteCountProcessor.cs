namespace CountersPlus.Counters.NoteCountProcessors
{
    /// <summary>
    /// Generic processor for use in the vanilla game.
    /// </summary>
    public class GenericNoteCountProcessor : NoteCountProcessor
    {
        public override bool ShouldIgnoreNote(NoteData data) => false;
    }
}
