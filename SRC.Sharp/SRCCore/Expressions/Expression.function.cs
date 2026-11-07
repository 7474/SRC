// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Events;
using SRCCore.Expressions.Functions;
using SRCCore.Lib;
using SRCCore.Logging;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        // === 関数に関する処理 ===

        private IDictionary<string, IFunction> systemFunctionMap = new IFunction[] {
            new Info(),
            // Unit
            new Action(),
            new Area(),
            new Condition(),
            new CountItem(),
            new CountPartner(),
            new CountPilot(),
            new Damage(),
            new EN(),
            new HP(),
            new IsAvailable(),
            new IsEquiped(),
            new Item(),
            new ItemID(),
            new Partner(),
            new Party(),
            new Pilot(),
            new PilotID(),
            new Rank(),
            new SpecialPower(),
            new Mind(),
            new Status(),
            new Functions.Unit(),
            new UnitID(),
            new X(),
            new Y(),
            new WX(),
            new WY(),
            // Pilot
            new Level(),
            new Morale(),
            new Plana(),
            new Relation(),
            new Skill(),
            new SP(),
            // Math
            new Abs(),
            new Atn(),
            new Cos(),
            new Int(),
            new Max(),
            new Min(),
            new Functions.Random(),
            new Round(),
            new RoundUp(),
            new RoundDown(),
            new Sin(),
            new Sqr(),
            new Tan(),
            // File
            new Dir(),
            new EOF(),
            new Loadfiledialog(),
            new Savefiledialog(),
            // String
            new Format(),
            new InStr(),
            new InStrRev(),
            new IsNumeric(),
            new Left(),
            new Len(),
            new LSet(),
            new Mid(),
            new Replace(),
            new Right(),
            new RSet(),
            new StrComp(),
            new String(),
            new Wide(),
            new InStrB(),
            new InStrRevB(),
            new LenB(),
            new LeftB(),
            new MidB(),
            new RightB(),
            new Asc(),
            new Chr(),
            new LCase(),
            new Trim(),
            // List
            new List(),
            new LLength(),
            new LIndex(),
            new LSearch(),
            // Paint
            new Font(),
            new RGB(),
            new TextHeight(),
            new TextWidth(),
            // Time
            new Year(),
            new Month(),
            new Weekday(),
            new Day(),
            new Hour(),
            new Minute(),
            new Second(),
            new DiffTime(),
            new GetTime(),
            // Regexp
            new RegExp(),
            new RegExpReplace(),
            // Other
            new Args(),
            new Call(),
            new Count(),
            new Eval(),
            new IIf(),
            new IsDefined(),
            new IsVarDefined(),
            new KeyState(),
            new Nickname(),
            new Term(),
            new WindowWidth(),
            new WindowHeight(),
        }.ToDictionary(x => x.Name.ToLower());

        // 式を関数呼び出しとして構文解析し、実行
        public ValueType CallFunction(string expr, ValueType etype, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            ValueType CallFunctionRet = default;
            string fname;
            int start_idx;
            int num, i, j, num2;
            string buf, buf2;
            double ldbl, rdbl;
            string pname2, pname, uname;
            int ret;
            int cur_depth;
            VarData var;
            int depth;
            bool in_single_quote, in_double_quote;
            var @params = new string[(Event.MaxArgIndex + 1)];
            int pcount;
            var is_term = new bool[(Event.MaxArgIndex + 1)];
            string dir_path;

            // 関数呼び出しの書式に合っているかチェック
            if (Strings.Right(expr, 1) != ")")
            {
                return ValueType.UndefinedType;
            }

            i = Strings.InStr(expr, " ");
            j = Strings.InStr(expr, "(");
            if (i > 0)
            {
                if (i < j)
                {
                    return ValueType.UndefinedType;
                }
            }
            else if (j == 0)
            {
                return ValueType.UndefinedType;
            }

            // ここまでくれば関数呼び出しと断定

            // パラメータの抽出
            pcount = 0;
            start_idx = (j + 1);
            depth = 0;
            in_single_quote = false;
            in_double_quote = false;
            num = Strings.Len(expr);
            int counter;
            counter = start_idx;
            var loopTo = (num - 1);
            for (i = counter; i <= loopTo; i++)
            {
                if (in_single_quote)
                {
                    if (Strings.Asc(Strings.Mid(expr, i, 1)) == 96) // `
                    {
                        in_single_quote = false;
                    }
                }
                else if (in_double_quote)
                {
                    if (Strings.Asc(Strings.Mid(expr, i, 1)) == 34) // "
                    {
                        in_double_quote = false;
                    }
                }
                else
                {
                    switch (Strings.Asc(Strings.Mid(expr, i, 1)))
                    {
                        case 9:
                        case 32: // タブ, 空白
                            {
                                if (start_idx == i)
                                {
                                    start_idx = (i + 1);
                                }
                                else
                                {
                                    is_term[pcount + 1] = false;
                                }

                                break;
                            }

                        case 40:
                        case 91: // (, [
                            {
                                depth = (depth + 1);
                                break;
                            }

                        case 41:
                        case 93: // ), ]
                            {
                                depth = (depth - 1);
                                break;
                            }

                        case 44: // ,
                            {
                                if (depth == 0)
                                {
                                    pcount = (pcount + 1);
                                    @params[pcount] = Strings.Mid(expr, start_idx, i - start_idx);
                                    start_idx = (i + 1);
                                    is_term[pcount + 1] = true;
                                }

                                break;
                            }

                        case 96: // `
                            {
                                in_single_quote = true;
                                break;
                            }

                        case 34: // "
                            {
                                in_double_quote = true;
                                break;
                            }
                    }
                }
            }

            if (num > start_idx)
            {
                pcount = (pcount + 1);
                @params[pcount] = Strings.Mid(expr, start_idx, num - start_idx);
            }

            var isUserFunc = false;
            // 先頭の文字で関数の種類を判断する
            switch (Strings.Asc(expr))
            {
                case 95: // _
                    {
                        // 必ずユーザー定義関数
                        fname = Strings.Left(expr, j - 1);
                        isUserFunc = true;
                        break;
                    }

                case var @case when 65 <= @case && @case <= 90:
                case var case1 when 97 <= case1 && case1 <= 122: // A To z
                    {
                        // システム関数の可能性あり
                        fname = Strings.Left(expr, j - 1);
                        break;
                    }

                default:
                    {
                        // 先頭がアルファベットでなければ必ずユーザー定義関数
                        // ただし括弧を含むユニット名等である場合があるため、チェックが必要
                        if (SRC.UDList.IsDefined(expr))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        if (SRC.PDList.IsDefined(expr))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        if (SRC.NPDList.IsDefined(expr))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        if (SRC.IDList.IsDefined(expr))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        fname = Strings.Left(expr, j - 1);
                        isUserFunc = true;
                        break;
                    }
            }

            // システム関数？
            if (!isUserFunc)
            {
                IFunction systemFunc;
                if (systemFunctionMap.TryGetValue(fname.ToLower(), out systemFunc))
                {
                    // 元の SRC: システム関数の Case はどれも Exit Function で抜ける。
                    // UndefinedType を返したときも、ユーザー定義関数は探さずにそのまま返す（EvalTerm が GetVariable に回す）
                    return systemFunc.Invoke(SRC, etype, @params, pcount, is_term, out str_result, out num_result);
                }
            }

            // ユーザー定義関数？
            ret = Event.FindNormalLabel(fname);
            if (ret >= 0)
            {
                return CallUserFunction(fname, etype, ret, @params, pcount, is_term, out str_result, out num_result);
            }

            // 実はシステム定義のグローバル変数？
            if (IsGlobalVariableDefined(expr))
            {
                return Event.GlobalVariableList[expr].ReferenceValue(etype, out str_result, out num_result);
            }

            // 結局ただの文字列……
            str_result = expr;
            return ValueType.StringType;
        }

        private ValueType CallUserFunction(string fname, ValueType etype, int labelId, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            try
            {
                // 関数が見つかった
                var ret = labelId + 1;

                // 呼び出し階層をチェック
                // 元の SRC と同じく、呼び出し階層が MaxCallDepth を越えたときだけ止める（Call コマンドと同じ）
                if (Event.CallDepth > Event.MaxCallDepth)
                {
                    Event.CallDepth = Event.MaxCallDepth;
                    Event.DisplayEventErrorMessage(Event.CurrentLineNum, GeneralLib.FormatNum(Event.MaxCallDepth) + "階層を越えるサブルーチンの呼び出しは出来ません");
                    return ValueType.UndefinedType;
                }

                // 引数用スタックが溢れないかチェック
                if ((Event.ArgIndex + pcount) > Event.MaxArgIndex)
                {
                    Event.DisplayEventErrorMessage(Event.CurrentLineNum, "サブルーチンの引数の総数が" + GeneralLib.FormatNum(Event.MaxArgIndex) + "個を超えています");
                    return ValueType.UndefinedType;
                }

                // 引数の値を先に求めておく
                // (スタックに積みながら計算すると、引数での関数呼び出しで不正になる)
                var loopTo14 = pcount;
                for (var i = 1; i <= loopTo14; i++)
                    @params[i] = GetValueAsString(@params[i], is_term[i]);

                // 現在の状態を保存
                Event.CallStack[Event.CallDepth] = Event.CurrentLineNum;
                Event.ArgIndexStack[Event.CallDepth] = Event.ArgIndex;
                Event.VarIndexStack[Event.CallDepth] = Event.VarIndex;
                Event.ForIndexStack[Event.CallDepth] = Event.ForIndex;
                // 元の SRC のユーザー定義関数は、Call コマンドと違って UpVar の段数を累計せずにそのまま積む
                // （戻ったあとの呼び出し元の UpVar の段数は呼ぶ前と同じ）
                Event.UpVarLevelStack[Event.CallDepth] = Event.UpVarLevel;

                // UpVarの階層数を初期化
                Event.UpVarLevel = 0;

                // 呼び出し階層数をインクリメント
                Event.CallDepth = (Event.CallDepth + 1);
                var cur_depth = Event.CallDepth;

                // 引数をスタックに積む
                SRC.Event.ArgIndex = (SRC.Event.ArgIndex + pcount);
                for (var i = 1; i <= pcount; i++)
                {
                    SRC.Event.ArgStack[SRC.Event.ArgIndex - i + 1] = @params[i];
                }

                // サブルーチン本体を実行
                do
                {
                    Event.CurrentLineNum = ret;
                    if (Event.CurrentLineNum >= Event.EventCmd.Count)
                    {
                        break;
                    }

                    {
                        var cmd = Event.EventCmd[Event.CurrentLineNum];
                        if (cur_depth == Event.CallDepth && cmd.Name == CmdType.ReturnCmd)
                        {
                            break;
                        }

                        ret = cmd.Exec();
                    }
                }
                while (ret >= 0);

                // 返り値
                {
                    var withBlock50 = Event.EventCmd[Event.CurrentLineNum];
                    if (withBlock50.ArgNum > 1)
                    {
                        str_result = withBlock50.GetArgAsString(2);
                    }
                    else
                    {
                        str_result = "";
                    }
                }

                // 呼び出し階層数をデクリメント
                Event.CallDepth = (Event.CallDepth - 1);

                // サブルーチン実行前の状態に復帰
                Event.CurrentLineNum = Event.CallStack[Event.CallDepth];
                Event.ArgIndex = Event.ArgIndexStack[Event.CallDepth];
                Event.VarIndex = Event.VarIndexStack[Event.CallDepth];
                Event.ForIndex = Event.ForIndexStack[Event.CallDepth];
                Event.UpVarLevel = Event.UpVarLevelStack[Event.CallDepth];
                if (etype == ValueType.NumericType)
                {
                    num_result = GeneralLib.StrToDbl(str_result);
                    return ValueType.NumericType;
                }
                else
                {
                    return ValueType.StringType;
                }
            }
            finally
            {
                SRC.ExpressionLog.TraceWithCaller("Called",
                    etype.ToString().Substring(0, 1),
                    fname + "(" + string.Join(",", Enumerable.Range(1, pcount).Select(x => @params[x])) + ")",
                    str_result,
                    num_result.ToString()
                    );
            }
        }
    }
}
