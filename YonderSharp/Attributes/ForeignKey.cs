using System;
using System.Linq;
using System.Reflection;

namespace YonderSharp.Attributes
{
    /// <summary>
    /// A Foreign-Key that points to some <see cref="PrimaryKey"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
                   AllowMultiple = false)]
    public class ForeignKey : Attribute
    {
        /// <summary>
        /// Type to which this ForeignKey points to
        /// </summary>
        public Type TargetClass { get; set; }

        /// <summary>
        /// Field of the TargetClass to which the ForeignKey points to
        /// </summary>
        public PropertyInfo TargetField { get; set; }

        /// <param name="targetClass">Class to which this FK points to</param>
        /// <param name="fieldName">Property of this class, to where the FK points to</param>
        /// <exception cref="ArgumentNullException">For the params className and fieldName</exception>
        public ForeignKey(Type targetClass, string fieldName)
        {
            TargetClass = targetClass ?? throw new ArgumentNullException(nameof(targetClass));
            TargetField = TargetClass.GetProperties().Where(x => x.Name == fieldName).First();
        }
    }
}