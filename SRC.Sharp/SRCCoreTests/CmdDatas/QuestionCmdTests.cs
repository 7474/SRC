using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    [TestClass]
    public class QuestionCmdTests
    {
        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC
            {
                GUI = gui,
            };
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return (src, gui);
        }

        /// <summary>
        /// Sets up event data for Question command tests:
        ///   ID=0: "Question {timeLimit}" (with optional msg)
        ///   ID=1..N: choice text lines
        ///   ID=N+1: "End"
        /// </summary>
        private QuestionCmd SetupQuestionEvent(SRC src, string cmdLine, IList<string> choices)
        {
            var parser = new CmdParser();
            var lines = new List<EventDataLine>();
            lines.Add(new EventDataLine(0, EventDataSource.Scenario, "test", 0, cmdLine));
            foreach (var choice in choices)
            {
                lines.Add(new EventDataLine(lines.Count, EventDataSource.Scenario, "test", lines.Count, choice));
            }
            lines.Add(new EventDataLine(lines.Count, EventDataSource.Scenario, "test", lines.Count, "End"));

            src.Event.EventData = lines;
            src.Event.EventCmd = new List<CmdData>(new CmdData[lines.Count]);
            for (var i = 0; i < lines.Count; i++)
            {
                src.Event.EventCmd[i] = parser.Parse(src, lines[i]);
            }

            return (QuestionCmd)src.Event.EventCmd[0];
        }

        [TestMethod]
        public void QuestionSelectFirstOptionTest()
        {
            var (src, gui) = CreateSrc();
            // LIPS returns 1 (first item selected)
            gui.LIPSHandler = (args, timeLimit) => 1;

            var cmd = SetupQuestionEvent(src, "Question 5", new[] { "選択肢A", "選択肢B", "選択肢C" });
            cmd.Exec();

            // First choice is at offset 1 from Question (ID=1), so ListItemID="1"
            Assert.AreEqual("1", src.Event.SelectedAlternative);
        }

        [TestMethod]
        public void QuestionSelectSecondOptionTest()
        {
            var (src, gui) = CreateSrc();
            // LIPS returns 2 (second item selected)
            gui.LIPSHandler = (args, timeLimit) => 2;

            var cmd = SetupQuestionEvent(src, "Question 5", new[] { "選択肢A", "選択肢B", "選択肢C" });
            cmd.Exec();

            // Second choice is at offset 2 from Question (ID=2), so ListItemID="2"
            Assert.AreEqual("2", src.Event.SelectedAlternative);
        }

        [TestMethod]
        public void QuestionTimeLimitPassedToLIPSTest()
        {
            var (src, gui) = CreateSrc();
            var capturedTimeLimit = -1;
            gui.LIPSHandler = (args, timeLimit) =>
            {
                capturedTimeLimit = timeLimit;
                return 1;
            };

            var cmd = SetupQuestionEvent(src, "Question 10", new[] { "選択肢A" });
            cmd.Exec();

            Assert.AreEqual(10, capturedTimeLimit);
        }

        [TestMethod]
        public void QuestionCustomMessageTest()
        {
            var (src, gui) = CreateSrc();
            string capturedInfo = null;
            gui.LIPSHandler = (args, timeLimit) =>
            {
                capturedInfo = args.lb_info;
                return 1;
            };

            var cmd = SetupQuestionEvent(src, "Question 5 どうする？", new[] { "選択肢A" });
            cmd.Exec();

            Assert.AreEqual("どうする？", capturedInfo);
        }

        [TestMethod]
        public void QuestionDefaultMessageTest()
        {
            var (src, gui) = CreateSrc();
            string capturedInfo = null;
            gui.LIPSHandler = (args, timeLimit) =>
            {
                capturedInfo = args.lb_info;
                return 1;
            };

            // ArgNum=2: no message specified, should use default
            var cmd = SetupQuestionEvent(src, "Question 5", new[] { "選択肢A" });
            cmd.Exec();

            Assert.AreEqual("さあ、どうする？", capturedInfo);
        }

        [TestMethod]
        public void QuestionNoChoicesTest()
        {
            var (src, gui) = CreateSrc();
            // LIPS should not be called if no choices
            gui.LIPSHandler = (args, timeLimit) =>
            {
                Assert.Fail("LIPS should not be called when there are no choices");
                return 0;
            };

            var cmd = SetupQuestionEvent(src, "Question 5", System.Array.Empty<string>());
            cmd.Exec();

            Assert.AreEqual("0", src.Event.SelectedAlternative);
            Assert.AreEqual(0, src.Commands.SelectedItem);
        }

        [TestMethod]
        public void QuestionReturnsNextIdAfterEndTest()
        {
            var (src, gui) = CreateSrc();
            gui.LIPSHandler = (args, timeLimit) => 1;

            // Lines: ID=0 Question, ID=1 ChoiceA, ID=2 End → End.NextID = 3
            var cmd = SetupQuestionEvent(src, "Question 5", new[] { "選択肢A" });
            var result = cmd.Exec();

            // End is at ID=2, NextID=3
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void QuestionMissingEndTest()
        {
            var (src, gui) = CreateSrc();
            gui.LIPSHandler = (args, timeLimit) => 1;

            // Set up event data without End command
            var parser = new CmdParser();
            var lines = new List<EventDataLine>
            {
                new EventDataLine(0, EventDataSource.Scenario, "test", 0, "Question 5"),
                new EventDataLine(1, EventDataSource.Scenario, "test", 1, "選択肢A"),
                // No End command
            };
            src.Event.EventData = lines;
            src.Event.EventCmd = new List<CmdData>
            {
                parser.Parse(src, lines[0]),
                parser.Parse(src, lines[1]),
            };
            var cmd = (QuestionCmd)src.Event.EventCmd[0];

            // Missing End should cause EventErrorException, caught by Exec() → returns -1
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
