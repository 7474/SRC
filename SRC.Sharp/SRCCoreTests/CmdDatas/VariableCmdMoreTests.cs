using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// 変数操作コマンドの追加ユニットテスト（VariableCmdTests.cs に未収録のケース）
    /// </summary>
    [TestClass]
    public class VariableCmdMoreTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
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
        // SetCmd - コメント付きサフィックス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetCmd_WithCommentSuffix_IgnoresComment()
        {
            // SetCmd ソース: 4番目引数が "#" で始まる場合は ArgNum を 3 に丸めてコメントを無視する
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Set x 100 # これはコメント");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(100d, src.Expression.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void SetCmd_WithFloatValue_StoresFloat()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Set ratio 1.5");
            cmd.Exec();

            Assert.AreEqual(1.5d, src.Expression.GetValueAsDouble("ratio"), 1e-10);
        }

        [TestMethod]
        public void SetCmd_WithVerySmallFloat_StoresCorrectly()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Set tiny 0.001");
            cmd.Exec();

            Assert.AreEqual(0.001d, src.Expression.GetValueAsDouble("tiny"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // IncrCmd - 小数点インクリメント・ゼロインクリメント
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IncrCmd_WithFloatIncrement_AddsFloat()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1.0d);
            var cmd = CreateCmd(src, "Incr x 0.5");
            cmd.Exec();

            Assert.AreEqual(1.5d, src.Expression.GetValueAsDouble("x"), 1e-10);
        }

        [TestMethod]
        public void IncrCmd_WithZeroIncrement_ValueUnchanged()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 42d);
            var cmd = CreateCmd(src, "Incr x 0");
            cmd.Exec();

            Assert.AreEqual(42d, src.Expression.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void IncrCmd_WithFractionalDecrement_SubtractsCorrectly()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("v", 3.0d);
            var cmd = CreateCmd(src, "Incr v -1.5");
            cmd.Exec();

            Assert.AreEqual(1.5d, src.Expression.GetValueAsDouble("v"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // GlobalCmd - $ プレフィックス変数名
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GlobalCmd_WithDollarPrefix_DefinesVariableWithoutPrefix()
        {
            // GlobalCmd ソース: Asc(vname) == 36 ($) の場合、先頭の $ を除去して定義する
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global $myGlobalVar");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("myGlobalVar"),
                "$ プレフィックスを除いた変数名が定義されているべき");
        }

        [TestMethod]
        public void GlobalCmd_WithDollarPrefix_InitiallyZero()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global $counter");
            cmd.Exec();

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("counter"));
        }

        // ──────────────────────────────────────────────
        // ArrayCmd - 混在型（数値と文字列）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayCmd_MixedTypes_StoresCorrectly()
        {
            // "1,hello,3.5" を "," で分割 → 数値と文字列が混在
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array v \"1,hello,3.5\" \",\"");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("v[1]"));
            Assert.AreEqual("hello", src.Expression.GetValueAsString("v[2]"));
            Assert.AreEqual(3.5d, src.Expression.GetValueAsDouble("v[3]"), 1e-10);
        }

        [TestMethod]
        public void ArrayCmd_EmptyElements_StoredAsEmptyString()
        {
            // "a,,c" のように空要素がある場合
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array v \"a,,c\" \",\"");
            cmd.Exec();

            Assert.AreEqual("a", src.Expression.GetValueAsString("v[1]"));
            Assert.AreEqual("c", src.Expression.GetValueAsString("v[3]"));
        }

        // ──────────────────────────────────────────────
        // UnSetCmd - 存在しない変数に対して実行
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnSetCmd_ForNonExistentVariable_ReturnsSuccess()
        {
            // 未定義変数に対して UnSet してもエラーにならない
            var src = CreateSrc();
            var cmd = CreateCmd(src, "UnSet doesNotExist");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void UnSetCmd_GlobalVariable_BecomesUndefined()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("g");
            src.Expression.SetVariableAsDouble("g", 100d);
            var cmd = CreateCmd(src, "UnSet g");
            cmd.Exec();

            // グローバル変数削除後は IsGlobalVariableDefined が false になる
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("g"));
        }

        // ──────────────────────────────────────────────
        // SwapCmd - 異なる型（数値と文字列）の入れ替え
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SwapCmd_NumericAndStringVars_SwapsCorrectly()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("num", 42d);
            src.Expression.SetVariableAsString("str", "hello");

            var cmd = CreateCmd(src, "Swap num str");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            // num には "hello" が入り、str には 42 が入る
            Assert.AreEqual("hello", src.Expression.GetValueAsString("num"));
            Assert.AreEqual(42d, src.Expression.GetValueAsDouble("str"));
        }

        // ──────────────────────────────────────────────
        // SortCmd - コマンドパーサー経由での昇順・降順ソート
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SortCmd_StringAscending_ViaParser()
        {
            // CmdParser 経由で Sort コマンドを実行（文字列昇順）
            var src = CreateSrc();
            src.Expression.SetVariableAsString("names[1]", "Zeta");
            src.Expression.SetVariableAsString("names[2]", "Alpha");
            src.Expression.SetVariableAsString("names[3]", "Mu");

            var cmd = CreateCmd(src, "Sort names 昇順 文字");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual("Alpha", src.Expression.GetValueAsString("names[1]"));
            Assert.AreEqual("Mu", src.Expression.GetValueAsString("names[2]"));
            Assert.AreEqual("Zeta", src.Expression.GetValueAsString("names[3]"));
        }

        [TestMethod]
        public void SortCmd_NumericDefault_SortsAscending()
        {
            // Sort コマンドは既定で昇順数値ソート
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("ns[1]", 50d);
            src.Expression.SetVariableAsDouble("ns[2]", 10d);
            src.Expression.SetVariableAsDouble("ns[3]", 30d);

            var cmd = CreateCmd(src, "Sort ns 昇順");
            cmd.Exec();

            Assert.AreEqual(10d, src.Expression.GetValueAsDouble("ns[1]"));
            Assert.AreEqual(30d, src.Expression.GetValueAsDouble("ns[2]"));
            Assert.AreEqual(50d, src.Expression.GetValueAsDouble("ns[3]"));
        }

        // ──────────────────────────────────────────────
        // CopyArrayCmd - コピー先に既存要素がある場合
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CopyArrayCmd_DestinationHasExistingElements_IsOverwritten()
        {
            // コピー先の既存要素はコピー後にソース由来の値に置き換わる
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("src_arr");
            src.Expression.SetVariableAsString("src_arr[1]", "A");
            src.Expression.SetVariableAsString("src_arr[2]", "B");

            src.Expression.DefineGlobalVariable("dst_arr");
            src.Expression.SetVariableAsString("dst_arr[1]", "old1");
            src.Expression.SetVariableAsString("dst_arr[2]", "old2");

            var cmd = CreateCmd(src, "CopyArray src_arr dst_arr");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual("A", src.Expression.GetValueAsString("dst_arr[1]"));
            Assert.AreEqual("B", src.Expression.GetValueAsString("dst_arr[2]"));
        }

        [TestMethod]
        public void CopyArrayCmd_NumericArray_CopiedCorrectly()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("orig");
            src.Expression.SetVariableAsDouble("orig[1]", 10d);
            src.Expression.SetVariableAsDouble("orig[2]", 20d);
            src.Expression.SetVariableAsDouble("orig[3]", 30d);

            var cmd = CreateCmd(src, "CopyArray orig copy");
            cmd.Exec();

            Assert.AreEqual(10d, src.Expression.GetValueAsDouble("copy[1]"));
            Assert.AreEqual(20d, src.Expression.GetValueAsDouble("copy[2]"));
            Assert.AreEqual(30d, src.Expression.GetValueAsDouble("copy[3]"));
        }
    }
}
