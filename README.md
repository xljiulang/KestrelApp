# KestrelApp
基于Kestrel的网络编程应用示例

### 1 项目意图
1. 了解网络编程不再需要从Socket开始
2. 了解网络编程不再需要第三方框架(包括Dotnetty)
3. 了解`telnet` over `websocket` over `tls` over `xxx私有加密` over `tcp`的套娃网络编程
4. 能基于KestrelFramework开发网络应用

### 2 文档资料
**内部文档**：[docs](docs)

**外部文档**
* [Pipelines](https://learn.microsoft.com/zh-cn/dotnet/standard/io/pipelines)
* [Buffers](https://learn.microsoft.com/zh-cn/dotnet/standard/io/buffers)

### 3 KestrelFramework
kestrel网络编程一些必要的基础库
1. Kestrel中间件接口与中间注册
2. System.Buffers: 缓冲区操作类
3. System.IO: 流的操作类
4. System.IO.Pipelines: 双工管道操作类
5. Middleware: kestrel的一些中间件

### 4 KestrelApp
Kestrel应用程序，内容包括
1. 监听的EndPoint的配置
2. EndPoint使用的协议配置

### 5 KestrelApp.Middleware
KestrelApp的中间件类库
#### 5.1 Echo
简单Echo应用协议的示例

#### 5.2 FlowAnalyze
传输层流量统计中间件示例

#### 5.3 FlowXor
传输层流量异或处理的中间件示例

#### 5.4 HttpProxy
http代理应用协议的示例

#### 5.5 Telnet
简单Telnet应用协议的示例

#### 5.6 TelnetProxy
流量转发到telnet服务器的的示例

### 开源有你更精彩
![打赏](reward.png)
