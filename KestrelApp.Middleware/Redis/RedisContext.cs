namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示redis上下文
    /// </summary>
    sealed class RedisContext
    {
        /// <summary>
        /// 获取redis客户端
        /// </summary>
        public RedisClient Client { get; }

        /// <summary>
        /// 获取redis命令
        /// </summary>
        public RedisCmd Cmd { get; }

        /// <summary>
        /// redis上下文
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd"></param>
        public RedisContext(RedisClient client, RedisCmd cmd)
        {
            Client = client;
            Cmd = cmd;
        }

        public override string ToString()
        {
            return $"{this.Client} {this.Cmd}";
        }
    }
}
