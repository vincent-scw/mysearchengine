using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File = System.IO.File;

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
                await stream.WriteLineAsync($"{id}|{pi.Title}|{pi.Url}|{pi.TokenCount}");
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
            var lines = await File.ReadAllLinesAsync(_binPath.Term);
            var ret = new Dictionary<int, string>();
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length != 2) continue;
                ret.TryAdd(Convert.ToInt32(parts[0]), parts[1]);
            }

            return ret;
        }

        public async Task<IDictionary<int, PageInfo>> ReadPagesAsync()
        {
            var lines = await File.ReadAllLinesAsync(_binPath.Page);
            var ret = new Dictionary<int, PageInfo>();
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length != 4) continue;
                ret.TryAdd(Convert.ToInt32(parts[0]), new PageInfo
                {
                    Id = Convert.ToInt32(parts[0]),
                    Title = parts[1],
                    Url = parts[2],
                    TokenCount = Convert.ToInt32(parts[3])
                });
            }

            return ret;
        }

        public async Task<IDictionary<int, List<(int pageId, int termCount)>>> ReadIndexAsync()
        {
            var lines = await File.ReadAllLinesAsync(_binPath.Index);
            var ret = new Dictionary<int, List<(int, int)>>();
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length != 2) continue;
                var indexes = parts[1].Split(',');
                var list = indexes.Select(i =>
                {
                    var pt = i.Split(':');
                    return (Convert.ToInt32(pt[0]), Convert.ToInt32(pt[1]));
                }).ToList();
                ret.TryAdd(Convert.ToInt32(parts[0]), list);
            }

            return ret;
        }
    }
}
