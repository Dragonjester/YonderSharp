using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using YonderSharp.Attributes;

namespace YonderSharp.WPF.DataManagement
{
    /// <summary>
    /// Sadly wpf doesn't support generics in their UI classes, so we can't use a generic here either #sadface
    /// </summary>
    public abstract class IDataGridSource
    {
        public IDataGridSource()
        {
            _items.CollectionChanged += OnItemListChangedEvent;
        }
        protected ObservableCollection<object> _items { get; set; } = new ObservableCollection<object>();

        #region foreignKey update listener

        private HashSet<IForeignKeyListChangedListener> _updateReceivers = new HashSet<IForeignKeyListChangedListener>();

        public void UnregisterUpdateReceiver(IForeignKeyListChangedListener listener)
        {
            if (listener == null)
            {
                return;
            }

            _updateReceivers.Remove(listener);
        }

        public void RegisterUpdateReceiver(IForeignKeyListChangedListener eventListener)
        {
            if (eventListener == null)
            {
                return;
            }

            _updateReceivers.Add(eventListener);
        }

        private void OnItemListChangedEvent(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var receiver in _updateReceivers)
            {
                try
                {
                    receiver.OnForeignKeyListChanged(GetTypeOfObjects());
                }
                catch (Exception exc)
                {
                    //TODO: Logging
                    Debugger.Break();
                }
            }
        }

        #endregion foreignKey update listener

        public object[] GetAllItems()
        {
            return _items.ToArray();
        }

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

        public object GetById(object v)
        {
            PropertyInfo pk = GetIDPropertyInfo();
            return GetAllItems().FirstOrDefault(x => pk.GetValue(x).Equals(v));
        }

        /// <summary>
        /// Items that can be added to the shown list
        /// </summary>
        /// <returns></returns>
        public virtual object[] GetAddableItems(IList<object> notAddableItems)
        {
            if (GetConfiguration().GetAddableItemsReturnAll || notAddableItems == null || notAddableItems.Count == 0)
            {
                return _items.ToArray();
            }
            else
            {
                return _items.Where(x => !notAddableItems.Contains(x)).ToArray();
            }
        }

        public abstract Type GetTypeOfObjects();


        public PropertyInfo GetIDPropertyInfo()
        {
            BindingFlags instancePublic = BindingFlags.Instance | BindingFlags.Public;
            return GetTypeOfObjects().GetProperties(instancePublic)
                         .Where(x => Attribute.IsDefined(x, typeof(DataMemberAttribute))
                                 && !Attribute.IsDefined(x, typeof(NonSerializedAttribute))).First(x => x.Name == GetNameOfIdProperty());
        }


        private static Dictionary<Type, string> NamesOfIdProperties = new Dictionary<Type, string>();
        /// <summary>
        /// returns the name of the [PrimaryKey] property
        /// </summary>
        public string GetNameOfIdProperty()
        {
            if (NamesOfIdProperties.TryGetValue(GetTypeOfObjects(), out string name))
            {
                return name;
            }

            foreach (var property in GetTypeOfObjects().GetProperties())
            {
                foreach (var attribute in property.GetCustomAttributes(false))
                {
                    if (attribute is PrimaryKey key)
                    {
                        NamesOfIdProperties.Add(GetTypeOfObjects(), property.Name);
                        return property.Name;
                    }
                }
            }

            return null;
        }


        public PropertyInfo GetTitlePropertyInfo()
        {
            BindingFlags instancePublic = BindingFlags.Instance | BindingFlags.Public;
            var x = GetTypeOfObjects().GetProperties(instancePublic).First(x => x.GetCustomAttribute<Title>() != null);
            return x;
        }

        /// <summary>
        /// string shown in the listview
        /// </summary>
        public virtual string GetShownItemTitle(object item)
        {
            var x = GetTitlePropertyInfo().GetValue(item);
            if (x == null)
            {
                return "EMPTY";
            }
            return x.ToString();
        }

