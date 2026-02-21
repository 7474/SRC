using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCCore.VB;
using System.Drawing;

namespace SRCCore.Tests
{
    /// <summary>
    /// ScreanDrawMode / ScreanDrawOption / GUIScreanExtension のユニットテスト
    /// </summary>
    [TestClass]
    public class ScreanDrawTests
    {
        // ──────────────────────────────────────────────
        // ScreanDrawMode enum
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ScreanDrawMode_Front_IsZero()
        {
            Assert.AreEqual(0, (int)ScreanDrawMode.Front);
        }

        [TestMethod]
        public void ScreanDrawMode_Preserve_IsOne()
        {
            Assert.AreEqual(1, (int)ScreanDrawMode.Preserve);
        }

        [TestMethod]
        public void ScreanDrawMode_Background_IsTwo()
        {
            Assert.AreEqual(2, (int)ScreanDrawMode.Background);
        }

        [TestMethod]
        public void ScreanDrawMode_HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(ScreanDrawMode)).Length);
        }

        [TestMethod]
        public void ScreanDrawMode_AllValuesDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ScreanDrawMode), ScreanDrawMode.Front));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ScreanDrawMode), ScreanDrawMode.Preserve));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ScreanDrawMode), ScreanDrawMode.Background));
        }

        // ──────────────────────────────────────────────
        // ScreanDrawOption コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ScreanDrawOption_DefaultConstructor_SetsDefaults()
        {
            var opt = new ScreanDrawOption();
            Assert.AreEqual(ScreanDrawMode.Front, opt.DrawOption);
            Assert.AreEqual(1, opt.DrawWidth);
            Assert.AreEqual(FillStyle.VbFSTransparent, opt.FillStyle);
        }

        [TestMethod]
        public void ScreanDrawOption_Properties_CanBeSetAndRead()
        {
            var opt = new ScreanDrawOption
            {
                DrawOption = ScreanDrawMode.Background,
                ForeColor = Color.Red,
                DrawWidth = 3,
                FillColor = Color.Blue,
                FillStyle = FillStyle.VbFSSolid
            };

            Assert.AreEqual(ScreanDrawMode.Background, opt.DrawOption);
            Assert.AreEqual(Color.Red, opt.ForeColor);
            Assert.AreEqual(3, opt.DrawWidth);
            Assert.AreEqual(Color.Blue, opt.FillColor);
            Assert.AreEqual(FillStyle.VbFSSolid, opt.FillStyle);
        }

        // ──────────────────────────────────────────────
        // GUIScreanExtension.ScreanDrawModeFrom
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ScreanDrawModeFrom_Background_ReturnsBackground()
        {
            Assert.AreEqual(ScreanDrawMode.Background, GUIScreanExtension.ScreanDrawModeFrom("背景"));
        }

        [TestMethod]
        public void ScreanDrawModeFrom_Preserve_ReturnsPreserve()
        {
            Assert.AreEqual(ScreanDrawMode.Preserve, GUIScreanExtension.ScreanDrawModeFrom("保持"));
        }

        [TestMethod]
        public void ScreanDrawModeFrom_Default_ReturnsFront()
        {
            Assert.AreEqual(ScreanDrawMode.Front, GUIScreanExtension.ScreanDrawModeFrom("前面"));
        }

        [TestMethod]
        public void ScreanDrawModeFrom_Empty_ReturnsFront()
        {
            Assert.AreEqual(ScreanDrawMode.Front, GUIScreanExtension.ScreanDrawModeFrom(""));
        }

        [TestMethod]
        public void ScreanDrawModeFrom_Null_ReturnsFront()
        {
            Assert.AreEqual(ScreanDrawMode.Front, GUIScreanExtension.ScreanDrawModeFrom(null));
        }
    }
}
