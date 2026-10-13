using ShiftJISExtension;
using SRCCore.Filesystem;
using SRCCore.Models;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCAuthoring.Balance
{
    /// <summary>外れ値（同じクラスのユニットの中で、平均から標準偏差の 2 倍以上離れた値）</summary>
    public sealed class Outlier
    {
        public string Unit { get; set; }
        public string Class { get; set; }
        public string Item { get; set; }
        public double Value { get; set; }
        public double Mean { get; set; }
        public double Z { get; set; }
    }

    public sealed class UnitRow
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public int HP { get; set; }
        public int EN { get; set; }
        public int Armor { get; set; }
        public int Mobility { get; set; }
        public int Speed { get; set; }
        public int Weapons { get; set; }
        public int MaxPower { get; set; }
        public int MaxRange { get; set; }
        public string File { get; set; }
    }

    public sealed class WeaponRow
    {
        public string Unit { get; set; }
        public string Name { get; set; }
        public int Power { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
        public int Precision { get; set; }
        public int Critical { get; set; }
        public int Bullet { get; set; }
        public int EN { get; set; }
        public int Morale { get; set; }
        public string Class { get; set; }
    }

    /// <summary>
    /// ユニットデータ（unit.txt・robot.txt）の能力値の一覧と外れ値。バランス調整の手がかりにする。
    /// </summary>
    public sealed class BalanceReport
    {
        public List<UnitRow> Units { get; } = new List<UnitRow>();
        public List<WeaponRow> Weapons { get; } = new List<WeaponRow>();
        public List<Outlier> Outliers { get; } = new List<Outlier>();
        public List<SrcDiagnostic> Diagnostics { get; } = new List<SrcDiagnostic>();

        /// <summary>外れ値とみなす z の絶対値</summary>
        public double Threshold { get; set; } = 2.0;
        /// <summary>外れ値を調べる、同じクラスのユニットの最少数</summary>
        public int MinGroupSize { get; set; } = 5;

        static BalanceReport()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static BalanceReport Create(IEnumerable<string> paths)
        {
            var report = new BalanceReport();
            var src = new SRCCore.SRC { GUI = new MockGUI(), FileSystem = new LocalFileSystem() };
            var files = paths
                .SelectMany(x => Directory.Exists(x)
                    ? Directory.EnumerateFiles(x, "*.txt", SearchOption.AllDirectories)
                    : File.Exists(x) ? new[] { x } : Array.Empty<string>())
                .Where(x => new[] { "unit.txt", "robot.txt" }.Contains(Path.GetFileName(x).ToLowerInvariant()))
                .ToList();
            foreach (var file in files)
            {
                var before = src.UDList.Items.Select(x => x.Name).ToHashSet();
                try
                {
                    using (var utf8 = new MemoryStream())
                    {
                        using (var fs = File.OpenRead(file))
                        {
                            fs.ConvertEncodingAsync(utf8, Encoding.GetEncoding(932), Encoding.UTF8).Wait();
                        }
                        utf8.Position = 0;
                        src.UDList.Load(Path.GetFileName(file), utf8);
                    }
                }
                catch (Exception ex)
                {
                    report.Diagnostics.Add(new SrcDiagnostic { File = file, Line = 1, Severity = DiagnosticSeverity.Error, Message = ex.Message });
                }
                foreach (var ud in src.UDList.Items.Where(x => !before.Contains(x.Name)))
                {
                    report.Add(ud, file);
                }
                src.ClearDataError();
            }
            report.FindOutliers();
            return report;
        }

        private void Add(UnitData ud, string file)
        {
            var weapons = ud.Weapons.ToList();
            Units.Add(new UnitRow
            {
                Name = ud.Name,
                Class = string.IsNullOrEmpty(ud.Class) ? "-" : ud.Class,
                HP = ud.HP,
                EN = ud.EN,
                Armor = ud.Armor,
                Mobility = ud.Mobility,
                Speed = ud.Speed,
                Weapons = weapons.Count,
                MaxPower = weapons.Select(x => x.Power).DefaultIfEmpty(0).Max(),
                MaxRange = weapons.Select(x => x.MaxRange).DefaultIfEmpty(0).Max(),
                File = file,
            });
            foreach (var w in weapons)
            {
                Weapons.Add(new WeaponRow
                {
                    Unit = ud.Name,
                    Name = w.Name,
                    Power = w.Power,
                    MinRange = w.MinRange,
                    MaxRange = w.MaxRange,
                    Precision = w.Precision,
                    Critical = w.Critical,
                    Bullet = w.Bullet,
                    EN = w.ENConsumption,
                    Morale = w.NecessaryMorale,
                    Class = w.Class,
                });
            }
        }

        private static readonly (string Name, Func<UnitRow, double> Value)[] Metrics =
        {
            ("HP", x => x.HP),
            ("装甲", x => x.Armor),
            ("運動性", x => x.Mobility),
            ("移動力", x => x.Speed),
            ("最大攻撃力", x => x.MaxPower),
        };

        private void FindOutliers()
        {
            Outliers.Clear();
            foreach (var group in Units.GroupBy(x => x.Class).Where(x => x.Count() >= MinGroupSize))
            {
                foreach (var (name, value) in Metrics)
                {
                    var values = group.Select(value).ToList();
                    var mean = values.Average();
                    var sd = Math.Sqrt(values.Select(v => (v - mean) * (v - mean)).Average());
                    if (sd <= 0)
                    {
                        continue;
                    }
                    foreach (var u in group)
                    {
                        var z = (value(u) - mean) / sd;
                        if (Math.Abs(z) >= Threshold)
                        {
                            Outliers.Add(new Outlier { Unit = u.Name, Class = group.Key, Item = name, Value = value(u), Mean = mean, Z = z });
                        }
                    }
                }
            }
        }

        private static string Cell(string s) => (s ?? "").Replace("|", "\\|");

        public string ToMarkdown()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# バランスレポート");
            sb.AppendLine();
            sb.AppendLine($"ユニット {Units.Count} 体、武器 {Weapons.Count} 個。");
            sb.AppendLine();
            sb.AppendLine($"## 外れ値（同じクラスのユニットが {MinGroupSize} 体以上のクラスで、|z| ≥ {Threshold}）");
            sb.AppendLine();
            if (Outliers.Count == 0)
            {
                sb.AppendLine("なし");
            }
            else
            {
                sb.AppendLine("| ユニット | クラス | 項目 | 値 | クラスの平均 | z |");
                sb.AppendLine("|---|---|---|--:|--:|--:|");
                foreach (var o in Outliers.OrderByDescending(x => Math.Abs(x.Z)))
                {
                    sb.AppendLine($"| {Cell(o.Unit)} | {Cell(o.Class)} | {o.Item} | {o.Value:0} | {o.Mean:0.0} | {o.Z:+0.00;-0.00} |");
                }
            }
            sb.AppendLine();
            sb.AppendLine("## クラスごとの平均");
            sb.AppendLine();
            sb.AppendLine("| クラス | 体数 | HP | EN | 装甲 | 運動性 | 移動力 | 最大攻撃力 |");
            sb.AppendLine("|---|--:|--:|--:|--:|--:|--:|--:|");
            foreach (var g in Units.GroupBy(x => x.Class).OrderByDescending(x => x.Count()))
            {
                sb.AppendLine($"| {Cell(g.Key)} | {g.Count()} | {g.Average(x => x.HP):0} | {g.Average(x => x.EN):0} | {g.Average(x => x.Armor):0} | {g.Average(x => x.Mobility):0} | {g.Average(x => x.Speed):0.0} | {g.Average(x => x.MaxPower):0} |");
            }
            sb.AppendLine();
            sb.AppendLine("## ユニット");
            sb.AppendLine();
            sb.AppendLine("| ユニット | クラス | HP | EN | 装甲 | 運動性 | 移動力 | 武器数 | 最大攻撃力 | 最大射程 |");
            sb.AppendLine("|---|---|--:|--:|--:|--:|--:|--:|--:|--:|");
            foreach (var u in Units)
            {
                sb.AppendLine($"| {Cell(u.Name)} | {Cell(u.Class)} | {u.HP} | {u.EN} | {u.Armor} | {u.Mobility} | {u.Speed} | {u.Weapons} | {u.MaxPower} | {u.MaxRange} |");
            }
            return sb.ToString();
        }

        private static string Csv(string s)
        {
            s ??= "";
            return s.IndexOfAny(new[] { ',', '"', '\n' }) >= 0 ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
        }

        public string UnitsCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("unit,class,hp,en,armor,mobility,speed,weapons,max_power,max_range,file");
            foreach (var u in Units)
            {
                sb.AppendLine(string.Join(",", Csv(u.Name), Csv(u.Class), u.HP, u.EN, u.Armor, u.Mobility, u.Speed, u.Weapons, u.MaxPower, u.MaxRange, Csv(u.File)));
            }
            return sb.ToString();
        }

        public string WeaponsCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("unit,weapon,power,min_range,max_range,precision,critical,bullet,en,morale,class");
            foreach (var w in Weapons)
            {
                sb.AppendLine(string.Join(",", Csv(w.Unit), Csv(w.Name), w.Power, w.MinRange, w.MaxRange, w.Precision, w.Critical, w.Bullet, w.EN, w.Morale, Csv(w.Class)));
            }
            return sb.ToString();
        }
    }
}
