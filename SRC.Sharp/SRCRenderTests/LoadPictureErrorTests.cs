using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCPlay;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// 壊れた画像・サポートしない形式の画像のエラーメッセージ（元の SRC の Susie.bas の LoadPicture2。7474/SRC#1536）。
    /// 元の SRC は GUI.bas の DrawPicture（6772 行）から LoadPicture2 を呼び、失敗した画像も PicBuf に名前を残すので、
    /// 同じ画像を続けて描いてもメッセージは 1 度だけ出る。
    /// </summary>
    [TestClass]
    public class LoadPictureErrorTests
    {
        private const string CrLf = "\r\n";

        /// <summary>
        /// シナリオを実行し、出た画像のエラーのダイアログの文言を返す（ダイアログは OK で閉じる）。
        /// onGame はゲームスレッドで実行する（シナリオの Wait の間に処理される）。
        /// </summary>
        private static List<string> PlayCollectingErrors(System.Action<ScenarioWorkspace> prepare, System.Action<ScreenGUI> onGame, params string[] commands)
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
                prepare(workspace);
                var lines = new List<string> { "プロローグ:", "ChangeMap test.map", "Show" };
                lines.AddRange(commands);
                lines.Add("Quit");
                lines.Add("");
                workspace.Write("test.eve", lines.ToArray());

                var errors = new List<string>();
                runner.GUI.ErrorShown += msg =>
                {
                    // プロローグだけのシナリオで出る「スタートイベントが定義されていません」などは数えない
                    if (!msg.StartsWith("画像ファイル", System.StringComparison.Ordinal))
                    {
                        return;
                    }
                    lock (errors)
                    {
                        errors.Add(msg);
                    }
                };
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                if (onGame != null)
                {
                    runner.Session.Game.Post(() => onGame(runner.GUI));
                }
                var deadline = System.DateTime.UtcNow + ScreenScenarioRunner.Timeout;
                while (!runner.IsFinished)
                {
                    Assert.IsTrue(System.DateTime.UtcNow < deadline, "timeout");
                    var dialog = runner.State.Dialog;
                    if (dialog != null)
                    {
                        var ok = runner.GUI.Renderer.LayoutDialog(dialog).ButtonRect(0);
                        runner.GUI.PointerPressed(ok.MidX, ok.MidY, GuiButton.Left);
                        runner.GUI.PointerReleased(ok.MidX, ok.MidY, GuiButton.Left);
                        runner.WaitFor(s => s.Dialog != dialog || runner.IsFinished, "dialog closed");
                    }
                    System.Threading.Thread.Sleep(10);
                }
                runner.WaitForTermination();
                lock (errors)
                {
                    return errors.ToList();
                }
            }
        }

        /// <summary>
        /// 読めない画像は「…の読み込み中にエラーが発生しました。画像ファイルが壊れていないか確認して下さい。」を 1 度だけ出す。
        /// 名前は見つかったファイルのパス（元の SRC の pfname = fpath &amp; fname）。SRC# はシナリオなどのフォルダからの相対パスで探すので、そのパスを出す。
        /// </summary>
        [TestMethod]
        public void BrokenPicture_ShowsErrorOnce()
        {
            var errors = PlayCollectingErrors(workspace =>
            {
                File.WriteAllText(Path.Combine(workspace.AppPath, "Bitmap", "Event", "broken.png"), "not a picture");
            }, null,
                "PaintPicture broken.png 0 0",
                "PaintPicture broken.png 32 0",
                "PaintPicture broken.png 64 0 透過");

            Assert.AreEqual(1, errors.Count, string.Join(" / ", errors));
            Assert.AreEqual(
                "画像ファイル" + CrLf
                + Path.Combine("Bitmap", "Event", "broken.png") + CrLf
                + "の読み込み中にエラーが発生しました。" + CrLf
                + "画像ファイルが壊れていないか確認して下さい。",
                errors[0]);
        }

        /// <summary>
        /// .bmp・.jpg・.gif・.png 以外は、中身によらず「…の画像フォーマットはサポートされていません。」を 1 度だけ出す。
        /// PaintPicture・ShowImage は拡張子を確かめるので、パイロット・ユニットの画像（データの指定）を描くときに起きる。
        /// </summary>
        [TestMethod]
        public void UnsupportedPicture_ShowsErrorOnce()
        {
            var results = new List<bool>();
            var errors = PlayCollectingErrors(workspace =>
            {
                workspace.WriteTestImage(Path.Combine("Bitmap", "Pilot", "test.png"));
                File.Copy(Path.Combine(workspace.AppPath, "Bitmap", "Pilot", "test.png"), Path.Combine(workspace.AppPath, "Bitmap", "Pilot", "test.tif"));
            }, gui =>
            {
                results.Add(gui.DrawPicture(@"Pilot\test.tif", 0, 0, 64, 64, 0, 0, 0, 0, ""));
                results.Add(gui.DrawPicture(@"Pilot\test.tif", 64, 0, 64, 64, 0, 0, 0, 0, "透過"));
                results.Add(gui.DrawPicture(@"Pilot\test.png", 128, 0, 64, 64, 0, 0, 0, 0, ""));
            },
                "Wait 5");

            Assert.AreEqual(1, errors.Count, string.Join(" / ", errors));
            Assert.AreEqual(
                "画像ファイル" + CrLf
                + Path.Combine("Bitmap", "Pilot", "test.tif") + CrLf
                + "の画像フォーマットはサポートされていません。",
                errors[0]);
            CollectionAssert.AreEqual(new[] { false, false, true }, results);
        }

        /// <summary>見つからない画像は、元の SRC と同じくメッセージを出さない（DrawPicture が False を返すだけ）。</summary>
        [TestMethod]
        public void MissingPicture_ShowsNoError()
        {
            var errors = PlayCollectingErrors(workspace => { }, null, "PaintPicture missing.png 0 0");
            Assert.AreEqual(0, errors.Count, string.Join(" / ", errors));
        }
    }
}
