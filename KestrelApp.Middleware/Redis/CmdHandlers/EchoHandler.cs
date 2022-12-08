using System;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// Echo处理者
    /// </summary>
    sealed class EchoHandler : RedisCmdHandler
    {
        public override RedisCmdName CmdName => RedisCmdName.Echo;

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override async Task HandleAsync(RedisClient client, RedisCmd cmd)
        {
            var echo = cmd.ArgumentCount > 0 ? cmd.Argument(0) : new RedisValue(Array.Empty<byte>());
            var response = new EchoResponse(echo);
            await client.ResponseAsync(response);
        }

        /// <summary>
        /// 表示回声
        /// </summary>
        private class EchoResponse : RedisResponse
        {
            private readonly BufferBuilder builder = new();

            /// <summary>
            /// 回声命令
            /// </summary>
            /// <param name="echo"></param>
            public EchoResponse(RedisValue echo)
            {
                //$2
                //xx 
                this.builder
                    .Write('$').Write(echo.Value.Length.ToString()).WriteLine()
                    .Write(echo.Value).WriteLine();
            }


            public override ReadOnlyMemory<byte> ToMemory()
            {
                return this.builder.WrittenMemory;
            }
        }
    }
}
