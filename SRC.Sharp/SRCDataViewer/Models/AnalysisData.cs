using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SRCDataViewer.Models
{
    // 組の評価（7474/SRC-Data の Pages の analysis/*.json。SRC の tools/pack-analysis --json が書く）。
    // 書式は tools/pack-analysis/README.md の「JSON」（7474/SRC#1607）

    /// <summary>評価の JSON の一覧（analysis/index.json）</summary>
    public class AnalysisIndex
    {
        public int Format { get; set; }
        public string GeneratedAt { get; set; }
        public List<AnalysisIndexFile> Files { get; set; }
    }

    public class AnalysisIndexFile
    {
        /// <summary>区分（ロボ・拡張・Sharp）</summary>
        public string Section { get; set; }
        public string File { get; set; }
    }

    /// <summary>1 つの区分の組の評価</summary>
    public class AnalysisSet
    {
        public int Format { get; set; }
        public string Pack { get; set; }
        public string GeneratedAt { get; set; }
        public int Level { get; set; }
        public List<string> Uses { get; set; } = new List<string>();
        public List<string> Indices { get; set; } = new List<string>();
        public List<string> Bands { get; set; } = new List<string>();
        public List<AnalysisWork> Works { get; set; } = new List<AnalysisWork>();
        public List<AnalysisPair> Pairs { get; set; } = new List<AnalysisPair>();
    }

    public class AnalysisWork
    {
        public string Name { get; set; }
        /// <summary>作品の組の数</summary>
        public int Pairs { get; set; }
        /// <summary>そのうち役割表で組んだ数（0 なら役割表が無い）</summary>
        public int Declared { get; set; }
    }

    public class AnalysisPair
    {
        public string Work { get; set; }
        /// <summary>組の決め方（役割表・名前の一致・クラスの一致など）</summary>
        public string How { get; set; }
        public string Band { get; set; }
        public string Role { get; set; }
        /// <summary>格（役割表・格の一覧。無ければ空）</summary>
        public string Rank { get; set; }
        public AnalysisPilot Pilot { get; set; }
        public AnalysisUnit Unit { get; set; }
        public List<string> SubPilots { get; set; }
        public bool Evaluated { get; set; }
        public Dictionary<string, string> Grades { get; set; } = new Dictionary<string, string>();
        /// <summary>使いどころごとの、同じ帯の中で上位何 % か（小さいほど強い）</summary>
        public Dictionary<string, double> Percentiles { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, AnalysisIndexValue> Indices { get; set; } = new Dictionary<string, AnalysisIndexValue>();
        public double? Gap { get; set; }
        public Dictionary<string, double> Values { get; set; } = new Dictionary<string, double>();

        /// <summary>区分（読み込んだときに入れる）</summary>
        [JsonIgnore]
        public string Section { get; set; }

        /// <summary>URL に残す組の鍵</summary>
        [JsonIgnore]
        public string Key => string.Join("|", Section, Work, Pilot?.Name, Unit?.Name);

        [JsonIgnore]
        public string Label => $"{Pilot?.Name}／{Unit?.Name}";

        /// <summary>指数（effective か simple）。無ければ null</summary>
        public double? IndexOf(string index, bool effective)
        {
            if (Indices == null || !Indices.TryGetValue(index, out var v) || v == null) return null;
            return effective ? v.Effective : v.Simple;
        }

        /// <summary>評価の S〜D を、使いどころの順に並べた文字列</summary>
        public string GradeString(IEnumerable<string> uses) =>
            string.Concat(uses.Select(u => Grades != null && Grades.TryGetValue(u, out var g) ? g : "-"));
    }

    public class AnalysisIndexValue
    {
        /// <summary>実効性能の指数（同じ帯の中央値 = 100）</summary>
        public double? Effective { get; set; }
        /// <summary>単純数字の指数</summary>
        public double? Simple { get; set; }
    }

    public class AnalysisPilot
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Bitmap { get; set; }
        public string Class { get; set; }
        public int Exp { get; set; }
        public int Sp { get; set; }
        public Dictionary<string, int> Stats { get; set; } = new Dictionary<string, int>();
        public List<string> SpecialPowers { get; set; } = new List<string>();
    }

    public class AnalysisUnit
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Bitmap { get; set; }
        public string Class { get; set; }
        public int Exp { get; set; }
        public int Hp { get; set; }
        public int En { get; set; }
        public int Armor { get; set; }
        public int Mobility { get; set; }
        public int Speed { get; set; }
        public string Size { get; set; }
        public AnalysisWeapon TopWeapon { get; set; }
    }

    public class AnalysisWeapon
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
    }
}
