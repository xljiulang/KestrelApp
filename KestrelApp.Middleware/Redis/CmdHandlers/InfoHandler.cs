using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// Info处理者
    /// </summary>
    sealed class InfoHandler : RedisCmdHandler
    {
        public override RedisCmdName CmdName => RedisCmdName.Info;

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override async Task HandleAsync(RedisClient client, RedisCmd cmd)
        {
            var response = new InfoResponse("redis_version: 9.9.9");
            await client.ResponseAsync(response);
        }

        private class InfoResponse : RedisResponse
        {
            public InfoResponse(string info)
            {
                //$935
                //redis_version: 2.4.6

                this.Write('$').Write(info.Length.ToString()).WriteLine()
                    .Write(info).WriteLine();
            }
        }
    }
}
