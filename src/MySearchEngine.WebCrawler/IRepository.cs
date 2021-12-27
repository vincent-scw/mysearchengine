using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    interface IRepository
    {
        Task StoreBooleanFilterAsync(bool[] values);
        Task<bool[]> ReadBooleanFilterAsync(int initCapacity);
    }
}
