using System;

namespace CountersPlus.Custom.Attributes
{
    /// <summary>
    /// Attach this to an <see cref="Action"/> and invoke it to cause the text of the counter to refresh, pulling data
    /// from any <see cref="DisplayNameAttribute"/> and <see cref="FormattedTextAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RefreshCounterAttribute : Attribute
    {
    }
}
