using System;

namespace YonderSharp.Attributes.DataManagement
{

    /// <summary>
    /// Field is disabled in UI 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class Disabled : Attribute
    {
    }
}
