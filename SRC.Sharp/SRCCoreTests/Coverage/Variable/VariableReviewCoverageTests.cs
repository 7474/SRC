using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.TestLib;
using System.Collections.Generic;
using VT = SRCCore.Expressions.ValueType;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Variable.cs の SetVariable（Plana/SP/Action の対象解決）とサブルーチンローカル変数の参照のテスト
    /// （VB: Expression.bas SetVariable, GetVariable, IsVariableDefined）
    /// </summary>
    [TestClass]
    public class VariableReviewCoverageTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>
            {
                new EventDataLine(0, EventDataSource.Scenario, "test.eve", 1, "Set x 1"),
            };
            return src;
        }

        private static Units.Unit CreateUnit(SRC src, string name)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            var u = src.UList.Add(name, 0, "味方");
            u.Update();
            u.FullRecover();
            return u;
        }

        private static Pilots.Pilot CreatePilot(SRC src, string name, int sp = 50, double plana = 0d)
        {
            var pd = src.PDList.Add(name);
            pd.SP = sp;
            if (plana > 0d)
            {
                pd.AddSkill("霊力", plana, "", 1);
            }
            return src.PList.Add(name, 1, "味方");
        }

        // サブルーチン呼び出し中（CallDepth=1）の状態でサブルーチンローカル変数を作る
        private static VarData AddSubLocal(SRC src, string name, VT type, string str, double num)
        {
            src.Event.CallDepth = 1;
            src.Event.VarIndexStack[0] = 0;
            var v = src.Event.NewSubLocalVar();
            v.SetValue(name, type, str, num);
            return v;
        }

        // ──────────────────────────────────────────────
        // Plana() / SP() / Action() への代入
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariable_PlanaByPilotName_SetsOnlyWhenPilotHasPlana()
        {
            // Plana() は霊力を持つパイロット（MaxPlana > 0）のみ変更される（VB: SetVariable の plana）
            var src = CreateSrc();
            var withPlana = CreatePilot(src, "霊力持ち", plana: 20d);
            var noPlana = CreatePilot(src, "霊力無し");
            Assert.IsTrue(withPlana.MaxPlana() > 15);
            Assert.AreEqual(0, noPlana.MaxPlana());

            src.Expression.SetVariableAsLong("Plana(霊力持ち)", 15);
            src.Expression.SetVariableAsLong("Plana(霊力無し)", 15);

            Assert.AreEqual(15, withPlana.Plana);
            Assert.AreEqual(0, noPlana.Plana);
        }

        [TestMethod]
        public void SetVariable_PlanaByUnitIdAsString_TargetsMainPilotAndClampsToMax()
        {
            // ユニットIDを指定するとメインパイロットが対象。文字列値は StrToLng で変換し、最大値で頭打ち
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            var p = CreatePilot(src, "テスト乗員", plana: 20d);
            p.Ride(u);

            src.Expression.SetVariableAsString("Plana(" + u.ID + ")", "9");
            Assert.AreEqual(9, p.Plana);

            src.Expression.SetVariableAsString("Plana(" + u.ID + ")", "99999");
            Assert.AreEqual(p.MaxPlana(), p.Plana);
        }

        [TestMethod]
        public void SetVariable_PlanaWithUnknownIndex_TargetsSelectedUnitMainPilot()
        {
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            var p = CreatePilot(src, "テスト乗員", plana: 20d);
            p.Ride(u);
            src.Event.SelectedUnitForEvent = u;

            src.Expression.SetVariableAsDouble("Plana(存在しない)", 4d);

            Assert.AreEqual(4, p.Plana);
        }

        [TestMethod]
        public void SetVariable_SpWithUnknownIndex_TargetsSelectedUnitMainPilot()
        {
            // 該当ユニットもパイロットもいない場合は SelectedUnitForEvent のメインパイロットが対象
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            var p = CreatePilot(src, "テスト乗員", sp: 50);
            p.Ride(u);
            src.Event.SelectedUnitForEvent = u;

            src.Expression.SetVariableAsString("SP(存在しない)", "21");

            Assert.AreEqual(21, p.SP);
        }

        [TestMethod]
        public void SetVariable_ActionByPilotName_TargetsRiddenUnit()
        {
            // パイロット名を指定すると搭乗ユニットの行動数が変わる
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            CreatePilot(src, "テスト乗員").Ride(u);

            src.Expression.SetVariableAsLong("Action(テスト乗員)", 0);

            Assert.AreEqual(u.MaxAction(), u.UsedAction);
            Assert.AreEqual(1, u.UsedAction);
        }

        [TestMethod]
        public void SetVariable_ActionWithUnknownIndex_TargetsSelectedUnitForEvent()
        {
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            CreatePilot(src, "テスト乗員").Ride(u);
            u.UsedAction = 1;
            src.Event.SelectedUnitForEvent = u;

            src.Expression.SetVariableAsString("Action(存在しない)", "1");

            Assert.AreEqual(0, u.UsedAction);
        }

        // ──────────────────────────────────────────────
        // サブルーチンローカル変数の型変換（VB: GetVariable のサブルーチンローカル変数部分）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetVariable_SubLocalString_ReadAsNumeric_IsConvertedByStrToDbl()
        {
            var src = CreateSrc();
            AddSubLocal(src, "sv", VT.StringType, "12.5", 0d);

            var type = src.Expression.GetVariable("sv", VT.NumericType, out _, out var num);

            Assert.AreEqual(VT.NumericType, type);
            Assert.AreEqual(12.5d, num);
        }

        [TestMethod]
        public void GetVariable_SubLocalNumeric_ReadAsString_IsFormatted()
        {
            var src = CreateSrc();
            AddSubLocal(src, "sv", VT.NumericType, "", 3.5d);

            var type = src.Expression.GetVariable("sv", VT.StringType, out var str, out _);

            Assert.AreEqual(VT.StringType, type);
            Assert.AreEqual("3.5", str);
        }

        [TestMethod]
        public void GetVariable_SubLocal_UndefinedTypeRequest_ReturnsStoredType()
        {
            // 型を指定しない参照では格納されている型のまま返す
            var src = CreateSrc();
            AddSubLocal(src, "sn", VT.NumericType, "", 7d);
            AddSubLocal(src, "ss", VT.StringType, "文字", 0d);

            var t1 = src.Expression.GetVariable("sn", VT.UndefinedType, out _, out var num);
            var t2 = src.Expression.GetVariable("ss", VT.UndefinedType, out var str, out _);

            Assert.AreEqual(VT.NumericType, t1);
            Assert.AreEqual(7d, num);
            Assert.AreEqual(VT.StringType, t2);
            Assert.AreEqual("文字", str);
        }

        [TestMethod]
        public void SubLocalArrayElement_WithExpressionIndex_IsResolvedForLookup()
        {
            // サブルーチンローカルな配列の要素もインデックスの式を評価してから照合する
            var src = CreateSrc();
            AddSubLocal(src, "sarr", VT.StringType, "", 0d);
            src.Expression.SetVariableAsString("sarr[2]", "二");
            src.Expression.SetVariableAsDouble("i", 1d);

            Assert.IsTrue(src.Expression.IsVariableDefined("sarr[i + 1]"));
            Assert.IsFalse(src.Expression.IsVariableDefined("sarr[i]"));
            var type = src.Expression.GetVariable("sarr[i + 1]", VT.StringType, out var str, out _);
            Assert.AreEqual(VT.StringType, type);
            Assert.AreEqual("二", str);
            // ローカル変数としては作られていない
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("sarr[2]"));
        }
    }
}
