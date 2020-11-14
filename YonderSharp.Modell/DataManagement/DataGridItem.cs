using System;

namespace YonderSharp.Modell.DataManagement
{
    public abstract class DataGridItem
    {
        public abstract DateTime GetCreationDate();
        public abstract int GetID();
        public abstract string GetTitle();
    }
}
