using System;
using YonderSharp.Attributes.DataManagement;

namespace YonderSharp.Test.Attributes.KeyTests
{
    internal class PrimaryClass
    {
        [PrimaryKey]
        public Guid ID { get; set; } = Guid.NewGuid();
    }
}
