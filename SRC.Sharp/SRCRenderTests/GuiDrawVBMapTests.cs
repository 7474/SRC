using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;
using System.IO;

namespace SRCRender.Tests
{
    /// <summary>
    /// 元の SRC（GUI.bas の DrawPicture・DrawString・DrawSysString・DataErrorMessage・DisplayTelop）との突き合わせ（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class GuiDrawVBMapTests
    {
        private static SKBitmap WaitClick(ScreenScenarioRunner runner)
        {
            var src = runner.Session.SRC;
            runner.WaitFor(_ => src.Commands.WaitClickMode && !runner.GUI.IsFormClicked, "Wait Click");
            var line = src.Event.CurrentLineNum;
            var bitmap = SKBitmap.Decode(runner.SnapshotPng());
            runner.GUI.IsFormClicked = true;
            runner.WaitFor(_ => runner.IsTerminated || src.Event.CurrentLineNum != line, "end of Wait Click");
            return bitmap;
        }

        /// <summary>単色の画像を書く</summary>
        private static void WriteSolidImage(ScenarioWorkspace workspace, string name, int width, int height, SKColor color)
        {
            var path = Path.Combine(workspace.AppPath, "Bitmap", "Event", name);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var bitmap = new SKBitmap(width, height))
            {
                bitmap.Erase(color);
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.Create(path))
                {
                    data.SaveTo(stream);
                }
            }
        }

        private static void Play(string[] commands, System.Action<ScenarioWorkspace> prepare, System.Action<ScreenScenarioRunner> check)
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
                workspace.WriteTestImage(Path.Combine("Bitmap", "Event", "test.png"));
                prepare?.Invoke(workspace);
                var lines = new System.Collections.Generic.List<string> { "プロローグ:", "ChangeMap test.map", "Show" };
                lines.AddRange(commands);
                lines.Add("Quit");
                lines.Add("");
                workspace.Write("test.eve", lines.ToArray());
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                check(runner);
                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// 回転で空いた部分は背景色（BGColor。指定が無ければ白、#rrggbb を指定すればその色）で塗る（Graphics.bas の Rotate）。
        /// 透過しなければそのまま描く。
        /// </summary>
        [TestMethod]
        public void DrawPicture_RotateFillsWithBackgroundColor()
        {
            Play(new[]
            {
                "PaintPicture test.png 0 0 右回転 45",
                "PaintPicture test.png 100 0 #0000FF 左回転 45",
                "Refresh",
                "Wait Click",
            }, null, runner =>
            {
                using (var bitmap = WaitClick(runner))
                {
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(1, 1));
                    Assert.AreEqual(SKColors.Blue, bitmap.GetPixel(100 + 1, 1));
                }
            });
        }

        /// <summary>
        /// 部分表示の塗りつぶしは、VB の Line と同じく整数の座標で塗る。上半分は (高さ \ 2) 行目から下、
        /// 下半分は (高さ \ 2 - 1) 行目まで。斜めの部分表示は列ごとに 45 度で塗る（縦横の比に合わせない）。
        /// </summary>
        [TestMethod]
        public void DrawPicture_PartialFillsLikeVbLine()
        {
            Play(new[]
            {
                // 32x33: 上半分は 16 行目から下を塗る
                "PaintPicture odd.png 0 0 上半分",
                // 下半分は 15 行目まで塗る
                "PaintPicture odd.png 40 0 下半分",
                // 64x32 の右上: 列 i は i 行目から下を塗る
                "PaintPicture wide.png 0 100 右上",
                // 左下: 列 i は 0〜i 行目を塗る
                "PaintPicture wide.png 100 100 左下",
                "Refresh",
                "Wait Click",
            }, workspace =>
            {
                WriteSolidImage(workspace, "odd.png", 32, 33, SKColors.Red);
                WriteSolidImage(workspace, "wide.png", 64, 32, SKColors.Red);
            }, runner =>
            {
                using (var bitmap = WaitClick(runner))
                {
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(5, 15));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(5, 16));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(40 + 5, 15));
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(40 + 5, 16));

                    // 列 20: 20 行目から塗る（隅と隅を結んだ線なら 10 行目から塗る）
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(20, 100 + 15));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(20, 100 + 20));
                    // 列 40: 高さを超えるので最後の行だけ塗る
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(40, 100 + 30));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(40, 100 + 31));
                    // 左下の列 20: 0〜20 行目を塗る
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(100 + 20, 100 + 20));
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(100 + 20, 100 + 21));
                    // 列 40: すべての行を塗る
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(100 + 40, 100 + 31));
                }
            });
        }

        /// <summary>
        /// black.bmp・white.bmp は塗りつぶすだけで、透過・効果のオプションを使わない（GUI.bas の DrawPicture は DrewPicture へ飛ぶ）。
        /// </summary>
        [TestMethod]
        public void DrawPicture_WhiteBmpIgnoresOptions()
        {
            Play(new[]
            {
                "PaintPicture white.bmp 0 0 32 32 透過",
                "PaintPicture white.bmp 40 0 32 32 暗",
                "Refresh",
                "Wait Click",
            }, null, runner =>
            {
                using (var bitmap = WaitClick(runner))
                {
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(5, 5));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(40 + 5, 5));
                }
            });
        }

        /// <summary>
        /// 「保持」は、描いたあとの画面のその範囲を picMain(1) に写す（透過した部分は、下に描いてあったものも残る）。
        /// また、描いた範囲を PaintedArea に記録する。
        /// </summary>
        [TestMethod]
        public void DrawPicture_KeepCopiesDrawnArea()
        {
            Play(new[]
            {
                "Line 0 2 200 2 #00FF00",
                "PaintPicture test.png 10 0 透過 保持",
                "ClearPicture",
                "Refresh",
                "Wait Click",
            }, null, runner =>
            {
                using (var bitmap = WaitClick(runner))
                {
                    // 画像の範囲の透過した部分は、下に描いた線が残る
                    Assert.AreEqual(new SKColor(0, 255, 0), bitmap.GetPixel(10 + 2, 2));
                    // 画像の範囲の外の線は消える
                    Assert.AreNotEqual(new SKColor(0, 255, 0), bitmap.GetPixel(150, 2));
                    // 画像は残る
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(10 + 16, 32));
                }
            });
        }

        [TestMethod]
        public void DrawPicture_RecordsPaintedArea()
        {
            Play(new[]
            {
                // Cls で描画した範囲を空にしてから描く
                "Cls",
                "PaintPicture test.png 10 20",
                "Wait Click",
            }, null, runner =>
            {
                WaitClick(runner).Dispose();
                Assert.AreEqual(10, runner.GUI.PaintedAreaX1);
                Assert.AreEqual(20, runner.GUI.PaintedAreaY1);
                Assert.AreEqual(10 + 64, runner.GUI.PaintedAreaX2);
                Assert.AreEqual(20 + 64, runner.GUI.PaintedAreaY2);
            });
        }

        /// <summary>
        /// 改行しない PaintString（末尾が「;」）は、VB の Print msg; と同じく、次の書き込み位置を文字列の右端にする。
        /// </summary>
        [TestMethod]
        public void DrawString_WithoutCrContinuesOnSameLine()
        {
            Play(new[]
            {
                "PaintString 10 10 ああ;",
                "PaintString いい",
                "Refresh",
                "Wait Click",
            }, null, runner =>
            {
                using (var bitmap = WaitClick(runner))
                {
                    var width = runner.GUI.MeasureString("ああ").Width;
                    var lineHeight = runner.GUI.MeasureString("ああ").Height;
                    // 「いい」は「ああ」の右に続けて描く
                    Assert.IsTrue(HasWhite(bitmap, (int)(10 + width) + 1, 10, (int)width - 2, (int)lineHeight), "右に続けて描く");
                    // 次の行には描かない
                    Assert.IsFalse(HasWhite(bitmap, 10, (int)(10 + lineHeight) + 1, (int)width * 2, (int)lineHeight), "改行しない");
                }
            });
        }

        private static bool HasWhite(SKBitmap bitmap, int x, int y, int w, int h)
        {
            for (var yy = y; yy < y + h; yy++)
            {
                for (var xx = x; xx < x + w; xx++)
                {
                    if (bitmap.GetPixel(xx, yy) == SKColors.White)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// DrawSysString のフォント: 戦闘アニメを使うときは 9pt の太字、使わないときは 8pt（GUI.bas の DrawSysString）。
        /// </summary>
        [TestMethod]
        public void DrawSysString_FontFollowsBattleAnimation()
        {
            var animation = ScreenGUI.SysStringPaintFont(true);
            Assert.AreEqual(9, animation.Size);
            Assert.IsTrue(animation.Bold);
            var normal = ScreenGUI.SysStringPaintFont(false);
            Assert.AreEqual(8, normal.Size);
            Assert.IsFalse(normal.Bold);
        }

        /// <summary>
        /// データのエラーの文言の順（GUI.bas の DataErrorMessage）: ファイル名と行・データ名・原因
        /// （どちらも無ければ「データが不正です。」）・データの行。
        /// </summary>
        [TestMethod]
        public void DataErrorMessage_OrderMatchesVb()
        {
            var culture = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("ja-JP");
            try
            {
                Assert.AreEqual(
                    "a.txt：3行目\r\nユニットのデータが不正です。\r\n原因\r\n行",
                    ScreenGUI.FormatDataErrorMessage("原因", "a.txt", 3, "行", "ユニット"));
                Assert.AreEqual(
                    "a.txt：3行目\r\n原因\r\n行",
                    ScreenGUI.FormatDataErrorMessage("原因", "a.txt", 3, "行", ""));
                Assert.AreEqual(
                    "a.txt：3行目\r\nデータが不正です。\r\n行",
                    ScreenGUI.FormatDataErrorMessage("", "a.txt", 3, "行", ""));
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = culture;
            }
        }

        /// <summary>
        /// Telop は最初の 2 つの「.」を改行にする（GUI.bas の DisplayTelop）。
        /// </summary>
        [TestMethod]
        public void DisplayTelop_ReplacesFirstTwoPeriodsWithNewLine()
        {
            Assert.AreEqual("あ\nい", ScreenGUI.TelopText("あ.い"));
            Assert.AreEqual("あ\nい\nう.え", ScreenGUI.TelopText("あ.い.う.え"));
            Assert.AreEqual("あい", ScreenGUI.TelopText("あい"));
        }
    }
}
