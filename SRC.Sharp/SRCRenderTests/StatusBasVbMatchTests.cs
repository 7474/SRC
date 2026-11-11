using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCPlay;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// ステータスウィンドウの表示を元の SRC（Status.bas の DisplayGlobalStatus・DisplayUnitStatus・DisplayPilotStatus）と
    /// 突き合わせる（7474/SRC#1498）。文字の大きさ・位置は、元の SRC の ＭＳ 明朝（9 ポイントは 1 行 12 ピクセル、半角 6 ピクセル）で考える。
    /// </summary>
    [TestClass]
    public class StatusBasVbMatchTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
            workspace.Write(Path.Combine("Data", "robot.txt"),
                "検証機",
                "検証機, けんしょうき, 汎用, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力",
                "盾",
                "ハイパーモード=ハイパーモード 検証機改",
                "3000, 100, 600, 60",
                "AAAA, -.bmp",
                "弾ビーム少, 500, 1, 2, +0, 5, -, -, AAAA, +0, Ｂ",
                "弾ビーム多, 500, 1, 2, +0, 10, -, -, AAAA, +0, Ｂ",
                "強パンチ, 300, 1, 1, +0, -, -, -, AAAA, +0, 突",
                "===",
                "自己修理, 回復Lv1, 0, 1, -, -, -",
                "修理ビーム, 回復Lv1, 1, 3, -, -, -",
                "",
                "検証機改",
                "検証機改, けんしょうきかい, 汎用, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力",
                "ノーマルモード=検証機",
                "3000, 100, 600, 60",
                "AAAA, -.bmp",
                "強パンチ, 300, 1, 1, +0, -, -, -, AAAA, +0, 突",
                "");
            workspace.Write(Path.Combine("Data", "pilot.txt"),
                "検証パイロット",
                "検証, けんしょう, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "120, 130, 140, 150, 160, 170, 普通",
                "ＳＰ, 10",
                "-.bmp, -.mid",
                "");
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "RenameTerm レベル ＬＶ",
                "RenameTerm 気力 ＭＲ",
                "Create 味方 検証機 0 検証パイロット 1 3 3",
                "Exit",
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

        private ScreenScenarioRunner StartRunner()
        {
            var runner = new ScreenScenarioRunner(workspace.AppPath);
            runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
            var src = runner.Session.SRC;
            runner.WaitFor(s => !runner.GUI.IsGUILocked && src.Commands.CommandState == "ユニット選択" && !runner.Session.Game.IsBusy,
                "ユニット選択");
            return runner;
        }

        private static (string Box, float X, float Y, string Text, System.Drawing.Color Color) Find(ScreenScenarioRunner runner, string box, Func<string, bool> match)
        {
            var found = runner.GUI.StatusTextLog.Where(x => x.Box == box && match(x.Text)).ToList();
            Assert.IsTrue(found.Count > 0, "見つからない: " + string.Join(" / ", runner.GUI.StatusTextLog.Select(x => $"{x.Box}:{x.X},{x.Y}:{x.Text}")));
            return found[0];
        }

        [TestMethod]
        public void UnitStatus_MatchesVbLayout()
        {
            using (var runner = StartRunner())
            {
                var src = runner.Session.SRC;
                RunOnGame(runner, () => runner.GUI.DisplayUnitStatus(src.Map.MapDataForUnit[3, 3]));

                // パイロットの欄: レベル・気力は Term で書く（RenameTerm が効く）
                Find(runner, "pilot", x => x == "ＬＶ ");
                Find(runner, "pilot", x => x == "ＭＲ ");

                // 格闘と射撃、命中と回避、技量と反応は同じ行に続けて書く（左の欄は 19 桁 = 114 ピクセル）
                var infight = Find(runner, "unit", x => x.StartsWith("格闘"));
                var shooting = Find(runner, "unit", x => x.StartsWith("射撃"));
                var hit = Find(runner, "unit", x => x.StartsWith("命中"));
                var dodge = Find(runner, "unit", x => x.StartsWith("回避"));
                Assert.AreEqual(0f, infight.X);
                Assert.AreEqual(infight.Y, shooting.Y);
                Assert.AreEqual(114f, shooting.X, 0.5f);
                Assert.AreEqual(infight.Y + 12f, hit.Y, 0.5f);
                Assert.AreEqual(hit.Y, dodge.Y);
                Assert.AreEqual(114f, dodge.X, 0.5f);

                // 装甲と運動性、タイプと移動力、適応とサイズも同じ行
                var armor = Find(runner, "unit", x => x.StartsWith("装甲"));
                var mobility = Find(runner, "unit", x => x.StartsWith("運動性"));
                var adaption = Find(runner, "unit", x => x.StartsWith("適応"));
                var size = Find(runner, "unit", x => x.StartsWith("サイズ"));
                Assert.AreEqual(armor.Y, mobility.Y);
                Assert.AreEqual(114f, mobility.X, 0.5f);
                Assert.AreEqual(adaption.Y, size.Y);
                Assert.AreEqual(114f, size.X, 0.5f);

                // ＨＰ と ＥＮ は別の行
                var hp = Find(runner, "unit", x => x.StartsWith("ＨＰ"));
                var en = Find(runner, "unit", x => x.StartsWith("ＥＮ"));
                Assert.AreEqual(hp.Y + 12f, en.Y, 0.5f);

                // ＨＰのゲージ: x = 116 の枠は RGB(100, 100, 100)、中は値の割合だけ RGB(0, 210, 0)
                var image = runner.GUI.UnitStatusImage;
                var frame = image.GetPixel(116, (int)hp.Y + 5);
                Assert.AreEqual((byte)100, frame.Red);
                Assert.AreEqual((byte)100, frame.Green);
                var bar = image.GetPixel(160, (int)hp.Y + 5);
                Assert.AreEqual((byte)0, bar.Red);
                Assert.AreEqual((byte)210, bar.Green);

                // レベル指定の無い盾は Lv1（AllFeatureLevel）として「盾(1/1)」
                Find(runner, "unit", x => x.StartsWith("盾(1/1)"));

                // レベル指定の無いハイパーモードは気力 110 が要る。気力 100 では赤（RGB(150, 0, 0)）
                var hyper = Find(runner, "unit", x => x.StartsWith("ハイパーモード"));
                Assert.AreEqual(System.Drawing.Color.FromArgb(150, 0, 0).ToArgb(), hyper.Color.ToArgb());

                // 武器は攻撃力の小さい順。攻撃力が同じで弾数のある武器は、弾数の多い順（元の SRC の挿入ソート）
                var weapons = runner.GUI.StatusTextLog
                    .Where(x => x.Box == "unit" && (x.Text.StartsWith("強パンチ") || x.Text.StartsWith("弾ビーム")))
                    .OrderBy(x => x.Y)
                    .Select(x => x.Text.Substring(0, 5))
                    .ToArray();
                CollectionAssert.AreEqual(new[] { "強パンチ ", "弾ビーム多", "弾ビーム少" }, weapons);

                // 射程 0 のアビリティ（「    -」）の後は改行する
                var selfRepair = Find(runner, "unit", x => x.StartsWith("自己修理"));
                var repairBeam = Find(runner, "unit", x => x.StartsWith("修理ビーム"));
                Assert.AreEqual(selfRepair.Y + 12f, repairBeam.Y, 0.5f);
                Assert.AreEqual(0f, repairBeam.X);
            }
        }

        [TestMethod]
        public void GlobalStatus_MatchesVbLayout()
        {
            using (var runner = StartRunner())
            {
                RunOnGame(runner, () =>
                {
                    runner.GUI.MouseX = runner.GUI.MapToPixelX(5) + 1;
                    runner.GUI.MouseY = runner.GUI.MapToPixelY(6) + 1;
                    runner.GUI.DisplayGlobalStatus();
                });

                // 文字の大きさは 12（1 行 16 ピクセル）。ターン数・資金・空行の後、地形の名称を (37, 65) から書く
                var turn = Find(runner, "unit", x => x.StartsWith("ターン数 "));
                var money = Find(runner, "unit", x => x.EndsWith(" 0") && !x.StartsWith("ターン数"));
                var terrain = Find(runner, "unit", x => x.StartsWith("(5,6) "));
                var hit = Find(runner, "unit", x => x.StartsWith("回避 "));
                Assert.AreEqual(0f, turn.Y);
                Assert.AreEqual(16f, money.Y, 0.5f);
                Assert.AreEqual(37f, terrain.X);
                Assert.AreEqual(65f, terrain.Y);
                Assert.AreEqual(0f, hit.X);
                Assert.AreEqual(81f, hit.Y, 0.5f);
            }
        }

        [TestMethod]
        public void StatusPilotIndex_MainPilotIsZeroOrOne()
        {
            using (var runner = StartRunner())
            {
                var src = runner.Session.SRC;
                RunOnGame(runner, () => runner.GUI.DisplayPilotStatus(src.Map.MapDataForUnit[3, 3].MainPilot()));
                Assert.AreEqual(1, runner.GUI.DisplayedPilotInd);
            }
        }
    }
}
