using System;

namespace YonderSharp.FileSources
{
    /// <summary>
    /// allows to put ItemSources into a list.
    /// You can't have a List<ItemSource<T>>....
    /// </summary>
    public interface ItemSourceForLists
    {
        /// <summary>
        /// Helper Method for reflection purposes
        /// </summary>
        public Type GetGenericType();

        /// <summary>
        /// Return the shown titles for the known entries
        /// </summary>
        public string[] GetTitles();

        ///<summary>
        /// Empties the list
        /// </summary>
        public void Clear();
    }
}