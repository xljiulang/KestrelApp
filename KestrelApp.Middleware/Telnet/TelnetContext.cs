using KestrelFramework.Application;
using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.Telnet
{
    sealed class TelnetContext : ApplicationContext
    {
        private readonly ConnectionContext context;

        public string Request { get; }

        public TelnetResponse Response { get; }

        public TelnetContext(string request, TelnetResponse response, ConnectionContext context)
            : base(context.Features)
        {
            this.context = context;
            this.Request = request;
            this.Response = response;
        }

        public void Abort()
        {
            this.context.Abort();
        }
    }
}
