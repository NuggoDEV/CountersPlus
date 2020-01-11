using System;

namespace CountersPlus.Custom.Attributes
{
    /// <summary>
    /// Triggered when the counter is destroyed. Used for unsubscribing from events and resetting variables to defaults.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CounterDestroyAttribute : Attribute
    {
    }
}
