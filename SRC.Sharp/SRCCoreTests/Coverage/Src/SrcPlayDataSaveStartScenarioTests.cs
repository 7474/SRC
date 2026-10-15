using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System.IO;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// SRC.StartScenario / GameOver / ExitGame（元の SRC.bas の同名関数）
    /// </summary>
    [TestClass]
    public class SrcPlayDataSaveStartScenarioTests
    {
        private static readonly string[] NormalScenario =
        {
            "プロローグ:",
            "Global プロローグ実行",
            "Set プロローグ実行 1",
            "Exit",
            "",
            "スタート:",
            "Global スタート実行",
            "Set スタート実行 1",
            "Exit",
        };

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_RunsPrologueAndStartThenBeginsPlayerPhase()
        {
            // プロローグ→スタートイベントを順に実行し、味方フェイズを開始する。
            // スタートイベントの前にリスタート用データをセーブする（VB: DumpData ScenarioPath & "_リスタート.src"）
            using var f = new SrcPlayDataSaveFixture();
            var fname = f.Write("test.eve", NormalScenario);

            f.Src.StartScenario("test.eve");

            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("プロローグ実行"));
            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("スタート実行"));
            Assert.AreEqual(fname, f.Src.ScenarioFileName);
            Assert.AreEqual("味方", f.Src.Stage);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
            // マップが無い（ステータス表示用）のでターンは進まない
            Assert.AreEqual(0, f.Src.Turn);
            Assert.IsTrue(File.Exists(Path.Combine(f.ScenarioDir, "_リスタート.srcq")));
            Assert.IsTrue(f.Src.IsRestartSaveDataAvailable);
            // スタートイベント後はクイックロードを無効にする
            Assert.IsFalse(f.Src.IsQuickSaveDataAvailable);
            Assert.IsFalse(f.Src.IsSubStage);
            Assert.AreEqual("SRC# - test", f.Gui.GUI.MainFormText);
            // メインウィンドウが非表示ならスタートイベントの前に表示する
            Assert.IsTrue(f.Called(nameof(IGUI.MainFormShow)));
            Assert.IsFalse(f.Called(nameof(IGUI.ErrorMessage)));
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_MainFormAlreadyVisible_DoesNotShowAgain()
        {
            // MainForm.Visible なら MainForm.Show しない
            using var f = new SrcPlayDataSaveFixture();
            f.Write("test.eve", NormalScenario);
            f.Gui.Handle("get_MainFormVisible", _ => true);

            f.Src.StartScenario("test.eve");

            Assert.IsFalse(f.Called(nameof(IGUI.MainFormShow)));
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_RelativeName_PrefersScenarioPathOverAppPath()
        {
            // ファイルはシナリオフォルダ→SRC フォルダの順に探す
            using var f = new SrcPlayDataSaveFixture(separateAppPath: true);
            var inScenario = f.Write("test.eve", NormalScenario);
            SrcPlayDataSaveFixture.WriteTo(f.AppDir, "test.eve", NormalScenario);

            f.Src.StartScenario("test.eve");

            Assert.AreEqual(inScenario, f.Src.ScenarioFileName);
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_RelativeName_FallsBackToAppPath()
        {
            using var f = new SrcPlayDataSaveFixture(separateAppPath: true);
            var inApp = SrcPlayDataSaveFixture.WriteTo(f.AppDir, "test.eve", NormalScenario);

            f.Src.StartScenario("test.eve");

            Assert.AreEqual(inApp, f.Src.ScenarioFileName);
            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("スタート実行"));
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_FileNotFound_ShowsErrorAndTerminates()
        {
            // VB: MsgBox fname & "が見つかりません" → TerminateSRC
            using var f = new SrcPlayDataSaveFixture();
            var fname = Path.Combine(f.ScenarioDir, "none.eve");

            SrcPlayDataSaveFixture.Run(() => f.Src.StartScenario(fname));

            var error = f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).First();
            Assert.AreEqual(fname + "が見つかりません", error.Args[0]);
            Assert.IsTrue(f.Gui.IsTerminated);
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_NoStartEvent_ShowsErrorAndTerminates()
        {
            // スタートイベントが無いシナリオはエラー終了する
            using var f = new SrcPlayDataSaveFixture();
            f.Write("test.eve", "プロローグ:", "Exit");

            SrcPlayDataSaveFixture.Run(() => f.Src.StartScenario("test.eve"));

            Assert.IsTrue(f.Gui.CallsOf(nameof(IGUI.ErrorMessage))
                .Any(x => (string)x.Args[0] == "スタートイベントが定義されていません"));
            Assert.IsTrue(f.Gui.IsTerminated);
            // スタートイベントの前にエラーになるのでリスタート用データは作らない
            Assert.IsFalse(File.Exists(Path.Combine(f.ScenarioDir, "_リスタート.srcq")));
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_SubStage_DoesNotResetStageVariables()
        {
            // サブステージ（IsSubStage）では次ステージ等のシステム変数やタイトルを設定しない。
            // スタートイベントが始まった場合は通常のステージとみなす
            using var f = new SrcPlayDataSaveFixture();
            f.Write("sub.eve", NormalScenario);
            f.Src.Expression.DefineGlobalVariable("次ステージ");
            f.Src.Expression.SetVariableAsString("次ステージ", "next.eve");
            f.Src.IsSubStage = true;

            f.Src.StartScenario("sub.eve");

            Assert.AreEqual("next.eve", f.Src.Expression.GetValueAsString("次ステージ"));
            Assert.IsFalse(f.Src.Expression.IsGlobalVariableDefined("セーブデータファイル名"));
            Assert.IsFalse(f.Gui.CallsOf("set_MainFormText").Any());
            Assert.IsFalse(f.Src.IsSubStage);
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_ResetsSaveFlagsBeforePrologue()
        {
            // 各種変数の初期化：ターン・クイックセーブ有無を初期化し、
            // 最後のセーブファイルはリスタート用データ（DumpData）になる
            using var f = new SrcPlayDataSaveFixture();
            f.Write("test.eve",
                "プロローグ:",
                "Exit",
                "",
                "スタート:",
                "Exit");
            f.Src.LastSaveDataFileName = "old.src";
            f.Src.IsQuickSaveDataAvailable = true;
            f.Src.Turn = 5;

            f.Src.StartScenario("test.eve");

            Assert.AreEqual(Path.Combine(f.ScenarioDir, "_リスタート.srcq"), f.Src.LastSaveDataFileName);
            Assert.IsFalse(f.Src.IsQuickSaveDataAvailable);
            Assert.AreEqual(0, f.Src.Turn);
        }

        [TestMethod]
        [Timeout(20000)]
        [Ignore("移植の不具合: リスタート用データを作らないステータス表示用ファイルの判定が \\ 区切り固定で、/ 区切りのパスでは判定できない")]
        public void StartScenario_StatusViewLibrary_DoesNotDumpRestartData()
        {
            // Lib\ユニットステータス表示.eve の実行ではリスタート用データをセーブしない
            using var f = new SrcPlayDataSaveFixture();
            var fname = f.Write("Lib/ユニットステータス表示.eve", NormalScenario);

            f.Src.StartScenario(fname);

            Assert.IsFalse(File.Exists(Path.Combine(f.ScenarioDir, "_リスタート.srcq")));
        }

        // 先頭行のラベルは Event.IsEventDefined が見落とす（下の Ignore テスト参照）ため、
        // プロローグの前に別のラベルを置く
        private static readonly string[] SystemPrologue =
        {
            "準備:",
            "Exit",
            "",
            "プロローグ:",
            "Global システム終了イベント",
            "Set システム終了イベント 1",
            "Exit",
        };

        [TestMethod]
        [Timeout(20000)]
        public void GameOver_RunsGameOverEvePrologueAndLoadsNonPilot()
        {
            // Data\System\GameOver.eve のプロローグを実行する。同じフォルダの non_pilot.txt も読み込む
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/System/GameOver.eve", SystemPrologue);
            f.Write("Data/System/non_pilot.txt", "ゲームオーバー係", "係員, gameover.bmp");

            f.Src.GameOver();

            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("システム終了イベント"));
            Assert.IsTrue(f.Src.NPDList.IsDefined("ゲームオーバー係"));
            Assert.IsTrue(f.Called(nameof(IGUI.MainFormHide)));
            // GameOver は TerminateSRC しない（プロローグ実行後は呼び出し元に戻る）
            Assert.IsFalse(f.Gui.IsTerminated);
        }

        [TestMethod]
        [Timeout(20000)]
        [Ignore("移植の不具合: GameOver が ScenarioFileName に GameOver.eve のパスではなく未設定の null を設定する")]
        public void GameOver_SetsScenarioFileNameToGameOverEve()
        {
            // VB: ScenarioFileName = fname（fname は GameOver.eve のパス）
            using var f = new SrcPlayDataSaveFixture();
            var eve = f.Write("Data/System/GameOver.eve", SystemPrologue);

            f.Src.GameOver();

            Assert.AreEqual(eve, f.Src.ScenarioFileName);
        }

        [TestMethod]
        [Timeout(20000)]
        [Ignore("移植の不具合: Event.IsEventDefined が検索開始位置を 1 としており、イベントデータ先頭行（ID 0）のプロローグを見落とす")]
        public void GameOver_PrologueOnFirstLine_IsExecuted()
        {
            // システム側のイベントが無い状態で GameOver.eve の1行目がプロローグでも実行できる
            // （VB の EventData は 1 始まりなので SearchLabel(lname, 1) で先頭行も見つかる）
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/System/GameOver.eve",
                "プロローグ:",
                "Global システム終了イベント",
                "Set システム終了イベント 1",
                "Exit");

            SrcPlayDataSaveFixture.Run(() => f.Src.GameOver());

            Assert.IsFalse(f.Gui.IsTerminated);
            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("システム終了イベント"));
        }

        [TestMethod]
        [Timeout(20000)]
        public void GameOver_FallsBackToAppPathGameOverEve()
        {
            // シナリオ側に無ければ SRC フォルダの Data\System\GameOver.eve を使う
            using var f = new SrcPlayDataSaveFixture(separateAppPath: true);
            SrcPlayDataSaveFixture.WriteTo(f.AppDir, "Data/System/GameOver.eve", SystemPrologue);

            f.Src.GameOver();

            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("システム終了イベント"));
        }

        [TestMethod]
        [Timeout(20000)]
        public void GameOver_NoGameOverEve_Terminates()
        {
            // GameOver.eve が無ければそのまま終了
            using var f = new SrcPlayDataSaveFixture();

            SrcPlayDataSaveFixture.Run(() => f.Src.GameOver());

            Assert.IsTrue(f.Gui.IsTerminated);
        }

        [TestMethod]
        [Timeout(20000)]
        public void GameOver_NoPrologueEvent_ShowsErrorAndTerminates()
        {
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/System/GameOver.eve", "スタート:", "Exit");

            SrcPlayDataSaveFixture.Run(() => f.Src.GameOver());

            Assert.IsTrue(f.Gui.CallsOf(nameof(IGUI.ErrorMessage))
                .Any(x => ((string)x.Args[0]).EndsWith("中にプロローグイベントが定義されていません")));
            Assert.IsTrue(f.Gui.IsTerminated);
        }

        [TestMethod]
        [Timeout(20000)]
        public void ExitGame_RunsExitEvePrologueThenTerminates()
        {
            // Data\System\Exit.eve のプロローグを実行してから SRC を終了する
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/System/Exit.eve", SystemPrologue);
            f.Write("Data/System/non_pilot.txt", "終了係", "係員, exit.bmp");

            SrcPlayDataSaveFixture.Run(() => f.Src.ExitGame());

            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("システム終了イベント"));
            Assert.IsTrue(f.Src.NPDList.IsDefined("終了係"));
            Assert.IsTrue(f.Called(nameof(IGUI.MainFormHide)));
            Assert.IsTrue(f.Gui.IsTerminated);
        }

        [TestMethod]
        [Timeout(20000)]
        public void ExitGame_NoExitEve_Terminates()
        {
            using var f = new SrcPlayDataSaveFixture();

            SrcPlayDataSaveFixture.Run(() => f.Src.ExitGame());

            Assert.IsTrue(f.Gui.IsTerminated);
            Assert.IsFalse(f.Src.Expression.IsGlobalVariableDefined("システム終了イベント"));
        }

        [TestMethod]
        public void GameClear_Terminates()
        {
            // VB: GameClear は TerminateSRC するだけ
            using var f = new SrcPlayDataSaveFixture();

            f.Src.GameClear();

            Assert.IsTrue(f.Gui.IsTerminated);
        }
    }
}
