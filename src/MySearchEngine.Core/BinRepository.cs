﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File = System.IO.File;

namespace MySearchEngine.Core
{
    public class BinRepository : IRepository
    {
        private readonly BinFile _binFile;
        public BinRepository()
        {
            _binFile = new BinFile();
        }

        public async Task StoreTermsAsync(IDictionary<string, int> termDictionary)
        {
            await using var stream = ReadFileAsync(_binFile.Term);
            foreach (var (term, id) in termDictionary)
            {
                await stream.WriteLineAsync($"{term}|{id}");
            }
        }

        public async Task StoreDocsAsync(IDictionary<int, DocInfo> pageDictionary)
        {
            await using var stream = ReadFileAsync(_binFile.Doc);
            foreach (var (id, pi) in pageDictionary)
            {
                await stream.WriteLineAsync($"{id}|{pi.Title}|{pi.Url}|{pi.TokenCount}");
            }
        }

        public async Task StoreIndexAsync(IDictionary<int, List<TermInDoc>> indexDictionary)
        {
            await using var stream = ReadFileAsync(_binFile.Index);
            foreach (var (termId, pages) in indexDictionary)
            {
                await stream.WriteLineAsync(
                    $"{termId}|{string.Join(',', pages.Select(x => $"{x.DocId}:{x.Count}"))}");
            }
        }

        public async Task StoreMatrixAsync(IDictionary<int, double[]> matrix)
        {
            await using var stream = ReadFileAsync(_binFile.TfIdfMatrix);
            foreach (var (_, vector) in matrix)
            {
                await stream.WriteLineAsync(string.Join(",", vector));
            }
        }

        public async Task<List<string>> ReadStopWordsAsync()
        {
            var fileName = "stop_words_english.json";
            var stopWordsStr = await File.ReadAllTextAsync(Path.Combine(FindResPath(fileName), fileName));
            return JsonConvert.DeserializeObject<List<string>>(stopWordsStr);
        }

        public async Task<IDictionary<string, int>> ReadTermsAsync()
        {
            var lines = await ReadLinesAsync(_binFile.Term);
            var ret = new Dictionary<string, int>();
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length != 2) continue;
                ret.TryAdd(parts[0], Convert.ToInt32(parts[1]));
            }

            return ret;
        }

        public async Task<IDictionary<int, DocInfo>> ReadDocsAsync()
        {
            var lines = await ReadLinesAsync(_binFile.Doc);
            var ret = new Dictionary<int, DocInfo>();
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length != 4) continue;
                ret.TryAdd(Convert.ToInt32(parts[0]), new DocInfo
                (
                    Convert.ToInt32(parts[0]),
                    parts[1],
                    parts[2],
                    Convert.ToInt32(parts[3])
                ));
            }

            return ret;
        }

        public async Task<IDictionary<int, List<TermInDoc>>> ReadIndexAsync()
        {
            var lines = await ReadLinesAsync(_binFile.Index);
            var ret = new Dictionary<int, List<TermInDoc>>();
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length != 2) continue;
                var indexes = parts[1].Split(',');
                var list = indexes.Select(i =>
                {
                    var pt = i.Split(':');
                    return new TermInDoc(Convert.ToInt32(parts[0]), Convert.ToInt32(pt[0]), Convert.ToInt32(pt[1]));
                }).ToList();
                ret.TryAdd(Convert.ToInt32(parts[0]), list);
            }

            return ret;
        }

        public async Task<IDictionary<int, double[]>> ReadMatrixAsync()
        {
            var lines = await ReadLinesAsync(_binFile.TfIdfMatrix);
            var ret = new Dictionary<int,  double[]>();
            for (var i = 0; i < lines.Length; i++)
            {
                ret.Add(i + 1, lines[i].Split(',').Select(double.Parse).ToArray());
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
