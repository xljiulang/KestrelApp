using System.IO.Pipelines;

namespace KestrelApp.Transforms.Security
{
    sealed class XorDuplexPipe : DelegatingDuplexPipe<XorStream>
    {
        public XorDuplexPipe(IDuplexPipe duplexPipe) :
            base(duplexPipe, stream => new XorStream(stream))
        {
        }
    }
}
