using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// PlayScript の turns / ScriptedPhase が、行動の結果としてシナリオ終了（GameOver / GameClear）になった場合や、
    /// 地形データが無い環境で移動する場合でも誤った警告や停止を起こさずに動くことを確かめる（7474/SRC#1668）。
    /// </summary>
    [TestClass]
    public class ScriptedPhaseTests
    {
        private static void CopyDirectory(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in Directory.GetFiles(from))
            {
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)), true);
            }
            foreach (var dir in Directory.GetDirectories(from))
            {
                CopyDirectory(dir, Path.Combine(to, Path.GetFileName(dir)));
            }
        }

        private static void WriteMinimalBattleData(ScenarioWorkspace workspace, int allySpeed, int weaponMinRange, int weaponMaxRange)
        {
            CopyDirectory(
                Path.Combine(Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory), "Content", "basic-data", "Lib"),
                Path.Combine(workspace.AppPath, "Lib"));
            workspace.Write(Path.Combine("Data", "テスト", "pilot.txt"),
                "テストパイロット",
                "テスト, てすと, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "180, 180, 180, 180, 180, 180, 普通",
                "ＳＰ, 50",
                "-.bmp, -.mid",
                "",
                "標的パイロット",
                "標的, ひょうてき, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "100, 100, 100, 100, 100, 100, 普通",
                "ＳＰ, 10",
                "-.bmp, -.mid",
                "");
            workspace.Write(Path.Combine("Data", "テスト", "robot.txt"),
                "テスト機",
                "テスト機, てすとき, 汎用, 1, 2",
                $"陸, {allySpeed}, M, 1000, 100",
                "特殊能力なし",
                "1000, 100, 600, 60",
                "AAAA, -.bmp",
                $"テストパンチ, 3000, {weaponMinRange}, {weaponMaxRange}, +50, -, -, -, AAAA, +10, Ｐ",
                "===",
                "",
                "標的機",
                "標的機, ひょうてきき, 汎用, 1, 2",
                "陸, 2, M, 1000, 100",
                "特殊能力なし",
                "800, 50, 300, 10",
                "AAAA, -.bmp",
                "標的パンチ, 600, 1, 1, +0, -, -, -, AAAA, +0, 突",
                "===",
                "");
        }

        [TestMethod]
        public void Turns_GameOverDuringPostMoveAttack_StopsCleanlyWithoutSelectionOrReadyError()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                WriteMinimalBattleData(workspace, allySpeed: 4, weaponMinRange: 1, weaponMaxRange: 1);
                workspace.WriteMap("test.map", 15, 15, (x, y) => 0);
                // 味方が (4,6) から移動して (7,6) の敵を攻撃すると破壊イベントで GameOver になる
                // FinishAttackCommand が IsScenarioFinished により WaitCommand 前に抜けるため CommandState は移動後ターゲット選択のまま終了する
                workspace.Write("test.eve",
                    "@テスト",
                    "プロローグ:",
                    "Exit",
                    "",
                    "スタート:",
                    "ChangeMap test.map",
                    "Create 味方 テスト機 0 テストパイロット 20 4 6",
                    "Create 敵 標的機 0 標的パイロット 1 7 6",
                    "Talk システム",
                    "戦闘開始",
                    "End",
                    "Exit",
                    "",
                    "破壊 標的パイロット:",
                    "Talk システム",
                    "敗北した",
                    "End",
                    "GameOver",
                    "");

                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    try
                    {
                        script.Run(new[]
                        {
                            "auto 30",
                            "turns 5",
                            "expect finished",
                            "expect message 敗北した",
                            "expect error none",
                        });
                        Assert.IsFalse(
                            script.TraceLog.Any(x => x.Contains("選択が残ったので取り消す")),
                            string.Join(Environment.NewLine, script.TraceLog));
                        Assert.IsFalse(
                            script.TraceLog.Any(x => x.Contains("操作を受け付ける状態になりません")),
                            string.Join(Environment.NewLine, script.TraceLog));
                    }
                    catch (Exception ex) when (!(ex is AssertFailedException))
                    {
                        Assert.Fail(ex.Message + Environment.NewLine
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
                }
            }
        }

        [TestMethod]
        public void Turns_GameClearDuringPostMoveAttack_StopsCleanlyWithoutSelectionOrReadyError()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                WriteMinimalBattleData(workspace, allySpeed: 4, weaponMinRange: 1, weaponMaxRange: 1);
                workspace.WriteMap("test.map", 15, 15, (x, y) => 0);
                workspace.Write("test.eve",
                    "@テスト",
                    "プロローグ:",
                    "Exit",
                    "",
                    "スタート:",
                    "ChangeMap test.map",
                    "Create 味方 テスト機 0 テストパイロット 20 4 6",
                    "Create 敵 標的機 0 標的パイロット 1 7 6",
                    "Talk システム",
                    "戦闘開始",
                    "End",
                    "Exit",
                    "",
                    "全滅 敵:",
                    "Talk システム",
                    "勝利した",
                    "End",
                    "GameClear",
                    "");

                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    try
                    {
                        script.Run(new[]
                        {
                            "auto 30",
                            "turns 5",
                            "expect finished",
                            "expect message 勝利した",
                            "expect error none",
                        });
                        Assert.IsFalse(
                            script.TraceLog.Any(x => x.Contains("選択が残ったので取り消す")),
                            string.Join(Environment.NewLine, script.TraceLog));
                        Assert.IsFalse(
                            script.TraceLog.Any(x => x.Contains("操作を受け付ける状態になりません")),
                            string.Join(Environment.NewLine, script.TraceLog));
                    }
                    catch (Exception ex) when (!(ex is AssertFailedException))
                    {
                        Assert.Fail(ex.Message + Environment.NewLine
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
                }
            }
        }

        [TestMethod]
        public void Turns_WithoutTerrainData_DoesNotDoubleMovementRange()
        {
            // terrain.txt が無い状態（AllowMissingAssets）で、移動力 2・射程 1 のユニットは
            // 距離 4 の敵に 1 ターン目では届かない（移動コスト 1 = MoveCost 2 のため）
            using (var workspace = new ScenarioWorkspace(withAssets: false))
            {
                WriteMinimalBattleData(workspace, allySpeed: 2, weaponMinRange: 1, weaponMaxRange: 1);
                workspace.WriteMap("test.map", 15, 15, (x, y) => 0);
                workspace.Write("test.eve",
                    "@テスト",
                    "プロローグ:",
                    "Exit",
                    "",
                    "スタート:",
                    "ChangeMap test.map",
                    "Create 味方 テスト機 0 テストパイロット 20 4 6",
                    "Create 敵 標的機 0 標的パイロット 1 8 6",
                    "Talk システム",
                    "戦闘開始",
                    "End",
                    "Exit",
                    "",
                    "ターン 2 味方:",
                    "Talk システム",
                    "2ターン目",
                    "End",
                    "Quit",
                    "");

                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    try
                    {
                        script.Run(new[]
                        {
                            "auto 30",
                            "turns 2",
                            "expect finished",
                            "expect message 2ターン目",
                            "expect error none",
                        });
                        // 1 ターン目は移動のみ（倍の移動範囲 4 になっていれば 1 ターン目で (7,6) まで届いて攻撃してしまう）
                        Assert.IsTrue(script.TraceLog.Any(x => x.Contains("PHASE 移動: (6,6)")), string.Join(Environment.NewLine, script.TraceLog));
                        Assert.IsFalse(script.TraceLog.Any(x => x.Contains("PHASE 攻撃:")), string.Join(Environment.NewLine, script.TraceLog));
                    }
                    catch (Exception ex) when (!(ex is AssertFailedException))
                    {
                        Assert.Fail(ex.Message + Environment.NewLine
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
                }
            }
        }
    }
}
