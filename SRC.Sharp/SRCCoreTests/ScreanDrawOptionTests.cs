using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SRCCore.Tests
{
    /// <summary>
    /// ScreanDrawOption / ScreanDrawMode / GUIScreanExtension の追加テスト
    /// 基本テストは ScreanDrawTests にて実施済み
    /// </summary>
    [TestClass]
    public class ScreanDrawOptionTests
    {
        // ──────────────────────────────────────────────
        // ScreanDrawOption デフォルトコンストラクタ: 色の初期値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultConstructor_ForeColor_IsDefaultColor()
        {
            var opt = new ScreanDrawOption();
            Assert.AreEqual(default(Color), opt.ForeColor);
        }

        [TestMethod]
        public void DefaultConstructor_FillColor_IsDefaultColor()
        {
            var opt = new ScreanDrawOption();
            Assert.AreEqual(default(Color), opt.FillColor);
        }

        // ──────────────────────────────────────────────
        // ScreanDrawOption プロパティ個別設定
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DrawOption_CanBeSetToPreserve()
        {
            var opt = new ScreanDrawOption { DrawOption = ScreanDrawMode.Preserve };
            Assert.AreEqual(ScreanDrawMode.Preserve, opt.DrawOption);
        }

        [TestMethod]
        public void DrawWidth_CanBeSetToZero()
        {
            var opt = new ScreanDrawOption { DrawWidth = 0 };
            Assert.AreEqual(0, opt.DrawWidth);
        }

        [TestMethod]
        public void FillStyle_CanBeSetToVbFSSolid()
        {
            var opt = new ScreanDrawOption { FillStyle = FillStyle.VbFSSolid };
            Assert.AreEqual(FillStyle.VbFSSolid, opt.FillStyle);
        }

        // ──────────────────────────────────────────────
        // ScreanDrawMode Parse / Uniqueness
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ScreanDrawMode_Parse_AllMembers()
        {
            Assert.AreEqual(ScreanDrawMode.Front, Enum.Parse<ScreanDrawMode>("Front"));
            Assert.AreEqual(ScreanDrawMode.Preserve, Enum.Parse<ScreanDrawMode>("Preserve"));
            Assert.AreEqual(ScreanDrawMode.Background, Enum.Parse<ScreanDrawMode>("Background"));
        }

        [TestMethod]
        public void ScreanDrawMode_AllValuesAreUnique()
        {
            var values = Enum.GetValues(typeof(ScreanDrawMode)).Cast<ScreanDrawMode>().Select(v => (int)v);
            var set = new HashSet<int>(values);
            Assert.AreEqual(Enum.GetValues(typeof(ScreanDrawMode)).Length, set.Count);
        }

        // ──────────────────────────────────────────────
        // GUIScreanExtension 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ScreanDrawModeFrom_UnknownString_ReturnsFront()
        {
            Assert.AreEqual(ScreanDrawMode.Front, GUIScreanExtension.ScreanDrawModeFrom("不明"));
        }

        [TestMethod]
        public void ScreanDrawModeFrom_WhiteSpace_ReturnsFront()
        {
            Assert.AreEqual(ScreanDrawMode.Front, GUIScreanExtension.ScreanDrawModeFrom(" "));
        }
    }
}
