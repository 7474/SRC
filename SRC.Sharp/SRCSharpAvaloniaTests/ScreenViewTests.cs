using Avalonia.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCSharpAvalonia.Tests
{
    [TestClass]
    public class ScreenViewTests
    {
        [TestMethod]
        [DataRow(1.0)]
        [DataRow(1.25)]
        [DataRow(1.5)]
        [DataRow(2.0)]
        public void InterpolationModeFor_Enlarging_IsNearestNeighbor(double scaling)
        {
            Assert.AreEqual(BitmapInterpolationMode.None, ScreenView.InterpolationModeFor(scaling));
        }

        [TestMethod]
        public void InterpolationModeFor_Shrinking_Interpolates()
        {
            Assert.AreEqual(BitmapInterpolationMode.HighQuality, ScreenView.InterpolationModeFor(0.75));
        }
    }
}
