using System.Threading.Tasks;

namespace YonderSharp.Maui
{
    public interface IFilePicker
    {
        public Task<byte[]> SelectFile(string pickerTitle = "");
    }
}
