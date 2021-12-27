using System;

namespace MySearchEngine.Core.Algorithm
{
    public class Tf_Idf
    {
        public static double Calculate(int termCountInDoc, int totalTermsInDoc, int totalDocs, int numberOfDocsWithTerm)
        {
            var tf = (double)termCountInDoc / totalTermsInDoc;
            var idf = Math.Log((double)totalDocs / numberOfDocsWithTerm);

            return tf * idf;
        }
    }
}
