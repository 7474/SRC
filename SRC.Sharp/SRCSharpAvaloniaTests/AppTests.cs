using Avalonia.Headless;
using Avalonia.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCRender;
using SRCRender.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCSharpAvalonia.Tests
{
    /// <summary>
    /// SRC#Avalonia の起動（App.StartAsync）。引数のファイルを開き、起動の確かめ（--smoke）の終了コードを返す。
    /// 音声の再生（効果音）が呼ばれることも確かめる（#1341）
    /// </summary>
    [TestClass]
    public class AppTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

        /// <summary>再生の呼び出しを残し、実際の再生（OpenAL）へ渡す</summary>
        private sealed class RecordingPlayer : IPlaySound
        {
            private readonly IPlaySound inner;

            public RecordingPlayer(IPlaySound inner)
            {
                this.inner = inner;
            }

            public readonly List<(int Channel, SoundType Type, long Length)> Played = new List<(int, SoundType, long)>();
            public bool Disposed;

            public BGMStatus BGMStatus => inner.BGMStatus;
            public float SoundVolume { get => inner.SoundVolume; set => inner.SoundVolume = value; }

            public void Play(int channel, Stream stream, SoundType soundType, PlaySoundMode mode)
            {
                lock (Played)
                {
                    Played.Add((channel, soundType, stream.Length));
                }
                inner.Play(channel, stream, soundType, mode);
            }

            public void Stop(int channel) => inner.Stop(channel);
            public SoundType ResolveSoundType(string path) => inner.ResolveSoundType(path);

            public void Dispose()
            {
                Disposed = true;
                inner.Dispose();
            }
        }

        private static void PumpUntil(Func<bool> condition, string description)
        {
            var deadline = DateTime.UtcNow + Timeout;
            while (!condition())
            {
                if (DateTime.UtcNow > deadline)
                {
                    throw new TimeoutException("timeout waiting for " + description);
                }
                Dispatcher.UIThread.RunJobs();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                Thread.Sleep(10);
            }
        }

        /// <summary>起動したアプリ。Shutdown に渡した終了コードを残す</summary>
        private sealed class Started
        {
            public App App;
            public MainWindow Window;
            public int? ExitCode;
        }

        /// <summary>App が開いたウィンドウでシナリオを始めるのと同じく、引数を渡して始める</summary>
        private static Started Start(Action<App> configure, params string[] args)
        {
            var started = new Started { App = new App(), Window = new MainWindow() };
            configure?.Invoke(started.App);
            started.Window.Show();
            _ = started.App.StartAsync(started.Window, args, code =>
            {
                started.ExitCode = code;
                started.App.Stop();
            });
            return started;
        }

        private static void Run(Action action)
        {
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    action();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public void Smoke_MissingFileExits3()
        {
            Run(() =>
            {
                var started = Start(null, "--smoke", "/no/such/scenario.eve");
                PumpUntil(() => started.ExitCode != null, "shutdown");
                Assert.AreEqual(3, started.ExitCode);
                Assert.IsNull(started.App.Session, "シナリオを始めない");
            });
        }

        [TestMethod]
        public void Smoke_ScenarioStartsExits0()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                workspace.WriteMap("test.map", 10, 10, (x, y) => 0);
                var eve = workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "Talk システム",
                    "起動の確かめ",
                    "End",
                    "Exit",
                    "");
                Run(() =>
                {
                    RecordingPlayer player = null;
                    var started = Start(x => x.CreateSoundPlayer = () => player = new RecordingPlayer(x.CreateDefaultSoundPlayer()), eve, "--smoke");
                    PumpUntil(() => started.ExitCode != null, "shutdown");
                    Assert.AreEqual(0, started.ExitCode);
                    // マップかメッセージを出したところで終える（終了コード 0）。シナリオは引数のファイル
                    Assert.AreEqual(eve, started.App.Session.SRC.ScenarioFileName);
                    // 終わるときにシナリオと音声の再生を片付ける
                    Assert.IsTrue(player.Disposed);
                });
            }
        }

        [TestMethod]
        public void Smoke_ScenarioDoesNotStartExits2()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                // メッセージもマップも出さずに待ち続ける
                var eve = workspace.Write("test.eve",
                    "プロローグ:",
                    "Wait 600",
                    "Exit",
                    "");
                Run(() =>
                {
                    var started = Start(x => x.SmokeTimeout = TimeSpan.FromSeconds(1), eve, "--smoke");
                    PumpUntil(() => started.ExitCode != null, "shutdown");
                    Assert.AreEqual(2, started.ExitCode);
                    Assert.IsFalse(App.HasStarted(started.App.Session.GUI.State));
                });
            }
        }

        [TestMethod]
        public void HasStarted()
        {
            Assert.IsFalse(App.HasStarted(null));
            var state = new ScreenState();
            Assert.IsFalse(App.HasStarted(state));
            state.ShowMap = true;
            Assert.IsTrue(App.HasStarted(state));
            state.Dialog = new DialogState();
            Assert.IsFalse(App.HasStarted(state), "ダイアログ（エラーなど）を出しているときは始まっていない");
            state.Dialog = null;
            state.ShowMap = false;
            state.Message.Visible = true;
            Assert.IsTrue(App.HasStarted(state));
        }

        [TestMethod]
        public void PlaySound_CallsPlayerForEffect()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                // E2E の sound と同じく、シナリオの Sound フォルダの WAV を PlaySound で鳴らす
                var dir = new DirectoryInfo(AppContext.BaseDirectory);
                while (!File.Exists(Path.Combine(dir.FullName, "SRC.Sharp.sln")))
                {
                    dir = dir.Parent;
                }
                var wav = File.ReadAllBytes(Path.Combine(dir.FullName, "E2E", "sound", "Sound", "se.wav"));
                Directory.CreateDirectory(Path.Combine(workspace.AppPath, "Sound"));
                File.WriteAllBytes(Path.Combine(workspace.AppPath, "Sound", "se.wav"), wav);
                workspace.WriteMap("test.map", 10, 10, (x, y) => 0);
                var eve = workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "PlaySound se.wav",
                    "Talk システム",
                    "効果音",
                    "End",
                    "Exit",
                    "");
                Run(() =>
                {
                    RecordingPlayer player = null;
                    var started = Start(x => x.CreateSoundPlayer = () => player = new RecordingPlayer(x.CreateDefaultSoundPlayer()), eve);
                    PumpUntil(() => started.App.Session?.GUI.State.Message.Lines.FirstOrDefault() == "効果音", "message");
                    lock (player.Played)
                    {
                        // 効果音のチャンネル（BGM 以外）で WAV を渡している
                        var effect = player.Played.Single();
                        Assert.AreNotEqual(PlaySoundConstants.CH_BGM, effect.Channel);
                        Assert.AreEqual(SoundType.Wave, effect.Type);
                        Assert.AreEqual(wav.Length, effect.Length);
                    }
                    // 設定（srcs.json）が無ければ音量は既定の 50%
                    Assert.AreEqual(0.5f, player.SoundVolume, 0.001f);
                    started.App.Stop();
                    Assert.IsTrue(player.Disposed);
                });
            }
        }
    }
}
