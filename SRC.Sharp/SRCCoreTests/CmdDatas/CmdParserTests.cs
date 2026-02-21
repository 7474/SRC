using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// CmdParser のユニットテスト
    /// 各イベントコマンド文字列が正しい CmdData サブクラスを生成するかを検証する
    /// </summary>
    [TestClass]
    public class CmdParserTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return src;
        }

        private CmdData Parse(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // Nop / ラベル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_EmptyLine_ReturnsNopCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "");
            Assert.IsInstanceOfType(cmd, typeof(NopCmd));
        }

        [TestMethod]
        public void Parse_WhitespaceLine_ReturnsNopCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "   ");
            Assert.IsInstanceOfType(cmd, typeof(NopCmd));
        }

        [TestMethod]
        public void Parse_LabelLine_ReturnsNopCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "someLabel:");
            Assert.IsInstanceOfType(cmd, typeof(NopCmd));
        }

        // ──────────────────────────────────────────────
        // 変数コマンド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_Set_ReturnsSetCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Set x 1");
            Assert.IsInstanceOfType(cmd, typeof(SetCmd));
        }

        [TestMethod]
        public void Parse_Incr_ReturnsIncrCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Incr x");
            Assert.IsInstanceOfType(cmd, typeof(IncrCmd));
        }

        [TestMethod]
        public void Parse_Global_ReturnsGlobalCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Global x");
            Assert.IsInstanceOfType(cmd, typeof(GlobalCmd));
        }

        [TestMethod]
        public void Parse_Array_ReturnsArrayCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Array x \"a,b\" \",\"");
            Assert.IsInstanceOfType(cmd, typeof(ArrayCmd));
        }

        [TestMethod]
        public void Parse_CopyArray_ReturnsCopyArrayCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "CopyArray a b");
            Assert.IsInstanceOfType(cmd, typeof(CopyArrayCmd));
        }

        [TestMethod]
        public void Parse_Swap_ReturnsSwapCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Swap x y");
            Assert.IsInstanceOfType(cmd, typeof(SwapCmd));
        }

        [TestMethod]
        public void Parse_UnSet_ReturnsUnSetCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "UnSet x");
            Assert.IsInstanceOfType(cmd, typeof(UnSetCmd));
        }

        [TestMethod]
        public void Parse_Sort_ReturnsSortCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Sort arr 昇順");
            Assert.IsInstanceOfType(cmd, typeof(SortCmd));
        }

        [TestMethod]
        public void Parse_UpVar_ReturnsUpVarCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "UpVar x");
            Assert.IsInstanceOfType(cmd, typeof(UpVarCmd));
        }

        // ──────────────────────────────────────────────
        // 制御フロー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_If_ReturnsIfCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "If 1 > 0 Then");
            Assert.IsInstanceOfType(cmd, typeof(IfCmd));
        }

        [TestMethod]
        public void Parse_ElseIf_ReturnsElseIfCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "ElseIf 1 > 0 Then");
            Assert.IsInstanceOfType(cmd, typeof(ElseIfCmd));
        }

        [TestMethod]
        public void Parse_Else_ReturnsElseCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Else");
            Assert.IsInstanceOfType(cmd, typeof(ElseCmd));
        }

        [TestMethod]
        public void Parse_EndIf_ReturnsEndIfCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "EndIf");
            Assert.IsInstanceOfType(cmd, typeof(EndIfCmd));
        }

        [TestMethod]
        public void Parse_For_ReturnsForCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "For i 1 10");
            Assert.IsInstanceOfType(cmd, typeof(ForCmd));
        }

        [TestMethod]
        public void Parse_Next_ReturnsNextCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Next i");
            Assert.IsInstanceOfType(cmd, typeof(NextCmd));
        }

        [TestMethod]
        public void Parse_ForEach_ReturnsForEachCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "ForEach x arr");
            Assert.IsInstanceOfType(cmd, typeof(ForEachCmd));
        }

        [TestMethod]
        public void Parse_Do_ReturnsDoCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Do");
            Assert.IsInstanceOfType(cmd, typeof(DoCmd));
        }

        [TestMethod]
        public void Parse_Loop_ReturnsLoopCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Loop");
            Assert.IsInstanceOfType(cmd, typeof(LoopCmd));
        }

        [TestMethod]
        public void Parse_Break_ReturnsBreakCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Break");
            Assert.IsInstanceOfType(cmd, typeof(BreakCmd));
        }

        [TestMethod]
        public void Parse_Continue_ReturnsContinueCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Continue");
            Assert.IsInstanceOfType(cmd, typeof(ContinueCmd));
        }

        [TestMethod]
        public void Parse_Goto_ReturnsGotoCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Goto someLabel");
            Assert.IsInstanceOfType(cmd, typeof(GotoCmd));
        }

        [TestMethod]
        public void Parse_Call_ReturnsCallCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Call subRoutine");
            Assert.IsInstanceOfType(cmd, typeof(CallCmd));
        }

        [TestMethod]
        public void Parse_Return_ReturnsReturnCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Return");
            Assert.IsInstanceOfType(cmd, typeof(ReturnCmd));
        }

        [TestMethod]
        public void Parse_Skip_ReturnsSkipCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Skip");
            Assert.IsInstanceOfType(cmd, typeof(SkipCmd));
        }

        [TestMethod]
        public void Parse_Switch_ReturnsSwitchCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Switch x");
            Assert.IsInstanceOfType(cmd, typeof(SwitchCmd));
        }

        [TestMethod]
        public void Parse_CaseElse_ReturnsCaseElseCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Case Else");
            Assert.IsInstanceOfType(cmd, typeof(CaseElseCmd));
        }

        [TestMethod]
        public void Parse_Case_ReturnsCaseCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Case 1");
            Assert.IsInstanceOfType(cmd, typeof(CaseCmd));
        }

        [TestMethod]
        public void Parse_EndSw_ReturnsEndSwCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "EndSw");
            Assert.IsInstanceOfType(cmd, typeof(EndSwCmd));
        }

        [TestMethod]
        public void Parse_Local_ReturnsLocalCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Local x");
            Assert.IsInstanceOfType(cmd, typeof(LocalCmd));
        }

        // ──────────────────────────────────────────────
        // Stage/Misc コマンド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_Money_ReturnsMoneyCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Money 100");
            Assert.IsInstanceOfType(cmd, typeof(MoneyCmd));
        }

        [TestMethod]
        public void Parse_Debug_ReturnsDebugCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Debug \"message\"");
            Assert.IsInstanceOfType(cmd, typeof(DebugCmd));
        }

        [TestMethod]
        public void Parse_End_ReturnsEndCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "End");
            Assert.IsInstanceOfType(cmd, typeof(EndCmd));
        }

        [TestMethod]
        public void Parse_Exit_ReturnsExitCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Exit");
            Assert.IsInstanceOfType(cmd, typeof(ExitCmd));
        }

        [TestMethod]
        public void Parse_GameClear_ReturnsGameClearCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "GameClear");
            Assert.IsInstanceOfType(cmd, typeof(GameClearCmd));
        }

        [TestMethod]
        public void Parse_GameOver_ReturnsGameOverCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "GameOver");
            Assert.IsInstanceOfType(cmd, typeof(GameOverCmd));
        }

        [TestMethod]
        public void Parse_Suspend_ReturnsSuspendCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Suspend");
            Assert.IsInstanceOfType(cmd, typeof(SuspendCmd));
        }

        [TestMethod]
        public void Parse_Wait_ReturnsWaitCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Wait 1000");
            Assert.IsInstanceOfType(cmd, typeof(WaitCmd));
        }

        [TestMethod]
        public void Parse_Option_ReturnsOptionCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Option x on");
            Assert.IsInstanceOfType(cmd, typeof(OptionCmd));
        }

        [TestMethod]
        public void Parse_Require_ReturnsRequireCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Require 「勝利条件」");
            Assert.IsInstanceOfType(cmd, typeof(RequireCmd));
        }

        [TestMethod]
        public void Parse_Finish_ReturnsFinishCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Finish");
            Assert.IsInstanceOfType(cmd, typeof(FinishCmd));
        }

        // ──────────────────────────────────────────────
        // Sound コマンド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_PlayMIDI_ReturnsPlayMIDICmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "PlayMIDI bgm.mid");
            Assert.IsInstanceOfType(cmd, typeof(PlayMIDICmd));
        }

        [TestMethod]
        public void Parse_PlaySound_ReturnsPlaySoundCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "PlaySound se.wav");
            Assert.IsInstanceOfType(cmd, typeof(PlaySoundCmd));
        }

        [TestMethod]
        public void Parse_StartBGM_ReturnsStartBGMCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "StartBGM bgm.mp3");
            Assert.IsInstanceOfType(cmd, typeof(StartBGMCmd));
        }

        [TestMethod]
        public void Parse_StopBGM_ReturnsStopBGMCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "StopBGM");
            Assert.IsInstanceOfType(cmd, typeof(StopBGMCmd));
        }

        [TestMethod]
        public void Parse_KeepBGM_ReturnsKeepBGMCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "KeepBGM");
            Assert.IsInstanceOfType(cmd, typeof(KeepBGMCmd));
        }

        // ──────────────────────────────────────────────
        // Pilot コマンド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_LevelUp_ReturnsLevelUpCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "LevelUp パイロット");
            Assert.IsInstanceOfType(cmd, typeof(LevelUpCmd));
        }

        [TestMethod]
        public void Parse_ExpUp_ReturnsExpUpCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "ExpUp パイロット 100");
            Assert.IsInstanceOfType(cmd, typeof(ExpUpCmd));
        }

        [TestMethod]
        public void Parse_SetSkill_ReturnsSetSkillCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "SetSkill パイロット スキル");
            Assert.IsInstanceOfType(cmd, typeof(SetSkillCmd));
        }

        [TestMethod]
        public void Parse_ClearSkill_ReturnsClearSkillCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "ClearSkill パイロット");
            Assert.IsInstanceOfType(cmd, typeof(ClearSkillCmd));
        }

        [TestMethod]
        public void Parse_RecoverSP_ReturnsRecoverSPCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "RecoverSP パイロット 100");
            Assert.IsInstanceOfType(cmd, typeof(RecoverSPCmd));
        }

        [TestMethod]
        public void Parse_RecoverPlana_ReturnsRecoverPlanaCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "RecoverPlana パイロット 100");
            Assert.IsInstanceOfType(cmd, typeof(RecoverPlanaCmd));
        }

        [TestMethod]
        public void Parse_IncreaseMorale_ReturnsIncreaseMoraleCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "IncreaseMorale パイロット 5");
            Assert.IsInstanceOfType(cmd, typeof(IncreaseMoraleCmd));
        }

        // ──────────────────────────────────────────────
        // File コマンド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_Open_ReturnsOpenCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Open file.txt 1");
            Assert.IsInstanceOfType(cmd, typeof(OpenCmd));
        }

        [TestMethod]
        public void Parse_Close_ReturnsCloseCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Close 1");
            Assert.IsInstanceOfType(cmd, typeof(CloseCmd));
        }

        [TestMethod]
        public void Parse_Read_ReturnsReadCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Read 1 x");
            Assert.IsInstanceOfType(cmd, typeof(ReadCmd));
        }

        [TestMethod]
        public void Parse_Write_ReturnsWriteCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Write 1 \"data\"");
            Assert.IsInstanceOfType(cmd, typeof(WriteCmd));
        }

        [TestMethod]
        public void Parse_LineRead_ReturnsLineReadCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "LineRead 1 x");
            Assert.IsInstanceOfType(cmd, typeof(LineReadCmd));
        }

        [TestMethod]
        public void Parse_CopyFile_ReturnsCopyFileCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "CopyFile src.txt dst.txt");
            Assert.IsInstanceOfType(cmd, typeof(CopyFileCmd));
        }

        [TestMethod]
        public void Parse_RemoveFile_ReturnsRemoveFileCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "RemoveFile file.txt");
            Assert.IsInstanceOfType(cmd, typeof(RemoveFileCmd));
        }

        // ──────────────────────────────────────────────
        // 代入式 (Assignment syntax)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_AssignmentSyntax_ReturnsSetCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "x = 42");
            Assert.IsInstanceOfType(cmd, typeof(SetCmd));
        }

        [TestMethod]
        public void Parse_AssignmentWithExpression_ReturnsSetCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "result = 3 + 4");
            Assert.IsInstanceOfType(cmd, typeof(SetCmd));
        }

        // ──────────────────────────────────────────────
        // 大文字小文字を区別しない
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_UppercaseCommand_ReturnsCorrectCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "SET x 1");
            Assert.IsInstanceOfType(cmd, typeof(SetCmd));
        }

        [TestMethod]
        public void Parse_MixedCaseCommand_ReturnsCorrectCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "If 1 > 0 Then");
            Assert.IsInstanceOfType(cmd, typeof(IfCmd));
        }

        [TestMethod]
        public void Parse_LowercaseCommand_ReturnsCorrectCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "global myVar");
            Assert.IsInstanceOfType(cmd, typeof(GlobalCmd));
        }

        // ──────────────────────────────────────────────
        // 未知コマンド → サブルーチンコールとして CallCmd を返す
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_UnknownCommand_ReturnsCallCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "MySubroutine");
            Assert.IsInstanceOfType(cmd, typeof(CallCmd));
        }

        [TestMethod]
        public void Parse_UnknownCommandWithArgs_ReturnsCallCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "SomeRoutine arg1 arg2");
            Assert.IsInstanceOfType(cmd, typeof(CallCmd));
        }

        // ──────────────────────────────────────────────
        // エイリアス (aliases)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_ClearAbility_ReturnsClearSkillCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "ClearAbility x");
            Assert.IsInstanceOfType(cmd, typeof(ClearSkillCmd));
        }

        [TestMethod]
        public void Parse_SetAbility_ReturnsSetSkillCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "SetAbility パイロット スキル");
            Assert.IsInstanceOfType(cmd, typeof(SetSkillCmd));
        }

        [TestMethod]
        public void Parse_Mind_ReturnsSpecialPowerCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Mind x y z");
            Assert.IsInstanceOfType(cmd, typeof(SpecialPowerCmd));
        }

        [TestMethod]
        public void Parse_MapWeapon_ReturnsMapAttackCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "MapWeapon x y");
            Assert.IsInstanceOfType(cmd, typeof(MapAttackCmd));
        }

        [TestMethod]
        public void Parse_ClearMind_ReturnsClearSpecialPowerCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "ClearMind x");
            Assert.IsInstanceOfType(cmd, typeof(ClearSpecialPowerCmd));
        }

        // ──────────────────────────────────────────────
        // Talk コマンド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_Talk_ReturnsTalkCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Talk パイロット \"こんにちは\"");
            Assert.IsInstanceOfType(cmd, typeof(TalkCmd));
        }

        [TestMethod]
        public void Parse_AutoTalk_ReturnsAutoTalkCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "AutoTalk 攻撃");
            Assert.IsInstanceOfType(cmd, typeof(AutoTalkCmd));
        }

        // ──────────────────────────────────────────────
        // Unit コマンド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_Create_ReturnsCreateCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Create ユニット パイロット");
            Assert.IsInstanceOfType(cmd, typeof(CreateCmd));
        }

        [TestMethod]
        public void Parse_Destroy_ReturnsDestroyCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Destroy ユニット");
            Assert.IsInstanceOfType(cmd, typeof(DestroyCmd));
        }

        [TestMethod]
        public void Parse_RecoverHP_ReturnsRecoverHPCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "RecoverHP ユニット 1000");
            Assert.IsInstanceOfType(cmd, typeof(RecoverHPCmd));
        }

        [TestMethod]
        public void Parse_RecoverEN_ReturnsRecoverENCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "RecoverEN ユニット 100");
            Assert.IsInstanceOfType(cmd, typeof(RecoverENCmd));
        }

        [TestMethod]
        public void Parse_Transform_ReturnsTransformCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Transform ユニット 形態");
            Assert.IsInstanceOfType(cmd, typeof(TransformCmd));
        }

        [TestMethod]
        public void Parse_Join_ReturnsJoinCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Join ユニット");
            Assert.IsInstanceOfType(cmd, typeof(JoinCmd));
        }

        [TestMethod]
        public void Parse_Leave_ReturnsLeaveCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Leave ユニット");
            Assert.IsInstanceOfType(cmd, typeof(LeaveCmd));
        }

        [TestMethod]
        public void Parse_Move_ReturnsMoveCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Move ユニット 5 3");
            Assert.IsInstanceOfType(cmd, typeof(MoveCmd));
        }

        [TestMethod]
        public void Parse_Supply_ReturnsSupplyCmd()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "Supply ユニット");
            Assert.IsInstanceOfType(cmd, typeof(SupplyCmd));
        }
    }
}
