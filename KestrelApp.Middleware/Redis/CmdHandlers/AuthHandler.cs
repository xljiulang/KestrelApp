using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// Auth处理者
    /// </summary>
    sealed class AuthHandler : RedisCmdHandler
    {
        private readonly IOptionsMonitor<RedisOptions> options;

        public override RedisCmdName CmdName => RedisCmdName.Auth;

        /// <summary>
        /// Auth处理者
        /// </summary>
        /// <param name="options"></param>
        public AuthHandler(IOptionsMonitor<RedisOptions> options)
        {
            this.options = options;
        }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override async Task HandleAsync(RedisClient client, RedisCmd cmd)
        {
            if (client.IsAuthed == null)
            {
                var auth = this.options.CurrentValue.Auth;
                if (string.IsNullOrEmpty(auth))
                {
                    client.IsAuthed = true;
                }
                else if (cmd.ArgumentCount > 0)
                {
                    var password = cmd.Argument(0).Value;
                    client.IsAuthed = password.Span.SequenceEqual(Encoding.UTF8.GetBytes(auth));
                }
            }

            if (client.IsAuthed == true)
            {
                await client.ResponseAsync(RedisResponse.OK);
            }
            else
            {
                await client.ResponseAsync(RedisResponse.Err);
            }
        }
    }
}
