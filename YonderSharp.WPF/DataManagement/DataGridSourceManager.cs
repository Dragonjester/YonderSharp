using System;
using System.Collections.Generic;

namespace YonderSharp.WPF.DataManagement
{
    public class DataGridSourceManager
    {
        private static Dictionary<Type, IDataGridSource> KnownSources = new Dictionary<Type, IDataGridSource>();

        public static void RegisterDataSource(IDataGridSource dataSource)
        {
            KnownSources.Add(dataSource.GetTypeOfObjects(), dataSource);
        }

        public static IDataGridSource GetSource(Type type)
        {
            return KnownSources[type] ?? throw new InvalidOperationException($"{type.Name} is not registered yet");
        }

        public static IDataGridSource GetSource<T>()
        {
            return GetSource(typeof(T));
        }
    }
}
