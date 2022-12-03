using System.IO.Pipelines;

namespace KestrelApp.Transforms.Analyzers
{
    sealed class FlowAnalyzeDuplexPipe : DelegatingDuplexPipe<FlowAnalyzeStream>
    {
        public FlowAnalyzeDuplexPipe(IDuplexPipe duplexPipe, IFlowAnalyzer flowAnalyzer) :
            base(duplexPipe, stream => new FlowAnalyzeStream(stream, flowAnalyzer))
        {
        }
    }
}
