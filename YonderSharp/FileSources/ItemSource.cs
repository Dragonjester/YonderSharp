using System.Collections.Generic;

namespace YonderSharp.FileSources
{
    /// <summary>
    /// A source for items. 
    /// </summary>
    public interface ItemSource<T> : ItemSourceForLists
    {

        /// <summary>
        /// Remove the given element from the Source
        /// </summary>
        public void Remove(T obj);

        /// <summary>
        /// Add single element
        /// </summary>
        public void Add(T obj);

        /// <summary>
        /// Add multiple items to the list
        /// </summary>
        public void Add(IList<T> list);

        /// <summary>
        /// WPF doesn't support generics, so some sources might want to add items of type object
        /// </summary>
        public void Add(IList<object> list);

        /// <summary>
        /// WPF doesn't support generics, so some sources might want to add items of type object
        /// </summary>
        public void Add(object item);

        /// <summary>
        /// Add multiple items to the list
        /// </summary>
        public void Add(T[] array);

        /// <summary>
        /// Get All known Elements
        /// </summary>
        public T[] GetAll();

    }
}
