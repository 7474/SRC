using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Set/Incr/Global/Array/CopyArray/Swap/UnSet コマンドのユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class VariableCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return src;
        }

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // SetCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetCmd_NumericValue_StoresValue()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Set x 100");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(100d, src.Expression.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void SetCmd_StringValue_StoresValue()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Set name \"テスト\"");
            cmd.Exec();

            Assert.AreEqual("テスト", src.Expression.GetValueAsString("name"));
        }

        [TestMethod]
        public void SetCmd_NoValue_SetsOne()
        {
            // ヘルプ: valueの項目を省略した場合にはvariableに1が代入されます
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Set flag");
            cmd.Exec();

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("flag"));
        }

        [TestMethod]
        public void SetCmd_AssignmentSyntax_StoresValue()
        {
            // ヘルプ: variable = value の形式でも同じ処理
            var src = CreateSrc();
            var cmd = CreateCmd(src, "x = 42");
            cmd.Exec();

            Assert.AreEqual(42d, src.Expression.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void SetCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Set x 1 2 3");
            var result = cmd.Exec();

            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // IncrCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IncrCmd_NoAmount_IncrementsBy1()
        {
            // ヘルプ: valueを指定しなければ変数の値が1だけ増加されます
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 5d);
            var cmd = CreateCmd(src, "Incr x");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(6d, src.Expression.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void IncrCmd_WithAmount_IncrementsByAmount()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 10d);
            var cmd = CreateCmd(src, "Incr x 3");
            cmd.Exec();

            Assert.AreEqual(13d, src.Expression.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void IncrCmd_NegativeAmount_Decrements()
        {
            // ヘルプ: valueにマイナスの値を指定して値を減少させることもできます
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 10d);
            var cmd = CreateCmd(src, "Incr x -4");
            cmd.Exec();

            Assert.AreEqual(6d, src.Expression.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void IncrCmd_UndefinedVariable_CreatesAndIncrements()
        {
            // ヘルプ: variableがまだ作成されていない場合は変数variableが自動的に作成されます
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Incr newVar");
            cmd.Exec();

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("newVar"));
        }

        [TestMethod]
        public void IncrCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Incr x 1 2");
            var result = cmd.Exec();

            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // GlobalCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GlobalCmd_DefinesGlobalVariable()
        {
            // ヘルプ: グローバル変数variableを作成します
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global myFlag");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("myFlag"));
        }

        [TestMethod]
        public void GlobalCmd_NewVariable_InitiallyZero()
        {
            // ヘルプ: 作成時の変数の値は空文字列(数値として扱う場合は0)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global counter");
            cmd.Exec();

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("counter"));
        }

        [TestMethod]
        public void GlobalCmd_AlreadyDefined_NoError()
        {
            // ヘルプ: 既にGlobalコマンドで作成されているグローバル変数に対してGlobalコマンドを実行してもエラーは発生しません
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("existing");
            src.Expression.SetVariableAsDouble("existing", 99d);
            var cmd = CreateCmd(src, "Global existing");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            // 値が保持されていることを確認
            Assert.AreEqual(99d, src.Expression.GetValueAsDouble("existing"));
        }

        // ──────────────────────────────────────────────
        // ArrayCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayCmd_CommaSeparator_SplitsToArray()
        {
            // ヘルプ例: Array Var "ABC,DEF,GHI" "," → Var[1]="ABC", Var[2]="DEF", Var[3]="GHI"
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array Var \"ABC,DEF,GHI\" \",\"");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual("ABC", src.Expression.GetValueAsString("Var[1]"));
            Assert.AreEqual("DEF", src.Expression.GetValueAsString("Var[2]"));
            Assert.AreEqual("GHI", src.Expression.GetValueAsString("Var[3]"));
        }

        [TestMethod]
        public void ArrayCmd_ListMode_SplitsByListElements()
        {
            // ヘルプ: separatorに"リスト"を指定した場合、stringをリスト形式の変数として扱う
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array Var \"アリス ボブ キャロル\" リスト");
            cmd.Exec();

            Assert.AreEqual("アリス", src.Expression.GetValueAsString("Var[1]"));
            Assert.AreEqual("ボブ", src.Expression.GetValueAsString("Var[2]"));
            Assert.AreEqual("キャロル", src.Expression.GetValueAsString("Var[3]"));
        }

        [TestMethod]
        public void ArrayCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array Var \"data\"");
            var result = cmd.Exec();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ArrayCmd_NumericValues_StoredAsNumeric()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array nums \"10,20,30\" \",\"");
            cmd.Exec();

            Assert.AreEqual(10d, src.Expression.GetValueAsDouble("nums[1]"));
            Assert.AreEqual(20d, src.Expression.GetValueAsDouble("nums[2]"));
            Assert.AreEqual(30d, src.Expression.GetValueAsDouble("nums[3]"));
        }

        // ──────────────────────────────────────────────
        // CopyArrayCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CopyArrayCmd_CopiesArrayElements()
        {
            // ヘルプ: var1の内容をvar2にコピーします
            var src = CreateSrc();
            src.Expression.SetVariableAsString("a[1]", "one");
            src.Expression.SetVariableAsString("a[2]", "two");
            src.Expression.SetVariableAsDouble("a[3]", 3d);

            var cmd = CreateCmd(src, "CopyArray a tmp");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual("one", src.Expression.GetValueAsString("tmp[1]"));
            Assert.AreEqual("two", src.Expression.GetValueAsString("tmp[2]"));
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("tmp[3]"));
        }

        [TestMethod]
        public void CopyArrayCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CopyArray a");
            var result = cmd.Exec();

            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SwapCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SwapCmd_SwapsNumericValues()
        {
            // ヘルプ: 変数var1と変数var2の値を入れ替えます
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 10d);
            src.Expression.SetVariableAsDouble("y", 20d);

            var cmd = CreateCmd(src, "Swap x y");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(20d, src.Expression.GetValueAsDouble("x"));
            Assert.AreEqual(10d, src.Expression.GetValueAsDouble("y"));
        }

        [TestMethod]
        public void SwapCmd_SwapsStringValues()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("a", "hello");
            src.Expression.SetVariableAsString("b", "world");

            var cmd = CreateCmd(src, "Swap a b");
            cmd.Exec();

            Assert.AreEqual("world", src.Expression.GetValueAsString("a"));
            Assert.AreEqual("hello", src.Expression.GetValueAsString("b"));
        }

        [TestMethod]
        public void SwapCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Swap x");
            var result = cmd.Exec();

            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // UnSetCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnSetCmd_UndefinedVariable_AfterUnset()
        {
            // ヘルプ: 変数variableを消去します
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 42d);

            var cmd = CreateCmd(src, "UnSet x");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            // 消去後は0が返る（未定義変数は0として扱われる）
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("x", true));
        }

        [TestMethod]
        public void UnSetCmd_ArrayElement_RemovesSingleElement()
        {
            // ヘルプ: インデックスを付けて指定すれば配列の要素１つを消すことができます
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("arr[1]", 100d);
            src.Expression.SetVariableAsDouble("arr[2]", 200d);

            var cmd = CreateCmd(src, "UnSet arr[1]");
            cmd.Exec();

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("arr[1]", true));
            Assert.AreEqual(200d, src.Expression.GetValueAsDouble("arr[2]"));
        }
    }
}
