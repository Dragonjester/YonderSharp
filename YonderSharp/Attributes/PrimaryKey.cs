using System;

namespace YonderSharp.Attributes
{
    /// <summary>
    /// Identifies something as the unique primary key. Usually, but not necessarily a <see cref="Guid"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
                       AllowMultiple = false)]
    public class PrimaryKey : Attribute
    {
    }
}
