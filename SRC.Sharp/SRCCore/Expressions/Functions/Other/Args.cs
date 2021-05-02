using SRCCore.Lib;

namespace SRCCore.Expressions.Functions
{
    public class Args : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            var CallFunctionRet = ValueType.UndefinedType;
            str_result = "";
            num_result = 0d;

            // UpVarコマンドの呼び出し回数を累計
            var num = SRC.Event.UpVarLevel;
            var i = SRC.Event.CallDepth;
            while (num > 0)
            {
                i = (i - num);
                if (i < 1)
                {
                    i = 1;
                    break;
                }

                num = SRC.Event.UpVarLevelStack[i];
            }

            if (i < 1)
            {
                i = 1;
            }

            // 引数の範囲内に納まっているかチェック
            num = SRC.Expression.GetValueAsLong(@params[1], is_term[1]);
            var baseIndex = SRC.Event.ArgIndexStack[i - 1];
            if (baseIndex + num <= SRC.Event.ArgIndex)
            {
                str_result = SRC.Event.ArgStack[baseIndex + num];
            }

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
