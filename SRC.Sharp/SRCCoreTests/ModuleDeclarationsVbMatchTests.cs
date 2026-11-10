using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using VT = SRCCore.Expressions.ValueType;

namespace SRCCore.Tests
{
    /// <summary>
    /// モジュールの宣言（手続きの外の変数）の初期値を元の SRC に合わせたことのテスト（7474/SRC#1498）。
    /// VB の String の変数の初期値は空文字列（""）で、システム変数で読んでも空文字列になる。
    /// </summary>
    [TestClass]
    public class ModuleDeclarationsVbMatchTests
    {
        private static SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private static string GetStr(SRC src, string name)
        {
            src.Expression.GetVariable(name, VT.StringType, out var s, out _);
            return s;
        }

        // SRC.bas: Public Stage As String
        [TestMethod]
        public void Stage_InitialValue_IsEmptyString()
        {
            var src = CreateSrc();
            Assert.AreEqual("", src.Stage);
            Assert.AreEqual("", GetStr(src, "フェイズ"));
            Assert.AreEqual("", src.Expression.GetValueAsString("フェイズ"));
        }

        // SRC.bas: Public LastSaveDataFileName As String
        [TestMethod]
        public void LastSaveDataFileName_InitialValue_IsEmptyString()
        {
            Assert.AreEqual("", CreateSrc().LastSaveDataFileName);
        }

        // Event.bas: Public SelectedAlternative As String
        [TestMethod]
        public void SelectedAlternative_InitialValue_IsEmptyString()
        {
            var src = CreateSrc();
            Assert.AreEqual("", src.Event.SelectedAlternative);
            Assert.AreEqual("", src.Expression.GetValueAsString("選択"));
        }

        // Command.bas: Public SelectedCommand As String など（選択の状況の文字列）
        [TestMethod]
        public void SelectedStateStrings_InitialValue_IsEmptyString()
        {
            var src = CreateSrc();
            Assert.AreEqual("", src.Commands.SelectedCommand);
            Assert.AreEqual("", src.Commands.SelectedWeaponName);
            Assert.AreEqual("", src.Commands.SelectedTWeaponName);
            Assert.AreEqual("", src.Commands.SelectedDefenseOption);
            Assert.AreEqual("", src.Commands.SelectedAbilityName);
            Assert.AreEqual("", src.Commands.SelectedSpecialPower);
            // 対象ユニット（Nothing）と選択されたユニット（Nothing）が同じなので SelectedSpecialPower を返す
            Assert.AreEqual("", GetStr(src, "対象ユニット使用スペシャルパワー"));
            Assert.AreEqual("", src.Expression.GetValueAsString("対象ユニット使用スペシャルパワー"));
        }
    }
}
