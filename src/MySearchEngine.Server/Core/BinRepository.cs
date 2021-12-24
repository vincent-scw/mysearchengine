using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using File = System.IO.File;

namespace MySearchEngine.Server.Core
{
    public class BinRepository
    {
        private readonly BinFile _binFile;
        public BinRepository(IOptions<BinFile> binFileOptions)
        {
            _binFile = binFileOptions.Value;
        }

        public async Task StoreTermsAsync(IDictionary<int, string> termDictionary)
        {
            await using var stream = ReadFileAsync(_binFile.Term);
            foreach (var (id, term) in termDictionary)
            {
                await stream.WriteLineAsync($"{id}|{term}");
            }
        }

        public async Task StorePagesAsync(IDictionary<int, PageInfo> pageDictionary)
        {
            await using var stream = ReadFileAsync(_binFile.Page);
            foreach (var (id, pi) in pageDictionary)
            {
                await stream.WriteLineAsync($"{id}|{pi.Title}|{pi.Url}|{pi.TokenCount}");
            }
        }

        public async Task StoreIndexAsync(IReadOnlyDictionary<int, List<(int pageId, int termCount)>> indexDictionary)
        {
            await using var stream = ReadFileAsync(_binFile.Index);
            foreach (var (termId, pages) in indexDictionary)
            {
                await stream.WriteLineAsync(
                    $"{termId}|{string.Join(',', pages.Select(x => $"{x.pageId}:{x.termCount}"))}");
            }
        }

        public async Task<List<string>> ReadStopWordsAsync()
        {
            var stopWordsStr = await File.ReadAllTextAsync("..\\..\\res\\stop_words_english.json");
            return JsonConvert.DeserializeObject<List<string>>(stopWordsStr);
        }

        public async Task<IDictionary<int, string>> ReadTermsAsync()
        {
            var lines = await ReadLinesAsync(_binFile.Term);
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
            var lines = await ReadLinesAsync(_binFile.Page);
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
            var lines = await ReadLinesAsync(_binFile.Index);
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

        private static StreamWriter ReadFileAsync(string fileName)
        {
            return File.CreateText(Path.Combine(FindResPath(Environment.CurrentDirectory), fileName));
        }

        private static Task<string[]> ReadLinesAsync(string fileName)
        {
            var path = Path.Combine(FindResPath(Environment.CurrentDirectory), fileName);
            return File.Exists(path) ? File.ReadAllLinesAsync(path) : Task.FromResult(new string[0]);
        }

        private static string FindResPath(string currentDirectory)
        {
            var newPath = Path.Combine(currentDirectory, "res");
            if (Directory.Exists(newPath))
                return newPath;

            return FindResPath(Path.Combine(currentDirectory, "..\\"));
        }
    }
}
