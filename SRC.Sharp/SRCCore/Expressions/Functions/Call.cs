using SRCCore.Events;
using SRCCore.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Expressions.Functions
{
    public class Call : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            var CallFunctionRet = ValueType.UndefinedType;
            str_result = "";
            num_result = 0d;

            // サブルーチンの場所は？
            // まずはサブルーチン名が式でないと仮定して検索
            var ret = SRC.Event.FindNormalLabel(@params[1]);
            if (ret < 0)
            {
                // 式で指定されている？
                ret = SRC.Event.FindNormalLabel(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
                if (ret < 0)
                {
                    SRC.Event.DisplayEventErrorMessage(SRC.Event.CurrentLineNum, "指定されたサブルーチン「" + @params[1] + "」が見つかりません");
                    return CallFunctionRet;
                }
            }

            ret = ret + 1;

            // 呼び出し階層をチェック
            if (SRC.Event.CallDepth >= Event.MaxCallDepth)
            {
                SRC.Event.CallDepth = Event.MaxCallDepth;
                SRC.Event.DisplayEventErrorMessage(SRC.Event.CurrentLineNum, Event.MaxCallDepth + "階層を越えるサブルーチンの呼び出しは出来ません");
                return CallFunctionRet;
            }

            // 引数用スタックが溢れないかチェック
            if ((SRC.Event.ArgIndex + pcount) >= Event.MaxArgIndex)
            {
                SRC.Event.DisplayEventErrorMessage(SRC.Event.CurrentLineNum, "サブルーチンの引数の総数が" + Event.MaxArgIndex + "個を超えています");
                return CallFunctionRet;
            }

            // 引数を評価しておく
            for (var i = 2; i <= pcount; i++)
                @params[i] = SRC.Expression.GetValueAsString(@params[i], is_term[i]);

            // 現在の状態を保存
            SRC.Event.CallStack[SRC.Event.CallDepth] = SRC.Event.CurrentLineNum;
            SRC.Event.ArgIndexStack[SRC.Event.CallDepth] = SRC.Event.ArgIndex;
            SRC.Event.VarIndexStack[SRC.Event.CallDepth] = SRC.Event.VarIndex;
            SRC.Event.ForIndexStack[SRC.Event.CallDepth] = SRC.Event.ForIndex;

            // UpVarが実行された場合、UpVar実行数は累計する
            if (SRC.Event.UpVarLevel > 0)
            {
                SRC.Event.UpVarLevelStack[SRC.Event.CallDepth] = (SRC.Event.UpVarLevel + SRC.Event.UpVarLevelStack[SRC.Event.CallDepth - 1]);
            }
            else
            {
                SRC.Event.UpVarLevelStack[SRC.Event.CallDepth] = 0;
            }

            // UpVarの階層数を初期化
            SRC.Event.UpVarLevel = 0;

            // 呼び出し階層数をインクリメント
            SRC.Event.CallDepth = (SRC.Event.CallDepth + 1);
            var cur_depth = SRC.Event.CallDepth;

            // 引数をスタックに積む
            for (var i = 1; i <= pcount; i++)
            {
                SRC.Event.ArgStack[SRC.Event.ArgIndex + i] = @params[i];
            }
            SRC.Event.ArgIndex = (SRC.Event.ArgIndex + pcount);

            // サブルーチン本体を実行
            do
            {
                SRC.Event.CurrentLineNum = ret;
                if (SRC.Event.CurrentLineNum >= SRC.Event.EventCmd.Count)
                {
                    break;
                }

                var cmd = SRC.Event.EventCmd[SRC.Event.CurrentLineNum];
                if (cur_depth == SRC.Event.CallDepth & cmd.Name == Events.CmdType.ReturnCmd)
                {
                    break;
                }

                ret = cmd.Exec();
            }
            while (ret >= 0);

            // 返り値
            {
                var withBlock1 = SRC.Event.EventCmd[SRC.Event.CurrentLineNum];
                if (withBlock1.ArgNum == 2)
                {
                    str_result = withBlock1.GetArgAsString(2);
                }
                else
                {
                    str_result = "";
                }
            }

            // 呼び出し階層数をデクリメント
            SRC.Event.CallDepth = (SRC.Event.CallDepth - 1);

            // サブルーチン実行前の状態に復帰
            SRC.Event.CurrentLineNum = SRC.Event.CallStack[SRC.Event.CallDepth];
            SRC.Event.ArgIndex = SRC.Event.ArgIndexStack[SRC.Event.CallDepth];
            SRC.Event.VarIndex = SRC.Event.VarIndexStack[SRC.Event.CallDepth];
            SRC.Event.ForIndex = SRC.Event.ForIndexStack[SRC.Event.CallDepth];
            SRC.Event.UpVarLevel = SRC.Event.UpVarLevelStack[SRC.Event.CallDepth];
            if (etype == ValueType.NumericType)
            {
                num_result = GeneralLib.StrToDbl(str_result);
                CallFunctionRet = ValueType.NumericType;
            }
            else
            {
                CallFunctionRet = ValueType.StringType;
            }

            return CallFunctionRet;
        }
    }
}
