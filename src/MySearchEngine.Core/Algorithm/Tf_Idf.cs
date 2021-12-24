using System;

namespace MySearchEngine.Core.Algorithm
{
    public class Tf_Idf
    {
        public static double Calculate(int termCountInPage, int totalTermsInPage, int totalPages, int termInPages)
        {
            var tf = (double)termCountInPage / totalTermsInPage;
            var idf = Math.Log10((double)totalPages / termInPages);

            return tf * idf;
        }
    }
}
