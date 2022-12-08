using System;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// redis响应接口
    /// </summary>
    interface IRedisResponse
    {
        ReadOnlyMemory<byte> ToMemory();
    }
}