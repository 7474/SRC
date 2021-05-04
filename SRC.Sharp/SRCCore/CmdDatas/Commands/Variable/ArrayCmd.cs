using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class ArrayCmd : CmdData
    {
        public ArrayCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ArrayCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            bool IsList;
            if (ArgNum != 4)
            {
                throw new EventErrorException(this, "Arrayコマンドの引数の数が違います");
            }
            else if (GetArgAsString(4) == "リスト")
            {
                IsList = true;
            }
            else
            {
                IsList = false;
            }

            // 代入先の変数名
            var var_name = GetArg(2);
            if (Strings.Left(var_name, 1) == "$")
            {
                var_name = Strings.Mid(var_name, 2);
            }
            // Eval関数
            if (Strings.LCase(Strings.Left(var_name, 5)) == "eval(")
            {
                if (Strings.Right(var_name, 1) == ")")
                {
                    var_name = Strings.Mid(var_name, 6, Strings.Len(var_name) - 6);
                    var_name = Expression.GetValueAsString(var_name);
                }
            }

            // 代入先の変数を初期化した上で再設定
            if (Expression.IsSubLocalVariableDefined(var_name))
            {
                // サブルーチンローカル変数の場合
                Expression.UndefineVariable(var_name);
                Event.VarIndex = (Event.VarIndex + 1);
                Event.VarStack[Event.VarIndex].SetValue(var_name, ValueType.UndefinedType, "", 0d);
            }
            else if (Expression.IsLocalVariableDefined(var_name))
            {
                // ローカル変数の場合
                Expression.UndefineVariable(var_name);
                Expression.DefineLocalVariable(var_name);
            }
            else if (Expression.IsGlobalVariableDefined(var_name))
            {
                // グローバル変数の場合
                Expression.UndefineVariable(var_name);
                Expression.DefineGlobalVariable(var_name);
            }

            string[] array_buf;
            if (IsList)
            {
                // リストを配列に変換
                GeneralLib.ListSplit(GetArgAsString(3), out array_buf);
            }
            else
            {
                // 文字列を分割して配列に代入
                var buf = GetArgAsString(3);
                var sep = GetArgAsString(4);
                array_buf = buf.Split(sep);
            }

            for (var i = 1; i <= array_buf.Length; i++)
            {
                var buf = array_buf[i - 1].Trim();
                Expressions.ValueType etype;
                string str_value;
                double num_value;
                if (Information.IsNumeric(buf))
                {
                    etype = Expressions.ValueType.NumericType;
                    str_value = "";
                    num_value = GeneralLib.StrToDbl(buf);
                }
                else
                {
                    etype = Expressions.ValueType.StringType;
                    str_value = buf;
                    num_value = 0d;
                }

                var vname = var_name + "[" + i + "]";
                Expression.SetVariable(vname, etype, str_value, num_value);
            }

            return EventData.NextID;
        }
    }
}
