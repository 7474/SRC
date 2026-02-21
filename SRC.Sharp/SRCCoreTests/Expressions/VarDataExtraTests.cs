using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// VarData と関連する拡張メソッドのエッジケーステスト
    /// （既存 3 ファイル未カバー分）
    /// </summary>
    [TestClass]
    public class VarDataExtraTests
    {
        // ──────────────────────────────────────────────
        // SetValue: 空名前・UndefinedType 変数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetValue_EmptyName_NameIsEmpty()
        {
            var v = new VarData();
            v.SetValue("", ValueType.NumericType, "", 7d);
            Assert.AreEqual("", v.Name);
            Assert.AreEqual(7d, v.NumericValue);
        }

        [TestMethod]
        public void SetValue_UndefinedType_BothValuesStoredAsIs()
        {
            var v = new VarData();
            v.SetValue("u", ValueType.UndefinedType, "rawStr", 3.14d);
            // UndefinedType は変換を行わずそのまま保持される
            Assert.AreEqual(ValueType.UndefinedType, v.VariableType);
            Assert.AreEqual("rawStr", v.StringValue);
            Assert.AreEqual(3.14d, v.NumericValue);
        }

        [TestMethod]
        public void SetValue_NumericType_ZeroValue_StringValueIsZero()
        {
            var v = new VarData();
            v.SetValue("n", ValueType.NumericType, "", 0d);
            Assert.AreEqual(0d, v.NumericValue);
            Assert.AreEqual("0", v.StringValue);
        }

        // ──────────────────────────────────────────────
        // Clear vs Init の動作差
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_SetsNameToEmpty_InitSetsNameToParameter()
        {
            var v1 = new VarData("before", ValueType.NumericType, "", 5d);
            v1.Clear();
            // Clear はデフォルトの Init("") を呼ぶので Name = ""
            Assert.AreEqual("", v1.Name);

            var v2 = new VarData("before", ValueType.NumericType, "", 5d);
            v2.Init("after");
            // Init("after") は Name = "after" にする
            Assert.AreEqual("after", v2.Name);
        }

        [TestMethod]
        public void Clear_VariableTypeBecomesStringType()
        {
            var v = new VarData("x", ValueType.NumericType, "", 99d);
            v.Clear();
            Assert.AreEqual(ValueType.StringType, v.VariableType);
        }

        [TestMethod]
        public void Init_VariableTypeBecomesStringType()
        {
            var v = new VarData("x", ValueType.NumericType, "", 99d);
            v.Init("y");
            Assert.AreEqual(ValueType.StringType, v.VariableType);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        // ──────────────────────────────────────────────
        // ReferenceValue: 未カバーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReferenceValue_StringRequest_UndefinedTypeVar_ReturnsStringValue()
        {
            // VariableType が UndefinedType の場合、StringType リクエストは
            // VariableType != NumericType → str_result = StringValue を返す
            var v = new VarData("u", ValueType.UndefinedType, "rawStr", 99d);
            var type = v.ReferenceValue(ValueType.StringType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, type);
            Assert.AreEqual("rawStr", str);
        }

        [TestMethod]
        public void ReferenceValue_NumericRequest_UndefinedTypeVar_ParsesStringValue()
        {
            var v = new VarData("u", ValueType.UndefinedType, "55", 0d);
            var type = v.ReferenceValue(ValueType.NumericType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            // NumericType でないので StringValue を Conversions.ToDouble で変換
            Assert.AreEqual(55d, num);
        }

        [TestMethod]
        public void ReferenceValue_Undefined_UndefinedTypeVar_ReturnsNumericType()
        {
            // VariableType が UndefinedType の場合、UndefinedType リクエストはデフォルトケース
            // VariableType == StringType でないため NumericType を返す
            var v = new VarData("u", ValueType.UndefinedType, "", 10d);
            var type = v.ReferenceValue(ValueType.UndefinedType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            Assert.AreEqual(10d, num);
        }

        [TestMethod]
        public void ReferenceValue_NumericRequest_EmptyStringVar_ReturnsZero()
        {
            var v = new VarData("s", ValueType.StringType, "", 0d);
            var type = v.ReferenceValue(ValueType.NumericType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            Assert.AreEqual(0d, num);
        }

        // ──────────────────────────────────────────────
        // SetFrom: 各種バリエーション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetFrom_UndefinedTypeSource_CopiesCorrectly()
        {
            var src = new VarData("raw", ValueType.UndefinedType, "rawStr", 3.14d);
            var dst = new VarData();
            dst.SetFrom(src);
            Assert.AreEqual("raw", dst.Name);
            Assert.AreEqual(ValueType.UndefinedType, dst.VariableType);
            Assert.AreEqual("rawStr", dst.StringValue);
            Assert.AreEqual(3.14d, dst.NumericValue);
        }

        [TestMethod]
        public void SetFrom_EmptyVarData_AllFieldsEmpty()
        {
            var src = new VarData(); // Clear() で初期化
            var dst = new VarData("existing", ValueType.NumericType, "", 100d);
            dst.SetFrom(src);
            Assert.AreEqual("", dst.Name);
            Assert.AreEqual(ValueType.StringType, dst.VariableType);
            Assert.AreEqual("", dst.StringValue);
            Assert.AreEqual(0d, dst.NumericValue);
        }

        [TestMethod]
        public void SetFrom_StringTypeWithJapanese_CopiesCorrectly()
        {
            var src = new VarData("jp", ValueType.StringType, "テスト文字列", 0d);
            var dst = new VarData();
            dst.SetFrom(src);
            Assert.AreEqual("jp", dst.Name);
            Assert.AreEqual("テスト文字列", dst.StringValue);
            Assert.AreEqual(ValueType.StringType, dst.VariableType);
        }

        // ──────────────────────────────────────────────
        // ArrayByName 拡張メソッド（追加パターン）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayByName_PrefixNotExact_DoesNotMatchNonArrayVar()
        {
            // "score" は "sco" + "[" で始まらないのでマッチしない
            var vars = new List<VarData>
            {
                new VarData("score", ValueType.NumericType, "100", 100d),
                new VarData("score[1]", ValueType.NumericType, "10", 10d),
            };
            var result = vars.ArrayByName("sco").ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ArrayByName_MatchesAllBracketVariants()
        {
            var vars = new List<VarData>
            {
                new VarData("hp[1]", ValueType.NumericType, "10", 10d),
                new VarData("hp[2]", ValueType.NumericType, "20", 20d),
                new VarData("hp[abc]", ValueType.StringType, "val", 0d),
                new VarData("mp[1]", ValueType.NumericType, "5", 5d),
            };
            var result = vars.ArrayByName("hp").ToList();
            Assert.AreEqual(3, result.Count);
            Assert.IsFalse(result.Any(v => v.Name == "mp[1]"));
        }

        [TestMethod]
        public void ArrayByName_EmptyCollection_ReturnsEmpty()
        {
            var vars = new List<VarData>();
            var result = vars.ArrayByName("x").ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ArrayByName_PartialNamePrefix_DoesNotMatch()
        {
            // "score[1]" は "scor" ではなく "score" で始まる
            var vars = new List<VarData>
            {
                new VarData("score[1]", ValueType.NumericType, "10", 10d),
            };
            var result = vars.ArrayByName("scor").ToList();
            Assert.AreEqual(0, result.Count);
        }

        // ──────────────────────────────────────────────
        // ArrayIndexesByName 拡張メソッド（追加パターン）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayIndexesByName_FiltersOutNonBracketVars()
        {
            // "score" 自体はインデックスを持たないので ArrayIndexesByName の結果に含まれない
            var vars = new List<VarData>
            {
                new VarData("score", ValueType.NumericType, "0", 0d),
                new VarData("score[1]", ValueType.NumericType, "10", 10d),
                new VarData("score[2]", ValueType.NumericType, "20", 20d),
            };
            var indexes = vars.ArrayIndexesByName("score").ToList();
            Assert.AreEqual(2, indexes.Count);
            Assert.IsTrue(indexes.Contains("1"));
            Assert.IsTrue(indexes.Contains("2"));
        }

        [TestMethod]
        public void ArrayIndexesByName_MultipleVars_ReturnsAllIndexes()
        {
            var vars = new List<VarData>
            {
                new VarData("x[a]", ValueType.StringType, "va", 0d),
                new VarData("x[b]", ValueType.StringType, "vb", 0d),
                new VarData("x[c]", ValueType.StringType, "vc", 0d),
            };
            var indexes = vars.ArrayIndexesByName("x").ToList();
            Assert.AreEqual(3, indexes.Count);
            CollectionAssert.Contains(indexes, "a");
            CollectionAssert.Contains(indexes, "b");
            CollectionAssert.Contains(indexes, "c");
        }

        [TestMethod]
        public void ArrayIndexesByName_NoMatchingVars_ReturnsEmpty()
        {
            var vars = new List<VarData>
            {
                new VarData("foo[1]", ValueType.NumericType, "1", 1d),
            };
            var indexes = vars.ArrayIndexesByName("bar").ToList();
            Assert.AreEqual(0, indexes.Count);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_NumericType_ContainsNumericValue()
        {
            var v = new VarData("val", ValueType.NumericType, "", 42d);
            var str = v.ToString();
            Assert.IsTrue(str.Contains("42"), $"Expected '42' in '{str}'");
        }

        [TestMethod]
        public void ToString_UndefinedType_DoesNotThrow()
        {
            var v = new VarData("u", ValueType.UndefinedType, "raw", 9d);
            var str = v.ToString();
            Assert.IsNotNull(str);
            Assert.IsTrue(str.Contains("u"));
        }
    }
}
