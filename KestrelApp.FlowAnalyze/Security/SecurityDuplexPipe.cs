using System.IO.Pipelines;

namespace KestrelApp.Transforms.Security
{
    sealed class SecurityDuplexPipe : DelegatingDuplexPipe<SecurityStream>
    {
        public SecurityDuplexPipe(IDuplexPipe duplexPipe) :
            base(duplexPipe, stream => new SecurityStream(stream))
        {
        }
    }
}
