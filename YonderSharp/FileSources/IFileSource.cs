using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace YonderSharp.FileSources
{
    /// <summary>
    /// simple Elementsource form hard disk
    /// </summary>
    public abstract class IFileSource<T>
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

        /// <summary>
        /// Removes the element from the store
        /// </summary>
        public virtual void Remove(T obj)
        {
            Load();
            _list.Remove(obj);
            RaiseChangedEvent();
        }

        /// <summary>
        /// Add single element
        /// </summary>
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

        /// <summary>
        /// Add multiple elements to the list
        /// </summary>
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


        /// <summary>
        /// Get all elements known
        /// </summary>
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
                    _list = JsonSerializer.Deserialize<HashSet<T>>(File.ReadAllText(GetPathToJsonFile()));
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
                File.WriteAllText(GetPathToJsonFile(), JsonSerializer.Serialize(_list));
            }
        }
    }
}
