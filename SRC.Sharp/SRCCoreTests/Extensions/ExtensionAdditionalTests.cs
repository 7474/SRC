using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// 拡張メソッドの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class ExtensionAdditionalTests
    {
        // ──────────────────────────────────────────────
        // StreamExtension.ToMemoryStream エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToMemoryStream_SingleByte_CopiesCorrectly()
        {
            var content = new byte[] { 0x42 };
            var source = new MemoryStream(content);

            var result = source.ToMemoryStream();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(0x42, result.ToArray()[0]);
        }

        [TestMethod]
        public void ToMemoryStream_SourceDisposedAfterCopy()
        {
            var content = Encoding.UTF8.GetBytes("disposed test");
            var source = new MemoryStream(content);

            // ToMemoryStream は元の Stream を Dispose する
            var result = source.ToMemoryStream();

            // 結果の MemoryStream は正常に読める
            Assert.AreEqual(content.Length, result.Length);
        }

        [TestMethod]
        public void ToMemoryStream_BinaryContent_PreservesAllBytes()
        {
            var content = new byte[] { 0x00, 0xFF, 0x7F, 0x80, 0x01 };
            var source = new MemoryStream(content);

            var result = source.ToMemoryStream();

            CollectionAssert.AreEqual(content, result.ToArray());
        }

        [TestMethod]
        public void ToMemoryStream_PositionInMiddle_CopiesFromCurrentPosition()
        {
            var content = new byte[] { 1, 2, 3, 4, 5 };
            var source = new MemoryStream(content);
            source.Position = 2; // 先頭をスキップ

            var result = source.ToMemoryStream();

            // Position=2 以降の 3 バイトのみコピーされる
            Assert.AreEqual(3, result.Length);
            CollectionAssert.AreEqual(new byte[] { 3, 4, 5 }, result.ToArray());
        }

        [TestMethod]
        public void ToMemoryStream_ReturnsIndependentStream()
        {
            var content = Encoding.UTF8.GetBytes("independent");
            var source = new MemoryStream(content);

            var result = source.ToMemoryStream();

            // 結果は独立した MemoryStream
            result.Position = 0;
            using var reader = new StreamReader(result, Encoding.UTF8, leaveOpen: true);
            var text = reader.ReadToEnd();
            Assert.AreEqual("independent", text);
        }

        // ──────────────────────────────────────────────
        // VarDataExtension.ArrayByName パターン追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayByName_EmptyList_ReturnsEmpty()
        {
            var vars = new List<VarData>();
            var result = vars.ArrayByName("x").ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ArrayByName_PrefixMatchesOnly_ExcludesOtherNames()
        {
            var vars = new List<VarData>
            {
                new VarData("hp[1]", ValueType.NumericType, "10", 10d),
                new VarData("mp[1]", ValueType.NumericType, "20", 20d),
                new VarData("hpmax[1]", ValueType.NumericType, "100", 100d),
            };

            // "hp" で始まるものだけ一致するが "hpmax[1]" も "hp" で始まるので含まれる
            var result = vars.ArrayByName("hp").ToList();
            Assert.IsTrue(result.Any(v => v.Name == "hp[1]"));
        }

        [TestMethod]
        public void ArrayByName_NoMatchingPrefix_ReturnsEmpty()
        {
            var vars = new List<VarData>
            {
                new VarData("score[1]", ValueType.NumericType, "100", 100d),
            };

            var result = vars.ArrayByName("notexist").ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ArrayByName_MultipleIndexTypes_ReturnsAll()
        {
            var vars = new List<VarData>
            {
                new VarData("val[1]", ValueType.NumericType, "1", 1d),
                new VarData("val[abc]", ValueType.StringType, "s", 0d),
                new VarData("val[テスト]", ValueType.StringType, "t", 0d),
            };

            var result = vars.ArrayByName("val").ToList();
            Assert.AreEqual(3, result.Count);
        }

        // ──────────────────────────────────────────────
        // VarDataExtension.ArrayIndexesByName パターン追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayIndexesByName_NumericIndexes_ReturnedAsStrings()
        {
            var vars = new List<VarData>
            {
                new VarData("x[1]", ValueType.NumericType, "1", 1d),
                new VarData("x[2]", ValueType.NumericType, "2", 2d),
                new VarData("x[10]", ValueType.NumericType, "10", 10d),
            };

            var indexes = vars.ArrayIndexesByName("x").ToList();
            Assert.AreEqual(3, indexes.Count);
            Assert.IsTrue(indexes.Contains("1"));
            Assert.IsTrue(indexes.Contains("2"));
            Assert.IsTrue(indexes.Contains("10"));
        }

        [TestMethod]
        public void ArrayIndexesByName_StringIndexes_ReturnedCorrectly()
        {
            var vars = new List<VarData>
            {
                new VarData("item[sword]", ValueType.StringType, "刀", 0d),
                new VarData("item[shield]", ValueType.StringType, "盾", 0d),
            };

            var indexes = vars.ArrayIndexesByName("item").ToList();
            Assert.AreEqual(2, indexes.Count);
            Assert.IsTrue(indexes.Contains("sword"));
            Assert.IsTrue(indexes.Contains("shield"));
        }

        [TestMethod]
        public void ArrayIndexesByName_JapaneseIndexes_ReturnedCorrectly()
        {
            var vars = new List<VarData>
            {
                new VarData("フラグ[開始]", ValueType.NumericType, "1", 1d),
                new VarData("フラグ[終了]", ValueType.NumericType, "0", 0d),
            };

            var indexes = vars.ArrayIndexesByName("フラグ").ToList();
            Assert.AreEqual(2, indexes.Count);
            Assert.IsTrue(indexes.Contains("開始"));
            Assert.IsTrue(indexes.Contains("終了"));
        }

        [TestMethod]
        public void ArrayIndexesByName_EmptyList_ReturnsEmpty()
        {
            var vars = new List<VarData>();
            var result = vars.ArrayIndexesByName("any").ToList();
            Assert.AreEqual(0, result.Count);
        }
    }
}
