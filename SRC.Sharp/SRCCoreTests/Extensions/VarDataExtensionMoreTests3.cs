using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// VarDataExtension の追加ユニットテスト（その3）
    /// </summary>
    [TestClass]
    public class VarDataExtensionMoreTests3
    {
        private List<VarData> CreateVars()
        {
            return new List<VarData>
            {
                new VarData("hp", ValueType.NumericType, "100", 100d),
                new VarData("hp[1]", ValueType.NumericType, "50", 50d),
                new VarData("hp[2]", ValueType.NumericType, "80", 80d),
                new VarData("hp[max]", ValueType.NumericType, "100", 100d),
                new VarData("mp", ValueType.NumericType, "30", 30d),
                new VarData("name", ValueType.StringType, "hero", 0d),
                new VarData("name[1]", ValueType.StringType, "alice", 0d),
                new VarData("name[2]", ValueType.StringType, "bob", 0d),
            };
        }

        // ──────────────────────────────────────────────
        // ArrayByName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayByName_MatchingPrefix_ReturnsAll()
        {
            var vars = CreateVars();
            var result = vars.ArrayByName("hp").ToList();
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void ArrayByName_DoesNotIncludeBaseVariable()
        {
            var vars = CreateVars();
            var result = vars.ArrayByName("hp").ToList();
            Assert.IsFalse(result.Any(v => v.Name == "hp"));
        }

        [TestMethod]
        public void ArrayByName_NoMatch_ReturnsEmpty()
        {
            var vars = CreateVars();
            var result = vars.ArrayByName("unknown").ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ArrayByName_StringTypeArray_ReturnsCorrectCount()
        {
            var vars = CreateVars();
            var result = vars.ArrayByName("name").ToList();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void ArrayByName_SingleElement_ReturnsOne()
        {
            var vars = new List<VarData>
            {
                new VarData("score", ValueType.NumericType, "0", 0d),
                new VarData("score[x]", ValueType.NumericType, "5", 5d),
            };
            var result = vars.ArrayByName("score").ToList();
            Assert.AreEqual(1, result.Count);
        }

        // ──────────────────────────────────────────────
        // ArrayIndexesByName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayIndexesByName_ReturnsNumericIndexes()
        {
            var vars = CreateVars();
            var indexes = vars.ArrayIndexesByName("hp").ToList();
            Assert.IsTrue(indexes.Contains("1"));
            Assert.IsTrue(indexes.Contains("2"));
            Assert.IsTrue(indexes.Contains("max"));
        }

        [TestMethod]
        public void ArrayIndexesByName_NoMatch_ReturnsEmpty()
        {
            var vars = CreateVars();
            var indexes = vars.ArrayIndexesByName("missing").ToList();
            Assert.AreEqual(0, indexes.Count);
        }

        [TestMethod]
        public void ArrayIndexesByName_CountMatchesArrayByName()
        {
            var vars = CreateVars();
            var byName = vars.ArrayByName("name").ToList();
            var byIndex = vars.ArrayIndexesByName("name").ToList();
            Assert.AreEqual(byName.Count, byIndex.Count);
        }

        [TestMethod]
        public void ArrayIndexesByName_EmptyList_ReturnsEmpty()
        {
            var vars = new List<VarData>();
            var indexes = vars.ArrayIndexesByName("hp").ToList();
            Assert.AreEqual(0, indexes.Count);
        }

        [TestMethod]
        public void ArrayIndexesByName_CorrectIndexValue_ForStringType()
        {
            var vars = CreateVars();
            var indexes = vars.ArrayIndexesByName("name").ToList();
            Assert.IsTrue(indexes.Contains("1"));
            Assert.IsTrue(indexes.Contains("2"));
        }

        [TestMethod]
        public void ArrayByName_PartialPrefixDoesNotMatch()
        {
            var vars = CreateVars();
            // "h" should not match "hp[1]" because prefix check uses "h["
            var result = vars.ArrayByName("h").ToList();
            Assert.AreEqual(0, result.Count);
        }
    }
}
