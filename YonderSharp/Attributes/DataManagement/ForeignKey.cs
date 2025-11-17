using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YonderSharp.Attributes.DataManagement
{
    /// <summary>
    /// A Foreign-Key that points to some <see cref="PrimaryKey"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
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
            try
            {
                TargetClass = targetClass ?? throw new ArgumentNullException(nameof(targetClass));
                TargetField = TargetClass.GetProperties().Where(x => x.Name == fieldName).First();
            }
            catch (Exception e)
            {
                throw new Exception($"Missing {fieldName} in {targetClass.Name}", e);
            }
        }

        /// <summary>
        /// Returns the types that are referenced as Foreign for the given type.
        /// i.e. if the given type has foreign relations to the types "book" and "shop" it will return those
        /// </summary>
        public static IEnumerable<Type> GetAllForeignTables(Type type)
        {
            HashSet<Type> knownResults = new HashSet<Type>();
            foreach (var property in type.GetProperties())
            {
                var fkAttribute = property.GetCustomAttribute<ForeignKey>();
                if (fkAttribute != null && knownResults.Add(fkAttribute.TargetClass))
                {
                    yield return fkAttribute.TargetClass;
                }
            }
        }
    }
}