using System.Text.Json.Serialization;

namespace SRCTestBlazor.Models
{
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(SrcBitmapIndex))]
    [JsonSerializable(typeof(SrcDataIndex))]
    [JsonSerializable(typeof(ResourceCatalog))]
    internal partial class SrcJsonContext : JsonSerializerContext
    {
    }
}
