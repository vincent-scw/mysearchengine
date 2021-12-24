# mysearchengine

MySearchEngine 是一个学习与实践文本搜索相关知识的个人项目。它是用C#语言基于.Net 5.0开发。

## 目标
我觉得[DifferenceBetween](https://www.differencebetween.com)是一个不错的网站。所以我想对它做一下内容爬取，并且可以对爬得的数据做个简单的搜索引擎。但是因为DifferenceBetween上面的大部分文章内容太过于学术化，所以我们就选取容易理解的[Language Category](https://www.differencebetween.com/category/language/)来开始爬取……

## 项目结构
MySearchEngine 由三个可运行客户端和一个核心Library项目组成。

- MySearchEngine.Core

  在Core中包含文本搜索的一些基础算法。

- MySearchEngine.QueueService

  这是一个简单的消息中间件。它负责把网络爬虫爬下来的HTML内容传送给Server端，并由Server端创建索引。

- MySearchEngine.WebCrawler

  WebCrawler是一个网络爬虫。

- MySearchEngine.Server

  Server端对网络爬虫爬得的HTML内容创建索引，并提供搜索的API端口。

## 过程分享
1. 实现一个网络爬虫
2. 实现索引的创建
3. 保存索引数据
4. 实现搜索功能
