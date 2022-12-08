using KestrelFramework.Application;
using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.Telnet
{
    sealed class TelnetContext : ApplicationContext
    {
        private readonly ConnectionContext context;

        public string Request { get; }

        public TelnetResponse Response { get; }

        public TelnetContext(string request, ConnectionContext context)
            : base(context.Features)
        {
            this.context = context;
            this.Request = request;
            this.Response = this.Features.Get<TelnetResponse>()!;
        }

        public void Abort()
        {
            this.context.Abort();
        }
    }
}
