using System.Text.Json.Serialization;

namespace SRCTestBlazor.Models
{
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(SrcBitmapIndex))]
    [JsonSerializable(typeof(SrcDataIndex))]
    [JsonSerializable(typeof(ViewerConfig))]
    [JsonSerializable(typeof(ArchiveIndex))]
    [JsonSerializable(typeof(ArchiveFileList))]
    internal partial class SrcJsonContext : JsonSerializerContext
    {
    }
}
