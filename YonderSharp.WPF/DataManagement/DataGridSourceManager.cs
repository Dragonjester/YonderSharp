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

        /// <exception cref="KeyNotFoundException">When the type isn't known yet</exception>
        public static IDataGridSource GetSource(Type type)
        {
            return KnownSources[type];
        }

        public static IDataGridSource GetSource<T>()
        {
            return GetSource(typeof(T));
        }

        public static void RegisterToForeignKeyUpdates(IForeignKeyListChangedListener listener, Type type)
        {
            if (listener == null || type == null)
            {
                return;
            }

            if (KnownSources.TryGetValue(type, out IDataGridSource source))
            {
                source.RegisterUpdateReceiver(listener);
            }
        }

        public static void UnregisterFromForeignKeyUpdates(IForeignKeyListChangedListener listener)
        {
            if (listener == null)
            {
                return;
            }

            foreach(var source in KnownSources.Values)
            {
                source.UnregisterUpdateReceiver(listener);
            }
        }
    }
}
