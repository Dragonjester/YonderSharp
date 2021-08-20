using System;

namespace YonderSharp.Attributes
{
    /// <summary>
    /// used to mark the member that is shown in the UI for FKs
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
                      AllowMultiple = false)]
    public class Title : Attribute
    {
    }
}
