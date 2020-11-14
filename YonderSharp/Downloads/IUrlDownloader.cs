namespace YonderSharp.Downloads
{
    public interface IUrlDownloader
    {
        /// <summary>
        /// Download the string content of the url
        /// </summary>
        string LoadFromUrl(string Url);

        /// <summary>
        /// Download the string content of the url, without any cache!
        /// </summary>
        string LoadFromUrl(string Url, bool forceDownload);
    }
}
