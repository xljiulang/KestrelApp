# KestrelApp
基于Kestrel的网络编程应用示例

### 项目用途
1. 了解网络编程不再需要从Socket开始
2. 了解网络编程不再需要第三方框架(包括Dotnetty)
3. 了解`telnet` over `websocket` over `tls` over `xxx私有加密` over `tcp`的套娃网络编程

### 更多文档
#### 内部文档库
[docs](docs)

#### 外部文档
* [Pipelines](https://learn.microsoft.com/zh-cn/dotnet/standard/io/pipelines)
* [Buffers](https://learn.microsoft.com/zh-cn/dotnet/standard/io/buffers)

### 项目介绍
#### 1 KestrelFramework
1. System.Buffers: 缓冲区操作类
2. System.IO: 流的操作类
3. System.IO.Pipelines: 双工管道操作类
4. Middleware: kestrel的一些中间件

#### 2 KestrelApp.Telnet
Telnet协议的简单示例项目，内容包括
1. 了解Kestrel的ConnectionHandler
2. 了解PipeReader和PipeWriter

#### 3 KestrelApp.HttpProxy
基于kestrel实现的http代理服务器，内容包括
1. Kestrel的中间件编写
2. Kestrel的Feature使用
3. Kestrel的Transport流量转发
4. Http中间件的编写
5. Yarp的简单使用

#### 4 KestrelApp.Transforms
Kestrel流量的变换，内容包括
1. 全局流量分析
2. 流量Xor变换

### 开源有你更精彩
![打赏](reward.png)
