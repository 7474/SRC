using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ForEach の パイロット一覧(…)・ユニット一覧(…) の並び順を元の SRC（CmdData.cls ExecForEachCmd）と比べるテスト。
    /// VB は選択ソートで、最大（名称は最小）の要素を先頭と入れ替えるので、同じ値の要素の順は保たれないことがある。
    /// 名称の比較は StrComp(…, vbTextCompare)（大文字・小文字、全角・半角、ひらがな・カタカナを区別しない）。
    /// </summary>
    [TestClass]
    public class ForEachListSortVBTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        private static IList<string> ExecForEach(SRC src, string line)
        {
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
            cmds[0].Exec();
            return src.Event.ForEachSet.ToList();
        }

        private static Units.Unit CreateUnit(SRC src, string name, int rank, string kana)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.KanaName = kana;
            var u = src.UList.Add(name, rank, "味方");
            u.Update();
            u.FullRecover();
            u.Status = "待機";
            return u;
        }

        [TestMethod]
        public void UnitListByRank_TiesAreReorderedLikeVBSelectionSort()
        {
            // VB: ランク [1, 1, 2] の A, B, C は、C を先頭と入れ替えて C, B, A になる（A と B の順が入れ替わる）
            var src = CreateSrc();
            var a = CreateUnit(src, "ユニットA", 1, "あ");
            var b = CreateUnit(src, "ユニットB", 1, "い");
            var c = CreateUnit(src, "ユニットC", 2, "う");

            var result = ExecForEach(src, "ForEach x In ユニット一覧(ランク)");

            CollectionAssert.AreEqual(new[] { c.ID, b.ID, a.ID }, result.ToList());
        }

        [TestMethod]
        public void UnitListByName_TiesAreReorderedLikeVBSelectionSort()
        {
            // VB: 読み仮名 [い, い, あ] の A, B, C は、C を先頭と入れ替えて C, B, A になる
            var src = CreateSrc();
            var a = CreateUnit(src, "ユニットA", 0, "い");
            var b = CreateUnit(src, "ユニットB", 0, "い");
            var c = CreateUnit(src, "ユニットC", 0, "あ");

            var result = ExecForEach(src, "ForEach x In ユニット一覧(名称)");

            CollectionAssert.AreEqual(new[] { c.ID, b.ID, a.ID }, result.ToList());
        }

        [TestMethod]
        public void UnitListByName_HiraganaAndKatakanaAreEqual()
        {
            // VB: StrComp(…, vbTextCompare) は「ア」と「あ」を同じとみなすので、入れ替えない
            var src = CreateSrc();
            var a = CreateUnit(src, "ユニットA", 0, "ア");
            var b = CreateUnit(src, "ユニットB", 0, "あ");

            var result = ExecForEach(src, "ForEach x In ユニット一覧(名称)");

            CollectionAssert.AreEqual(new[] { a.ID, b.ID }, result.ToList());
        }

        [TestMethod]
        public void PilotListByLevel_TiesAreReorderedLikeVBSelectionSort()
        {
            // VB: key = 500 * Level + Exp。レベル [1, 1, 2] の A, B, C は C, B, A になる
            var src = CreateSrc();
            src.PDList.Add("パイロットA");
            src.PDList.Add("パイロットB");
            src.PDList.Add("パイロットC");
            var a = src.PList.Add("パイロットA", 1, "味方");
            var b = src.PList.Add("パイロットB", 1, "味方");
            var c = src.PList.Add("パイロットC", 2, "味方");

            var result = ExecForEach(src, "ForEach x In パイロット一覧(レベル)");

            CollectionAssert.AreEqual(new[] { c.ID, b.ID, a.ID }, result.ToList());
        }
    }
}
