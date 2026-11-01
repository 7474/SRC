using Avalonia.Headless;
using Avalonia.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace SRCSharpAvalonia.Tests
{
    [TestClass]
    public class ScreenViewTests
    {
        [TestMethod]
        [DataRow(0.5)]
        [DataRow(0.75)]
        [DataRow(1.0)]
        [DataRow(1.25)]
        [DataRow(2.0)]
        public void InterpolationModeFor_Enlarging_IsNearestNeighbor(double scaling)
        {
            Assert.AreEqual(BitmapInterpolationMode.None, ScreenView.InterpolationModeFor(scaling, 2));
        }

        [TestMethod]
        public void InterpolationModeFor_Shrinking_Interpolates()
        {
            Assert.AreEqual(BitmapInterpolationMode.HighQuality, ScreenView.InterpolationModeFor(0.49, 2));
            Assert.AreEqual(BitmapInterpolationMode.HighQuality, ScreenView.InterpolationModeFor(0.75, 1));
            Assert.AreEqual(BitmapInterpolationMode.None, ScreenView.InterpolationModeFor(1.0, 1));
        }

        [TestMethod]
        [DataRow(1920.0, 1000.0, 2.0)]
        [DataRow(1536.0, 780.0, 1.5)]
        [DataRow(1470.0, 850.0, 1.5)]
        [DataRow(1366.0, 700.0, 1.0)]
        [DataRow(4000.0, 3000.0, 2.0)]
        [DataRow(600.0, 400.0, 1.0)]
        public void FitScale_ChoosesLargestScaleThatFits(double width, double height, double expected)
        {
            Assert.AreEqual(expected, ScreenView.FitScale(width, height));
        }

        [TestMethod]
        public void DisplayScale_ChangesDisplaySize()
        {
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var view = new ScreenView();
                    Assert.AreEqual(720, view.Width);
                    Assert.AreEqual(496, view.Height);
                    view.DisplayScale = 1.5;
                    Assert.AreEqual(1080, view.Width);
                    Assert.AreEqual(744, view.Height);
                    Assert.ThrowsExactly<System.ArgumentOutOfRangeException>(() => view.DisplayScale = 3);
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }
    }
}
