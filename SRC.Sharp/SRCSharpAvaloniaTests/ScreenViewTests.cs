using Avalonia.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.AreEqual(BitmapInterpolationMode.None, ScreenView.InterpolationModeFor(scaling));
        }

        [TestMethod]
        public void InterpolationModeFor_Shrinking_Interpolates()
        {
            Assert.AreEqual(BitmapInterpolationMode.HighQuality, ScreenView.InterpolationModeFor(0.49));
        }

        [TestMethod]
        public void Constructor_ScalesDisplaySizeTo200Percent()
        {
            var view = new ScreenView();

            Assert.AreEqual(1440, view.Width);
            Assert.AreEqual(992, view.Height);
        }
    }
}
