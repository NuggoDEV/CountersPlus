using System;

namespace CountersPlus.Custom
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RefreshCounterAttribute : Attribute
    {
    }
}
