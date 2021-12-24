# mysearchengine

MySearchEngine 是一个学习与实践文本搜索相关知识的个人项目。它是用C#语言基于.Net 5.0开发。

## 项目结构
MySearchEngine 由三个可运行客户端和一个核心Library项目组成。

### MySearchEngine.Core
在Core中包含文本搜索的一些基础算法。

### MySearchEngine.QueueService
这是一个简单的消息中间件。它负责把网络爬虫爬下来的HTML内容传送给Server端，并由Server端创建索引。

### MySearchEngine.WebCrawler
WebCrawler是一个网络爬虫。

### MySearchEngine.Server
Server端对网络爬虫爬得的HTML内容创建索引，并提供搜索的API端口。
