using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests.CommandsCoverage;
using SRCCore.Events;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// If（Goto）・Join・Leave を元の SRC（CmdData.cls の ExecIfCmd・ExecJoinCmd・ExecLeaveCmd）と突き合わせたテスト
    /// </summary>
    [TestClass]
    public class IfJoinLeaveCmdVBMatchTests
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

        private static CmdData[] BuildEvent(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            var cmds = new CmdData[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                cmds[i] = parser.Parse(src, line);
                src.Event.EventCmd.Add(cmds[i]);
                var trimmed = lines[i].Trim();
                if (trimmed.EndsWith(":"))
                {
                    src.Event.AddLabel(trimmed.Substring(0, trimmed.Length - 1), i);
                }
            }
            return cmds;
        }

        [TestMethod]
        public void IfGoto_LabelIsExpression_JumpsToEvaluatedLabel()
        {
            // 元の SRC は FindLabel(GetArg(5)) で見つからなければ FindLabel(GetArgAsString(5)) を探す（Goto コマンドと同じ）
            var src = CreateSrc();
            src.Expression.SetVariableAsString("lbl", "target");
            var cmds = BuildEvent(src,
                "If 1 Goto lbl",
                "Set skipped 1",
                "target:");

            Assert.AreEqual(3, cmds[0].Exec());
        }

        [TestMethod]
        public void IfGoto_LabelNotFound_IsError()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If 1 Goto nolabel",
                "Set skipped 1");

            Assert.AreEqual(-1, cmds[0].Exec());
        }

        private static (CommandUnitFormFixture f, Unit a, Unit b) CreateTwoForms(string statusB)
        {
            // 形態 A（他形態）と形態 B（いまの形態）を持つ 1 体のユニット
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("形態A");
            f.AddUnitData("形態B");
            var a = f.Src.UList.Add("形態A", 0, "味方");
            var b = f.Src.UList.Add("形態B", 0, "味方");
            a.AddOtherForm(b);
            b.AddOtherForm(a);
            a.Status = "他形態";
            b.Status = statusB;
            return (f, a, b);
        }

        private static int Exec(SRC src, string text)
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, text);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd.Exec();
        }

        [TestMethod]
        public void Join_UnitNameOfOtherForm_DoesNotJoinCurrentForm()
        {
            // 元の SRC は見つけたユニットの CurrentForm の名前が指定と違えば u を Nothing にして、何もしない
            var (f, a, b) = CreateTwoForms("離脱");
            using (f)
            {
                Assert.AreEqual(1, Exec(f.Src, "Join 形態A"));
                Assert.AreEqual("離脱", b.Status);
                Assert.AreEqual("他形態", a.Status);
            }
        }

        [TestMethod]
        public void Join_UnitNameOfCurrentForm_Joins()
        {
            var (f, _, b) = CreateTwoForms("離脱");
            using (f)
            {
                Assert.AreEqual(1, Exec(f.Src, "Join 形態B"));
                Assert.AreEqual("待機", b.Status);
            }
        }

        [TestMethod]
        public void Leave_UnitNameOfOtherForm_IsError()
        {
            // 元の SRC は u を Nothing にしたあと PList.Item(pname).Away を参照して実行時エラー（イベントデータが不正です）になる
            var (f, _, b) = CreateTwoForms("待機");
            using (f)
            {
                Assert.AreEqual(-1, Exec(f.Src, "Leave 形態A"));
                Assert.AreEqual("待機", b.Status);
            }
        }

        [TestMethod]
        public void Leave_UnitNameOfCurrentForm_Leaves()
        {
            var (f, _, b) = CreateTwoForms("待機");
            using (f)
            {
                Assert.AreEqual(1, Exec(f.Src, "Leave 形態B"));
                Assert.AreEqual("離脱", b.Status);
            }
        }
    }
}