        /// <summary>
        /// Content shown to the user for the given field
        /// </summary>
        public virtual string GetLabel(string fieldName)
        {
            //Currently just the fieldname
            //TODO: Add some translation stuff. LOW PRIORITY
            return fieldName;
        }

        /// <summary>
        /// Is the field part of the ItemTitle generation? If yes this will result in an update of the list on change of the fieldvalue
        /// </summary>
        private static Dictionary<string, bool> _fieldPartOfListTexts = new Dictionary<string, bool>();

        /// <inheritdoc/>
        public virtual bool IsFieldPartOfListText(string fieldName)
        {
            string cashKey = $"{GetTypeOfObjects().FullName} {fieldName}";

            if (_fieldPartOfListTexts.ContainsKey(cashKey))
            {
                return _fieldPartOfListTexts[cashKey];
            }

            var property = GetTypeOfObjects().GetProperty(fieldName);
            if (property == null)
            {
                _fieldPartOfListTexts.Add(cashKey, false);
                return false;
            }

            foreach (var attribute in property.GetCustomAttributes(false))
            {
                if (attribute is Title key)
                {
                    _fieldPartOfListTexts.Add(cashKey, true);
                    return true;
                }
            }

            _fieldPartOfListTexts.Add(cashKey, false);

            return false;
        }

        #region actions
        /// <summary>
        /// Add an item to the 
        /// </summary>
        public void AddItem(object item)
        {
            _items.Add(item);
        }

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
            return GetConfiguration().IsAllowedToCreateNewEntry;
        }

        /// <summary>
        /// Can an item be added from GetAddableItems()?
        /// </summary>
        public bool IsAllowedToAddFromList()
        {
            return GetConfiguration().IsAllowedToIsAllowedToAddFromList;
        }

        protected DataGridSourceConfiguration _config;
        public virtual DataGridSourceConfiguration GetConfiguration()
        {
            if (_config == null)
            {
                _config = new DataGridSourceConfiguration();
                _config.IsAllowedToIsAllowedToAddFromList = true;
                _config.IsAllowedToCreateNewEntry = true;
                _config.IsAllowedToRemove = true;
                _config.HasSearch = true;
                _config.GetAddableItemsReturnAll = true;
                _config.IsPrimaryKeyDisabled = true;
                _config.ShowSaveDialog = true;
                _config.IsReadOnlyMode = false;
                _config.ShowSaveButton = true;
            }

            return _config;
        }

        /// <summary>
        /// Creates a new Entry
        /// </summary>
        public void AddNewItem()
        {
            AddItem(CreateNewItem());
        }

        /// <summary>
        /// Internal implementation on how to create a new Item
        /// </summary>
        protected virtual object CreateNewItem()
        {
            return Activator.CreateInstance(GetTypeOfObjects());
        }

        /// <summary>
        /// Remove an item from the shown list
        /// </summary>
        public void RemoveShownItem(object item)
        {
            _items.Remove(item);
        }

        /// <summary>
        /// Can shown items be removed by the user?
        /// </summary>
        public bool IsAllowedToRemove()
        {
            return GetConfiguration().IsAllowedToRemove;
        }


        /// <summary>
        /// The item currently selected in the DataGrid-UI
        /// </summary>
        public object SelectedItem { get; set; }

        /// <summary>
        /// Save the current list
        /// </summary>
        public void Save()
        {
            Save(_items);
        }


        /// <summary>
        /// Implementation for how to save the list
        /// </summary>
        protected abstract void Save(IList<object> items);
        #endregion actions

        #region search

        string _searchText { get; set; }
        /// <summary>
        /// Show the search textbox?
        /// </summary>
        public bool HasSearch()
        {
            return GetConfiguration().HasSearch;
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

        /// <summary>
        /// Should the textbox for the primary field key be disabled?
        /// </summary>
        public bool IsPrimaryKeyDisabled()
        {
            return GetConfiguration().IsPrimaryKeyDisabled;
        }

        /// <summary>
        /// be carefull using this. It is the source list of this DataGrid and not just a copy!
        /// </summary>
        public ObservableCollection<object> GetObservable()
        {
            return _items;
        }

        #endregion search
    }
}
