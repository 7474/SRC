using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.EventsUnitList
{
    /// <summary>
    /// Organize コマンド（OrganizeCmd.cs / VB: CmdData.ExecOrganizeCmd）のテスト。
    /// 出撃候補の絞り込み、レベル順の並べ替え、選択したユニットの出撃を確認する。
    /// </summary>
    [TestClass]
    public class EventsUnitListOrganizeCmdTests
    {
        private const int MapSize = 10;

        private static (SRC src, HeadlessGUI gui) CreateSrc(string terrainClass = "陸", string terrainName = "平地")
        {
            var src = new SRC();
            var gui = new HeadlessGUI();
            gui.Install(src);
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();

            // 地形を指定したマップを直接用意する
            var terrain = new TerrainData { ID = 0, Name = terrainName, Class = terrainClass };
            var map = src.Map;
            map.MapFileName = "test.map";
            map.MapWidth = MapSize;
            map.MapHeight = MapSize;
            map.MapData = new Src2DArray<MapCell>(MapSize, MapSize);
            for (var x = 1; x <= MapSize; x++)
            {
                for (var y = 1; y <= MapSize; y++)
                {
                    map.MapData[x, y] = new MapCell
                    {
                        X = x,
                        Y = y,
                        BoxType = BoxTypes.Under,
                        UnderTerrain = terrain,
                    };
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(MapSize, MapSize);
            map.MaskData = new Src2DArray<bool>(MapSize, MapSize);
            map.TotalMoveCost = new int[MapSize + 2, MapSize + 2];
            map.PointInZOC = new int[MapSize + 2, MapSize + 2];
            return (src, gui);
        }

        private static int Exec(SRC src, string cmdText)
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd.Exec();
        }

        private static Unit AddUnit(SRC src, string name, int level = 1, string party = "味方",
            string uclass = "汎用", string size = "M", string transportation = "陸", string adaption = "AAAA",
            bool withPilot = true, params string[] features)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            ud.Class = uclass;
            ud.Size = size;
            ud.Transportation = transportation;
            ud.Adaption = adaption;
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            var u = src.UList.Add(name, 0, party);
            u.Status = "待機";
            if (withPilot)
            {
                var pname = name + "のパイロット";
                var pd = src.PDList.Add(pname);
                pd.SP = 10;
                pd.Adaption = "AAAA";
                var p = src.PList.Add(pname, level, party);
                p.Ride(u);
            }
            u.Update();
            return u;
        }

        private static ListBoxArgs CaptureOrganizeList(SRC src, HeadlessGUI gui, string cmdText, out int maxNum, out int result)
        {
            ListBoxArgs captured = null;
            var max = -1;
            gui.Handle(nameof(IGUI.MultiSelectListBox), args =>
            {
                captured = (ListBoxArgs)args[0];
                max = (int)args[1];
                // 何も選ばずに決定する
                return 1;
            });
            result = Exec(src, cmdText);
            maxNum = max;
            return captured;
        }

        [TestMethod]
        public void Organize_ListsOnlyPilotedFriendlyStandbyUnits()
        {
            // 候補は「味方」「待機」かつパイロットが乗っているユニットのみ
            var (src, gui) = CreateSrc();
            var ok = AddUnit(src, "味方機");
            AddUnit(src, "敵機", party: "敵");
            AddUnit(src, "無人機", withPilot: false);
            var deployed = AddUnit(src, "出撃済機");
            deployed.Status = "出撃";

            var args = CaptureOrganizeList(src, gui, "Organize 3 2 2", out var maxNum, out var result);

            Assert.AreEqual(1, result);
            Assert.IsNotNull(args);
            Assert.AreEqual("出撃ユニット選択", args.lb_caption);
            Assert.AreEqual(3, maxNum, "選択できる最大数は第2引数");
            CollectionAssert.AreEqual(new[] { ok.ID }, args.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        public void Organize_UnitShortOfRequiredPilots_IsExcludedUnlessSoloAllowed()
        {
            // 必要パイロット数(2人乗り)に満たないユニットは「１人乗り可能」がない限り除外
            var (src, gui) = CreateSrc();
            var twoSeat = AddUnit(src, "複座機");
            twoSeat.Data.PilotNum = 2;
            var solo = AddUnit(src, "複座機改", features: "１人乗り可能");
            solo.Data.PilotNum = 2;
            solo.Update();

            var args = CaptureOrganizeList(src, gui, "Organize 2 1 1", out _, out _);

            CollectionAssert.AreEqual(new[] { solo.ID }, args.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        public void Organize_NoCandidate_DoesNotOpenList()
        {
            // 候補が一つもなければリストを表示せずに次の行へ進む
            var (src, gui) = CreateSrc();
            AddUnit(src, "敵機", party: "敵");

            var called = false;
            gui.Handle(nameof(IGUI.MultiSelectListBox), _ => { called = true; return 1; });
            var result = Exec(src, "Organize 1 1 1");

            Assert.IsFalse(called);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Organize_InvalidUnitCount_IsError()
        {
            // ユニット数が1未満はエラー
            var (src, gui) = CreateSrc();
            AddUnit(src, "味方機");

            Assert.AreEqual(-1, Exec(src, "Organize 0 1 1"));
            Assert.IsFalse(gui.CallsOf(nameof(IGUI.MultiSelectListBox)).Any());
        }

        [TestMethod]
        public void Organize_SpaceMap_ExcludesUnitsWithoutSpaceAdaption()
        {
            // 宇宙マップでは宇宙適応(適応の4文字目)が「-」のユニットは出撃できない
            var (src, gui) = CreateSrc("宇宙", "宇宙");
            var spaceOk = AddUnit(src, "宇宙機", adaption: "AAAA");
            AddUnit(src, "陸戦機", adaption: "AAA-");

            var args = CaptureOrganizeList(src, gui, "Organize 2 1 1", out _, out _);

            CollectionAssert.AreEqual(new[] { spaceOk.ID }, args.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        public void Organize_GroundMap_ExcludesSpaceOnlyUnits()
        {
            // 宇宙専用（移動タイプ「宇宙」）のユニットは宇宙以外では出撃できない
            var (src, gui) = CreateSrc();
            var ground = AddUnit(src, "陸戦機");
            AddUnit(src, "宇宙専用機", transportation: "宇宙");

            var args = CaptureOrganizeList(src, gui, "Organize 2 1 1", out _, out _);

            CollectionAssert.AreEqual(new[] { ground.ID }, args.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        public void Organize_SkyMap_RequiresFlyingUnits()
        {
            // マップの左上・中央・右下が「空」なら空中マップで、飛行できるユニットのみ出撃できる
            var (src, gui) = CreateSrc("空中", "空");
            var flyer = AddUnit(src, "飛行機", transportation: "空");
            AddUnit(src, "戦車", transportation: "陸");

            var args = CaptureOrganizeList(src, gui, "Organize 2 1 1", out _, out _);

            CollectionAssert.AreEqual(new[] { flyer.ID }, args.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        public void Organize_ShipClassFilters()
        {
            // 「母艦ユニット」は母艦のみ、「通常ユニット」は母艦以外のみ
            var (src, gui) = CreateSrc();
            var ship = AddUnit(src, "戦艦", features: "母艦");
            var normal = AddUnit(src, "戦闘機");

            var shipArgs = CaptureOrganizeList(src, gui, "Organize 2 1 1 母艦ユニット", out _, out _);
            CollectionAssert.AreEqual(new[] { ship.ID }, shipArgs.Items.Select(x => x.ListItemID).ToArray());

            var normalArgs = CaptureOrganizeList(src, gui, "Organize 2 1 1 通常ユニット", out _, out _);
            CollectionAssert.AreEqual(new[] { normal.ID }, normalArgs.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        public void Organize_SizeFilter_ExcludesLargerUnits()
        {
            // サイズ指定「M」は M 以下 (M/S/SS) のみ。L, LL, XL は除外
            var (src, gui) = CreateSrc();
            AddUnit(src, "XL機", size: "XL");
            AddUnit(src, "LL機", size: "LL");
            AddUnit(src, "L機", size: "L");
            var m = AddUnit(src, "M機", size: "M");
            var s = AddUnit(src, "S機", size: "S");

            var args = CaptureOrganizeList(src, gui, "Organize 5 1 1 M", out _, out _);

            CollectionAssert.AreEquivalent(new[] { m.ID, s.ID }, args.Items.Select(x => x.ListItemID).ToArray());

            // 「LL」は XL のみ除外
            var llArgs = CaptureOrganizeList(src, gui, "Organize 5 1 1 LL", out _, out _);
            Assert.AreEqual(4, llArgs.Items.Count);
            Assert.IsFalse(llArgs.Items.Any(x => src.UList.Item(x.ListItemID).Size == "XL"));
        }

        [TestMethod]
        public void Organize_UnitClassList_MatchesAnyOfSpecifiedClasses()
        {
            // 第5引数以降はユニットクラスのリストとして扱い、いずれかに一致するユニットのみ
            var (src, gui) = CreateSrc();
            var real = AddUnit(src, "リアル機", uclass: "リアル");
            var super = AddUnit(src, "スーパー機", uclass: "スーパー");
            AddUnit(src, "艦載機", uclass: "戦闘機");

            var args = CaptureOrganizeList(src, gui, "Organize 3 1 1 リアル スーパー", out _, out _);

            CollectionAssert.AreEquivalent(new[] { real.ID, super.ID }, args.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        public void Organize_SortsByPilotLevelDescending()
        {
            // パイロットのレベルにばらつきがあるときはレベルの高い順に並べる
            var (src, gui) = CreateSrc();
            var lv3 = AddUnit(src, "機体A", level: 3);
            var lv10 = AddUnit(src, "機体B", level: 10);
            var lv5 = AddUnit(src, "機体C", level: 5);

            var args = CaptureOrganizeList(src, gui, "Organize 3 1 1", out _, out _);

            CollectionAssert.AreEqual(new[] { lv10.ID, lv5.ID, lv3.ID }, args.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        public void Organize_SameLevel_KeepsUnitListOrder()
        {
            // レベルが全員同じなら並べ替えず登録順のまま
            var (src, gui) = CreateSrc();
            var a = AddUnit(src, "機体A", level: 4);
            var b = AddUnit(src, "機体B", level: 4);
            var c = AddUnit(src, "機体C", level: 4);

            var args = CaptureOrganizeList(src, gui, "Organize 3 1 1", out _, out _);

            CollectionAssert.AreEqual(new[] { a.ID, b.ID, c.ID }, args.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        [Ignore("移植の不具合: レベル順の並べ替えキーが VB の 500*Level+Exp ではなく Level+Exp になっている")]
        public void Organize_SortKeyWeighsLevelOverExperience()
        {
            // VB では並べ替えキーは 500 * Level + Exp。Lv5(経験値0) は Lv4(経験値400) より上に来る
            var (src, gui) = CreateSrc();
            var lv4 = AddUnit(src, "機体A", level: 4);
            lv4.MainPilot().Exp = 400;
            var lv5 = AddUnit(src, "機体B", level: 5);

            var args = CaptureOrganizeList(src, gui, "Organize 2 1 1", out _, out _);

            CollectionAssert.AreEqual(new[] { lv5.ID, lv4.ID }, args.Items.Select(x => x.ListItemID).ToArray());
        }

        [TestMethod]
        public void Organize_ListTextShowsUnitPilotAndLevelInColumns()
        {
            // 表示はユニット愛称(36バイト幅) + パイロット愛称(17バイト幅) + レベル(右寄せ2桁)
            var (src, gui) = CreateSrc();
            var u = AddUnit(src, "ABC", level: 7);
            u.Data.Nickname = "ABC";
            src.PDList.Item("ABCのパイロット").Nickname = "Pilot";

            var args = CaptureOrganizeList(src, gui, "Organize 1 1 1", out _, out _);

            var expected = "ABC" + new string(' ', 33) + "Pilot" + new string(' ', 12) + " 7";
            Assert.AreEqual(expected, args.Items.Single().Text);
            Assert.AreEqual("ユニット                            パイロット       Lv", args.lb_info);
        }

        [TestMethod]
        public void Organize_SelectedUnits_AreDeployedAndCountersReset()
        {
            // 選択されたユニットのみ出撃し、行動回数などのカウンタがリセットされる
            var (src, gui) = CreateSrc();
            var picked = AddUnit(src, "選抜機");
            var left = AddUnit(src, "待機機");
            picked.UsedAction = 1;
            picked.UsedSupportAttack = 2;
            picked.UsedSupportGuard = 3;
            picked.UsedSyncAttack = 4;
            picked.UsedCounterAttack = 5;

            gui.Handle(nameof(IGUI.MultiSelectListBox), a =>
            {
                var largs = (ListBoxArgs)a[0];
                largs.Items.Single(x => x.ListItemID == picked.ID).ListItemFlag = true;
                return 1;
            });
            var result = Exec(src, "Organize 1 5 5");

            Assert.AreEqual(1, result);
            Assert.AreEqual("出撃", picked.Status);
            Assert.AreEqual("待機", left.Status);
            Assert.AreEqual(0, picked.UsedAction);
            Assert.AreEqual(0, picked.UsedSupportAttack);
            Assert.AreEqual(0, picked.UsedSupportGuard);
            Assert.AreEqual(0, picked.UsedSyncAttack);
            Assert.AreEqual(0, picked.UsedCounterAttack);
            Assert.AreSame(picked, src.Map.MapDataForUnit[picked.x, picked.y]);
        }

        [TestMethod]
        public void Organize_OutOfRangeCoordinates_AreClampedIntoMap()
        {
            // 座標はマップ内に丸められる（ここでは右下隅付近に配置される）
            var (src, gui) = CreateSrc();
            var u = AddUnit(src, "選抜機");
            gui.Handle(nameof(IGUI.MultiSelectListBox), a =>
            {
                ((ListBoxArgs)a[0]).Items.Single().ListItemFlag = true;
                return 1;
            });

            Exec(src, "Organize 1 99 99");

            Assert.AreEqual("出撃", u.Status);
            Assert.AreEqual(MapSize, u.x);
            Assert.AreEqual(MapSize, u.y);
        }

        private static void SelectAll(HeadlessGUI gui)
        {
            gui.Handle(nameof(IGUI.MultiSelectListBox), a =>
            {
                foreach (var item in ((ListBoxArgs)a[0]).Items)
                {
                    item.ListItemFlag = true;
                }
                return 1;
            });
        }

        [TestMethod]
        public void Organize_DefaultFormation_LeavesGapBetweenUnits()
        {
            // オプションなしは「部隊配置」で、隣接させず1マス空けて配置する（距離が奇数の位置は使わない）
            var (src, gui) = CreateSrc();
            var a = AddUnit(src, "機体A");
            var b = AddUnit(src, "機体B");
            SelectAll(gui);

            Exec(src, "Organize 2 5 5");

            Assert.AreEqual((5, 5), (a.x, a.y));
            Assert.AreEqual(2, System.Math.Abs(b.x - 5) + System.Math.Abs(b.y - 5));
        }

        [TestMethod]
        public void Organize_DenseOption_PlacesUnitsAdjacent()
        {
            // 「密集」指定時は「部隊配置」を付けずに出撃させるので、隣接したマスに詰めて配置する。
            // オプションも第5引数以降のユニットクラス指定に含まれるため、クラス「汎用」と併記する
            var (src, gui) = CreateSrc();
            var a = AddUnit(src, "機体A");
            var b = AddUnit(src, "機体B");
            SelectAll(gui);

            Exec(src, "Organize 2 5 5 汎用 密集");

            Assert.AreEqual("出撃", b.Status);
            Assert.AreEqual((5, 5), (a.x, a.y));
            Assert.AreEqual(1, System.Math.Abs(b.x - 5) + System.Math.Abs(b.y - 5));
        }

        [TestMethod]
        public void Organize_AsyncOption_CentersMapOnDeployPointBeforeDeploying()
        {
            // 「非同期」指定時は出撃前に配置位置を画面中央にして画面を更新する
            var (src, gui) = CreateSrc();
            var a = AddUnit(src, "機体A");
            SelectAll(gui);

            Exec(src, "Organize 1 4 6 汎用 非同期");

            Assert.AreEqual("出撃", a.Status);
            var calls = gui.Calls.ToList();
            var center = calls.FindIndex(c => c.Member == nameof(IGUI.Center));
            Assert.IsTrue(center >= 0);
            CollectionAssert.AreEqual(new object[] { 4, 6 }, calls[center].Args);
            Assert.IsTrue(calls.Skip(center + 1).Any(c => c.Member == nameof(IGUI.RefreshScreen)));
        }

        [TestMethod]
        public void Organize_WithoutAsyncOption_DoesNotCenterMap()
        {
            // 「非同期」指定がなければ Organize 自身は画面の中央移動を行わない
            var (src, gui) = CreateSrc();
            AddUnit(src, "機体A");
            SelectAll(gui);

            Exec(src, "Organize 1 4 6");

            Assert.IsFalse(gui.CallsOf(nameof(IGUI.Center)).Any(c => (int)c.Args[0] == 4 && (int)c.Args[1] == 6));
        }

        [TestMethod]
        public void Organize_LifeSizeOption_ListsUnitAndLevelOnly()
        {
            // オプション「等身大基準」ではパイロット名を省き、ユニット愛称(52バイト幅) + レベルを表示する
            var (src, gui) = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(等身大基準)");
            var u = AddUnit(src, "ABC", level: 7);
            u.Data.Nickname = "ABC";

            var args = CaptureOrganizeList(src, gui, "Organize 1 1 1", out _, out _);

            Assert.AreEqual("ABC" + new string(' ', 49) + " 7", args.Items.Single().Text);
            Assert.AreEqual("ユニット                                            Lv", args.lb_info);
        }
    }
}
