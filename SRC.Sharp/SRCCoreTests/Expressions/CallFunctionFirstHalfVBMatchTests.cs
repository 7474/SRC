using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// 元の SRC（Expression.bas の CallFunction の前半。args〜isnumeric）と突き合わせたテスト
    /// </summary>
    [TestClass]
    public class CallFunctionFirstHalfVBMatchTests
    {
        private static (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return (src, gui);
        }

        private static void BuildEvent(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                src.Event.EventCmd.Add(parser.Parse(src, line));
                var trimmed = lines[i].Trim();
                if (trimmed.EndsWith(":"))
                {
                    src.Event.AddLabel(trimmed.Substring(0, trimmed.Length - 1), i);
                }
            }
        }

        private static Units.Unit CreateUnit(SRC src, string name, params string[] features)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            var u = src.UList.Add(name, 0, "味方");
            u.Update();
            u.FullRecover();
            return u;
        }

        // ── call ──

        [TestMethod]
        public void Call_AtMaxCallDepth_IsNotError()
        {
            // VB: If CallDepth > MaxCallDepth Then エラー。CallDepth = MaxCallDepth のときはまだ呼べる
            // （CallStack などは 0〜MaxCallDepth の MaxCallDepth + 1 個ある）
            var (src, _) = CreateSrc();
            BuildEvent(src, "Set a 1", "sub:", "Return 7");
            src.Event.CallDepth = Event.MaxCallDepth;

            Assert.AreEqual("7", src.Expression.GetValueAsString("Call(sub)"));
            Assert.AreEqual(0, src.DataErrors.Count);
            Assert.AreEqual(Event.MaxCallDepth, src.Event.CallDepth);
        }

        [TestMethod]
        public void Call_OverMaxCallDepth_IsError()
        {
            var (src, _) = CreateSrc();
            BuildEvent(src, "Set a 1", "sub:", "Return 7");
            src.Event.CallDepth = Event.MaxCallDepth + 1;

            src.Expression.GetValueAsString("Call(sub)");
            CollectionAssert.Contains(src.DataErrors.Select(x => x.msg).ToList(),
                Event.MaxCallDepth + "階層を越えるサブルーチンの呼び出しは出来ません");
            Assert.AreEqual(Event.MaxCallDepth, src.Event.CallDepth);
        }

        [TestMethod]
        public void Call_ArgIndexPlusPcountEqualsMaxArgIndex_IsNotError()
        {
            // VB: If ArgIndex + pcount > MaxArgIndex Then エラー。等しいときは積める（積むのは pcount - 1 個）
            var (src, _) = CreateSrc();
            BuildEvent(src, "Set a 1", "sub:", "Return 7");
            src.Event.CallDepth = 1;
            src.Event.ArgIndex = Event.MaxArgIndex - 2;

            Assert.AreEqual("7", src.Expression.GetValueAsString("Call(sub, 1)"));
            Assert.AreEqual(0, src.DataErrors.Count);
            Assert.AreEqual(Event.MaxArgIndex - 2, src.Event.ArgIndex);
        }

        [TestMethod]
        public void Call_ArgIndexPlusPcountOverMaxArgIndex_IsError()
        {
            var (src, _) = CreateSrc();
            BuildEvent(src, "Set a 1", "sub:", "Return 7");
            src.Event.CallDepth = 1;
            src.Event.ArgIndex = Event.MaxArgIndex - 1;

            src.Expression.GetValueAsString("Call(sub, 1)");
            CollectionAssert.Contains(src.DataErrors.Select(x => x.msg).ToList(),
                "サブルーチンの引数の総数が" + Event.MaxArgIndex + "個を超えています");
        }

        // ── isavailable ──

        [TestMethod]
        public void IsAvailable_OneArg_AliasUsesFirstElementType()
        {
            // VB: 引数 1 個のときは buf = ALDList.Item(buf).AliasType(1)（データの先頭が名前と同じ要素は探さない）
            var (src, _) = CreateSrc();
            var alias = src.ALDList.Add("テスト能力");
            alias.AddAlias("能力Ａ");
            alias.AddAlias("能力Ｂ=テスト能力");
            var u = CreateUnit(src, "テスト機", "能力Ａ");
            src.Event.SelectedUnitForEvent = u;

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("IsAvailable(テスト能力)"));
        }

        [TestMethod]
        public void IsAvailable_TwoArgs_AliasUsesElementWhoseDataStartsWithName()
        {
            // VB: 引数 2 個のときは LIndex(AliasData(i), 1) = buf の要素の種類（無ければ AliasType(1)）
            var (src, _) = CreateSrc();
            var alias = src.ALDList.Add("テスト能力");
            alias.AddAlias("能力Ａ");
            alias.AddAlias("能力Ｂ=テスト能力");
            CreateUnit(src, "テスト機", "能力Ａ");

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("IsAvailable(テスト機, テスト能力)"));
        }

        // ── countitem ──

        [TestMethod]
        public void CountItem_PilotNamedMisoubi_CountsPilotUnitItems()
        {
            // VB: UList.IsDefined2・PList.IsDefined を先に見る。「未装備」を数えるのはユニットにもパイロットにも当たらないときだけ
            // （パイロットに当たり、乗っているユニットが無ければ 0）
            var (src, _) = CreateSrc();
            src.PDList.Add("未装備");
            src.PList.Add("未装備", 1, "味方");
            src.IDList.Add("テストアイテム");
            src.IList.Add("テストアイテム");

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("CountItem(未装備)"));
        }

        [TestMethod]
        public void CountItem_Misoubi_CountsUnequippedItems()
        {
            var (src, _) = CreateSrc();
            src.IDList.Add("テストアイテム");
            src.IList.Add("テストアイテム");

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("CountItem(未装備)"));
        }
    }
}
