using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System.Text.RegularExpressions;
using VT = SRCCore.Expressions.ValueType;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression.GetVariable・GetVariableObject・IsSubLocalVariableDefined を元の SRC（Expression.bas）に合わせたことのテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class GetVariableVBMatchTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static Units.Unit CreateUnit(SRC src, string name)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            var u = src.UList.Add(name, 0, "味方");
            u.Update();
            return u;
        }

        [TestMethod]
        public void Selection_StringRequest_ReturnsSelectedAlternativeAsIs()
        {
            // VB の GetVariable の「選択」: 文字列として求めると SelectedAlternative をそのまま返す。
            // 数値に読める「007」も「7」にしない
            var src = CreateSrc();
            src.Event.SelectedAlternative = "007";

            var t = src.Expression.GetVariable("選択", VT.StringType, out var s, out _);
            Assert.AreEqual(VT.StringType, t);
            Assert.AreEqual("007", s);

            var tn = src.Expression.GetVariable("選択", VT.NumericType, out _, out var n);
            Assert.AreEqual(VT.NumericType, tn);
            Assert.AreEqual(7d, n);
        }

        [TestMethod]
        public void Selection_UndefinedRequest_IsString()
        {
            // VB: 型を決めずに求めると StringType（Set で代入すると文字列の変数になる）
            var src = CreateSrc();
            src.Event.SelectedAlternative = "007";

            var t = src.Expression.GetVariable("選択", VT.UndefinedType, out var s, out _);
            Assert.AreEqual(VT.StringType, t);
            Assert.AreEqual("007", s);

            var etype = src.Expression.EvalTerm("選択", VT.UndefinedType, out var str, out _);
            Assert.AreEqual(VT.StringType, etype);
            Assert.AreEqual("007", str);
        }

        [TestMethod]
        public void UsedWeaponNumber_NoMatchingUnit_IsUndefinedAndEmpty()
        {
            // VB: 対象ユニットが攻撃側でも防御側でもないときは型を決めず、空文字列を返す
            var src = CreateSrc();
            var u = CreateUnit(src, "ユニット");
            src.Event.SelectedUnitForEvent = u;
            src.Event.SelectedTargetForEvent = u;
            src.Commands.SelectedUnit = null;
            src.Commands.SelectedTarget = null;
            src.Commands.SelectedWeapon = 3;

            Assert.AreEqual(VT.UndefinedType, src.Expression.GetVariable("対象ユニット使用武器番号", VT.StringType, out var s1, out _));
            Assert.AreEqual("", s1);
            Assert.AreEqual(VT.UndefinedType, src.Expression.GetVariable("相手ユニット使用武器番号", VT.StringType, out var s2, out _));
            Assert.AreEqual("", s2);
            Assert.AreEqual(VT.UndefinedType, src.Expression.GetVariable("対象ユニット使用アビリティ番号", VT.NumericType, out _, out var n));
            Assert.AreEqual(0d, n);
        }

        [TestMethod]
        public void UsedWeaponNumber_StringRequest_IsString()
        {
            // VB: 文字列として求めると Format$(SelectedWeapon) を StringType で返す
            var src = CreateSrc();
            var u = CreateUnit(src, "ユニット");
            src.Event.SelectedUnitForEvent = u;
            src.Commands.SelectedUnit = u;
            src.Commands.SelectedWeapon = 2;

            Assert.AreEqual(VT.StringType, src.Expression.GetVariable("対象ユニット使用武器番号", VT.StringType, out var s, out _));
            Assert.AreEqual("2", s);
        }

        [TestMethod]
        public void TurnCount_StringRequest_IsString()
        {
            // VB: 「ターン数」を文字列として求めると StringType
            var src = CreateSrc();
            src.Turn = 5;

            Assert.AreEqual(VT.StringType, src.Expression.GetVariable("ターン数", VT.StringType, out var s, out _));
            Assert.AreEqual("5", s);
            Assert.AreEqual(VT.NumericType, src.Expression.GetVariable("ターン数", VT.UndefinedType, out _, out var n));
            Assert.AreEqual(5d, n);
        }

        [TestMethod]
        public void Now_IsJapaneseWindowsDateTimeFormat()
        {
            // VB の CStr(Now)。日本語版の Windows では「2026/11/06 9:05:03」の形
            var src = CreateSrc();
            src.Expression.GetVariable("Now", VT.StringType, out var s, out _);

            StringAssert.Matches(s, new Regex(@"^\d{4}/\d{2}/\d{2}( \d{1,2}:\d{2}:\d{2})?$"));
        }

        [TestMethod]
        public void UndefinedTypeVariable_StringRequest_FormatsNumericValue()
        {
            // VB の GetVariable: 型が StringType でない変数を文字列として求めると FormatNum(NumericValue)
            var src = CreateSrc();
            src.Expression.SetVariable("v", VT.UndefinedType, "abc", 2.5d);

            Assert.AreEqual(VT.StringType, src.Expression.GetVariable("v", VT.StringType, out var s, out _));
            Assert.AreEqual("2.5", s);
            Assert.AreEqual(VT.NumericType, src.Expression.GetVariable("v", VT.UndefinedType, out _, out var n));
            Assert.AreEqual(2.5d, n);
        }

        [TestMethod]
        public void SubLocalVariable_IsNotVisibleOutsideSubroutine()
        {
            // VB の IsSubLocalVariableDefined・SetVariable: CallDepth = 0 ではサブルーチンローカル変数を見ない
            var src = CreateSrc();
            src.Event.NewSubLocalVar().SetValue("x", VT.NumericType, "", 1d);

            Assert.IsFalse(src.Expression.IsSubLocalVariableDefined("x"));
            Assert.IsFalse(src.Expression.IsVariableDefined("x"));

            src.Event.CallDepth = 1;
            Assert.IsTrue(src.Expression.IsSubLocalVariableDefined("x"));
        }

        [TestMethod]
        public void GetVariableObject_ConfigVariable_IsNotSystemVariable()
        {
            // VB の GetVariableObject はコンフィグ変数を探さない
            var src = CreateSrc();
            var u = CreateUnit(src, "ユニット");
            src.Event.BCVariable.IsConfig = true;
            src.Event.BCVariable.MeUnit = u;
            src.Event.BCVariable.AttackExp = 100;

            Assert.IsNull(src.Expression.GetVariableObject("攻撃値"));
            Assert.AreEqual(100d, src.Expression.GetValueAsDouble("攻撃値"));
        }
    }
}
