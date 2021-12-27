using MySearchEngine.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySearchEngine.Server.Core
{
    public interface IDocIndexer
    {
        void Index(DocInfo doc, string content);
        int GetTotalDocCount();
        bool TryGetIndexedDocs(string term, out List<TermInDoc> pages);
        bool TryGetDocInfo(int pageId, out DocInfo pi);

        Task StoreDataAsync();
    }
}
