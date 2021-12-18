using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    interface IProcessingService
    {
        void DoWork(string uri, CancellationToken cancellationToken);
    }
}
