using System;

namespace YonderSharp.WPF.DataManagement
{
    public interface IForeignKeyListChangedListener
    {
        /// <param name="type">The type of the list that changed</param>
        public void OnForeignKeyListChanged(Type type);
    }
}