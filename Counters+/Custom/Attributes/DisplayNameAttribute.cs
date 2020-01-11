using System;

namespace CountersPlus.Custom.Attributes
{
    /// <summary>
    /// Display Name for the counter, that will go above whatever you are trying to show.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DisplayNameAttribute : Attribute
    {
    }
}
