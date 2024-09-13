using System;

namespace YonderSharp.DataTypes
{
    public interface ICreatedOn
    {
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
