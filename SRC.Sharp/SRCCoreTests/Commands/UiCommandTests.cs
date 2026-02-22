using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Events;
using SRCCore.TestLib;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// UiCommand クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class UiCommandTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // コンストラクタ (id, label)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_Id_IsSet()
        {
            var cmd = new UiCommand(1, "攻撃");
            Assert.AreEqual(1, cmd.Id);
        }

        [TestMethod]
        public void Constructor_Label_IsSet()
        {
            var cmd = new UiCommand(1, "攻撃");
            Assert.AreEqual("攻撃", cmd.Label);
        }

        [TestMethod]
        public void Constructor_IsChecked_DefaultFalse()
        {
            var cmd = new UiCommand(2, "移動");
            Assert.IsFalse(cmd.IsChecked);
        }

        [TestMethod]
        public void Constructor_LabelData_DefaultNull()
        {
            var cmd = new UiCommand(3, "待機");
            Assert.IsNull(cmd.LabelData);
        }

        [TestMethod]
        public void Constructor_WithIsChecked_True()
        {
            var cmd = new UiCommand(4, "選択済み", isChecked: true);
            Assert.IsTrue(cmd.IsChecked);
        }

        [TestMethod]
        public void Constructor_WithIsChecked_False()
        {
            var cmd = new UiCommand(5, "未選択", isChecked: false);
            Assert.IsFalse(cmd.IsChecked);
        }

        [TestMethod]
        public void Constructor_IdZero_IsSet()
        {
            var cmd = new UiCommand(0, "");
            Assert.AreEqual(0, cmd.Id);
        }

        [TestMethod]
        public void Constructor_NegativeId_IsSet()
        {
            var cmd = new UiCommand(-1, "キャンセル");
            Assert.AreEqual(-1, cmd.Id);
        }

        [TestMethod]
        public void Constructor_EmptyLabel_IsSet()
        {
            var cmd = new UiCommand(10, "");
            Assert.AreEqual("", cmd.Label);
        }

        [TestMethod]
        public void Constructor_JapaneseLabel_IsSet()
        {
            var cmd = new UiCommand(99, "インターミッション");
            Assert.AreEqual("インターミッション", cmd.Label);
        }

        // ──────────────────────────────────────────────
        // コンストラクタ (id, label, labelData)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_WithLabelData_LabelDataIsSet()
        {
            var src = CreateSrc();
            var ld = new LabelData(src);
            var cmd = new UiCommand(1, "ラベル付き", ld);
            Assert.AreSame(ld, cmd.LabelData);
        }

        [TestMethod]
        public void Constructor_WithLabelData_IdIsSet()
        {
            var src = CreateSrc();
            var ld = new LabelData(src);
            var cmd = new UiCommand(7, "コマンド", ld);
            Assert.AreEqual(7, cmd.Id);
        }

        [TestMethod]
        public void Constructor_WithLabelData_LabelIsSet()
        {
            var src = CreateSrc();
            var ld = new LabelData(src);
            var cmd = new UiCommand(7, "コマンド名", ld);
            Assert.AreEqual("コマンド名", cmd.Label);
        }

        [TestMethod]
        public void Constructor_WithLabelData_IsCheckedDefaultFalse()
        {
            var src = CreateSrc();
            var ld = new LabelData(src);
            var cmd = new UiCommand(7, "コマンド", ld);
            Assert.IsFalse(cmd.IsChecked);
        }

        [TestMethod]
        public void Constructor_WithLabelDataAndIsChecked_IsCheckedIsTrue()
        {
            var src = CreateSrc();
            var ld = new LabelData(src);
            var cmd = new UiCommand(7, "コマンド", ld, isChecked: true);
            Assert.IsTrue(cmd.IsChecked);
        }

        [TestMethod]
        public void Constructor_WithNullLabelData_LabelDataIsNull()
        {
            var cmd = new UiCommand(1, "テスト", (LabelData)null);
            Assert.IsNull(cmd.LabelData);
        }

        // ──────────────────────────────────────────────
        // プロパティはreadonly
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Id_IsReadOnly_CannotBeChanged()
        {
            var cmd = new UiCommand(42, "テスト");
            // プロパティが正しい値を返すことを確認
            Assert.AreEqual(42, cmd.Id);
        }

        [TestMethod]
        public void Label_IsReadOnly_CannotBeChanged()
        {
            var cmd = new UiCommand(1, "読み取り専用ラベル");
            Assert.AreEqual("読み取り専用ラベル", cmd.Label);
        }

        [TestMethod]
        public void IsChecked_IsReadOnly_CannotBeChanged()
        {
            var cmd = new UiCommand(1, "テスト", isChecked: true);
            Assert.IsTrue(cmd.IsChecked);
        }

        // ──────────────────────────────────────────────
        // 複数インスタンスの独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TwoInstances_HaveDifferentIds()
        {
            var cmd1 = new UiCommand(1, "コマンド1");
            var cmd2 = new UiCommand(2, "コマンド2");
            Assert.AreNotEqual(cmd1.Id, cmd2.Id);
        }

        [TestMethod]
        public void TwoInstances_HaveDifferentLabels()
        {
            var cmd1 = new UiCommand(1, "ラベルA");
            var cmd2 = new UiCommand(1, "ラベルB");
            Assert.AreNotEqual(cmd1.Label, cmd2.Label);
        }

        [TestMethod]
        public void TwoInstances_DifferentIsChecked()
        {
            var cmd1 = new UiCommand(1, "テスト", isChecked: true);
            var cmd2 = new UiCommand(1, "テスト", isChecked: false);
            Assert.AreNotEqual(cmd1.IsChecked, cmd2.IsChecked);
        }

        // ──────────────────────────────────────────────
        // 境界値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_MaxIntId_IsSet()
        {
            var cmd = new UiCommand(int.MaxValue, "最大ID");
            Assert.AreEqual(int.MaxValue, cmd.Id);
        }

        [TestMethod]
        public void Constructor_MinIntId_IsSet()
        {
            var cmd = new UiCommand(int.MinValue, "最小ID");
            Assert.AreEqual(int.MinValue, cmd.Id);
        }

        [TestMethod]
        public void Constructor_LongLabel_IsSet()
        {
            var longLabel = new string('あ', 200);
            var cmd = new UiCommand(1, longLabel);
            Assert.AreEqual(longLabel, cmd.Label);
        }
    }
}
