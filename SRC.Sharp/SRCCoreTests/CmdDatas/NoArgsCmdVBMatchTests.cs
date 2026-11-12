using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// 引数の無いコマンド（ArgNum = 1）の実行を、元の SRC（CmdData.cls）と突き合わせる（#1511）。
    /// 元の SRC の Parse は、引数の無いコマンドでは引数の配列（strArgs(2 To ArgNum)）を作らない。
    /// 引数の数を確かめずに引数を読むコマンドは、配列が無いので実行時エラー（「イベントデータが不正です」）になる
    /// （引数の無い Incr・Ride・UnSet は元の SRC の実機で確かめた。SRC.Sharp/Compat/probe-expected/probe-bounds）。
    /// </summary>
    [TestClass]
    public class NoArgsCmdVBMatchTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        private string ExecError(string cmdText)
        {
            var src = CreateSrc();
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            Assert.AreEqual(1, cmd.ArgNum);
            Assert.AreEqual(-1, cmd.Exec());
            return src.DataErrors.Last().msg;
        }

        // 元の SRC: ExecCreateCmd は num = ArgNum の後、Select Case GetArgAsString(num) で strArgs(1) を読む
        [TestMethod]
        public void Create_NoArgs_IsInvalidEventData()
        {
            Assert.AreEqual("イベントデータが不正です", ExecError("Create"));
        }

        // 元の SRC: ExecLaunchCmd も Select Case GetArgAsString(num) で strArgs(1) を読む
        [TestMethod]
        public void Launch_NoArgs_IsInvalidEventData()
        {
            Assert.AreEqual("イベントデータが不正です", ExecError("Launch"));
        }

        // 元の SRC: ExecMapAttackCmd は num <= 6 なら GetArgAsString(num) で strArgs(1) を読む
        [TestMethod]
        public void MapAttack_NoArgs_IsInvalidEventData()
        {
            Assert.AreEqual("イベントデータが不正です", ExecError("MapAttack"));
            Assert.AreEqual("イベントデータが不正です", ExecError("MapWeapon"));
        }

        // 元の SRC: ExecShowImageCmd は引数の数を確かめずに GetArgAsString(2) を読む（「引数の数が違います」は出さない）
        [TestMethod]
        public void ShowImage_NoArgs_IsInvalidEventData()
        {
            Assert.AreEqual("イベントデータが不正です", ExecError("ShowImage"));
        }

        // 元の SRC も引数の数を確かめずに 2 番目の引数を読むコマンド（SRC# も同じく読んでエラーになる）
        [TestMethod]
        public void CommandsReadingSecondArgWithoutCheck_NoArgs_IsInvalidEventData()
        {
            foreach (var name in new[] { "Incr", "Ride", "UnSet", "Call", "ClearSkill", "ClearAbility", "MakeUnitList", "Move" })
            {
                Assert.AreEqual("イベントデータが不正です", ExecError(name), name);
            }
        }
    }
}
