using System;
using YonderSharp.Attributes;

namespace YonderSharp.Test.Attributes.KeyTests
{
    internal class PrimaryClass
    {
        [PrimaryKey]
        public Guid ID { get; set; } = Guid.NewGuid();
    }
}
