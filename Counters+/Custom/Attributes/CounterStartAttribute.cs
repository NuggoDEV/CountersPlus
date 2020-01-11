using System;

namespace CountersPlus.Custom.Attributes
{
    /// <summary>
    /// Triggered when the counter has started. Used for finding Unity objects and attaching delegates to events.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CounterStartAttribute : Attribute
    {
    }
}
