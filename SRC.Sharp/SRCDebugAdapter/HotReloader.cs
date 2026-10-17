using SRCAuthoring;
using SRCAuthoring.Analysis;
using SRCAuthoring.Linting;
using SRCCore;
using SRCPlay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SRCDebugAdapter
{
    /// <summary>
    /// デバッグ中のシナリオのイベント・データ・画像を、プレイの状態を保ったまま読み直す（ホットリロード）。
    /// 状態は SRC のクイックセーブと同じ形でメモリに書き出し、読み直した後にクイックロードと同じ処理で戻す。
    /// セーブとロードは、味方フェイズのマップでコマンドを待っている時（イベントやコマンドの処理の途中でない時）だけ行い、
    /// それまでは待つ。画像はいつでも捨て、次に描く時に読み直す。
    /// </summary>
    public sealed class HotReloader : IDisposable
    {
        /// <summary>読み直しのリクエスト（arguments: events・data・images（bool）、files（変わったファイル、表示用））</summary>
        public const string ReloadCommand = "srcsharpReload";
        /// <summary>読み直しの結果のイベント（body: status = waiting・reloaded・failed・canceled、message、diagnostics）</summary>
        public const string ReloadEvent = "srcsharpReload";

        [Flags]
        public enum Kinds
        {
            None = 0,
            Events = 1,
            Data = 2,
            Images = 4,
        }

        private static readonly TimeSpan RetryInterval = TimeSpan.FromMilliseconds(300);

        private readonly ScreenScenarioRunner runner;
        private readonly Func<string, JsonNode, Task> sendEvent;
        private readonly Timer timer;
        private int pending;
        private int posted;
        private bool waitingReported;
        private volatile bool disposed;

        public HotReloader(ScreenScenarioRunner runner, Func<string, JsonNode, Task> sendEvent)
        {
            this.runner = runner;
            this.sendEvent = sendEvent;
            timer = new Timer(_ => Tick());
        }

        private SRC Src => runner.Session.SRC;

        /// <summary>読み直しを受け付ける。結果は ReloadEvent で送る</summary>
        public JsonNode Request(JsonNode args)
        {
            // 足された・名前の変わったファイルを探せるよう、フォルダの一覧のキャッシュを捨てる
            SRCCore.Filesystem.PathResolver.Invalidate();
            var kinds = Kinds.None;
            if (args?["events"]?.GetValue<bool>() ?? false)
            {
                kinds |= Kinds.Events;
            }
            if (args?["data"]?.GetValue<bool>() ?? false)
            {
                kinds |= Kinds.Data;
            }
            if (args?["images"]?.GetValue<bool>() ?? false)
            {
                kinds |= Kinds.Images;
            }
            if (args?["cancel"]?.GetValue<bool>() ?? false)
            {
                Interlocked.Exchange(ref pending, 0);
                Send("canceled", "読み直しを取りやめました");
                return new JsonObject { ["status"] = "canceled" };
            }
            if (kinds == Kinds.None)
            {
                // 種類の指定が無ければすべて
                kinds = Kinds.Events | Kinds.Data | Kinds.Images;
            }
            if (kinds.HasFlag(Kinds.Images))
            {
                // 画像はいつでも捨ててよい（次に描く時に読み直す）
                runner.Session.Game.Post(() => runner.GUI.ClearImageCache());
            }
            int before, after;
            do
            {
                before = pending;
                after = before | (int)kinds;
            }
            while (Interlocked.CompareExchange(ref pending, after, before) != before);
            lock (timer)
            {
                waitingReported = false;
            }
            timer.Change(TimeSpan.Zero, RetryInterval);
            return new JsonObject { ["status"] = "accepted" };
        }

        private void Tick()
        {
            if (disposed || pending == 0)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            // デバッガで止めている間はゲームスレッドが待っているため、再開するまで待つ
            if (runner.IsSuspended || runner.IsFinished)
            {
                return;
            }
            if (Interlocked.Exchange(ref posted, 1) == 0)
            {
                runner.Session.Game.Post(TryApply);
            }
        }

        /// <summary>今ならセーブとロードをしてよいか（ゲームスレッドで呼ぶ）。よくなければ理由を返す</summary>
        private string Unsafe()
        {
            var src = Src;
            if (!runner.Session.Game.IsTopLevel)
            {
                return "イベントやコマンドの処理の途中";
            }
            if (src.Turn <= 0 || src.Stage != "味方")
            {
                return "味方フェイズのマップになっていない";
            }
            if (runner.GUI.IsGUILocked)
            {
                return "画面の操作を受け付けていない";
            }
            if (src.Commands.CommandState != "ユニット選択")
            {
                return "ユニットのコマンドの途中";
            }
            return null;
        }

        private void TryApply()
        {
            Interlocked.Exchange(ref posted, 0);
            if (disposed || pending == 0)
            {
                return;
            }
            var reason = Unsafe();
            if (reason != null)
            {
                lock (timer)
                {
                    if (waitingReported)
                    {
                        return;
                    }
                    waitingReported = true;
                }
                Send("waiting", $"{reason}のため、味方フェイズのマップでコマンドを待つ時に読み直します");
                return;
            }
            var kinds = (Kinds)Interlocked.Exchange(ref pending, 0);
            if ((kinds & (Kinds.Events | Kinds.Data)) == Kinds.None)
            {
                // 画像だけなら描き直す
                runner.GUI.RedrawScreen();
                Send("reloaded", "画像を読み直しました");
                return;
            }
            Apply(kinds);
        }

        private void Apply(Kinds kinds)
        {
            var src = Src;
            var diagnostics = Check(kinds);
            if (diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error))
            {
                Send("failed", "エラーがあるため読み直しませんでした。直して保存し直してください", diagnostics);
                return;
            }

            // 今の状態をクイックセーブと同じ形でメモリに書き出す（クイックセーブの有無は変えない）
            var quickSaveAvailable = src.IsQuickSaveDataAvailable;
            byte[] state;
            using (var stream = new MemoryStream())
            {
                src.DumpData(stream, SRCSaveKind.Quik);
                state = stream.ToArray();
            }
            src.IsQuickSaveDataAvailable = quickSaveAvailable;

            var done = new List<string>();
            runner.GUI.LockGUI();
            try
            {
                if (kinds.HasFlag(Kinds.Data))
                {
                    // 最初の読み込みと同じ順に読む。同じ名前のデータは上書きされる（消したデータは残る）
                    src.ClearDataError();
                    foreach (var title in src.Titles.ToList())
                    {
                        src.IncludeData(title);
                    }
                    src.LoadLocalData();
                    foreach (var error in src.DataErrors)
                    {
                        diagnostics.Add(new SrcDiagnostic { File = error.fname, Line = error.line_num, Severity = DiagnosticSeverity.Warning, Message = $"{error.dname}: {error.msg}" });
                    }
                    done.Add("データ");
                }
                if (kinds.HasFlag(Kinds.Events))
                {
                    src.Event.LoadEventData(src.ScenarioFileName, "クイックロード");
                    done.Add("イベント");
                }
                if (kinds.HasFlag(Kinds.Images))
                {
                    done.Add("画像");
                }
                // クイックロードと同じ処理で状態を戻す（「再開」イベントが起きる）
                using (var stream = new MemoryStream(state))
                {
                    src.RestoreData(stream, SRCSaveKind.Quik);
                }
                runner.GUI.RedrawScreen();
                src.GUIStatus.DisplayGlobalStatus();
            }
            finally
            {
                runner.GUI.UnlockGUI();
            }
            Send("reloaded", string.Join("・", done) + "を読み直しました", diagnostics);
        }

        /// <summary>読み直す前に、イベントは解析、データは lint で調べる</summary>
        private List<SrcDiagnostic> Check(Kinds kinds)
        {
            var src = Src;
            var result = new List<SrcDiagnostic>();
            if (kinds.HasFlag(Kinds.Events))
            {
                var analysis = new ScenarioAnalyzer { AppPath = src.AppPath }.Analyze(src.ScenarioFileName);
                result.AddRange(analysis.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error));
            }
            if (kinds.HasFlag(Kinds.Data))
            {
                var dirs = src.Titles
                    .Select(x => src.SearchDataFolder(x))
                    .Append(Path.Combine(src.ScenarioPath, "Data"))
                    .Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x))
                    .Distinct()
                    .ToList();
                if (dirs.Count > 0)
                {
                    var lint = new DataLinter().LintAsync(dirs, src.ScenarioPath).GetAwaiter().GetResult();
                    result.AddRange(lint.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error && !(x.File ?? "").EndsWith(".eve", StringComparison.OrdinalIgnoreCase)));
                }
            }
            return result;
        }

        private void Send(string status, string message, IEnumerable<SrcDiagnostic> diagnostics = null)
        {
            var list = new JsonArray();
            foreach (var d in diagnostics ?? Enumerable.Empty<SrcDiagnostic>())
            {
                list.Add(new JsonObject
                {
                    ["file"] = d.File,
                    ["line"] = d.Line,
                    ["severity"] = d.Severity == DiagnosticSeverity.Error ? "error" : "warning",
                    ["message"] = d.Message,
                });
            }
            _ = sendEvent(ReloadEvent, new JsonObject { ["status"] = status, ["message"] = message, ["diagnostics"] = list });
        }

        public void Dispose()
        {
            disposed = true;
            timer.Dispose();
        }
    }
}
