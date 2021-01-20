using System;

namespace YonderSharp.WPF.DataManagement
{
    public interface IDataGridSource
    {
        /// <summary>
        /// Items shown in the ListView, respecting the search (if avaiable)
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
        /// string shown in the listview
        /// </summary>
        public string GetShownItemTitle(object item);

        /// <summary>
        /// Content shown to the user for the given field
        /// </summary>
        public string GetLabel(string fieldName);

        /// <summary>
        /// Is the field part of the ItemTitle generation? If yes this will result in an update of the list on change of the fieldvalue
        /// </summary>
        bool IsFieldPartOfListText(string fieldName);


        #region actions
        /// <summary>
        /// Add an item to the 
        /// </summary>
        public void AddShownItem(object item);

        /// <summary>
        /// Can new items be added by the user?
        /// </summary>
        public bool IsAllowedToAddNew();

        /// <summary>
        /// Remove an item from the shown list
        /// </summary>
        public void RemoveShownItem(object item);

        /// <summary>
        /// Can shown items be removed by the user?
        /// </summary>
        public bool IsAllowedToRemove();

        /// <summary>
        /// Save the current list
        /// </summary>
        public void Save();
        #endregion actions

        #region search
        /// <summary>
        /// Show the search textbox?
        /// </summary>
        public bool HasSearch();

        /// <summary>
        /// Sets the search filter. 
        /// After setting the search filter, the GetShownItems() has to handle the filtering
        /// </summary>
        /// <param name="searchValue"></param>
        public void SetSearchText(string searchValue);
        public string GetSearchText();
        #endregion search
    }
}
