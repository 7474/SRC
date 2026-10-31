using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// インクルード側の常時イベント（*付きラベル）が重複して起動しないこと（#1332）。
    /// 同じイベントファイルを二重に読み込むと常時イベントのラベルが二重に登録され、
    /// 常時イベントは１回の HandleEvent で該当ラベルすべてを実行するため繰り返し起動してしまう。
    /// </summary>
    [TestClass]
    public class IncludeAlwaysEventTests
    {
        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-inc-" + Guid.NewGuid());
            Directory.CreateDirectory(appPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                Directory.Delete(appPath, true);
            }
            catch
            {
                // ignore
            }
        }

        private void WriteFile(string name, params string[] lines)
        {
            var path = Path.Combine(appPath, name);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, string.Join("\r\n", lines) + "\r\n", new UTF8Encoding(false));
        }

        private SRC CreateSrc(string extDataPath = null)
        {
            var src = new SRC
            {
                SystemConfig = new LocalFileConfig { AppPath = appPath, ExtDataPath = extDataPath ?? "" },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            src.FileSystem = fs;
            new HeadlessGUI().Install(src);
            src.ScenarioPath = appPath;
            src.Event.SkipExternalSourceLoad = true;
            src.Event.InitEventData();
            src.Event.LoadEventData(Path.Combine(appPath, "test.eve"));
            src.Expression.DefineGlobalVariable("記録");
            src.Expression.SetVariableAsString("記録", "");
            return src;
        }

        private static int CountEventLabel(SRC src, string data)
        {
            return src.Event.colEventLabelList.List.Count(x => x.Data == data);
        }

        [TestMethod]
        public void DataInclude_IsLoadedOnlyOnce_WhenScenarioPathEqualsAppPath()
        {
            // VB: Event.bas LoadEventData の汎用Dataインクルードファイルは
            // ScenarioPath → ExtDataPath → ExtDataPath2 → AppPath の順で最初に見つかったものだけを読み込む
            WriteFile("test.eve",
                "エピローグ:",
                "Set 記録 (記録 & \"M\")",
                "Exit");
            WriteFile(Path.Combine("Data", "include.eve"),
                "*エピローグ:",
                "Set 記録 (記録 & \"A\")",
                "Exit");

            var src = CreateSrc(extDataPath: appPath);

            Assert.AreEqual(1, CountEventLabel(src, "*エピローグ"));
            Assert.AreEqual(1, src.Event.EventData.Count(x => x.Data == "*エピローグ:"));

            src.Event.HandleEvent("エピローグ");

            // 通常イベント１回＋常時イベント１回
            Assert.AreEqual("MA", src.Expression.GetValueAsString("記録"));
        }

        [TestMethod]
        public void DataInclude_UnitCommandLabels_AreRegisteredOnce_WhenAppPathIsSeparate()
        {
            // #1331: Web 版と同じ配置（シナリオのフォルダと SRC のフォルダ（AppPath）が別、ExtDataPath・ExtDataPath2 は空）。
            // 空の ExtDataPath と結合した "Data/include.eve" は相対パスになり、ファイルシステムがシナリオのフォルダから解決するため、
            // ScenarioPath・ExtDataPath・ExtDataPath2 の３回 Data\include.eve を読み込み、
            // そこ（とそこからインクルードしたファイル）のユニットコマンドがメニューに３つずつ表示されていた。
            // 報告のシナリオ（Data\include.eve の「ユニットコマンド 消耗品交換 味方」と、
            // lib の第２次α風ステータス表示の、条件付きで重ねた「ユニットコマンド 能力 …」）を模している
            var scenarioPath = Path.Combine(appPath, "scenario");
            var sysPath = Path.Combine(appPath, "app");
            Directory.CreateDirectory(sysPath);
            WriteFile(Path.Combine("scenario", "test.eve"),
                "スタート:",
                "Exit");
            WriteFile(Path.Combine("scenario", "Data", "include.eve"),
                "<lib\\status.eve>",
                "ユニットコマンド 消耗品交換 味方:",
                "Exit");
            WriteFile(Path.Combine("scenario", "lib", "status.eve"),
                "ユニットコマンド 能力 敵 (Condition(対象パイロット,ユニット情報隠蔽) = 0):",
                "ユニットコマンド 能力 ＮＰＣ (Condition(対象パイロット,ユニット情報隠蔽) = 0):",
                "-*ユニットコマンド 能力 味方 (Condition(対象パイロット,ユニット情報隠蔽) = 0):",
                "Exit");

            var src = new SRC
            {
                SystemConfig = new LocalFileConfig { AppPath = sysPath, ExtDataPath = "", ExtDataPath2 = "" },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(scenarioPath);
            fs.AddPath(sysPath);
            src.FileSystem = fs;
            new HeadlessGUI().Install(src);
            src.ScenarioPath = scenarioPath;
            src.Event.SkipExternalSourceLoad = true;
            src.Event.InitEventData();
            src.Event.LoadEventData(Path.Combine(scenarioPath, "test.eve"));

            var unitCommands = src.Event.colEventLabelList.List
                .Where(x => x.Name == LabelType.UnitCommandEventLabel)
                .Select(x => x.Data)
                .ToList();
            CollectionAssert.AreEquivalent(new List<string>
            {
                "ユニットコマンド 能力 敵 (Condition(対象パイロット,ユニット情報隠蔽) = 0)",
                "ユニットコマンド 能力 ＮＰＣ (Condition(対象パイロット,ユニット情報隠蔽) = 0)",
                "-*ユニットコマンド 能力 味方 (Condition(対象パイロット,ユニット情報隠蔽) = 0)",
                "ユニットコマンド 消耗品交換 味方",
            }, unitCommands);

            // 次のステージでも増えない
            src.Event.LoadEventData(Path.Combine(scenarioPath, "test.eve"));
            Assert.AreEqual(4, src.Event.colEventLabelList.List.Count(x => x.Name == LabelType.UnitCommandEventLabel));
        }

        [TestMethod]
        public void DataInclude_IsLoadedOnlyOnce_OnNextStage()
        {
            WriteFile("test.eve",
                "エピローグ:",
                "Set 記録 (記録 & \"M\")",
                "Exit");
            WriteFile(Path.Combine("Data", "include.eve"),
                "*エピローグ:",
                "Set 記録 (記録 & \"A\")",
                "Exit");

            var src = CreateSrc();
            // 次のステージ（ScenarioLibChecked 後の LoadEventData）
            src.Event.LoadEventData(Path.Combine(appPath, "test.eve"));

            Assert.AreEqual(1, CountEventLabel(src, "*エピローグ"));

            src.Event.HandleEvent("エピローグ");

            Assert.AreEqual("MA", src.Expression.GetValueAsString("記録"));
        }

        [TestMethod]
        public void Require_SkipsFileAlreadyIncluded()
        {
            // VB: CmdData.cls ExecRequireCmd は EventFileNames に記録済み（<> でインクルード済みを含む）のファイルを読み込まない
            WriteFile("test.eve",
                "<sub.eve>",
                "スタート:",
                "Require sub.eve",
                "Exit",
                "エピローグ:",
                "Set 記録 (記録 & \"M\")",
                "Exit");
            WriteFile("sub.eve",
                "*エピローグ:",
                "Set 記録 (記録 & \"A\")",
                "Exit");

            var src = CreateSrc();
            Assert.AreEqual(1, CountEventLabel(src, "*エピローグ"));

            src.Event.HandleEvent("スタート");

            Assert.AreEqual(1, CountEventLabel(src, "*エピローグ"));

            src.Event.HandleEvent("エピローグ");

            // <sub.eve> は先頭でインクルードされるので常時イベントが先に実行される
            Assert.AreEqual("AM", src.Expression.GetValueAsString("記録"));
        }

        [TestMethod]
        public void Require_LoadsNewFileOnce()
        {
            WriteFile("test.eve",
                "スタート:",
                "Require sub.eve",
                "Require sub.eve",
                "Exit",
                "エピローグ:",
                "Set 記録 (記録 & \"M\")",
                "Exit");
            WriteFile("sub.eve",
                "*エピローグ:",
                "Set 記録 (記録 & \"A\")",
                "Exit");

            var src = CreateSrc();
            Assert.AreEqual(0, CountEventLabel(src, "*エピローグ"));

            src.Event.HandleEvent("スタート");
            src.Event.HandleEvent("スタート");

            Assert.AreEqual(1, CountEventLabel(src, "*エピローグ"));

            src.Event.HandleEvent("エピローグ");

            Assert.AreEqual("MA", src.Expression.GetValueAsString("記録"));
        }
        [TestMethod]
        public void Restore_LoadsRequiredFileOnce_AndRequireAfterRestoreDoesNotReload()
        {
            // VB: Event.bas RestoreEventData は AdditionalEventFileNames を復元し、
            // ScenarioPath からの相対パスでファイルを読み込む
            WriteFile("test.eve",
                "スタート:",
                "Require sub.eve",
                "Exit",
                "エピローグ:",
                "Set 記録 (記録 & \"M\")",
                "Exit");
            WriteFile("sub.eve",
                "*エピローグ:",
                "Set 記録 (記録 & \"A\")",
                "Exit");

            var src = CreateSrc();
            // 中断データからの再開（リストア）
            src.Event.LoadEventData(Path.Combine(appPath, "test.eve"), "リストア");
            src.Event.Restore(new SRCSuspendData
            {
                AdditionalEventFileNames = new List<string> { "sub.eve" },
                DisableEventLabels = new List<string>(),
            });

            Assert.AreEqual(1, CountEventLabel(src, "*エピローグ"));
            CollectionAssert.AreEqual(new List<string> { "sub.eve" }, src.Event.AdditionalEventFileNames.ToList());

            // 再開後に同じ Require を実行しても読み込み直さない
            src.Event.HandleEvent("スタート");
            Assert.AreEqual(1, CountEventLabel(src, "*エピローグ"));

            src.Event.HandleEvent("エピローグ");

            Assert.AreEqual("MA", src.Expression.GetValueAsString("記録"));
        }
    }
}
