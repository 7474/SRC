using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// If の条件式に引用符で囲んだ文字列を書いた場合（元の SRC と同じく、引用符を残したまま式として評価する）
    /// </summary>
    [TestClass]
    public class IfCmdQuotedLiteralTests
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

        private static string Run(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            var cmds = new List<CmdData>();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                var cmd = parser.Parse(src, line);
                src.Event.EventCmd.Add(cmd);
                cmds.Add(cmd);
            }
            var current = 0;
            for (var step = 0; step < 100 && current >= 0 && current < cmds.Count; step++)
            {
                current = cmds[current].Exec();
            }
            return src.Expression.GetValueAsString("result");
        }

        [TestMethod]
        public void CloseParenLiteral_ComparesAsString()
        {
            // ロボダンのアップグレード画面: If Left(文章[2], 1) = ")" Then
            var src = CreateSrc();
            src.Expression.SetVariableAsString("文章[2]", "・フルアーマー");
            Assert.AreEqual("else", Run(src,
                "If Left(文章[2], 1) = \")\" Then",
                "Set result \"then\"",
                "Else",
                "Set result \"else\"",
                "EndIf"));
        }

        [TestMethod]
        public void CloseParenLiteral_Matches()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("文章[2]", ")ABC");
            Assert.AreEqual("then", Run(src,
                "If Left(文章[2], 1) = \")\" Then",
                "Set result \"then\"",
                "Else",
                "Set result \"else\"",
                "EndIf"));
        }

        [TestMethod]
        public void QuotedStringWithSpace_ComparesWholeString()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("選択", "a b");
            Assert.AreEqual("then", Run(src,
                "If 選択 = \"a b\" Then",
                "Set result \"then\"",
                "Else",
                "Set result \"else\"",
                "EndIf"));
        }
    }
}
