using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using System.IO;
using System.Text;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// MessageDataList.Load メソッドのユニットテスト
    /// </summary>
    [TestClass]
    public class MessageDataListLoadTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private Stream ToStream(string content)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        // ──────────────────────────────────────────────
        // 基本ロード
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Load_SingleEntry_AddsMessageData()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "アムロ\n攻撃,ビームライフルで撃つ！\n";
            mdl.Load("test.msg", false, ToStream(content));
            Assert.IsTrue(mdl.IsDefined("アムロ"));
        }

        [TestMethod]
        public void Load_SingleEntry_CountIsOne()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "シャア\n攻撃,ザクで攻める！\n";
            mdl.Load("test.msg", false, ToStream(content));
            Assert.AreEqual(1, mdl.Count());
        }

        [TestMethod]
        public void Load_MultipleEntries_CountIsCorrect()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "アムロ\n攻撃,行くぞ！\n\nシャア\n攻撃,見せてやろう！\n";
            mdl.Load("test.msg", false, ToStream(content));
            Assert.AreEqual(2, mdl.Count());
        }

        [TestMethod]
        public void Load_MessageIsParsedCorrectly()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "アムロ\n攻撃,ビームライフルで撃つ！\n";
            mdl.Load("test.msg", false, ToStream(content));
            var md = mdl.Item("アムロ");
            Assert.IsNotNull(md);
            Assert.AreEqual("ビームライフルで撃つ！", md.SelectMessage("攻撃"));
        }

        [TestMethod]
        public void Load_MultipleMessagesPerEntry_AllParsed()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "シャア\n攻撃,見せてやろう！\n撃墜,やられた...\n";
            mdl.Load("test.msg", false, ToStream(content));
            var md = mdl.Item("シャア");
            Assert.AreEqual("見せてやろう！", md.SelectMessage("攻撃"));
            Assert.AreEqual("やられた...", md.SelectMessage("撃墜"));
        }

        [TestMethod]
        public void Load_EmptyStream_CountIsZero()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            mdl.Load("empty.msg", false, ToStream(""));
            Assert.AreEqual(0, mdl.Count());
        }

        // ──────────────────────────────────────────────
        // 重複エントリの処理
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Load_DuplicateEntry_NotEffect_OverwritesPrevious()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            // isEffect=false の場合は後の定義を優先（前を削除して再追加）
            var content = "アムロ\n攻撃,初回！\n\nアムロ\n攻撃,二回目！\n";
            mdl.Load("test.msg", false, ToStream(content));
            var md = mdl.Item("アムロ");
            Assert.AreEqual("二回目！", md.SelectMessage("攻撃"));
        }

        [TestMethod]
        public void Load_DuplicateEntry_IsEffect_MergesMessages()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            // isEffect=true の場合は既存に追加
            var content = "汎用\n攻撃,エフェクト1\n\n汎用\n回避,エフェクト2\n";
            mdl.Load("test.effect", true, ToStream(content));
            var md = mdl.Item("汎用");
            Assert.IsNotNull(md);
            // 両方のメッセージが登録されているか確認
            Assert.AreEqual("エフェクト1", md.SelectMessage("攻撃"));
            Assert.AreEqual("エフェクト2", md.SelectMessage("回避"));
        }

        // ──────────────────────────────────────────────
        // エラーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Load_EntryWithCommaInName_ThrowsException()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "アムロ,コズキ\n攻撃,行くぞ！\n";
            Assert.Throws<Exceptions.InvalidSrcDataException>(() => mdl.Load("test.msg", false, ToStream(content)));
        }

        [TestMethod]
        public void Load_EntryWithSpaceInName_ThrowsException()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "アムロ レイ\n攻撃,行くぞ！\n";
            Assert.Throws<Exceptions.InvalidSrcDataException>(() => mdl.Load("test.msg", false, ToStream(content)));
        }

        [TestMethod]
        public void Load_EntryWithFullWidthParenInName_ThrowsException()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "アムロ（レイ）\n攻撃,行くぞ！\n";
            Assert.Throws<Exceptions.InvalidSrcDataException>(() => mdl.Load("test.msg", false, ToStream(content)));
        }

        [TestMethod]
        public void Load_MessageWithoutComma_ThrowsException()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            // コンマが見つからない行
            var content = "アムロ\n攻撃だ\n";
            Assert.Throws<Exceptions.InvalidSrcDataException>(() => mdl.Load("test.msg", false, ToStream(content)));
        }

        [TestMethod]
        public void Load_MessageWithQuoteInName_ThrowsException()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "アムロ\"レイ\n攻撃,行くぞ！\n";
            Assert.Throws<Exceptions.InvalidSrcDataException>(() => mdl.Load("test.msg", false, ToStream(content)));
        }

        // ──────────────────────────────────────────────
        // 名前のバリエーション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Load_JapaneseName_IsDefined()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "ガンダム\n攻撃,ビームライフル！\n";
            mdl.Load("test.msg", false, ToStream(content));
            Assert.IsTrue(mdl.IsDefined("ガンダム"));
        }

        [TestMethod]
        public void Load_AlphanumericName_IsDefined()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "RX78\n攻撃,OK！\n";
            mdl.Load("test.msg", false, ToStream(content));
            Assert.IsTrue(mdl.IsDefined("RX78"));
        }

        [TestMethod]
        public void Load_EmptyMessageContent_IsAllowed()
        {
            var src = CreateSrc();
            var mdl = new MessageDataList(src);
            var content = "アムロ\n攻撃,\n";
            mdl.Load("test.msg", false, ToStream(content));
            var md = mdl.Item("アムロ");
            Assert.IsNotNull(md);
            Assert.AreEqual("", md.SelectMessage("攻撃"));
        }
    }
}
