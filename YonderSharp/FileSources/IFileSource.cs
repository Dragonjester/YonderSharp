using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace YonderSharp.FileSources
{
    public abstract class IFileSource<T>
    {
        public abstract string GetPathToJsonFile();

        private HashSet<T> _list = new HashSet<T>();
        private bool isInitialized;

        public virtual void Remove(T obj)
        {
            Load();
            _list.Remove(obj);
        }

        /// <summary>
        /// Add single element
        /// </summary>
        public virtual void Add(T obj)
        {
            Load();
            _list.Add(obj);
        }

        /// <summary>
        /// Add multiple elements to the list
        /// </summary>
        public virtual void Add(IList<T> list)
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
        public virtual T[] GetAll()
        {
            Load();
            return _list.ToArray();
        }

        private object locker = new object();

        /// <summary>
        /// Deserialize the contents of the file into the list
        /// </summary>
        private void Load()
        {
            lock (locker)
            {
                if (isInitialized)
                {
                    return;
                }
                isInitialized = true;
            }


            if (File.Exists(GetPathToJsonFile()))
            {
                try
                {
                    _list = JsonSerializer.Deserialize<HashSet<T>>(File.ReadAllText(GetPathToJsonFile()));
                }
                catch (Exception e)
                {
                    //TODO: LOGGING
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
                File.WriteAllText(GetPathToJsonFile(), JsonSerializer.Serialize(_list));
            }
        }
    }
}
