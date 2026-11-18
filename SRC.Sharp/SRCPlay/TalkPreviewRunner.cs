using SRCCore;
using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.TestLib;
using SRCRender;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace SRCPlay
{
    public sealed class TalkPreviewBlockSummary
    {
        public int Index { get; set; }
        public string Command { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public string EndKind { get; set; }
        public string EnclosingLabel { get; set; }
        public int StepCount { get; set; }
        public string PreviewText { get; set; }
    }

    public sealed class TalkPreviewPage
    {
        public int PageIndex { get; set; }
        public int StepIndex { get; set; }
        public int Line { get; set; }
        public string Command { get; set; }
        public string Speaker { get; set; }
        public string RawSpeaker { get; set; }
        public string ImageOptions { get; set; }
        public string Text { get; set; }
        public string[] RenderedLines { get; set; }
        public byte[] PngBytes { get; set; }
    }

    public sealed class TalkPreviewResult
    {
        public string File { get; set; }
        public string ScenarioRoot { get; set; }
        public string AppPath { get; set; }
        public int CursorLine { get; set; }
        public int SelectedBlockIndex { get; set; }
        public int StartPageIndex { get; set; }
        public List<TalkPreviewBlockSummary> Blocks { get; } = new List<TalkPreviewBlockSummary>();
        public List<TalkPreviewPage> Pages { get; } = new List<TalkPreviewPage>();
        public List<string> Warnings { get; } = new List<string>();

        public string ToJson(bool includeImages = true)
        {
            var payload = new
            {
                file = File,
                scenarioRoot = ScenarioRoot,
                appPath = AppPath,
                cursorLine = CursorLine,
                selectedBlockIndex = SelectedBlockIndex,
                startPageIndex = StartPageIndex,
                blocks = Blocks.Select(b => new
                {
                    index = b.Index,
                    command = b.Command,
                    startLine = b.StartLine,
                    endLine = b.EndLine,
                    endKind = b.EndKind,
                    label = b.EnclosingLabel,
                    stepCount = b.StepCount,
                    previewText = b.PreviewText,
                }),
                pages = Pages.Select(p => new
                {
                    pageIndex = p.PageIndex,
                    stepIndex = p.StepIndex,
                    line = p.Line,
                    command = p.Command,
                    speaker = p.Speaker,
                    rawSpeaker = p.RawSpeaker,
                    imageOptions = p.ImageOptions,
                    text = p.Text,
                    renderedLines = p.RenderedLines,
                    pngBase64 = includeImages && p.PngBytes != null && p.PngBytes.Length > 0
                        ? Convert.ToBase64String(p.PngBytes)
                        : null,
                }),
                warnings = Warnings,
            };
            return JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            });
        }
    }

    /// <summary>
    /// Talk・AutoTalk のブロックを SRCRender の実際のメッセージウィンドウ描画に通し、
    /// 顔画像・名前・改ページ・文字折り返しを含めてページごとの画像（PNG）にする（7474/SRC#1655）。
    /// </summary>
    public static class TalkPreviewRunner
    {
        private static readonly Regex LoadCommandRegex = new Regex(@"^\s*Load\s+(.+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string FindRunRoot(string file)
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(file));
            var root = dir;
            for (var d = dir; !string.IsNullOrEmpty(d); d = Path.GetDirectoryName(d))
            {
                if (File.Exists(Path.Combine(d, "src.ini")) || File.Exists(Path.Combine(d, "Src.ini"))
                    || Directory.Exists(Path.Combine(d, "Data")) || Directory.Exists(Path.Combine(d, "data")))
                {
                    root = d;
                    break;
                }
            }
            if (string.IsNullOrEmpty(root) || root == dir)
            {
                return dir;
            }
            if (Directory.Exists(Path.Combine(root, "Data", "System")) || Directory.Exists(Path.Combine(root, "data", "System")))
            {
                return dir;
            }
            return root;
        }

        private static string FindAppPath(string scenarioRoot)
        {
            for (var d = Path.GetDirectoryName(scenarioRoot ?? ""); !string.IsNullOrEmpty(d); d = Path.GetDirectoryName(d))
            {
                if (Directory.Exists(Path.Combine(d, "Lib")) || Directory.Exists(Path.Combine(d, "Data", "System")))
                {
                    return d;
                }
            }
            return null;
        }

        public static TalkPreviewResult Render(
            string evePath,
            string text = null,
            int cursorLine = 1,
            int? blockIndex = null,
            string scenarioRoot = null,
            string appPath = null)
        {
            var fullEve = !string.IsNullOrEmpty(evePath) ? Path.GetFullPath(evePath) : null;
            var resolvedRoot = !string.IsNullOrEmpty(scenarioRoot)
                ? Path.GetFullPath(scenarioRoot)
                : fullEve != null
                    ? FindRunRoot(fullEve)
                    : Environment.CurrentDirectory;
            var resolvedAppPath = !string.IsNullOrEmpty(appPath)
                ? Path.GetFullPath(appPath)
                : FindAppPath(resolvedRoot) ?? resolvedRoot;
            var content = text ?? (fullEve != null && File.Exists(fullEve) ? File.ReadAllText(fullEve) : "");

            var doc = TalkPreviewScript.Parse(content);
            var result = new TalkPreviewResult
            {
                File = fullEve ?? "",
                ScenarioRoot = resolvedRoot,
                AppPath = resolvedAppPath,
                CursorLine = cursorLine,
            };

            foreach (var b in doc.Blocks)
            {
                var firstUtterance = b.Steps.FirstOrDefault(s => !s.ClearFaceOnly);
                var preview = firstUtterance != null
                    ? (string.IsNullOrEmpty(firstUtterance.Speaker) ? firstUtterance.Text : $"{firstUtterance.Speaker}: {firstUtterance.Text}")
                    : b.Command;
                if (preview.Length > 48)
                {
                    preview = preview.Substring(0, 48) + "…";
                }
                result.Blocks.Add(new TalkPreviewBlockSummary
                {
                    Index = b.Index,
                    Command = b.Command,
                    StartLine = b.StartLine,
                    EndLine = b.EndLine,
                    EndKind = b.EndKind,
                    EnclosingLabel = b.EnclosingLabel,
                    StepCount = b.Steps.Count,
                    PreviewText = preview,
                });
            }

            if (doc.Blocks.Count == 0)
            {
                result.Warnings.Add("Talk または AutoTalk のブロックが見つかりません。");
                return result;
            }

            TalkPreviewBlock selectedBlock;
            int startStepIndex;
            if (blockIndex.HasValue && blockIndex.Value >= 0 && blockIndex.Value < doc.Blocks.Count)
            {
                selectedBlock = doc.Blocks[blockIndex.Value];
                startStepIndex = 0;
            }
            else
            {
                (selectedBlock, startStepIndex) = doc.SelectAtLine(cursorLine);
            }
            result.SelectedBlockIndex = selectedBlock.Index;

            if (selectedBlock.Steps.Count == 0)
            {
                result.Warnings.Add("選択した会話ブロックに表示するセリフがありません。");
                return result;
            }

            var host = new HeadlessHost();
            var prevSkipWait = GeneralLib.SkipWait;
            GeneralLib.SkipWait = true;
            try
            {
                using (var session = new ScreenSession(host, resolvedAppPath, new NullSoundPlayer()))
                {
                    var src = session.SRC;
                    src.AllowMissingAssets = true;
                    src.ShowDataErrorDialogs = false;
                    foreach (var dir in new[] { fullEve != null ? Path.GetDirectoryName(fullEve) : null, resolvedRoot }.Where(d => !string.IsNullOrEmpty(d)).Distinct())
                    {
                        src.FileSystem.AddPath(dir);
                        src.FileSystem.AddSafePath(dir);
                    }
                    src.ScenarioPath = resolvedRoot;
                    src.LoadScenarioConfig();
                    src.GUI.MessageWait = 0;

                    using (var done = new ManualResetEventSlim())
                    {
                        Exception error = null;
                        session.Game.Start();
                        session.Game.Post(() =>
                        {
                            try
                            {
                                RenderOnGameThread(session, doc, content, selectedBlock, startStepIndex, result);
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
                        if (!done.Wait(TimeSpan.FromSeconds(20)))
                        {
                            throw new TimeoutException("会話のプレビューが時間内に終わりませんでした。");
                        }
                        if (error != null)
                        {
                            throw new InvalidOperationException(error.Message, error);
                        }
                    }
                }
            }
            finally
            {
                GeneralLib.SkipWait = prevSkipWait;
            }

            return result;
        }

        private static void RenderOnGameThread(
            ScreenSession session,
            TalkPreviewDocument doc,
            string content,
            TalkPreviewBlock selectedBlock,
            int startStepIndex,
            TalkPreviewResult result)
        {
            var src = session.SRC;
            var gui = session.GUI;

            LoadSystemAndScenarioData(src, content, selectedBlock.StartLine, result.Warnings);

            // Option フラグ（会話パイロット名改行・改行時余白短縮など）をブロック開始前まで反映
            foreach (var opt in doc.Options.Where(o => o.Line < selectedBlock.StartLine))
            {
                var vname = "Option(" + opt.Name + ")";
                if (opt.Enabled)
                {
                    if (!src.Expression.IsGlobalVariableDefined(vname))
                    {
                        src.Expression.DefineGlobalVariable(vname);
                    }
                    src.Expression.SetVariableAsLong(vname, 1);
                }
                else if (src.Expression.IsGlobalVariableDefined(vname))
                {
                    src.Expression.UndefineVariable(vname);
                }
            }

            gui.OpenMessageForm(null, null);

            var currentStepIndex = 0;
            TalkPreviewStep currentStep = null;
            var capturedStart = false;

            gui.OnMessagePageRendered = () =>
            {
                if (currentStep == null)
                {
                    return;
                }
                var png = gui.RenderMessageWindowPng();
                var renderedLines = gui.State.Message.Lines.ToArray();
                var speaker = string.IsNullOrEmpty(gui.State.Message.Speaker) ? currentStep.Speaker : gui.State.Message.Speaker;
                if (!capturedStart && currentStepIndex >= startStepIndex)
                {
                    result.StartPageIndex = result.Pages.Count;
                    capturedStart = true;
                }
                result.Pages.Add(new TalkPreviewPage
                {
                    PageIndex = result.Pages.Count,
                    StepIndex = currentStepIndex,
                    Line = currentStep.Line,
                    Command = currentStep.Command,
                    Speaker = speaker,
                    RawSpeaker = currentStep.Speaker,
                    ImageOptions = currentStep.ImageOptions,
                    Text = currentStep.Text,
                    RenderedLines = renderedLines,
                    PngBytes = png,
                });
                gui.AdvanceMessage();
            };

            for (var i = 0; i < selectedBlock.Steps.Count; i++)
            {
                currentStepIndex = i;
                currentStep = selectedBlock.Steps[i];
                EnsureFallbackSpeaker(src, currentStep.Speaker, result.Warnings);

                if (currentStep.ClearFaceOnly)
                {
                    if (currentStep.Command == "AutoTalk")
                    {
                        gui.DisplayBattleMessage(currentStep.Speaker, "", currentStep.ImageOptions);
                    }
                    else
                    {
                        gui.DisplayMessage(currentStep.Speaker, "", currentStep.ImageOptions);
                    }
                    continue;
                }

                if (currentStep.Command == "AutoTalk")
                {
                    gui.DisplayBattleMessage(currentStep.Speaker, currentStep.Text, currentStep.ImageOptions);
                }
                else
                {
                    gui.DisplayMessage(currentStep.Speaker, currentStep.Text, currentStep.ImageOptions);
                }
            }

            gui.OnMessagePageRendered = null;
            gui.CloseMessageForm();
        }

        private static void LoadSystemAndScenarioData(SRC src, string content, int beforeLine, List<string> warnings)
        {
            foreach (var aliasFile in new[]
            {
                src.FileSystem.PathCombine(src.ScenarioPath, "Data", "System", "alias.txt"),
                src.FileSystem.PathCombine(src.AppPath, "Data", "System", "alias.txt"),
            })
            {
                if (src.FileSystem.FileExists(aliasFile))
                {
                    src.ALDList.Load(aliasFile);
                    break;
                }
            }
            foreach (var spFile in new[]
            {
                src.FileSystem.PathCombine(src.ScenarioPath, "Data", "System", "sp.txt"),
                src.FileSystem.PathCombine(src.ScenarioPath, "Data", "System", "mind.txt"),
                src.FileSystem.PathCombine(src.AppPath, "Data", "System", "sp.txt"),
                src.FileSystem.PathCombine(src.AppPath, "Data", "System", "mind.txt"),
            })
            {
                if (src.FileSystem.FileExists(spFile))
                {
                    src.SPDList.Load(spFile);
                    break;
                }
            }

            // Load コマンドで読まれる外部作品フォルダ（プレビュー対象より前の行）
            var rawLines = content.Replace("\r\n", "\n").Split('\n');
            for (var i = 0; i < Math.Min(rawLines.Length, beforeLine); i++)
            {
                var m = LoadCommandRegex.Match(rawLines[i]);
                if (!m.Success)
                {
                    continue;
                }
                var titles = m.Groups[1].Value.Split(new[] { ' ', '\t', '\u3000' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var title in titles)
                {
                    if (title.IndexOfAny(new[] { '$', '(', '`', '[' }) >= 0)
                    {
                        continue;
                    }
                    var folder = src.SearchDataFolder(title);
                    if (!string.IsNullOrEmpty(folder))
                    {
                        try
                        {
                            src.LoadDataDirectory(folder);
                        }
                        catch (Exception ex)
                        {
                            warnings.Add($"データ「{title}」の読み込みで警告: {ex.Message}");
                        }
                    }
                }
            }

            try
            {
                src.LoadLocalData();
            }
            catch (Exception ex)
            {
                warnings.Add($"シナリオデータの読み込みで警告: {ex.Message}");
            }
        }

        private static void EnsureFallbackSpeaker(SRC src, string speaker, List<string> warnings)
        {
            if (string.IsNullOrEmpty(speaker) || speaker == "システム")
            {
                return;
            }
            if (src.PList.IsDefined(speaker) || src.PDList.IsDefined(speaker) || src.NPDList.IsDefined(speaker))
            {
                return;
            }
            var npd = src.NPDList.Add(speaker);
            npd.Nickname = speaker;
            npd.Bitmap = "-.bmp";
            var msg = $"パイロット「{speaker}」が定義されていないため、仮の名前で表示しました。";
            if (!warnings.Contains(msg))
            {
                warnings.Add(msg);
            }
        }

        private sealed class HeadlessHost : IScreenHost
        {
            public void Present(SKBitmap frame) { }
            public void SetTitle(string title) { }
            public bool Terminate() => false;
        }
    }
}
