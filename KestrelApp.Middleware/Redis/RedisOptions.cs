namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// redis选项
    /// </summary>
    public record RedisOptions
    {
        /// <summary>
        /// 秘钥
        /// </summary>
        public string? Auth { get; set; }
    }
}
