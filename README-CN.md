# MySearchEngine

[English](.\README.md)|中文

MySearchEngine 是一个学习与实践文本搜索相关知识的个人项目。它基于C#语言在.Net 5.0环境下开发。

## 目标
尽量不使用第三方工具，自己实现一个简单的搜索引擎。其数据来源于网络。我们从爬取某网页开始，然后分析内容，创建索引。最后能够完成搜索，并按照得分顺序返回结果。

## 项目结构
MySearchEngine 由三个可运行客户端和一个核心Library项目组成。

![design](res/design.png)

注：Client可以为任意调用Server API的客户端

- MySearchEngine.Core

  在Core中包含文本搜索的一些基础算法。

- MySearchEngine.QueueService

  这是一个简单的消息中间件。它负责把网络爬虫爬下来的HTML内容传送给Server端，并由Server端创建索引。

- MySearchEngine.WebCrawler

  WebCrawler是一个网络爬虫。

- MySearchEngine.Server

  Server端对网络爬虫爬得的HTML内容创建索引，并提供搜索的API端口。

## 阶段&过程
1. 网络爬虫阶段

   在爬虫阶段，我觉得[DifferenceBetween](https://www.differencebetween.com)是一个不错的网站。所以我想对它做一下内容爬取。但是因为DifferenceBetween上面的大部分文章内容太过于学术化，所以我们就选取容易理解的[Language Category](https://www.differencebetween.com/category/language/)开始吧……  
   爬虫的主要工作就是下载网页，读取网页内的所有超链接。然后选择我们认为有价值的链接再爬取。如此循环往复。其中值得一提的点是***如何判断网页链接是否已经爬取过？***。  
   很显然随着时间的推移，已爬取链接的数量剧增。仅使用Arrary，Dictionary或者HashMap都不太实际。它们要么有性能问题，要么占用太多内存资源。这边推荐使用***布隆过滤器***来实现。  

   > ***布隆过滤器***是一种非常高效、资源利用率高的数据结构。
   > 布隆过滤器维护一个定长的boolean数组。它的原理是通过若干个不同的Hash函数来对源文本取Hash值。把Hash值取模之后，在过滤器中的相应位置的boolean值置为1。  
   > 这样可对新的源文本同样取Hash值，如果过滤器中的相应位置已经全为1，则此文本已经被访问过了。只要有一个位置为0，则此源文本未被访问。   
   > 布隆过滤器的问题是依然会有哈希碰撞，导致新的源文本有几率会被误认为已被访问。不过这种情况在网络爬虫中是可被接受的，因为结果无非是少爬取几个网页罢了。 
   >
   > ![bloomfilter](res/bloomfilter.png)
   >
   > 详情请参考[wiki](https://en.wikipedia.org/wiki/Bloom_filter)

1. 索引创建阶段
  
   在索引创建的阶段，其实质就是对文本进行分析的过程。文本分析的结果就是一组Token，也就是文本中的单词或者词组。  
   这里参考了Elasticsearch的Analyzer的设计。分析分为三个过程：Character filter(s), Tokenizer, Token filter(s)。其中Filter(s)可以有多个，按顺序执行。  
   - Character filter(s)：可以对原始文本所包含的字符进行添加、删除或者更改。
   - Tokenizer：把经过Character filter(s)之后的内容分解为Token。因为这里爬取的都是英文网站，所以使用最简单的基于whitespace的分解。
   - Token filter(s)：对已经分解好的Token进行进一步的处理。比如转化为小写，合并相同词根，添加同义词等等。 
   
   
   > 文本分析过程  
   >
   > ![textanalyzer](res/textanalyzer.png)
   >
   > 详情请参考[Analyzer Anatomy](https://www.elastic.co/guide/en/elasticsearch/reference/current/analyzer-anatomy.html)
   
1. 索引数据保存阶段

   - 将每一个Term以 `{term}|{termId}` 的格式保存为term.bin文件
   - 将每一个Doc以 `{docId}|{url}|{allTermsInDoc}` 的格式保存为doc.bin文件
   - 将倒排索引以 `{termId}|{docId}:{termCountInDoc},...` 的格式保存为index.bin文件（注：用逗号分隔每一个doc）

1. 搜索阶段

   与索引阶段相同，将输入的SearchText用TextAnalyzer分析出一组Token。然后通过倒排索引找到对应的Doc。最后利用TF-IDF进行算分。
   
   > ***TF-IDF***（Term Frequency - Inverse Document Frequency）  
   > Term Frequency: 一般认为同一个Term在某文档中出现的次数越多，则这个Term在这篇文档中的重要性越高。  
   > Inverse Document Frequency：相反的，同一个Term在不同文档中出现的次数越多，则这个Term就更通用。它相对的重要性就越低。
   >
   > 详情请参考[tfidf](http://tfidf.com/)

## 实现列表
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