namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 命令的名称
    /// </summary>
    enum RedisCmd
    {
        /// <summary>
        /// 未知
        /// </summary>
        UnKnow,

        /// <summary>
        /// 授权
        /// </summary>
        Auth,

        /// <summary>
        /// ping
        /// </summary>
        Ping,

        /// <summary>
        /// 退出
        /// </summary>
        Quit,

        /// <summary>
        /// 回声
        /// </summary>
        Echo,

        /// <summary>
        /// 消息
        /// </summary>
        Info,

        /// <summary>
        /// 选择db
        /// </summary>
        Select,
    }
}
