using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// 画面と音の見比べ（SRC.Sharp/Compat/screens。#1553）を SRC# で流し、場面ごとの画面と、鳴らした音の記録を書き出す。
    /// 元の SRC（VB5 の Src.exe）のスクリーンショット（SRC.Sharp/Compat/screens-expected）と並べた一覧は
    /// SRC.Sharp/Compat/screens_report.py が作る（手順は SRC.Sharp/Compat/SCREENS.md）。
    /// フォントなどで違うので、画像は比べない。確かめるのは、シナリオが最後まで進み、
    /// 場面の一覧（screens-scenes.txt）のすべての場面を撮れることと、イベントのエラーが無いこと。
    /// </summary>
    [TestClass]
    public class CompatScreensTests
    {
        private static string SharpDirectory => Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory);

        private static string CompatDirectory => Path.Combine(SharpDirectory, "Compat");

        /// <summary>書き出す場所（CI では screenshots の成果物に入る）</summary>
        public static string ResultDirectory => Path.Combine(AppContext.BaseDirectory, "TestResults", "e2e", "compat-screens");

        /// <summary>鳴らした音を記録する。BGM は再生中かどうかも返す（同じ BGM の StartBGM が続けて鳴らすかを決めるため）</summary>
        private sealed class RecordingPlayer : SRCCore.IPlaySound
        {
            private readonly object sync = new object();
            private readonly List<string> events = new List<string>();
            private string lastPath;

            public SRCCore.BGMStatus BGMStatus { get; private set; } = SRCCore.BGMStatus.Stopped;

            public float SoundVolume { get; set; } = 1f;

            /// <summary>ここまでに記録した音を返し、記録を空にする</summary>
            public List<string> Take()
            {
                lock (sync)
                {
                    var result = events.ToList();
                    events.Clear();
                    return result;
                }
            }

            public void Play(int channel, Stream stream, SRCCore.SoundType soundType, SRCCore.PlaySoundMode mode)
            {
                stream.Dispose();
                var name = Path.GetFileName(lastPath ?? "");
                lock (sync)
                {
                    if (channel == SRCCore.PlaySoundConstants.CH_BGM)
                    {
                        BGMStatus = SRCCore.BGMStatus.Playing;
                        events.Add($"BGM 開始 {name}（{(mode == SRCCore.PlaySoundMode.Repeat ? "繰り返す" : "1 回")}）");
                    }
                    else
                    {
                        events.Add($"効果音 {name}");
                    }
                }
            }

            public void Stop(int channel)
            {
                // 効果音の停止（次の効果音の前などに呼ばれる）は記録しない
                if (channel != SRCCore.PlaySoundConstants.CH_BGM)
                {
                    return;
                }
                lock (sync)
                {
                    if (BGMStatus == SRCCore.BGMStatus.Playing)
                    {
                        events.Add("BGM 停止");
                    }
                    BGMStatus = SRCCore.BGMStatus.Stopped;
                }
            }

            public SRCCore.SoundType ResolveSoundType(string path)
            {
                // Sound.cs は Play の直前に、鳴らすファイルの名前でこれを呼ぶ
                lastPath = path;
                switch (Path.GetExtension(path ?? "").ToLowerInvariant())
                {
                    case ".mid":
                        return SRCCore.SoundType.Midi;
                    case ".wav":
                        return SRCCore.SoundType.Wave;
                    case ".mp3":
                        return SRCCore.SoundType.Mp3;
                    default:
                        return SRCCore.SoundType.Unknown;
                }
            }

            public void Dispose()
            {
            }
        }

        /// <summary>場面の一覧（screens-scenes.txt）の ID</summary>
        internal static List<string> SceneIds()
        {
            return File.ReadAllLines(Path.Combine(CompatDirectory, "screens-scenes.txt"), Encoding.UTF8)
                .Where(x => x.Length > 0 && !x.StartsWith("#", StringComparison.Ordinal))
                .Select(x => x.Split('\t')[0])
                .ToList();
        }

        [TestMethod]
        public void Screens()
        {
            var resultDirectory = ResultDirectory;
            if (Directory.Exists(resultDirectory))
            {
                Directory.Delete(resultDirectory, true);
            }
            Directory.CreateDirectory(resultDirectory);

            var app = Path.Combine(Path.GetTempPath(), "srcs-basic-data-" + Guid.NewGuid());
            var scenario = Path.Combine(Path.GetTempPath(), "srcs-screens-" + Guid.NewGuid());
            var snapshots = new List<string>();
            var sounds = new StringBuilder();
            try
            {
                E2ECorpusTests.CopyDirectory(Path.Combine(SharpDirectory, "Content", "basic-data"), app);
                E2ECorpusTests.CopyDirectory(Path.Combine(CompatDirectory, "screens"), scenario);
                var player = new RecordingPlayer();
                using (var runner = new ScreenScenarioRunner(app, soundPlayer: player))
                {
                    var script = new PlayScript(runner)
                    {
                        // 場面ごとに、前の場面からそこまでに鳴った音を記録する
                        SnapshotTaken = (name, png) =>
                        {
                            snapshots.Add(name);
                            File.WriteAllBytes(Path.Combine(resultDirectory, name + ".png"), png);
                            var played = player.Take();
                            sounds.Append(name).Append('\t').Append(played.Count == 0 ? "（なし）" : string.Join(" → ", played)).Append('\n');
                        },
                    };
                    runner.Start(Path.Combine(scenario, "screens.eve"));
                    try
                    {
                        script.Run(File.ReadAllLines(Path.Combine(scenario, "screens.play"), Encoding.UTF8));
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(ex.Message + Environment.NewLine
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
                    var rest = player.Take();
                    if (rest.Count > 0)
                    {
                        sounds.Append("(終わり)\t").Append(string.Join(" → ", rest)).Append('\n');
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

            File.WriteAllText(Path.Combine(resultDirectory, "sounds.txt"), sounds.ToString(), new UTF8Encoding(false));
            CollectionAssert.AreEqual(SceneIds(), snapshots,
                "screens.play の snapshot と screens-scenes.txt の場面の ID（並び）が揃っていない\n"
                + string.Join(",", snapshots));
        }
    }
}
