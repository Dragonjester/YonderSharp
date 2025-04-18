﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using YonderSharp.Attributes.DataManagement;

namespace YonderSharp.FileSources
{
    /// <summary>
    /// simple Elementsource form hard disk
    /// </summary>
    public abstract class IFileSource<T> : ItemSource<T>
    {
        /// <summary>
        /// Where is the content stored on the hard disk?
        /// </summary>
        public abstract string GetPathToJsonFile();

        private IList<T> _list = new List<T>();
        private bool isInitialized;
        private ObservableCollection<string> _titles = new ObservableCollection<string>();

        private PropertyInfo _titlePropertyInfo;
        private PropertyInfo _primaryKeyPropertyInfo;

        public IFileSource()
        {
            if (_primaryKeyPropertyInfo == null)
            {
                _primaryKeyPropertyInfo = GetGenericType().GetProperties().FirstOrDefault(x => x.CanRead && x.GetCustomAttributes().Any(y => y.GetType() == typeof(PrimaryKey)));
            }

            if (_titlePropertyInfo == null)
            {
                _titlePropertyInfo = GetGenericType().GetProperties().FirstOrDefault(x => x.CanRead && x.GetCustomAttributes().Any(y => y.GetType() == typeof(Title)));
            }

            if (_titlePropertyInfo == null)
            {
                Debugger.Break();
            }
        }

        /// <summary>
        /// Raised when the list of known entries has changed
        /// </summary>
        public Action EntriesHaveChangedEvent;

        /// <inheritdoc/>
        public void Remove(T obj)
        {
            if (obj == null)
            {
                return;
            }

            Load();

            var index = _list.IndexOf(obj);
            if (index == -1)
            {
                return;
            }

            _list.Remove(obj);
            _titles.RemoveAt(index);

            RaiseChangedEvent();
        }

        /// <inheritdoc/>
        public void Add(T obj)
        {
            if (_list.Contains(obj))
            {
                return;
            }

            Load();
            _list.Add(obj);
            _titles.Add(GetTitle(obj));
            RaiseChangedEvent();
        }


        /// <inheritdoc/>
        public void Add(T[] array)
        {
            if (array == null || array.Length == null)
            {
                return;
            }

            foreach (var item in array)
            {
                Add(item);
            }
        }

        /// <inheritdoc/>
        public void Add(IList<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            foreach (var item in list)
            {
                Add(item);
            }
        }

        private void RaiseChangedEvent()
        {
            EntriesHaveChangedEvent?.Invoke();
        }

        /// <inheritdoc/>
        public void Add(object item)
        {
            if (item.GetType() == typeof(T))
            {
                Add((T)item);
            }
        }

        /// <inheritdoc/>
        public void Add(IList<T> list)
        {
            if (list == null)
            {
                return;
            }

            foreach (T obj in list)
            {
                Add(obj);
            }
        }

        /// <inheritdoc/>
        public Type GetGenericType()
        {
            return typeof(T);
        }

        /// <inheritdoc/>
        public T[] GetAll()
        {
            Load();
            return _list.ToArray();
        }

        private object locker = new object();

        /// <summary>
        /// Deserialize the contents of the file into the list
        /// </summary>
        public void Load(bool manualLoad = false)
        {
            lock (locker)
            {
                if (isInitialized && !manualLoad)
                {
                    return;
                }
                isInitialized = true;


                _list = new List<T>();

                if (File.Exists(GetPathToJsonFile()))
                {
                    try
                    {
                        _list = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(GetPathToJsonFile())).OrderBy(x => _titlePropertyInfo.GetValue(x)).ToList();
                        _titles.Clear();
                        foreach (var entry in _list)
                        {
                            _titles.Add(GetTitle(entry));
                        }
                    }
                    catch (Exception e)
                    {
                        Debugger.Break();
                        //TODO: LOGGING
                    }

                    RaiseChangedEvent();
                }
            }
        }

        /// <summary>
        /// Stores all the objects into the file
        /// </summary>
        public void Save()
        {
            if (!isInitialized)
            {
                return;
            }

            lock (locker)
            {
                FileInfo file = new FileInfo(GetPathToJsonFile());
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }

                File.WriteAllText(GetPathToJsonFile(), JsonConvert.SerializeObject(_list, Formatting.Indented));
            }
        }

        /// <inheritdoc/>
        public string GetTitle(object obj)
        {
            if (obj == null)
            {
                return "Object is null";
            }

            return Title.GetTitel(obj);
        }

        /// <inheritdoc/>
        public ObservableCollection<string> GetTitles()
        {
            Load();
            return _titles;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _list.Clear();
            _titles.Clear();
            RaiseChangedEvent();
        }

        /// <inheritdoc/>
        public T GetByPrimaryKey(object obj)
        {

            Guid castedToGuid = (Guid)obj;
            if (castedToGuid != null)
            {
                return GetByPrimaryKey(castedToGuid);
            }

            Load();

            return _list.First(x => _primaryKeyPropertyInfo.GetValue(x) == obj);
        }

        public T GetByPrimaryKey(Guid id)
        {
            Load();
            if (_primaryKeyPropertyInfo == null)
            {
                _primaryKeyPropertyInfo = GetGenericType().GetProperties().FirstOrDefault(x => x.CanRead && x.GetCustomAttributes().Any(y => y.GetType() == typeof(PrimaryKey)));
            }

            if (_primaryKeyPropertyInfo == null)
            {
                Debugger.Break();
            }

            return _list.FirstOrDefault(x => ((Guid)_primaryKeyPropertyInfo.GetValue(x)) == id);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public T ElementAt(int index)
        {
            return _list.ElementAt(index);
        }

        /// <inheritdoc/>
        public int GetIndexOf(object obj)
        {
            try
            {
                return GetIndexOf((T)obj);
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        /// <inheritdoc/>
        public int GetIndexOf(T obj)
        {
            return _list.IndexOf(obj);
        }
    }
}