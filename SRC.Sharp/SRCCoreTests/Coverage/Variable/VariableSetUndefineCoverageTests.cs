using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Variable.cs の SetVariable / UndefineVariable / IsVariableDefined / 配列インデックス解決のテスト
    /// （VB: Expression.bas SetVariable, UndefineVariable, IsVariableDefined）
    /// </summary>
    [TestClass]
    public class VariableSetUndefineCoverageTests
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

        private static Units.Unit CreateUnit(SRC src, string name, string party = "味方")
        {
            if (!src.UDList.IsDefined(name))
            {
                var ud = src.UDList.Add(name);
                ud.HP = 1000;
                ud.EN = 100;
                ud.PilotNum = 1;
            }
            var u = src.UList.Add(name, 0, party);
            u.Update();
            u.FullRecover();
            return u;
        }

        private static Pilots.Pilot CreatePilot(SRC src, string name, int sp = 100)
        {
            var pd = src.PDList.Add(name);
            pd.SP = sp;
            return src.PList.Add(name, 1, "味方");
        }

        // サブルーチン呼び出し中（CallDepth=1）の状態でサブルーチンローカル変数を作る
        private static VarData AddSubLocal(SRC src, string name, string value)
        {
            src.Event.CallDepth = 1;
            src.Event.VarIndexStack[0] = 0;
            var v = src.Event.NewSubLocalVar();
            v.SetValue(name, Expressions.ValueType.StringType, value, 0d);
            return v;
        }

        // ──────────────────────────────────────────────
        // 左辺値を伴う関数 HP() / EN() / SP() / Action() / Eval()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariable_HpByUnitId_SetsHpAndClampsZeroToOne()
        {
            // HP(ユニットID) への代入。0 以下は 1 に補正される（ユニットは破壊されない）
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");

            src.Expression.SetVariableAsLong("HP(" + u.ID + ")", 300);
            Assert.AreEqual(300, u.HP);

            src.Expression.SetVariableAsLong("HP(" + u.ID + ")", 0);
            Assert.AreEqual(1, u.HP);
        }

        [TestMethod]
        public void SetVariable_HpByPilotName_SetsHpOfRiddenUnitFromString()
        {
            // パイロット名を指定した場合はそのパイロットが乗っているユニットが対象。文字列値は StrToLng で変換
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            var p = CreatePilot(src, "テスト乗員");
            p.Ride(u);

            src.Expression.SetVariableAsString("HP(テスト乗員)", "450");

            Assert.AreEqual(450, u.HP);
            // 変数としては作られない
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("HP(テスト乗員)"));
        }

        [TestMethod]
        public void SetVariable_HpWithUnknownIndex_TargetsSelectedUnitForEvent()
        {
            // 該当ユニットもパイロットもいない場合は SelectedUnitForEvent が対象
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            src.Event.SelectedUnitForEvent = u;

            src.Expression.SetVariableAsLong("HP(存在しない)", 123);

            Assert.AreEqual(123, u.HP);
        }

        [TestMethod]
        public void SetVariable_EnToZeroOnSortiedUnit_RepaintsUnitBitmap()
        {
            // EN が 0 になった出撃中のユニットは EN 切れ表示のため再描画される
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            var painted = new List<Units.Unit>();
            mock.PaintUnitBitmapHandler = (unit, mode) => painted.Add(unit);
            var u = CreateUnit(src, "テスト機");
            u.Status = "出撃";

            src.Expression.SetVariableAsLong("EN(" + u.ID + ")", 40);
            Assert.AreEqual(40, u.EN);
            Assert.AreEqual(0, painted.Count);

            src.Expression.SetVariableAsLong("EN(" + u.ID + ")", 0);
            Assert.AreEqual(0, u.EN);
            CollectionAssert.AreEqual(new[] { u }, painted);
        }

        [TestMethod]
        public void SetVariable_EnToZeroOnStandbyUnit_DoesNotRepaint()
        {
            // 出撃していないユニットは再描画しない
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            var painted = 0;
            mock.PaintUnitBitmapHandler = (unit, mode) => painted++;
            var u = CreateUnit(src, "テスト機");
            u.Status = "待機";

            src.Expression.SetVariableAsString("EN(" + u.ID + ")", "0");

            Assert.AreEqual(0, u.EN);
            Assert.AreEqual(0, painted);
        }

        [TestMethod]
        public void SetVariable_SpByPilotName_SetsSpOnlyWhenPilotHasSp()
        {
            // SP() はＳＰを持つパイロットのみ変更される（MaxSP > 0）
            var src = CreateSrc();
            var withSp = CreatePilot(src, "ＳＰ持ち", sp: 50);
            var noSp = CreatePilot(src, "ＳＰ無し", sp: 0);
            var before = noSp.SP;

            src.Expression.SetVariableAsLong("SP(ＳＰ持ち)", 7);
            src.Expression.SetVariableAsLong("SP(ＳＰ無し)", 7);

            Assert.AreEqual(7, withSp.SP);
            Assert.AreEqual(before, noSp.SP);
        }

        [TestMethod]
        public void SetVariable_SpByUnitId_TargetsMainPilot()
        {
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            var p = CreatePilot(src, "テスト乗員", sp: 50);
            p.Ride(u);

            src.Expression.SetVariableAsString("SP(" + u.ID + ")", "12");

            Assert.AreEqual(12, p.SP);
        }

        [TestMethod]
        public void SetVariable_Action_SetsUsedActionAsMaxActionMinusValue()
        {
            // Action(ユニット) = 残り行動数。UsedAction = MaxAction - 値
            var src = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            CreatePilot(src, "テスト乗員").Ride(u);
            Assert.AreEqual(1, u.MaxAction());

            src.Expression.SetVariableAsLong("Action(" + u.ID + ")", 0);
            Assert.AreEqual(1, u.UsedAction);

            src.Expression.SetVariableAsString("Action(" + u.ID + ")", "1");
            Assert.AreEqual(0, u.UsedAction);
        }

        [TestMethod]
        public void SetVariable_Eval_SetsVariableNamedByEvaluatedExpression()
        {
            // Eval(式) を左辺に使うと式の評価結果を変数名として代入する
            var src = CreateSrc();

            src.Expression.SetVariableAsString("Eval(\"ab\" & \"cd\")", "値");

            Assert.IsTrue(src.Expression.IsLocalVariableDefined("abcd"));
            Assert.AreEqual("値", src.Expression.GetValueAsString("abcd"));
        }

        // ──────────────────────────────────────────────
        // 書き込み可能なシステム変数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariable_BaseXBaseY_UpdatesEventAndNotifiesGui()
        {
            // BaseX/BaseY は大文字小文字を区別せず、描画基準位置として GUI に反映される
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            int? gx = null, gy = null;
            mock.UpdateBaseXHandler = x => gx = x;
            mock.UpdateBaseYHandler = y => gy = y;

            src.Expression.SetVariableAsString("BASEX", "120");
            src.Expression.SetVariableAsDouble("basey", 45d);

            Assert.AreEqual(120, src.Event.BaseX);
            Assert.AreEqual(45, src.Event.BaseY);
            Assert.AreEqual(120, gx);
            Assert.AreEqual(45, gy);
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("BASEX"));
        }

        [TestMethod]
        public void SetVariable_TurnCounters_UpdateSrcInsteadOfCreatingVariable()
        {
            var src = CreateSrc();

            src.Expression.SetVariableAsDouble("ターン数", 5d);
            src.Expression.SetVariableAsString("総ターン数", "17");

            Assert.AreEqual(5, src.Turn);
            Assert.AreEqual(17, src.TotalTurn);
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("ターン数"));
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("総ターン数"));
        }

        [TestMethod]
        public void SetVariable_Money_ResetsThenIncrementsWithinRange()
        {
            // 資金は 0 にしてから IncrMoney するため 0～999999999 に収まる
            var src = CreateSrc();
            src.Money = 5000;

            src.Expression.SetVariableAsDouble("資金", 1200d);
            Assert.AreEqual(1200, src.Money);

            src.Expression.SetVariableAsString("資金", "-300");
            Assert.AreEqual(0, src.Money);

            src.Expression.SetVariableAsLong("資金", 1500000000);
            Assert.AreEqual(999999999, src.Money);
        }

        [TestMethod]
        public void SetVariable_LocalVariableShadowsSystemVariable()
        {
            // ローカル変数として定義済みならシステム変数より優先される
            var src = CreateSrc();
            src.Turn = 3;
            src.Expression.DefineLocalVariable("ターン数");

            src.Expression.SetVariableAsDouble("ターン数", 9d);

            Assert.AreEqual(3, src.Turn);
            Assert.AreEqual(9d, src.Expression.GetValueAsDouble("ターン数"));
        }

        // ──────────────────────────────────────────────
        // 配列
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariable_UndefinedArray_CreatesLocalArrayHeaderAndElement()
        {
            // 未定義の配列への代入はローカル変数の配列として配列名と要素が作られる
            var src = CreateSrc();

            src.Expression.SetVariableAsDouble("arr[1 + 1]", 8d);

            Assert.IsTrue(src.Expression.IsLocalVariableDefined("arr"));
            Assert.IsTrue(src.Expression.IsLocalVariableDefined("arr[2]"));
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("arr[2]"));
            Assert.AreEqual(8d, src.Expression.GetValueAsDouble("arr[2]"));
        }

        [TestMethod]
        public void SetVariable_GlobalArrayName_CreatesElementAsGlobal()
        {
            // 配列名がグローバル変数なら要素もグローバル変数として作られる（Global は配列名に対して使う）
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("garr");

            src.Expression.SetVariableAsString("garr[キー]", "値");

            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("garr[キー]"));
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("garr[キー]"));
            Assert.AreEqual("値", src.Expression.GetValueAsString("garr[キー]"));
        }

        [TestMethod]
        public void SetVariable_MultiDimensionIndex_EvaluatesEachIndexAndTrims()
        {
            // 多次元配列は「,」で区切った各インデックスを個別に評価して連結する
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("n", 3d);

            src.Expression.SetVariableAsString("m[1 + 1, n ,\"a,b\"]", "x");

            Assert.IsTrue(src.Expression.IsLocalVariableDefined("m[2,3,a,b]"));
            Assert.AreEqual("x", src.Expression.GetValueAsString("m[2,3,a,b]"));
        }

        [TestMethod]
        public void SetVariable_NestedBracketInIndex_IsNotSplitAtInnerComma()
        {
            // 括弧内の「,」はインデックスの区切りとみなさない
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("t[5]", 7d);

            src.Expression.SetVariableAsString("m[Max(1,2), t[5]]", "y");

            Assert.IsTrue(src.Expression.IsLocalVariableDefined("m[2,7]"));
        }

        [TestMethod]
        public void SetVariable_NameWithDoubleQuote_ReportsInvalidVariableError()
        {
            // 「"」を含む変数名は不正な変数としてエラー表示（変数自体は作られる）
            var src = CreateSrc();

            src.Expression.SetVariableAsString("bad\"name", "v");

            Assert.IsTrue(src.DataErrors.Any(e => e.msg.Contains("不正な変数「bad\"name」が作成されました")));
            Assert.IsTrue(src.Expression.IsLocalVariableDefined("bad\"name"));
        }

        // ──────────────────────────────────────────────
        // サブルーチンローカル変数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariable_SubLocalVariable_UpdatesStackEntryNotLocalList()
        {
            var src = CreateSrc();
            var v = AddSubLocal(src, "sv", "初期");

            src.Expression.SetVariableAsDouble("sv", 42d);

            Assert.AreEqual(Expressions.ValueType.NumericType, v.VariableType);
            Assert.AreEqual(42d, v.NumericValue);
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("sv"));
            Assert.IsTrue(src.Expression.IsVariableDefined("sv"));
        }

        [TestMethod]
        public void SetVariable_SubLocalArray_CreatesElementOnStack()
        {
            // サブルーチンローカルな配列の要素はスタック上に作成される
            var src = CreateSrc();
            AddSubLocal(src, "sarr", "");
            var before = src.Event.VarIndex;

            src.Expression.SetVariableAsString("sarr[1]", "要素");

            Assert.AreEqual(before + 1, src.Event.VarIndex);
            Assert.AreEqual("要素", src.Event.SubLocalVar("sarr[1]")?.StringValue);
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("sarr[1]"));
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("sarr"));
        }

        [TestMethod]
        public void SetVariable_SubLocalArrayOverflow_ReportsErrorAndKeepsIndex()
        {
            // サブルーチンローカル変数の総数が MaxVarIndex を超える場合はエラー
            var src = CreateSrc();
            src.Event.CallDepth = 1;
            src.Event.VarIndexStack[0] = Event.MaxVarIndex - 1;
            src.Event.VarIndex = Event.MaxVarIndex;
            src.Event.VarStack[Event.MaxVarIndex].SetValue("sarr", Expressions.ValueType.StringType, "", 0d);

            src.Expression.SetVariableAsString("sarr[1]", "要素");

            Assert.AreEqual(Event.MaxVarIndex, src.Event.VarIndex);
            Assert.IsTrue(src.DataErrors.Any(e => e.msg.Contains("サブルーチンローカル変数の総数が" + Event.MaxVarIndex + "個を超えています")));
            Assert.IsNull(src.Event.SubLocalVar("sarr[1]"));
        }

        [TestMethod]
        public void SetVariable_SubLocalNameWithDifferentCase_DoesNotOverwriteSubLocal()
        {
            // VB の SetVariable/GetVariable はどちらも VarStack(i).Name = vname（完全一致）で照合するため、
            // 大文字小文字の違う名前への代入は別のローカル変数になり、元のサブルーチンローカル変数は変わらない
            var src = CreateSrc();
            var v = AddSubLocal(src, "abc", "サブ");

            src.Expression.SetVariableAsString("ABC", "代入値");

            Assert.AreEqual("abc", v.Name);
            Assert.AreEqual("サブ", src.Expression.GetValueAsString("abc"));
            Assert.AreEqual("代入値", src.Expression.GetValueAsString("ABC"));
        }

        [TestMethod]
        public void SetVariable_SingleIndexWithSpaces_IsTrimmed()
        {
            // VB: 単一インデックスは Trim$ してから評価する
            var src = CreateSrc();

            src.Expression.SetVariableAsString("arr[ 2 ]", "v");

            Assert.IsTrue(src.Expression.IsLocalVariableDefined("arr[2]"));
        }

        [TestMethod]
        public void GetValue_SubLocalVariable_TakesPrecedenceOverLocal()
        {
            // 変数参照はサブルーチンローカル → ローカル → グローバルの順
            var src = CreateSrc();
            src.Expression.SetVariableAsString("x", "ローカル");
            AddSubLocal(src, "x", "サブ");

            Assert.AreEqual("サブ", src.Expression.GetValueAsString("x"));
        }

        // ──────────────────────────────────────────────
        // IsVariableDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsVariableDefined_ArrayIndexExpression_IsResolvedBeforeLookup()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 2d);
            src.Expression.SetVariableAsString("arr[3]", "v");

            Assert.IsTrue(src.Expression.IsVariableDefined("arr[i + 1]"));
            Assert.IsTrue(src.Expression.IsVariableDefined("$arr[3]"));
            Assert.IsFalse(src.Expression.IsVariableDefined("arr[i]"));
        }

        [TestMethod]
        public void IsVariableDefined_GlobalVariable_ReturnsTrue()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("gv");

            Assert.IsTrue(src.Expression.IsVariableDefined("gv"));
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("gv"));
        }

        // ──────────────────────────────────────────────
        // UndefineVariable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UndefineVariable_LocalArrayName_RemovesAllElementsOnly()
        {
            // 配列名を指定すると「名前[」で始まる要素もすべて消去される。前方一致の別変数は残る
            var src = CreateSrc();
            src.Expression.SetVariableAsString("arr[1]", "a");
            src.Expression.SetVariableAsString("arr[2]", "b");
            src.Expression.SetVariableAsString("arrX", "c");

            src.Expression.UndefineVariable("arr");

            Assert.IsFalse(src.Expression.IsLocalVariableDefined("arr"));
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("arr[1]"));
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("arr[2]"));
            Assert.IsTrue(src.Expression.IsLocalVariableDefined("arrX"));
        }

        [TestMethod]
        public void UndefineVariable_GlobalArrayName_RemovesAllElements()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("garr");
            src.Expression.SetVariableAsString("garr[1]", "a");
            src.Expression.SetVariableAsString("garr[2]", "b");
            src.Expression.DefineGlobalVariable("garrY");

            src.Expression.UndefineVariable("$garr");

            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("garr"));
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("garr[1]"));
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("garr[2]"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("garrY"));
        }

        [TestMethod]
        public void UndefineVariable_ArrayElementWithExpressionIndex_RemovesOnlyThatElement()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("arr[1]", "a");
            src.Expression.SetVariableAsString("arr[2]", "b");

            src.Expression.UndefineVariable("arr[1 + 1]");

            Assert.IsTrue(src.Expression.IsLocalVariableDefined("arr[1]"));
            Assert.IsFalse(src.Expression.IsLocalVariableDefined("arr[2]"));
            Assert.IsTrue(src.Expression.IsLocalVariableDefined("arr"));
        }

        [TestMethod]
        public void UndefineVariable_MultiDimensionElement_ResolvesEachIndex()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("m[1,2]", "a");
            src.Expression.SetVariableAsString("m[1,3]", "b");
            src.Expression.DefineGlobalVariable("g");
            src.Expression.SetVariableAsString("g[x,1]", "c");

            src.Expression.UndefineVariable("m[1, 1 + 1]");
            src.Expression.UndefineVariable("g[\"x\",1]");

            Assert.IsFalse(src.Expression.IsLocalVariableDefined("m[1,2]"));
            Assert.IsTrue(src.Expression.IsLocalVariableDefined("m[1,3]"));
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("g[x,1]"));
        }

        [TestMethod]
        public void UndefineVariable_Eval_RemovesVariableNamedByExpression()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("abcd", "v");

            src.Expression.UndefineVariable("Eval(\"ab\" & \"cd\")");

            Assert.IsFalse(src.Expression.IsVariableDefined("abcd"));
        }

        [TestMethod]
        public void UndefineVariable_SubLocalArray_ClearsHeaderAndElements()
        {
            // サブルーチンローカル変数は名前を消去して未定義にする（要素も含む）
            var src = CreateSrc();
            AddSubLocal(src, "sarr", "");
            src.Expression.SetVariableAsString("sarr[1]", "a");
            AddSubLocal(src, "other", "o");

            src.Expression.UndefineVariable("sarr");

            Assert.IsFalse(src.Expression.IsSubLocalVariableDefined("sarr"));
            Assert.IsFalse(src.Expression.IsSubLocalVariableDefined("sarr[1]"));
            Assert.IsTrue(src.Expression.IsSubLocalVariableDefined("other"));
        }

        [TestMethod]
        public void UndefineVariable_SubLocalArrayElement_ClearsOnlyThatElement()
        {
            var src = CreateSrc();
            AddSubLocal(src, "sarr", "");
            src.Expression.SetVariableAsString("sarr[1]", "a");
            src.Expression.SetVariableAsString("sarr[2]", "b");

            src.Expression.UndefineVariable("sarr[1]");

            Assert.IsFalse(src.Expression.IsSubLocalVariableDefined("sarr[1]"));
            Assert.IsTrue(src.Expression.IsSubLocalVariableDefined("sarr[2]"));
            Assert.IsTrue(src.Expression.IsSubLocalVariableDefined("sarr"));
        }
    }
}
