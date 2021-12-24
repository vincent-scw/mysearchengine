﻿using System.Collections.Generic;

namespace MySearchEngine.Core
{
    public class Token
    {
        public int Id { get; set; }
        public string Term { get; set; }
        public List<int> Positions { get; set; }
        public int TermInDocCount => Positions.Count;

        public Token(int id, string term)
        {
            Id = id;
            Term = term;
            Positions = new List<int>();
        }
    }
}