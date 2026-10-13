using SRCAuthoring.Analysis;
using SRCCore.CmdDatas;
using SRCCore.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SRCDebugAdapter
{
    /// <summary>止まった理由（DAP の stopped イベントの reason）</summary>
    public sealed class StopInfo
    {
        public string Reason { get; set; }
        public string Text { get; set; }
        public EventDataLine Line { get; set; }
    }

    /// <summary>
    /// イベントのデバッガ。ゲームスレッドでコマンドを実行する前に呼ばれ、止める条件に合えば再開されるまで待つ。
    /// </summary>
    public sealed class EventDebugger : IEventDebugger
    {
        private enum StepMode
        {
            None,
            Pause,
            StepIn,
            Next,
            StepOut,
        }

        private readonly SRCCore.SRC src;
        private readonly object sync = new object();
        private readonly Dictionary<string, HashSet<int>> breakpoints = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
        private readonly SemaphoreSlim resume = new SemaphoreSlim(0);
        private StepMode step = StepMode.None;
        private int stepDepth;
        private volatile bool terminated;

        public EventDebugger(SRCCore.SRC src)
        {
            this.src = src;
        }

        /// <summary>止まった時（ゲームスレッドから呼ばれる）</summary>
        public Action<StopInfo> Stopped { get; set; }

        /// <summary>再開した時</summary>
        public Action Resumed { get; set; }

        /// <summary>イベントのエラーで止めるか</summary>
        public bool StopOnError { get; set; } = true;

        /// <summary>止まっているか</summary>
        public bool IsPaused { get; private set; }

        /// <summary>止まっている行（止まっていなければ null）</summary>
        public EventDataLine CurrentLine { get; private set; }

        /// <summary>止まっている時の Call の深さ</summary>
        public int CurrentDepth { get; private set; }

        public void SetBreakpoints(string file, IEnumerable<int> lines)
        {
            lock (sync)
            {
                breakpoints[OverlayFileSystem.Normalize(file)] = new HashSet<int>(lines);
            }
        }

        /// <summary>最初のコマンドで止める</summary>
        public void StopOnEntry()
        {
            lock (sync)
            {
                step = StepMode.StepIn;
            }
        }

        public void Pause()
        {
            lock (sync)
            {
                step = StepMode.Pause;
            }
        }

        public void Continue() => Release(StepMode.None);
        public void StepIn() => Release(StepMode.StepIn);
        public void Next() => Release(StepMode.Next);
        public void StepOut() => Release(StepMode.StepOut);

        /// <summary>止まっていれば再開し、以後は止めない</summary>
        public void Terminate()
        {
            terminated = true;
            Release(StepMode.None);
        }

        private void Release(StepMode mode)
        {
            lock (sync)
            {
                step = mode;
                stepDepth = CurrentDepth;
                if (!IsPaused)
                {
                    return;
                }
                IsPaused = false;
            }
            resume.Release();
        }

        private bool HasBreakpoint(EventDataLine line)
        {
            if (line == null || string.IsNullOrEmpty(line.File))
            {
                return false;
            }
            lock (sync)
            {
                return breakpoints.Count > 0
                    && breakpoints.TryGetValue(OverlayFileSystem.Normalize(line.File), out var lines)
                    && lines.Contains(line.LineNum);
            }
        }

        public void BeforeExecute(CmdData command)
        {
            if (terminated)
            {
                return;
            }
            var line = command.EventData;
            var depth = src.Event.CallDepth;
            string reason = null;
            lock (sync)
            {
                switch (step)
                {
                    case StepMode.Pause:
                        reason = "pause";
                        break;
                    case StepMode.StepIn:
                        reason = "step";
                        break;
                    case StepMode.Next:
                        if (depth <= stepDepth)
                        {
                            reason = "step";
                        }
                        break;
                    case StepMode.StepOut:
                        if (depth < stepDepth)
                        {
                            reason = "step";
                        }
                        break;
                }
            }
            if (reason == null && HasBreakpoint(line))
            {
                reason = "breakpoint";
            }
            if (reason != null)
            {
                Stop(new StopInfo { Reason = reason, Line = line });
            }
        }

        public void OnError(EventDataLine line, string message)
        {
            if (!terminated && StopOnError)
            {
                Stop(new StopInfo { Reason = "exception", Text = message, Line = line });
            }
        }

        private void Stop(StopInfo info)
        {
            lock (sync)
            {
                IsPaused = true;
                step = StepMode.None;
                CurrentLine = info.Line;
                CurrentDepth = src.Event.CallDepth;
            }
            Stopped?.Invoke(info);
            resume.Wait();
            CurrentLine = null;
            Resumed?.Invoke();
        }

        /// <summary>止まっている位置からのスタック（先頭が現在の行。続いて Call した行）</summary>
        public IList<EventDataLine> StackLines()
        {
            var result = new List<EventDataLine>();
            if (CurrentLine == null)
            {
                return result;
            }
            result.Add(CurrentLine);
            var data = src.Event.EventData;
            for (var d = CurrentDepth - 1; d >= 0; d--)
            {
                var id = src.Event.CallStack[d];
                if (d == 0 && id == 0)
                {
                    break;
                }
                if (id >= 0 && id < data.Count)
                {
                    result.Add(data[id]);
                }
            }
            return result;
        }

        /// <summary>Local で作った変数（サブルーチンのローカル変数）</summary>
        public IList<(string Name, string Value)> LocalVariables()
        {
            var ev = src.Event;
            var result = new List<(string, string)>();
            for (var i = 0; i <= ev.VarIndex && i < ev.VarStack.Count; i++)
            {
                var v = ev.VarStack[i];
                if (!string.IsNullOrEmpty(v.Name))
                {
                    result.Add((v.Name, Format(v)));
                }
            }
            foreach (var v in ev.LocalVariableList.Values)
            {
                result.Add((v.Name, Format(v)));
            }
            return result;
        }

        public IList<(string Name, string Value)> GlobalVariables()
        {
            return src.Event.GlobalVariableList.Values
                .OrderBy(x => x.Name, StringComparer.Ordinal)
                .Select(x => (x.Name, Format(x)))
                .ToList();
        }

        private static string Format(SRCCore.Expressions.VarData v)
        {
            return v.VariableType == SRCCore.Expressions.ValueType.NumericType
                ? SRCCore.VB.SrcFormatter.Format(v.NumericValue)
                : "\"" + v.StringValue + "\"";
        }
    }
}
