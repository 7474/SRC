using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Coverage.Tests.UnitsCoverage;
using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// SRC# 独自の GeneralLib.ToList を使っていた箇所を、元の SRC の書き方（ListLength・ListIndex・ListTail・LLength・LIndex）に
    /// そろえたことを確かめるテスト（7474/SRC#1534）。
    /// 元の SRC の ListLength は、括弧・クォートが閉じていないか、閉じ括弧が多すぎると -1 を返す。
    /// </summary>
    [TestClass]
    public class ListParseCallersVBMatchTests
    {
        // ──────────────────────────────────────────────
        // ForEach（CmdData.cls ExecForEachCmd）
        // ──────────────────────────────────────────────

        private static SRC CreateEventSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        private static int ExecForEach(SRC src, string list)
        {
            src.Expression.DefineGlobalVariable("リスト");
            src.Expression.SetVariableAsString("リスト", list);
            var line = "ForEach x In リスト";
            var parser = new CmdParser();
            var lines = new[] { line, "Next" };
            var cmds = new List<CmdData>();
            for (var i = 0; i < lines.Length; i++)
            {
                var data = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(data);
                var cmd = parser.Parse(src, data);
                src.Event.EventCmd.Add(cmd);
                cmds.Add(cmd);
            }
            return cmds[0].Exec();
        }

        [TestMethod]
        public void ForEach_List_SplitsLikeListIndex()
        {
            // VB: ForEachSet(i) = ListIndex(buf, i)。括弧の中の空白では分けない
            var src = CreateEventSrc();
            var ret = ExecForEach(src, "a (b c) d");
            Assert.AreEqual(1, ret);
            CollectionAssert.AreEqual(new[] { "a", "(b c)", "d" }, src.Event.ForEachSet.ToList());
        }

        [TestMethod]
        public void ForEach_ListWithUnclosedParen_StopsWithEventError()
        {
            // VB: ListLength(buf) が -1 なので ReDim ForEachSet(-1) が実行時エラーになり、
            // Exec の ErrorHandler が「イベントデータが不正です」を出してイベントを止める（-1 を返す）
            var src = CreateEventSrc();
            var ret = ExecForEach(src, "a (b");
            Assert.AreEqual(-1, ret);
        }

        [TestMethod]
        public void ForEach_ListWithExtraCloseParen_StopsWithEventError()
        {
            // VB: 閉じ括弧が多すぎても ListLength は -1
            var src = CreateEventSrc();
            var ret = ExecForEach(src, "a b) c");
            Assert.AreEqual(-1, ret);
        }

        // ──────────────────────────────────────────────
        // AbilityData.SetEffect・SpecialPowerData.SetEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilitySetEffect_UnclosedParen_AddsNoEffect()
        {
            // VB: TrimString elist、For i = 1 To ListLength(elist)。-1 なら効果を 1 つも追加しない
            var src = new SRC { GUI = new MockGUI() };
            var ad = new AbilityData(src);
            ad.SetEffect("回復Lv1 (x");
            Assert.AreEqual(0, ad.Effects.Count);
        }

        [TestMethod]
        public void AbilitySetEffect_TrimsTabsAndFullWidthSpaces()
        {
            // VB: TrimString はタブ・半角スペース・全角スペースを前後から取り除く
            var src = new SRC { GUI = new MockGUI() };
            var ad = new AbilityData(src);
            ad.SetEffect("\t　回復Lv1 補給　\t");
            Assert.AreEqual(2, ad.Effects.Count);
            Assert.AreEqual("回復", ad.Effects[0].Name);
            Assert.AreEqual("補給", ad.Effects[1].Name);
        }

        [TestMethod]
        public void SpecialPowerLoad_EffectWithUnclosedParen_StopsLoadingWithoutMessage()
        {
            // VB: SetEffect の ReDim strEffectType(ListLength(elist)) が ReDim (-1) で実行時エラーになり、
            // Load の ErrorHandler が原因を書かない（err_msg が空の）データエラーとして読み込みを打ち切る
            var src = new SRC { GUI = new MockGUI() };
            var data = "熱血, ねっけつ\n熱, 40, 自分, 攻撃, -, -, 熱血アニメ\nダメージ増加Lv1 (x\n解説\n";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var ex = Assert.ThrowsExactly<InvalidSrcDataException>(() => src.SPDList.Load("sp.txt", stream));
                Assert.AreEqual("", ex.InvalidDataList.Single().msg);
                Assert.AreEqual("熱血", ex.InvalidDataList.Single().dname);
            }
        }

        // ──────────────────────────────────────────────
        // 改造のときの分離形態のランク（Intermission.bas RankUpCommand）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUp_SeparatedUnits_SplitLikeLIndex()
        {
            // VB: For i = 2 To LLength(buf)、UList.IsDefined(LIndex(buf, i))。
            // LIndex は空白だけで分けるので、「`x」の後ろの「相棒機」も分離形態として探す
            var h = new UnitLookupTestHelper();
            h.DefineUnit("合体機", "分離=分離 `x 相棒機");
            h.DefineUnit("相棒機");
            var u = h.Create("合体機", null);
            var part = h.Create("相棒機", null);

            u.RankUp();

            Assert.AreEqual(1, u.Rank);
            Assert.AreEqual(1, part.Rank);
        }

        // ──────────────────────────────────────────────
        // 合体技のパートナー（Unit.cls CombinationPartner）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CombinationPartner_PartnerList_SplitLikeLIndex()
        {
            // VB: clist = ListTail(FeatureData(i), 2)、cnum = LLength(clist)、uname = LIndex(clist, i)。
            // パートナーは空白だけで分けるので、閉じていない括弧を含む名前「相棒機(改」もそのまま 1 体として探す
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク");
            h.DefinePilot("ゾルグ");
            h.DefineUnit("主役機", "合体技=ダブルアタック 相棒機(改 僚機",
                "ダブルアタック, 3000, 1, 1, +0, -, 50, -, AAAA, +0, 合");
            h.DefineUnit("相棒機(改");
            h.DefineUnit("僚機");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            var partner1 = h.Place("相棒機(改", "ベルク", 4, 3);
            var partner2 = h.Place("僚機", "ゾルグ", 2, 3);

            var result = leader.Weapon(1).CombinationPartner();

            CollectionAssert.AreEquivalent(new[] { partner1, partner2 }, result.ToArray());
        }
    }
}
