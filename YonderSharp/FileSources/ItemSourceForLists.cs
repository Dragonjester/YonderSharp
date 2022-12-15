using System;
using System.Collections.ObjectModel;

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
        /// Also: WPF doesn't support generics :(
        /// </summary>
        public Type GetGenericType();

        /// <summary>
        /// Return the shown titles for the known entries
        /// </summary>
        public ObservableCollection<string> GetTitles();

        /// <summary>
        /// Generates the title for the given object, if it is of the type declared via GetGenericType()
        /// </summary>
        public string GetTitle(object obj);

        ///<summary>
        /// Empties the list
        /// </summary>
        public void Clear();
    }
}