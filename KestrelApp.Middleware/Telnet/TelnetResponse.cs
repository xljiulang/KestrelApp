using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Telnet
{
    sealed class TelnetResponse
    {
        private readonly PipeWriter writer;

        public TelnetResponse(PipeWriter writer)
        {
            this.writer = writer;
        }

        public ValueTask<FlushResult> WriteLineAsync(ReadOnlySpan<char> text, Encoding? encoding = null)
        {
            this.WriteLine(text, encoding);
            return this.FlushAsync();
        }

        public TelnetResponse WriteLine(ReadOnlySpan<char> text, Encoding? encoding = null)
        {
            this.writer.Write(text, encoding ?? Encoding.UTF8);
            this.writer.WriteCRLF();
            return this;
        }

        public ValueTask<FlushResult> FlushAsync()
        {
            return this.writer.FlushAsync();
        }
    }
}
