using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YonderSharp.Attributes;
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

        private string _nameOfIdProperty;
        /// <inheritdoc/>
        public string GetNameOfIdProperty()
        {
            if (string.IsNullOrWhiteSpace(_nameOfIdProperty))
            {
                foreach (var property in GetTypeOfObjects().GetProperties())
                {
                    foreach (var attribute in property.GetCustomAttributes(false))
                    {
                        if (attribute is PrimaryKey key)
                        {
                            _nameOfIdProperty = property.Name;
                            return _nameOfIdProperty;
                        }
                    }
                }
            }

            return _nameOfIdProperty;
        }

        /// <inheritdoc/>
        public Type GetTypeOfObjects()
        {
            return _fileSource.GetGenericType();
        }

        Dictionary<string, bool> _fieldPartOfListTexts = new Dictionary<string, bool>();
        /// <inheritdoc/>
        public bool IsFieldPartOfListText(string fieldName)
        {
            if (_fieldPartOfListTexts.ContainsKey(fieldName))
            {
                return _fieldPartOfListTexts[fieldName];
            }

            var property = GetTypeOfObjects().GetProperty(fieldName);
            if(property == null)
            {
                _fieldPartOfListTexts.Add(fieldName, false);
                return false;
            }

            foreach (var attribute in property.GetCustomAttributes(false))
            {
                if (attribute is Title key)
                {
                    _fieldPartOfListTexts.Add(fieldName, true);
                    return true;
                }
            }

            _fieldPartOfListTexts.Add(fieldName, false);
            return false;

        }



        /// <inheritdoc/>
        public void Save()
        {
            _fileSource.Save();
        }
    }
}
