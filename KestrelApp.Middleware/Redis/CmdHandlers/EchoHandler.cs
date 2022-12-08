using System;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// Echo处理者
    /// </summary>
    sealed class EchoHandler : IRedisCmdHanler
    {
        public RedisCmd Cmd => RedisCmd.Echo;

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param> 
        /// <returns></returns>
        public async ValueTask HandleAsync(RedisContext context)
        {
            var echo = context.Reqeust.ArgumentCount > 0
                ? context.Reqeust.Argument(0)
                : new RedisValue(ReadOnlyMemory<byte>.Empty);

            //$2
            //xx 
            await context.Response
                .Write('$').Write(echo.Value.Length.ToString()).WriteLine()
                .Write(echo.Value).WriteLine()
                .FlushAsync();
        }
    }
}
