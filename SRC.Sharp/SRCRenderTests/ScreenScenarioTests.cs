using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SRCCore.Units;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// ScreenGUI でシナリオをプレイする E2E テストと VRT。
    /// </summary>
    [TestClass]
    public class ScreenScenarioTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            // 左半分が平地、右半分が海の 20x20 マップ
            workspace.WriteMap("test.map", 20, 20, (x, y) => x <= 10 ? 0 : 1);
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Talk システム",
                "ヘッドレステスト開始",
                "End",
                "Ask 選択してください",
                "はい",
                "いいえ",
                "End",
                "If 選択 = 1 Then",
                "Talk システム",
                "はいが選ばれた",
                "End",
                "EndIf",
                "Quit",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        [TestMethod]
        public void PlayScenario_MessagesListAndMap()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                Assert.AreEqual("ヘッドレステスト開始", runner.WaitForMessage());
                Assert.IsTrue(runner.State.ShowMap, "map is visible");
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "scenario-01-message");
                runner.ClickMessage();

                var list = runner.WaitForList();
                Assert.AreEqual("選択", list.Caption);
                CollectionAssert.AreEqual(new[] { "はい", "いいえ" }, new[] { list.Items[0].Text, list.Items[1].Text });
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "scenario-02-list");
                runner.ChooseListItem(0);

                Assert.AreEqual("はいが選ばれた", runner.WaitForMessage());
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "scenario-03-after-choice");
                runner.ClickMessage();

                runner.WaitForTermination();
                Assert.IsTrue(runner.IsTerminated);
            }
        }

        [TestMethod]
        public void SystemAliasFile_HidesAliasedFeature()
        {
            // 元の SRC と同じく Data\System\alias.txt を読み込み、エリアスで「非表示」にした能力を表示しない。7474/SRC#1425
            workspace.Write(Path.Combine("Data", "System", "alias.txt"),
                "隠しバリア",
                "バリア=非表示 全 1000 10",
                "");
            workspace.Write(Path.Combine("Data", "robot.txt"),
                "標的機",
                "標的機, ひょうてきき, 汎用, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力",
                "隠しバリア, 分身",
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
                "");
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 味方 標的機 0 テスト 1 3 3",
                "Talk システム",
                "待機",
                "End",
                "Quit",
                "");

            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();

                Assert.IsTrue(runner.Session.SRC.ALDList.IsDefined("隠しバリア"));
                var unit = runner.Session.SRC.Map.MapDataForUnit[3, 3];
                Assert.IsTrue(unit.IsFeatureAvailable("バリア"));
                CollectionAssert.AreEqual(new[] { "分身" },
                    unit.StatusFeatures().Select(x => x.Name).Where(x => !string.IsNullOrEmpty(x)).ToArray());
            }
        }

        [TestMethod]
        public void MessageWindowStatusAnimation_DisplaysIntegerHPAndEN()
        {
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
                "");
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 敵 標的機 0 テスト 1 3 3",
                "Refresh",
                "Talk システム",
                "待機",
                "End",
                "Quit",
                "");

            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();

                var unit = runner.Session.SRC.Map.MapDataForUnit[3, 3];
                var labels = new[]
                {
                    runner.GUI.MessageWindow.txtHP1,
                    runner.GUI.MessageWindow.txtHP2,
                    runner.GUI.MessageWindow.txtEN1,
                    runner.GUI.MessageWindow.txtEN2,
                };
                var observed = labels.ToDictionary(label => label, _ => new List<string>());
                System.Exception updateError = null;
                using (var done = new ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        try
                        {
                            runner.GUI.OpenMessageForm(unit, unit);
                            runner.GUI.UpdateMessageForm(unit, unit);
                            foreach (var label in labels)
                            {
                                var refreshed = label.Refreshed;
                                label.Refreshed = () =>
                                {
                                    observed[label].Add(label.Text);
                                    refreshed?.Invoke();
                                };
                            }
                            unit.HP = 777;
                            unit.EN = 73;
                            runner.GUI.UpdateMessageForm(unit, unit);
                        }
                        catch (System.Exception ex)
                        {
                            updateError = ex;
                        }
                        finally
                        {
                            done.Set();
                        }
                    });
                    Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout), "HP/EN animation completed");
                }

                Assert.IsNull(updateError, updateError?.ToString());
                foreach (var values in observed.Values)
                {
                    Assert.IsTrue(values.Count > 0, "each HP/EN field was refreshed");
                    Assert.IsFalse(values.Any(value => value.Contains(".")), string.Join(", ", values));
                }
            }
        }

        [TestMethod]
        public void UpdateMessageForm_ReflectionKeepsBattleUnitOrder()
        {
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

            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();

                Unit attacker = null;
                Exception updateError = null;
                using (var done = new ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        try
                        {
                            attacker = runner.Session.SRC.Map.MapDataForUnit[3, 3];
                            attacker.HP = 500;
                            var src = runner.Session.SRC;
                            var defender = src.UList.Add("標的機", 0, "敵");
                            src.PList.Add("テスト", 1, "敵", "defender").Ride(defender);
                            defender.x = 8;
                            defender.y = 3;
                            defender.Update();
                            defender.FullRecover();
                            runner.GUI.OpenMessageForm(attacker, defender);
                        }
                        catch (Exception ex)
                        {
                            updateError = ex;
                        }
                        finally
                        {
                            done.Set();
                        }
                    });
                    Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
                }
                Assert.IsNull(updateError, updateError?.ToString());
                var before = runner.SnapshotPng();

                runner.Session.Game.Post(() => runner.GUI.UpdateMessageForm(attacker, null));
                runner.Sync();

                CollectionAssert.AreEqual(before, runner.SnapshotPng());
            }
        }

        [TestMethod]
        public void UpdateMessageForm_SingleUnitRedrawsLikeOmittedArgument()
        {
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

            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();

                Unit attacker = null;
                Exception updateError = null;
                using (var done = new ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        try
                        {
                            attacker = runner.Session.SRC.Map.MapDataForUnit[3, 3];
                            attacker.HP = 500;
                            var src = runner.Session.SRC;
                            var defender = src.UList.Add("標的機", 0, "敵");
                            src.PList.Add("テスト", 1, "敵", "defender").Ride(defender);
                            defender.x = 8;
                            defender.y = 3;
                            defender.Update();
                            defender.FullRecover();
                            runner.GUI.OpenMessageForm(attacker, defender);
                        }
                        catch (Exception ex)
                        {
                            updateError = ex;
                        }
                        finally
                        {
                            done.Set();
                        }
                    });
                    Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
                }
                Assert.IsNull(updateError, updateError?.ToString());
                var before = runner.SnapshotPng();

                // 元の SRC で u2 を省略した UpdateMessageForm と同じく、前回の表示を維持せずに 1 体だけの表示にする
                // （u2 に Nothing を渡す反射攻撃の場合とは異なる）
                runner.Session.Game.Post(() => runner.GUI.UpdateMessageForm(attacker));
                runner.Sync();

                CollectionAssert.AreNotEqual(before, runner.SnapshotPng());
            }
        }

        [TestMethod]
        public void PlayScenario_RightClickOnAskWithoutCancelOption_ShowsListAgain()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                runner.ClickMessage();

                // キャンセル可オプションの無い Ask は、右クリックしても選択肢を出し直す
                var first = runner.WaitForList();
                runner.GUI.PointerPressed(10, 10, SRCCore.GuiButton.Right);
                runner.GUI.PointerReleased(10, 10, SRCCore.GuiButton.Right);
                runner.WaitFor(s => s.List != null && s.List != first, "list shown again");

                // 「いいえ」を選ぶとメッセージを出さずに終了する
                runner.ChooseListItem(1);
                runner.WaitForTermination();
                Assert.IsTrue(runner.IsTerminated);
            }
        }
    
        [TestMethod]
        public void VisualAssert_DetectsDifference()
        {
            if (!VisualAssert.IsBaselineEnvironment)
            {
                Assert.Inconclusive("VRT の基準環境ではありません");
            }
            if (VisualAssert.IsUpdateMode)
            {
                // 基準画像を上書きしてしまうため、更新モードでは実行しない
                Assert.Inconclusive("基準画像の更新中です");
            }
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                // 基準画像と異なる画面（メッセージ送り後の選択肢）と比較すると失敗する
                runner.ClickMessage();
                runner.WaitForList();
                var png = runner.SnapshotPng();
                Assert.ThrowsExactly<AssertFailedException>(() => VisualAssert.MatchesBaseline(png, "scenario-01-message"));
            }
        }
    }
}
