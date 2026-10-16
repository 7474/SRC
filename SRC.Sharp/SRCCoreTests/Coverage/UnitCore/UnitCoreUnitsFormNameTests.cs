using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Coverage.Tests.UnitCore
{
    /// <summary>
    /// Units.Add で、1番目の要素が能力名（形態名ではない）特殊能力の読み取りのテスト。
    /// VB Units.cls の Add は 変形（For i = 2）、ハイパーモード・パーツ分離・変形技（LIndex(.., 2)）、
    /// 合体（LIndex(.., 2)）、分離（For j = 2）で2番目の要素から形態名として読む。
    /// （換装・他形態は1番目の要素から読む）
    /// </summary>
    [TestClass]
    public class UnitCoreUnitsFormNameTests
    {
        private static string UnitBlock(string name, int pilotNum = 1, params string[] features)
        {
            var feat = features.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", features);
            return $@"
{name}
{name}, てすと, 汎用, {pilotNum}, 2
陸, 5, M, 1000, 100
{feat}
3000, 100, 800, 90
AA-A, test.bmp
テストソード, 1500, 1, 1, +15, -, -, -, AAAA, +10, 武
";
        }

        private static SRC CreateSrcWithUnits(params string[] blocks)
        {
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src, string.Join("\n", blocks));
            return src;
        }

        [TestMethod]
        [DataRow("変形=未定義名 変形先")]
        [DataRow("ハイパーモード=未定義名 変形先")]
        [DataRow("パーツ分離=未定義名 変形先")]
        [DataRow("変形技=未定義名 変形先")]
        public void Add_FirstElementIsAbilityName_NotTreatedAsForm(string feature)
        {
            // 1番目の要素（能力名）はユニットデータとして探さず、2番目の要素の形態を作る
            var src = CreateSrcWithUnits(
                UnitBlock("本体", features: feature),
                UnitBlock("変形先"));

            var u = src.UList.Add("本体", 0, "味方");

            Assert.IsNotNull(u);
            Assert.IsTrue(u.IsOtherFormDefined("変形先"));
            Assert.IsFalse(u.IsOtherFormDefined("未定義名"));
        }

        [TestMethod]
        public void Add_CombineAbilityNameMatchesExistingUnit_NotOtherForm()
        {
            // 合体=能力名 合体後 … の能力名と同名のユニットがいても他形態にはならない（2番目の合体後で判定）
            var src = CreateSrcWithUnits(
                UnitBlock("合体技"),
                UnitBlock("パーツＡ", features: "合体=合体技 合体後 パーツＢ"),
                UnitBlock("合体後", pilotNum: 2, features: "分離=分離 パーツＡ パーツＢ"),
                UnitBlock("パーツＢ", features: "合体=合体技 合体後 パーツＡ"));
            src.UList.Add("合体技", 0, "味方");

            var u = src.UList.Add("パーツＡ", 0, "味方");

            Assert.AreNotEqual("他形態", u.Status);
        }

        [TestMethod]
        public void Add_SplitAbilityNameMatchesExistingUnit_NotOtherForm()
        {
            // 分離=能力名 パーツ… の能力名と同名のユニットがいても他形態にはならない（2番目以降で判定）
            var src = CreateSrcWithUnits(
                UnitBlock("分離技"),
                UnitBlock("合体後", pilotNum: 2, features: "分離=分離技 パーツＡ パーツＢ"),
                UnitBlock("パーツＡ"),
                UnitBlock("パーツＢ"));
            src.UList.Add("分離技", 0, "味方");

            var u = src.UList.Add("合体後", 0, "味方");

            Assert.AreNotEqual("他形態", u.Status);
        }
    }
}
