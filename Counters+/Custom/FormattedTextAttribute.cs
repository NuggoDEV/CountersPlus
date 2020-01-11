using System;

namespace CountersPlus.Custom
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormattedTextAttribute : Attribute
    {
    }
}
