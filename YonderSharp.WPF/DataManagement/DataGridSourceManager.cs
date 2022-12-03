using System;
using System.Collections.Generic;
using YonderSharp.Attributes;

namespace YonderSharp.WPF.DataManagement
{
    public class DataGridSourceManager
    {
        private static Dictionary<Type, IDataGridSource> KnownSources = new Dictionary<Type, IDataGridSource>();

        public static void RegisterDataSource(IDataGridSource dataSource)
        {
            KnownSources.Add(dataSource.GetTypeOfObjects(), dataSource);

            //Register "new entry is known in foreign table
            //so that i.e. dropdowns can know that a new entry is avaiable
            foreach(var sourceType in KnownSources.Keys)
            {
                var source = KnownSources[sourceType];
                foreach(var foreignType in ForeignKey.GetAllForeignTables(sourceType))
                {
                    //TODO: ????
                }
            }
        }

        /// <exception cref="KeyNotFoundException">When the type isn't known yet</exception>
        public static IDataGridSource GetSource(Type type)
        {
            return KnownSources[type];
        }

        public static IDataGridSource GetSource<T>()
        {
            return GetSource(typeof(T));
        }

    }
}
