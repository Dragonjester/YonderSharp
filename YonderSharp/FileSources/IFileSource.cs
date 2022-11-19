using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YonderSharp.Attributes;

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

        private HashSet<T> _list = new HashSet<T>();
        private bool isInitialized;


        /// <summary>
        /// Raised when the list of known entries has changed
        /// </summary>
        public Action EntriesHaveChangedEvent;

        /// <inheritdoc/>
        public void Remove(T obj)
        {
            Load();
            _list.Remove(obj);
            RaiseChangedEvent();
        }

        /// <inheritdoc/>
        public void Add(T obj)
        {
            Load();
            _list.Add(obj);
            RaiseChangedEvent();
        }

        private void RaiseChangedEvent()
        {
            if (EntriesHaveChangedEvent != null)
            {
                EntriesHaveChangedEvent.Invoke();
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
            }

            _list = new HashSet<T>();

            if (File.Exists(GetPathToJsonFile()))
            {
                try
                {
                    _list = JsonConvert.DeserializeObject<HashSet<T>>(File.ReadAllText(GetPathToJsonFile()));
                }
                catch
                {
                    //TODO: LOGGING
                }

                RaiseChangedEvent();
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

        public string[] GetTitles()
        {
            var allItems = GetAll();
            if (allItems == null)
            {
                return null;
            }

            var propertyInfoOfTitle = GetGenericType().GetProperties().FirstOrDefault(x => x.CanRead && x.GetCustomAttributes().Any(y => y.GetType() == typeof(Title)));

            var result = new List<string>();
            foreach (var item in allItems)
            {
                var title = (propertyInfoOfTitle.GetValue(item) ?? string.Empty).ToString();
                result.Add(title);

            }

            return result.ToArray();

        }
    }
}
