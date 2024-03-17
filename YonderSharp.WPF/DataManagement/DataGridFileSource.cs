using System;
using System.Collections.Generic;
using YonderSharp.FileSources;

namespace YonderSharp.WPF.DataManagement
{
    public class DataGridFileSource<T> : IDataGridSource
    {
        dynamic _fileSource;
        Type _fileSourceType;

        public DataGridFileSource() : base()
        {

        }

        public DataGridFileSource(IFileSource<T> fileSource) : base()
        {
            SetFileSource(fileSource);
        }

        /// <param name="fileSource">instanceof <see cref="IFileSource{T}"/></param>
        public void SetFileSource(IFileSource<T> fileSource)
        {
            _fileSource = fileSource ?? throw new ArgumentNullException(nameof(fileSource));
            _fileSourceType = fileSource.GetType();
            foreach (var item in _fileSource.GetAll())
            {
                _items.Add(item);
            }
        }

        protected override void Save(IList<object> items)
        {
            _fileSource.Clear();
            foreach (var item in items)
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
