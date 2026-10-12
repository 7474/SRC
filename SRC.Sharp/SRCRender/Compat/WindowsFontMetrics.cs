using System;
using System.Collections.Generic;

namespace SRCRender.Compat
{
    /// <summary>
    /// Windows のフォントの縦の寸法（em に対する比）。元の SRC（VB 版、GDI）は文字の上端を指定位置に置き、
    /// ベースラインを ascent（OS/2 の usWinAscent）だけ下げて描く。行の高さは ascent + descent。
    /// 代わりのフォント（Noto CJK など）で描く場合も、この寸法で配置して表示位置を合わせる。
    /// </summary>
    public static class WindowsFontMetrics
    {
        public readonly struct Metrics
        {
            public Metrics(float ascent, float descent, bool verified)
            {
                Ascent = ascent;
                Descent = descent;
                Verified = verified;
            }

            /// <summary>ascent（em に対する比）</summary>
            public float Ascent { get; }

            /// <summary>descent（em に対する比、正の値）</summary>
            public float Descent { get; }

            /// <summary>Windows の実フォントで確かめた値か</summary>
            public bool Verified { get; }

            public float LineHeight => Ascent + Descent;
        }

        // 値は usWinAscent / unitsPerEm と usWinDescent / unitsPerEm。
        // Verified = false は公開情報からの暫定値。Windows では SRCRenderTests の WindowsFontMetricsTests が
        // 実フォント（GDI+ の FontFamily.GetCellAscent など）と照合し、測った値を TestResults/fontmetrics.txt に書き出す。
        private static readonly Dictionary<string, Metrics> table = new Dictionary<string, Metrics>(StringComparer.OrdinalIgnoreCase)
        {
            // ＭＳ ゴシック・ＭＳ 明朝系（unitsPerEm 256、winAscent 220、winDescent 36）
            ["MS Gothic"] = new Metrics(220f / 256, 36f / 256, false),
            ["MS PGothic"] = new Metrics(220f / 256, 36f / 256, false),
            ["MS UI Gothic"] = new Metrics(220f / 256, 36f / 256, false),
            ["MS Mincho"] = new Metrics(220f / 256, 36f / 256, false),
            ["MS PMincho"] = new Metrics(220f / 256, 36f / 256, false),
            // メイリオ（unitsPerEm 2048、winAscent 2171、winDescent 901）
            ["Meiryo"] = new Metrics(2171f / 2048, 901f / 2048, false),
            ["Meiryo UI"] = new Metrics(2171f / 2048, 901f / 2048, false),
            // 游ゴシック（unitsPerEm 2048、winAscent 2228、winDescent 477）
            ["Yu Gothic"] = new Metrics(2228f / 2048, 477f / 2048, false),
            ["Yu Gothic UI"] = new Metrics(2228f / 2048, 477f / 2048, false),
        };

        // 日本語名と英語名の対応
        private static readonly Dictionary<string, string> aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MS ゴシック"] = "MS Gothic",
            ["MS Pゴシック"] = "MS PGothic",
            ["MS 明朝"] = "MS Mincho",
            ["MS P明朝"] = "MS PMincho",
            ["メイリオ"] = "Meiryo",
            ["游ゴシック"] = "Yu Gothic",
            // SRC の Font コマンドでよく使われる省略形
            ["ゴシック"] = "MS Gothic",
            ["Pゴシック"] = "MS PGothic",
            ["明朝"] = "MS Mincho",
            ["P明朝"] = "MS PMincho",
        };

        /// <summary>表にあるフォント名（英語名）</summary>
        public static IEnumerable<string> Names => table.Keys;

        /// <summary>全角の ＭＳ・Ｐ・空白などを半角にそろえる。</summary>
        public static string Normalize(string name)
        {
            var n = (name ?? "").Trim()
                .Replace("ＭＳ", "MS").Replace("ｍｓ", "MS").Replace("Ｐ", "P").Replace("ｐ", "P").Replace("　", " ")
                .Replace("ＵＩ", "UI");
            while (n.Contains("  "))
            {
                n = n.Replace("  ", " ");
            }
            return aliases.TryGetValue(n, out var english) ? english : n;
        }

        public static bool TryGet(string name, out Metrics metrics)
        {
            return table.TryGetValue(Normalize(name), out metrics);
        }
    }
}
