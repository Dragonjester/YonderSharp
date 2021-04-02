namespace YonderSharp.Config
{
    public interface IConfigManager
    {
        public string GetValue(string key);
        public string[] GetAllKeys();
        public void SetValue(string key, string value);
        public void Save();
        public void Load();
        /// <summary>
        /// Deletes all Entries
        /// </summary>
        void Clear();
    }
}
