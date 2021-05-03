using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class CopyArrayCmd : CmdData
    {
        public CopyArrayCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CopyArrayCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 3)
            {
                throw new EventErrorException(this, "CopyArrayコマンドの引数の数が違います");
            }

            // コピー元の変数名
            var name1 = GetArg(2);
            if (Strings.Left(name1, 1) == "$")
            {
                name1 = Strings.Mid(name1, 2);
            }
            // Eval関数
            if (Strings.LCase(Strings.Left(name1, 5)) == "eval(")
            {
                if (Strings.Right(name1, 1) == ")")
                {
                    name1 = Strings.Mid(name1, 6, Strings.Len(name1) - 6);
                    name1 = Expression.GetValueAsString(name1);
                }
            }

            // コピー先の変数名
            var name2 = GetArg(3);
            if (Strings.Left(name2, 1) == "$")
            {
                name1 = Strings.Mid(name2, 2);
            }
            // Eval関数
            if (Strings.LCase(Strings.Left(name2, 5)) == "eval(")
            {
                if (Strings.Right(name2, 1) == ")")
                {
                    name2 = Strings.Mid(name2, 6, Strings.Len(name2) - 6);
                    name2 = Expression.GetValueAsString(name2);
                }
            }

            // コピー先の変数を初期化
            // サブルーチンローカル変数の場合
            if (Expression.IsSubLocalVariableDefined(name2))
            {
                Expression.UndefineVariable(name2);
                Event.VarIndex = (Event.VarIndex + 1);
                {
                    var withBlock = Event.VarStack[Event.VarIndex];
                    withBlock.Name = name2;
                    withBlock.VariableType = Expressions.ValueType.StringType;
                    withBlock.StringValue = "";
                }
            }
            // ローカル変数の場合
            else if (Expression.IsLocalVariableDefined(name2))
            {
                Expression.UndefineVariable(name2);
                Expression.DefineLocalVariable(name2);
            }
            // グローバル変数の場合
            else if (Expression.IsGlobalVariableDefined(name2))
            {
                Expression.UndefineVariable(name2);
                Expression.DefineGlobalVariable(name2);
            }

            // 配列を検索し、配列要素を見つける
            var buf = "";
            if (Expression.IsSubLocalVariableDefined(name1))
            {
                // サブルーチンローカルな配列に対するCopyArray
                var loopTo = Event.VarIndex;
                for (var i = Event.VarIndexStack[Event.CallDepth - 1] + 1; i <= loopTo; i++)
                {
                    {
                        var withBlock1 = Event.VarStack[i];
                        if (Strings.InStr(withBlock1.Name, name1 + "[") == 1)
                        {
                            buf = name2 + Strings.Mid(withBlock1.Name, Strings.InStr(withBlock1.Name, "["));
                            Expression.SetVariable(buf, withBlock1.VariableType, withBlock1.StringValue, withBlock1.NumericValue);
                        }
                    }
                }

                if (string.IsNullOrEmpty(buf))
                {
                    var var = Expression.GetVariableObject(name1);
                    Expression.SetVariable(name2, var.VariableType, var.StringValue, var.NumericValue);
                }
            }
            else if (Expression.IsLocalVariableDefined(name1))
            {
                // ローカルな配列に対するCopyArray
                // XXX 列挙時の順番がDictionaryだと問題になるかも
                foreach (VarData currentVar in Event.LocalVariableList.Values)
                {
                    var var = currentVar;
                    {
                        var withBlock3 = var;
                        if (Strings.InStr(withBlock3.Name, name1 + "[") == 1)
                        {
                            buf = name2 + Strings.Mid(withBlock3.Name, Strings.InStr(withBlock3.Name, "["));
                            Expression.SetVariable(buf, withBlock3.VariableType, withBlock3.StringValue, withBlock3.NumericValue);
                        }
                    }
                }

                if (string.IsNullOrEmpty(buf))
                {
                    var var = Expression.GetVariableObject(name1);
                    Expression.SetVariable(name2, var.VariableType, var.StringValue, var.NumericValue);
                }
            }
            else if (Expression.IsGlobalVariableDefined(name1))
            {
                // グローバルな配列に対するCopyArray
                // XXX 列挙時の順番がDictionaryだと問題になるかも
                foreach (VarData currentVar1 in Event.GlobalVariableList.Values)
                {
                    var var = currentVar1;
                    {
                        var withBlock5 = var;
                        if (Strings.InStr(withBlock5.Name, name1 + "[") == 1)
                        {
                            buf = name2 + Strings.Mid(withBlock5.Name, Strings.InStr(withBlock5.Name, "["));
                            Expression.SetVariable(buf, withBlock5.VariableType, withBlock5.StringValue, withBlock5.NumericValue);
                        }
                    }
                }

                if (string.IsNullOrEmpty(buf))
                {
                    var var = Expression.GetVariableObject(name1);
                    {
                        var withBlock6 = var;
                        Expression.SetVariable(name2, withBlock6.VariableType, withBlock6.StringValue, withBlock6.NumericValue);
                    }
                }
            }

            return EventData.NextID;
        }
    }
}
