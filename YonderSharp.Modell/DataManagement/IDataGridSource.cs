using System;
using System.Collections.Generic;
using System.Linq;

namespace YonderSharp.Modell.DataManagement
{
    public abstract class IDataGridSource<T> where T : DataGridItem
    {
        private List<T> entries = new List<T>();
        DataGridOrder order = DataGridOrder.ByCreationDateAsc;

        public void SetOrder(DataGridOrder orderby)
        {
            order = orderby;
            switch (orderby)
            {
                case DataGridOrder.ByCreationDateAsc:
                    entries = entries.OrderBy(x => x.GetCreationDate()).ToList();
                    break;
                case DataGridOrder.ByCreationDateDesc:
                    entries = entries.OrderByDescending(x => x.GetCreationDate()).ToList();
                    break;
                case DataGridOrder.ByIdAsc:
                    entries = entries.OrderBy(x => x.GetID()).ToList();
                    break;
                case DataGridOrder.ByIdDesc:
                    entries = entries.OrderByDescending(x => x.GetID()).ToList();
                    break;
                case DataGridOrder.ByTitleAsc:
                    entries = entries.OrderBy(x => x.GetTitle()).ToList();
                    break;
                case DataGridOrder.ByTitleDesc:
                    entries = entries.OrderByDescending(x => x.GetTitle()).ToList();
                    break;
            }
        }

        public T[] GetList()
        {
            return entries.ToArray();
        }

        public T Get(int id)
        {
            return entries.First(x => x.GetID() == id);
        }

        public void AddToList(int id)
        {
            var entry = GetPossibleValues().First(x => x.GetID() == id);
          
            EntryAdded(entry);
            entries.Add(entry);

            SetOrder(order);
        }

        public void Remove(int id)
        {
            Remove(entries.First(x => x.GetID() == id));
        }

        public void Remove(T obj)
        {
            RemoveEntry(obj);
            entries.Remove(obj);
        }


        #region toImplementInInheritance
        public abstract T CreateNew();
        protected abstract void EntryAdded(T entry);
        protected abstract void RemoveEntry(T entry);
        public abstract T[] GetPossibleValues();
        #endregion toImplementInInheritance
    }
}
