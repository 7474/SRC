using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.Lib;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// 元の SRC（VB5）のセーブデータ（クリアデータ・中断データ）を読む（#1577）。
    /// 見本は元の SRC の書き出しの手続き（SRC.bas の SaveData・DumpData と、各クラスの Save・Dump。docs/history/porting/vb-savedata.md）から
    /// 組み立てたもの（行をそのまま書いたものと、VbSaveDataWriter で SRC# の状態から書いたもの）。
    /// 期待値は元の SRC の読み込みの手続き（LoadData・RestoreData と版による読み替え）から読んだ値
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class VbSaveDataTests
    {
        private static readonly string[] Scenario =
        {
            // 先頭行のラベルは IsEventDefined が見落とすため、ダミーのラベルを先頭に置く
            "準備:",
            "Exit",
            "",
            "再開:",
            "Global 再開回数",
            "Incr 再開回数",
            "Exit",
            "",
            "スタート:",
            "Global スタート回数",
            "Incr スタート回数",
            "Exit",
            "",
            "ターン 2 味方:",
            "Exit",
        };

        /// <summary>シナリオのフォルダに、パイロット・ユニット・アイテム・スペシャルパワー・エリアスのデータとイベントを置く</summary>
        private static void WriteData(SrcPlayDataSaveFixture f)
        {
            f.Write("Data/pilot.txt",
                "試験員アキ",
                "アキ, しけんいんあき, 女性, 試験, AAAA, 150",
                "特殊能力なし",
                "140, 140, 140, 140, 140, 140, 普通",
                "ＳＰ, 40, 集中, 1",
                "-.bmp, -.mid",
                "",
                "試験員ハル",
                "ハル, しけんいんはる, 男性, 試験, AAAA, 150",
                "特殊能力なし",
                "140, 140, 140, 140, 140, 140, 普通",
                "ＳＰ, 50, 集中, 1",
                "-.bmp, -.mid",
                "");
            f.Write("Data/unit.txt",
                "試験機",
                "試験機, しけんき, 試験, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                "3000, 100, 600, 80",
                "AAAA, -.bmp",
                "試験ライフル, 1200, 1, 4, +0, 6, -, -, AABA, +0, Ｐ",
                "===",
                "応急処置, 回復Lv1, 1, 1, 3, 10, -",
                "");
            f.Write("Data/item.txt",
                "試験フレーム",
                "試験フレーム, しけんふれーむ, 汎用, 強化パーツ",
                "特殊能力なし",
                "500, 20, 0, 0, 0",
                "*最大ＨＰ+500、最大ＥＮ+20。",
                "");
            f.Write("Data/sp.txt",
                "集中, しゅうちゅう",
                "集, 15, 自分, ターン, -, -, -",
                "命中強化Lv3 回避強化Lv3",
                "このターンの間、命中と回避が30上がる。",
                "");
            f.Write("Data/alias.txt",
                "作品エリアス",
                "バリア=バリア 1000",
                "");
            f.Write("test.eve", Scenario);
            f.Write("sub.eve",
                "ターン 5 味方:",
                "Exit",
                "");
        }

        private static MemoryStream Sjis(params string[] lines)
        {
            return new MemoryStream(VbSaveDataReader.ShiftJis.GetBytes(string.Join("\r\n", lines) + "\r\n"));
        }

        // ── 中断データ（行をそのまま書いた見本） ──

        /// <summary>
        /// 中断データの見本（Ver.2.2.33。レイヤは書かない）。header は版とシナリオのファイル名の行、
        /// additionalSupport はユニットの追加サポートの行を書くか（版 20210 から）、requireFiles は Require のファイルの行を書くか（版 20004 から）
        /// </summary>
        private static string[] SuspendSample(string[] header, bool additionalSupport = true, bool requireFiles = true,
            string condition = "\"攻撃力ＵＰ\",3,-1000,\"\"", bool layer = false, bool random = true)
        {
            var lines = new List<string>(header);
            lines.AddRange(new[]
            {
                // 使用するデータ（作品）
                "0",
                // Turn, TotalTurn, Money
                "3",
                "10",
                "5000",
                // グローバル変数（数値の変数も Format$ の文字列で書く）
                "2",
                "\"次ステージ\",\"\"",
                "\"進行度\",\"7\"",
                // ローカル変数
                "1",
                "\"作戦\",\"正面\"",
                // イベント用ラベル（2 つ目だけ無効。保存した数より多いラベルは有効にする）
                "2",
                "#TRUE#",
                "#FALSE#",
            });
            if (requireFiles)
            {
                lines.AddRange(new[] { "1", "\"sub.eve\"" });
            }
            lines.AddRange(new[]
            {
                // パイロット
                "2",
                "\"試験員アキ\",\"試験員アキ\",\"味方\"",
                "5,120",
                "30,120,0",
                "#TRUE#,#FALSE#,0",
                "\"試験機:1\"",
                "\"試験員ハル\",\"試験員ハル\",\"敵\"",
                "3,0",
                "40,100,0",
                "#TRUE#,#FALSE#,0",
                "\"試験機:2\"",
                // アイテム
                "1",
                "\"試験フレーム\",\"試験フレーム_1\",#TRUE#",
                "\"試験機:1\"",
                // ユニット
                "2",
                "\"試験機\",\"試験機:1\",\"味方\"",
                "0,-1,2,2",
                "\"陸\",1,\"通常\",\"出撃\"",
                "\"-\"",
                "\"-\"",
                "1,0",
                "0,1",
                "0",
                "0",
                "1",
                "\"試験員アキ\"",
                "0",
            });
            if (additionalSupport)
            {
                lines.Add("\"\"");
            }
            lines.AddRange(new[]
            {
                "0",
                "1",
                "\"試験フレーム_1\"",
                "1",
                "\"集中\",\"\"",
                "1",
                condition,
                "1",
                "3",
                "1",
                "1",
                "2500,80",
                "\"試験機\",\"試験機:2\",\"敵\"",
                "0,-1,3,3",
                "\"陸\",0,\"通常\",\"出撃\"",
                "\"-\"",
                "\"-\"",
                "0,0",
                "0,0",
                "0",
                "0",
                "1",
                "\"試験員ハル\"",
                "0",
            });
            if (additionalSupport)
            {
                lines.Add("\"\"");
            }
            lines.AddRange(new[]
            {
                "0",
                "0",
                "0",
                "0",
                "1",
                "6",
                "1",
                "3",
                "3000,100",
                // マップ
                "\"test.map\",\"\"",
                "3,3",
            });
            for (var i = 1; i <= 9; i++)
            {
                lines.Add("0,0");
            }
            lines.Add("2,2");
            if (layer)
            {
                lines.Add("\"Layer\"");
                for (var i = 1; i <= 9; i++)
                {
                    lines.Add(i == 5 ? "0,1,2" : "10000,10000,1");
                }
            }
            // ＢＧＭ（見つからない）、連続演奏・固定・ボス用
            lines.AddRange(new[] { "\"\"", "#FALSE#", "#FALSE#", "#FALSE#" });
            if (random)
            {
                lines.AddRange(new[] { "12345", "7" });
            }
            return lines.ToArray();
        }

        private static SrcPlayDataSaveFixture Restore(string[] sample)
        {
            var f = new SrcPlayDataSaveFixture();
            WriteData(f);
            f.Src.RestoreData(Sjis(sample), SRCSaveKind.Suspend);
            Assert.IsFalse(f.Gui.IsTerminated, string.Join("\n", f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Select(x => x.Args[0])));
            return f;
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Ver2233_RestoresState()
        {
            GeneralLib.RndSeed = 1;
            using var f = Restore(SuspendSample(new[] { "20233", "\"test.eve\"" }));
            var src = f.Src;

            // シナリオ・ターン・資金
            Assert.AreEqual(Path.Combine(f.ScenarioDir, "test.eve"), src.ScenarioFileName);
            Assert.AreEqual(3, src.Turn);
            Assert.AreEqual(10, src.TotalTurn);
            Assert.AreEqual(5000, src.Money);
            Assert.AreEqual("味方", src.Stage);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);

            // 変数。元の SRC は数値の変数も文字列で書くので、文字列の変数として読まれる
            var progress = src.Event.GlobalVariableList["進行度"];
            Assert.AreEqual(ValueType.StringType, progress.VariableType);
            Assert.AreEqual("7", progress.StringValue);
            Assert.AreEqual(7d, src.Expression.GetValueAsDouble("進行度"));
            Assert.AreEqual("正面", src.Event.LocalVariableList["作戦"].StringValue);

            // イベントラベル: 2 つ目だけ無効。Require のファイルを読み込む
            var labels = src.Event.colEventLabelList.List;
            Assert.IsFalse(labels[1].Enable);
            Assert.IsTrue(labels.Where((x, i) => i != 1).All(x => x.Enable));
            CollectionAssert.AreEqual(new[] { "sub.eve" }, src.Event.AdditionalEventFileNames.ToArray());
            Assert.IsTrue(labels.Any(x => x.Data == "ターン 5 味方"));

            // パイロット
            var aki = src.PList.Item("試験員アキ");
            Assert.AreEqual(5, aki.Level);
            Assert.AreEqual(120, aki.Exp);
            Assert.AreEqual(30, aki.SP);
            Assert.AreEqual(120, aki.Morale);
            Assert.AreEqual("味方", aki.Party);
            Assert.AreEqual("試験機:1", aki.Unit.ID);
            var haru = src.PList.Item("試験員ハル");
            Assert.AreEqual("敵", haru.Party);
            Assert.AreEqual(100, haru.Morale);

            // ユニット
            var u1 = src.UList.Item("試験機:1");
            Assert.AreEqual("味方", u1.Party0);
            Assert.AreEqual(2, u1.x);
            Assert.AreEqual(2, u1.y);
            Assert.AreEqual("出撃", u1.Status);
            Assert.AreEqual(1, u1.UsedAction);
            Assert.AreEqual(1, u1.UsedSupportAttack);
            Assert.AreEqual(1, u1.UsedCounterAttack);
            Assert.AreEqual("試験員アキ", u1.MainPilot().ID);
            Assert.IsTrue(u1.IsSpecialPowerInEffect("集中"));
            Assert.AreEqual(3, u1.Conditions.Single(x => x.Name == "攻撃力ＵＰ").Lifetime);
            Assert.AreEqual(3, u1.Weapon(1).Bullet());
            Assert.AreEqual(1, u1.Ability(1).Stock());
            // ＨＰ・ＥＮは特殊能力（アイテム）を付けた後で設定し直す
            Assert.AreEqual(3500, u1.MaxHP);
            Assert.AreEqual(2500, u1.HP);
            Assert.AreEqual(80, u1.EN);
            var u2 = src.UList.Item("試験機:2");
            Assert.AreEqual("敵", u2.Party0);
            Assert.AreEqual(6, u2.Weapon(1).Bullet());

            // アイテム
            var item = src.IList.Item("試験フレーム_1");
            Assert.AreEqual("試験機:1", item.Unit.ID);
            Assert.AreSame(item, u1.ItemList.Single());

            // マップ
            Assert.AreEqual(Path.Combine(f.ScenarioDir, "test.map"), src.Map.MapFileName);
            Assert.AreEqual(3, src.Map.MapWidth);
            Assert.AreSame(u1, src.Map.MapDataForUnit[2, 2]);
            Assert.AreSame(u2, src.Map.MapDataForUnit[3, 3]);
            // レイヤを書いていない（Ver.2.2.33）ので、上層は無し
            Assert.AreEqual(Maps.MapCell.NO_LAYER_NUM, src.Map.MapData[2, 2].LayerType);

            // 新しく起動して読んだのでマップが変わり、再開のイベントが起きる
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("再開回数"));
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("スタート回数"));

            // 乱数系列
            Assert.AreEqual(12345, GeneralLib.RndSeed);
            Assert.AreEqual(7, GeneralLib.RndIndex);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Ver240_ReadsLayer()
        {
            // Ver.2.4.0 の手直し（240a）で足したレイヤ（"Layer" の行から）を読む
            using var f = Restore(SuspendSample(new[] { "20400", "\"test.eve\"" }, layer: true));

            var cell = f.Src.Map.MapData[2, 2];
            Assert.AreEqual(0, cell.LayerType);
            Assert.AreEqual(1, cell.LayerBitmapNo);
            Assert.AreEqual(Maps.BoxTypes.Upper, cell.BoxType);
            Assert.AreEqual(Maps.MapCell.NO_LAYER_NUM, f.Src.Map.MapData[1, 1].LayerType);
            Assert.AreEqual(2500, f.Src.UList.Item("試験機:1").HP);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Before20210_HasNoAdditionalSupportLine()
        {
            // 版 20210 より前はユニットの追加サポートを書いていない
            using var f = Restore(SuspendSample(new[] { "20200", "\"test.eve\"" }, additionalSupport: false));

            Assert.AreEqual(2500, f.Src.UList.Item("試験機:1").HP);
            Assert.AreEqual(3, f.Src.UList.Item("試験機:1").Weapon(1).Bullet());
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Before20004_HasNoRequireFiles()
        {
            // 版 20003 までは Require のイベントファイルを書いていない
            using var f = Restore(SuspendSample(new[] { "20003", "\"test.eve\"" }, additionalSupport: false, requireFiles: false));

            Assert.AreEqual(0, f.Src.Event.AdditionalEventFileNames.Count);
            Assert.IsFalse(f.Src.Event.colEventLabelList.List.Any(x => x.Data == "ターン 5 味方"));
            Assert.AreEqual("試験機:1", f.Src.PList.Item("試験員アキ").Unit.ID);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Before10741_SkipsPilotAbilityConditions()
        {
            // 版 10741 より前は「パイロット能力付加」を特殊状態で持っていたので、その特殊状態を読まない
            using var f = Restore(SuspendSample(new[] { "10740", "\"test.eve\"" }, additionalSupport: false, requireFiles: false,
                condition: "\"底力付加\",-1,-1000,\"底力 パイロット能力付加 1\""));

            Assert.AreEqual(0, f.Src.UList.Item("試験機:1").Conditions.Count);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Since10741_KeepsConditionsWithCommaAndQuote()
        {
            // 特殊状態のデータは「"」や「,」を含みうるので、行を手で区切って読む
            using var f = Restore(SuspendSample(new[] { "20233", "\"test.eve\"" },
                condition: "\"装甲付加\",2,1,\"装甲 \"強化\", 付加\""));

            var cnd = f.Src.UList.Item("試験機:1").Conditions.Single();
            Assert.AreEqual("装甲付加", cnd.Name);
            Assert.AreEqual(2, cnd.Lifetime);
            Assert.AreEqual(1d, cnd.Level);
            Assert.AreEqual("装甲 \"強化\", 付加", cnd.StrData);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_WithoutVersion_ReadsScenarioFromFirstLine()
        {
            // 版を書いていない中断データは、最初の値がシナリオのファイル名（版を 1 として読む）
            using var f = Restore(SuspendSample(new[] { "\"test.eve\"" }, additionalSupport: false, requireFiles: false));

            Assert.AreEqual(Path.Combine(f.ScenarioDir, "test.eve"), f.Src.ScenarioFileName);
            Assert.AreEqual(3, f.Src.Turn);
            Assert.AreEqual(5000, f.Src.Money);
            Assert.AreEqual(2, f.Src.UList.Items.Count);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_WithoutRandomSeries_KeepsCurrentSeries()
        {
            // 乱数系列を書いていない（ファイルの終わり）なら、乱数系列は戻さない
            GeneralLib.RndSeed = 4321;
            using var f = Restore(SuspendSample(new[] { "20233", "\"test.eve\"" }, random: false));

            Assert.AreEqual(4321, GeneralLib.RndSeed);
            Assert.AreEqual(3, f.Src.Turn);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_AbsoluteMapPath_FindsMapInScenarioFolder()
        {
            // マップがシナリオのフォルダの外なら元の SRC は絶対パスで書く。いまのシナリオのフォルダにある同じ名前のファイルを探す
            using var f = new SrcPlayDataSaveFixture();
            WriteData(f);
            var map = f.Write("maps/test.map", "");
            var sample = SuspendSample(new[] { "20233", "\"test.eve\"" })
                .Select(x => x == "\"test.map\",\"\"" ? "\"C:\\old\\scenario\\maps\\test.map\",\"通常(マップ限定)\"" : x)
                .ToArray();

            f.Src.RestoreData(Sjis(sample), SRCSaveKind.Suspend);

            Assert.AreEqual(map, f.Src.Map.MapFileName);
            Assert.AreEqual("通常", f.Src.Map.MapDrawMode);
            Assert.IsTrue(f.Src.Map.MapDrawIsMapOnly);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Quick_KeepsTitlesAndDoesNotShowLoadingForm()
        {
            // クイックロードでは使用するデータを読み飛ばし、ロード画面を出さない
            using var f = new SrcPlayDataSaveFixture();
            WriteData(f);
            f.Src.RestoreData(Sjis(SuspendSample(new[] { "20233", "\"test.eve\"" })), SRCSaveKind.Suspend);
            f.Src.Money = 1;
            var mark = f.Gui.Calls.Count;

            f.Src.RestoreData(Sjis(SuspendSample(new[] { "20233", "\"test.eve\"" })), SRCSaveKind.Quik);

            Assert.AreEqual(5000, f.Src.Money);
            Assert.IsFalse(f.CalledSince(mark, nameof(IGUI.OpenNowLoadingForm)));
            Assert.IsTrue(f.Src.IsQuickSaveDataAvailable);
            Assert.AreEqual(2, f.Src.UList.Items.Count);
        }

        [TestMethod]
        public void RestoreData_UndefinedUnit_ShowsErrorAndTerminates()
        {
            using var f = new SrcPlayDataSaveFixture();
            WriteData(f);
            var sample = SuspendSample(new[] { "20233", "\"test.eve\"" })
                .Select(x => x == "\"試験機\",\"試験機:2\",\"敵\"" ? "\"無い機体\",\"試験機:2\",\"敵\"" : x)
                .ToArray();

            SrcPlayDataSaveFixture.Run(() => f.Src.RestoreData(Sjis(sample), SRCSaveKind.Suspend));

            Assert.IsTrue(f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Any(x => (string)x.Args[0] == "ロード中にエラーが発生しました"));
            Assert.IsTrue(f.Gui.IsTerminated);
        }

        // ── クリアデータ（行をそのまま書いた見本） ──

        private static string[] ClearSample(string[] head, string unitId = "試験機:1", bool items = true, params string[] variables)
        {
            return ClearSample(head, unitId, items, "試験フレーム_1", variables);
        }

        private static string[] ClearSample(string[] head, string unitId, bool items, string unitItem, string[] variables)
        {
            var lines = new List<string>(head)
            {
                // 次ステージ, TotalTurn, Money, パーツ用のダミー
                "\"stage2.eve\"",
                "42",
                "12345",
                "0",
            };
            lines.Add((2 + variables.Length).ToString(System.Globalization.CultureInfo.InvariantCulture));
            lines.Add("\"次ステージ\",\"\"");
            lines.Add("\"クリア数\",\"4\"");
            lines.AddRange(variables);
            lines.AddRange(new[]
            {
                // パイロット
                "2",
                "\"試験員アキ\"",
                "5,120",
                "\"" + unitId + "\"",
                "\"試験員ハル\"",
                "3,0",
                "\"離脱\"",
                // ユニット
                "3",
                "1",
                "\"試験機\"",
                "\"" + unitId + "\",2,\"待機\"",
                "0",
                "1",
                "\"試験員アキ\"",
                "0",
                "1",
                "\"" + unitItem + "\"",
            });
            if (items)
            {
                lines.AddRange(new[]
                {
                    "1",
                    "1",
                    "\"試験フレーム\"",
                    "\"試験フレーム_1\",\"" + unitId + "\"",
                });
            }
            return lines.ToArray();
        }

        private static SrcPlayDataSaveFixture Load(string[] sample)
        {
            var f = new SrcPlayDataSaveFixture();
            WriteData(f);
            f.Src.LoadData(Sjis(sample));
            Assert.IsFalse(f.Gui.IsTerminated, string.Join("\n", f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Select(x => x.Args[0])));
            return f;
        }

        [TestMethod]
        public void LoadData_Ver2233_RestoresState()
        {
            using var f = Load(ClearSample(new[] { "20233", "0" }));
            var src = f.Src;

            Assert.AreEqual("stage2.eve", src.Expression.GetValueAsString("次ステージ"));
            Assert.AreEqual(42, src.TotalTurn);
            Assert.AreEqual(12345, src.Money);
            Assert.AreEqual("4", src.Event.GlobalVariableList["クリア数"].StringValue);

            var aki = src.PList.Item("試験員アキ");
            Assert.AreEqual(5, aki.Level);
            Assert.AreEqual(120, aki.Exp);
            Assert.AreEqual("味方", aki.Party);
            Assert.AreEqual("試験機:1", aki.Unit.ID);
            var haru = src.PList.Item("試験員ハル");
            Assert.IsTrue(haru.Away);
            Assert.IsNull(haru.Unit);

            var u = src.UList.Item("試験機:1");
            Assert.AreEqual(3, src.UList.IDCount);
            Assert.AreEqual(2, u.Rank);
            Assert.AreEqual("待機", u.Status);
            Assert.AreEqual("味方", u.Party0);
            Assert.AreEqual("試験員アキ", u.MainPilot().ID);
            // 全回復した状態（ランクと装備したアイテムを足した最大値）
            Assert.AreEqual(u.MaxHP, u.HP);
            Assert.IsTrue(u.MaxHP > 3500);

            var item = src.IList.Item("試験フレーム_1");
            Assert.AreSame(u, item.Unit);
            Assert.AreSame(item, u.ItemList.Single());
            // ローカルデータ（エリアス）も読む
            Assert.IsTrue(src.ALDList.IsDefined("作品エリアス"));
        }

        [TestMethod]
        public void LoadData_WithoutVersion_FirstValueIsTitleCount()
        {
            // 版を書いていない古いクリアデータは、最初の値が作品の数
            using var f = Load(ClearSample(new[] { "0" }));

            Assert.AreEqual(12345, f.Src.Money);
            Assert.AreEqual("試験機:1", f.Src.PList.Item("試験員アキ").Unit.ID);
        }

        [TestMethod]
        public void LoadData_Before10700_ConvertsUnitId()
        {
            // 版 10700 より前のユニットのＩＤは「名前+数」。「名前:数」に変える
            using var f = Load(ClearSample(new[] { "10600", "0" }, unitId: "試験機1"));

            Assert.IsTrue(f.Src.UList.IsDefined2("試験機:1"));
            Assert.AreEqual("試験機:1", f.Src.PList.Item("試験員アキ").Unit.ID);
        }

        [TestMethod]
        public void LoadData_WithoutItems_CreatesItemsFromItemData()
        {
            // アイテムを書いていない古いクリアデータ（ファイルの終わり）。
            // ユニットのアイテムが IList に無く、アイテムデータの名前なら、作って付ける（Units.LoadLinkInfo）
            using var f = Load(ClearSample(new[] { "20233", "0" }, "試験機:1", false, "試験フレーム", new string[0]));

            var u = f.Src.UList.Item("試験機:1");
            Assert.AreEqual("試験フレーム", u.ItemList.Single().Name);
            Assert.AreSame(u, f.Src.IList.List.Single().Unit);
        }

        [TestMethod]
        public void LoadData_WithoutItems_DropsUnknownItemId()
        {
            // IList にもアイテムデータにも無いＩＤのアイテムは付けない
            using var f = Load(ClearSample(new[] { "20233", "0" }, items: false));

            Assert.AreEqual(0, f.Src.UList.Item("試験機:1").ItemList.Count);
            Assert.AreEqual(0, f.Src.IList.List.Count);
        }

        [TestMethod]
        public void LoadData_OldAbilityVariables_AreConverted()
        {
            // SetSkill の変数（Ability(…)）の読み替え（版 10724・10730・10731・10732・20027 より前）
            using var f = Load(ClearSample(new[] { "10723", "0" }, "試験機:1", true,
                "\"Ability(試験員アキ,作品エリアス)\",\"1\"",
                "\"Ability(試験員ハル,非表示)\",\"1\"",
                "\"Ability(試験員ハル)\",\"非表示 底力 底力\""));
            var vars = f.Src.Event.GlobalVariableList;

            // エリアスの最初の能力の名前に置き換え、元の名前を値に足す（10724）
            Assert.IsFalse(vars.ContainsKey("Ability(試験員アキ,作品エリアス)"));
            Assert.AreEqual("1 作品エリアス", vars["Ability(試験員アキ,バリア)"].StringValue);
            // 個々の能力から、能力の一覧の変数を作る（10730）
            Assert.AreEqual("バリア", vars["Ability(試験員アキ)"].StringValue);
            // 非表示の能力は読まない（10731）。一覧から非表示と重複を除く（10732）
            Assert.IsFalse(vars.ContainsKey("Ability(試験員ハル,非表示)"));
            Assert.AreEqual("底力", vars["Ability(試験員ハル)"].StringValue);
        }

        [TestMethod]
        public void LoadData_Before20027_FixesAliasExplanationLevel()
        {
            // エリアスの解説が無効になる不具合（版 20027 より前）。レベル 0 の解説は DEFAULT_LEVEL にする
            using var f = Load(ClearSample(new[] { "20026", "0" }, "試験機:1", true,
                "\"Ability(試験員アキ,バリア)\",\"0 解説 説明の文\""));

            Assert.AreEqual("-1000 解説 説明の文", f.Src.Event.GlobalVariableList["Ability(試験員アキ,バリア)"].StringValue);
        }

        [TestMethod]
        public void LoadData_UndefinedPilot_ShowsMessageAndTerminates()
        {
            // 元の SRC はメッセージを出して終わる（「ロード中にエラーが発生しました」は出さない）
            using var f = new SrcPlayDataSaveFixture();
            WriteData(f);
            var sample = ClearSample(new[] { "20233", "0" }).Select(x => x == "\"試験員ハル\"" ? "\"いない人\"" : x).ToArray();

            SrcPlayDataSaveFixture.Run(() => f.Src.LoadData(Sjis(sample)));

            CollectionAssert.AreEqual(new[] { "いない人のデータが定義されていません" },
                f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Select(x => (string)x.Args[0]).ToArray());
            Assert.IsTrue(f.Gui.IsTerminated);
        }

        // ── SRC# の状態から元の SRC の形式で書いて読む（VbSaveDataWriter） ──

        /// <summary>中断の前の状態を作る（Ride・アイテム・特殊状態・スペシャルパワー・格納・他形態の無いユニット）</summary>
        private static void Prepare(SrcPlayDataSaveFixture f)
        {
            WriteData(f);
            var src = f.Src;
            src.TDList.Load(f.Write("Data/System/terrain.txt", "0", "平地, plain", "陸, 1, 0, 0", ""));
            src.LoadLocalData();
            var path = Path.Combine(f.ScenarioDir, "test.eve");
            src.ScenarioFileName = path;
            src.Event.LoadEventData(path, "");
            src.Map.SetMapSize(4, 4);
            src.Map.MapData.All.ToList().ForEach(cell => cell.Restore(src));
            src.Map.MapFileName = Path.Combine(f.ScenarioDir, "test.map");
            src.Turn = 2;
            src.TotalTurn = 8;
            src.Money = 777;
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            src.Expression.DefineGlobalVariable("文");
            src.Expression.SetVariableAsString("文", "a,b");
            src.Expression.DefineLocalVariable("数");
            src.Expression.SetVariableAsDouble("数", 2.5);

            var aki = src.PList.Add("試験員アキ", 7, "味方");
            var haru = src.PList.Add("試験員ハル", 4, "敵");
            var u1 = src.UList.Add("試験機", 1, "味方");
            var u2 = src.UList.Add("試験機", 0, "敵");
            aki.Ride(u1);
            haru.Ride(u2);
            u1.AddItem(src.IList.Add("試験フレーム"));
            u1.StandBy(1, 2);
            u2.StandBy(4, 4);
            u1.AddCondition("攻撃力ＵＰ", 2);
            u1.MakeSpecialPowerInEffect("集中");
            u1.Weapon(1).SetBullet(2);
            u1.HP = 1234;
            u1.EN = 55;
            aki.SP = 12;
            aki.Morale = 130;
            src.Event.colEventLabelList.List.First(x => x.Data == "ターン 2 味方").Enable = false;
        }

        [TestMethod]
        [Timeout(20000)]
        [DataRow(20233, false)]
        [DataRow(20400, true)]
        public void DumpThenRestore_KeepsState(int version, bool layer)
        {
            using var f = new SrcPlayDataSaveFixture();
            Prepare(f);
            var bytes = new VbSaveDataWriter { Version = version, WriteLayer = layer }.DumpData(f.Src).ToBytes();

            f.NewSession();
            WriteData(f);
            f.Src.RestoreData(new MemoryStream(bytes), SRCSaveKind.Suspend);
            var src = f.Src;

            Assert.IsFalse(f.Gui.IsTerminated);
            Assert.AreEqual(2, src.Turn);
            Assert.AreEqual(8, src.TotalTurn);
            Assert.AreEqual(777, src.Money);
            Assert.AreEqual("a,b", src.Expression.GetValueAsString("文"));
            Assert.AreEqual("2.5", src.Event.LocalVariableList["数"].StringValue);
            Assert.IsFalse(src.Event.colEventLabelList.List.First(x => x.Data == "ターン 2 味方").Enable);

            var aki = src.PList.Item("試験員アキ");
            Assert.AreEqual(7, aki.Level);
            Assert.AreEqual(12, aki.SP);
            Assert.AreEqual(130, aki.Morale);
            var u1 = aki.Unit;
            Assert.AreEqual(1, u1.Rank);
            Assert.AreEqual(1, u1.x);
            Assert.AreEqual(2, u1.y);
            Assert.AreEqual(1234, u1.HP);
            Assert.AreEqual(55, u1.EN);
            Assert.AreEqual(2, u1.Weapon(1).Bullet());
            Assert.AreEqual("試験フレーム", u1.ItemList.Single().Name);
            Assert.IsTrue(u1.IsConditionSatisfied("攻撃力ＵＰ"));
            Assert.IsTrue(u1.IsSpecialPowerInEffect("集中"));
            Assert.AreSame(u1, src.Map.MapDataForUnit[1, 2]);
            Assert.AreEqual("敵", src.PList.Item("試験員ハル").Unit.Party0);
            Assert.AreEqual(4, src.Map.MapWidth);
        }

        [TestMethod]
        public void SaveThenLoad_KeepsState()
        {
            using var f = new SrcPlayDataSaveFixture();
            Prepare(f);
            f.Src.PList.Item("試験員アキ").Exp = 300;
            var bytes = new VbSaveDataWriter().SaveData(f.Src).ToBytes();

            f.NewSession();
            WriteData(f);
            f.Src.LoadData(new MemoryStream(bytes));
            var src = f.Src;

            Assert.IsFalse(f.Gui.IsTerminated);
            Assert.AreEqual("next.eve", src.Expression.GetValueAsString("次ステージ"));
            Assert.AreEqual(777, src.Money);
            var aki = src.PList.Item("試験員アキ");
            Assert.AreEqual(7, aki.Level);
            Assert.AreEqual(300, aki.Exp);
            Assert.AreEqual("試験フレーム", aki.Unit.ItemList.Single().Name);
            Assert.AreEqual(1, aki.Unit.Rank);
        }

        // ── 種類の判定（拡張子 .src） ──

        [TestMethod]
        public void IsClearSaveData_DecidesByContent()
        {
            byte[] Bytes(params string[] lines) => Sjis(lines).ToArray();

            Assert.IsTrue(SRC.IsClearSaveData(Bytes("20233", "1", "\"作品\"")));
            Assert.IsFalse(SRC.IsClearSaveData(Bytes("20233", "\"test.eve\"")));
            // 版を書いていない古い形式
            Assert.IsTrue(SRC.IsClearSaveData(Bytes("0", "\"stage2.eve\"")));
            Assert.IsFalse(SRC.IsClearSaveData(Bytes("\"test.eve\"", "0")));
            // SRC# の JSON は Kind で決める
            Assert.IsTrue(SRC.IsClearSaveData(System.Text.Encoding.UTF8.GetBytes("{\"Kind\":0}")));
            Assert.IsFalse(SRC.IsClearSaveData(System.Text.Encoding.UTF8.GetBytes("{\"Kind\":1}")));
        }

        [TestMethod]
        public void ConvertUnitID_InsertsColonBeforeNumber()
        {
            Assert.AreEqual("試験機:12", SRC.ConvertUnitID("試験機12"));
            Assert.AreEqual("試験機:1", SRC.ConvertUnitID("試験機:1"));
            Assert.AreEqual("機体2号:3", SRC.ConvertUnitID("機体2号3"));
        }

        [TestMethod]
        public void Reader_ReadsLikeVbInput()
        {
            var r = new VbSaveDataReader("\"a,b\",12, x y ,#TRUE#\r\n\"c\"\r\nrest of \"line\"\r\n");
            Assert.AreEqual("a,b", r.Input());
            Assert.AreEqual(12, r.InputInt());
            Assert.AreEqual("x y", r.Input());
            Assert.IsTrue(r.InputBool());
            Assert.AreEqual("c", r.Input());
            Assert.AreEqual("rest of \"line\"", r.LineInput());
            Assert.IsTrue(r.EOF);
            Assert.Throws<EndOfStreamException>(() => r.Input());
        }
    }
}
