using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YonderSharp.Attributes.DataManagement;

namespace YonderSharp.Test.Attributes.KeyTests
{
    [TestFixture]
    public class KeyTest
    {
        [Test]
        public void PrimaryKeysCanBeInitialized()
        {
            PrimaryClass primary = new PrimaryClass();
            Assert.IsNotNull(primary);

            var properties = typeof(PrimaryClass).GetProperties().Where(prop => prop.IsDefined(typeof(PrimaryKey), false)).ToList();
            Assert.IsNotNull(properties);
            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual("ID", properties[0].Name);
        }

        [Test]
        public void HowToUseForeignKey()
        {
            ForeignClass foreign = new ForeignClass();
            List<PrimaryClass> primaryTable = new List<PrimaryClass>();
            primaryTable.Add(new PrimaryClass() { ID = foreign.RefId });

            PropertyInfo fkProperty = typeof(ForeignClass).GetProperties().Where(prop => prop.IsDefined(typeof(ForeignKey), false)).First();
            ForeignKey fkAttribute = (ForeignKey)fkProperty.GetCustomAttributes(typeof(ForeignKey), false).First();

            Assert.AreEqual(typeof(PrimaryClass), fkAttribute.TargetClass);
            Assert.AreEqual("ID", fkAttribute.TargetField.Name);

            PropertyInfo pkPropertyOfPrimary = fkAttribute.TargetClass.GetProperties().Where(x => x == fkAttribute.TargetField).First();

            PrimaryClass primaryEntry = primaryTable.Where(x => (Guid)pkPropertyOfPrimary.GetValue(x) == foreign.RefId).First();
            Assert.IsNotNull(primaryEntry);
        }
    }
}
