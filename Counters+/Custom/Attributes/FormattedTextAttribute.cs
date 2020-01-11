using System;

namespace CountersPlus.Custom.Attributes
{
    /// <summary>
    /// Formatted Text for your counter, that will appear underneath the display name.
    /// More than likely you'd want to just call <see cref="int.ToString()"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormattedTextAttribute : Attribute
    {
    }
}
