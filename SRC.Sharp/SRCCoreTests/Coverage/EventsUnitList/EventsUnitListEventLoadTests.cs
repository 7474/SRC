using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests.EventsUnitList
{
    /// <summary>
    /// イベントファイルの読み込み（Event.data.cs / VB: Event.LoadEventData）と
    /// ラベル検索（Event.label.cs / VB: Event.SearchLabel 等）のテスト。
    /// 実際に一時フォルダへ .eve を書き出して読み込む。
    /// </summary>
    [TestClass]
    public class EventsUnitListEventLoadTests
    {
        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-eul-" + Guid.NewGuid());
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
            File.WriteAllText(Path.Combine(appPath, name), string.Join("\r\n", lines) + "\r\n", new UTF8Encoding(false));
        }

        private (SRC src, MockGUI gui) Load(params string[] lines)
        {
            WriteFile("test.eve", lines);
            return LoadFile("test.eve");
        }

        private (SRC src, MockGUI gui) LoadFile(string name)
        {
            // MockGUI.Terminate は false を返すので TerminateSRC でプロセスは終了しない
            var gui = new MockGUI();
            var src = new SRC
            {
                GUI = gui,
                SystemConfig = new LocalFileConfig { AppPath = appPath },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            src.FileSystem = fs;
            src.ScenarioPath = appPath;
            // データフォルダやマップの読み込みは行わない
            src.Event.SkipExternalSourceLoad = true;
            src.Event.InitEventData();
            src.Event.LoadEventData(Path.Combine(appPath, name));
            return (src, gui);
        }

        private static int LineOf(SRC src, string data)
        {
            return src.Event.EventData.First(x => x.Data == data).ID;
        }

        private static string[] ErrorMessages(SRC src)
        {
            return src.DataErrors.Select(x => x.msg).ToArray();
        }

        // ──────────────────────────────────────────────
        // 構文チェック (ParseCommand / ValidateCommandArgs)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LoadEventData_WellFormedControlStructures_HasNoError()
        {
            // 対応の取れた制御構造は書式エラーにならない
            var (src, gui) = Load(
                "スタート:",
                "If a = 1 Then",
                "Talk システム",
                "こんにちは",
                "End",
                "ElseIf a = 2 Then",
                "Set b 1",
                "Else",
                "Set b 2",
                "EndIf",
                "Do While b < 3",
                "Incr b",
                "Loop",
                "For i = 1 To 3",
                "Next",
                "ForEach 味方",
                "Next",
                "Switch b",
                "Case 1",
                "Set c 1",
                "CaseElse",
                "Set c 2",
                "EndSw",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
            // コマンドは行ごとに解析されている
            Assert.AreEqual(src.Event.EventData.Count, src.Event.EventCmd.Count);
        }

        [DataTestMethod]
        [DataRow("If a = 1 Then|Set b 1", "Ifに対応するEndIfがありません", "If a = 1 Then")]
        [DataRow("Set b 1|EndIf", "EndIfに対応するIfがありません", "EndIf")]
        [DataRow("Do|Set b 1", "Doに対応するLoopがありません", "Do")]
        [DataRow("Set b 1|Loop", "Loopに対応するDoがありません", "Loop")]
        [DataRow("For i = 1 To 3|Set b 1", "Forに対応するNextがありません", "For i = 1 To 3")]
        [DataRow("ForEach 味方|Set b 1", "ForEachに対応するNextがありません", "ForEach 味方")]
        [DataRow("Switch b|Case 1", "Switchに対応するEndSwがありません", "Switch b")]
        [DataRow("Set b 1|EndSw", "EndSwに対応するSwitchがありません", "EndSw")]
        [DataRow("Do|ElseIf a = 2 Then|EndIf|Loop", "ElseIfに対応するIfがありません", "ElseIf a = 2 Then")]
        [DataRow("Do|Case 1|EndSw|Loop", "Caseに対応するSwitchがありません", "Case 1")]
        [DataRow("Do|Next|Loop", "Nextに対応するコマンドがありません", "Next")]
        [DataRow("Do|End|Loop", "Endに対応するTalkがありません", "End")]
        [DataRow("Do|Suspend|Loop", "Suspendに対応するTalkがありません", "Suspend")]
        [DataRow("If a = 1 Then|Talk システム|こんにちは|EndIf", "Talkに対応するEndがありません", "Talk システム")]
        public void LoadEventData_UnbalancedStructure_ReportsErrorAndTerminates(string script, string message, string errorLine)
        {
            // 制御構造の対応が取れていなければ該当行を示してエラーを出し、SRC を終了する
            var (src, gui) = Load(new[] { "スタート:" }.Concat(script.Split('|')).ToArray());

            var error = src.DataErrors.FirstOrDefault(x => x.msg == message);
            Assert.IsNotNull(error, string.Join(",", ErrorMessages(src)));
            Assert.AreEqual(errorLine, error.line_buf);
            Assert.AreEqual("test.eve", Path.GetFileName(error.fname));
            Assert.IsTrue(gui.TerminateCalled);
        }

        [TestMethod]
        [Ignore("移植の不具合: 制御構造スタックが空のとき Talk を積まないため、最上位の Talk の End 抜けを検出しない")]
        public void LoadEventData_TopLevelTalkWithoutEnd_IsError()
        {
            // VB ではスタックの底(NullCmd)と異なるので Talk を積み、End がないままファイル末尾に達するとエラー
            var (src, gui) = Load(
                "スタート:",
                "Talk システム",
                "こんにちは");

            CollectionAssert.Contains(ErrorMessages(src), "Talkに対応するEndがありません");
            Assert.IsTrue(gui.TerminateCalled);
        }

        [TestMethod]
        [Ignore("移植の不具合: 制御構造スタックが空のときの ElseIf/Case/Next/End の対応チェックが省かれている")]
        public void LoadEventData_TopLevelEndIfLikeCommandsWithoutOpener_AreErrors()
        {
            // VB ではスタックが空(NullCmd)でも ElseIf / Next / End は対応する開始コマンドがないとエラー
            var (src, gui) = Load(
                "スタート:",
                "ElseIf a = 1 Then",
                "EndIf",
                "Next",
                "End");

            var messages = ErrorMessages(src);
            CollectionAssert.Contains(messages, "ElseIfに対応するIfがありません");
            CollectionAssert.Contains(messages, "Nextに対応するコマンドがありません");
            CollectionAssert.Contains(messages, "Endに対応するTalkがありません");
            Assert.IsTrue(gui.TerminateCalled);
        }

        [DataTestMethod]
        [DataRow("Create 味方 ガンダム 0 アムロ 1 1", "Createコマンドのパラメータ数が違います")]
        [DataRow("Pilot アムロ", "Pilotコマンドのパラメータ数が違います")]
        [DataRow("Unit ガンダム", "Unitコマンドのパラメータ数が違います")]
        [DataRow("Unit ガンダム 0 余分", "Unitコマンドのパラメータ数が違います")]
        public void LoadEventData_MainCommandsWithWrongArgCount_ReportErrorAndTerminate(string line, string message)
        {
            // Create は8個以上、Pilot は3個以上、Unit はちょうど3個の引数が必要
            var (src, gui) = Load("スタート:", line, "Exit");

            CollectionAssert.Contains(ErrorMessages(src), message);
            Assert.IsTrue(gui.TerminateCalled);
        }

        [TestMethod]
        public void LoadEventData_MainCommandsWithValidArgCount_HaveNoError()
        {
            var (src, gui) = Load(
                "スタート:",
                "Create 味方 ガンダム 0 アムロ 1 1 1",
                "Pilot アムロ 1",
                "Unit ガンダム 0",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        [TestMethod]
        public void LoadEventData_LabelEndingWithFullWidthColon_IsReported()
        {
            // ラベル末尾が全角の「：」になっているのは書式エラー
            var (src, gui) = Load("スタート：", "Exit");

            CollectionAssert.Contains(ErrorMessages(src), "ラベルの末尾が全角文字になっています");
            Assert.AreEqual(-1, src.Event.FindLabel("スタート"));
        }

        [TestMethod]
        [Ignore("移植の不具合: 全角コロンのラベルを検出しても error_found を立てず SRC を終了しない")]
        public void LoadEventData_LabelEndingWithFullWidthColon_Terminates()
        {
            // VB では error_found = True となり構文チェック後に TerminateSRC される
            var (src, gui) = Load("スタート：", "Exit");

            Assert.IsTrue(gui.TerminateCalled);
        }

        [DataTestMethod]
        [DataRow("Ask 選んでください|はい|いいえ", "Askに対応するEndがありません", "Ask 選んでください")]
        [DataRow("Ask 選んでください キャンセル可|はい", "Askに対応するEndがありません", "Ask 選んでください キャンセル可")]
        [DataRow("Question 25 その声は|アリスだな？", "Questionに対応するEndがありません", "Question 25 その声は")]
        [DataRow("If a = 1 Then|AutoTalk システム|こんにちは", "AutoTalkに対応するEndがありません", "AutoTalk システム")]
        public void LoadEventData_BlockDialogWithoutEndAtEndOfFile_IsError(string script, string message, string errorLine)
        {
            // 書式１の Ask（オプションを除いた引数が2個以下）や Question、AutoTalk は End までがブロック。
            // End がないままファイル末尾に達するとブロックの開始行を示してエラー（VB: Event.LoadEventData 末尾のチェック）
            var (src, gui) = Load(new[] { "スタート:" }.Concat(script.Split('|')).ToArray());

            var error = src.DataErrors.FirstOrDefault(x => x.msg == message);
            Assert.IsNotNull(error, string.Join(",", ErrorMessages(src)));
            Assert.AreEqual(errorLine, error.line_buf);
            Assert.IsTrue(gui.TerminateCalled);
        }

        [TestMethod]
        public void LoadEventData_BlockAskInterruptedByExit_IsError()
        {
            // Ask ブロックの途中で Exit が現れたら End 抜けとして Ask の行を示す
            var (src, gui) = Load("スタート:", "Ask 選んでください", "はい", "Exit");

            var error = src.DataErrors.FirstOrDefault(x => x.msg == "Talk/AutoTalk/Ask/Questionに対応するEndがありません");
            Assert.IsNotNull(error, string.Join(",", ErrorMessages(src)));
            Assert.AreEqual("Ask 選んでください", error.line_buf);
            Assert.IsTrue(gui.TerminateCalled);
        }

        [DataTestMethod]
        [DataRow("Ask 選択肢 選んでください キャンセル可")]
        [DataRow("Ask 選択肢 選んでください")]
        [DataRow("Ask 選んでください 終了")]
        public void LoadEventData_InlineAsk_DoesNotOpenBlock(string line)
        {
            // 書式２（配列名 + メッセージ）や「終了」指定の Ask はブロックを作らないので End は不要
            var (src, gui) = Load("スタート:", line, "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        [TestMethod]
        public void LoadEventData_BlockAskClosedByEnd_HasNoError()
        {
            // End で Ask / Question ブロックが閉じられていればエラーにならない
            var (src, gui) = Load(
                "スタート:",
                "Ask 選んでください",
                "はい",
                "いいえ",
                "End",
                "Question 25",
                "アリスだな？",
                "End",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        [TestMethod]
        public void LoadEventData_IncludeLine_LoadsOtherFileInPlace()
        {
            // 「<ファイル名>」の行は、その位置に別のイベントファイルの内容を読み込む
            WriteFile("sub.eve", "サブ:", "Set x 1", "Return");
            var (src, gui) = Load(
                "スタート:",
                "Call サブ",
                "Exit",
                "<sub.eve>",
                "エピローグ:",
                "Exit");

            var data = src.Event.EventData.Select(x => x.Data).ToList();
            var sub = data.IndexOf("サブ:");
            Assert.IsTrue(sub > data.IndexOf("Exit"));
            Assert.IsTrue(sub < data.IndexOf("エピローグ:"));
            CollectionAssert.DoesNotContain(data, "<sub.eve>");
            Assert.AreEqual(LineOf(src, "サブ:"), src.Event.FindLabel("サブ"));
            Assert.IsTrue(src.Event.EventData.First(x => x.Data == "サブ:").File.EndsWith("sub.eve"));
        }

        [TestMethod]
        public void LoadEventData_DataTitleLine_RegistersTitle()
        {
            // 「@作品名」の行は使用するデータ（作品）の指定
            var (src, gui) = Load("@テスト作品", "スタート:", "Exit");

            CollectionAssert.Contains(src.Titles.ToList(), "テスト作品");
        }

        [TestMethod]
        public void LoadDynamic_AddsLabelsOnce()
        {
            // シナリオ開始後に読み込んだファイルのラベルも検索でき、同じファイルは二度読み込まない
            WriteFile("dyn.eve", "動的ラベル:", "Exit");
            var (src, gui) = Load("スタート:", "Exit");
            var dyn = Path.Combine(appPath, "dyn.eve");

            src.Event.LoadDynamic(dyn);
            var count = src.Event.EventData.Count;

            Assert.AreEqual(LineOf(src, "動的ラベル:"), src.Event.FindLabel("動的ラベル"));
            Assert.AreEqual(count, src.Event.EventCmd.Count);

            src.Event.EventFileNames.Add(dyn);
            src.Event.LoadDynamic(dyn);
            Assert.AreEqual(count, src.Event.EventData.Count);
        }

        // ──────────────────────────────────────────────
        // ラベル検索 (SearchLabel / IsEventDefined / FindLabel)
        // ──────────────────────────────────────────────

        private (SRC src, MockGUI gui) LoadLabels()
        {
            return Load(
                "開始:",
                "Exit",
                "プロローグ:",
                "Exit",
                "ターン 2 味方:",
                "Exit",
                "ターン 全 敵:",
                "Exit",
                "損傷率 ガンダム 50:",
                "Exit",
                "攻撃 アムロ シャア:",
                "Exit",
                "*破壊 ザク:",
                "Exit",
                "破壊 ザク:",
                "Exit",
                "進入 ガンダム 3 4:",
                "Exit",
                "マップコマンド 回復 (a = 1):",
                "Exit",
                "使用 全 ビーム:",
                "Exit",
                "破壊 敵:",
                "Exit",
                "*破壊 グフ:",
                "Exit",
                "通常ラベル:",
                "Exit",
                "通常ラベル:",
                "Exit");
        }

        [TestMethod]
        public void SearchLabel_SimpleEventLabels()
        {
            var (src, _) = LoadLabels();

            Assert.AreEqual(LineOf(src, "プロローグ:"), src.Event.SearchLabel("プロローグ"));
            Assert.AreEqual(-1, src.Event.SearchLabel("エピローグ"));
        }

        [TestMethod]
        public void SearchLabel_TurnLabel_ComparesTurnNumberAndParty()
        {
            // ターン数は数値として比較。「ターン 全」は毎ターンのイベントで、個別のターン数とは一致しない
            var (src, _) = LoadLabels();

            Assert.AreEqual(LineOf(src, "ターン 2 味方:"), src.Event.SearchLabel("ターン 2 味方"));
            Assert.AreEqual(-1, src.Event.SearchLabel("ターン 3 味方"));
            Assert.AreEqual(-1, src.Event.SearchLabel("ターン 2 敵"));
            Assert.AreEqual(-1, src.Event.SearchLabel("ターン 5 敵"));
            Assert.AreEqual(LineOf(src, "ターン 全 敵:"), src.Event.SearchLabel("ターン 全 敵"));
        }

        [TestMethod]
        public void SearchLabel_DamageLabel_MatchesWhenDamageRateReachesThreshold()
        {
            // 損傷率ラベルは損傷率がラベルの値以上なら一致
            var (src, _) = LoadLabels();
            var line = LineOf(src, "損傷率 ガンダム 50:");

            Assert.AreEqual(line, src.Event.SearchLabel("損傷率 ガンダム 70"));
            Assert.AreEqual(line, src.Event.SearchLabel("損傷率 ガンダム 50"));
            Assert.AreEqual(-1, src.Event.SearchLabel("損傷率 ガンダム 30"));
            Assert.AreEqual(-1, src.Event.SearchLabel("損傷率 ザク 70"));
        }

        [TestMethod]
        public void SearchLabel_EnterLabel_ComparesCoordinates()
        {
            // 進入ラベルの座標は数値として比較
            var (src, _) = LoadLabels();

            Assert.AreEqual(LineOf(src, "進入 ガンダム 3 4:"), src.Event.SearchLabel("進入 ガンダム 3 4"));
            Assert.AreEqual(-1, src.Event.SearchLabel("進入 ガンダム 3 5"));
        }

        [TestMethod]
        public void SearchLabel_AllWildcard_MatchesAnyUnit()
        {
            // 「全」はどのユニットにも一致
            var (src, _) = LoadLabels();

            Assert.AreEqual(LineOf(src, "使用 全 ビーム:"), src.Event.SearchLabel("使用 誰か ビーム"));
            Assert.AreEqual(-1, src.Event.SearchLabel("使用 誰か ミサイル"));
        }

        [TestMethod]
        public void SearchLabel_PartyName_MatchesPilotOfThatParty()
        {
            // ラベルに陣営名が書かれていればパイロットの所属陣営で比較する
            var (src, _) = LoadLabels();
            src.PDList.Add("ザコ兵");
            src.PList.Add("ザコ兵", 1, "敵");

            Assert.AreEqual(LineOf(src, "破壊 敵:"), src.Event.SearchLabel("破壊 ザコ兵"));
        }

        [TestMethod]
        public void SearchLabel_MapCommandLabel_IgnoresConditionParameter()
        {
            // マップコマンドラベルの最後のパラメータは条件式なので、パラメータ数が違っても一致する
            var (src, _) = LoadLabels();

            Assert.AreEqual(LineOf(src, "マップコマンド 回復 (a = 1):"), src.Event.SearchLabel("マップコマンド 回復"));
            Assert.AreEqual(-1, src.Event.SearchLabel("マップコマンド 補給"));
        }

        [TestMethod]
        public void SearchLabel_StartLine_SkipsEarlierLabels()
        {
            // 検索開始行より前のラベルは対象外。常時イベント(*)と通常イベントの両方がある場合は順に見つかる
            var (src, _) = LoadLabels();
            var always = LineOf(src, "*破壊 ザク:");
            var normal = LineOf(src, "破壊 ザク:");

            Assert.AreEqual(always, src.Event.SearchLabel("破壊 ザク"));
            Assert.AreEqual(normal, src.Event.SearchLabel("破壊 ザク", always + 1));
            // 通常イベントより後ろには「破壊 敵」「*破壊 グフ」しかなく、ザクは敵のパイロットでもないので一致しない
            Assert.AreEqual(-1, src.Event.SearchLabel("破壊 ザク", normal + 1));
        }

        [TestMethod]
        public void SearchLabel_ForwardOrderAttackLabel_KeepsSelectedUnits()
        {
            // 攻撃ラベルが対象・相手の順で一致した場合は選択ユニットを入れ替えない
            var (src, _) = LoadLabels();
            var attacker = new Unit(src);
            var target = new Unit(src);
            src.Event.SelectedUnitForEvent = attacker;
            src.Event.SelectedTargetForEvent = target;

            Assert.AreEqual(LineOf(src, "攻撃 アムロ シャア:"), src.Event.SearchLabel("攻撃 アムロ シャア"));
            Assert.AreSame(attacker, src.Event.SelectedUnitForEvent);
            Assert.AreSame(target, src.Event.SelectedTargetForEvent);
        }

        [TestMethod]
        [Ignore("移植の不具合: 入れ替えラベルの存在判定が FindLabel(..) == 0 のままで、未定義(-1)を判定できず入れ替え一致しない")]
        public void SearchLabel_ReversedAttackLabel_MatchesAndSwapsSelectedUnits()
        {
            // VB: 「攻撃 シャア アムロ」のラベルがなければ「攻撃 アムロ シャア」に入れ替えて一致させ、
            // SelectedUnitForEvent と SelectedTargetForEvent を入れ替える
            var (src, _) = LoadLabels();
            var attacker = new Unit(src);
            var target = new Unit(src);
            src.Event.SelectedUnitForEvent = attacker;
            src.Event.SelectedTargetForEvent = target;

            Assert.AreEqual(LineOf(src, "攻撃 アムロ シャア:"), src.Event.SearchLabel("攻撃 シャア アムロ"));
            Assert.AreSame(target, src.Event.SelectedUnitForEvent);
            Assert.AreSame(attacker, src.Event.SelectedTargetForEvent);
        }

        [TestMethod]
        public void ClearLabel_DisablesLabel_AndRestoreLabelEnablesIt()
        {
            // ClearEvent 相当: 指定行（またはその前後10行以内）のラベルを無効化し、RestoreEvent で復活する
            var (src, _) = LoadLabels();
            var line = LineOf(src, "進入 ガンダム 3 4:");

            src.Event.ClearLabel(line + 1);
            Assert.AreEqual(-1, src.Event.SearchLabel("進入 ガンダム 3 4"));

            src.Event.RestoreLabel("進入 ガンダム 3 4");
            Assert.AreEqual(line, src.Event.SearchLabel("進入 ガンダム 3 4"));
        }

        [TestMethod]
        public void IsEventDefined_NormalEventOnly_SkipsAlwaysEventLabels()
        {
            // normal_event_only の場合は常時イベント(*付き)のラベルは数えない
            var (src, _) = LoadLabels();

            Assert.IsTrue(src.Event.IsEventDefined("破壊 グフ"));
            Assert.IsFalse(src.Event.IsEventDefined("破壊 グフ", true));
            Assert.IsTrue(src.Event.IsEventDefined("破壊 ザク", true));
            Assert.IsFalse(src.Event.IsEventDefined("破壊 ドム"));
        }

        [TestMethod]
        [Ignore("移植の不具合: IsEventDefined が SearchLabel(lname, i + 1) を i = 0 から始めるため 0 行目のラベルを見落とす")]
        public void IsEventDefined_LabelOnFirstLine_IsFound()
        {
            // VB の行番号は1始まりで start = 1 から検索するが、C# の EventDataId は0始まり
            var (src, _) = Load("破壊 ザク:", "Exit");

            Assert.AreEqual(0, src.Event.SearchLabel("破壊 ザク"));
            Assert.IsTrue(src.Event.IsEventDefined("破壊 ザク"));
        }

        [TestMethod]
        public void FindLabel_NormalLabelAndSpacingInsensitiveEventLabel()
        {
            // 通常ラベルは重複定義されていれば最初のものが有効。
            // イベントラベルはパラメータ間の空白の違いを無視して探せる
            var (src, _) = LoadLabels();

            Assert.AreEqual(LineOf(src, "通常ラベル:"), src.Event.FindLabel("通常ラベル"));
            Assert.AreEqual(LineOf(src, "ターン 2 味方:"), src.Event.FindLabel("ターン   2  味方"));
            Assert.IsTrue(src.Event.IsLabelDefined("ターン 2 味方"));
            Assert.AreEqual(-1, src.Event.FindLabel("存在しないラベル"));
        }
    }
}
