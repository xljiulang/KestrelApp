using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Telnet
{
    static class PipeWriterExtensions
    {
        private static readonly byte[] crlf = Encoding.ASCII.GetBytes("\r\n");

        public static ValueTask<FlushResult> WriteLineAsync(this PipeWriter writer, ReadOnlySpan<char> text, Encoding? encoding = null)
        {
            writer.WriteLine(text, encoding);
            return writer.FlushAsync();
        }

        public static void WriteLine(this PipeWriter writer, ReadOnlySpan<char> text, Encoding? encoding = null)
        {
            writer.Write(text, encoding ?? Encoding.UTF8);
            writer.Write(crlf);
        }
    }
}
