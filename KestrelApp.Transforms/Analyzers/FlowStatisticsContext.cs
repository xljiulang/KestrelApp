using System.Text.Json.Serialization;

namespace KestrelApp.Transforms.Analyzers
{
    [JsonSerializable(typeof(FlowStatistics))]
    public partial class FlowStatisticsContext : JsonSerializerContext
    {
    }
}
