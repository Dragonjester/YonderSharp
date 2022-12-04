using System;
using System.Collections.Generic;
using System.Reflection;
using YonderSharp.FileSources;

namespace YonderSharp.WPF.DataManagement
{
    public abstract class DataGridFileSource : IDataGridSource
    {
        dynamic _fileSource;
        Type _fileSourceType;
        /// <param name="fileSource">instanceof <see cref="IFileSource{T}"/></param>
        public void SetFileSource(dynamic fileSource)
        {
            _fileSource = fileSource ?? throw new ArgumentNullException(nameof(fileSource));
            _fileSourceType = fileSource.GetType();

            if (!_fileSourceType.BaseType.Name.Contains("IFileSource"))
            {
                throw new ArgumentException($"{_fileSourceType.FullName} doesn't implement IFileSource");
            }

            foreach(var item in _fileSource.GetAll())
            {
                _items.Add(item);
            }
        }

        protected override void Save(IList<object> items)
        {
            _fileSource.Clear();
            foreach(var item in items)
            {
                _fileSource.Add(item);
            }
            _fileSource.Save();
        }
        /// <inheritdoc/>
        public override Type GetTypeOfObjects()
        {
            return _fileSource.GetGenericType();
        }
    }
}
