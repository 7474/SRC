using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Coverage.Tests.UnitAttack;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.EventCommands
{
    /// <summary>
    /// CmdData.cls の ExecArcCmd〜ExecChangeAreaCmd（1401〜2705 行）と C# のコマンドの突き合わせ（7474/SRC#1503）。
    /// 期待値は元の SRC（SRC/SRC_20121125/CmdData.cls）の動作。
    /// </summary>
    [TestClass]
    public class CmdDataArcToChangeAreaVBMatchTests
    {
        private static UnitAttackFixture CreateFixture()
        {
            var f = new UnitAttackFixture();
            f.Src.Event.EventData = new List<EventDataLine>();
            f.Src.Event.EventCmd = new List<CmdData>();
            f.Src.Event.EventFileNames = new List<string>();
            f.Src.Event.AdditionalEventFileNames = new List<string>();
            f.Src.Event.EventQue = new List<string>();
            return f;
        }

        private static CmdData[] BuildEvent(SRC src, params string[] lines)
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
                    src.Event.AddLabel(trimmed.Substring(0, trimmed.Length - 1).Trim(), i);
                }
            }
            return cmds;
        }

        private static IList<string> Errors(SRC src)
        {
            return src.DataErrors.Select(x => x.msg).ToList();
        }

        /// <summary>
        /// メッセージウィンドウの開閉で MessageFormVisible が変わるようにし、開いた回数を数える
        /// </summary>
        private static System.Func<int> TrackMessageForm(UnitAttackFixture f)
        {
            var opened = 0;
            var visible = false;
            f.Gui.Handle("get_" + nameof(IGUI.MessageFormVisible), args => visible);
            f.Gui.Handle(nameof(IGUI.OpenMessageForm), args => { opened++; visible = true; return null; });
            f.Gui.Handle(nameof(IGUI.CloseMessageForm), args => { visible = false; return null; });
            return () => opened;
        }

        // ── ExecArcCmd ──

        // 色は #rrggbb の 7 文字だけ。それ以外は「色指定が不正です」
        [TestMethod]
        public void Arc_ShortColor_IsError()
        {
            var f = CreateFixture();
            var cmds = BuildEvent(f.Src, "Arc 10 10 5 0 90 #FFF");
            Assert.AreEqual(-1, cmds[0].Exec());
            CollectionAssert.Contains(Errors(f.Src).ToList(), "色指定が不正です");
            Assert.AreEqual(0, f.Calls(nameof(IGUIScrean.ArcCmd)).Count);
        }

        [TestMethod]
        public void Arc_InvalidHexColor_IsColorError()
        {
            var f = CreateFixture();
            var cmds = BuildEvent(f.Src, "Arc 10 10 5 0 90 #GG0000");
            Assert.AreEqual(-1, cmds[0].Exec());
            CollectionAssert.Contains(Errors(f.Src).ToList(), "色指定が不正です");
        }

        // VB の Circle メソッド: 開始から終了へ反時計回り。終了が小さければ 0 度をまたぎ、同じなら一周
        [TestMethod]
        public void ScreanArc_SweepsCounterClockwiseFromStartToEnd()
        {
            var a = new ScreanArc(0f, 90f);
            Assert.AreEqual(0f, a.StartAngle);
            Assert.AreEqual(90f, a.SweepAngle);
            Assert.IsFalse(a.IsPie);

            var b = new ScreanArc(270f, 90f);
            Assert.AreEqual(270f, b.StartAngle);
            Assert.AreEqual(180f, b.SweepAngle);

            var c = new ScreanArc(90f, 90f);
            Assert.AreEqual(360f, c.SweepAngle);
        }

        // 負の角度は中心への線。両方が負なら扇形（塗りつぶしのとき ArcCmd が角度を負にする）
        [TestMethod]
        public void ScreanArc_NegativeAnglesDrawRadiusLines()
        {
            var pie = new ScreanArc(-0.000001f, -90f);
            Assert.IsTrue(pie.IsPie);
            Assert.AreEqual(90f, pie.SweepAngle, 0.001f);

            var one = new ScreanArc(-30f, 120f);
            Assert.IsTrue(one.LineFromStart);
            Assert.IsFalse(one.LineToEnd);
            Assert.IsFalse(one.IsPie);
            Assert.AreEqual(30f, one.StartAngle);
            Assert.AreEqual(90f, one.SweepAngle);
        }

        [TestMethod]
        public void Arc_FillStyle_PassesNegativeAngles()
        {
            var f = CreateFixture();
            f.Src.Event.ObjFillStyle = SRCCore.VB.FillStyle.VbFSSolid;
            var cmds = BuildEvent(f.Src, "Arc 10 10 5 0 90");
            Assert.AreEqual(1, cmds[0].Exec());
            var call = f.Calls(nameof(IGUIScrean.ArcCmd)).Single();
            var arc = new ScreanArc((float)call.Args[4], (float)call.Args[5]);
            Assert.IsTrue(arc.IsPie);
            Assert.AreEqual(0f, arc.StartAngle, 0.001f);
            Assert.AreEqual(90f, arc.SweepAngle, 0.001f);
        }

        // ── ExecArrayCmd ──

        // サブルーチンローカル変数は数値型の 0 で作り直す
        [TestMethod]
        public void Array_SubLocalVariable_IsRecreatedAsNumeric()
        {
            var f = CreateFixture();
            var ev = f.Src.Event;
            ev.CallDepth = 1;
            ev.VarIndexStack[0] = 0;
            ev.VarIndex = 1;
            ev.VarStack[1].SetValue("arr", ValueType.StringType, "x", 0d);
            var cmds = BuildEvent(f.Src, "Array arr \"a,b\" \",\"");
            Assert.AreEqual(1, cmds[0].Exec());
            var v = ev.SubLocalVars().First(x => x.Name == "arr");
            Assert.AreEqual(ValueType.NumericType, v.VariableType);
            Assert.AreEqual("a", f.Src.Expression.GetValueAsString("arr[1]"));
            Assert.AreEqual("b", f.Src.Expression.GetValueAsString("arr[2]"));
        }

        // TrimString はタブ・空白・全角空白だけを除く（改行は残る）
        [TestMethod]
        public void Array_TrimsOnlyTabsAndSpaces()
        {
            var f = CreateFixture();
            f.Src.Expression.SetVariableAsString("src", "\t a 　,b\n");
            var cmds = BuildEvent(f.Src, "Array arr src \",\"");
            Assert.AreEqual(1, cmds[0].Exec());
            Assert.AreEqual("a", f.Src.Expression.GetValueAsString("arr[1]"));
            Assert.AreEqual("b\n", f.Src.Expression.GetValueAsString("arr[2]"));
        }

        // ── ExecAskCmd ──

        // 配列の書式で選択肢が無いときも SelectedAlternative は 0
        [TestMethod]
        public void Ask_ArrayFormWithoutItems_SelectsZero()
        {
            var f = CreateFixture();
            f.Src.Expression.DefineGlobalVariable("choices");
            f.Src.Event.SelectedAlternative = "x";
            var cmds = BuildEvent(f.Src, "Ask choices 選んでください");
            Assert.AreEqual(1, cmds[0].Exec());
            Assert.AreEqual("0", f.Src.Event.SelectedAlternative);
            Assert.AreEqual(0, f.Calls(nameof(IGUI.ListBox)).Count);
        }

        // ── ExecAttackCmd ──

        // 「自動」で反撃できる武器が無くてもエラーにならない（VB は IsTargetWithinRange(0) が False で選び直す）
        [TestMethod]
        public void Attack_AutoDefenseWithoutCounterWeapon_DoesNotFail()
        {
            using (UnitAttackFixture.FixDice(0.5))
            {
                var f = CreateFixture();
                var a = f.AddUnit("攻撃機", "味方", 2, 2, weapons: new[] { f.Weapon("ビームライフル", 100) });
                var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);
                var cmds = BuildEvent(f.Src, "Attack 攻撃機パイロット ビームライフル 標的機パイロット 自動");
                Assert.AreEqual(1, cmds[0].Exec());
                Assert.AreEqual(0, f.Src.DataErrors.Count, string.Join(",", Errors(f.Src)));
                Assert.IsTrue(t.HP < 5000);
            }
        }

        // ── ExecAutoTalkCmd ──

        [TestMethod]
        public void AutoTalk_TooManyArgs_UsesAutoTalkMessage()
        {
            var f = CreateFixture();
            TrackMessageForm(f);
            var cmds = BuildEvent(f.Src, "AutoTalk システム 1 2 3", "msg", "End");
            Assert.AreEqual(-1, cmds[0].Exec());
            CollectionAssert.Contains(Errors(f.Src).ToList(), "AutoTalkコマンドの引数の数が違います");
        }

        [TestMethod]
        public void AutoTalk_WithoutEnd_UsesAutoTalkMessage()
        {
            var f = CreateFixture();
            TrackMessageForm(f);
            var cmds = BuildEvent(f.Src, "AutoTalk システム", "msg");
            Assert.AreEqual(-1, cmds[0].Exec());
            CollectionAssert.Contains(Errors(f.Src).ToList(), "AutoTalkとEndが対応していません");
        }

        // AutoTalk の End は「枠外」を戻さない（Talk は戻す）
        [TestMethod]
        public void AutoTalk_End_KeepsMessageWindowIsOut()
        {
            var f = CreateFixture();
            TrackMessageForm(f);
            var cmds = BuildEvent(f.Src, "AutoTalk システム 枠外", "msg", "End");
            Assert.AreEqual(3, cmds[0].Exec());
            Assert.IsTrue(f.Gui.GUI.MessageWindowIsOut);
        }

        [TestMethod]
        public void Talk_End_ResetsMessageWindowIsOut()
        {
            var f = CreateFixture();
            TrackMessageForm(f);
            var cmds = BuildEvent(f.Src, "Talk システム 枠外", "msg", "End");
            Assert.AreEqual(3, cmds[0].Exec());
            Assert.IsFalse(f.Gui.GUI.MessageWindowIsOut);
        }

        // AutoTalk の座標指定は話者名を渡さない。座標が 0 なら中央表示しない
        [TestMethod]
        public void AutoTalk_ZeroCoordinates_DoesNotCenterSpeaker()
        {
            var f = CreateFixture();
            TrackMessageForm(f);
            f.AddUnit("攻撃機", "味方", 4, 5);
            var cmds = BuildEvent(f.Src, "AutoTalk 攻撃機パイロット 0 0", "msg", "End");
            Assert.AreEqual(3, cmds[0].Exec());
            Assert.AreEqual(0, f.Calls(nameof(IGUI.Center)).Count);
        }

        // プロローグでは話者の中央表示をせず、その行ではメッセージウィンドウを開かない
        [TestMethod]
        public void Talk_SpeakerInPrologue_DoesNotOpenMessageFormOnThatLine()
        {
            var f = CreateFixture();
            var opened = TrackMessageForm(f);
            f.Src.Stage = "プロローグ";
            var cmds = BuildEvent(f.Src, "AutoTalk システム", "End");
            Assert.AreEqual(2, cmds[0].Exec());
            Assert.AreEqual(0, opened());
        }

        // 引数の無い Talk は、メッセージウィンドウが開いていれば開き直さない
        [TestMethod]
        public void Talk_NoArgs_DoesNotReopenVisibleMessageForm()
        {
            var f = CreateFixture();
            var opened = TrackMessageForm(f);
            var cmds = BuildEvent(f.Src, "AutoTalk システム", "msg", "AutoTalk", "msg2", "End");
            Assert.AreEqual(5, cmds[0].Exec());
            Assert.AreEqual(1, opened());
        }

        // ── CenterUnit ──

        // 話者のユニットが出撃中なら、そのユニットだけを中央表示する（母艦へは移らない）
        [TestMethod]
        public void CenterUnit_SortiedSpeaker_DoesNotMoveToMothership()
        {
            var f = CreateFixture();
            TrackMessageForm(f);
            f.Gui.GUI.IsCursorVisible = true;
            f.AddUnit("母艦機", "味方", 8, 8, features: new[] { "母艦" });
            f.AddUnit("攻撃機", "味方", 4, 5);
            var cmds = BuildEvent(f.Src, "AutoTalk 攻撃機パイロット 中央", "msg", "End");
            Assert.AreEqual(3, cmds[0].Exec());
            var centers = f.Calls(nameof(IGUI.Center)).Select(x => ((int)x.Args[0], (int)x.Args[1])).ToList();
            CollectionAssert.AreEqual(new[] { (4, 5) }, centers);
        }

        // ── ExecCallCmd ──

        // 呼び出しの階層は CallDepth が MaxCallDepth を超えたときだけエラー
        [TestMethod]
        public void Call_AtMaxCallDepth_IsAllowed()
        {
            var f = CreateFixture();
            var cmds = BuildEvent(f.Src, "Call sub", "sub:", "Return");
            f.Src.Event.CallDepth = Event.MaxCallDepth;
            Assert.AreEqual(2, cmds[0].Exec());
            Assert.AreEqual(Event.MaxCallDepth + 1, f.Src.Event.CallDepth);
        }

        [TestMethod]
        public void Call_OverMaxCallDepth_IsError()
        {
            var f = CreateFixture();
            var cmds = BuildEvent(f.Src, "Call sub", "sub:", "Return");
            f.Src.Event.CallDepth = Event.MaxCallDepth + 1;
            Assert.AreEqual(-1, cmds[0].Exec());
            CollectionAssert.Contains(Errors(f.Src).ToList(), Event.MaxCallDepth + "階層を越えるサブルーチンの呼び出しは出来ません");
        }

        // 引数のスタックは MaxArgIndex 番目まで使える
        [TestMethod]
        public void Call_ArgsUpToMaxArgIndex_AreAllowed()
        {
            var f = CreateFixture();
            var cmds = BuildEvent(f.Src, "Call sub 1", "sub:", "Return");
            f.Src.Event.CallDepth = 1;
            f.Src.Event.ArgIndex = Event.MaxArgIndex - 1;
            Assert.AreEqual(2, cmds[0].Exec());
            Assert.AreEqual(Event.MaxArgIndex, f.Src.Event.ArgIndex);
            Assert.AreEqual("1", f.Src.Event.ArgStack[Event.MaxArgIndex]);
        }

        // ── ExecCallInterMissionCommandCmd ──

        // ステータス表示のイベントが無いときは知らせて次の行へ進む（インターミッションのコマンドと同じ。元の SRC は読み込みのエラーで終了）
        [TestMethod]
        public void CallInterMissionCommand_StatusEveMissing_ShowsErrorAndContinues()
        {
            var f = CreateFixture();
            f.Src.ScenarioPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));
            var cmds = BuildEvent(f.Src, "CallInterMissionCommand パイロットステータス");
            Assert.AreEqual(1, cmds[0].Exec());
            CollectionAssert.AreEqual(new[] { @"Lib\パイロットステータス表示.eve が見つかりません" },
                f.Calls(nameof(IGUI.ErrorMessage)).Select(x => (string)x.Args[0]).ToList());
            Assert.AreEqual(0, f.Calls(nameof(IGUI.CloseListBox)).Count);
            Assert.IsFalse(f.Src.IsScenarioFinished);
        }

        // ── ExecChangeAreaCmd ──

        // マップの外のユニットは地形で制限しない
        [TestMethod]
        public void ChangeArea_UnitOutsideMap_SetsArea()
        {
            var f = CreateFixture();
            var u = f.AddUnit("攻撃機", "味方", 2, 2);
            u.Escape();
            u.x = 0;
            u.y = 0;
            var cmds = BuildEvent(f.Src, "ChangeArea 攻撃機パイロット 空中");
            Assert.AreEqual(1, cmds[0].Exec());
            Assert.AreEqual("空中", u.Area);
        }
    }
}
