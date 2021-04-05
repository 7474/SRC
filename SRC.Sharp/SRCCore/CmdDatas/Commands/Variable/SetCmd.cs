using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class SetCmd : CmdData
    {
        public SetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int num = ArgNum;
            if (num > 3)
            {
                // 過去のバージョンのシナリオを読み込めるようにするため、
                // Setコマンドの後ろの「#」形式のコメントは無視する
                if (Strings.Left(GetArg(4), 1) == "#")
                {
                    num = 3;
                }
                else
                {
                    throw new EventErrorException(this, "Setコマンドの引数の数が違います");
                }
            }

            switch (num)
            {
                case 2:
                    Expression.SetVariableAsLong(GetArg(2), 1);
                    break;

                case 3:
                    switch (GetArgRaw(3).argType)
                    {
                        case ValueType.UndefinedType:
                            string str_result;
                            double num_result;
                            var etype = Expression.EvalTerm(GetArgRaw(3).strArg, ValueType.UndefinedType, out str_result, out num_result);
                            if (etype == ValueType.NumericType)
                            {
                                Expression.SetVariableAsDouble(GetArg(2), num_result);
                            }
                            else
                            {
                                Expression.SetVariableAsString(GetArg(2), str_result);
                            }
                            break;

                        case ValueType.StringType:
                            Expression.SetVariableAsString(GetArg(2), GetArgRaw(3).strArg);
                            break;

                        case ValueType.NumericType:
                            Expression.SetVariableAsDouble(GetArg(2), GetArgRaw(3).dblArg);
                            break;
                    }

                    break;
            }

            return EventData.ID + 1;
        }
    }
}
