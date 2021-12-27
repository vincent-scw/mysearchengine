using System;
using System.IO;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    class BinRepository : IRepository
    {
        private const string BF_BIN = "bf.bin";

        public async Task StoreBooleanFilterAsync(bool[] values)
        {
            var bytes = Array.ConvertAll(values, b => b ? (byte) 1 : (byte) 0);
            await using var stream = File.CreateText(Path.Combine(FindResPath(Environment.CurrentDirectory), BF_BIN));
            await stream.WriteAsync(Convert.ToHexString(bytes));
        }

        public async Task<bool[]> ReadBooleanFilterAsync(int initCapacity)
        {
            if (!File.Exists(BF_BIN)) return new bool[initCapacity];

            var values = await File.ReadAllTextAsync(BF_BIN);
            var bytes = Convert.FromHexString(values);
            return Array.ConvertAll(bytes, b => b == (byte)1);

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
