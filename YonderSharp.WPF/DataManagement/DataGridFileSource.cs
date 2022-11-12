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
        }

        public string _searchText { get; set; }

        private MethodInfo _addMethod;
        /// <inheritdoc/>
        public void AddItem(object item)
        {
            if (_addMethod == null)
            {
                var methods = _fileSourceType.GetMethods();
                foreach (var method in methods)
                {
                    if (method.Name.Contains("Add"))
                    {
                        var parameters = method.GetParameters();
                        if(parameters[0].Name == "obj")
                        {
                            _addMethod = method;
                            break;
                        }
                    }
                }
            }

            _addMethod.Invoke(_fileSource, new[] { item });
        }

        private MethodInfo _removeMethod;
        /// <inheritdoc/>
        public void RemoveShownItem(object item)
        {
            if (_removeMethod == null)
            {
                var methods = _fileSourceType.GetMethods();
                foreach (var method in methods)
                {
                    if (method.Name.Contains("Remove"))
                    {
                        var parameters = method.GetParameters();
                        if (parameters[0].Name == "obj")
                        {
                            _removeMethod = method;
                            break;
                        }
                    }
                }
            }

            _removeMethod.Invoke(_fileSource, new[] { item });
        }

        /// <inheritdoc/>
        public abstract void AddNewItem();

        /// <inheritdoc/>
        public abstract object[] GetAddableItems(IList<object> notAddableItems);

        /// <inheritdoc/>
        public object[] GetAllItems()
        {
            return _fileSource.GetAll();
        }

     
        /// <inheritdoc/>
        public Type GetTypeOfObjects()
        {
            return _fileSource.GetGenericType();
        }

        /// <inheritdoc/>
        public void Save()
        {
            _fileSource.Save();
        }
    }
}
