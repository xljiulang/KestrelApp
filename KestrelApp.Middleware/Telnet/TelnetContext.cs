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
            this.Request = request;
            this.context = context;
            this.Response = new TelnetResponse(context.Transport.Output);
        }

        public void Abort()
        {
            this.context.Abort();
        }
    }
}
