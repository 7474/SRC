using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// PilotDataList.Load を元の SRC（PilotDataList.cls Load）の動作と突き合わせるテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class PilotDataListLoadVBMatchTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static void Load(SRC src, string text)
        {
            src.PDList.Load("pilot.txt", new MemoryStream(Encoding.UTF8.GetBytes(text)));
        }

        private static string[] Errors(SRC src) => src.DataErrors.Select(x => x.msg).ToArray();

        private static string PilotWith(string name = "テスト", string feature = "特殊能力なし", string rest = "")
        {
            return name + "\n"
                + "テスト,てすと,男性,汎用,AAAA,100\n"
                + feature + "\n"
                + "100,100,100,100,100,100,普通\n"
                + "ＳＰなし\n"
                + "test.bmp,-\n"
                + rest;
        }

        [TestMethod]
        public void NewFormat_LevelOnly_RoundsToInteger()
        {
            // 新形式の「Lvn」だけの指定は CInt で整数に丸める（偶数丸め）
            var src = CreateSrc();
            Load(src, PilotWith(feature: "特殊能力\n底力Lv1,1,Lv2.5,10"));
            var pd = src.PDList.Item("テスト");

            Assert.AreEqual(1d, pd.SkillLevel(1, "底力"));
            Assert.AreEqual(2d, pd.SkillLevel(10, "底力"));
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void NewFormat_LevelOnlyNotNumeric_ReportsAndStopsLoading()
        {
            // 「Lvx」は「レベル指定が不正です。」を出したあと、CInt が実行時エラーになって読み込みを打ち切る
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, PilotWith(feature: "特殊能力\n底力Lv1,1,Lvx,10")));

            CollectionAssert.AreEqual(new[] { "特殊能力「底力」のレベル指定が不正です。" }, Errors(src));
            Assert.AreEqual("", ex.InvalidDataList.Single().msg);
            Assert.AreEqual("テスト", ex.InvalidDataList.Single().dname);
        }

        [TestMethod]
        public void NewFormat_NameWithNonNumericLevel_StopsLoading()
        {
            // 「底力Lv」のようにレベルが数値でなければ CDbl が実行時エラーになって読み込みを打ち切る
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, PilotWith(feature: "特殊能力\n底力Lv,1")));

            Assert.AreEqual("", ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void OldFormat_NameWithNonNumericLevel_StopsLoading()
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, PilotWith(feature: "特殊能力,底力Lvx,1")));

            Assert.AreEqual("", ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void DuplicateNameDifferentOnlyInCase_StopsLoading()
        {
            // 元の SRC は IsDefined（大文字・小文字を区別しない Collection）で見つけたデータの名前が違うと
            // Add を呼び、同じキーの追加で実行時エラーになる
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, PilotWith(name: "abc") + "\n" + PilotWith(name: "ABC")));

            Assert.AreEqual("", ex.InvalidDataList.Single().msg);
            Assert.AreEqual("ABC", ex.InvalidDataList.Single().dname);
        }

        [TestMethod]
        public void DuplicateSameName_ReplacesData()
        {
            var src = CreateSrc();
            Load(src, PilotWith(feature: "特殊能力\n底力,1") + "\n" + PilotWith());

            var pd = src.PDList.Item("テスト");
            Assert.AreEqual(0, pd.Skills.Count);
        }

        [TestMethod]
        public void FeatureSection_BlankLineAfterSeparator_ReportsAndReadsNextLine()
        {
            // 「===」の直後の行は空行でも特殊能力として読み、「行頭から1番目の特殊能力の設定が間違っています。」を出す。
            // 続く行が空行でなければ特殊能力として読み続ける
            var src = CreateSrc();
            Load(src, PilotWith(rest: "===\n\nバリア=バリア 1000\n\n"));
            var pd = src.PDList.Item("テスト");

            CollectionAssert.AreEqual(new[] { "行頭から1番目の特殊能力の設定が間違っています。" }, Errors(src));
            Assert.IsTrue(pd.IsFeatureAvailable("バリア"));
        }

        [TestMethod]
        public void FeatureSection_Normal_NoError()
        {
            var src = CreateSrc();
            Load(src, PilotWith(rest: "===\nバリア=バリア 1000\n\n"));

            Assert.AreEqual(0, src.DataErrors.Count);
            Assert.IsTrue(src.PDList.Item("テスト").IsFeatureAvailable("バリア"));
        }

        [TestMethod]
        public void AbilitySection_NoComma_UsesPilotMessage()
        {
            // パイロットデータのアビリティは「終りには」（ユニットデータは「終りに」）
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, PilotWith(rest: "===\n===\n===\n回復\n")));

            Assert.AreEqual("アビリティデータの終りには空行を置いてください。", ex.InvalidDataList.Single().msg);
        }
    }
}
