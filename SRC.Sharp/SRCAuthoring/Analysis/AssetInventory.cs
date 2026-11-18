using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SRCAuthoring.Analysis
{
    /// <summary>素材を探す場所の 1 つ（シナリオ・拡張データ・SRC 本体）と、その中のファイル（相対パス）</summary>
    public sealed class AssetRoot
    {
        public AssetRoot(string kind, string name, IEnumerable<string> files)
        {
            Kind = kind;
            Name = name;
            Files = files.Select(x => x.Replace('\\', '/')).ToList();
        }

        /// <summary>scenario・extdata・extdata2・app</summary>
        public string Kind { get; }
        /// <summary>表示する名前（フォルダのパスなど）</summary>
        public string Name { get; }
        public IReadOnlyList<string> Files { get; }

        /// <summary>フォルダの中のファイルを（.git などを除いて）集める</summary>
        public static AssetRoot FromDirectory(string kind, string directory)
        {
            var full = Path.GetFullPath(directory);
            var files = Directory.Exists(full)
                ? Directory.EnumerateFiles(full, "*", SearchOption.AllDirectories)
                    .Select(x => Path.GetRelativePath(full, x).Replace('\\', '/'))
                    .Where(x => !x.Split('/').Any(p => p.StartsWith(".") || p == "node_modules"))
                    .OrderBy(x => x, StringComparer.Ordinal)
                : Enumerable.Empty<string>();
            return new AssetRoot(kind, full, files);
        }
    }

    /// <summary>素材を参照している場所（ファイルと行）</summary>
    public sealed class AssetUsage
    {
        public string File { get; set; }
        public int Line { get; set; }
    }

    /// <summary>名前で参照している素材 1 つ</summary>
    public sealed class AssetEntry
    {
        public string Name { get; set; }
        /// <summary>image・sound・map</summary>
        public string Kind { get; set; }
        /// <summary>見つかった場所の種類（scenario・extdata・extdata2・app）。見つからなければ null</summary>
        public string FoundIn { get; set; }
        /// <summary>見つかったファイル（その場所からの相対パス）</summary>
        public string FoundPath { get; set; }
        public bool Missing => FoundIn == null;
        public List<AssetUsage> Usages { get; } = new List<AssetUsage>();
    }

    /// <summary>式で組み立てるため確かめられない参照</summary>
    public sealed class AssetDynamicReference
    {
        public string File { get; set; }
        public int Line { get; set; }
        public string Text { get; set; }
    }

    /// <summary>
    /// シナリオに必要な素材が揃っているかの一覧（7474/SRC#1654）。
    /// データとイベントが参照する画像・音・マップ（AssetReferenceChecker と同じ字句の読み方）を、参照している場所と一緒に並べ、
    /// SRC と同じ順（シナリオ → 拡張データ → SRC 本体）で探した結果を出す。素材の入手・再配布は扱わない
    /// </summary>
    public sealed class AssetReport
    {
        public string ScenarioRoot { get; set; }
        public List<AssetRoot> Roots { get; } = new List<AssetRoot>();
        public List<AssetEntry> Assets { get; } = new List<AssetEntry>();
        public List<AssetDynamicReference> Dynamic { get; } = new List<AssetDynamicReference>();
        /// <summary>シナリオのフォルダにあって、どこからも名前で参照していない素材（消すかは作者が決める）</summary>
        public List<string> Unused { get; } = new List<string>();
        /// <summary>調べたファイル（シナリオのフォルダからの相対パス）</summary>
        public List<string> CheckedFiles { get; } = new List<string>();

        public int MissingCount => Assets.Count(x => x.Missing);

        /// <summary>
        /// シナリオのフォルダと探す場所から一覧を作る。targets は調べるテキスト（シナリオのフォルダからの相対パスと中身）。
        /// roots は探す順（シナリオを先頭に）
        /// </summary>
        public static AssetReport Build(IReadOnlyList<AssetRoot> roots, IEnumerable<(string path, string text)> targets, string scenarioRoot = null)
        {
            var report = new AssetReport { ScenarioRoot = scenarioRoot };
            report.Roots.AddRange(roots);
            var indexes = roots.Select(r => new SuffixIndex(r.Files)).ToList();
            var byName = new Dictionary<string, AssetEntry>(StringComparer.OrdinalIgnoreCase);

            foreach (var (path, text) in targets.OrderBy(x => x.path, StringComparer.Ordinal))
            {
                var rel = path.Replace('\\', '/');
                report.CheckedFiles.Add(rel);
                foreach (var token in AssetReferenceChecker.Scan(text))
                {
                    if (token.Dynamic)
                    {
                        report.Dynamic.Add(new AssetDynamicReference { File = rel, Line = token.Line, Text = token.Text });
                        continue;
                    }
                    if (!byName.TryGetValue(token.Name, out var entry))
                    {
                        entry = new AssetEntry { Name = token.Name, Kind = KindOf(token.Name) };
                        for (var i = 0; i < roots.Count && entry.FoundIn == null; i++)
                        {
                            var found = indexes[i].Find(token.Name);
                            // MIDI は同じ名前の MP3 があればそれを使う（Sound.SearchMidiFile）
                            if (found == null && token.Name.EndsWith(".mid", StringComparison.OrdinalIgnoreCase))
                            {
                                found = indexes[i].Find(token.Name.Substring(0, token.Name.Length - 4) + ".mp3");
                            }
                            if (found != null)
                            {
                                entry.FoundIn = roots[i].Kind;
                                entry.FoundPath = found;
                            }
                        }
                        byName[token.Name] = entry;
                        report.Assets.Add(entry);
                    }
                    if (!entry.Usages.Any(u => u.File == rel && u.Line == token.Line))
                    {
                        entry.Usages.Add(new AssetUsage { File = rel, Line = token.Line });
                    }
                }
            }
            report.Assets.Sort((a, b) => a.Missing != b.Missing ? (a.Missing ? -1 : 1) : string.CompareOrdinal(a.Name, b.Name));

            var scenario = roots.FirstOrDefault(r => r.Kind == "scenario");
            if (scenario != null)
            {
                report.Unused.AddRange(FindUnused(scenario.Files, report.Assets));
            }
            return report;
        }

        /// <summary>シナリオのフォルダ（と拡張データ・SRC 本体のフォルダ）から一覧を作る</summary>
        public static AssetReport FromDirectories(string scenarioRoot, string appPath = null, string extDataPath = null, string extDataPath2 = null)
        {
            var root = Path.GetFullPath(scenarioRoot);
            var roots = new List<AssetRoot> { AssetRoot.FromDirectory("scenario", root) };
            foreach (var (kind, dir) in new[] { ("extdata", extDataPath), ("extdata2", extDataPath2), ("app", appPath) })
            {
                if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir) && !SamePath(dir, root))
                {
                    roots.Add(AssetRoot.FromDirectory(kind, dir));
                }
            }
            var targets = roots[0].Files
                .Where(AssetReferenceChecker.IsTarget)
                .Select(f => (f, AssetReferenceChecker.ReadText(Path.Combine(root, f))));
            return Build(roots, targets, root);
        }

        private static bool SamePath(string a, string b)
        {
            return string.Equals(Path.GetFullPath(a).TrimEnd('/', '\\'), Path.GetFullPath(b).TrimEnd('/', '\\'), StringComparison.OrdinalIgnoreCase);
        }

        public static string KindOf(string name)
        {
            var ext = Path.GetExtension(name).ToLowerInvariant();
            switch (ext)
            {
                case ".map":
                    return "map";
                case ".mid":
                case ".midi":
                case ".mp3":
                case ".wav":
                case ".ogg":
                    return "sound";
                default:
                    return "image";
            }
        }

        /// <summary>
        /// 参照していない素材。名前の末尾が一致する参照があれば使っているものとする。
        /// ほかに、イベントと同じ名前のマップ（ステージのマップ）・MIDI の代わりの MP3・表情などの差分（名前(…).bmp）・
        /// 地形の画像（Bitmap/Map。地形データとマップが番号で指す）は使っているものとする
        /// </summary>
        private static IEnumerable<string> FindUnused(IReadOnlyList<string> files, IReadOnlyList<AssetEntry> assets)
        {
            var referenced = new HashSet<string>(StringComparer.Ordinal);
            foreach (var a in assets)
            {
                var key = Normalize(a.Name);
                referenced.Add(key);
                if (key.EndsWith(".mid"))
                {
                    referenced.Add(key.Substring(0, key.Length - 4) + ".mp3");
                }
            }
            var events = new HashSet<string>(files.Where(f => f.EndsWith(".eve", StringComparison.OrdinalIgnoreCase))
                .Select(f => Normalize(f.Substring(0, f.Length - 4))), StringComparer.Ordinal);

            foreach (var file in files.Where(AssetReferenceChecker.HasAssetExtension))
            {
                var key = Normalize(file);
                if (key.StartsWith("bitmap/map/") || key.Contains("/bitmap/map/"))
                {
                    continue;
                }
                if (key.EndsWith(".map") && events.Contains(key.Substring(0, key.Length - 4)))
                {
                    continue;
                }
                if (Suffixes(key).Any(referenced.Contains))
                {
                    continue;
                }
                // 表情などの差分: 名前(…).bmp は 名前.bmp を参照していれば使っているものとする
                var paren = key.LastIndexOf('(');
                if (paren > 0 && key.EndsWith(")" + Path.GetExtension(key)))
                {
                    var baseName = key.Substring(0, paren) + Path.GetExtension(key);
                    if (Suffixes(baseName).Any(referenced.Contains))
                    {
                        continue;
                    }
                }
                yield return file;
            }
        }

        private static IEnumerable<string> Suffixes(string key)
        {
            for (var i = 0; i < key.Length; i = key.IndexOf('/', i) + 1)
            {
                yield return key.Substring(i);
                if (key.IndexOf('/', i) < 0)
                {
                    yield break;
                }
            }
        }

        internal static string Normalize(string path)
        {
            return string.Join("/", path.Replace('\\', '/').Split('/').Where(x => x.Length > 0 && x != ".")).ToLowerInvariant();
        }

        /// <summary>見つかった場所の表示名</summary>
        public static string RootLabel(string kind)
        {
            switch (kind)
            {
                case "scenario":
                    return "シナリオ";
                case "extdata":
                    return "拡張データ";
                case "extdata2":
                    return "拡張データ2";
                case "app":
                    return "SRC 本体";
                default:
                    return "見つからない";
            }
        }

        private static string KindLabel(string kind)
        {
            switch (kind)
            {
                case "map":
                    return "マップ";
                case "sound":
                    return "音";
                default:
                    return "画像";
            }
        }

        private static string MdCell(string s) => (s ?? "").Replace("|", "\\|").Replace("\r", "").Replace("\n", " ");

        private static string Location(AssetUsage u) => $"{u.File}:{u.Line}";

        public string ToMarkdown()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# 素材の一覧");
            sb.AppendLine();
            sb.AppendLine($"- 参照している素材: {Assets.Count}（見つからない {MissingCount}）");
            sb.AppendLine($"- 確かめられない参照（式で組み立てる名前）: {Dynamic.Count}");
            sb.AppendLine($"- 参照していない素材（シナリオのフォルダ）: {Unused.Count}");
            sb.AppendLine($"- 探した順: {string.Join(" → ", Roots.Select(r => RootLabel(r.Kind)))}");
            sb.AppendLine();
            if (Assets.Count > 0)
            {
                sb.AppendLine("## 参照している素材");
                sb.AppendLine();
                sb.AppendLine("| 素材 | 種類 | 見つかった場所 | 参照している場所 |");
                sb.AppendLine("|---|---|---|---|");
                foreach (var a in Assets)
                {
                    var where = a.Missing ? "**見つからない**" : $"{RootLabel(a.FoundIn)}: `{MdCell(a.FoundPath)}`";
                    var usages = string.Join("<br>", a.Usages.Take(5).Select(u => $"`{MdCell(Location(u))}`"));
                    if (a.Usages.Count > 5)
                    {
                        usages += $"<br>ほか {a.Usages.Count - 5} か所";
                    }
                    sb.AppendLine($"| `{MdCell(a.Name)}` | {KindLabel(a.Kind)} | {where} | {usages} |");
                }
                sb.AppendLine();
            }
            if (Dynamic.Count > 0)
            {
                sb.AppendLine("## 確かめられない参照");
                sb.AppendLine();
                sb.AppendLine("式で名前を組み立てているため、どのファイルを使うかはプレイしないと分かりません。");
                sb.AppendLine();
                foreach (var d in Dynamic)
                {
                    sb.AppendLine($"- `{MdCell(d.File)}:{d.Line}`: `{MdCell(d.Text)}`");
                }
                sb.AppendLine();
            }
            if (Unused.Count > 0)
            {
                sb.AppendLine("## 参照していない素材");
                sb.AppendLine();
                sb.AppendLine("シナリオのフォルダにあって、どこからも名前で参照していないファイルです。式で組み立てる名前で使っていることもあるので、消すかは確かめてから決めてください。");
                sb.AppendLine();
                foreach (var u in Unused)
                {
                    sb.AppendLine($"- `{MdCell(u)}`");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// README に書く「必要な素材」の下書き。シナリオのフォルダに無い素材（拡張データ・SRC 本体にあるものと、見つからないもの）の名前を種類ごとに並べる
        /// </summary>
        public string ToReadme()
        {
            var sb = new StringBuilder();
            sb.AppendLine("## 必要な素材");
            sb.AppendLine();
            sb.AppendLine("このシナリオには含まれていない素材です。汎用グラフィック集などから用意し、SRC 本体か拡張データのフォルダに置いてください。");
            sb.AppendLine();
            var external = Assets.Where(a => a.FoundIn != "scenario").ToList();
            if (external.Count == 0)
            {
                sb.AppendLine("（なし。参照している素材はすべてシナリオに含まれています）");
                return sb.ToString();
            }
            foreach (var kind in new[] { "image", "sound", "map" })
            {
                var list = external.Where(a => a.Kind == kind).OrderBy(a => a.Name, StringComparer.Ordinal).ToList();
                if (list.Count == 0)
                {
                    continue;
                }
                sb.AppendLine($"### {KindLabel(kind)}");
                sb.AppendLine();
                foreach (var a in list)
                {
                    sb.AppendLine($"- `{MdCell(a.Name)}`");
                }
                sb.AppendLine();
            }
            if (Dynamic.Count > 0)
            {
                sb.AppendLine($"ほかに、式で名前を組み立てる参照が {Dynamic.Count} か所あります（`srcsharp assets` で確かめられます）。");
            }
            return sb.ToString();
        }

        public string ToJson()
        {
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            {
                w.WriteStartObject();
                if (ScenarioRoot != null)
                {
                    w.WriteString("scenarioRoot", ScenarioRoot);
                }
                w.WriteStartArray("roots");
                foreach (var r in Roots)
                {
                    w.WriteStartObject();
                    w.WriteString("kind", r.Kind);
                    w.WriteString("name", r.Name);
                    w.WriteNumber("files", r.Files.Count);
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteStartArray("assets");
                foreach (var a in Assets)
                {
                    w.WriteStartObject();
                    w.WriteString("name", a.Name);
                    w.WriteString("kind", a.Kind);
                    w.WriteBoolean("missing", a.Missing);
                    if (!a.Missing)
                    {
                        w.WriteString("foundIn", a.FoundIn);
                        w.WriteString("foundPath", a.FoundPath);
                    }
                    w.WriteStartArray("usages");
                    foreach (var u in a.Usages)
                    {
                        w.WriteStartObject();
                        w.WriteString("file", u.File);
                        w.WriteNumber("line", u.Line);
                        w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteStartArray("dynamic");
                foreach (var d in Dynamic)
                {
                    w.WriteStartObject();
                    w.WriteString("file", d.File);
                    w.WriteNumber("line", d.Line);
                    w.WriteString("text", d.Text);
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteStartArray("unused");
                foreach (var u in Unused)
                {
                    w.WriteStringValue(u);
                }
                w.WriteEndArray();
                w.WriteNumber("checkedFiles", CheckedFiles.Count);
                w.WriteNumber("missing", MissingCount);
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>パスの末尾（フォルダの切れ目から後ろ）で引ける、ファイルの一覧</summary>
        private sealed class SuffixIndex
        {
            private readonly Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.Ordinal);

            public SuffixIndex(IEnumerable<string> files)
            {
                foreach (var file in files)
                {
                    foreach (var suffix in Suffixes(Normalize(file)))
                    {
                        // 浅いところにあるものを先に（同じ末尾なら最初に見つけたもの）
                        if (!map.ContainsKey(suffix))
                        {
                            map[suffix] = file;
                        }
                    }
                }
            }

            public string Find(string name)
            {
                return map.TryGetValue(Normalize(name), out var found) ? found : null;
            }
        }
    }
}
