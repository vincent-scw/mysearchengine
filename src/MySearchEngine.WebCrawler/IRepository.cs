using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    interface IRepository
    {
        Task StoreBloomFilterAsync(bool[] values);
        Task<bool[]> ReadBloomFilterAsync(int initCapacity);
    }
}
