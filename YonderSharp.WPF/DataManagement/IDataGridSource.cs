﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using YonderSharp.Attributes;

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

        public object GetById(object v)
        {
            PropertyInfo pk = GetIDPropertyInfo();
            return GetAllItems().FirstOrDefault(x => pk.GetValue(x).Equals(v));
        }

        /// <summary>
        /// Items that can be added to the shown list
        /// </summary>
        /// <returns></returns>
        public object[] GetAddableItems(IList<object> notAddableItems);

        public Type GetTypeOfObjects();


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
        public string GetShownItemTitle(object item)
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
        public bool IsFieldPartOfListText(string fieldName)
        {
            string cashKey = $"{GetTypeOfObjects().FullName} {fieldName}";

            if (_fieldPartOfListTexts.ContainsKey(cashKey))
            {
                return _fieldPartOfListTexts[cashKey];
            }

            var property = GetTypeOfObjects().GetProperty(cashKey);
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

        /// <summary>
        /// Should the textbox for the primary field key be disabled?
        /// </summary>
        public virtual bool IsPrimaryKeyDisabled()
        {
            return true;
        }
        #endregion search
    }
}
