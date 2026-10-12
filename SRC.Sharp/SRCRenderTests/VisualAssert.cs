using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SRCRender.Tests
{
    /// <summary>
    /// ビジュアルリグレッションテスト（VRT）。
    /// 画面の PNG を Baselines フォルダの基準画像と比較する。
    /// 環境変数 SRC_VRT_UPDATE=1 で実行すると基準画像を更新する。
    /// 差分があった場合は TestResults/vrt に actual / diff を書き出す。
    /// 基準画像は Ubuntu 24.04 + fonts-noto-cjk（Noto Sans CJK JP）で作成している。
    /// </summary>
    public static class VisualAssert
    {
        // チャンネルごとの許容差（アンチエイリアスの揺れを吸収する）
        private const int ChannelTolerance = 24;
        // 許容する不一致ピクセルの割合
        private const double MaxMismatchRatio = 0.002;

        /// <summary>
        /// 基準画像と同じフォントで描画できる環境か。
        /// フォントが異なると文字の描画が変わるため、それ以外の環境では比較しない。
        /// </summary>
        public static bool IsBaselineEnvironment
        {
            get
            {
                using (var typeface = SKTypeface.FromFamilyName(ScreenRenderer.PreferredFontFamily))
                {
                    return typeface?.FamilyName == ScreenRenderer.PreferredFontFamily;
                }
            }
        }

        /// <summary>基準画像を更新するモードか（環境変数 SRC_VRT_UPDATE=1）</summary>
        public static bool IsUpdateMode => Environment.GetEnvironmentVariable("SRC_VRT_UPDATE") == "1";

        public static void MatchesBaseline(byte[] actualPng, string name, [CallerFilePath] string callerPath = "")
        {
            var baselineDir = Path.Combine(Path.GetDirectoryName(callerPath), "Baselines");
            var baselinePath = Path.Combine(baselineDir, name + ".png");
            var outputDir = Path.Combine(AppContext.BaseDirectory, "TestResults", "vrt");
            Directory.CreateDirectory(outputDir);
            File.WriteAllBytes(Path.Combine(outputDir, name + ".actual.png"), actualPng);

            if (!IsBaselineEnvironment)
            {
                Assert.Inconclusive($"{ScreenRenderer.PreferredFontFamily} が無い環境のため VRT の比較を行いません（{name}）");
            }
            if (IsUpdateMode)
            {
                Directory.CreateDirectory(baselineDir);
                File.WriteAllBytes(baselinePath, actualPng);
                return;
            }
            if (!File.Exists(baselinePath))
            {
                Assert.Fail($"基準画像がありません: {baselinePath}。SRC_VRT_UPDATE=1 で実行すると作成します。");
            }

            using (var expected = SKBitmap.Decode(baselinePath))
            using (var actual = SKBitmap.Decode(actualPng))
            {
                Assert.AreEqual(expected.Width, actual.Width, "width");
                Assert.AreEqual(expected.Height, actual.Height, "height");
                using (var diff = new SKBitmap(actual.Width, actual.Height))
                {
                    var mismatch = 0;
                    for (var y = 0; y < actual.Height; y++)
                    {
                        for (var x = 0; x < actual.Width; x++)
                        {
                            var e = expected.GetPixel(x, y);
                            var a = actual.GetPixel(x, y);
                            var same = Math.Abs(e.Red - a.Red) <= ChannelTolerance
                                && Math.Abs(e.Green - a.Green) <= ChannelTolerance
                                && Math.Abs(e.Blue - a.Blue) <= ChannelTolerance
                                && Math.Abs(e.Alpha - a.Alpha) <= ChannelTolerance;
                            if (!same)
                            {
                                mismatch++;
                            }
                            diff.SetPixel(x, y, same ? new SKColor(a.Red, a.Green, a.Blue, 0x40) : SKColors.Magenta);
                        }
                    }
                    var ratio = (double)mismatch / (actual.Width * actual.Height);
                    if (ratio > MaxMismatchRatio)
                    {
                        using (var image = SKImage.FromBitmap(diff))
                        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                        using (var stream = File.Create(Path.Combine(outputDir, name + ".diff.png")))
                        {
                            data.SaveTo(stream);
                        }
                        Assert.Fail($"{name}: 基準画像と {ratio:P2}（{mismatch} px）異なります。差分は {outputDir} を参照");
                    }
                }
            }
        }
    }
}
