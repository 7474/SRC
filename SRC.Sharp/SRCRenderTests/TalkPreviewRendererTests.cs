using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    [TestClass]
    public class TalkPreviewRendererTests
    {
        [TestMethod]
        public void TalkPreview_RendersMessageWindowPngPagesAndTracksCursorLine()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                var eve = workspace.Write("talk.eve",
                    "スタート:",
                    "Talk システム",
                    "1ページ目:2ページ目",
                    "End",
                    "",
                    "AutoTalk 未定義キャラ",
                    "最初のセリフ",
                    "Talk 未定義キャラ",
                    "カーソル位置のセリフ",
                    "End",
                    "Exit");

                // 1つ目のブロック（Talk）: 「:」で 2 ページに分かれる
                var r1 = TalkPreviewRunner.Render(eve, cursorLine: 3);
                Assert.AreEqual(2, r1.Blocks.Count);
                Assert.AreEqual(0, r1.SelectedBlockIndex);
                Assert.AreEqual(2, r1.Pages.Count);
                foreach (var page in r1.Pages)
                {
                    Assert.IsNotNull(page.PngBytes);
                    CollectionAssert.AreEqual(
                        new byte[] { 0x89, 0x50, 0x4E, 0x47 },
                        page.PngBytes.Take(4).ToArray(),
                        $"ページ {page.PageIndex} の出力が PNG でない");
                }
                StringAssert.Contains(string.Join("\n", r1.Pages[0].RenderedLines), "1ページ目");
                StringAssert.Contains(string.Join("\n", r1.Pages[1].RenderedLines), "2ページ目");

                // 2つ目のブロック（AutoTalk）内の 2 番目の話者の行を指定すると StartPageIndex が 1 になる
                var r2 = TalkPreviewRunner.Render(eve, cursorLine: 9);
                Assert.AreEqual(1, r2.SelectedBlockIndex);
                Assert.AreEqual(2, r2.Pages.Count);
                Assert.AreEqual(1, r2.StartPageIndex);
                Assert.AreEqual("未定義キャラ", r2.Pages[1].Speaker);
                Assert.IsTrue(r2.Warnings.Any(w => w.Contains("未定義キャラ")));
            }
        }
    }
}
