using System.IO.Pipelines;

namespace KestrelApp.Middleware.FlowXor
{
    sealed class XorDuplexPipe : DelegatingDuplexPipe<XorStream>
    {
        public XorDuplexPipe(IDuplexPipe duplexPipe) :
            base(duplexPipe, stream => new XorStream(stream))
        {
        }
    }
}
