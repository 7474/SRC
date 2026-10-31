using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Golden.Tests
{
    /// <summary>
    /// 戦闘の計算結果（使えるか・命中率・ダメージ・クリティカル率・期待ダメージ）を、組み合わせごとの表にして固定する（#1355）。
    /// いまの結果を Golden/Battle/expected.txt に置き、変わったら知らせる。正しさの確かめは
    /// UnitWeaponCovCombatTests など（VB 版を期待値にしたもの）が受け持ち、ここは「変わっていないこと」を見る。
    /// 意図して変えたときは、SRC_GOLDEN_UPDATE=1 で表を作り直し、差分を PR に載せる（docs/testing.md）。
    /// </summary>
    [TestClass]
    public class BattleGoldenTests
    {
        private static readonly string[] AttackerPilots = { "標準パイロット", "エースパイロット" };
        private static readonly string[] TargetUnits = { "基準機", "重装甲機", "高機動機", "空戦機", "巨大機" };
        // 防御側のマスの地形（名前・クラス・命中の修正・ダメージの修正）
        private static readonly (string Name, string Class, int Hit, int Damage)[] Terrains =
        {
            ("平地", "陸", 0, 0),
            ("林", "陸", 15, 10),
            ("宇宙", "宇宙", 0, 0),
        };
        private static readonly int[] Distances = { 1, 3 };
        private static readonly int[] Morales = { 100, 130 };
        // かかっているスペシャルパワー（なし・攻撃側・防御側）
        private static readonly (string Label, string Attacker, string Target)[] SpecialPowers =
        {
            ("なし", null, null),
            ("攻撃側に必中", "必中", null),
            ("攻撃側に熱血", "熱血", null),
            ("防御側に集中", null, "集中"),
        };

        [TestMethod]
        public void BattleResults_MatchGolden()
        {
            var actual = Render();
            var expectedPath = Path.Combine(GoldenDir(), "expected.txt");
            if (Environment.GetEnvironmentVariable("SRC_GOLDEN_UPDATE") == "1")
            {
                File.WriteAllText(expectedPath, actual, new UTF8Encoding(false));
                Assert.Inconclusive("expected.txt を作り直した（SRC_GOLDEN_UPDATE=1）");
            }
            Assert.IsTrue(File.Exists(expectedPath), "expected.txt が無い。SRC_GOLDEN_UPDATE=1 dotnet test --filter BattleGoldenTests で作る");
            var expected = File.ReadAllText(expectedPath).Replace("\r\n", "\n");
            if (expected == actual)
            {
                return;
            }
            // どの組み合わせが変わったかを先頭から 20 件だけ出す
            var expectedLines = expected.Split('\n');
            var actualLines = actual.Split('\n');
            var diffs = new List<string>();
            for (var i = 0; i < Math.Max(expectedLines.Length, actualLines.Length) && diffs.Count < 20; i++)
            {
                var e = i < expectedLines.Length ? expectedLines[i] : "(無し)";
                var a = i < actualLines.Length ? actualLines[i] : "(無し)";
                if (e != a)
                {
                    diffs.Add($"- {e}\n+ {a}");
                }
            }
            Assert.Fail("戦闘の計算結果が変わった。意図した変更なら SRC_GOLDEN_UPDATE=1 で作り直して差分を PR に載せる\n" + string.Join("\n", diffs));
        }

        private static string Render()
        {
            var sb = new StringBuilder();
            sb.Append("# 戦闘の計算結果（BattleGoldenTests が作る。手で直さない）\n");
            sb.Append("# 攻撃側のパイロット/ユニット | 武器 | 防御側 | 地形 | 距離 | 気力 | スペシャルパワー => 使える 命中率 ダメージ クリティカル率 期待ダメージ\n");
            foreach (var pilot in AttackerPilots)
            {
                var weapons = NewBattle(pilot, "基準機", Terrains[0], 1, 100).Attacker.Weapons.Select(w => w.Name).ToList();
                foreach (var weapon in weapons)
                {
                    foreach (var target in TargetUnits)
                    {
                        foreach (var terrain in Terrains)
                        {
                            foreach (var distance in Distances)
                            {
                                foreach (var morale in Morales)
                                {
                                    foreach (var sp in SpecialPowers)
                                    {
                                        var b = NewBattle(pilot, target, terrain, distance, morale);
                                        if (sp.Attacker != null)
                                        {
                                            b.Attacker.MakeSpecialPowerInEffect(sp.Attacker);
                                        }
                                        if (sp.Target != null)
                                        {
                                            b.Target.MakeSpecialPowerInEffect(sp.Target);
                                        }
                                        var w = b.Attacker.Weapons.First(x => x.Name == weapon);
                                        sb.Append($"{pilot}/基準機 | {weapon} | {target} | {terrain.Name} | {distance} | {morale} | {sp.Label} => ")
                                            .Append(w.IsWeaponAvailable("移動前") ? "可" : "不可")
                                            .Append($" {w.HitProbability(b.Target, true)} {w.Damage(b.Target, true)} {w.CriticalProbability(b.Target)} {w.ExpDamage(b.Target, true)}\n");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }

        private class Battle
        {
            public Unit Attacker;
            public Unit Target;
        }

        private static Battle NewBattle(string attackerPilot, string targetUnit, (string Name, string Class, int Hit, int Damage) terrain, int distance, int morale)
        {
            var src = CreateSrc(width: 8, height: 3);
            src.GUI = new MockGUI();
            src.Stage = "味方";
            var fs = new SRCCore.Filesystem.LocalFileSystem();
            fs.AddPath(GoldenDir());
            fs.AddPath(BasicDataSystemDir());
            src.FileSystem = fs;
            src.SPDList.Load(Path.Combine(BasicDataSystemDir(), "sp.txt"));
            src.LoadDataDirectory(Path.Combine(GoldenDir(), "Data"));
            SetTerrain(src, 1 + distance, 2, terrain.Name, terrain.Class, terrain.Hit, terrain.Damage);

            var attacker = Place(src, "基準機", attackerPilot, "味方", 1, 2, "地上");
            attacker.MainPilot().Morale = morale;
            var area = terrain.Class == "宇宙" ? "宇宙" : targetUnit == "空戦機" ? "空中" : "地上";
            var target = Place(src, targetUnit, "防御側パイロット", "敵", 1 + distance, 2, area);
            return new Battle { Attacker = attacker, Target = target };
        }

        private static Unit Place(SRC src, string unitName, string pilotName, string party, int x, int y, string area)
        {
            var u = src.UList.Add(unitName, 0, party);
            var p = src.PList.Add(pilotName, 30, party);
            p.Ride(u);
            u.Update();
            u.Status = "出撃";
            u.x = x;
            u.y = y;
            u.Area = area;
            u.HP = u.MaxHP;
            u.EN = u.MaxEN;
            src.Map.MapDataForUnit[x, y] = u;
            return u;
        }

        private static string GoldenDir([CallerFilePath] string path = "")
        {
            return Path.Combine(Path.GetDirectoryName(path), "Battle");
        }

        private static string BasicDataSystemDir([CallerFilePath] string path = "")
        {
            // SRC.Sharp/SRCCoreTests/Golden → SRC.Sharp/Content/basic-data/Data/System
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), "..", "..", "Content", "basic-data", "Data", "System"));
        }
    }
}
