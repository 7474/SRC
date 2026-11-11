using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// 「Now Loading ...」の画面と進み具合のバー（元の SRC の NowLoading.frm・GUI.bas の OpenNowLoadingForm・
    /// SetLoadImageSize・DisplayLoadingProgress。7474/SRC#1536）。
    /// </summary>
    [TestClass]
    public class NowLoadingFormTests
    {
        // NowLoading.frm: ClientWidth 3210・ClientHeight 1320（Twip）＝ 214×88 ピクセル。灰色（&H00C0C0C0&）
        private const int ClientWidth = 214;
        private const int ClientHeight = 88;
        // picBar: Left 240・Top 840・Width 2745・Height 195（Twip）＝ 左 16・上 56・183×13。3D の枠（2 ピクセル）の内側は 179×9
        private const int BarLeft = 16 + 2;
        private const int BarTop = 56 + 2;
        private const int BarWidth = 179;
        private const int BarHeight = 9;

        private static readonly SKColor FormGray = new SKColor(0xC0, 0xC0, 0xC0);
        // FillColor &H00800000&（BGR）
        private static readonly SKColor BarColor = new SKColor(0x00, 0x00, 0x80);

        /// <summary>画面の中央に出したダイアログのクライアント領域の左上（タイトルバーの下）</summary>
        private static SKPointI ClientOrigin()
        {
            var left = (ScreenLayout.Width - ClientWidth) / 2;
            var top = (ScreenLayout.Height - (ClientHeight + Compat.Form.TitleBarHeight)) / 2;
            return new SKPointI(left, top + Compat.Form.TitleBarHeight);
        }

        [TestMethod]
        public void Progress_FillsBarByValueOverSize()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.GUI.OpenNowLoadingForm();
                runner.GUI.SetLoadImageSize(4);
                runner.GUI.DisplayLoadingProgress();
                runner.GUI.DisplayLoadingProgress();

                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    var o = ClientOrigin();
                    // ダイアログの地は灰色
                    Assert.AreEqual(FormGray, bitmap.GetPixel(o.X + 4, o.Y + 4));
                    Assert.AreEqual(FormGray, bitmap.GetPixel(o.X + ClientWidth - 8, o.Y + ClientHeight - 8));
                    // Progress: picBar.Line (0, 0)-(ScaleWidth * Value \ Size, ScaleHeight), , BF。179 * 2 \ 4 = 89 で、両端を含む 90 ピクセル
                    var y = o.Y + BarTop + BarHeight / 2;
                    Assert.AreEqual(BarColor, bitmap.GetPixel(o.X + BarLeft, y));
                    Assert.AreEqual(BarColor, bitmap.GetPixel(o.X + BarLeft + 89, y));
                    Assert.AreEqual(BarColor, bitmap.GetPixel(o.X + BarLeft + 89, o.Y + BarTop));
                    Assert.AreEqual(BarColor, bitmap.GetPixel(o.X + BarLeft + 89, o.Y + BarTop + BarHeight - 1));
                    // 残りは白地
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(o.X + BarLeft + 90, y));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(o.X + BarLeft + BarWidth - 1, y));
                    // 枠の外は灰色
                    Assert.AreEqual(FormGray, bitmap.GetPixel(o.X + BarLeft - 3, y));
                }
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "nowloading-01-progress");

                // 総数に達したら、バーは右端まで
                runner.GUI.DisplayLoadingProgress();
                runner.GUI.DisplayLoadingProgress();
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    var o = ClientOrigin();
                    Assert.AreEqual(BarColor, bitmap.GetPixel(o.X + BarLeft + BarWidth - 1, o.Y + BarTop + BarHeight / 2));
                }

                runner.GUI.CloseNowLoadingForm();
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    var o = ClientOrigin();
                    Assert.AreNotEqual(FormGray, bitmap.GetPixel(o.X + 4, o.Y + 4));
                }
            }
        }

        [TestMethod]
        public void OpenNowLoadingForm_BeforeProgress_ShowsEmptyBar()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.GUI.OpenNowLoadingForm();
                runner.GUI.SetLoadImageSize(10);
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    var o = ClientOrigin();
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(o.X + BarLeft, o.Y + BarTop + BarHeight / 2));
                }
            }
        }

        [TestMethod]
        public void DisplayLoadingProgress_WithoutForm_DoesNothing()
        {
            // SRC# は frmNowLoading を表示していないときも DisplayLoadingProgress を呼ぶ（SRC.IncludeData）。表示していなければ何もしない
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.GUI.DisplayLoadingProgress();
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    var o = ClientOrigin();
                    Assert.AreNotEqual(FormGray, bitmap.GetPixel(o.X + 4, o.Y + 4));
                }
            }
        }
    }
}
