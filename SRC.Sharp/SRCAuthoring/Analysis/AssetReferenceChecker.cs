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
                foreach (var token in Scan(ReadText(file)))
                {
                    if (token.Name != null && !Exists(token.Name) && reported.Add(token.Name))
                    {
                        yield return new SrcDiagnostic
                        {
                            File = file,
                            Line = token.Line,
                            Severity = DiagnosticSeverity.Information,
                            Message = "素材が見つかりません: " + token.Name,
                            Source = token.Name,
                        };
                    }
                }
            }
        }

        /// <summary>
        /// テキスト（イベントかデータ）の中の素材の参照を、行ごとに返す（7474/SRC#1654 の素材の一覧と共通）。
        /// 確かめられる名前は Name に、式で組み立てる名前（$(...)・"..." の連結など）は Name を null・Dynamic を true にする
        /// </summary>
        public static IEnumerable<AssetToken> Scan(string text)
        {
            var lines = (text ?? "").Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("#") || line.StartsWith("//"))
                {
                    continue;
                }
                var dynamicReported = false;
                foreach (Match m in Token.Matches(line))
                {
                    var raw = m.Groups["t"].Value;
                    var name = AssetName(raw);
                    if (name != null)
                    {
                        yield return new AssetToken(i + 1, name, false, raw);
                    }
                    else if (!dynamicReported && IsDynamicAssetToken(raw))
                    {
                        // 1 行に式がいくつあっても 1 件にする（行を見れば分かる）
                        dynamicReported = true;
                        yield return new AssetToken(i + 1, null, true, line);
                    }
                }
            }
        }

        /// <summary>素材の拡張子を持つが、式で組み立てるため名前を確かめられない字句か（ダミーの -.bmp などは除く）</summary>
        public static bool IsDynamicAssetToken(string token)
        {
            var name = token.Trim();
            var eq = name.IndexOf('=');
            if (eq >= 0)
            {
                name = name.Substring(eq + 1).Trim();
            }
            var lower = name.ToLowerInvariant();
            var ext = Extensions.FirstOrDefault(x => lower.Contains(x));
            if (ext == null)
            {
                return false;
            }
            var stem = name.Substring(0, lower.IndexOf(ext, StringComparison.Ordinal)).Trim('(', ')');
            if (stem == "-")
            {
                return false;
            }
            return stem.Length == 0 || name.IndexOfAny(new[] { '$', '%', '(', ')', '[', '*', '?' }) >= 0;
        }

        /// <summary>素材の拡張子か</summary>
        public static bool HasAssetExtension(string path)
        {
            return Extensions.Any(x => path.EndsWith(x, StringComparison.OrdinalIgnoreCase));
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
        internal static string ReadText(string path)
        {
            return DecodeText(File.ReadAllBytes(path));
        }

        /// <summary>バイト列を文字列にする（UTF-8 として正しく読めれば UTF-8、だめなら Shift_JIS）</summary>
        public static string DecodeText(byte[] bytes)
        {
            try
            {
                return new UTF8Encoding(false, true).GetString(bytes).TrimStart('\uFEFF');
            }
            catch (DecoderFallbackException)
            {
                return Encoding.GetEncoding(932).GetString(bytes);
            }
        }
    }

    /// <summary>テキストの中の素材の参照 1 件。Name が null なら式で組み立てる名前（Text はその行）</summary>
    public sealed class AssetToken
    {
        public AssetToken(int line, string name, bool dynamic, string text)
        {
            Line = line;
            Name = name;
            Dynamic = dynamic;
            Text = text;
        }

        public int Line { get; }
        public string Name { get; }
        public bool Dynamic { get; }
        public string Text { get; }
    }
}
