using System;
using YonderSharp.Attributes.DataManagement;

namespace YonderSharp.Test.Attributes.KeyTests
{
    internal class ForeignClass
    {
        [PrimaryKey]
        public Guid ID { get; set; } = Guid.NewGuid();

        [ForeignKey(typeof(PrimaryClass), "ID")]
        public Guid RefId { get; set; } = Guid.NewGuid();

    }
}
