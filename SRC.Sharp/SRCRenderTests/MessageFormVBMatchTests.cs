using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCCore.Units;
using SRCPlay;
using SRCRender.Compat;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using PointF = System.Drawing.PointF;

namespace SRCRender.Tests
{
    /// <summary>
    /// メッセージウィンドウ（元の SRC の GUI.bas の OpenMessageForm〜DisplaySysMessage）を、元の SRC の動作に合わせたことを確かめる（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class MessageFormVBMatchTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
            workspace.Write(Path.Combine("Data", "robot.txt"),
                "標的機",
                "標的機, ひょうてきき, 汎用, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                "1000, 100, 1000, 60",
                "AAAA, -.bmp",
                "===",
                "");
            workspace.Write(Path.Combine("Data", "pilot.txt"),
                "テスト",
                "テスト, てすと, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "100, 100, 100, 100, 100, 100, 普通",
                "ＳＰなし",
                "-.bmp, -.mid",
                "",
                "顔なし",
                "顔なし, かおなし, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "100, 100, 100, 100, 100, 100, 普通",
                "ＳＰなし",
                "missing_face.bmp, -.mid",
                "");
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 味方 標的機 0 テスト 1 3 3",
                "Refresh",
                "Talk システム",
                "待機",
                "End",
                "Quit",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        private static void RunOnGame(ScreenScenarioRunner runner, Action action)
        {
            Exception error = null;
            using (var done = new ManualResetEventSlim())
            {
                runner.Session.Game.Post(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    finally
                    {
                        done.Set();
                    }
                });
                Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
            }
            Assert.IsNull(error, error?.ToString());
        }

        private ScreenScenarioRunner StartAtMessage()
        {
            var runner = new ScreenScenarioRunner(workspace.AppPath);
            runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
            runner.WaitForMessage();
            return runner;
        }

        private static object CallPrivate(ScreenGUI gui, string name)
        {
            return typeof(ScreenGUI).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null).Invoke(gui, null);
        }

        private static PointF CurrentMessagePoint(ScreenGUI gui)
        {
            return (PointF)typeof(ScreenGUI).GetField("currentMessagePoint", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gui);
        }

        private static float TextWidth(string text, float size = 12f)
        {
            using (var bitmap = new SKBitmap(1, 1))
            using (var g = Graphics.FromImage(bitmap))
            {
                return g.MeasureString(text, new Font("ＭＳ Ｐ明朝", size, FontStyle.Regular, GraphicsUnit.Point)).Width;
            }
        }

        private static string Repeat(string s, int count) => string.Concat(Enumerable.Repeat(s, count));

        // メッセージ欄の幅（元の SRC の picMessage.width）
        private static int MessageWidth(ScreenScenarioRunner runner) => runner.GUI.MessageWindow.picMessage.Width;

        [TestMethod]
        public void OpenMessageForm_KeepsDisplayedUnits_ForKeepMessageFormStatus()
        {
            // 元の SRC の OpenMessageForm は最初に LeftUnit・RightUnit を消し、UpdateMessageForm で表示したユニットを残す。
            // 戦闘中にメッセージウィンドウが閉じても、KeepMessageFormStatus がユニットの表示付きで開き直す
            using (var runner = StartAtMessage())
            {
                RunOnGame(runner, () =>
                {
                    var src = runner.Session.SRC;
                    var attacker = src.Map.MapDataForUnit[3, 3];
                    var defender = src.UList.Add("標的機", 0, "敵");
                    src.PList.Add("テスト", 1, "敵", "defender").Ride(defender);
                    defender.x = 8;
                    defender.y = 3;
                    defender.Update();
                    defender.FullRecover();
                    var gui = runner.GUI;
                    gui.OpenMessageForm(attacker, defender);
                    gui.SaveMessageFormStatus();
                    gui.CloseMessageForm();
                    gui.KeepMessageFormStatus();
                });

                var window = runner.GUI.MessageWindow;
                Assert.IsTrue(window.Visible);
                Assert.IsTrue(window.picUnit1.Visible, "左のユニットを表示する");
                Assert.IsTrue(window.picUnit2.Visible, "右のユニットを表示する");
                Assert.AreEqual(118, window.Height);
            }
        }

        [TestMethod]
        public void DisplayMessage_MissingFace_RecordsIsBitmapMissing()
        {
            // 元の SRC の DisplayMessage は、パイロットの画像が描けなければ IsBitmapMissing を立てる（以後 Bitmap が "-.bmp" になる）
            using (var runner = StartAtMessage())
            {
                var missing = false;
                RunOnGame(runner, () =>
                {
                    runner.GUI.MessageWait = 0;
                    runner.GUI.AutoMessageMode = true;
                    runner.GUI.DisplayMessage("顔なし", "こんにちは", "");
                    runner.GUI.AutoMessageMode = false;
                    missing = runner.Session.SRC.PDList.Item("顔なし").IsBitmapMissing;
                });
                Assert.IsTrue(missing);
            }
        }

        [TestMethod]
        public void DisplayBattleMessage_PrintsSpeakerNameOnItsOwnLine()
        {
            // 元の SRC は話者名を p.Print（改行あり）で書き、せりふは次の行から書く
            using (var runner = StartAtMessage())
            {
                string[] lines = null;
                RunOnGame(runner, () =>
                {
                    runner.GUI.MessageWait = 0;
                    runner.GUI.DisplayBattleMessage("テスト", "よし", "");
                    lines = runner.GUI.State.Message.Lines.ToArray();
                });
                CollectionAssert.AreEqual(new[] { "テスト", "「よし」" }, lines);
            }
        }

        [TestMethod]
        public void DisplayBattleMessage_OverflowBreak_MovesCurrentCharacterToNextLine()
        {
            // 元の SRC の DisplayBattleMessage は、メッセージ長の超過で改行するとき、いまの文字を次の行に回す（line_head = j）
            using (var runner = StartAtMessage())
            {
                string[] lines = null;
                var msg = Repeat("あ", 40);
                RunOnGame(runner, () =>
                {
                    runner.GUI.MessageWait = 0;
                    runner.GUI.DisplayBattleMessage("システム", msg, "");
                    lines = runner.GUI.State.Message.Lines.ToArray();
                });
                Assert.AreEqual(msg, string.Concat(lines));
                var limit = 0.8f * MessageWidth(runner);
                var n = lines[0].Length;
                Assert.IsTrue(TextWidth(Repeat("あ", n)) > limit, "1 行目は超過した幅まで書く");
                Assert.IsTrue(TextWidth(Repeat("あ", n - 1)) <= limit, "超過した文字の次の文字は次の行に回す");
            }
        }

        [TestMethod]
        public void DisplayBattleMessage_SpaceBreak_DropsSpace()
        {
            // 元の SRC の DisplayBattleMessage は、空白で改行するときは空白を書かない。改行するのは "。" と同じ位置を超えたとき
            using (var runner = StartAtMessage())
            {
                var width = MessageWidth(runner);
                var k = Enumerable.Range(1, 40).First(x => TextWidth(Repeat("あ", x)) > 0.6f * width);
                if (TextWidth(Repeat("あ", k)) > 0.8f * width)
                {
                    Assert.Inconclusive("フォントの幅が想定と違う");
                }
                string[] lines = null;
                var msg = Repeat("あ", k) + " " + Repeat("い", 30);
                RunOnGame(runner, () =>
                {
                    runner.GUI.MessageWait = 0;
                    runner.GUI.DisplayBattleMessage("システム", msg, "");
                    lines = runner.GUI.State.Message.Lines.ToArray();
                });
                Assert.AreEqual(Repeat("あ", k), lines[0]);
                StringAssert.StartsWith(lines[1], "い");
            }
        }

        [TestMethod]
        public void DisplayBattleMessage_HiddenWindow_StillPrints()
        {
            // 元の SRC はメッセージウィンドウが最小化されているときだけ表示をやめる（隠れているときは書いて待つ）
            using (var runner = StartAtMessage())
            {
                string[] lines = null;
                var visible = true;
                RunOnGame(runner, () =>
                {
                    runner.GUI.MessageWait = 0;
                    runner.GUI.CloseMessageForm();
                    runner.GUI.DisplayBattleMessage("システム", "テスト", "");
                    lines = runner.GUI.State.Message.Lines.ToArray();
                    visible = runner.GUI.MessageFormVisible;
                });
                CollectionAssert.AreEqual(new[] { "テスト" }, lines);
                Assert.IsFalse(visible, "ウィンドウは隠れたまま");
            }
        }

        [TestMethod]
        public void DisplaySysMessage_FitsWithinWholeWidth_OneLine()
        {
            // 元の SRC の DisplaySysMessage は、全体が picMessage の幅に収まれば改行しない（0.95 ではなく 1 倍）
            using (var runner = StartAtMessage())
            {
                var width = MessageWidth(runner);
                string msg = null;
                for (var a = 1; a < 30 && msg == null; a++)
                {
                    if (TextWidth(Repeat("あ", a)) <= 0.5f * width)
                    {
                        continue;
                    }
                    for (var b = 0; b < 30; b++)
                    {
                        var s = Repeat("あ", a) + " " + Repeat("あ", b);
                        var w = TextWidth(s);
                        if (w > 0.95f * width && w < width)
                        {
                            msg = s;
                            break;
                        }
                    }
                }
                if (msg == null)
                {
                    Assert.Inconclusive("フォントの幅が想定と違う");
                }
                string[] lines = null;
                RunOnGame(runner, () =>
                {
                    runner.GUI.MessageWait = 0;
                    runner.GUI.DisplaySysMessage(msg, false);
                    lines = runner.GUI.State.Message.Lines.ToArray();
                });
                CollectionAssert.AreEqual(new[] { msg }, lines);
            }
        }

        [TestMethod]
        public void DisplaySysMessage_HiddenWindow_StillPrints()
        {
            using (var runner = StartAtMessage())
            {
                string[] lines = null;
                RunOnGame(runner, () =>
                {
                    runner.GUI.MessageWait = 0;
                    runner.GUI.CloseMessageForm();
                    runner.GUI.DisplaySysMessage("テスト", false);
                    lines = runner.GUI.State.Message.Lines.ToArray();
                });
                CollectionAssert.AreEqual(new[] { "テスト" }, lines);
            }
        }

        [TestMethod]
        public void PrintMessage_LtTag_AdvancesByPrintedCharacter()
        {
            // <lt> で書いた "<" の分だけ次の文字を右に書く（元の SRC の p.Print "<";）
            using (var runner = StartAtMessage())
            {
                byte[] withTag = null, plain = null;
                RunOnGame(runner, () =>
                {
                    var gui = runner.GUI;
                    var pic = gui.MessageWindow.picMessage;
                    CallPrivate(gui, "ResetMessageArea");
                    CallPrivate(gui, "ResetFont");
                    gui.PrintMessage("<lt>|", false);
                    withTag = pic.Image.Bytes.ToArray();
                    CallPrivate(gui, "ResetMessageArea");
                    CallPrivate(gui, "ResetFont");
                    // 閉じていないタグはそのまま 1 度に書く
                    gui.PrintMessage("<|", false);
                    plain = pic.Image.Bytes.ToArray();
                });
                CollectionAssert.AreEqual(plain, withTag);
            }
        }

        [TestMethod]
        public void PrintMessage_SmallTag_NextLineFollowsSmallFont()
        {
            // 元の SRC の PrintMessage は max_y を 0 から数えるので、<small> だけの行の次の行は小さい文字の高さの下から書く
            using (var runner = StartAtMessage())
            {
                PointF start = default, end = default;
                RunOnGame(runner, () =>
                {
                    var gui = runner.GUI;
                    CallPrivate(gui, "ResetMessageArea");
                    CallPrivate(gui, "ResetFont");
                    start = CurrentMessagePoint(gui);
                    gui.PrintMessage("<small>あ", false);
                    end = CurrentMessagePoint(gui);
                    CallPrivate(gui, "ResetFont");
                });
                var smallHeight = new Font("ＭＳ Ｐ明朝", 10f, FontStyle.Regular, GraphicsUnit.Point).GetHeight();
                Assert.AreEqual(start.Y + smallHeight + 1, end.Y, 0.01);
                Assert.AreEqual(1f, start.X, "元の SRC と同じく CurrentX = 1 から書く");
                Assert.AreEqual(1f, end.X);
            }
        }
    }
}
