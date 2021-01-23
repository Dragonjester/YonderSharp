using System;
using System.Linq;

namespace YonderSharp.WPF.DataManagement
{
    /// <summary>
    /// Sadly wpf doesn't support generics in their UI classes, so we can't use a generic here either #sadface
    /// </summary>
    public interface IDataGridSource
    {
        public object[] GetAllItems();

        /// <summary>
        /// Items shown in the ListView, respecting the search (if avaiable)
        /// </summary>
        /// <returns></returns>
        public object[] GetShownItems()
        {
            if (HasSearch() && !string.IsNullOrEmpty(GetSearchText()))
            {
                return GetAllItems().Where(x => string.IsNullOrEmpty(GetShownItemTitle(x)) || GetShownItemTitle(x).Contains(GetSearchText(), StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            else
            {
                return GetAllItems().ToArray();
            }
        }

        /// <summary>
        /// Items that can be added to the shown list
        /// </summary>
        /// <returns></returns>
        public abstract object[] GetAddableItems();

        public Type GetTypeOfObjects();

        /// <summary>
        /// string shown in the listview
        /// </summary>
        public string GetShownItemTitle(object item);

        /// <summary>
        /// Content shown to the user for the given field
        /// </summary>
        public virtual string GetLabel(string fieldName)
        {
            return fieldName;
        }

        /// <summary>
        /// Is the field part of the ItemTitle generation? If yes this will result in an update of the list on change of the fieldvalue
        /// </summary>
        public abstract bool IsFieldPartOfListText(string fieldName);


        #region actions
        /// <summary>
        /// Add an item to the 
        /// </summary>
        public void AddItem(object item);

        /// <summary>
        /// Can new items be added by the user at all?
        /// </summary>
        public virtual bool IsAllowedToAddNew()
        {
            return IsAllowedToCreateNewEntry() || IsAllowedToAddFromList();
        }

        /// <summary>
        /// Can a new item be created?
        /// </summary>
        public virtual bool IsAllowedToCreateNewEntry()
        {
            return true;
        }

        /// <summary>
        /// Can an item be added from GetAddableItems()?
        /// </summary>
        public virtual bool IsAllowedToAddFromList()
        {
            return true;
        }

        /// <summary>
        /// Creates a new Entry
        /// </summary>
        void AddNewItem();

        /// <summary>
        /// Remove an item from the shown list
        /// </summary>
        public void RemoveShownItem(object item);

        /// <summary>
        /// Can shown items be removed by the user?
        /// </summary>
        public virtual bool IsAllowedToRemove()
        {
            return true;
        }

        /// <summary>
        /// Save the current list
        /// </summary>
        public abstract void Save();
        #endregion actions

        #region search

        string _searchText { get; set; }
        /// <summary>
        /// Show the search textbox?
        /// </summary>
        public virtual bool HasSearch()
        {
            return true;
        }

        /// <summary>
        /// Sets the search filter. 
        /// After setting the search filter, the GetShownItems() has to handle the filtering
        /// </summary>
        /// <param name="searchValue"></param>
        public void SetSearchText(string searchValue)
        {
            _searchText = searchValue;
        }
        public virtual string GetSearchText()
        {
            return _searchText;
        }
        #endregion search
    }
}
