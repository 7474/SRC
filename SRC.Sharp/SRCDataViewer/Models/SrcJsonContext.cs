using System.Text.Json.Serialization;

namespace SRCDataViewer.Models
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

    // 組の評価（tools/pack-analysis --json）。項目は camelCase
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(AnalysisIndex))]
    [JsonSerializable(typeof(AnalysisSet))]
    internal partial class AnalysisJsonContext : JsonSerializerContext
    {
    }
}
