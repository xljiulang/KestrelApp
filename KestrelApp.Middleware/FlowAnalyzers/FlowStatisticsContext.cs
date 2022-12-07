using System.Text.Json.Serialization;

namespace KestrelApp.Middleware.FlowAnalyzers
{
    [JsonSerializable(typeof(FlowStatistics))]
    public partial class FlowStatisticsContext : JsonSerializerContext
    {
    }
}
