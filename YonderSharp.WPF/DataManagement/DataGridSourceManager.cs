using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YonderSharp.FileSources;

namespace YonderSharp.WPF.DataManagement
{
    public class DataGridSourceManager
    {
        private static Dictionary<Type, IDataGridSource> KnownSources = new Dictionary<Type, IDataGridSource>();

        public static void RegisterDataSource(IDataGridSource dataSource)
        {
            KnownSources.Add(dataSource.GetTypeOfObjects(), dataSource);
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
