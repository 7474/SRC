using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// VarDataExtension クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class VarDataExtensionTests
    {
        private List<VarData> CreateVars()
        {
            return new List<VarData>
            {
                new VarData("score", ValueType.NumericType, "100", 100d),
                new VarData("score[1]", ValueType.NumericType, "10", 10d),
                new VarData("score[2]", ValueType.NumericType, "20", 20d),
                new VarData("score[abc]", ValueType.StringType, "val", 0d),
                new VarData("name", ValueType.StringType, "hero", 0d),
                new VarData("name[a]", ValueType.StringType, "alice", 0d),
            };
        }

        // ──────────────────────────────────────────────
        // ArrayByName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayByName_ReturnsElementsWithMatchingPrefix()
        {
            var vars = CreateVars();
            var result = vars.ArrayByName("score").ToList();
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(v => v.Name == "score[1]"));
            Assert.IsTrue(result.Any(v => v.Name == "score[2]"));
            Assert.IsTrue(result.Any(v => v.Name == "score[abc]"));
        }

        [TestMethod]
        public void ArrayByName_NoMatch_ReturnsEmpty()
        {
            var vars = CreateVars();
            var result = vars.ArrayByName("unknown").ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ArrayByName_DoesNotIncludeNonArrayVariable()
        {
            var vars = CreateVars();
            var result = vars.ArrayByName("score").ToList();
            // "score" (without bracket) should not be included
            Assert.IsFalse(result.Any(v => v.Name == "score"));
        }

        [TestMethod]
        public void ArrayByName_SingleElement()
        {
            var vars = CreateVars();
            var result = vars.ArrayByName("name").ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("name[a]", result[0].Name);
        }

        // ──────────────────────────────────────────────
        // ArrayIndexesByName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayIndexesByName_ReturnsIndexStrings()
        {
            var vars = CreateVars();
            var indexes = vars.ArrayIndexesByName("score").ToList();
            Assert.AreEqual(3, indexes.Count);
            Assert.IsTrue(indexes.Contains("1"));
            Assert.IsTrue(indexes.Contains("2"));
            Assert.IsTrue(indexes.Contains("abc"));
        }

        [TestMethod]
        public void ArrayIndexesByName_NoMatch_ReturnsEmpty()
        {
            var vars = CreateVars();
            var indexes = vars.ArrayIndexesByName("unknown").ToList();
            Assert.AreEqual(0, indexes.Count);
        }

        [TestMethod]
        public void ArrayIndexesByName_SingleElement_ReturnsIndex()
        {
            var vars = CreateVars();
            var indexes = vars.ArrayIndexesByName("name").ToList();
            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual("a", indexes[0]);
        }

        [TestMethod]
        public void ArrayIndexesByName_EmptyList_ReturnsEmpty()
        {
            var vars = new List<VarData>();
            var indexes = vars.ArrayIndexesByName("score").ToList();
            Assert.AreEqual(0, indexes.Count);
        }
    }
}
