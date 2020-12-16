using System;

namespace YonderSharp.WPF.DataManagement
{
    public interface IDataGridSource
    {
        /// <summary>
        /// Can new items be added by the user?
        /// </summary>
        public bool IsAllowedToAddNew();

        /// <summary>
        /// Can shown items be removed by the user?
        /// </summary>
        public bool IsAllowedToRemove();

        /// <summary>
        /// Items shown in the ListView
        /// </summary>
        /// <returns></returns>
        public object[] GetShownItems();

        /// <summary>
        /// Items that can be added to the shown list
        /// </summary>
        /// <returns></returns>
        public object[] GetAddableItems();

        public Type GetTypeOfObjects();

        /// <summary>
        /// Add an item to the 
        /// </summary>
        public void AddShownItem(object item);

        /// <summary>
        /// Remove an item from the shown list
        /// </summary>
        public void RemoveShownItem(object item);

        /// <summary>
        /// string shown in the listview
        /// </summary>
        public string GetShownItemTitle(object item);
    }
}
