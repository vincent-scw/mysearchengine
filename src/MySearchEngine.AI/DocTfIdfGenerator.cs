using MySearchEngine.Core;
using MySearchEngine.Core.Algorithm;
using System;

namespace MySearchEngine.AI
{
    internal class DocTfIdfGenerator
    {
        private IRepository _repository;
        private IDictionary<int, double[]> _matrix;

        private IDictionary<int, DocInfo> _docs;
        private IDictionary<string, int> _terms;
        private IDictionary<int, List<TermInDoc>> _indexes;

        public DocTfIdfGenerator(IRepository repository) 
        {
            _repository = repository;
            _matrix = new Dictionary<int, double[]>();
        }

        public async Task Generate()
        {
            _docs = await _repository.ReadDocsAsync();
            _terms = await _repository.ReadTermsAsync();
            _indexes = await _repository.ReadIndexAsync();

            foreach (var doc in _docs)
            {
                var vector = new double[_terms.Count];
                foreach (var i in _indexes) 
                {
                    vector[i.Key - 1] = Math.Round(Tf_Idf.Calculate(
                        i.Value.SingleOrDefault(x => x.DocId == doc.Key)?.Count ?? 0, 
                        doc.Value.TokenCount, 
                        _docs.Count, 
                        i.Value.Count) * 10, // 乘以10
                        4); // 保留4位小数
                }

                _matrix.Add(doc.Key, vector);
            }

            await _repository.StoreMatrixAsync(_matrix);
        }

        public async Task Load()
        {
            _docs = await _repository.ReadDocsAsync();
            _terms = await _repository.ReadTermsAsync();
            _indexes = await _repository.ReadIndexAsync();

            _matrix = await _repository.ReadMatrixAsync();
        }

        public async Task<SimilarDocuments> GetSimilarDocsAsync(int docId)
        {
            var doc = _docs.Single(d => d.Key == docId);
            var docVector = _matrix[docId];
            var priorityQueue = new PriorityQueue<DocInfo, double>(Comparer<double>.Create((x, y) => y.CompareTo(x)));

            for (var i = 0; i < _matrix.Count; i++)
            {
                if (i == docId - 1)
                {
                    continue;
                }

                var another = _matrix[i + 1];
                var similarity = CalculateCosine(docVector, another);
                priorityQueue.Enqueue(_docs[i + 1], similarity);
            }

            // 使用一个列表来存储前10个最大的元素
            List<DocInfo> top10 = new List<DocInfo>();

            // 从优先级队列中取出前10个元素
            while (priorityQueue.Count > 0 && top10.Count < 10)
            {
                top10.Add(priorityQueue.Dequeue());
            }

            top10.OrderDescending();

            return new SimilarDocuments
            {
                DocTitle = doc.Value.Title,
                DocId = docId,
                DocUrl = doc.Value.Url,
                SimilarDocs = top10
            };
        }

        public static double CalculateCosine(double[] vectorA, double[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                throw new ArgumentException("Vectors must have the same dimensions.");
            }

            double dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
            double magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
            double magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));

            if (magnitudeA == 0 || magnitudeB == 0)
            {
                throw new ArgumentException("Vectors must not have zero magnitude.");
            }

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}
