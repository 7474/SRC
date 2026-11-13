using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCAuthoring.Rules
{
    /// <summary>
    /// 規則のグループ（タグ）。グループは規則の観点（誰の、何の基準か）を表す。
    /// 1 つの規則は 1 つ以上のグループに入る。lint は選んだグループのどれかに入る規則を動かす。
    /// 方針は docs/decisions/2026-11-13-authoring-tools-convergence.md
    /// </summary>
    public static class RuleGroups
    {
        /// <summary>SRC の仕様と食い違い、意図どおりに動かないデータ。根拠は元の SRC の実装。既定で動く</summary>
        public const string Src = "src";
        /// <summary>GSC データパックのロボ基準の書き方の決まり</summary>
        public const string GscRobo = "gsc-robo";
        /// <summary>GSC データパックの拡張基準の書き方の決まり</summary>
        public const string GscExt = "gsc-ext";
        /// <summary>ロボダン（ロボッとダンジョンＢ！）の Lib\UnitChecker.eve の観点。ロボダンの基準で、ほかの作品の基準ではない</summary>
        public const string Robodun = "robodun";

        public static IReadOnlyList<string> All { get; } = new[] { Src, GscRobo, GscExt, Robodun };

        /// <summary>既定で動くグループ</summary>
        public static IReadOnlyList<string> Default { get; } = new[] { Src };

        public static string Describe(string group)
        {
            return group switch
            {
                Src => "SRC の仕様と食い違うデータ（既定）",
                GscRobo => "GSC データパックのロボ基準の書き方の決まり",
                GscExt => "GSC データパックの拡張基準の書き方の決まり",
                Robodun => "ロボダン（ロボッとダンジョンＢ！）の UnitChecker.eve の観点（ロボダンの基準）",
                _ => "",
            };
        }

        /// <summary>
        /// 「src,gsc-ext」のような指定を読む。none なら空。知らないグループは ArgumentException
        /// </summary>
        public static IReadOnlyList<string> Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Default;
            }
            var groups = value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToLowerInvariant())
                .ToList();
            if (groups.Contains("none"))
            {
                return Array.Empty<string>();
            }
            var unknown = groups.Where(x => !All.Contains(x)).ToList();
            if (unknown.Any())
            {
                throw new ArgumentException($"知らない規則のグループ: {string.Join(", ", unknown)}（{string.Join(", ", All)}・none）");
            }
            return groups.Distinct().ToList();
        }
    }

    /// <summary>規則が調べるデータ。検査の対象のファイルで定義されたものだけを入れる（先に読む汎用データは入れない）</summary>
    public sealed class DataRuleContext
    {
        public SRCCore.SRC Src { get; set; }
        public IReadOnlyList<UnitData> Units { get; set; } = Array.Empty<UnitData>();
        public IReadOnlyList<PilotData> Pilots { get; set; } = Array.Empty<PilotData>();
        public IReadOnlyList<MessageData> Messages { get; set; } = Array.Empty<MessageData>();
        /// <summary>DataLocation.FileName（読み込みに渡した名前）から、診断に書くパス</summary>
        public Func<string, string> ResolveFile { get; set; } = x => x;
    }

    /// <summary>データの規則。読み込めたデータを調べ、診断を返す</summary>
    public interface IDataRule
    {
        /// <summary>小文字とハイフン。作品の観点の規則はグループ名で始める（robodun-…）</summary>
        string Id { get; }
        IReadOnlyList<string> Groups { get; }
        DiagnosticSeverity Severity { get; }
        string Description { get; }
        /// <summary>出どころ（根拠の文書・実装、作品のチェッカーとその版）</summary>
        string Origin { get; }
        IEnumerable<SrcDiagnostic> Check(DataRuleContext context);
    }

    public abstract class DataRule : IDataRule
    {
        public abstract string Id { get; }
        public abstract IReadOnlyList<string> Groups { get; }
        public virtual DiagnosticSeverity Severity => DiagnosticSeverity.Information;
        public abstract string Description { get; }
        public abstract string Origin { get; }
        public abstract IEnumerable<SrcDiagnostic> Check(DataRuleContext context);

        protected SrcDiagnostic Report(DataRuleContext context, DataLocation location, int line, string message, string source)
        {
            return new SrcDiagnostic
            {
                File = context.ResolveFile(location?.FileName ?? ""),
                Line = Math.Max(1, line),
                Severity = Severity,
                Message = message,
                Source = source,
                RuleId = Id,
            };
        }
    }

    /// <summary>組み込みの規則の一覧</summary>
    public static class DataRuleCatalog
    {
        private static readonly IDataRule[] rules = new IDataRule[]
        {
        };

        public static IReadOnlyList<IDataRule> All => rules;

        /// <summary>選んだグループのどれかに入る規則（1 つの規則は 1 回だけ）</summary>
        public static IReadOnlyList<IDataRule> Select(IEnumerable<string> groups, IEnumerable<IDataRule> from = null)
        {
            var set = new HashSet<string>(groups ?? RuleGroups.Default);
            return (from ?? rules).Where(x => x.Groups.Any(set.Contains)).ToList();
        }

        public static IReadOnlyList<SrcDiagnostic> Run(IEnumerable<IDataRule> selected, DataRuleContext context)
        {
            var result = new List<SrcDiagnostic>();
            foreach (var rule in selected)
            {
                try
                {
                    result.AddRange(rule.Check(context));
                }
                catch (Exception ex)
                {
                    result.Add(new SrcDiagnostic
                    {
                        File = "",
                        Line = 1,
                        Severity = DiagnosticSeverity.Information,
                        Message = $"規則 {rule.Id} を動かせませんでした: {ex.Message}",
                        RuleId = rule.Id,
                    });
                }
            }
            return result;
        }
    }
}
