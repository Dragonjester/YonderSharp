﻿using System;
using System.Linq;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Collections.Generic;

namespace YonderSharp.WPF.DataManagement {
    public class ForeignKeyConverter : IValueConverter {
        PropertyInfo _pkPropertyInfo;

        IDataGridSource _dataSource;
        public ForeignKeyConverter(IDataGridSource dataSource) {
            _dataSource = dataSource ?? throw new ArgumentNullException();
            _pkPropertyInfo = dataSource.GetIDPropertyInfo();
        }

        /// <summary>
        /// Return entry by id
        /// </summary>
        public object Convert(object id, Type targetType, object parameter, CultureInfo culture) {
            //TODO: IEnumerable???
            if(id is IList) {
                IList<Guid> list = (IList<Guid>) id;
                return list.Select(x => _dataSource.GetById(x));
            }

            return _dataSource.GetById(id);

        }

        public object GetId(object element) {
            return _pkPropertyInfo.GetValue(element);
        }

        /// <summary>
        /// Return PK value of entry
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return _pkPropertyInfo.GetValue(value);
        }
    }
}
