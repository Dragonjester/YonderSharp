namespace YonderSharp.FileSources
{
    public class FileSource<T> : IFileSource<T>
    {
        private string _pathToFile;

        public FileSource(string pathToFile)
        {
            _pathToFile = pathToFile;
        }

        public override string GetPathToJsonFile()
        {
            return _pathToFile;
        }
    }
}
