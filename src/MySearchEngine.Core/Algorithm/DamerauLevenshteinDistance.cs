using System;

namespace MySearchEngine.Core.Algorithm
{
    public class DamerauLevenshteinDistance
    {
        public static int DistanceBetween(string one, string other, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(one) || string.IsNullOrEmpty(other))
            {
                return Math.Max(one?.Length ?? 0, other?.Length ?? 0);
            }

            if (ignoreCase)
            {
                one = one.ToLower();
                other = other.ToLower();
            }

            int m = one.Length + 1, n = other.Length + 1;
            int[,] matrix = new int[m, n];

            for (var i = 0; i < m; i++) { matrix[i, 0] = i; }
            for (var j = 0; j < n; j++) { matrix[0, j] = j; }

            for (var p = 1; p < m; p++)
            {
                for (var q = 1; q < n; q++)
                {
                    // If the characters at current position are same, then the cost is 0
                    var cost = one[p - 1] == other[q - 1] ? 0 : 1;
                    var insertion = matrix[p, q - 1] + 1;
                    var deletion = matrix[p - 1, q] + 1;
                    var sub = matrix[p - 1, q - 1] + cost;

                    // Get the minimum
                    var distance = Math.Min(insertion, Math.Min(deletion, sub));
                    if (p > 1 && q > 1 && one[p - 1] == other[q - 2] && one[q - 2] == other[q - 1])
                    {
                        distance = Math.Min(distance, matrix[q - 2, p - 2] + cost);
                    }

                    matrix[p, q] = distance;
                }
            }

            return matrix[m - 1, n - 1];
        }
    }
}
