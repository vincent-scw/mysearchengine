using MySearchEngine.Core;
using MySearchEngine.Server.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySearchEngine.Server
{
    public interface IRepository
    {
        Task<List<string>> ReadStopWordsAsync();
        Task<IDictionary<string, int>> ReadTermsAsync();
        Task<IDictionary<int, DocInfo>> ReadDocsAsync();
        Task<IDictionary<int, List<TermInDoc>>> ReadIndexAsync();
        Task StoreTermsAsync(IDictionary<string, int> termDictionary);
        Task StoreDocsAsync(IDictionary<int, DocInfo> pageDictionary);
        Task StoreIndexAsync(IDictionary<int, List<TermInDoc>> indexDictionary);
    }
}
