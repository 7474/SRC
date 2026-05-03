using System.Text.Json.Serialization;

namespace SRCTestBlazor.Models
{
    [JsonSerializable(typeof(SrcBitmapIndex))]
    [JsonSerializable(typeof(SrcDataIndex))]
    internal partial class SrcJsonSerializerContext : JsonSerializerContext
    {
    }
}
