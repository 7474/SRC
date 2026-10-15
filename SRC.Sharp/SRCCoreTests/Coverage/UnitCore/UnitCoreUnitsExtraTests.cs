using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitCore
{
    /// <summary>
    /// Units.cs（VB: Units.cls）の Add の形態追加・Update・Clean・自動変形チェックの追加テスト
    /// </summary>
    [TestClass]
    public class UnitCoreUnitsExtraTests
    {
        private static string UnitBlock(string name, int hp = 3000, string adaption = "AAAA", params string[] features)
        {
            var feat = features.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", features);
            return $@"
{name}
{name}, てすと, 汎用, 1, 2
陸, 5, M, 1000, 100
{feat}
{hp}, 100, 800, 90
{adaption}, test.bmp
テストソード, 1500, 1, 1, +15, -, -, -, AAAA, +10, 武
";
        }

        private static SRC CreateSrcWithUnits(params string[] blocks)
        {
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src, string.Join("\n", blocks));
            return src;
        }

        // ──────────────────────────────────────────────
        // Add: 各種形態
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_TransformAttack_AddsResultFormAsOtherForm()
        {
            // 変形技=技名 変形先 の変形先（2番目の要素）を他形態として作成する（VB Units.cls: 変形技）
            var src = CreateSrcWithUnits(
                UnitBlock("通常型", features: "変形技=必殺変形 必殺型"),
                UnitBlock("必殺型"));

            var u = src.UList.Add("通常型", 0, "味方");

            Assert.IsNotNull(u);
            Assert.AreEqual("他形態", u.OtherForm("必殺型").Status);
            Assert.AreEqual(2, src.UList.Count());
        }

        [TestMethod]
        public void Add_TransformAttackTargetMissingOrUndefined_ReturnsNull()
        {
            // 変形技の変形先が指定されていない・定義されていない場合はエラーとして Nothing を返す
            var src = CreateSrcWithUnits(
                UnitBlock("指定なし", features: "変形技=必殺変形"),
                UnitBlock("未定義先", features: "変形技=必殺変形 未定義型"));

            Assert.IsNull(src.UList.Add("指定なし", 0, "味方"));
            Assert.IsNull(src.UList.Add("未定義先", 0, "味方"));
        }

        [TestMethod]
        public void Add_NormalModeAlone_AddsNormalForm()
        {
            // ノーマルモード=戻り先 の1番目の要素を他形態として作成する
            var src = CreateSrcWithUnits(
                UnitBlock("覚醒型", features: "ノーマルモード=通常型"),
                UnitBlock("通常型"));

            var u = src.UList.Add("覚醒型", 0, "味方");

            Assert.AreEqual("待機", u.Status);
            Assert.AreEqual("他形態", u.OtherForm("通常型").Status);
            Assert.AreSame(u, u.OtherForm("通常型").OtherForm("覚醒型"));
        }

        [TestMethod]
        public void Add_NormalModeTargetUndefined_ReturnsNull()
        {
            var src = CreateSrcWithUnits(UnitBlock("覚醒型", features: "ノーマルモード=未定義型"));
            Assert.IsNull(src.UList.Add("覚醒型", 0, "味方"));
        }

        [TestMethod]
        public void Add_PartsCombineAlone_AddsCombinedForm()
        {
            // パーツ合体=合体先 の1番目の要素を他形態として作成する
            var src = CreateSrcWithUnits(
                UnitBlock("軽装型", features: "パーツ合体=全装備型"),
                UnitBlock("全装備型"));

            var u = src.UList.Add("軽装型", 0, "味方");

            Assert.IsTrue(u.IsOtherFormDefined("全装備型"));
        }

        [TestMethod]
        public void Add_PartsCombineTargetUndefined_ReturnsNull()
        {
            var src = CreateSrcWithUnits(UnitBlock("軽装型", features: "パーツ合体=未定義型"));
            Assert.IsNull(src.UList.Add("軽装型", 0, "味方"));
        }

        [TestMethod]
        public void Add_ExchangeFeature_CreatesFormsAfterFirst()
        {
            // 換装の2番目以降の形態は他形態として作成される（1番目の扱いは不具合として別テスト）
            var src = CreateSrcWithUnits(
                UnitBlock("陸戦装備", features: "換装=宇宙装備 空戦装備"),
                UnitBlock("宇宙装備"),
                UnitBlock("空戦装備"));

            var u = src.UList.Add("陸戦装備", 0, "味方");

            Assert.AreEqual("他形態", u.OtherForm("空戦装備").Status);
        }

        [TestMethod]
        public void Add_ExchangeFirstTargetUndefined_ReturnsNull()
        {
            // VB Units.cls: For i = 1 To LLength で全換装先を検査し、未定義ならエラーで Nothing
            var src = CreateSrcWithUnits(UnitBlock("陸戦装備", features: "換装=未定義装備"));
            Assert.IsNull(src.UList.Add("陸戦装備", 0, "味方"));
        }

        // ──────────────────────────────────────────────
        // Update
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Update_UnloadsCarriedUnitsAndReturnsStoredUnitsToStandby()
        {
            // 母艦に格納されたユニットは降ろされ、格納状態のユニットは待機に戻る
            var src = CreateSrcWithUnits(UnitBlock("母艦", features: "母艦"), UnitBlock("艦載機"));
            var carrier = src.UList.Add("母艦", 0, "味方");
            var fighter = src.UList.Add("艦載機", 0, "味方");
            carrier.LoadUnit(fighter);
            fighter.Status = "格納";
            Assert.AreEqual(1, carrier.CountUnitOnBoard());

            src.UList.Update();

            Assert.AreEqual(0, carrier.CountUnitOnBoard());
            Assert.AreEqual("待機", fighter.Status);
            // 降ろされたユニットはリストに残る
            CollectionAssert.Contains(src.UList.Items.ToList(), fighter);
        }

        [TestMethod]
        public void Update_ItemsOfDeletedUnitsNoLongerExist()
        {
            // 削除されるユニット（味方以外）が装備していたアイテムは消滅し、味方の装備は残る
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            src.IDList.Add("テスト装備");
            var ally = src.UList.Add("テスト機", 0, "味方");
            var enemy = src.UList.Add("テスト機", 0, "敵");
            var allyItem = src.IList.Add("テスト装備");
            var enemyItem = src.IList.Add("テスト装備");
            ally.AddItem0(allyItem);
            enemy.AddItem0(enemyItem);

            src.UList.Update();

            Assert.IsFalse(enemyItem.Exist);
            Assert.IsTrue(allyItem.Exist);
            CollectionAssert.AreEqual(new[] { ally }, src.UList.Items.ToList());
        }

        [TestMethod]
        public void Clean_ItemsOfDiscardedUnitsNoLongerExist()
        {
            // Clean で削除されるユニットの装備アイテムも消滅する
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            src.IDList.Add("テスト装備");
            var ally = src.UList.Add("テスト機", 0, "味方");
            var enemy = src.UList.Add("テスト機", 0, "敵");
            var allyItem = src.IList.Add("テスト装備");
            var enemyItem = src.IList.Add("テスト装備");
            ally.AddItem0(allyItem);
            enemy.AddItem0(enemyItem);

            src.UList.Clean();

            Assert.IsFalse(enemyItem.Exist);
            Assert.IsTrue(allyItem.Exist);
        }

        // ──────────────────────────────────────────────
        // CheckAutoHyperMode / CheckAutoNormalMode
        // ──────────────────────────────────────────────

        private static SRC CreateMapSrc(string units, out MockGUI gui, out List<string> redraws)
        {
            var log = new List<string>();
            gui = new MockGUI
            {
                CenterHandler = (x, y) => { },
                RedrawScreenHandler = (late) => log.Add("Redraw"),
                RefreshScreenHandler = (a, b) => { },
                EraseUnitBitmapHandler = (x, y, r) => { },
                PaintUnitBitmapHandler = (u, m) => { },
                OpenMessageFormHandler = (u1, u2) => { },
                CloseMessageFormHandler = () => { },
                DisplaySysMessageHandler = (m, b) => { },
                UnlockGUIHandler = () => { },
                LockGUIHandler = () => { },
                SleepHandler = (ms, e) => { },
                UpdateScreenHandler = () => { },
                MainFormShowHandler = () => { },
                ResetDrawStringHandler = () => { },
                DisplayTelopHandler = (m) => { },
            };
            redraws = log;
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            src.Titles = new List<string>();
            UnitCoreFixture.LoadUnits(src, units);
            UnitCoreFixture.LoadPilots(src, @"
操縦者(ザコ)
操縦者, そうじゅうしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
100, 100, 100, 100, 100, 100, 普通
ＳＰなし
-.bmp, -.mid
");
            var map = src.Map;
            map.MapFileName = "test.map";
            map.MapWidth = 3;
            map.MapHeight = 3;
            map.MapData = new Src2DArray<MapCell>(3, 3);
            var land = new TerrainData { ID = 1, Name = "平地", Class = "陸", MoveCost = 2 };
            foreach (var x in map.MapData.XRange)
            {
                foreach (var y in map.MapData.YRange)
                {
                    map.MapData[x, y] = new MapCell { X = x, Y = y, UnderTerrain = land, BoxType = BoxTypes.Under };
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(3, 3);
            map.MaskData = new Src2DArray<bool>(3, 3);
            return src;
        }

        private static Unit Deploy(SRC src, string uname, int x, int y)
        {
            var u = src.UList.Add(uname, 0, "味方");
            UnitCoreFixture.Board(src, u, "操縦者(ザコ)");
            u.Update();
            u.FullRecover();
            u.StandBy(x, y);
            return u;
        }

        [TestMethod]
        public void CheckAutoHyperMode_TransformsOnlyDeployedUnitsMeetingCondition()
        {
            // 出撃中でＨＰが1/4以下のユニットだけがハイパーモードに自動変形する（待機中のユニットは対象外）
            var units = UnitBlock("通常型", features: "ハイパーモード=ハイパーモード 覚醒型 自動発動")
                + UnitBlock("覚醒型", features: "ノーマルモード=通常型");
            var src = CreateMapSrc(units, out _, out _);
            var healthy = Deploy(src, "通常型", 1, 1);
            var damaged = Deploy(src, "通常型", 3, 3);
            damaged.HP = damaged.MaxHP / 4;
            var idle = src.UList.Add("通常型", 0, "味方");
            UnitCoreFixture.Board(src, idle, "操縦者(ザコ)");
            idle.HP = 1;

            src.UList.CheckAutoHyperMode();

            Assert.AreEqual("通常型", healthy.CurrentForm().Name);
            Assert.AreEqual("覚醒型", damaged.CurrentForm().Name);
            Assert.AreEqual("出撃", damaged.CurrentForm().Status);
            Assert.AreEqual("待機", idle.Status);
            Assert.AreEqual("通常型", idle.CurrentForm().Name);
        }

        [TestMethod]
        public void CheckAutoNormalMode_ReturnsWhenCurrentTerrainIsNotEnterable()
        {
            // 現在の地形に留まれなくなったハイパーモード形態はノーマルモード先へ自動で戻る
            // （宇宙適応が無い覚醒型を宇宙に置く）
            var units = UnitBlock("通常型", adaption: "AAAA", features: "ハイパーモード=ハイパーモード 覚醒型")
                + UnitBlock("覚醒型", adaption: "AAA-", features: "ノーマルモード=通常型");
            var src = CreateMapSrc(units, out _, out _);
            var stay = Deploy(src, "覚醒型", 1, 1);
            var leave = Deploy(src, "覚醒型", 3, 3);
            src.Map.MapData[3, 3].UnderTerrain = new TerrainData { ID = 2, Name = "宇宙", Class = "宇宙", MoveCost = 2 };

            src.UList.CheckAutoNormalMode();

            Assert.AreEqual("覚醒型", stay.CurrentForm().Name);
            Assert.AreEqual("通常型", leave.CurrentForm().Name);
            Assert.AreEqual("出撃", leave.CurrentForm().Status);
        }
    }
}
