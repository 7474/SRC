using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SRCAuthoring.Analysis
{
    /// <summary>
    /// イベント（*.eve）とデータ（robot.txt・pilot.txt など）に書かれた画像・音・マップのファイル名が、手元のファイルにあるかを確かめる。
    /// 素材を同梱せずに公開するシナリオは多いので、見つからないものは情報（Information）として出す。
    /// 式（$(...) など）で組み立てるファイル名と、ダミーのファイル名（-.bmp など）は確かめない。
    /// </summary>
    public sealed class AssetReferenceChecker
    {
        static AssetReferenceChecker()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private static readonly string[] Extensions = { ".bmp", ".png", ".jpg", ".jpeg", ".gif", ".mid", ".midi", ".mp3", ".wav", ".ogg", ".map" };

        // 素材を参照するデータのファイル（ほかの *.txt は説明書などのことがあるので見ない）
        private static readonly Regex DataFile = new Regex(@"^(robot|unit|pilot|non_pilot|animation|effect|item|terrain)\.txt$", RegexOptions.IgnoreCase);

        // 引数の区切り（空白・カンマ）と、戦闘アニメ・特殊効果のデータの ; （「1;Shock(low).wav;1;...」のように続ける）
        private static readonly Regex Token = new Regex(@"""(?<t>[^""]*)""|(?<t>[^\s,;""]+)");

        // 手元のファイルのパス（小文字・/ 区切り）の、フォルダの切れ目から後ろの部分すべて
        private readonly HashSet<string> known = new HashSet<string>(StringComparer.Ordinal);

        /// <summary>
        /// 手元にあるファイル（シナリオの zip と汎用データの zip の中のパスなど）を加える。
        /// SRC は Bitmap\Unit などのフォルダとシナリオ・ExtData・本体のフォルダを順に探すので、パスの末尾が一致すればあるものとする
        /// </summary>
        public void AddKnownFiles(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                var name = Normalize(path);
                for (var i = 0; i < name.Length; i = name.IndexOf('/', i) + 1)
                {
                    known.Add(name.Substring(i));
                    if (name.IndexOf('/', i) < 0)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>確かめる対象のファイルか（*.eve と、素材を参照するデータ）</summary>
        public static bool IsTarget(string path)
        {
            var name = Path.GetFileName(path);
            return name.EndsWith(".eve", StringComparison.OrdinalIgnoreCase) || DataFile.IsMatch(name);
        }

        /// <summary>ファイルを読み、見つからない素材をファイルごと・名前ごとに 1 件ずつ返す</summary>
        public IEnumerable<SrcDiagnostic> Check(IEnumerable<string> files)
        {
            foreach (var file in files.Where(IsTarget))
            {
                var reported = new HashSet<string>(StringComparer.Ordinal);
                var lines = ReadText(file).Split('\n');
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (line.StartsWith("#") || line.StartsWith("//"))
                    {
                        continue;
                    }
                    foreach (Match m in Token.Matches(line))
                    {
                        var name = AssetName(m.Groups["t"].Value);
                        if (name != null && !Exists(name) && reported.Add(name))
                        {
                            yield return new SrcDiagnostic
                            {
                                File = file,
                                Line = i + 1,
                                Severity = DiagnosticSeverity.Information,
                                Message = "素材が見つかりません: " + name,
                                Source = name,
                            };
                        }
                    }
                }
            }
        }

        /// <summary>素材のファイル名なら（/ 区切りにして）返す。式やダミーのファイル名は null</summary>
        public static string AssetName(string token)
        {
            var name = token.Trim();
            // データの特殊能力は「ＢＧＭ=曲.mid」のように 名前=値 と書くので、値だけを見る
            var eq = name.IndexOf('=');
            if (eq >= 0)
            {
                name = name.Substring(eq + 1).Trim();
            }
            // SearchMidiFile と同じく、全体を囲む () は外す
            if (name.StartsWith("(") && name.EndsWith(")"))
            {
                name = name.Substring(1, name.Length - 2);
            }
            if (name.Length == 0 || name.IndexOfAny(new[] { '$', '%', '(', '*', '?', '[' }) == 0 || name.Contains("$(") || name.Contains("*") || name.Contains("?"))
            {
                return null;
            }
            name = name.Replace('\\', '/');
            if (!Extensions.Any(x => name.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }
            var stem = Path.GetFileNameWithoutExtension(name);
            if (stem.Length == 0 || stem == "-")
            {
                return null;
            }
            return name;
        }

        private bool Exists(string name)
        {
            var key = Normalize(name);
            if (known.Contains(key))
            {
                return true;
            }
            // MIDI は同じ名前の MP3 があればそれを使う（Sound.SearchMidiFile）
            return key.EndsWith(".mid") && known.Contains(key.Substring(0, key.Length - 4) + ".mp3");
        }

        private static string Normalize(string path)
        {
            return string.Join("/", path.Replace('\\', '/').Split('/').Where(x => x.Length > 0 && x != ".")).ToLowerInvariant();
        }

        // UTF-8 として正しく読めれば UTF-8、だめなら Shift_JIS（SRC のデータの文字コード）
        private static string ReadText(string path)
        {
            var bytes = File.ReadAllBytes(path);
            try
            {
                return new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                return Encoding.GetEncoding(932).GetString(bytes);
            }
        }
    }
}
