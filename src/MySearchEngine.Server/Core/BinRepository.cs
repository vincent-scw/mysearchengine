using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MySearchEngine.Server.Core
{
    public class BinRepository
    {
        private readonly BinPath _binPath;
        public BinRepository(IOptions<BinPath> binPath)
        {
            _binPath = binPath.Value;
        }

        public async Task StoreTermsAsync(IDictionary<int, string> termDictionary)
        {
            await using var stream = File.CreateText(_binPath.Term);
            foreach (var (id, term) in termDictionary)
            {
                await stream.WriteLineAsync($"{id}|{term}");
            }
        }

        public async Task StorePagesAsync(IDictionary<int, PageInfo> pageDictionary)
        {
            await using var stream = File.CreateText(_binPath.Page);
            foreach (var (id, pi) in pageDictionary)
            {
                await stream.WriteLineAsync($"{id}|{pi.Title}|{pi.Url}");
            }
        }

        public async Task StoreIndexAsync(IReadOnlyDictionary<int, List<(int pageId, int termCount)>> indexDictionary)
        {
            await using var stream = File.CreateText(_binPath.Index);
            foreach (var (termId, pages) in indexDictionary)
            {
                await stream.WriteLineAsync(
                    $"{termId}|{string.Join(',', pages.Select(x => $"{x.pageId}:${x.termCount}"))}");
            }
        }

        public async Task<IDictionary<int, string>> ReadTermsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IDictionary<int, PageInfo>> ReadPagesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IDictionary<int, List<int>>> ReadIndexAsync()
        {
            throw new NotImplementedException();
        }
    }
}
