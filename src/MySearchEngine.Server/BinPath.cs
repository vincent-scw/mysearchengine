﻿namespace MySearchEngine.Server
{
    public class BinPath
    {
        /// <summary>
        /// The file path for inverted index
        /// </summary>
        public string Index { get; set; }
        /// <summary>
        /// The file path for Term
        /// </summary>
        public string Term { get; set; }
        /// <summary>
        /// The file path for Page
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// The file path for term offset in inverted index file
        /// </summary>
        public string TermOffset { get; set; }
    }
}
