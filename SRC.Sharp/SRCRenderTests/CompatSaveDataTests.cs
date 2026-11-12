using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCCore.Expressions;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using SRCPlay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// 元の SRC（VB5）のセーブデータ（中断データ・クリアデータ）を、検証用のシナリオ「トレス島試験記録」（SRC.Sharp/Compat/torres）で読む（#1577）。
    /// <list type="bullet">
    /// <item>各話の PlayScript（Compat/torres-ep02.play〜torres-ep08.play）の「中断」のところで、元の SRC の形式（VbSaveDataWriter。元の SRC の DumpData を写したもの）で
    /// 中断データを書き、そのファイル（.src）から再開して、中断の前・SRC# の JSON の中断データから読んだ状態と同じになり、
    /// PlayScript の続き（再開の後の expect）が通ることを確かめる</item>
    /// <item>見本を作るシナリオ（Compat/savedata/savedata.eve）を流し、元の SRC の形式のもの
    /// （実機で作った見本 Compat/savedata-expected/suspend.src・clear.src があればそれ、無ければ VbSaveDataWriter で書いたもの）を読んで、
    /// SRC# の JSON の中断データ・クリアデータから読んだ状態と同じになることを確かめる（クリアデータはデータセーブしたときの状態とも比べる）。
    /// 実機の見本の作り方は Compat/PROBE.md</item>
    /// </list>
    /// </summary>
    [TestClass]
    // 乱数（GeneralLib の Rnd）はプロセスで 1 つなので、ほかのテストと並べて流すと系列が乱れ、
    // 戦闘の結果（クリティカルなど）で各話の expect が揺れる。種を compat_play.py と同じ 1 にして、並べずに流す
    [DoNotParallelize]
    public class CompatSaveDataTests
    {
        private const string StopMarker = "# ここから最後まで自動で進める";

        private static string SharpDirectory => Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory);
        private static string CompatDirectory => Path.Combine(SharpDirectory, "Compat");
        private static string ExpectedDirectory => Path.Combine(CompatDirectory, "savedata-expected");

        /// <summary>中断の後に再開のイベントが足す変数（中断の前と比べない）</summary>
        private static readonly string[] ResumeVariables = { "再開記録" };

        [TestMethod]
        [DataRow("torres-ep02.play", "ep02.eve")]
        [DataRow("torres-ep03.play", "ep03.eve")]
        [DataRow("torres-ep04.play", "ep04.eve")]
        [DataRow("torres-ep05.play", "ep05.eve")]
        [DataRow("torres-ep06.play", "ep06.eve")]
        [DataRow("torres-ep07.play", "ep07.eve")]
        [DataRow("torres-ep08.play", "ep08.eve")]
        public void Episode_SuspendInOriginalFormat_Resumes(string playFile, string eveFile)
        {
            var lines = File.ReadAllLines(Path.Combine(CompatDirectory, playFile))
                .TakeWhile(x => !x.StartsWith(StopMarker, StringComparison.Ordinal))
                .ToList();
            var resume = lines.FindIndex(x => x.Trim() == "resume");
            Assert.IsTrue(resume > 0, $"{playFile} に resume の行が無い");
            var savefile = lines.FindLastIndex(resume, x => x.Trim().StartsWith("savefile ", StringComparison.Ordinal));
            Assert.IsTrue(savefile > 0, $"{playFile} の resume の前に savefile の行が無い");

            Run(eveFile, (runner, script, scenario) =>
            {
                // 中断データ（JSON）を書いて SRC が終わるまで。
                // 保存のダイアログに答えるところ（DumpData の直前）で、同じ状態を元の SRC の形式でも書く
                script.Run(lines.Take(savefile + 1));
                var captured = CaptureOnSave(runner, src => new VbSaveDataWriter().DumpData(src).ToBytes());
                script.Run(lines.Take(resume).Skip(savefile + 1));
                Assert.IsTrue(runner.IsFinished);
                Assert.IsNotNull(captured.Bytes, "中断データの保存のダイアログが出なかった");

                var expected = captured.Digest;
                var file = Path.Combine(scenario, Path.GetFileNameWithoutExtension(eveFile) + "を中断.src");
                File.WriteAllBytes(file, captured.Bytes);

                // 起動し直して .src から再開する（元の SRC と同じく中身で中断データと分かる）。
                // 再開のイベントのメッセージで止まったところで、中断の前と比べる
                runner.Restart(file);
                runner.WaitFor(s => s.Message.Visible && s.Message.Waiting, "再開のメッセージ");
                var resumed = Digest(runner.Session.SRC, ResumeVariables);
                AssertSameState(expected, resumed, "中断の前と、元の SRC の形式から再開した後");

                // SRC# の JSON の中断データから読んだ状態とも比べる
                var json = LoadHeadless(scenario, src => src.RestoreData(captured.Path, SRCSaveKind.Suspend));
                AssertSameState(Digest(json, ResumeVariables), resumed, "JSON と元の SRC の形式から再開した後");

                // PlayScript の続き（再開の後の expect と手順）
                script.Run(lines.Skip(resume + 1).Append("expect error none"));
            });
        }

        [TestMethod]
        public void SaveDataScenario_OriginalSamples_ReadLikeJson()
        {
            Run("savedata.eve", (runner, script, scenario) =>
            {
                // 1 ターン目の味方フェイズで中断する（JSON の中断データ）。保存のダイアログに答えるところで元の SRC の形式でも書く
                script.Run(new[]
                {
                    "auto 60",
                    "expect eval 見本のターン = 1",
                    "savefile suspend.srcq",
                });
                var suspended = CaptureOnSave(runner, src => new VbSaveDataWriter().DumpData(src).ToBytes());
                script.Run(new[]
                {
                    "cell 1 1 right",
                    "menu 中断",
                    "wait dialog",
                    "dialog OK",
                    "wait finished",
                });
                var suspendSample = SampleOrWrite("suspend.src", scenario, () => suspended.Bytes);

                // 起動し直し、ターン終了 → 2 ターン目に Continue → インターミッションのデータセーブ（JSON のクリアデータ）
                runner.Restart(Path.Combine(scenario, "savedata.eve"));
                script.Run(new[]
                {
                    "holdlist コマンド",
                    "auto 60",
                    "cell 1 1 right",
                    "menu ターン終了",
                    "wait dialog",
                    "dialog OK",
                    "auto 60",
                    "wait list コマンド",
                    "savefile clear.srcs",
                    "list データセーブ",
                    "wait list コマンド",
                });
                // データセーブは保存のダイアログの後で UList.Update（追加パイロットを消す）をしてから書くので、書いた後の状態から作る
                var clearSample = SampleOrWrite("clear.src", scenario, () => new VbSaveDataWriter().SaveData(runner.Session.SRC).ToBytes());
                var saved = Digest(runner.Session.SRC, new[] { "セーブデータファイル名" }, clear: true);

                // 同じシナリオのフォルダで、JSON と元の SRC の形式を新しく読んで比べる
                var json = LoadHeadless(scenario, src => src.RestoreData(Path.Combine(scenario, "suspend.srcq"), SRCSaveKind.Suspend));
                var original = LoadHeadless(scenario, src => src.RestoreData(suspendSample, SRCSaveKind.Suspend));
                // セーブデータファイル名は、ステージの初めに元の SRC は「～までクリア.src」、SRC# は「～までクリア.srcs」にする。
                // 元の SRC の形式から読んだ値はそのまま残し、保存するときに SRC# の拡張子にする（GeneralLib.ToSaveFileName）
                var saveFileName = new[] { "セーブデータファイル名" };
                AssertSameState(Digest(json, saveFileName), Digest(original, saveFileName), "中断データ（JSON と元の SRC の形式）");
                Assert.AreEqual(
                    SRCCore.Lib.GeneralLib.ToSaveFileName(json.Expression.GetValueAsString("セーブデータファイル名"), "srcs"),
                    SRCCore.Lib.GeneralLib.ToSaveFileName(original.Expression.GetValueAsString("セーブデータファイル名"), "srcs"));
                Assert.AreEqual("局所の値", original.Event.LocalVariableList["見本の局所"].StringValue);
                Assert.IsTrue(original.Event.colEventLabelList.List.Any(x => x.Data == "ターン 1 味方" && !x.Enable));

                // クリアデータは、JSON から読んだ状態と、元の SRC の形式から読んだ状態を比べる。
                // データセーブしたときの状態とも比べる
                json = LoadHeadless(scenario, src => src.LoadData(new MemoryStream(File.ReadAllBytes(Path.Combine(scenario, "clear.srcs")))));
                original = LoadHeadless(scenario, src => src.LoadData(new MemoryStream(File.ReadAllBytes(clearSample))));
                AssertSameState(Digest(json, saveFileName, clear: true), Digest(original, saveFileName, clear: true), "クリアデータ（JSON と元の SRC の形式）");
                AssertSameState(saved, Digest(original, saveFileName, clear: true), "クリアデータ（データセーブしたときと、元の SRC の形式から読んだ後）");
                Assert.AreEqual("savedata.eve", original.Expression.GetValueAsString("次ステージ"));
            });
        }

        /// <summary>
        /// 元の SRC の実機で torres を通したとき（#1549）に保存した中断データ（Compat/savedata-expected/torres）から再開する。
        /// 再開のイベントが起き、マップのコマンドを待つところまでイベントのエラーが出ないこと、
        /// 実機の Lib（汎用戦闘アニメなど）のラベルの分だけ多く保存したイベントのラベルが後ろからそろい、
        /// 中断の前に起きた 1 回だけのラベル（「*」の無いもの）が無効になっていることを確かめる
        /// </summary>
        [TestMethod]
        [DataRow("ep01を中断.src", 1, "破壊 錆蟲(ザコ);攻撃 カイ 敵;行動終了 ノア")]
        [DataRow("ep02を中断.src", 2, "行動終了 ダイチ")]
        [DataRow("ep07を中断.src", 1, "")]
        public void Torres_OriginalSuspend_Resumes(string file, int resumed, string disabledLabels)
        {
            Run(file, (runner, script, scenario) =>
            {
                script.Run(new[]
                {
                    "wait message 試験を再開します",
                    $"expect eval 再開記録 = {resumed}",
                    "auto 60",
                    "expect error none",
                });
                var disabled = runner.Session.SRC.Event.colEventLabelList.List.Where(x => !x.Enable).Select(x => x.Data);
                Assert.AreEqual(disabledLabels, string.Join(";", disabled), file);
            });
        }

        /// <summary>
        /// 元の SRC の実機で torres を通したとき（#1549）に保存したクリアデータ（Compat/savedata-expected/torres）を読んで、
        /// インターミッションから次の話を始め、前の話の部隊と記録を引き継いでいること（単体で始めたときの部隊を作らないこと）を確かめる
        /// </summary>
        [TestMethod]
        [DataRow("ep01までクリア.src", "ep02.eve", "第一話の試験;")]
        [DataRow("ep02までクリア.src", "ep03.eve", "第一話の試験;第二話の調査;")]
        [DataRow("ep03までクリア.src", "ep04.eve", "第一話の試験;第二話の調査;第三話の測量;")]
        [DataRow("ep04までクリア.src", "ep05.eve", "第一話の試験（省略）;第二話の調査（省略）;第三話の測量（省略）;第四話の飛行;")]
        public void Torres_OriginalClear_StartsNextStage(string file, string next, string record)
        {
            Run(file, (runner, script, scenario) =>
            {
                script.Run(new[]
                {
                    "wait list コマンド",
                    $"expect eval 次ステージ = {next}",
                    $"expect eval 試験記録 = \"{record}\"",
                    "expect eval IsDefined(湊カイ, パイロット) = 1",
                    "list 次のステージへ",
                    "auto 60",
                    $"expect eval 試験記録 = \"{record}\"",
                    "expect error none",
                });
            });
        }

        private sealed class Captured
        {
            public byte[] Bytes;
            public List<string> Digest;
            /// <summary>SRC# が JSON で書いたファイル</summary>
            public string Path;
        }

        /// <summary>
        /// 次に保存のダイアログに答えるとき（SRC が DumpData・SaveData で書く直前。ゲームのスレッド）に、そのときの状態を write で書き、状態の行を控える。
        /// 中断の後の SRC は ExitGame でイベントのデータ（ローカル変数・ラベル）を消すので、書いた後の状態からは作らない
        /// </summary>
        private static Captured CaptureOnSave(ScreenScenarioRunner runner, Func<SRC, byte[]> write)
        {
            var captured = new Captured();
            var inner = runner.SaveFileHandler;
            runner.SaveFileHandler = request =>
            {
                if (captured.Bytes == null)
                {
                    captured.Bytes = write(runner.Session.SRC);
                    captured.Digest = Digest(runner.Session.SRC, ResumeVariables, relinkItems: true);
                }
                var path = inner(request);
                captured.Path = captured.Path ?? path;
                return path;
            };
            return captured;
        }

        /// <summary>実機で作った見本があればシナリオのフォルダに写し、無ければ元の SRC の形式で書いたものを置く</summary>
        private static string SampleOrWrite(string name, string scenario, Func<byte[]> write)
        {
            var path = Path.Combine(scenario, name);
            var expected = Path.Combine(ExpectedDirectory, name);
            if (File.Exists(expected))
            {
                File.Copy(expected, path, true);
            }
            else
            {
                File.WriteAllBytes(path, write());
            }
            return path;
        }

        /// <summary>torres と見本を作るシナリオを写したフォルダで eve を始め、action を行う</summary>
        private static void Run(string eveFile, Action<ScreenScenarioRunner, PlayScript, string> action)
        {
            var app = Path.Combine(Path.GetTempPath(), "srcs-basic-data-" + Guid.NewGuid());
            var scenario = Path.Combine(Path.GetTempPath(), "srcs-torres-" + Guid.NewGuid());
            try
            {
                E2ECorpusTests.CopyDirectory(Path.Combine(SharpDirectory, "Content", "basic-data"), app);
                E2ECorpusTests.CopyDirectory(Path.Combine(CompatDirectory, "torres"), scenario);
                foreach (var file in Directory.GetFiles(Path.Combine(CompatDirectory, "savedata"), "*.eve")
                    .Concat(Directory.GetFiles(Path.Combine(ExpectedDirectory, "torres"), "*.src")))
                {
                    File.Copy(file, Path.Combine(scenario, Path.GetFileName(file)));
                }
                using (var runner = new ScreenScenarioRunner(app))
                {
                    var script = new PlayScript(runner)
                    {
                        SnapshotDirectory = Path.Combine(AppContext.BaseDirectory, "TestResults", "e2e", "compat-savedata-" + Path.GetFileNameWithoutExtension(eveFile)),
                    };
                    runner.Session.SRC.InitialRndSeed = 1;
                    runner.Start(Path.Combine(scenario, eveFile));
                    try
                    {
                        action(runner, script, scenario);
                    }
                    catch (PlayScriptException ex)
                    {
                        Assert.Fail(ex.Message + Environment.NewLine
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
                }
            }
            finally
            {
                foreach (var dir in new[] { app, scenario })
                {
                    if (Directory.Exists(dir))
                    {
                        Directory.Delete(dir, true);
                    }
                }
            }
        }

        /// <summary>画面の無い SRC で、シナリオのフォルダのセーブデータを読む</summary>
        private static SRC LoadHeadless(string scenario, Action<SRC> load)
        {
            var src = new SRC();
            var gui = new HeadlessGUI();
            gui.Install(src);
            src.SystemConfig.AppPath = scenario;
            src.ScenarioPath = scenario + Path.DirectorySeparatorChar;
            var fs = new LocalFileSystem();
            fs.AddPath(scenario);
            src.FileSystem = fs;
            src.Event.InitEventData();
            src.ALDList.Load(Path.Combine(scenario, "Data", "System", "alias.txt"));
            src.SPDList.Load(Path.Combine(scenario, "Data", "System", "sp.txt"));
            src.TDList.Load(Path.Combine(scenario, "Data", "System", "terrain.txt"));
            load(src);
            Assert.IsFalse(gui.IsTerminated, string.Join("\n", gui.CallsOf(nameof(IGUI.ErrorMessage)).Select(x => x.Args[0])));
            return src;
        }

        private static void AssertSameState(IList<string> expected, IList<string> actual, string what)
        {
            var missing = expected.Except(actual).ToList();
            var extra = actual.Except(expected).ToList();
            Assert.IsTrue(missing.Count == 0 && extra.Count == 0,
                $"{what}で状態が違う\n期待: \n  {string.Join("\n  ", missing)}\n実際: \n  {string.Join("\n  ", extra)}");
        }

        /// <summary>
        /// セーブデータで戻す状態（ユニット・パイロット・アイテム・資金・ターン・変数・マップ・イベントラベル）を行にする。
        /// 元の SRC は数値の変数も文字列で書くので、変数は文字列にして比べる。追加パイロットは元の SRC も書かないので比べない。
        /// clear はクリアデータ（ターン・ローカル変数・イベントラベル・マップ・ユニットの位置と場所を書かない）。
        /// relinkItems は、アイテムの持ち主を中断データを読んだ後のもの（元の SRC の RestoreData）にする。
        /// 元の SRC の Unit.RestoreLinkInfo は装備しているアイテムの Unit を AddItem0 で設定し直すので、
        /// 合体した後などで Item.Unit が空のまま装備しているアイテム（第二話のミカヅチ。元の SRC も同じ）は、読んだ後は装備しているユニットが持ち主になる（#1579）
        /// </summary>
        internal static List<string> Digest(SRC src, string[] ignoredVariables = null, bool clear = false, bool relinkItems = false)
        {
            ignoredVariables = ignoredVariables ?? new string[0];
            var lines = new List<string>
            {
                (clear ? "" : $"ターン {src.Turn} ") + $"総ターン {src.TotalTurn} 資金 {src.Money}",
                "作品 " + string.Join(";", src.Titles ?? new List<string>()),
            };
            string Value(VarData v) => v.VariableType == SRCCore.Expressions.ValueType.StringType ? v.StringValue : SRCCore.VB.SrcFormatter.Format(v.NumericValue);
            lines.AddRange(src.Event.GlobalVariableList.List.Where(x => !ignoredVariables.Contains(x.Name)).Select(x => $"グローバル変数 {x.Name} = {Value(x)}"));
            if (!clear)
            {
                lines.AddRange(src.Event.LocalVariableList.List.Where(x => !ignoredVariables.Contains(x.Name)).Select(x => $"ローカル変数 {x.Name} = {Value(x)}"));
                lines.Add("無効なラベル " + string.Join(";", src.Event.colEventLabelList.List.Where(x => !x.Enable).Select(x => x.Data)));
            }

            string Ids<T>(IEnumerable<T> items, Func<T, string> id) => "[" + string.Join(",", items.Select(id)) + "]";
            foreach (var p in src.PList.Items.Where(x => !x.IsAdditionalPilot))
            {
                lines.Add($"パイロット {p.ID} {p.Name} {p.Party} Lv{p.Level} Exp{p.Exp} SP{p.SP} 気力{p.Morale} 霊力{p.Plana} 生存{p.Alive} 離脱{p.Away} 追加サポート{p.IsAdditionalSupport} ユニット{p.Unit?.ID}");
            }
            foreach (var u in src.UList.Items)
            {
                var place = clear ? "" : $" ({u.x},{u.y}) {u.Area}";
                lines.Add($"ユニット {u.ID} {u.Name} {u.Party0} ランク{u.Rank} ボス{u.BossRank}{place} 行動{u.UsedAction} {u.Mode} {u.Status}"
                    + $" HP{u.HP}/{u.MaxHP} EN{u.EN}/{u.MaxEN}"
                    + $" パイロット{Ids(u.Pilots, x => x.ID)} サポート{Ids(u.Supports, x => x.ID)} 他形態{Ids(u.OtherForms, x => x.ID)}"
                    + $" 格納{Ids(u.UnitOnBoards, x => x.ID)} 召喚{Ids(u.Servants, x => x.ID)} アイテム{Ids(u.ItemList, x => x.ID)}"
                    + $" 状態{Ids(u.Conditions, x => x.Name + ":" + x.Lifetime)} ＳＰ{Ids(u.SpecialPowersInEffect, x => x.Name)}"
                    + $" 残弾{Ids(Enumerable.Range(1, u.CountWeapon()), i => u.Weapon(i).Bullet().ToString())}"
                    + $" 回数{Ids(Enumerable.Range(1, u.CountAbility()), i => u.Ability(i).Stock().ToString())}"
                    + $" 主{u.Master?.ID} 召喚主{u.Summoner?.ID}");
            }
            foreach (var it in src.IList.List)
            {
                var owner = relinkItems ? src.UList.Items.LastOrDefault(u => u.ItemList.Contains(it)) ?? it.Unit : it.Unit;
                lines.Add($"アイテム {it.ID} {it.Name} 存在{it.Exist} 装備{owner?.ID}");
            }
            if (clear)
            {
                return lines;
            }
            var map = src.Map;
            lines.Add($"マップ {Path.GetFileName(map.MapFileName ?? "")} {map.MapWidth}x{map.MapHeight} {map.MapDrawMode} {map.MapDrawIsMapOnly}");
            if (map.MapData != null)
            {
                lines.Add("地形 " + string.Join(";", map.MapData.All.Select(c => $"{c.TerrainType},{c.BitmapNo},{c.LayerType},{c.LayerBitmapNo},{(int)c.BoxType}")));
            }
            return lines;
        }
    }
}
