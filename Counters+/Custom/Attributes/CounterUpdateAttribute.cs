using System;

namespace CountersPlus.Custom.Attributes
{
    /// <summary>
    /// Similar to MonoBehaviour.Update, except with an Atribute instead of a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CounterUpdateAttribute : Attribute
    {
    }
}
