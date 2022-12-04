# Kestrel
Kestrel 是一个跨平台的http服务器，其包含在 ASP.NET Core 项目模板中的 Web 服务器，默认处于启用状态。

Kestrel默认支持以下基础传输层和协议中间件：
* tcp传输层
* quic传输层
* tls协议中间件
* http/websocket协议中间件

### 1 Kestrel的连接对象
当客户端以tcp协议与kestrel建立连接时，kestrel会得到一个`ConnectionContext`对象:
```c#
public abstract class ConnectionContext : BaseConnectionContext, IAsyncDisposable
{
    public abstract IDuplexPipe Transport { get; set; }

    public override void Abort(ConnectionAbortedException abortReason);

    public override void Abort();
}
```
其中，最核心的是抽象的IDuplexPipe传输层对象，里面包含一个Input和一个Output，用于接收连接的输入和向连接输出数据
```c#
public interface IDuplexPipe
{
    PipeReader Input { get; }

    PipeWriter Output { get; }
}
```
 
 ### 2 Kestrel的连接处理者
 ConnectionHandler是一个抽象类型，向kestrel的监听选项注册ConnectionHandler子类，当kestrel收到连接时就调用子类的OnConnectedAsync方法。
 ```c#
public abstract class ConnectionHandler
{        
    public abstract Task OnConnectedAsync(ConnectionContext connection);
}
```

我们可以基于ConnectionHandler来开发某种协议的解析，比如开发MqttConnectionHandler让kestrel支持mqtt协议，比基于Socket开发或其它第三方网络框架都要高效与简单。

### 3 Kestrel的中间件
在kestrel里有一个`IConnectionBuilder.Use(Func<ConnectionDelegate, ConnectionDelegate> middleware)`方法用于注册中间件，`Func<ConnectionDelegate, ConnectionDelegate>`就是kestrel的中间件，我们可以如下安装kestrel的中间件：

```c#
listen.Use(next => context =>
{
    if(true)
    {
        // 中间件1的逻辑 
    }else
    {
        return next(context);
    }
})
.Use(next => context =>
{
    if(true)
    {
        // 中间件2的逻辑
    }else
    {
        return next(context);
    }
});
```
实际上ConnectionHandler就是kestrel里没有next的特殊中间件。