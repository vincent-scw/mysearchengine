# MySearchEngine

MySearchEngine is a learning and practice project for searching. It is developed by C# language in .Net 5.0 environment.

## Target
Try NOT use any 3rd party tools, and build a simple search engine by myself.  
The source data comes from internet. We will start with web crawling, text analyzing, index building. At last, it should support searching, the result should be sorted by relevance score.

## Project Structure

## Processing Stage

## Features
[x] Web Crawler
[ ] Index Building
	[x] Character Filter
		[x] Html Filter
	[ ] Tokenizer
		[x] Simple Tokenizer
		[ ] Support phrase
	[ ] Token Filter
		[x] Lowercase Filter
		[x] Stemmer Filter
		[x] Stop Words Filter
		[ ] Synonym Filter
[x] Index Storage
	[x] Term
	[x] Doc
	[x] Inverted Index
[ ] Search
	[x] TF-IDF
	[ ] BM25