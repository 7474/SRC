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
    /// 実機との比較のプローブ（SRC.Sharp/Compat/probe・probe-bounds・probe-errors。#1550）を SRC# で流し、
    /// シナリオが書き出した「項目名&lt;TAB&gt;値」のファイルと、エラーのダイアログの文言を、
    /// 元の SRC（VB5 の Src.exe）の出力（SRC.Sharp/Compat/probe-expected）と比べる。
    /// 期待値のファイルがまだ無いケースは、SRC# の出力を TestResults/e2e/compat-probe/ に書き出すだけで成功にする
    /// （実機の出力を置けば、そのまま比べる。手順は SRC.Sharp/Compat/PROBE.md）。
    /// </summary>
    [TestClass]
    public class CompatProbeTests
    {
        /// <summary>比べない項目（実行する環境で変わるもの）の項目名の頭</summary>
        private const string ReferencePrefix = "参考.";

        /// <summary>エラーのダイアログの区切り（期待値の *.errors.txt と同じ書き方）</summary>
        private const string ErrorSeparator = "----";

        private static string SharpDirectory => Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory);

        private static string CompatDirectory => Path.Combine(SharpDirectory, "Compat");

        private static string ExpectedDirectory => Path.Combine(CompatDirectory, "probe-expected");

        static CompatProbeTests()
        {
            // 元の SRC の出力（Shift_JIS）を読むため
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// ケース（シナリオのフォルダの Compat からの相対パスと、イベントファイルの名前）。
        /// probe/probe.eve・probe-bounds/*.eve・probe-errors/*/probe.eve
        /// </summary>
        public static IEnumerable<object[]> Cases
        {
            get
            {
                var compat = Path.Combine(Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory), "Compat");
                var cases = new List<(string folder, string eve)> { ("probe", "probe.eve") };
                cases.AddRange(Directory.GetFiles(Path.Combine(compat, "probe-bounds"), "*.eve")
                    .Select(x => ("probe-bounds", Path.GetFileName(x))));
                cases.AddRange(Directory.GetDirectories(Path.Combine(compat, "probe-errors"))
                    .Where(x => File.Exists(Path.Combine(x, "probe.eve")))
                    .Select(x => ("probe-errors/" + Path.GetFileName(x), "probe.eve")));
                return cases
                    .OrderBy(x => x.folder, StringComparer.Ordinal)
                    .ThenBy(x => x.eve, StringComparer.Ordinal)
                    .Select(x => new object[] { x.folder, x.eve });
            }
        }

        [TestMethod]
        [DynamicData(nameof(Cases))]
        public void Probe(string folder, string eve)
        {
            var name = Path.GetFileNameWithoutExtension(eve);
            var (output, errors, dataErrors, trace) = Run(folder, eve);

            // SRC# の出力を書き出す（実機の出力と見比べるため。CI ではスクリーンショットと一緒に回収する）
            var resultDirectory = Path.Combine(AppContext.BaseDirectory, "TestResults", "e2e", "compat-probe", folder);
            Directory.CreateDirectory(resultDirectory);
            var outputPath = Path.Combine(resultDirectory, name + ".txt");
            if (output != null)
            {
                File.WriteAllText(outputPath, output, new UTF8Encoding(false));
            }
            else if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            File.WriteAllText(Path.Combine(resultDirectory, name + ".errors.txt"), FormatErrors(errors), new UTF8Encoding(false));
            // SRC# はデータの誤りをダイアログに出さず SRC.DataErrors に集める（読み込みを打ち切る誤りは例外にする）。見比べるために別に書き出す
            File.WriteAllText(Path.Combine(resultDirectory, name + ".dataerrors.txt"), string.Join("\n", dataErrors) + (dataErrors.Count > 0 ? "\n" : ""), new UTF8Encoding(false));

            // 期待値が無くても確かめること: 主なプローブと範囲外の引数のシナリオは、SRC# では最後まで進む
            if (!folder.StartsWith("probe-errors", StringComparison.Ordinal))
            {
                Assert.IsNotNull(output, $"{folder}/{eve}: 出力のファイルができていない\n{trace}");
                Assert.IsTrue(Parse(output).ContainsKey("終了"), $"{folder}/{eve}: 最後（終了）まで進んでいない\n{output}\n{trace}");
            }

            var expectedOutputPath = Path.Combine(ExpectedDirectory, folder, name + ".txt");
            var expectedErrorsPath = Path.Combine(ExpectedDirectory, folder, name + ".errors.txt");
            if (!File.Exists(expectedOutputPath) && !File.Exists(expectedErrorsPath))
            {
                // 実機の出力がまだ無い。SRC# の出力を書き出すだけ
                return;
            }

            var known = LoadKnownDifferences($"{folder}/{eve}");
            var differences = new List<string>();
            if (File.Exists(expectedOutputPath))
            {
                if (output == null)
                {
                    differences.Add("元の SRC は出力のファイルを書いたが、SRC# は書かなかった（読み込みを打ち切った・途中で止まった）");
                }
                else
                {
                    differences.AddRange(CompareOutputs(Parse(ReadText(expectedOutputPath)), Parse(output), known));
                }
            }
            else if (output != null)
            {
                differences.Add("元の SRC は出力のファイルを書かなかった（読み込みを打ち切った・途中で落ちた）が、SRC# は書いた");
            }

            if (File.Exists(expectedErrorsPath) && !known.ContainsKey("#errors"))
            {
                var expectedErrors = SplitErrors(ReadText(expectedErrorsPath));
                var actualErrors = errors.Select(NormalizeError).ToList();
                if (!expectedErrors.SequenceEqual(actualErrors))
                {
                    differences.Add("エラーのダイアログが違う\n--- 元の SRC\n" + string.Join("\n" + ErrorSeparator + "\n", expectedErrors)
                        + "\n--- SRC#\n" + string.Join("\n" + ErrorSeparator + "\n", actualErrors));
                }
            }

            File.WriteAllText(Path.Combine(resultDirectory, name + ".diff.txt"), string.Join("\n", differences), new UTF8Encoding(false));
            Assert.AreEqual(0, differences.Count, $"{folder}/{eve}: 元の SRC の出力と違う（{differences.Count} 件。理由のあるものは probe-expected/known-differences.txt に書く）\n"
                + string.Join("\n", differences));
        }

        /// <summary>
        /// シナリオを流し、(出力のファイルの中身（無ければ null）, エラーのダイアログの文言, SRC# が集めたデータの誤りと打ち切りの例外, 操作の記録) を返す
        /// </summary>
        private static (string output, IList<string> errors, IList<string> dataErrors, string trace) Run(string folder, string eve)
        {
            var app = Path.Combine(Path.GetTempPath(), "srcs-basic-data-" + Guid.NewGuid());
            var scenario = Path.Combine(Path.GetTempPath(), "srcs-probe-" + Guid.NewGuid());
            try
            {
                E2ECorpusTests.CopyDirectory(Path.Combine(SharpDirectory, "Content", "basic-data"), app);
                E2ECorpusTests.CopyDirectory(Path.Combine(CompatDirectory, folder), scenario);
                using (var runner = new ScreenScenarioRunner(app))
                {
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(scenario, eve));
                    try
                    {
                        // メッセージ・エラーのダイアログを進めながら、シナリオが終わる（Quit・読み込みの打ち切り）まで流す
                        script.Run(new[] { "auto 120", "expect finished" });
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail($"{folder}/{eve}: {ex.Message}\n"
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
                    var path = Path.Combine(scenario, "out_" + Path.GetFileNameWithoutExtension(eve) + ".txt");
                    var output = File.Exists(path) ? ReadText(path) : null;
                    var trace = string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 20)));
                    var dataErrors = runner.Session.SRC.DataErrors
                        .Select(x => $"{Path.GetFileName(x.fname)}({x.line_num}): {x.dname}: {x.msg}")
                        .ToList();
                    if (runner.Error is SRCCore.Exceptions.InvalidSrcDataException invalid)
                    {
                        dataErrors.AddRange(invalid.InvalidDataList.Select(x => $"{Path.GetFileName(x.fname)}({x.line_num}): {x.dname}: {x.msg}（打ち切り）"));
                    }
                    else if (runner.Error != null && !(runner.Error is SRCCore.TestLib.HeadlessTerminatedException))
                    {
                        dataErrors.Add("例外: " + runner.Error.GetType().Name + ": " + runner.Error.Message);
                    }
                    return (output, script.ErrorMessages, dataErrors, trace);
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

        /// <summary>UTF-8 として読めればそのまま、読めなければ Shift_JIS（元の SRC・SRC# の Open の既定）として読む。改行は LF にそろえる</summary>
        internal static string ReadText(string path)
        {
            var bytes = File.ReadAllBytes(path);
            string text;
            try
            {
                text = new UTF8Encoding(false, true).GetString(bytes);
                if (text.Length > 0 && text[0] == '﻿')
                {
                    text = text.Substring(1);
                }
            }
            catch (DecoderFallbackException)
            {
                text = Encoding.GetEncoding(932).GetString(bytes);
            }
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        /// <summary>「項目名&lt;TAB&gt;値」の行を読む。同じ項目名が 2 回あれば後の行を使う（エラーになるかもしれない項目は、先に「&lt;エラー&gt;」を書く）</summary>
        internal static Dictionary<string, string> Parse(string text)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var line in text.Split('\n'))
            {
                if (line.Length == 0)
                {
                    continue;
                }
                var tab = line.IndexOf('\t');
                if (tab < 0)
                {
                    result[line] = "";
                }
                else
                {
                    result[line.Substring(0, tab)] = line.Substring(tab + 1);
                }
            }
            return result;
        }

        internal static IEnumerable<string> CompareOutputs(Dictionary<string, string> expected, Dictionary<string, string> actual, Dictionary<string, string> known)
        {
            foreach (var key in expected.Keys.Concat(actual.Keys).Distinct().OrderBy(x => x, StringComparer.Ordinal))
            {
                if (key.StartsWith(ReferencePrefix, StringComparison.Ordinal) || known.ContainsKey(key))
                {
                    continue;
                }
                var hasExpected = expected.TryGetValue(key, out var e);
                var hasActual = actual.TryGetValue(key, out var a);
                if (!hasExpected)
                {
                    yield return $"{key}: 元の SRC の出力に無い（SRC# は「{a}」）";
                }
                else if (!hasActual)
                {
                    yield return $"{key}: SRC# の出力に無い（元の SRC は「{e}」）";
                }
                else if (e != a)
                {
                    yield return $"{key}: 元の SRC は「{e}」、SRC# は「{a}」";
                }
            }
        }

        /// <summary>
        /// 理由のある違い（probe-expected/known-differences.txt）。1 行に「ケース&lt;TAB&gt;項目名&lt;TAB&gt;理由」。
        /// ケースは「probe/probe.eve」のように書く。項目名を「#errors」にすると、エラーのダイアログを比べない
        /// </summary>
        private static Dictionary<string, string> LoadKnownDifferences(string caseName)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            var path = Path.Combine(ExpectedDirectory, "known-differences.txt");
            if (!File.Exists(path))
            {
                return result;
            }
            foreach (var line in ReadText(path).Split('\n'))
            {
                if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal) && !line.StartsWith("#errors", StringComparison.Ordinal))
                {
                    continue;
                }
                var cols = line.Split('\t');
                if (cols.Length >= 2 && cols[0] == caseName)
                {
                    result[cols[1]] = cols.Length >= 3 ? cols[2] : "";
                }
            }
            return result;
        }

        /// <summary>
        /// エラーのダイアログの文言を比べられる形にする。1 行目（「ファイル名：n行目」）のファイル名はフォルダを除き、
        /// 行の終わりの空白と、終わりの空の行を除く
        /// </summary>
        internal static string NormalizeError(string message)
        {
            var lines = message.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n').Select(x => x.TrimEnd()).ToList();
            if (lines.Count > 0)
            {
                var first = lines[0];
                var colon = first.IndexOf('：');
                if (colon > 0)
                {
                    var file = first.Substring(0, colon);
                    var cut = Math.Max(file.LastIndexOf('\\'), file.LastIndexOf('/'));
                    lines[0] = file.Substring(cut + 1) + first.Substring(colon);
                }
            }
            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }
            return string.Join("\n", lines);
        }

        private static string FormatErrors(IEnumerable<string> errors)
        {
            var list = errors.Select(NormalizeError).ToList();
            return list.Count == 0 ? "" : string.Join("\n" + ErrorSeparator + "\n", list) + "\n";
        }

        /// <summary>期待値の *.errors.txt を、ダイアログごとに分ける（「----」だけの行で区切る）</summary>
        internal static IList<string> SplitErrors(string text)
        {
            var result = new List<string>();
            var current = new List<string>();
            foreach (var line in text.Split('\n'))
            {
                if (line.Trim() == ErrorSeparator)
                {
                    result.Add(NormalizeError(string.Join("\n", current)));
                    current.Clear();
                }
                else
                {
                    current.Add(line);
                }
            }
            if (current.Any(x => x.Trim().Length > 0))
            {
                result.Add(NormalizeError(string.Join("\n", current)));
            }
            return result.Where(x => x.Length > 0).ToList();
        }
    }
}
