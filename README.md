# MySearchEngine

English | [中文](README-CN.md)

MySearchEngine is a learning and practice project for searching. It is developed by C# language in .Net 5.0 environment.

## Target
Try NOT use any 3rd party tools, and build a simple search engine by myself.  
The source data comes from internet. We will start with web crawling, text analyzing, index building. At last, it should support searching, the result should be sorted by relevance score.

## How to start
1. Run [QueueService](start_queue.bat).
1. Run [WebCrawler](start_crawler.bat).
1. Run [Server](start_sever.bat).

## Project Structure
MySearchEngine consits of three runable consoles and one core library.

![design](res/design.png)

Note：Client can be anything calling Server API

- MySearchEngine.Core

  It contains a few core algorithm about searching.

- MySearchEngine.QueueService

  It is a simple queue service. It's responsible to transit the HTML content crawled from internet to Server.

- MySearchEngine.WebCrawler

  It is a web crawler.

- MySearchEngine.Server

  Server indexes the HTML content, and serve API endpoint for searching.

## Processing Stage
1. Web Crawling Stage

   I like the website [DifferenceBetween](https://www.differencebetween.com). So I want to crawl this site for searching. However, because most of the contents on DifferenceBetween are very technical, I'd choose start crawling from [Language Category](https://www.differencebetween.com/category/language/)……  
   Crawler's responsibility is downloading pages, and grab all links from the page. Then choose those valuable links to crawl again. Repeat these steps. The key point for this stage is ***How to determine whether a link has been visited or not?***   
   It's quite obivious that when time goes by, the visited links grows rapidly. We'll run into issues if use Arrary，Dictionary or HashMap to store links. They will have either performance issue or memory issue. So ***Bloom Filter*** is recommended for this case.  

   > ***Bloom Filter***是一种非常高效、资源利用率高的数据结构。
   > 布隆过滤器维护一个定长的boolean数组。它的原理是通过若干个不同的Hash函数来对源文本取Hash值。把Hash值取模之后，在过滤器中的相应位置的boolean值置为1。  
   > 这样可对新的源文本同样取Hash值，如果过滤器中的相应位置已经全为1，则此文本已经被访问过了。只要有一个位置为0，则此源文本未被访问。   
   > 布隆过滤器的问题是依然会有哈希碰撞，导致新的源文本有几率会被误认为已被访问。不过这种情况在网络爬虫中是可被接受的，因为结果无非是少爬取几个网页罢了。 
   >
   > ![bloomfilter](res/bloomfilter.png)
   >
   > Ref. to [wiki](https://en.wikipedia.org/wiki/Bloom_filter)

1. Index Creating Stage
  
   在索引创建的阶段，其实质就是对文本进行分析的过程。文本分析的结果就是一组Token，也就是文本中的单词或者词组。  
   这里参考了Elasticsearch的Analyzer的设计。分析分为三个过程：Character filter(s), Tokenizer, Token filter(s)。其中Filter(s)可以有多个，按顺序执行。  
   - Character filter(s)：可以对原始文本所包含的字符进行添加、删除或者更改。
   - Tokenizer：把经过Character filter(s)之后的内容分解为Token。因为这里爬取的都是英文网站，所以使用最简单的基于whitespace的分解。
   - Token filter(s)：对已经分解好的Token进行进一步的处理。比如转化为小写，合并相同词根，添加同义词等等。 
   
   
   > The process of ***Analyzing***  
   >
   > ![textanalyzer](res/textanalyzer.png)  
   > 
   > Take an example of below input:  
   > ```html
   > <div><p>The QUICK brown fox jumps.</p></div>
   > ```
   > 
   > |Process|Description|Outcome|
   > |---|---|---|
   > |[HtmlElementFilter](src/MySearchEngine.Core/Analyzer/CharacterFilters/HtmlElementFilter.cs)|Remove Html elements. |The QUICK brown fox jumps. |
   > |[SimpleTokenizer](src/MySearchEngine.Core/Analyzer/Tokenizers/SimpleTokenizer.cs)|Split text to tokens by whitespace. Leave only letters & digits.|["The", "QUICK", "brown", "fox", "jumps"]|
   > |[LowercaseTokenFilter](src/MySearchEngine.Core/Analyzer/TokenFilters/LowercaseTokenFilter.cs)|Update tokens to lowercased.|["the", "quick", "brown", "fox", "jumps"]|
   > |[StemmerTokenFilter](src/MySearchEngine.Core/Analyzer/TokenFilters/StemmerTokenFilter.cs)|Update tokens to its stem using [PorterStemmer](https://iq.opengenus.org/porter-stemmer/).|["the", "quick", "brown", "fox", "jump"]|
   > |[StopWordTokenFilter](src/MySearchEngine.Core/Analyzer/TokenFilters/StopWordTokenFilter.cs)|Remove tokens of stop words.|["quick", "brown", "fox", "jump"]|
   >
   > Ref. to [Analyzer Anatomy](https://www.elastic.co/guide/en/elasticsearch/reference/current/analyzer-anatomy.html)
   
1. Index Storing Stage

   - 将每一个Term以 `{term}|{termId}` 的格式保存为term.bin文件
   - 将每一个Doc以 `{docId}|{url}|{allTermsInDoc}` 的格式保存为doc.bin文件
   - 将倒排索引以 `{termId}|{docId}:{termCountInDoc},...` 的格式保存为index.bin文件（注：用逗号分隔每一个doc）

1. Searching Stage

   与索引阶段相同，将输入的SearchText用TextAnalyzer分析出一组Token。然后通过倒排索引找到对应的Doc。最后利用TF-IDF进行算分。
   
   > ***TF-IDF***（Term Frequency - Inverse Document Frequency）  
   > Term Frequency: 一般认为同一个Term在某文档中出现的次数越多，则这个Term在这篇文档中的重要性越高。  
   > Inverse Document Frequency：相反的，同一个Term在不同文档中出现的次数越多，则这个Term就更通用。它相对的重要性就越低。
   >
   > Ref. to [tfidf](http://tfidf.com/)
   
## Summary

## Features
- [x] Web Crawler
- [ ] Index Building
	- [x] Character Filter
		- [x] Html Filter
	- [ ] Tokenizer
		- [x] Simple Tokenizer
		- [ ] Support phrase
	- [ ] Token Filter
		- [x] Lowercase Filter
		- [x] Stemmer Filter
		- [x] Stop Words Filter
		- [ ] Synonym Filter
- [x] Index Storage
	- [x] Term
	- [x] Doc
	- [x] Inverted Index
- [ ] Search
	- [x] TF-IDF
	- [ ] BM25