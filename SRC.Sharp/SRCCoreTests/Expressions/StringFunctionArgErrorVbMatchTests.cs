using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// 文字列の関数に範囲外の引数を渡したときの動作を、元の SRC（Expression.bas の CallFunction）と突き合わせたテスト。
    /// 元の SRC は VB の Mid$・Left$・Right$・String$・InStr・Asc・Chr$ などに引数をそのまま渡す。
    /// VB の関数は範囲外の引数で実行時エラー 5（プロシージャの呼び出し、または引数が不正です）を起こす。
    /// CallFunction はエラーを処理しないので、CmdData.cls の Exec の ErrorHandler が「イベントデータが不正です」を出して
    /// イベントを止める（Exec = -1）。これらの関数は VB のランタイムの関数なので、SRC.VBP の
    /// コンパイルの設定（範囲・あふれのチェックをしない）に左右されない。
    /// </summary>
    [TestClass]
    public class StringFunctionArgErrorVbMatchTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        private static CmdData[] BuildEvent(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            var cmds = new CmdData[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                cmds[i] = parser.Parse(src, line);
                src.Event.EventCmd.Add(cmds[i]);
            }
            return cmds;
        }

        // VB のエラー 5 になる引数（元の SRC の CallFunction がそのまま VB の関数に渡すもの）
        [TestMethod]
        [DataRow("Mid(\"abc\",0,1)")]       // Mid$ の開始位置が 0 以下
        [DataRow("Mid(\"abc\",-1,1)")]
        [DataRow("Mid(\"abc\",0)")]
        [DataRow("Mid(\"abc\",1,-1)")]      // Mid$ の文字数が負
        [DataRow("Left(\"abc\",-1)")]       // Left$ の文字数が負
        [DataRow("Right(\"abc\",-1)")]      // Right$ の文字数が負
        [DataRow("String(-1,\"a\")")]       // String$ の回数が負（文字列が 1 文字以下のとき String$ を呼ぶ）
        [DataRow("String(3,\"\")")]         // String$ に長さ 0 の文字列
        [DataRow("InStr(\"abc\",\"b\",0)")] // InStr の開始位置が 0 以下
        [DataRow("InStr(\"abc\",\"b\",-1)")]
        [DataRow("InStrRev(\"abcabc\",\"c\",0)")] // InStrRev は InStr(num - Len(buf2) + 1, ...) から始める
        [DataRow("Asc(\"\")")]              // Asc に長さ 0 の文字列
        [DataRow("Chr(70000)")]             // Chr$ の文字コードが DBCS の範囲（-32768〜65535）の外
        [DataRow("Chr(-32769)")]
        [DataRow("LeftB(\"abc\",-1)")]
        [DataRow("RightB(\"abc\",-1)")]
        [DataRow("MidB(\"abc\",0,1)")]
        [DataRow("MidB(\"abc\",1,-1)")]
        [DataRow("InStrB(\"abc\",\"b\",0)")]
        [DataRow("Replace(\"abcde\",\"c\",\"X\",0)")]   // Left$(buf, num - 1) の文字数が負
        [DataRow("Replace(\"abcde\",\"c\",\"X\",7)")]   // Right$(buf, Len(buf) - num + 1) の文字数が負
        [DataRow("Replace(\"abcde\",\"c\",\"X\",0,2)")] // Mid$(buf, num, num2) の開始位置が 0
        [DataRow("Replace(\"abcde\",\"c\",\"X\",3,3)")] // Right$(buf, Len(buf) - (num + num2 - 1) - 1) の文字数が負
        public void OutOfRangeArgument_IsVbError5(string expr)
        {
            var src = CreateSrc();
            var ex = Assert.Throws<VbRuntimeErrorException>(() => src.Expression.GetValueAsString(expr));
            Assert.AreEqual(5, ex.Number);
        }

        // VB でもエラーにならない引数（境界）
        [TestMethod]
        [DataRow("Mid(\"abc\",4,1)", "")]          // 開始位置が長さを越えるのは空文字列
        [DataRow("Mid(\"abc\",1,0)", "")]
        [DataRow("Mid(\"abc\",2)", "bc")]
        [DataRow("Left(\"abc\",0)", "")]
        [DataRow("Left(\"abc\",5)", "abc")]
        [DataRow("Right(\"abc\",0)", "")]
        [DataRow("String(0,\"a\")", "")]
        [DataRow("String(-1,\"ab\")", "")]          // 2 文字以上は For ループで繋ぐので、負の回数は空文字列
        [DataRow("InStr(\"abc\",\"b\",4)", "0")]
        [DataRow("InStr(\"abc\",\"b\",5)", "0")]
        [DataRow("InStrRev(\"abc\",\"c\",1)", "0")]  // i = 1 から 0 まで戻って抜ける
        [DataRow("InStrRev(\"ab\",\"abc\",0)", "0")] // Len(buf) < Len(buf2) は InStr を呼ばない
        [DataRow("LSet(\"abc\",-1)", "abc")]         // LSet・RSet は Space$ を呼ぶ前に長さを比べる
        [DataRow("RSet(\"abc\",-1)", "abc")]
        [DataRow("Replace(\"abcde\",\"c\",\"X\",6)", "abcde")]
        [DataRow("Replace(\"abcde\",\"c\",\"X\",1)", "abXde")]
        public void BoundaryArgument_IsNotError(string expr, string expected)
        {
            var src = CreateSrc();
            Assert.AreEqual(expected, src.Expression.GetValueAsString(expr));
        }

        [TestMethod]
        public void OutOfRangeArgument_InEvent_StopsWithInvalidEventData()
        {
            // 元の SRC: CallFunction のエラーは Exec の ErrorHandler に戻り、「イベントデータが不正です」を出して -1 を返す
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Set a Mid(\"abc\",0,1)");
            Assert.AreEqual(-1, cmds[0].Exec());
            CollectionAssert.Contains(src.DataErrors.Select(x => x.msg).ToList(), "イベントデータが不正です");
            Assert.IsFalse(src.Expression.IsVariableDefined("a"));
        }

        // `\` は Mod と同じく、割る前に両辺を Long に丸める。Long に収まらなければ VB ではエラー（あふれ）。
        // SRC# の Mod（VbMod）は以前からエラーにしていたので、`\` を同じにする
        [TestMethod]
        [DataRow("1e10 \\ 1")]
        [DataRow("2147483648 \\ 1")]
        [DataRow("5 \\ 1e10")]
        public void IntDiv_OperandOutOfLong_IsError(string expr)
        {
            var src = CreateSrc();
            Assert.Throws<OverflowException>(() => src.Expression.GetValueAsString(expr));
        }

        [TestMethod]
        [DataRow("2147483647 \\ 1", "2147483647")]
        [DataRow("7.5 \\ 2", "4")]   // 7.5 は偶数丸めで 8
        [DataRow("7 \\ 2.5", "3")]   // 2.5 は偶数丸めで 2
        [DataRow("-7 \\ 2", "-3")]
        [DataRow("5 \\ 0", "0")]     // Expression.bas は右辺が 0 なら 0
        public void IntDiv_RoundsOperandsToLong(string expr, string expected)
        {
            var src = CreateSrc();
            Assert.AreEqual(expected, src.Expression.GetValueAsString(expr));
        }
    }
}
