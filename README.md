# KestrelApp
基于Kestrel的网络编程应用示例
希望你能从本项目中学到如何开发 `echo` over `websocket` over `https` over `xxx私有加密` over `tcp`的套娃网络编程

### KestrelApp.Client
让项目支持IConnectionFactory依赖注入，方便从Socket创建ConnectionContext对象

### KestrelApp.Echo
Echo私有协议的简单示例项目，内容包括
1. 了解kestrel的ConnectionHandler

### KestrelApp.HttpProxy
基于kestrel实现的http代理服务器，内容包括
1. Kestrel的中间件编写
2. Kestrel的Feature使用
3. Kestrel的Transport流量转发
4. Http中间件的编写
5. Yarp的简单使用

### KestrelApp.TlsDetect
客户端流量tls协议侦测，内容包括
1. Kestrel的Transport流量协议侦测
2. 单端口多协议服务器的编写技巧

### KestrelApp.Transforms
Kestrel流量的变换，内容包括
1. IDuplexPipe包装为Stream
2. Stream委托
3. Kestrel的Transport的替换
4. 全局流量分析和流量加解密
