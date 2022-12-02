using System;
using System.Collections.Generic;

namespace YonderSharp.FileSources
{
    public class ItemSourceContainer
    {
        private Dictionary<Type, ItemSourceForLists> _cache = new Dictionary<Type, ItemSourceForLists>();

        /// <summary>
        /// READ ONLY
        /// </summary>
        public Dictionary<Type, ItemSourceForLists> Cache { get { return new Dictionary<Type, ItemSourceForLists>(_cache); } }

        public ItemSourceForLists Get(Type type)
        {
            if (_cache.TryGetValue(type, out var itemSource))
            {
                return itemSource;
            }

            return null;
        }

        public void Add(ItemSourceForLists itemSource)
        {
            if (itemSource == null)
            {
                return;
            }

            _cache.Add(itemSource.GetGenericType(), itemSource);
        }

        public void Remove(Type type)
        {
            if(type == null)
            {
                return;
            }

            _cache.Remove(type);
        }

        public void Remove(ItemSourceForLists source)
        {
            if (source == null)
            {
                return;
            }

            _cache.Remove(source.GetGenericType());
        }
    }
}
