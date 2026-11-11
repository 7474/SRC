using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SRCCore.E2E.Tests
{
    /// <summary>
    /// 互換性の検証用のシナリオ「トレス島試験記録」（SRC.Sharp/Compat/torres。#1549）のデータを読み込む。
    /// データ・地形・マップ・イベントが誤り無く読めることと、データが指す画像・形態・スペシャルパワーが揃っていることを確かめる。
    /// シナリオを最初から流すテストは SRCRenderTests の CompatScenarioTests（PlayScript で第一話から最後まで進める）。
    /// </summary>
    [TestClass]
    public class CompatScenarioDataTests
    {
        private static string ScenarioDir([CallerFilePath] string path = "")
        {
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), "..", "..", "Compat", "torres"));
        }

        private static SRC LoadAll()
        {
            var dir = ScenarioDir();
            var src = new SRC();
            new HeadlessGUI().Install(src);
            src.SystemConfig.AppPath = dir;
            src.ScenarioPath = dir + Path.DirectorySeparatorChar;
            var fs = new LocalFileSystem();
            fs.AddPath(dir);
            src.FileSystem = fs;
            src.Event.InitEventData();
            src.Event.SkipExternalSourceLoad = true;

            // 元の SRC の SRC.bas（Data\System の alias.txt・sp.txt・terrain.txt）と Event.bas（Data のローカルデータ）の順
            src.ALDList.Load(Path.Combine(dir, "Data", "System", "alias.txt"));
            src.SPDList.Load(Path.Combine(dir, "Data", "System", "sp.txt"));
            src.TDList.Load(Path.Combine(dir, "Data", "System", "terrain.txt"));
            src.LoadDataDirectory(Path.Combine(dir, "Data"));
            AssertNoDataError(src, "データ");
            return src;
        }

        private static void AssertNoDataError(SRC src, string what)
        {
            Assert.IsFalse(src.HasDataError, what + "の誤り:\n" + string.Join("\n", src.DataErrors.Select(x => $"{x.fname}({x.line_num}): {x.msg}")));
        }

        [TestMethod]
        public void DataPack_LoadsWithoutErrors()
        {
            var src = LoadAll();
            Assert.IsTrue(src.PDList.Items.Count >= 8, "パイロット");
            Assert.IsTrue(src.UDList.Items.Count >= 14, "ユニット");
            Assert.IsTrue(src.IDList.IsDefined("スパークボム") && src.IDList.IsDefined("試作スパークボム"), "アイテム");
            Assert.IsTrue(src.NPDList.IsDefined("湊カイ(笑顔)"), "非戦闘員（表情の差分）");
            Assert.IsTrue(src.SPDList.IsDefined("手当て"), "スペシャルパワー");
            Assert.IsTrue(src.ALDList.IsDefined("学舎式連携"), "エリアス");
            Assert.IsTrue(src.TDList.IsDefined(1000), "地形");
        }

        [TestMethod]
        public void Maps_UseDefinedTerrainsWithBitmaps()
        {
            var dir = ScenarioDir();
            var src = LoadAll();
            foreach (var map in Directory.GetFiles(dir, "*.map"))
            {
                // 元の SRC の LoadMapData と同じく、幅・高さの次に x ごとに y を並べた「地形番号,画像番号」が続く
                var lines = File.ReadAllLines(map).Where(x => x.Trim() != "").ToList();
                Assert.AreEqual("MapData", lines[0], map);
                var size = lines[1].Split(',').Select(int.Parse).ToArray();
                Assert.IsTrue(size[0] >= 15 && size[1] >= 15, $"{map}: マップは 15×15 以上");
                var cells = lines.Skip(2).Select(x => x.Split(',').Select(int.Parse).ToArray()).ToList();
                Assert.AreEqual(size[0] * size[1], cells.Count, map);
                foreach (var cell in cells)
                {
                    Assert.IsTrue(src.TDList.IsDefined(cell[0]), $"{map}: {cell[0]} 番の地形が無い");
                    var bitmap = src.TDList.Item(cell[0]).Bitmap;
                    var file = Path.Combine(dir, "Bitmap", "Map", bitmap, $"{bitmap}{cell[1]:0000}.bmp");
                    Assert.IsTrue(File.Exists(file), $"{map}: {file} が無い");
                }

                src.Map.LoadMapData(map);
                Assert.AreEqual(size[0], src.Map.MapWidth, map);
                Assert.AreEqual(size[1], src.Map.MapHeight, map);
            }
        }

        [TestMethod]
        public void Data_ReferencesExist()
        {
            var dir = ScenarioDir();
            var src = LoadAll();
            var missing = new List<string>();
            void RequireFile(string folder, string name, string owner)
            {
                // SRC が自分で足すデータ（ステータス表示用のダミー・ナレーター）は画像を持たない
                var stem = Path.GetFileNameWithoutExtension(name ?? "");
                if (stem != "" && stem != "-" && !File.Exists(Path.Combine(dir, "Bitmap", folder, name)))
                {
                    missing.Add($"{owner}: Bitmap/{folder}/{name}");
                }
            }
            void RequireUnit(string name, string owner)
            {
                if (!src.UDList.IsDefined(name))
                {
                    missing.Add($"{owner}: ユニット「{name}」");
                }
            }
            void RequirePilot(string name, string owner)
            {
                if (!src.PDList.IsDefined(name))
                {
                    missing.Add($"{owner}: パイロット「{name}」");
                }
            }

            foreach (var ud in src.UDList.Items)
            {
                RequireFile("Unit", ud.Bitmap0, ud.Name);
                // 召喚のアビリティ（「召喚=ユニット名」）の行き先
                foreach (var ad in ud.Abilities)
                {
                    foreach (var effect in ad.Effects.Where(x => x.EffectType == "召喚"))
                    {
                        RequireUnit(effect.Data, $"{ud.Name} の{ad.Name}");
                    }
                }
                foreach (var fd in ud.Features)
                {
                    var args = fd.Data.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    switch (fd.Name)
                    {
                        case "変形":
                        case "分離":
                            args.Skip(1).ToList().ForEach(x => RequireUnit(x, $"{ud.Name} の{fd.Name}"));
                            break;
                        case "合体":
                            args.Skip(1).ToList().ForEach(x => RequireUnit(x, $"{ud.Name} の合体"));
                            break;
                        case "換装":
                            args.ToList().ForEach(x => RequireUnit(x, $"{ud.Name} の換装"));
                            break;
                        case "ハイパーモード":
                        case "パーツ分離":
                            // 「名前 形態」
                            RequireUnit(args.Last(), $"{ud.Name} の{fd.Name}");
                            break;
                        case "ノーマルモード":
                            // 「形態 残り時間 …」
                            RequireUnit(args[0], $"{ud.Name} の{fd.Name}");
                            break;
                        case "追加パイロット":
                        case "追加サポート":
                        case "暴走時パイロット":
                            RequirePilot(args[0], $"{ud.Name} の{fd.Name}");
                            break;
                        case "合体技":
                            if (!ud.Weapons.Any(w => w.Name == args[0]))
                            {
                                missing.Add($"{ud.Name} の合体技: 武器「{args[0]}」");
                            }
                            args.Skip(1).ToList().ForEach(x => RequireUnit(x, $"{ud.Name} の合体技"));
                            break;
                    }
                }
            }
            foreach (var pd in src.PDList.Items)
            {
                RequireFile("Pilot", pd.Bitmap0, pd.Name);
                for (var i = 1; i <= pd.CountSpecialPower(99); i++)
                {
                    var sp = pd.SpecialPower(99, i);
                    if (!src.SPDList.IsDefined(sp))
                    {
                        missing.Add($"{pd.Name}: スペシャルパワー「{sp}」");
                    }
                }
            }
            foreach (var npd in src.NPDList.Items)
            {
                RequireFile("Pilot", npd.Bitmap0, npd.Name);
            }
            Assert.AreEqual(0, missing.Count, "見つからないもの:\n" + string.Join("\n", missing));
        }

        /// <summary>
        /// 第六話の Load moon で読む Data\moon は、シナリオの始めには読まれず、Load で誤り無く読めること。
        /// 読んだユニットが指す画像とパイロットが揃っていること
        /// </summary>
        [TestMethod]
        public void LoadedData_Moon_LoadsWithoutErrors()
        {
            var dir = ScenarioDir();
            var src = LoadAll();
            Assert.IsFalse(src.UDList.IsDefined("月面錆蟲"), "Data\\moon はシナリオの始めには読まれない");
            src.LoadDataDirectory(Path.Combine(dir, "Data", "moon"));
            AssertNoDataError(src, "Data\\moon");
            var ud = src.UDList.Item("月面錆蟲");
            Assert.IsNotNull(ud, "月面錆蟲");
            Assert.IsTrue(File.Exists(Path.Combine(dir, "Bitmap", "Unit", ud.Bitmap0)), ud.Bitmap0);
            foreach (var pname in new[] { "月蟲(ザコ)", "月蟲長" })
            {
                Assert.IsTrue(src.PDList.IsDefined(pname), pname);
                Assert.IsTrue(File.Exists(Path.Combine(dir, "Bitmap", "Pilot", src.PDList.Item(pname).Bitmap0)), pname);
            }
        }

        [TestMethod]
        public void Events_LoadWithoutErrors()
        {
            var dir = ScenarioDir();
            var src = LoadAll();
            var files = Directory.GetFiles(dir, "*.eve", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.Ordinal).ToList();
            Assert.IsTrue(files.Count >= 9, "ep01.eve〜ep08.eve・Lib/include.eve");
            foreach (var file in files)
            {
                src.Event.LoadEventData(file, "");
                AssertNoDataError(src, Path.GetFileName(file));
            }
        }
    }
}
