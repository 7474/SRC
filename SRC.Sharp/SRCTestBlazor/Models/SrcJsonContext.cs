using System.Text.Json.Serialization;

namespace SRCTestBlazor.Models
{
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(SrcBitmapIndex))]
    [JsonSerializable(typeof(SrcDataIndex))]
    internal partial class SrcJsonContext : JsonSerializerContext
    {
    }
}
