using SRCCore.Lib;
using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCAuthoring.Analysis
{
    /// <summary>イベントのコマンドが名前で指すデータの種類</summary>
    public enum DataKind
    {
        Unit,
        Pilot,
        Item,
    }

    /// <summary>イベントの行にある、データの名前の参照（7474/SRC#1642）</summary>
    public sealed class DataReference
    {
        public string File { get; set; }
        /// <summary>1 始まりの行</summary>
        public int Line { get; set; }
        /// <summary>行の中の位置（0 始まり）と長さ</summary>
        public int Column { get; set; }
        public int Length { get; set; }
        public DataKind Kind { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
    }

    /// <summary>データの定義（名前・愛称・場所・ホバーに出す要約）</summary>
    public sealed class DataEntry
    {
        public DataKind Kind { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        /// <summary>定義のファイル（無ければ null）と、名前の行（1 始まり）</summary>
        public string File { get; set; }
        public int Line { get; set; }
        /// <summary>ホバーに出す要約（Markdown）</summary>
        public string Summary { get; set; }
    }

    /// <summary>
    /// シナリオのデータ（ユニット・パイロット・アイテム）の名前の一覧。SRCCore の読み込み処理で読んだ一覧から作る。
    /// イベントの参照の検査・補完・ホバー・定義へ移動に使う
    /// </summary>
    public sealed class DataCatalog
    {
        private readonly Dictionary<DataKind, Dictionary<string, DataEntry>> byName = new Dictionary<DataKind, Dictionary<string, DataEntry>>();
        private readonly List<DataEntry> entries = new List<DataEntry>();

        public IReadOnlyList<DataEntry> Entries => entries;

        public static readonly DataCatalog Empty = new DataCatalog();

        /// <summary>その種類のデータが 1 つでもあるか（無ければ、参照の検査をしない。ほかの場所のデータを使うシナリオがあるため）</summary>
        public bool Has(DataKind kind) => byName.TryGetValue(kind, out var d) && d.Count > 0;

        /// <summary>名前か愛称で引く</summary>
        public DataEntry Find(DataKind kind, string name) =>
            byName.TryGetValue(kind, out var d) && d.TryGetValue(name, out var e) ? e : null;

        /// <summary>どの種類でもよいので名前か愛称で引く（ホバー・定義へ移動）</summary>
        public DataEntry FindAny(string name) =>
            Find(DataKind.Unit, name) ?? Find(DataKind.Pilot, name) ?? Find(DataKind.Item, name);

        private void Add(DataEntry e)
        {
            entries.Add(e);
            if (!byName.TryGetValue(e.Kind, out var d))
            {
                d = new Dictionary<string, DataEntry>(StringComparer.Ordinal);
                byName[e.Kind] = d;
            }
            d.TryAdd(e.Name, e);
            if (!string.IsNullOrEmpty(e.Nickname))
            {
                d.TryAdd(e.Nickname, e);
            }
        }

        /// <summary>読み込んだ SRC の一覧から作る。files は、場所の無いデータ（アイテム）の行を探すためのデータのファイル</summary>
        public static DataCatalog FromSrc(SRCCore.SRC src, IEnumerable<string> files = null)
        {
            var catalog = new DataCatalog();
            // 場所の無いもの（SRCCore が内部で作る仮のユニット・パイロット）は、シナリオのデータに数えない
            foreach (var u in src.UDList.Items.Where(x => x.Location != null))
            {
                catalog.Add(new DataEntry
                {
                    Kind = DataKind.Unit,
                    Name = u.Name,
                    Nickname = u.Nickname,
                    File = u.Location?.FileName,
                    Line = u.Location?.Line ?? 0,
                    Summary = string.Format(CultureInfo.InvariantCulture, "ユニット **{0}**（{1}）  \nHP {2}・EN {3}・装甲 {4}・運動性 {5}・移動力 {6}・サイズ {7}",
                        u.Name, u.Class, u.HP, u.EN, u.Armor, u.Mobility, u.Speed, u.Size),
                });
            }
            foreach (var p in src.PDList.Items.Where(x => x.Location != null))
            {
                catalog.Add(new DataEntry
                {
                    Kind = DataKind.Pilot,
                    Name = p.Name,
                    Nickname = p.Nickname,
                    File = p.Location?.FileName,
                    Line = p.Location?.Line ?? 0,
                    Summary = string.Format(CultureInfo.InvariantCulture, "パイロット **{0}**（{1}）  \n格闘 {2}・射撃 {3}・命中 {4}・回避 {5}・技量 {6}・反応 {7}・SP {8}",
                        p.Name, p.Class, p.Infight, p.Shooting, p.Hit, p.Dodge, p.Technique, p.Intuition, p.SP),
                });
            }
            var itemFiles = (files ?? Enumerable.Empty<string>()).Where(f => Path.GetFileName(f).Equals("item.txt", StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var i in src.IDList.Items)
            {
                var (file, line) = FindLine(itemFiles, i.Name);
                catalog.Add(new DataEntry
                {
                    Kind = DataKind.Item,
                    Name = i.Name,
                    Nickname = i.Nickname,
                    File = file,
                    Line = line,
                    Summary = $"アイテム **{i.Name}**（{i.Class}）",
                });
            }
            return catalog;
        }

        /// <summary>名前だけの行を探す（アイテムは読み込みで場所を覚えないため）</summary>
        private static (string file, int line) FindLine(IEnumerable<string> files, string name)
        {
            foreach (var f in files)
            {
                try
                {
                    var lines = File.ReadAllLines(f);
                    for (var n = 0; n < lines.Length; n++)
                    {
                        if (lines[n].Trim() == name)
                        {
                            return (f, n + 1);
                        }
                    }
                }
                catch (IOException)
                {
                }
            }
            return (null, 0);
        }
    }

    /// <summary>
    /// イベントのコマンドの引数に書いたデータの名前（ユニット・パイロット・アイテム）を見つけ、データにあるかを調べる（7474/SRC#1642）。
    /// 名前を取る引数がはっきりしたコマンドだけを見る。式（$ を含む）・変数（Set・Local・Global・Incr で値を入れた名前）は見ない
    /// </summary>
    public static class DataReferenceChecker
    {
        /// <summary>コマンドと、名前を取る引数（コマンドの次を 1 とする）</summary>
        private static readonly Dictionary<string, (int arg, DataKind kind)[]> Commands = new Dictionary<string, (int, DataKind)[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Create"] = new[] { (2, DataKind.Unit), (4, DataKind.Pilot) },
            ["Unit"] = new[] { (1, DataKind.Unit) },
            ["Pilot"] = new[] { (1, DataKind.Pilot) },
            ["Item"] = new[] { (1, DataKind.Item) },
        };

        private static readonly string[] VariableCommands = { "Set", "Local", "Global", "Incr" };

        /// <summary>イベントのテキストから参照を探す</summary>
        public static IReadOnlyList<DataReference> Find(string file, string text)
        {
            var lines = (text ?? "").Replace("\r\n", "\n").Split('\n');
            var variables = new HashSet<string>(StringComparer.Ordinal);
            var found = new List<(int line, string raw, string[] args)>();
            for (var n = 0; n < lines.Length; n++)
            {
                var line = lines[n].Trim();
                if (line == "" || line.StartsWith("#", StringComparison.Ordinal) || line.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }
                if (GeneralLib.ListSplit(line, out var args) < 1)
                {
                    continue;
                }
                if (VariableCommands.Any(c => c.Equals(args[0], StringComparison.OrdinalIgnoreCase)) && args.Length > 1)
                {
                    variables.Add(args[1]);
                }
                if (Commands.ContainsKey(args[0]))
                {
                    found.Add((n, lines[n], args));
                }
            }
            var result = new List<DataReference>();
            foreach (var (n, raw, args) in found)
            {
                var search = 0;
                foreach (var (arg, kind) in Commands[args[0]])
                {
                    if (arg >= args.Length)
                    {
                        continue;
                    }
                    var token = args[arg];
                    var column = raw.IndexOf(token, search, StringComparison.Ordinal);
                    if (column >= 0)
                    {
                        search = column + token.Length;
                    }
                    var name = token.Length >= 2 && token[0] == '"' && token[^1] == '"' ? token.Substring(1, token.Length - 2) : token;
                    if (name == "" || name.Contains('$') || variables.Contains(name))
                    {
                        continue;
                    }
                    result.Add(new DataReference
                    {
                        File = file,
                        Line = n + 1,
                        Column = Math.Max(0, column),
                        Length = token.Length,
                        Kind = kind,
                        Name = name,
                        Command = args[0],
                    });
                }
            }
            return result;
        }

        /// <summary>データに無い参照を診断にする（その種類のデータが 1 つも無ければ調べない）</summary>
        public static IEnumerable<SrcDiagnostic> Check(IEnumerable<DataReference> references, DataCatalog catalog)
        {
            foreach (var r in references)
            {
                if (!catalog.Has(r.Kind) || catalog.Find(r.Kind, r.Name) != null)
                {
                    continue;
                }
                yield return new SrcDiagnostic
                {
                    File = r.File,
                    Line = r.Line,
                    Severity = DiagnosticSeverity.Warning,
                    Message = $"{KindName(r.Kind)}「{r.Name}」がシナリオのデータにありません（{r.Command}）",
                };
            }
        }

        public static string KindName(DataKind kind) => kind switch
        {
            DataKind.Unit => "ユニット",
            DataKind.Pilot => "パイロット",
            _ => "アイテム",
        };
    }

    /// <summary>イベントのファイルを読む（UTF-8 として読めなければ Shift_JIS）</summary>
    public static class DataReferenceText
    {
        public static string Read(string file)
        {
            var bytes = File.ReadAllBytes(file);
            try
            {
                return new UTF8Encoding(false, true).GetString(bytes).TrimStart('\uFEFF');
            }
            catch (DecoderFallbackException)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                return Encoding.GetEncoding(932).GetString(bytes);
            }
        }
    }
}
