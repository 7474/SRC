using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// SituationExtension.SelectMessage のユニットテスト
    /// </summary>
    [TestClass]
    public class SituationExtensionTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private List<MessageDataItem> CreateItems(params (string situation, string message)[] entries)
        {
            var list = new List<MessageDataItem>();
            foreach (var (s, m) in entries)
            {
                list.Add(new MessageDataItem(s, m));
            }
            return list;
        }

        // ──────────────────────────────────────────────
        // 完全一致
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_ExactMatch_ReturnsMatchingItem()
        {
            var src = CreateSRC();
            var items = CreateItems(
                ("攻撃", "攻撃メッセージ"),
                ("回避", "回避メッセージ")
            );

            var result = items.SelectMessage(src, "回避");
            Assert.IsNotNull(result);
            Assert.AreEqual("回避メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_NoMatch_ReturnsNull()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃", "攻撃メッセージ"));

            var result = items.SelectMessage(src, "修理");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SelectMessage_EmptyList_ReturnsNull()
        {
            var src = CreateSRC();
            var items = new List<MessageDataItem>();

            var result = items.SelectMessage(src, "攻撃");
            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────
        // 格闘・射撃 → 攻撃 へのフォールバック
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘_MatchesExact()
        {
            var src = CreateSRC();
            var items = CreateItems(("格闘", "格闘メッセージ"));

            var result = items.SelectMessage(src, "格闘");
            Assert.IsNotNull(result);
            Assert.AreEqual("格闘メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_格闘_FallsBackTo攻撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃", "攻撃メッセージ"));

            // 格闘で一致がなければ攻撃にフォールバック
            var result = items.SelectMessage(src, "格闘");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_射撃_FallsBackTo攻撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃", "攻撃メッセージ"));

            var result = items.SelectMessage(src, "射撃");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // IsAvailable フィルタ (u=null では true)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_NullUnit_ItemsAlwaysAvailable()
        {
            var src = CreateSRC();
            var items = CreateItems(
                ("攻撃", "攻撃メッセージ1"),
                ("攻撃", "攻撃メッセージ2")
            );

            var result = items.SelectMessage(src, "攻撃", u: null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Message.StartsWith("攻撃メッセージ"));
        }

        // ──────────────────────────────────────────────
        // ユニット限定のサブシチュエーション（#555）
        // ──────────────────────────────────────────────

        private static Units.Unit CreateUnit(SRC src, string name, string unitClass)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Class = unitClass;
            var u = src.UList.Add(name, 0, "味方");
            u.Update();
            return u;
        }

        [TestMethod]
        public void SelectMessage_UnitClassSubSituation_Matches()
        {
            // 元の SRC は全シチュエーション×全サブシチュエーションで照合する。
            // ユニット名以外（ここではユニットクラス）のサブシチュエーションでも一致する
            var src = CreateSRC();
            var u = CreateUnit(src, "テスト機", "テストクラス");
            var items = CreateItems(("回避(テストクラス)", "クラス限定の回避メッセージ"));

            var result = items.SelectMessage(src, "回避", u);
            Assert.IsNotNull(result);
            Assert.AreEqual("クラス限定の回避メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_UnitNameSubSituation_Matches()
        {
            var src = CreateSRC();
            var u = CreateUnit(src, "テスト機", "テストクラス");
            var items = CreateItems(("回避(テスト機)", "機体限定の回避メッセージ"));

            var result = items.SelectMessage(src, "回避", u);
            Assert.IsNotNull(result);
            Assert.AreEqual("機体限定の回避メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // 相手限定メッセージの選ばれ方（#555）
        // ──────────────────────────────────────────────

        private static Units.Unit CreatePilotedUnit(SRC src, string name, string party)
        {
            var pd = src.PDList.Add(name + "のパイロット");
            pd.SP = 100;
            var p = src.PList.Add(name + "のパイロット", 1, party);
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            var u = src.UList.Add(name, 0, party);
            p.Ride(u);
            u.Update();
            u.Status = "出撃";
            u.x = 1;
            u.y = 1;
            return u;
        }

        private static void SetUpMap(SRC src)
        {
            // 相手のいる地形（(対地形名)）を調べるため、1 マスに地形を置く
            // SetMapSize は GUI のマップの実装を要するため、フィールドに直接置く
            src.Map.MapWidth = 1;
            src.Map.MapHeight = 1;
            src.Map.MapData = new Lib.Src2DArray<Maps.MapCell>(1, 1);
            src.Map.MapData[1, 1] = new Maps.MapCell { X = 1, Y = 1, UnderTerrain = new Models.TerrainData { Name = "平地" } };
        }

        private static int CountTargetMessages(SRC src, Units.Unit u, List<MessageDataItem> items, int trials)
        {
            // 乱数の系列を初期化する（初期化しないと Dice が同じ値を返し続ける）
            SRCCore.Lib.GeneralLib.RndSeed = 12345;
            SRCCore.Lib.GeneralLib.RndReset();
            var count = 0;
            for (var i = 0; i < trials; i++)
            {
                if (items.SelectMessage(src, "回避", u).Message == "相手限定")
                {
                    count++;
                }
            }
            return count;
        }

        [TestMethod]
        public void SelectMessage_TargetMessage_HalfByDefault()
        {
            // 既定は元の SRC と同じく、相手限定メッセージを 50% の確率で使う
            var src = CreateSRC();
            SetUpMap(src);
            var u = CreatePilotedUnit(src, "自機", "味方");
            var t = CreatePilotedUnit(src, "敵機", "敵");
            src.Commands.SelectedUnit = t;
            src.Commands.SelectedTarget = u;
            var items = CreateItems(
                ("回避(対敵機のパイロット)", "相手限定"),
                ("回避", "汎用1"), ("回避", "汎用2"), ("回避", "汎用3"));

            var count = CountTargetMessages(src, u, items, 2000);
            Assert.IsTrue(count > 850 && count < 1150, $"相手限定メッセージ {count}/2000");
        }

        [TestMethod]
        public void SelectMessage_TargetMessage_EvenWithOption()
        {
            // Option「相手限定メッセージ等確率」では、相手限定メッセージも汎用メッセージと同じく 1 件ずつ等確率
            var src = CreateSRC();
            src.Expression.DefineGlobalVariable("Option(" + SituationExtension.TargetMessageEvenOption + ")");
            SetUpMap(src);
            var u = CreatePilotedUnit(src, "自機", "味方");
            var t = CreatePilotedUnit(src, "敵機", "敵");
            src.Commands.SelectedUnit = t;
            src.Commands.SelectedTarget = u;
            var items = CreateItems(
                ("回避(対敵機のパイロット)", "相手限定"),
                ("回避", "汎用1"), ("回避", "汎用2"), ("回避", "汎用3"));

            // 4 件から等確率なので約 25%
            var count = CountTargetMessages(src, u, items, 2000);
            Assert.IsTrue(count > 380 && count < 620, $"相手限定メッセージ {count}/2000");
        }
    }
}
