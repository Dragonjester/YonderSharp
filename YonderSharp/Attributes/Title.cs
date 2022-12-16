using System;
using System.Linq;
using System.Reflection;

namespace YonderSharp.Attributes
{
    /// <summary>
    /// used to mark the member that is shown in the UI for FKs
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
                      AllowMultiple = false)]
    public class Title : Attribute
    {

        /// <summary>
        /// Returns the titel value for the given object
        /// </summary>
        public static string GetTitel(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.GetCustomAttribute<Title>() != null)
                {
                    return property.GetValue(obj).ToString();
                }
            }

            return obj.ToString();
        }

        /// <summary>
        /// Does the Type of the given object contain a property that is declared [Title]
        /// </summary>
        public static bool TypeHasTitelAttribute(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return TypeHasTitelAttribute(obj.GetType());
        }

        /// <summary>
        /// Does the Type contain a property that is declared [Title]
        /// </summary>
        public static bool TypeHasTitelAttribute(Type type)
        {
            if (type == null)
            {
                return false;
            }

            //In case your property isn't checked: onyl those with a getter and setter are found
           return type.GetProperties().Any(x => x.GetCustomAttribute<Title>() != null);
        }
    }
}
