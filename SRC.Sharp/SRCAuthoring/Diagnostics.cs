namespace SRCAuthoring
{
    public enum DiagnosticSeverity
    {
        Error,
        Warning,
        Information,
    }

    /// <summary>データ・シナリオの問題 1 件（行番号は 1 始まり）</summary>
    public sealed class SrcDiagnostic
    {
        public string File { get; set; }
        public int Line { get; set; }
        public DiagnosticSeverity Severity { get; set; }
        public string Message { get; set; }
        /// <summary>問題のあったデータの種類や名前（あれば）</summary>
        public string Source { get; set; }

        public override string ToString()
        {
            var severity = Severity switch
            {
                DiagnosticSeverity.Error => "error",
                DiagnosticSeverity.Warning => "warning",
                _ => "info",
            };
            // SRCDataLinter と同じ書式（src-lint.json の problemMatcher で読める）
            return $"{File}({Line}): {severity}: {Message}" + (string.IsNullOrEmpty(Source) ? "" : $"[{Source}]");
        }
    }
}
