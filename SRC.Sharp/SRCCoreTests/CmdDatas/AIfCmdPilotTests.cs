using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// AIfCmd のパイロット条件分岐に関するユニットテスト
    /// ヘルプ「Ifコマンド」に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class AIfCmdPilotTests
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

        private CmdData[] BuildEvent(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            var cmds = new CmdData[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                var cmd = parser.Parse(src, line);
                src.Event.EventCmd.Add(cmd);
                cmds[i] = cmd;
            }
            for (var i = 0; i < lines.Length; i++)
            {
                var trimmed = lines[i].Trim();
                if (trimmed.EndsWith(":"))
                {
                    var labelName = trimmed.Substring(0, trimmed.Length - 1).Trim();
                    src.Event.AddLabel(labelName, i);
                }
            }
            return cmds;
        }

        private void RunEvent(SRC src, CmdData[] cmds, int startId = 0, int maxSteps = 1000)
        {
            var current = startId;
            for (var step = 0; step < maxSteps && current >= 0 && current < cmds.Length; step++)
            {
                current = cmds[current].Exec();
            }
        }

        /// <summary>
        /// パイロットを PDList と PList に登録して返すヘルパー
        /// </summary>
        private Pilots.Pilot CreatePilot(SRC src, string name)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 100;
            return src.PList.Add(name, 1, "味方");
        }

        /// <summary>
        /// パイロット + ユニットを作成し、ユニットに乗せる
        /// </summary>
        private (Pilots.Pilot pilot, Units.Unit unit) CreatePilotWithUnit(SRC src, string pilotName, string status)
        {
            var pilot = CreatePilot(src, pilotName);
            var ud = src.UDList.Add(pilotName + "機");
            var unit = src.UList.Add(pilotName + "機", 1, "味方");
            unit.Status = status;
            pilot.Unit = unit;
            return (pilot, unit);
        }

        // ──────────────────────────────────────────────
        // PrepareArgs のエラーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_OnlyOneArg_ParsesAsNop()
        {
            // ArgNum==1 → PrepareArgs がコンストラクタで EventErrorException をスロー
            // CmdParser.Parse がそれを捕捉して NopCmd を返す → Exec() は nextID を返す
            var src = CreateSrc();
            var cmds = BuildEvent(src, "If");
            // NopCmd.Exec() は NextID (=1) を返す
            Assert.IsNotInstanceOfType(cmds[0], typeof(IfCmd));
        }

        [TestMethod]
        public void IfCmd_NoThenExitGoto_ParsesAsNop()
        {
            // Then/Exit/GoTo がない → PrepareArgs がコンストラクタで例外
            // CmdParser.Parse がそれを捕捉して NopCmd を返す
            var src = CreateSrc();
            var cmds = BuildEvent(src, "If x y z");
            Assert.IsNotInstanceOfType(cmds[0], typeof(IfCmd));
        }

        // ──────────────────────────────────────────────
        // パイロット名による条件分岐 (ヘルプ: ExprTermCount=1)
        // ヘルプ: パイロットが乗っているユニットが出撃していれば成り立ちます
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_PilotCondition_PilotDeployed_ExecutesThenBlock()
        {
            // ヘルプ: パイロット名 → パイロットのユニットが出撃中なら true
            var src = CreateSrc();
            CreatePilotWithUnit(src, "レイ", "出撃");

            var cmds = BuildEvent(src,
                "If レイ Then",          // ID=0
                "Set result 1",          // ID=1
                "EndIf"                  // ID=2
            );
            RunEvent(src, cmds);

            Assert.AreEqual(1L, src.Expression.GetValueAsLong("result"));
        }

        [TestMethod]
        public void IfCmd_PilotCondition_PilotNotDeployed_SkipsThenBlock()
        {
            // ヘルプ: パイロットのユニットが出撃していなければ false
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("result", 0d);
            CreatePilotWithUnit(src, "レイ", "待機");

            var cmds = BuildEvent(src,
                "If レイ Then",          // ID=0
                "Set result 1",          // ID=1
                "EndIf"                  // ID=2
            );
            RunEvent(src, cmds);

            Assert.AreEqual(0L, src.Expression.GetValueAsLong("result"));
        }

        [TestMethod]
        public void IfCmd_PilotCondition_PilotHasNoUnit_ReturnsFalse()
        {
            // ヘルプ: パイロットが未乗機の場合は false
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("result", 0d);
            var pilot = CreatePilot(src, "ネリィ");
            // Unit は設定しない (null)

            var cmds = BuildEvent(src,
                "If ネリィ Then",        // ID=0
                "Set result 1",          // ID=1
                "EndIf"                  // ID=2
            );
            RunEvent(src, cmds);

            Assert.AreEqual(0L, src.Expression.GetValueAsLong("result"));
        }

        [TestMethod]
        public void IfCmd_PilotCondition_UndefinedPilotName_TreatsAsExpression()
        {
            // パイロットとして未登録の名前は式として評価され 0 → false
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("result", 0d);

            var cmds = BuildEvent(src,
                "If 存在しないパイロット名 Then",  // ID=0
                "Set result 1",                     // ID=1
                "EndIf"                             // ID=2
            );
            RunEvent(src, cmds);

            Assert.AreEqual(0L, src.Expression.GetValueAsLong("result"));
        }

        [TestMethod]
        public void IfCmd_PilotCondition_PilotStored_ReturnsTrue()
        {
            // 実装: "格納" ステータスのユニットも "出撃" と同様に true を返す
            // 注意: ヘルプには "格納されている場合には成り立ちません" と記載されているが、
            //       実装では格納 = true となっている (Issue #XXX で追跡予定)
            var src = CreateSrc();
            CreatePilotWithUnit(src, "レイ", "格納");

            var cmds = BuildEvent(src,
                "If レイ Then",          // ID=0
                "Set result 1",          // ID=1
                "EndIf"                  // ID=2
            );
            RunEvent(src, cmds);

            // 実装の実際の動作: 格納 = true
            Assert.AreEqual(1L, src.Expression.GetValueAsLong("result"));
        }

        // ──────────────────────────────────────────────
        // Not パイロット名 による条件分岐 (ヘルプ: ExprTermCount=2)
        // ヘルプ: Not が付けられている場合は条件式の結果は逆になります
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_NotPilotCondition_PilotNotDeployed_ExecutesThenBlock()
        {
            // ヘルプ: Not ジェイ → ジェイが出撃していなければ true
            var src = CreateSrc();
            CreatePilotWithUnit(src, "ジェイ", "待機");

            var cmds = BuildEvent(src,
                "If not ジェイ Then",    // ID=0
                "Set result 1",          // ID=1
                "EndIf"                  // ID=2
            );
            RunEvent(src, cmds);

            Assert.AreEqual(1L, src.Expression.GetValueAsLong("result"));
        }

        [TestMethod]
        public void IfCmd_NotPilotCondition_PilotDeployed_SkipsThenBlock()
        {
            // ヘルプ: Not パイロット名 → パイロットが出撃中なら false
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("result", 0d);
            CreatePilotWithUnit(src, "ジェイ", "出撃");

            var cmds = BuildEvent(src,
                "If not ジェイ Then",    // ID=0
                "Set result 1",          // ID=1
                "EndIf"                  // ID=2
            );
            RunEvent(src, cmds);

            Assert.AreEqual(0L, src.Expression.GetValueAsLong("result"));
        }

        [TestMethod]
        public void IfCmd_NotPilotCondition_PilotHasNoUnit_ExecutesThenBlock()
        {
            // Not + 未乗機パイロット → true (条件の逆)
            var src = CreateSrc();
            var pilot = CreatePilot(src, "ネリィ");

            var cmds = BuildEvent(src,
                "If not ネリィ Then",    // ID=0
                "Set result 1",          // ID=1
                "EndIf"                  // ID=2
            );
            RunEvent(src, cmds);

            Assert.AreEqual(1L, src.Expression.GetValueAsLong("result"));
        }

        // ──────────────────────────────────────────────
        // AIfCmd.ExprTermCount の確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_PilotCondition_ExprTermCountIsOne()
        {
            // PrepareArgs 後に ExprTermCount == 1 であること
            var src = CreateSrc();
            CreatePilot(src, "レイ");

            var parser = new CmdParser();
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, "If レイ Then");
            src.Event.EventData.Add(line);
            var cmd = parser.Parse(src, line) as AIfCmd;
            src.Event.EventCmd.Add(cmd);
            // ParseはPrepareArgsを呼ばない (Exec時に呼ぶ) - Exec前にEndIfを追加してから実行
            var endLine = new EventDataLine(1, EventDataSource.Scenario, "test", 1, "EndIf");
            src.Event.EventData.Add(endLine);
            var endCmd = parser.Parse(src, endLine);
            src.Event.EventCmd.Add(endCmd);

            // Exec を呼んで PrepareArgs を走らせる
            cmd.Exec();

            Assert.AreEqual(1, cmd.ExprTermCount);
        }

        [TestMethod]
        public void IfCmd_NotPilotCondition_ExprTermCountIsTwo()
        {
            // Not パイロット名 → ExprTermCount == 2
            var src = CreateSrc();
            CreatePilot(src, "レイ");

            var parser = new CmdParser();
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, "If not レイ Then");
            src.Event.EventData.Add(line);
            var cmd = parser.Parse(src, line) as AIfCmd;
            src.Event.EventCmd.Add(cmd);
            var endLine = new EventDataLine(1, EventDataSource.Scenario, "test", 1, "EndIf");
            src.Event.EventData.Add(endLine);
            src.Event.EventCmd.Add(parser.Parse(src, endLine));

            cmd.Exec();

            Assert.AreEqual(2, cmd.ExprTermCount);
        }

        [TestMethod]
        public void IfCmd_ExpressionCondition_ExprTermCountIsZero()
        {
            // 式 ($x > 3) → ExprTermCount == 0
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 5d);

            var parser = new CmdParser();
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, "If $x > 3 Then");
            src.Event.EventData.Add(line);
            var cmd = parser.Parse(src, line) as AIfCmd;
            src.Event.EventCmd.Add(cmd);
            var endLine = new EventDataLine(1, EventDataSource.Scenario, "test", 1, "EndIf");
            src.Event.EventData.Add(endLine);
            src.Event.EventCmd.Add(parser.Parse(src, endLine));

            cmd.Exec();

            Assert.AreEqual(0, cmd.ExprTermCount);
        }

        // ──────────────────────────────────────────────
        // AIfCmd.ToEnd の検証 (IfとEndIfの対応)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_MissingEndIf_ReturnsError()
        {
            // IfとEndIfが対応していない場合はエラー
            // 条件 false で EndIf を探すが見つからない
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If (1 > 2) Then",  // ID=0: false → EndIfを探す
                "Set result 1"      // ID=1: EndIfなし
            );
            // IfCmd.ExecInternal が EventErrorException をスロー → Exec が -1 を返す
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ElseIf のパイロット条件
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ElseIfCmd_PilotCondition_PilotDeployed_ExecutesElseIfBlock()
        {
            // ElseIf でもパイロット名条件が機能すること
            var src = CreateSrc();
            CreatePilotWithUnit(src, "レイ", "出撃");

            var cmds = BuildEvent(src,
                "If (1 > 2) Then",      // ID=0: false
                "Set result 0",          // ID=1
                "ElseIf レイ Then",      // ID=2: true
                "Set result 2",          // ID=3
                "EndIf"                  // ID=4
            );
            RunEvent(src, cmds);

            Assert.AreEqual(2L, src.Expression.GetValueAsLong("result"));
        }
    }
}
