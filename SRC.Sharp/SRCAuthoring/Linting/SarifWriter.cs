using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SRCAuthoring.Linting
{
    /// <summary>
    /// 診断を SARIF 2.1.0 で書き出す（GitHub のコードスキャンに表示できる）。
    /// https://docs.github.com/en/code-security/code-scanning/integrating-with-code-scanning/sarif-support-for-code-scanning
    /// </summary>
    public static class SarifWriter
    {
        public static string Write(IEnumerable<SrcDiagnostic> diagnostics, string baseDirectory)
        {
            var results = new JsonArray();
            foreach (var d in diagnostics)
            {
                results.Add(new JsonObject
                {
                    ["ruleId"] = d.Severity == DiagnosticSeverity.Error ? "src-data-error" : "src-data-warning",
                    ["level"] = d.Severity switch { DiagnosticSeverity.Error => "error", DiagnosticSeverity.Warning => "warning", _ => "note" },
                    ["message"] = new JsonObject { ["text"] = d.Message + (string.IsNullOrEmpty(d.Source) ? "" : $" [{d.Source}]") },
                    ["locations"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["physicalLocation"] = new JsonObject
                            {
                                ["artifactLocation"] = new JsonObject { ["uri"] = ToUri(d.File, baseDirectory) },
                                ["region"] = new JsonObject { ["startLine"] = Math.Max(1, d.Line) },
                            },
                        },
                    },
                });
            }
            var sarif = new JsonObject
            {
                ["$schema"] = "https://json.schemastore.org/sarif-2.1.0.json",
                ["version"] = "2.1.0",
                ["runs"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["tool"] = new JsonObject
                        {
                            ["driver"] = new JsonObject
                            {
                                ["name"] = "srcsharp",
                                ["informationUri"] = "https://github.com/7474/SRC",
                                ["rules"] = new JsonArray
                                {
                                    new JsonObject { ["id"] = "src-data-error", ["shortDescription"] = new JsonObject { ["text"] = "データ・イベントのエラー" } },
                                    new JsonObject { ["id"] = "src-data-warning", ["shortDescription"] = new JsonObject { ["text"] = "データ・イベントの警告" } },
                                },
                            },
                        },
                        ["results"] = results,
                    },
                },
            };
            return sarif.ToJsonString(new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }

        private static string ToUri(string file, string baseDirectory)
        {
            if (string.IsNullOrEmpty(file))
            {
                return "";
            }
            var path = Path.GetFullPath(file);
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                var relative = Path.GetRelativePath(Path.GetFullPath(baseDirectory), path);
                if (!relative.StartsWith(".."))
                {
                    return relative.Replace('\\', '/');
                }
            }
            return new Uri(path).AbsoluteUri;
        }
    }
}
